namespace Web.ontrollers
{
    using Microsoft.AspNetCore.Mvc;

    using QueryModels;

    public partial class AttendeesController : Controller
    {
        [Route("barcodes/{happening}")]
        public virtual ActionResult Barcodes(string happening)
        {
            var h = QueryModelRepository.Dashboard.Happenings[happening];

            if (h == null)
            {
                return NotFound();
            }

            return View(h.Attendees);
        }
    }
}