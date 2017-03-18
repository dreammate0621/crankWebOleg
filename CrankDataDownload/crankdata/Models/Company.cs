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
    public class Company
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public List<string> OtherNames = new List<string>();
        public List<Address> Addresses = new List<Address>();
        public List<Email> Emails = new List<Email>();
        public List<Phone> Phones = new List<Phone>();
        public List<Phone> Faxes = new List<Phone>();
        public List<ObjectId> Admins = new List<ObjectId>();
        public List<string> References = new List<string>();
        public bool isActive { get; set; }
        public bool HasApiKeySent { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public List<ImageThumbnail> Images = new List<ImageThumbnail>();
    }

    public class Address
    {
        public string Type { get; set; } // primary, secondary
        public string Number { get; set; }
        public List<string> Streets = new List<string>();
        public string Landmark { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Zipcode { get; set; }
    }
    public class Email
    {
        public string Type { get; set; } // primary, secondary
        public string EmailId { get; set; }
    }

    public class Phone
    {
        public string Type { get; set; } // work, cell, home
        public long? CountryCode { get; set; }
        public long? AreaCode { get; set; }
        public long Number { get; set; }
    }
}
