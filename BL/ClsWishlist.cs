using LapAdvisor.Models;
using Microsoft.EntityFrameworkCore;

namespace LapAdvisor.Bl
{
    public interface IWishlist
    {
        bool AddToWishlist(string userId, int itemId);
        bool RemoveFromWishlist(string userId, int itemId);
        List<VwItem> GetWishlistByUser(string userId);
        bool IsInWishlist(string userId, int itemId);
    }
    public class ClsWishlist : IWishlist
    {
        private readonly LapAdvisorDbContext dbContext;

        public ClsWishlist(LapAdvisorDbContext ctx)
        {
            dbContext = ctx;
        }

        // Add item to wishlist
        public bool AddToWishlist(string userId, int itemId)
        {
            try
            {
                var exists = dbContext.TbWishlists
                    .Any(w => w.UserId == userId && w.ItemId == itemId);

                if (exists)
                    return false;

                TbWishlist wishlist = new TbWishlist
                {
                    UserId = userId,
                    ItemId = itemId,
                    CreatedAt = DateTime.Now
                };

                dbContext.TbWishlists.Add(wishlist);
                dbContext.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }


        // Remove item from wishlist
        public bool RemoveFromWishlist(string userId, int itemId)
        {
            try
            {
                var item = dbContext.TbWishlists
                    .FirstOrDefault(w => w.UserId == userId && w.ItemId == itemId);

                if (item != null)
                {
                    dbContext.TbWishlists.Remove(item);
                    dbContext.SaveChanges();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Get wishlist by user
        public List<VwItem> GetWishlistByUser(string userId)
        {
            try
            {
                var items = (from w in dbContext.TbWishlists
                             join v in dbContext.VwItems
                             on w.ItemId equals v.ItemId
                             where w.UserId == userId
                             select v).ToList();

                return items;
            }
            catch
            {
                return new List<VwItem>();
            }
        }


        // Check if item is in wishlist
        public bool IsInWishlist(string userId, int itemId)
        {
            return dbContext.TbWishlists
                .Any(w => w.UserId == userId && w.ItemId == itemId);
        }
    }
}
