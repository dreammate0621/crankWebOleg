using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace Crankdata.Models
{
    public class UserReqModel
    {
        public string Id { get; set; }
        public string Userid { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string LastLogin { get; set; }
        public bool IsSuperUser { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsActive { get; set; }
        public int LoginAttemptCount { get; set; }
        public bool IsLocked { get; set; }
        public string JoinDate { get; set; }
        public string UserType { get; set; }
        public bool IsStaff { get; set; }
        public string Location { get; set; }
        public string CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string StationId { get; set; }
        public SortedSet<string> Venues { get; set; } = new SortedSet<string>();
        public SortedSet<string> Artists { get; set; } = new SortedSet<string>();
        public SortedSet<string> Stations { get; set; } = new SortedSet<string>();
        public SortedSet<string> Events { get; set; } = new SortedSet<string>();
        public SortedSet<string> Team { get; set; } = new SortedSet<string>();
        public SortedSet<string> ConnectedUsers { get; set; } = new SortedSet<string>();
        public SortedSet<string> Digitals { get; set; } = new SortedSet<string>();
        public SortedSet<string> Modules { get; set; } = new SortedSet<string>();
    }
}