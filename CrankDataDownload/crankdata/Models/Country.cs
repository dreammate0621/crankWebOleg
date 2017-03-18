using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Crankdata.Models
{
    [BsonIgnoreExtraElements]
    public class Country
    {
        [BsonId]
        public ObjectId _id { get; set; }
        public string id { get; set; }
        public string fips { get; set; }
        public string iso2 { get; set; }
        public string iso3 { get; set; }
        public string un { get; set; }
        public string name { get; set; }
        public long area { get; set; }
        public long pop2005 { get; set; }
        public long region { get; set; }
        public long subregion { get; set; }
        public decimal lon { get; set; }
        public decimal lat { get; set; }
        public string geom { get; set; }
    }
}
