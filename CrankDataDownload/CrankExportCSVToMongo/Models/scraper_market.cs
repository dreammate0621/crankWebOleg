using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.GeoJsonObjectModel;
using System;

namespace CrankExportCSVToMongo.Models
{
    public class scraper_market
    {               
        [BsonId]
        [BsonElement("Id")]
        public int id { get; set; }

        [BsonElement("Name")]
        public string name { get; set; }

        [BsonIgnore]
        public string st_asewkt { get; set; }

        [BsonElement("Location")]
        public GeoJson2DCoordinates location { set { value = new GeoJson2DCoordinates(0, 0); } get { return new GeoJson2DCoordinates(Program.GetLatitude(st_asewkt), Program.GetLongitude(st_asewkt)); } }
        
    }
}
