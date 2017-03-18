using MongoDB.Bson;
using MongoDB.Driver.GeoJsonObjectModel;
using MongoDB.Bson.Serialization.Attributes;

namespace Crankdata.Models
{
    public class VenueViewModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Street { get; set; }

        public double? Lng { get; set; }
        public double? Lat { get; set; }

        public string Phone { get; set; }
        public string Website { get; set; }

        public string City { get; set; }

        public string State { get; set; }
        public string Country { get; set; }
        public int? Capacity { get; set; }

        public string Description { get; set; }

        public MetroAreaViewModel MetroArea { get; set; }

    }
}
