using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crankdata.Models
{
    public class EventExtrasViewModel
    {
        public string Id { get; set; }
        public string EventId { get; set; }
        public List<AssociationInfoViewModel> Associations = new List<AssociationInfoViewModel>();
        public List<MarketPromotionInfoViewModel> MarketPromotions = new List<MarketPromotionInfoViewModel>();
//        public List<MarketInfoViewModel> SubMarkets = new List<MarketInfoViewModel>();
    }

    public class AssociationInfoViewModel
    {
        public string AssignedBy { get; set; }
        public string AssignedTo { get; set; }
        public string AssignedAs { get; set; } // artist_manager, radio_manager, agency, promoter, label, digital
        public DateTime LastUpdatedDate { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class MarketPromotionInfoViewModel
    {
        public string MetroAreaId { get; set; }
        public List<PromoStationInfoViewModel> PromoStations = new List<PromoStationInfoViewModel>();
    }

    public class PromoStationInfoViewModel
    {
        public string StationId { get; set; }
        public List<string> SelectedBy = new List<string>();
        public List<PromotionInfoViewModel> Promotions = new List<PromotionInfoViewModel>();
    }
    public class PromotionInfoViewModel
    {
        public string Type { get; set; } // ticket, appearance, interview
        public List<PromotionInstanceInfoViewModel> PromoInstances = new List<PromotionInstanceInfoViewModel>();
    }

    public class PromotionInstanceInfoViewModel
    {
        public int? TypeCount { get; set; } // Number associated with promotion
        public string AssignedBy { get; set; } // User who defined the promotion
        public DateTime LastUpdatedDate { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    //public class PromotionInfoViewModel
    //{
    //    public string Type { get; set; } // ticket, appearance, interview
    //    public int? TypeCount { get; set; } // Number associated with promotion
    //    public string AssignedBy { get; set; } // User who defined the promotion
    //    public DateTime LastUpdatedDate { get; set; }

    //}
    //public class PromotionInfoViewModel
    //{
    //    public string Type { get; set; } // ticket, appearance, interview
    //    public int? TypeCount { get; set; } // Number associated with promotion
    //    public string DefinedBy { get; set; } // User who defined the promotion
    //    public string AssignedTo { get; set; } // StationId
    //}
    //public class MarketInfoViewModel
    //{
    //    public string MetroAreadId { get; set; }
    //    public List<string> SelectedStations = new List<string>();
    //    public ObjectId AssignedBy { get; set; }
    //    public DateTime CreatedDate { get; set; }
    //    public DateTime LastUpdatedDate { get; set; }
    //}
}
