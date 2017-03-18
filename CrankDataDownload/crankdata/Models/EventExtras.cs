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
    public class EventExtras
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public ObjectId EventId { get; set; }
        public List<AssociationInfo> Associations = new List<AssociationInfo>();
        public List<MarketPromotionInfo> MarketPromotions = new List<MarketPromotionInfo>();
        //public List<PromotionInfo> Promotions = new List<PromotionInfo>();
        //public List<MarketInfo> SubMarkets = new List<MarketInfo>();
        //public List<ObjectId> SubMarkets = new List<ObjectId>();
        public bool isEditable { get; set; }
    }

    public class AssociationInfo
    {
        public ObjectId AssignedBy { get; set; }
        public ObjectId AssignedTo { get; set; }
        public string AssignedAs { get; set; } // artist_manager, radio_manager, agency, promoter, label, digital
        public DateTime CreatedDate { get; set; }
        public DateTime LastUpdatedDate { get; set; }
    }

    public class MarketPromotionInfo
    {
        public ObjectId MetroAreaId { get; set; }
        public List<PromoStationInfo> PromoStations = new List<PromoStationInfo>();
    }

    public class PromoStationInfo
    {
        public ObjectId StationId { get; set; }
        public List<ObjectId> SelectedBy = new List<ObjectId>();
        public List<PromotionInfo> Promotions = new List<PromotionInfo>();
    }

    public class PromotionInfo
    {
        public string Type { get; set; } // ticket, appearance, interview
        public List<PromotionInstanceInfo> PromoInstances = new List<PromotionInstanceInfo>();
    }

    public class PromotionInstanceInfo
    {
        public int? TypeCount { get; set; } // Number associated with promotion
        public ObjectId AssignedBy { get; set; } // User who defined the promotion
        public DateTime CreatedDate { get; set; }
        public DateTime LastUpdatedDate { get; set; }

    }

    //public class MarketInfo
    //{
    //    public ObjectId MetroAreadId { get; set; } 
    //    public List<ObjectId> SelectedStations = new List<ObjectId>();
    //    public ObjectId AssignedBy { get; set; }
    //    public DateTime CreatedDate { get; set; }
    //    public DateTime LastUpdatedDate { get; set; }
    //}
}
