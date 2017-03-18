using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace Crankdata.Models
{
    [BsonIgnoreExtraElements]
    public class UserSummary
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Location { get; set; }
        public ObjectId CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string UserType { get; set; }
        public bool IsActive { get; set; }
        public bool IsLocked { get; set; }
    }
}