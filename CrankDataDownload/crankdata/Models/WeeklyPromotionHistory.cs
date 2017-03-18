using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crankdata.Models
{
    public class WeeklyPromotionHistory
    {
        public DateTime StartWeekDate { get; set; }
        public List<PromoStationInfo>  PormotionStations = new List<PromoStationInfo>();
        public int TicketPromotionCount { get; set; }
        public int AppearancePromotionCount { get; set; }
        public int MeetnGreetPromotionCount { get; set; }
        public int InterviewPromotionCount { get; set; }
    }
}
