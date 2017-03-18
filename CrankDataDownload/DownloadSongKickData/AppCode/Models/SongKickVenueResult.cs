using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloadSongKickData.AppCode.Models
{
    public class SongKickVenueResults
    {
        public string Status { get; set; }
        public SongKickVenue Results { get; set; }
    }

    public  class SongKickVenue
    {
        public SKVenue Venue { get; set; }
    }
    public class SongKickVenueResultsPage
    {
        public SongKickVenueResults ResultsPage { get; set; }
    }
}
