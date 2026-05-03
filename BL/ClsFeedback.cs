using Domains;
using LapAdvisor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapAdvisor.Bl
{
    public interface IFeedback
    {
        List<TbFeedback> GetByItemId(int itemId);
        bool Add(TbFeedback feedback);
    }
    public class ClsFeedback : IFeedback
    {
        private readonly LapAdvisorDbContext db;
        public ClsFeedback(LapAdvisorDbContext ctx) => db = ctx;

        public List<TbFeedback> GetByItemId(int itemId)
        {
            return db.TbFeedbacks
                     .Where(x => x.ItemId == itemId && x.CurrentState == 1)
                     .OrderByDescending(x => x.CreatedDate)
                     .ToList();
        }

        public bool Add(TbFeedback feedback)
        {
            try
            {
                feedback.CreatedDate = DateTime.Now;
                feedback.CurrentState = 1;

                db.TbFeedbacks.Add(feedback);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
