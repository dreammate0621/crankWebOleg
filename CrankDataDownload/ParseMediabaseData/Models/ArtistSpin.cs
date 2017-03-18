using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseMediabaseData.Models
{
    public class ArtistSpin
    {
        public string PeakRank { get; set; }
        public string RankLW { get; set; }
        public int RankTW { get; set; }
        public string Artist { get; set; }
        public string Title { get; set; }
        public string Label { get; set; }
        public int Peak { get; set; }
        public int TW { get; set; }
        public int LW { get; set; }
        public int Move { get; set; }
        public int OVN { get; set; }
        public int AMD { get; set; }
        public int MID { get; set; }
        public int PMD { get; set; }
        public int EVE { get; set; }
        public int StationsOn { get; set; }
        public int NewStations { get; set; }

    }

    public sealed class ArtistSpinMap: CsvClassMap<ArtistSpin>
    {
        public ArtistSpinMap()
        {

        }
    }
}
