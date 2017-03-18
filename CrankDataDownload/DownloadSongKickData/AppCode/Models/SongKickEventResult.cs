using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloadSongKickData.AppCode.Models
{
    public class SongKickEventResult
    {
        public string Status { get; set; }
        public int PerPage { get; set; }
        public int Page { get; set; }
        public int TotalEntries { get; set; }
        public SongKickEventList Results { get; set; }

    }

    public class SongKickEventList
    {
        public List<SongKickEvent> Event { get; set; }
    }

    public class SongKickEventResultsPage
    {
        public SongKickEventResult ResultsPage { get; set; }
    }
}
