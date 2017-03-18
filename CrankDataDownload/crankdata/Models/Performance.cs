using MongoDB.Bson;

namespace Crankdata.Models
{
    public class Performance
    {
        public int BillingIndex { get; set; }
        public string ArtistName { get; set; }
        public int ArtistSKId { get; set; }
        public int PerfSKId { get; set; }
        public string Billing { get; set; }
        public ObjectId ArtistId { get; set; }

    }
}