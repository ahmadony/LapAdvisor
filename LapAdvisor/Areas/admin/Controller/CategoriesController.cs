using Microsoft.AspNetCore.Mvc;
using LapAdvisor.Models;
using LapAdvisor.Bl;
using LapAdvisor.Utlities;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
namespace LapAdvisor.Areas.admin.Controllers
{
    [Authorize(Roles = "Admin")]
    // Defines that this controller belongs to the "admin" area of the application.
    [Area("admin")]
    public class CategoriesController : Controller
    {
        public CategoriesController(ICategories categories) 
        {
            oClsCategories = categories;
        }
        ICategories oClsCategories;
        public IActionResult List()
        {
            return View(oClsCategories.GetAll());
        }

        public IActionResult Edit(int? categoryId)
        {
            var category = new TbCategory();
            if (categoryId != null)
            {
                category = oClsCategories.GetById(Convert.ToInt32(categoryId));
            }
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(TbCategory category, List<IFormFile> Files)
        {
            if(!ModelState.IsValid)
               return View("Edit",category);

            //This Helper is class to Upload image
            category.ImageName = await Helper.UploadImage(Files, "Categories");

            oClsCategories.Save(category);

            return RedirectToAction("List");
        }

        public IActionResult Delete(int categoryId)
        {
            oClsCategories.Delete(categoryId);
            return RedirectToAction("List");
        }

        
    }
}
