using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;



namespace CrankExportCSVToMongo.Models
{
    public class scraper_agency
    {
        [BsonId]        
        public int Id { get; set; }
        
        public string Title { get; set; }
    }
}
