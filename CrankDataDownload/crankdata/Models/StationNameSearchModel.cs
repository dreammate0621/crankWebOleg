using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crankdata.Models
{
    [BsonIgnoreExtraElements]
    public class StationNameSearchModel
    {
        public ObjectId Id { get; set; }

        public string Name { get; set; }
        public int Rank { get; set; }
        public string Callcode { get; set; }
        public string SType { get; set; }
        public string Market { get; set; }
        public string Format { get; set; }
    }
}
