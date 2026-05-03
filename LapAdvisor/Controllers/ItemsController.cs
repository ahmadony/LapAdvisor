using Domains;
using Domains.Dtos;
using LapAdvisor.Bl;
using LapAdvisor.Model;
using Microsoft.AspNetCore.Mvc;
namespace LapAdvisor.Controllers
{
    public class ItemsController : Controller
    {
        // this is for dependency injection
        IItems oItem;
        IItemImages oItemImages;
        IFeedback oFeedback;
        public ItemsController(IItems iItem, IItemImages Itemimages, IFeedback feedback)
        {
            oItem = iItem;
            oItemImages = Itemimages;
            oFeedback = feedback;
        }

        [HttpPost]
        public IActionResult FilterItems([FromBody] ItemFilterDto filter)
        {
            var result = oItem.FilterItems(filter); 
            return Json(result);
        }
        public IActionResult ItemDetails(int id)
        {
            var item = oItem.GetItemId(id);

            VmItemDetails vm = new VmItemDetails();
            vm.Item = item;
            vm.lstRecommendedItems = oItem.GetRecommendedItems(id).Take(20).ToList();
            vm.lstItemImages = oItemImages.GetByItemId(id);
            vm.lstFeedbacks = oFeedback.GetByItemId(id);
            return View(vm);
        }

        [HttpPost]
        public IActionResult SubmitReview(TbFeedback feedback)
        {
            // Validation بسيط
            if (feedback.ItemId <= 0) return RedirectToAction("ItemList");

            if (feedback.Rating < 1 || feedback.Rating > 5)
                return RedirectToAction("ItemDetails", new { id = feedback.ItemId });

            if (string.IsNullOrWhiteSpace(feedback.Name) || string.IsNullOrWhiteSpace(feedback.Email))
                return RedirectToAction("ItemDetails", new { id = feedback.ItemId });

            oFeedback.Add(feedback);

            return RedirectToAction("ItemDetails", new { id = feedback.ItemId });
        }

        public IActionResult ItemList(string? brand)
        {
            ViewBag.Brand = brand;
            return View();
        }

    }
}
