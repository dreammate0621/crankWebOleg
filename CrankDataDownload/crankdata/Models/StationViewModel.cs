using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crankdata.Models
{
    public class StationViewModel
    {
        public string Id { get; set; }

        public string Name { get; set; }
        public int? Rank { get; set; }
        public string Callcode { get; set; }
        public string SType { get; set; }
        public string Market { get; set; }
        public string Group { get; set; }
        public string ContactInfo { get; set; }
        public string Frequency { get; set; }
        public string Format { get; set; }
        public string Owner { get; set; }
        public string AQH { get; set; }
        public string Phone { get; set; }
        public string Alias { get; set; }
    }
}
