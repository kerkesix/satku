using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Models;
using System.Security.Claims;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index(string happening, string catchall)
        {
            string openPage = "report";

            if (!string.IsNullOrWhiteSpace(catchall))
            {
                // Remove hashbang
                if (catchall.StartsWith("#!/"))
                {
                    catchall = catchall.Substring(3);
                }

                openPage = catchall.Split(
                    "/".ToCharArray(),
                    StringSplitOptions.RemoveEmptyEntries)[0];
            }

            ViewData["Page"] = openPage;
            ViewData["Happening"] = happening;
            ViewData["DefaultHappening"] = (string)RouteData.DataTokens["DefaultHappening"];
            
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
