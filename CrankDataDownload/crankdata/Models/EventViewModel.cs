using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crankdata.Models
{
    public class EventViewModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string EventType { get; set; }

        public double? Popularity { get; set; }

        public string StartDate { get; set; }

        public string AgeRestriction { get; set; }
        public string City { get; set; }

        public double? Lng { get; set; }
        public double? Lat { get; set; }

        public VenueViewModel EventVenue { get; set; }

        public List<PerformanceViewModel> Performance { get; set; }
    }
}
