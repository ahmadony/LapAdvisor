using LapAdvisor.Bl;
using LapAdvisor.Models;
using LapAdvisor.Model;
using Domains.Dtos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LapAdvisor.Controllers
{
    public class RecommendationController : Controller
    {
        private readonly IItems _items;

        public RecommendationController(IItems items)
        {
            _items = items;
        }

        // صفحة Recommendation
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        
        [HttpPost]
        public IActionResult GetRecommendations([FromBody] RecommendationRequestDto req)
        {
            // 1) نجيب كل العناصر (من View الجاهز عندك)
            var all = _items.GetAllItemsData(null)
                            .Where(x => x.CurrentState == 1)
                            .ToList();

            // 2) نحسب Score ونفلتر حسب إجابات المستخدم
            var ranked = all.Select(x => new RecommendationResultRow
            {
                Item = x,
                Score = CalculateScore(x, req)
            })
            .Where(r => r.Score > 0)
            .OrderByDescending(r => r.Score)
            .ThenByDescending(r => r.Item.CreatedDate)
            .ToList();

            // 3) Pagination
            int totalCount = ranked.Count;

            int page = req.Page <= 0 ? 1 : req.Page;
            int pageSize = req.PageSize <= 0 ? 12 : req.PageSize;

            var pageData = ranked.Skip((page - 1) * pageSize)
                                 .Take(pageSize)
                                 .Select(r => new
                                 {
                                     itemId = r.Item.ItemId,
                                     itemName = r.Item.ItemName,
                                     salesPrice = r.Item.SalesPrice,
                                     imageName = r.Item.ImageName,
                                     processor = r.Item.Processor,
                                     gpu = r.Item.Gpu,
                                     ramSize = r.Item.RamSize,
                                     hardDisk = r.Item.HardDisk,
                                     screenSize = r.Item.ScreenSize,
                                     score = r.Score
                                 })
                                 .ToList();

            return Json(new
            {
                data = pageData,
                totalCount = totalCount
            });
        }

        // Popup: External Links (Partial HTML)
        [HttpGet]
        public IActionResult ExternalLinksPopup(int id)
        {
            var item = _items.GetItemId(id);
            if (item == null || item.ItemId == 0) return Content("");

            var stores = BuildExternalStores(item);

            var vm = new ExternalLinksVm
            {
                ItemId = item.ItemId,
                ItemName = item.ItemName,
                Stores = stores
            };

            return PartialView("_ExternalLinksPopup", vm);
        }

        // =========================
        // Recommendation Logic
        // =========================
        private int CalculateScore(VwItem item, RecommendationRequestDto req)
        {
            int score = 0;

            // Budget (أساسي)
            var (min, max) = ParseBudget(req.Budget);
            if (min.HasValue && max.HasValue)
            {
                if (item.SalesPrice >= min && item.SalesPrice <= max)
                    score += 40;
                else
                    score += 10; // بعيد عن الميزانية بس مش مستبعد
            }


            // Purpose (تقديري - لأنه ما عندك field مباشر لـ purpose)
            // بنستخدم مؤشرات من المواصفات
            string purpose = (req.Purpose ?? "").Trim().ToLower();

            if (purpose == "gaming")
            {
                if (!string.IsNullOrEmpty(item.Gpu) && item.Gpu.ToLower().Contains("rtx")) score += 30;
                else if (!string.IsNullOrEmpty(item.Gpu) && item.Gpu.ToLower().Contains("gtx")) score += 20;
                else score -= 10;

                if ((item.RamSize ?? 0) >= 16) score += 10;
            }
            else if (purpose == "programming")
            {
                if ((item.RamSize ?? 0) >= 16) score += 20;
                else if ((item.RamSize ?? 0) >= 8) score += 10;

                // نعتبر SSD مؤشر (هاردسك عندك نصي)
                if (!string.IsNullOrEmpty(item.HardDisk) && item.HardDisk.ToLower().Contains("ssd")) score += 10;
            }
            else if (purpose == "university")
            {
                if ((item.RamSize ?? 0) >= 8) score += 15;
                if (item.SalesPrice <= 600) score += 10;
            }
            else if (purpose == "business")
            {
                if (item.SalesPrice <= 700) score += 10;
                if ((item.RamSize ?? 0) >= 8) score += 10;
            }
            else if (purpose == "design")
            {
                if (!string.IsNullOrEmpty(item.Gpu) && (item.Gpu.ToLower().Contains("rtx") || item.Gpu.ToLower().Contains("gtx")))
                    score += 20;
                if ((item.RamSize ?? 0) >= 16) score += 15;
            }

            // Priority
            string priority = (req.Priority ?? "").Trim().ToLower();
            if (priority == "performance")
            {
                if ((item.RamSize ?? 0) >= 16) score += 10;
                if (!string.IsNullOrEmpty(item.Processor) && item.Processor.ToLower().Contains("i7")) score += 10;
                if (!string.IsNullOrEmpty(item.Processor) && item.Processor.ToLower().Contains("ryzen 7")) score += 10;
            }
            else if (priority == "price")
            {
                score += item.SalesPrice <= 600 ? 15 : 5;
            }
            else if (priority == "display")
            {
                if (!string.IsNullOrEmpty(item.ScreenReslution) && item.ScreenReslution.ToLower().Contains("fhd")) score += 10;
                if (!string.IsNullOrEmpty(item.ScreenReslution) && item.ScreenReslution.ToLower().Contains("2k")) score += 15;
                if (!string.IsNullOrEmpty(item.ScreenReslution) && item.ScreenReslution.ToLower().Contains("4k")) score += 20;
            }

            // Screen size (اختياري)
            if (!string.IsNullOrWhiteSpace(req.ScreenSize))
            {
                // req.ScreenSize مثل: "14", "15.6", "16+"
                if (req.ScreenSize == "16+")
                {
                    if (!string.IsNullOrEmpty(item.ScreenSize) && (item.ScreenSize.Contains("16") || item.ScreenSize.Contains("17")))
                        score += 5;
                }
                else
                {
                    if (!string.IsNullOrEmpty(item.ScreenSize) && item.ScreenSize.Contains(req.ScreenSize))
                        score += 5;
                }
            }

            return Math.Max(score, 0);
        }

        private (decimal? min, decimal? max) ParseBudget(string? budget)
        {
            budget = (budget ?? "").Trim();
            return budget switch
            {
                "0-400" => (0, 400),
                "400-600" => (400, 600),
                "600-800" => (600, 800),
                "800+" => (800, null),
                _ => (null, null)
            };
        }

        // =========================
        // External Links Generation
        // =========================
        private List<ExternalStoreRow> BuildExternalStores(VwItem item)
        {
            var keywords = item.ItemName
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Take(2);

            var query = Uri.EscapeDataString(string.Join(" ", keywords));

            return new List<ExternalStoreRow>
    {
        new ExternalStoreRow
        {
            StoreName = "City Center",
            ApproxPrice = Math.Round(item.SalesPrice * 1.05m, 2),
            Url = $"https://citycenter.jo/product/search?search={query}"
        },
        new ExternalStoreRow
        {
             StoreName = "PC Circle",
            ApproxPrice = Math.Round(item.SalesPrice * 1.04m, 2),
            Url = $"https://pccircle.com/?s={query}"
        },
        new ExternalStoreRow
        {
            StoreName = "GTS",
            ApproxPrice = Math.Round(item.SalesPrice * 1.02m, 2),
            Url = $"https://gts.jo/search?keyword={query}"
        }
    };
        }



    }
}
