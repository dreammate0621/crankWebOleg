using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Crankdata.Models
{
    [BsonIgnoreExtraElements]
    public class ArtistNameSearchModel
    {
            [BsonId]
            public ObjectId Id { get; set; }
            public Guid Mbid { get; set; }
            public string Title { get; set; }
            public string FirstName { get; set; }
            public string MiddleName { get; set; }
            public string LastName { get; set; }
            public string SortName { get; set; }
            public string SType { get; set; }
    }
}
