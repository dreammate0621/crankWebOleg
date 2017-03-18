using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.GeoJsonObjectModel;
using System;

namespace CrankExportCSVToMongo.Models
{
    class world_worldborder
    {
        [BsonId]
        [BsonElement("Id")]
        public int id { get; set; }


        [BsonElement("Fips")]
        public string fips { get; set; }


        [BsonElement("ISO2")]
        public string iso2 { get; set; }


        [BsonElement("ISO3")]
        public string iso3 { get; set; }
        [BsonElement("UN")]
        public int un { get; set; }
        [BsonElement("Name")]
        public string name { get; set; }
        [BsonElement("Area")]
        public int area { get; set; }
        [BsonElement("Pop2005")]
        public int pop2005 { get; set; }
        [BsonElement("Region")]
        public int region { get; set; }
        [BsonElement("SubRegion")]
        public int subregion { get; set; }
        [BsonElement("Lon")]
        public double lon { get; set; }
        [BsonElement("Lat")]
        public double lat { get; set; }

        [BsonIgnore]
        public string geomaterypolygon { get; set; }
        [BsonElement("Geom")]
        public GeoJsonMultiPolygon<GeoJson2DGeographicCoordinates> geom { set { value = null; } get { return Program.GetMultiPolyGon(geomaterypolygon); } }

    }
}
