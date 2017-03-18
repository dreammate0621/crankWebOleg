using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Crankdata.Models
{
    public class MetroArea
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public string Name { get; set; }

        public int? MetroSKId { get; set; }

        public string State { get; set; }

        public string Country { get; set; }

    }
}