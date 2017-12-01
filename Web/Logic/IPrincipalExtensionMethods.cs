namespace Web.Logic
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Principal;

    public static class IPrincipalExtensionMethods
    {
        public static bool IsContributor(this IPrincipal principal)
        {
            return principal != null;
        }
    }
}