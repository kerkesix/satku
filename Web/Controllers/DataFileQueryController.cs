namespace Web.ontrollers
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;

    using QueryModels;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize()]
    public partial class DataFileQueryController : Controller
    {
        [Route("api/{happening}/[controller]")]
        public virtual IActionResult Get(string happening)
        {
            if (!QueryModelRepository.Csv.ContainsKey(happening))
            {
                return NotFound();
            }

            var h = QueryModelRepository.Csv[happening];
            string fileName = string.Format(CultureInfo.InvariantCulture, "{0}-{1:o}.csv", happening, DateTime.UtcNow);

            // Working around Excel problems opening UTF-8 files - Excel needs BOM. 
            // See e.g. http://stackoverflow.com/questions/4414088/how-to-getbytes-in-c-sharp-with-utf8-encoding-with-bom/4414118#4414118
            // Without this problem could just use StringContent class.
            var data = Encoding.UTF8.GetBytes(h.GenerateFile());
            var dataWithByteOrderMark = Encoding.UTF8.GetPreamble().Concat(data).ToArray();

            var res = new FileStreamResult(new MemoryStream(dataWithByteOrderMark), "application/csv");
            res.FileDownloadName = fileName;
                        
            return res;
        }
    }
}
