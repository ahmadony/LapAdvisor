using LapAdvisor.Bl;
using Microsoft.AspNetCore.Mvc;

namespace LapAdvisor.Controllers
{
    public class PagesController : Controller
    {
        IPages oClsPages;
        public PagesController(IPages page) 
        {
            oClsPages = page;
        }
        public IActionResult page(int pageId)
        {
            var page = oClsPages.GetById(pageId);
            return View(page);
        }
    }
}
