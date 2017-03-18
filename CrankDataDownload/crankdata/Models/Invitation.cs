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
    public class Invitation
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public ObjectId UserId { get; set; }
        public string Email { get; set; }
        public DateTime SentDateTime { get; set; }
        public Guid GUID { get; set; }
        public long ExpiryInSecs { get; set; }
        public DateTime? registeredDateTime { get; set; }
        public bool? registered { get; set; }
        public ObjectId ComapnyId { get; set; }

    }
}
