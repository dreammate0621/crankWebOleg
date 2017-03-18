using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloadSongKickData.AppCode.Models
{
    public class SongKickArtistResult
    {
        public string Status { get; set; }
        public int PerPage { get; set; }
        public int Page { get; set; }
        public int TotalEntries { get; set; }
        public SongKickArtistList Results { get; set; }
    }

    public class SongKickArtistList
    {
        public List<SongKickArtist> Artist { get; set; }

    }

    public class SongKickArtistResultsPage
    {
        public SongKickArtistResult ResultsPage { get; set; }
    }
}
