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
    public class ModuleImage
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public byte[] Logo { get; set; }
    }
}
