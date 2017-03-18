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
    public class APIAccessKey
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public ObjectId CompanyId { get; set; }
        public Guid APIKey { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public List<API> APIs = new List<API>();
    }

    public class API
    {
        public string Endpoint { get; set; }
        public DateTime IssueDate { get; set; }
        public ObjectId SentToUserId { get; set; }
        public ObjectId SentByUserId { get; set; }
    }
}
