using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;



namespace CrankExportCSVToMongo.Models
{
    public class scraper_agency
    {
        [BsonId]
        [BsonElement("Id")]
        public int id { get; set; }

        [BsonElement("Title")]
        public string title { get; set; }
    }
}
