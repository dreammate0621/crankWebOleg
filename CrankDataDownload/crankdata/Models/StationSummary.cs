using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.GeoJsonObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crankdata.Models
{
    [BsonIgnoreExtraElements]
    public class StationSummary
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public int? Rank { get; set; }
        public string Name { get; set; }
        public string Callcode { get; set; }
        public string Market { get; set; }
        public ObjectId MetroAreaId { get; set; }

        [BsonElement("Type")]
        public string SType { get; set; }
        public string Owner { get; set; }
        public string Frequency { get; set; }
        public string Format { get; set; }

        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public SortedSet<string> Countries { get; set; } = new SortedSet<string>();

        public string LatDirection { get; set; }
        public int? LatDegrees { get; set; }
        public int? LatMinutes { get; set; }
        public float? LatSeconds { get; set; }
        public string LngDirection { get; set; }
        public int? LngDegrees { get; set; }
        public int? LngMinutes { get; set; }
        public float? LngSeconds { get; set; }
        public String Licensee { get; set; }
        public int? FCCAppId { get; set; }

        public string CoverageMap { get; set; }
        public GeoJson2DGeographicCoordinates LngLat { get; set; }

        public bool IsShowInviteLink{ get; set; }
        public User CompanyAdmin { get; set; }
        public ObjectId CompanyId { get; set; }

    }
}
