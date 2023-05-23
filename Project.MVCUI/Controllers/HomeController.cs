using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Project.MVCUI.Controllers
{
    [Route("[Controller]/[Action]")]
    public class HomeController : Controller
    {
        [Route("/")]
        [Route("/Home")]
        [Route("/Home/Index")]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}