namespace Web.ontrollers
{
    using System.Linq;

    using QueryModels;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Happening = QueryModels.Happening;

    [Authorize()]
    public class HappeningQueryController: Controller
    {
        [Route("api/{happening}/[controller]")]
        public IActionResult Get()
        {
            var data = QueryModelRepository.EditHappenings;
            return Json(data.OrderBy(m => m.Key));
        }
    }
}