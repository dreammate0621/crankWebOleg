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
    public class PermissionSet
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        //public List<Role> roles = new List<Role>();
        public List<Permission> permissions = new List<Permission>();
    }

    /*public class Role
    {
        public string Name { get; set; }
        public List<Permission> permissions = new List<Permission>();
    }*/

    public class Permission
    {
        public string name { get; set; }
        public string value { get; set; }
    }

    public class UserPermissionSetMap
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public ObjectId UserId { get; set; }
        public List<PermissionSet> permissionSets = new List<PermissionSet>();
    }
}
