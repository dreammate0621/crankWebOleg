using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseMediabaseData.Models
{
    class StationRank
    {
        public int Rank { get; set; }
        public string Market { get; set; }
        public string Station { get; set; }
        public string Format { get; set; }
        public string AQH { get; set; }
        public string Owner { get; set; }
        public string Phone { get; set; }
        public DateTime? FirstMonitored { get; set; }
    }
}
