using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Linq;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Authentication
{
    public static class AzureAdAuthenticationBuilderExtensions
    {
        public static AuthenticationBuilder AddAzureAd(this AuthenticationBuilder builder)
            => builder.AddAzureAd(_ => { });

        public static AuthenticationBuilder AddAzureAd(this AuthenticationBuilder builder, Action<AzureAdOptions> configureOptions)
        {
            builder.Services.Configure(configureOptions);
            builder.Services.AddSingleton<IConfigureOptions<OpenIdConnectOptions>, ConfigureAzureOptions>();
            builder.AddOpenIdConnect();
            return builder;
        }

        private class ConfigureAzureOptions : IConfigureNamedOptions<OpenIdConnectOptions>
        {
            private readonly AzureAdOptions _azureOptions;

            public ConfigureAzureOptions(IOptions<AzureAdOptions> azureOptions)
            {
                _azureOptions = azureOptions.Value;
            }

            public void Configure(string name, OpenIdConnectOptions options)
            {
                options.ClientId = _azureOptions.ClientId;
                options.Authority = $"{_azureOptions.Instance}{_azureOptions.TenantId}";
                options.UseTokenLifetime = true;
                options.CallbackPath = _azureOptions.CallbackPath;
                options.RequireHttpsMetadata = false;
                // This can be used in combination with OnUserInformationReceived event below
                // options.GetClaimsFromUserInfoEndpoint = true;

                options.Events = new OpenIdConnectEvents()
                {
                    // Debug output for validated tokens, and ensure that the domain matches the one 
                    // configured in settings. This is custom and added on top of the ASP.NET template.
                    OnTokenValidated = context =>
                    {
                        var logger = Web.Startup.LoggerFactory.CreateLogger("OnTokenValidated");
                        var upn = "";

                        // Debug print claim data. All of these are copied to Principal and can be accessed from there on subsequent requests
                        foreach (var claim in context.SecurityToken.Claims)
                        {
                            logger.LogInformation("Claim {0}:{1}", claim.Type, claim.Value);

                            if (claim.Type.Equals("upn"))
                            {
                                upn = claim.Value;
                            }
                        }

                        if (!upn.EndsWith("@" + _azureOptions.Domain))
                        {
                            logger.LogError("Invalid authentication domain, only " + _azureOptions.Domain + " is allowed.");
                            
                            context.HandleResponse();
                            context.Principal = null;
                            context.Response.Redirect("/account/signout?callbackUrl=/account/accessdenied");
                        }

                        return Task.FromResult(0);
                    }
                };
            }

            public void Configure(OpenIdConnectOptions options)
            {
                Configure(Options.DefaultName, options);
            }
        }
    }
}
