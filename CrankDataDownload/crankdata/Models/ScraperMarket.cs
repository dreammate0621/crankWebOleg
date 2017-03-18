using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.GeoJsonObjectModel;
using System;

namespace CrankExportCSVToMongo.Models
{
    public class scraper_market
    {               
        [BsonId]       
        public int Id { get; set; }
        public string Name { get; set; }     
        
        public GeoJson2DCoordinates Location { set { value = new GeoJson2DCoordinates(0, 0); } get { return new GeoJson2DCoordinates(Program.GetLatitude(st_asewkt), Program.GetLongitude(st_asewkt)); } }
        
    }
}
