using MongoDB.Bson;

namespace Crankdata.Models
{
    public class PerformanceViewModel
    {
        public int BillingIndex { get; set; }
        public string ArtistName { get; set; }
        public string Billing { get; set; }
        public string ArtistId { get; set; }
    }
}