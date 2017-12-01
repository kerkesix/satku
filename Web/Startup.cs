using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.ApplicationInsights;
using System.Diagnostics;
using KsxEventTracker.Domain.Repositories;
using KsxEventTracker.Domain.Messages;
using KsxEventTracker.Domain.Messages.Handlers;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Web.Hubs;
using Web.Logic;
using Web.QueryModels;
using Web.Models;
using Microsoft.AspNetCore.Rewrite;

namespace Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            // Save static copies for some ugly usage from elsewhere
            Configuration = configuration;
            LoggerFactory = loggerFactory;
        }

        public IConfiguration Configuration { get; private set; }
        public static ILoggerFactory LoggerFactory { get; private set; }
        public static IHubContext<CommandBus> CommandBus { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(sharedOptions =>
            {
                sharedOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                sharedOptions.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddAzureAd(options => Configuration.Bind("AzureAd", options))
            .AddCookie();

            services.AddMvc();

            // Configurations
            services.AddOptions();
            services.Configure<AzureTableStorageOptions>(Configuration.GetSection("storage"));
            services.Configure<AnalyticsOptions>(Configuration.GetSection("analytics"));

            // Allow reading HTTP Context from views (required in main layout)
            // services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // Own BL services
            var eventTopic = new InMemoryEventTopic(LoggerFactory);

            services.AddSingleton(new DenormalizerDispatcher());
            services.AddSingleton<IEventTopic>(eventTopic);
            services.AddSingleton<IEventPublisher>(eventTopic);

            services.AddScoped<RegistrationRepository>(sp => {
                var opts = sp.GetService<IOptions<AzureTableStorageOptions>>();
                return new RegistrationRepository(opts.Value);
            });
            services.AddScoped<CommandRepository>(sp => {
                var opts = sp.GetService<IOptions<AzureTableStorageOptions>>();
                return new CommandRepository(opts.Value);
            });
            
            services.AddScoped<ITimeConverter, TimeConverter>();

            services.AddTransient<IAverageCheckpointTimeCalculator, AverageCheckpointTimeCalculator>();
            services.AddTransient<IAttendeeSpeedCalculator, AttendeeSpeedCalculator>();
            services.AddTransient<CommandHandlerFactory>(sp => {
                var opts = sp.GetService<IOptions<AzureTableStorageOptions>>();
                var ep = sp.GetService<IEventPublisher>();
                return new CommandHandlerFactory(ep, opts.Value);
            });
            

            // Use current HTTP context as security context when filtering sensitive information
            services.AddScoped<IAttendeeDataFilter>(sp => new AttendeeDataFilter(ClaimsPrincipal.Current));

            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // Since this method is resolved through the DI already, we can request any registered services by
        // just adding respective parameters.
        public void Configure(
            IApplicationBuilder app, 
            IHostingEnvironment env, 
            ILoggerFactory loggerFactory,
            IEventTopic eventTopic,
            DenormalizerDispatcher dispatcher,
            CommandHandlerFactory commandHandlerFactory,
            IOptions<AzureTableStorageOptions> tableStorageOptions,
            IHubContext<CommandBus> commandBus)
        {
            Startup.CommandBus = commandBus;

            var configureLogger = loggerFactory.CreateLogger("Configure");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseRewriter(new RewriteOptions().AddRedirectToHttps());                
            }

            app.UseStaticFiles();
            app.UseAuthentication();

            // Replay all events once from DB to the in-memory query model. 
            // Explicitly wait as routing (below) and controllers expect the state is loaded .
            ReplayEvents(loggerFactory, dispatcher, tableStorageOptions.Value).Wait();

            // Register all denormalizers to listen for all events. Could have more fine grained subscription logic, but this works for now.
            foreach (var d in dispatcher.Denormalizers.Cast<IEventConsumer>())
            {
                configureLogger.LogInformation("Registering denormalizer {0} to receive all events", d.GetType());
                eventTopic.Subscribe(d);
            }

            // Register all command handlers to listen for all events.
            foreach (var d in commandHandlerFactory.All.Cast<IEventConsumer>())
            {
                configureLogger.LogInformation("Registering command handler {0} to receive all events", d.GetType());
                eventTopic.Subscribe(d);
            }

            // Routes use event data (state after events), therefore register routes only after the replay
            string defaultHappening = QueryModelRepository.Routing.DefaultHappening;
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "root",
                    template: "{happening:regex(^satku[a-z]+$)?}",
                    defaults: new { happening = defaultHappening, Controller = "Home", Action = "Index"},
                    constraints: null,
                    dataTokens: new { DefaultHappening = defaultHappening });

                routes.MapRoute(
                    name: "default",
                    template: "{happening:regex(^satku[a-z]+$)}/{*catchall}",
                    defaults: new { happening = defaultHappening, Controller = "Home", Action = "Index"},
                    constraints: null,
                    dataTokens: new { DefaultHappening = defaultHappening });       
            });

            app.UseSignalR(routes =>
            {
                routes.MapHub<CommandBus>("commandbus");
            });
        }

        private async Task ReplayEvents(ILoggerFactory loggerFactory, DenormalizerDispatcher dispatcher,
            AzureTableStorageOptions dboptions)
        {
            var logger = loggerFactory.CreateLogger("Replay");

            logger.LogInformation("Replaying all the events from event store to denormalizers.");

            var tc = new TelemetryClient();
            var watch = Stopwatch.StartNew();


            var eventStore = new EventStream(dboptions);

            var allEvents = await eventStore.ReadAllEvents();

            watch.Stop();
            tc.TrackMetric("LoadEvents", watch.ElapsedMilliseconds);
            Trace.TraceInformation("Events queried from event store in {0:n0} ms.", watch.ElapsedMilliseconds);

            watch = Stopwatch.StartNew();

            var i = 0;
            foreach (var e in allEvents)
            {
                dispatcher.ReplayEvent(e);
                i++;
            }

            watch.Stop();
            logger.LogInformation("Replay complete in {0:n0} ms. Replayed {1} events.", watch.ElapsedMilliseconds, i);
            tc.TrackMetric("ReplayEvents", watch.ElapsedMilliseconds);
            tc.TrackMetric("ReplayEventsCount", i);
        }
    }
}
