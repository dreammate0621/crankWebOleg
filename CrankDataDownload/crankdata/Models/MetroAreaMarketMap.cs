using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crankdata.Models
{
    public class MetroAreaMarketMap
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public ObjectId MetroAreaId { get; set; }
        public string Name { get; set; }
        public List<string> aliases = new List<string>();
    }
}
