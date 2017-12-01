using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using KsxEventTracker.Registration.Models;
using KsxEventTracker.Domain.Repositories;
using Microsoft.Extensions.Options;
using System;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.Mvc;

namespace KsxEventTracker.Registration
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();

            services.AddOptions();

            services.Configure<AzureTableStorageOptions>(Configuration.GetSection("storage"));
            services.Configure<RegistrationOptions>(Configuration.GetSection("registration"));

            services.AddScoped<IAuthenticateRegistration, RegistrationAccess>();

            services.AddSingleton(c => new RegistrationRepository(c.GetService<IOptions<AzureTableStorageOptions>>().Value));
            services.AddSingleton<RegistrationOrchestrator>();

            // Enforce HTTPS when local development is done via HTTPS https://docs.microsoft.com/en-us/aspnet/core/security/https
            // services.Configure<MvcOptions>(options =>
            // {
            //     options.Filters.Add(new RequireHttpsAttribute());
            // });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");

                // Redirect to HTTP to HTTPS. 
                var options = new RewriteOptions().AddRedirectToHttps();
                app.UseRewriter(options);
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{happening}/{controller=Registration}/{action=Register}/{id?}");
            });
        }
    }

    public class RegistrationOptions
    {
        public RegistrationHappening Happening { get; set; }
        public EmailOptions Email { get; set; }
    }

    public class RegistrationHappening
    {
        public string Id { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public int MaxAttendees { get; set; }
        public string OverrideSecret { get; set; }
    }

    public class EmailOptions
    {
        public string APIKey { get; set; }

        public string FromAddress { get; set; }
        public string FromDisplayName { get; set; }
        public string ConfirmationSubject { get; set; }
        public string ConfirmationTemplate { get; set; }
        public string CompletedSubject { get; set; }

        public string CompletedTemplate { get; set; }
    }
}
