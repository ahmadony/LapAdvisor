using Domains;
using LapAdvisor.Models;

namespace LapAdvisor.Model
{
    public class VmItemDetails
    {
        public VmItemDetails()
        {
            Item = new VwItem();
            lstItemImages = new List<TbItemImage>();
            lstRecommendedItems = new List<VwItem>();
            lstFeedbacks = new List<TbFeedback>();
            NewFeedback = new TbFeedback();
        }
        public VwItem Item { get; set; }
        public List<TbItemImage> lstItemImages { get; set; }
        public List<VwItem> lstRecommendedItems { get; set; }
        public List<TbFeedback> lstFeedbacks { get; set; }
        public TbFeedback NewFeedback { get; set; }
    }
}
