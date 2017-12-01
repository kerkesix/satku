namespace Web.ontrollers
{
    using System.Web;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    public class SystemController : Controller
    {
        private IApplicationLifetime ApplicationLifetime { get; set; }

        public SystemController(IApplicationLifetime appLifetime)
        {
            ApplicationLifetime = appLifetime;
        }

        [Route("api/{happening}/[controller]")]
        [HttpDelete]
        public void Delete()
        {
            // This forces all the query model data to be reloaded from event store.
            this.ApplicationLifetime.StopApplication();
        }
    }
}