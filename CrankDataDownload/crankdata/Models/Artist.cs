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
    public class Artist
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public Guid Mbid { get; set; }
        public int? ArtistSKId { get; set; }
        public string ArtistSKDisplayName { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string SortName { get; set; }
        [BsonElement("Type")]
        public string SType { get; set; }
        public string Gender { get; set; }
        [BsonDateTimeOptions(DateOnly = true)]
        public DateTime? DOB { get; set; }
        public string Location { get; set; }
        public string IPICode { get; set; }
        public string ISNICode { get; set; }
        public string Description { get; set; }
        public string Bio { get; set; }
        public List<string> Alias { get; set; }
    }

    public class ArtistMinInfo
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public int? ArtistSKId { get; set; }
        public string Title { get; set; }
        public string ArtistSKDisplayName { get; set; }
    }
}
