using LapAdvisor.Bl;
using LapAdvisor.Model;
using LapAdvisor.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Policy;
namespace LapAdvisor.Controllers
{
    public class HomeController : Controller
    {
        // this is for dependency injection
        IItems oClsItems;
        ISliders oClsSliders;
        ICategories oClsCategories;
        public HomeController(IItems item , ISliders sliders , ICategories categories) 
        {
            oClsItems = item;
            oClsSliders = sliders;
            oClsCategories = categories;
        }
        public IActionResult Index()
        {
            // calling class model VmHomePage to add items in home page
            VmHomePage vm = new VmHomePage();
            vm.lstAllItems = oClsItems.GetAllItemsData(null).Skip(40).Take(25).ToList();
            vm.lstRecommendedItems = oClsItems.GetAllItemsData(null).Skip(85).Take(10).ToList();
            vm.lstNewItems = oClsItems.GetAllItemsData(null).Skip(90).Take(10).ToList();
            vm.lstFreeDelivery = oClsItems.GetAllItemsData(null).Skip(180).Take(4).ToList();
            vm.lstSliders = oClsSliders.GetAll();
            vm.lstCategories = oClsCategories.GetAll().Take(4).ToList();
            // Today Deals are dynamically generated based on the latest products added to the system
            vm.lstTodaysDeal = oClsItems
               .GetAllItemsData(null)
               .OrderByDescending(x => x.ItemId)
               .Take(24)
               .ToList();

            return View(vm);
        }
    }
}
