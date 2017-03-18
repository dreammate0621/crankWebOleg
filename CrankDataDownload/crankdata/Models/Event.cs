using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using MongoDB.Driver.GeoJsonObjectModel;
using System.Collections.Generic;

namespace Crankdata.Models
{
    [BsonIgnoreExtraElements]
    public class Event
    {
        [BsonId]
        [BsonIgnoreIfDefault]
        public ObjectId Id { get; set; }

        public string Name { get; set; }

        public string EventType { get; set; }

        public double? Popularity { get; set; }

        public string Status { get; set; }

        public DateTime? StartDateTime { get; set; }

        [BsonDateTimeOptions(DateOnly = true)]
        public DateTime? StartDate { get; set; }

        public TimeSpan? StartTime { get; set; }

        public string AgeRestriction { get; set; }
        public string City { get; set; }

        public GeoJson2DCoordinates LngLat { get; set; }

        public int EventSKId { get; set; }

        public Venue EventVenue { get; set; }

        public List<Performance> Performance { get; set; }

        public bool isEditable { get; set; }
    }
}
