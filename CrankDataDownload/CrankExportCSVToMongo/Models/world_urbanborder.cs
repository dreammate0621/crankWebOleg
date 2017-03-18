using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.GeoJsonObjectModel;
using System;


namespace CrankExportCSVToMongo.Models
{
    /// <summary>
    /// "id","ua_id","name","lsad","lsad_desc","pop2000","pop00_sqmi","households","hse_units","sqmi","oid","geom"
    /// </summary>
    public class world_urbanborder
    {
        [BsonId]
        [BsonElement("Id")]
        public int id { get; set; }
        [BsonElement("UaId")]
        public string ua_id { get; set; }

        [BsonElement("Name")]
        public string name { get; set; }

        [BsonElement("Lsad")]
        public string lsad { get; set; }

        [BsonElement("LsadDesc")]
        public string lsad_desc { get; set; }

        [BsonElement("Pop2000")]
        public int pop2000 { get; set; }

        [BsonElement("Pop00Sqmi")]
        public double pop00_sqmi { get; set; }

        [BsonElement("Households")]
        public int households { get; set; }

        [BsonElement("HseUnits")]
        public int hse_units { get; set; }

        [BsonElement("Sqmi")]
        public double sqmi { get; set; }

        [BsonElement("Oid")]
        public int oid { get; set; }


        [BsonIgnore]
        public string geomaterypolygon { get; set; }

        [BsonElement("Geom")]
        public GeoJsonMultiPolygon<GeoJson2DGeographicCoordinates> geom { set { value = null; } get { return Program.GetMultiPolyGon(geomaterypolygon); } }



    }
}
