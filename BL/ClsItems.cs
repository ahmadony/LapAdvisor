using LapAdvisor.Models;
using Domains.Dtos;
namespace LapAdvisor.Bl
{
    public interface IItems
    {
        public List<TbItem> GetAll();
        public List<VwItem> GetAllItemsData(int? categoryId);

        public PagedResult<VwItem> FilterItems(ItemFilterDto filter);
        public List<VwItem> GetRecommendedItems(int ItemId);
        public TbItem GetById(int id);
        public VwItem GetItemId(int id);
        public bool Save(TbItem item);
        public bool Delete(int id);
    }
    public class ClsItems : IItems
    {
        LapAdvisorDbContext dbContext;
        public ClsItems(LapAdvisorDbContext ctx)
        {
            dbContext = ctx;
        }
        public List<TbItem> GetAll()
        {
            try
            {
                var lstCategories = dbContext.TbItems.ToList();
                return lstCategories;
            }

            catch
            {
                return new List<TbItem>();
            }
        }

        public List<VwItem> GetAllItemsData(int? categoryId)
        {
            try
            {
                // This for filtering
                var lstCategories = dbContext.VwItems.Where(a=> (a.CategoryId==categoryId||categoryId==null || categoryId==0)
                && a.CurrentState==1).ToList();
                return lstCategories;
            }

            catch
            {
                return new List<VwItem>();
            }
        }

        public PagedResult<VwItem> FilterItems(ItemFilterDto filter)
        {
            var query = dbContext.VwItems.Where(x => x.CurrentState == 1);

            if (filter.Brands.Any())
                query = query.Where(x => filter.Brands.Contains(x.CategoryName));

            if (filter.LaptopTypes.Any())
                query = query.Where(x => filter.LaptopTypes.Contains(x.ItemTypeName));

            if (filter.OS.Any())
                query = query.Where(x => filter.OS.Contains(x.OsName));

            if (filter.Processors != null && filter.Processors.Any())
            {
                query = query.Where(x =>
                    filter.Processors.Any(p =>
                        x.Processor != null &&
                        x.Processor.Contains(p)
                    )
                );
            }

            if (filter.Rams.Any())
                query = query.Where(x => filter.Rams.Contains(x.RamSize ?? 0));

            if (filter.MinPrice.HasValue)
                query = query.Where(x => x.SalesPrice >= filter.MinPrice);

            if (filter.MaxPrice.HasValue)
                query = query.Where(x => x.SalesPrice <= filter.MaxPrice);

            int totalCount = query.Count(); // 🔥 العدد الكلي 1300

            var data = query
                .OrderByDescending(x => x.ItemId)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();

            return new PagedResult<VwItem>
            {
                Data = data,
                TotalCount = totalCount
            };
        }

        /*
        The system recommends laptops based on category, 
        laptop type, 
        and price range to ensure functional similarity rather than only price similarity
         */
        public List<VwItem> GetRecommendedItems(int itemId)
        {
            try
            {
                // bring the item who the user click on it
                var current = dbContext.VwItems.FirstOrDefault(x => x.ItemId == itemId);
                // if the item is existing
                if (current == null)
                    return new List<VwItem>();
                // LINQ query
                var result = (from item in dbContext.VwItems
                              join img in dbContext.TbItemImages
                              on item.ItemId equals img.ItemId
                              // do not bring the current item to remove repetition
                              where item.ItemId != itemId
                              // The product is activated
                              && item.CurrentState == 1
                              //Same classification , apple = apple etc...
                              && item.CategoryId == current.CategoryId
                              // Same type of device , gaming = gaming etc...
                              && item.ItemTypeId == current.ItemTypeId

                              /* The difference between the current
                                 and suggested product prices must be
                                 less than or equal to 15%
                               */
                              && Math.Abs(item.SalesPrice - current.SalesPrice)
                                               <= current.SalesPrice * (decimal)0.15
                              select new VwItem
                              {
                                  //Identifying the displayed data
                                  ItemId = item.ItemId,
                                  ItemName = item.ItemName,
                                  SalesPrice = item.SalesPrice,
                                  ImageName = img.ImageName
                              })
                              // To avoid duplication of the same product
                              .Distinct()
                              .Take(12)
                              .ToList();

                return result;
            }
            catch
            {
                return new List<VwItem>();
            }
        }


        public TbItem GetById(int id)
        {
            try
            {
                var item = dbContext.TbItems.FirstOrDefault(a => a.ItemId == id && a.CurrentState == 1);
                return item;

            }

            catch
            {
                return new TbItem();
            }
        }

        public VwItem GetItemId(int id)
        {
            try
            {
                var item = dbContext.VwItems.FirstOrDefault(a => a.ItemId == id && a.CurrentState == 1);
                return item;

            }

            catch
            {
                return new VwItem();
            }
        }

        public bool Save(TbItem item)
        {
            try
            {    
                if (item.ItemId == 0)
                {
                    item.CurrentState = 1;
                    item.CreatedBy = "1";
                    item.CreatedDate = DateTime.Now;
                    dbContext.TbItems.Add(item);
                }
                else
                {
                    item.UpdatedBy = "1";
                    item.UpdatedDate = DateTime.Now;
                    item.CurrentState = 1;
                    dbContext.Entry(item).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                }
                dbContext.SaveChanges();
                return true;
                

            }
            catch
            {
                return false;
            }
        }

        public bool Delete(int id)
        {
            try
            {
                var item = dbContext.TbItems.FirstOrDefault(a => a.ItemId == id);
                if (item != null)
                {
                    //dbContext.TbItems.Remove(item);
                    item.CurrentState = 0;
                    dbContext.Entry(item).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    dbContext.SaveChanges();
                }
                return true;
                
            }

            catch
            {
                return false;
            }
        }
    }
}
