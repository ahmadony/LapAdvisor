using LapAdvisor.Models;

namespace LapAdvisor.Bl
{
    public interface IItemImages
    {
        public List<TbItemImage> GetByItemId(int id);
    }

    public class ClsItemImages : IItemImages
    {
        LapAdvisorDbContext dbContext;
        public ClsItemImages(LapAdvisorDbContext ctx)
        {
            dbContext = ctx;
        }

        public List<TbItemImage> GetByItemId(int id)
        {
            try
            {
                var item = dbContext.TbItemImages.Where(a => a.ItemId == id).ToList();
                return item;
            }
            catch
            {
                return new List<TbItemImage>();
            }
        }
    }
}
