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
    public class User
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string Userid { get; set; }
        public string Email { get; set; }
        public DateTime LastLogin { get; set; }
        public bool IsSuperUser { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsActive { get; set; }
        public bool IsLocked { get; set; }
        public string UserType { get; set; }
        public bool IsStaff { get; set; }
        public ObjectId CompanyId { get; set; }
    }
}