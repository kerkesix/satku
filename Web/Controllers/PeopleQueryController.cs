namespace Web.ontrollers
{
    using System.Linq;

    using QueryModels;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize()]
    public class PeopleQueryController: Controller
    {
        [Route("api/{happening}/[controller]")]
        public IActionResult Get()
        {
            var data = QueryModelRepository.EditPeople;
            return Json(data.OrderBy(m => m.Lastname));
        }
    }
}