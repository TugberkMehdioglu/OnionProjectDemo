using Microsoft.AspNetCore.Mvc;

namespace Project.MVCUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("[Area]/[Controller]/[Action]")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
