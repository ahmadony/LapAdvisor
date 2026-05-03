using LapAdvisor.Models;
using System.Runtime;

namespace LapAdvisor.Model
{
    public class VmHomePage
    {
        public VmHomePage()
        {
            lstAllItems = new List<VwItem>();
            lstRecommendedItems = new List<VwItem>();
            lstNewItems = new List<VwItem>();
            lstFreeDelivery = new List<VwItem>();
            lstCategories = new List<TbCategory>();
            lstTodaysDeal = new List<VwItem>();
            lstSliders = new List<TbSlider>();
        }
        public List<VwItem> lstAllItems { get; set; }
        public List<VwItem> lstRecommendedItems { get; set; }
        public List<VwItem> lstNewItems { get; set; }
        public List<VwItem> lstFreeDelivery { get; set; }
        public List<TbCategory> lstCategories { get; set; }
        public List<VwItem> lstTodaysDeal { get; set; }
        public List<TbSlider> lstSliders { get; set; }
        
    }
}

