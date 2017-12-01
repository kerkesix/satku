namespace Web.ontrollers
{
    using Microsoft.AspNetCore.Mvc;
    using QueryModels;
    
    public class DashboardQueryController : Controller
    {
        [Route("api/{happening}/[controller]")]
        public IActionResult Get(string happening)
        {
            var data = QueryModelRepository.Dashboard.Happenings[happening];
            return Json(data);
        }
    }
}
