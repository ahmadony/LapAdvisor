using LapAdvisor.Models;
namespace LapAdvisor.Bl
{
    public interface ICategories
    {
        public List<TbCategory> GetAll();
        public TbCategory GetById(int id);
        public bool Save(TbCategory category);
        public bool Delete(int id);
    }

    public class ClsCategories : ICategories
    {
        LapAdvisorDbContext dbContext;
        public ClsCategories(LapAdvisorDbContext ctx)
        {
            dbContext = ctx;
        }
        public List<TbCategory> GetAll()
        {
            try
            {
                var lstCategories = dbContext.TbCategories.Where(a=> a.CurrentState==1).ToList();
                return lstCategories;
            }

            catch
            {
                return new List<TbCategory>();
            }
        }

        public TbCategory GetById(int id)
        {
            try
            {
                var category = dbContext.TbCategories.FirstOrDefault(a => a.CategoryId == id && a.CurrentState == 1);
                return category;

            }

            catch
            {
                return new TbCategory();
            }
        }

        public bool Save(TbCategory category)
        {
            try
            {
                if (category.CategoryId == 0)
                {
                    category.CreatedBy = "1";
                    category.CreatedDate = DateTime.Now;
                    category.CurrentState = 1;
                    dbContext.TbCategories.Add(category);
                }
                else
                {
                    category.UpdatedBy = "1";
                    category.UpdatedDate = DateTime.Now;
                    category.CurrentState= 1;
                    dbContext.Entry(category).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
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
                var category = dbContext.TbCategories.FirstOrDefault(a => a.CategoryId == id);
                if (category != null)
                {
                    category.CurrentState = 0;
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
