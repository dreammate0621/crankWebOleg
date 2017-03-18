using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloadSongKickArtistImage.AppCode.Models
{
    public class StationThumbnail
    {
        public string Name { get; set; }

        public List<Thumbnail> Thumbnails = new List<Thumbnail>();
    }

    public class Thumbnail
    {
        public string Size { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public string Source { get; set; }
        public string MimeType { get; set; }
        public byte[] data { get; set; }
    }
}
