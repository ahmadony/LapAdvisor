using LapAdvisor.Bl;
using LapAdvisor.Models;
using LapAdvisor.Utlities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace LapAdvisor.Areas.admin.Controllers
{
    [Authorize (Roles ="Admin")]
    // Defines that this controller belongs to the "admin" area of the application
    [Area("admin")]
    public class ItemsController : Controller
    {
        public ItemsController(ICategories categories , 
            IItems item , IOs os , IItemTypes itemTypes)
        {
            oClsCategories = categories;
            oClsItems = item;
            oClsOs = os;
            oClsItemTypes = itemTypes;
        }

        IItems oClsItems;
        ICategories oClsCategories;
        IItemTypes oClsItemTypes;
        IOs oClsOs;
        public IActionResult List()
        {
            ViewBag.lstCategories = oClsCategories.GetAll();
            var items = oClsItems.GetAllItemsData(null);
            return View(items);
        }

        //filtering for item/category list
        public IActionResult Search(int id)
        {
            ViewBag.lstCategories = oClsCategories.GetAll();
            var items = oClsItems.GetAllItemsData(id);
            return View("List", items);
        }

        
        public IActionResult Edit(int? itemId)
        {
            var item = new Models.TbItem();
            ViewBag.lstCategories = oClsCategories.GetAll();
            ViewBag.lstItemTypes = oClsItemTypes.GetAll();
            ViewBag.lstOs = oClsOs.GetAll();
            if (itemId != null)
            {
                item = oClsItems.GetById(Convert.ToInt32(itemId));
            }
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(TbItem item, List<IFormFile> Files)
        {
            if (!ModelState.IsValid)
                return View("Edit", item);

            //This Helper is class to Upload image
            item.ImageName = await Helper.UploadImage(Files, "Items");

            oClsItems.Save(item);

            return RedirectToAction("List");
        }

        public IActionResult Delete(int itemId)
        {
            oClsItems.Delete(itemId);
            return RedirectToAction("List");
        }
    }
}
