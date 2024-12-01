using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace ExpenseApp.Controllers
{
    [Route("[controller]")]
    public class PdfViewerController : Controller
    {
        // Endpoint to load PDF file
        [HttpPost("Load")]
        public IActionResult Load([FromBody] object request)
        {
            string reportPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Reports", "ReportRPT.pdf");

            if (System.IO.File.Exists(reportPath))
            {
                var pdfStream = System.IO.File.OpenRead(reportPath);
                return new FileStreamResult(pdfStream, "application/pdf");
            }
            else
            {
                return NotFound("PDF file not found.");
            }
        }


        [HttpPost("RenderPdfPages")]
        public IActionResult RenderPdfPages([FromBody] object jsonObject)
        {

            return Content("Render pages logic goes here", "application/json");
        }

        [HttpPost("Unload")]
        public IActionResult Unload([FromBody] object jsonObject)
        {
            return Content("Document unloaded", "application/json");
        }
    }
}
