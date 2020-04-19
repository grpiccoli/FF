using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FPFI.Models.ViewModels;
using System.IO;

namespace FPFI.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        [Route(".well-known/acme-challenge/{id}")]
        public IActionResult LetsEncrypt(string id)
        {
            var file = Path.Combine(Directory.GetCurrentDirectory(), ".well-known", "acme-challenge", id);
            return PhysicalFile(file, "text/plain");
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
