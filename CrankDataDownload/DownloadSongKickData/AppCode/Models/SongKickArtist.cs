using System;
using System.Collections.Generic;

namespace DownloadSongKickData.AppCode.Models
{
    public class SongKickArtist
    {
        public string DisplayName { get; set; }
        public string Uri { get; set; }

        public List<ArtistIdentifier> Identifier { get; set; }
        public DateTime? OnTourUntil { get; set; }

        public int Id { get; set; } = -1;
    }

    public class ArtistIdentifier
    {
        public string EventsHref { get; set; }
        public string SetlistsHref { get; set; }
        public string Mbid { get; set; }
        public string Href { get; set; }
    }
}

