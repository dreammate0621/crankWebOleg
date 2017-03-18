using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using MongoDB.Driver.GeoJsonObjectModel;
using System.Collections.Generic;

namespace Crankdata.Models
{
    [BsonIgnoreExtraElements]
    public class EventSummary
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public VenueSummary EventVenue { get; set; }
        public List<PerformanceSummary> Performance { get; set; }
        public DateTime? StartDate { get; set; }
        public string City { get; set; }
        public string Associations { get; set; }
        public bool isEditable { get; set; }
        public bool HasAppearnce { get; set; }
        public bool HasTicket { get; set; }
        public bool HasMeetNGreet { get; set; }
        public bool HasInterview { get; set; }
        public bool IsDisplay { get; set; } = true;
        public List<WeeklyPromotionHistory> weeklyPromoHistory = new List<WeeklyPromotionHistory>();
        public int TotalPromotionCount { get; set; }
        public GeoJson2DCoordinates LngLat { get; set; }
    }
}
