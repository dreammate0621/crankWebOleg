using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Crankdata.Models
{
    [BsonIgnoreExtraElements]
    public class PerformanceSummary
    {
        public string ArtistName { get; set; }
        public ObjectId ArtistId { get; set; }
    }
}