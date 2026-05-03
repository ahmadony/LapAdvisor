using LapAdvisor.Bl;
using LapAdvisor.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace LapAdvisor.Controllers
{
    public class CompareController : Controller
    {
        private const string KEY = "COMPARE_IDS";
        private const int MAX = 4;

        private readonly IItems _items;

        public CompareController(IItems items)
        {
            _items = items;
        }

        // صفحة المقارنة
        [HttpGet]
        public IActionResult Index()
        {
            var ids = GetIds();

            var items = ids
                .Select(id => _items.GetItemId(id))
                .Where(x => x != null && x.ItemId != 0)
                .ToList();

            return View(items);
        }

        // AJAX: إضافة
        [HttpPost]
        public IActionResult AddAjax(int itemId)
        {
            var ids = GetIds();

            if (!ids.Contains(itemId))
            {
                if (ids.Count >= MAX)
                    return Json(new { status = "FULL", max = MAX });

                ids.Add(itemId);
                SaveIds(ids);
            }

            return Json(new { status = "ADDED", count = ids.Count });
        }

        // AJAX: إزالة
        [HttpPost]
        public IActionResult RemoveAjax(int itemId)
        {
            var ids = GetIds();
            ids.Remove(itemId);
            SaveIds(ids);

            return Json(new { status = "REMOVED", count = ids.Count });
        }

        // AJAX: تفريغ
        [HttpPost]
        public IActionResult ClearAjax()
        {
            SaveIds(new List<int>());
            return Json(new { status = "CLEARED", count = 0 });
        }

        // AJAX: عدّاد (للبادج)
        [HttpGet]
        public IActionResult Count()
        {
            var ids = GetIds();
            return Json(new { count = ids.Count });
        }

        // =====================
        // Helpers
        // =====================
        private List<int> GetIds()
        {
            return HttpContext.Session.GetObject<List<int>>(KEY) ?? new List<int>();
        }

        private void SaveIds(List<int> ids)
        {
            HttpContext.Session.SetObject(KEY, ids);
        }
    }
}
