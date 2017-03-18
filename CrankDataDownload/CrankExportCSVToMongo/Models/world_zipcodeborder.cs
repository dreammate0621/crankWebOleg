using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.GeoJsonObjectModel;
using System;

namespace CrankExportCSVToMongo.Models
{
    /// <summary>
    /// "id","zip","po_name","state","sumblkpop","pop2007","pop07_sqmi","sqmi","oid","geom"
    /// </summary>
    public class world_zipcodeborder
    {
        [BsonId]
        [BsonElement("Id")]
        public int id { get; set; }

        [BsonElement("Zip")]
        public string zip { get; set; }

        [BsonElement("POName")]
        public string po_name { get; set; }

        [BsonElement("State")]
        public string state { get; set; }

        [BsonElement("SumbLkPop")]
        public int sumblkpop { get; set; }

        [BsonElement("Pop2007")]
        public int pop2007 { get; set; }

        [BsonElement("Pop07Sqmi")]
        public double pop07_sqmi { get; set; }

        [BsonElement("Sqmi")]
        public double sqmi { get; set; }

        [BsonElement("Oid")]
        public int oid { get; set; }

        [BsonIgnore]
        public string st_astext { get; set; }
        [BsonElement("Geom")]
        public GeoJsonMultiPolygon<GeoJson2DGeographicCoordinates> geom { set { value = null; } get { return Program.GetMultiPolyGon(st_astext); } }

    }
}
