using LapAdvisor.Bl;
using LapAdvisor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LapAdvisor.Controllers
{

    public class WishlistController : Controller
    {
        private readonly LapAdvisorDbContext _context;

        public WishlistController(LapAdvisorDbContext context)
        {
            _context = context;
        }

        [Authorize]
        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var items = (from w in _context.TbWishlists
                         join v in _context.VwItems
                         on w.ItemId equals v.ItemId
                         where w.UserId == userId
                         select v).ToList();

            return View(items);
        }

        [HttpPost]
        public IActionResult Add(int itemId)
        {
            if (!User.Identity.IsAuthenticated)
                return Json(new { status = "NOT_LOGGED_IN" });

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            bool exists = _context.TbWishlists
                .Any(w => w.UserId == userId && w.ItemId == itemId);

            if (exists)
                return Json(new { status = "EXISTS" });

            _context.TbWishlists.Add(new TbWishlist
            {
                UserId = userId,
                ItemId = itemId,
                CreatedAt = DateTime.Now
            });

            _context.SaveChanges();

            return Json(new { status = "ADDED" });
        }

        // POST: /Wishlist/Remove
        [HttpPost]
        public IActionResult Remove(int itemId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Json("NOT_LOGGED_IN");

            var row = _context.TbWishlists
                .FirstOrDefault(w => w.UserId == userId && w.ItemId == itemId);

            if (row == null) return Json(false);

            _context.TbWishlists.Remove(row);
            _context.SaveChanges();

            return Json(true);
        }

        [HttpPost]
        public IActionResult AddAjax(int itemId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Json(new { status = "NOT_LOGGED_IN" });

            var exists = _context.TbWishlists
                .Any(x => x.UserId == userId && x.ItemId == itemId);

            if (exists)
                return Json(new { status = "EXISTS" });

            _context.TbWishlists.Add(new TbWishlist
            {
                UserId = userId,
                ItemId = itemId,
                CreatedAt = DateTime.Now
            });

            _context.SaveChanges();
            return Json(new { status = "ADDED" });
        }

        [HttpPost]
        public IActionResult RemoveAjax(int itemId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Json(new { status = "NOT_LOGGED_IN" });

            var row = _context.TbWishlists
                .FirstOrDefault(x => x.UserId == userId && x.ItemId == itemId);

            if (row != null)
            {
                _context.TbWishlists.Remove(row);
                _context.SaveChanges();
            }

            return Json(new { status = "REMOVED" });
        }


    }
}

