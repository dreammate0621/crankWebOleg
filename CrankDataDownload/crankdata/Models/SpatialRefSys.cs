using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;


namespace CrankExportCSVToMongo.Models
{
    /// <summary>
    /// "srid","auth_name","auth_srid","srtext","proj4text"
    /// </summary>
    public class SpatialRefSys
    {               
        [BsonId]
        
        public int SrId { get; set; }

        
        public string AuthName { get; set; }

        
        public string AuthSrId { get; set; }

        
        public string SrText { get; set; }

        
        public string Proj4Text { get; set; }
    }
}
