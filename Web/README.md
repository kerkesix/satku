# Kerkesix Sysimusta Satku Tracking Web App

## Stuff that should be done

- Use Webpack for Typescript transpiling and JS bundling to be able to use proper JS imports everywhere
- Upgrade to Bootstrap 4 final (hidden attribute changes, card-block/card-body, others)

## Lost in conversion to ASP.NET CORE

### Eventual consistency and reliability through Azure ServiceBus topics

### Trace filters and Event Tracing for Windows

The web app used to write way more log and ETW events in request pipeline. Most of that is removed.

### Authorization roles

Previous version of the app used to query roles from Azure AD. The new version removed that for simplicity - all logged in users are admins. The logic was like below, should be possible with ASP.NET Core if OpenID is used in less abstract way:

```csharp
new OpenIdConnectAuthenticationOptions
{
    ClientId = ClientId,
    Authority = Authority,
    Notifications = new OpenIdConnectAuthenticationNotifications {
        // If there is a code in the OpenID Connect response, redeem it for an access token and 
        // refresh token, and store those away.
        AuthorizationCodeReceived = async context =>
        {
            string userObjectId = context.AuthenticationTicket.Identity.FindFirst(
                    "http://schemas.microsoft.com/identity/claims/objectidentifier").Value;

            // Without token cache, check TokenCache class if performance is a concern
            var authContext = new AuthenticationContext(Authority, true);
            var tokenFetcher = await authContext.AcquireTokenByAuthorizationCodeAsync(
                context.Code,
                new Uri(HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Path)),
                new ClientCredential(ClientId, AppKey),
                GraphResourceId);

            var client = GetActiveDirectoryClient(tokenFetcher.AccessToken);
            var userFetcher = await client.Users.GetByObjectId(userObjectId).ExecuteAsync() as IUserFetcher;

            if (userFetcher == null)
            {
                return;
            }

            var pagedCollection = await userFetcher.MemberOf.ExecuteAsync();
            var groupMembership = new List<Group>();

            do
            {
                var currentPage = pagedCollection.CurrentPage.ToList();
                groupMembership.AddRange(currentPage.OfType<Group>());
                pagedCollection = await pagedCollection.GetNextPageAsync();
            } while (pagedCollection != null && pagedCollection.MorePagesAvailable);

            // Convert groups to claims and add them to the authentication ticket, 
            // ASP.NET will pick them from there
            var roleClaims = groupMembership.Select(
                    g => new Claim(context.AuthenticationTicket.Identity.RoleClaimType, g.DisplayName));

            context.AuthenticationTicket.Identity.AddClaims(roleClaims);
        },

        RedirectToIdentityProvider = context =>
        {
            // This ensures that the address used for sign in and sign out is picked up dynamically from the request
            // this allows you to deploy your app (to Azure Web Sites, for example) without having to change settings
            // Remember that the base URL of the address used here must be provisioned in Azure AD beforehand.
            string appBaseUrl = context.Request.Scheme + "://" + context.Request.Host + context.Request.PathBase;
            context.Options.RedirectUri = appBaseUrl + "/";
            context.Options.PostLogoutRedirectUri = appBaseUrl;
            return Task.FromResult(0);
        },
        AuthenticationFailed = context =>
        {
            context.OwinContext.Response.Redirect("/Error");
            context.HandleResponse(); // Suppress the exception
            return Task.FromResult(0);
        }
    }

}
```