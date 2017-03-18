using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.GeoJsonObjectModel;
using System;


namespace CrankExportCSVToMongo.Models
{
    /// <summary>
    ///  "id","name","country","population","capital","geom"
    /// </summary>
    public class world_cityborder
    {
        [BsonId]
        [BsonElement("Id")]
        public int id { get; set; }

        [BsonElement("Name")]
        public string name { get; set; }

        [BsonElement("Country")]
        public string country { get; set; }


        [BsonElement("Population")]
        public double population { get; set; }

        [BsonElement("Capital")]
        public string capital { get; set; }

        [BsonIgnore]
        public string st_asewkt { get; set; }


        [BsonElement("Geom")]
        public GeoJson2DGeographicCoordinates geom { set { value = new GeoJson2DGeographicCoordinates(0, 0); } get { return new GeoJson2DGeographicCoordinates(Program.GetLatitude(st_asewkt), Program.GetLongitude(st_asewkt)); } }


    }
}
