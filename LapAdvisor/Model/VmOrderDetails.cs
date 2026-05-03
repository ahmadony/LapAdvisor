using LapAdvisor.Models;
using System.Collections.Generic;

namespace LapAdvisor.Model
{
    public class VmOrderDetails
    {
        public TbSalesInvoice Invoice { get; set; }
        public List<VmOrderDetailsItem> Items { get; set; } = new();
        public decimal Total => Items.Sum(x => x.LineTotal);
    }

    public class VmOrderDetailsItem
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string ImageName { get; set; }
        public int Qty { get; set; }
        public decimal Price { get; set; }
        public decimal LineTotal => Qty * Price;
    }
}
