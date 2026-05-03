namespace LapAdvisor.Model
{
    public class VmAdminOrderDetails
    {
        public int InvoiceId { get; set; }
        public string CustomerName { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime DelivryDate { get; set; }
        public string OrderStatus { get; set; }

        public decimal Total { get; set; }
        public List<VmAdminOrderItem> Items { get; set; } = new();
    }

    public class VmAdminOrderItem
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string ImageName { get; set; }
        public int Qty { get; set; }
        public decimal Price { get; set; }
        public decimal LineTotal => Qty * Price;
    }
}
