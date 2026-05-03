using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace LapAdvisor.Areas.admin.Controllers
{
    // Defines that this controller belongs to the "admin" area of the application.
    [Area("admin")]
    public class HomeController : Controller
    {
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
