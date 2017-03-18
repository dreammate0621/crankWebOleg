using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.GeoJsonObjectModel;
using System;

namespace CrankExportCSVToMongo.Models
{
    /// <summary>
    /// "id","feature","name","st_abbrev","fips","place_fips","pop_2000","pop2007","status","oid","geom"
    /// </summary>
    public class world_uscityborder
    {
        [BsonId]
        [BsonElement("Id")]
        public int id { get; set; }

        [BsonElement("Feature")]
        public string feature { get; set; }

        [BsonElement("Name")]
        public string name { get; set; }

        [BsonElement("StAbbrev")]
        public string st_abbrev { get; set; }

        [BsonElement("Fips")]
        public string fips { get; set; }

        [BsonElement("PlaceFips")]
        public string place_fips { get; set; }

        [BsonElement("Pop2000")]
        public int pop_2000 { get; set; }

        [BsonElement("Pop2007")]
        public int pop2007 { get; set; }

        [BsonElement("Status")]
        public string status { get; set; }

        [BsonElement("Oid")]
        public int oid { get; set; }
        [BsonIgnore]
        public string st_asewkt { get; set; }


        [BsonElement("Geom")]
        public GeoJson2DGeographicCoordinates geom { set { value = new GeoJson2DGeographicCoordinates(0, 0); } get { return new GeoJson2DGeographicCoordinates(Program.GetLatitude(st_asewkt), Program.GetLongitude(st_asewkt)); } }

    }
}
