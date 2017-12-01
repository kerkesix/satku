namespace Web.ontrollers
{
    using System.Linq;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using QueryModels;

    [Authorize()]
    public class LinkPeopleQueryController : Controller
    {
        [Route("api/{happening}/[controller]")]
        public IActionResult Get(string happening)
        {
            // Get all persons available for this happening
            var data = QueryModelRepository.LinkPeople.Where(m => !m.Happenings.ContainsKey(happening));
            return Json(data.OrderBy(m => m.Name));
        }
    }
}