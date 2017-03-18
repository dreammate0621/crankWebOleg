using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloadArtistDetailFrom7Digital
{
    public class ArtistThumbnail
    {
        public int Id { get; set; } = -1;
        public string Title { get; set; }

        public List<Thumbnail> Thumbnails = new List<Thumbnail>();
    }

    public class Thumbnail
    {
        public string Size { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public string Source { get; set; }
        public byte[] data { get; set; }
    }
}
