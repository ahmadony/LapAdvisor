using LapAdvisor.Models;

namespace LapAdvisor.Model
{
    public class RecommendationResultRow
    {
        public VwItem Item { get; set; } = new VwItem();
        public int Score { get; set; }
    }
}
