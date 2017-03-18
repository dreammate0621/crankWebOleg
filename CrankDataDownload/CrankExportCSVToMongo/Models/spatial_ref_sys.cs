using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;


namespace CrankExportCSVToMongo.Models
{
    /// <summary>
    /// "srid","auth_name","auth_srid","srtext","proj4text"
    /// </summary>
    public class spatial_ref_sys
    {               
        [BsonId]
        [BsonElement("SrId")]
        public int srid { get; set; }

        [BsonElement("AuthName")]
        public string auth_name { get; set; }

        [BsonElement("AuthSrId")]
        public string auth_srid { get; set; }

        [BsonElement("SrText")]
        public string srtext { get; set; }

        [BsonElement("Proj4Text")]
        public string proj4text { get; set; }
    }
}
