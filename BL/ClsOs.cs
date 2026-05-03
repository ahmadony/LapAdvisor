using LapAdvisor.Models;
namespace LapAdvisor.Bl
{
    public interface IOs
    {
        public List<TbO> GetAll();
        public TbO GetById(int id);
        public bool Save(TbO os);
        public bool Delete(int id);
    }
    public class ClsOs : IOs
    {
        LapAdvisorDbContext dbContext;
        public ClsOs(LapAdvisorDbContext ctx)
        {
            dbContext = ctx;
        }
        public List<TbO> GetAll()
        {
            try
            {
                var lstCategories = dbContext.TbOs.Where(a => a.CurrentState == 1).ToList();
                return lstCategories;
            }

            catch
            {
                return new List<TbO>();
            }
        }

        public TbO GetById(int id)
        {
            try
            {
                var os = dbContext.TbOs.FirstOrDefault(a => a.OsId == id && a.CurrentState==1);
                return os;

            }

            catch
            {
                return new TbO();
            }
        }

        public bool Save(TbO os)
        {
            try
            {
                if (os.OsId == 0)
                {
                    os.CreatedBy = "1";
                    os.CreatedDate = DateTime.Now;
                    dbContext.TbOs.Add(os);
                }
                else
                {
                    os.UpdatedBy = "1";
                    os.UpdatedDate = DateTime.Now;
                    dbContext.Entry(os).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
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
                var os = dbContext.TbOs.FirstOrDefault(a => a.OsId == id);
                if (os != null)
                {
                    os.CurrentState = 0;
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
