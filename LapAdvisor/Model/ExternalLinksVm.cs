namespace LapAdvisor.Model
{
    public class ExternalLinksVm
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; } = "";
        public List<ExternalStoreRow> Stores { get; set; } = new();
    }
}
