using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crankdata.Models
{
    [BsonIgnoreExtraElements]
    public class ArtistImage
    {
        [BsonId]
        [BsonIgnoreIfDefault]
        public ObjectId Id { get; set; }
        public ObjectId ArtistId { get; set; }

        public List<ImageThumbnail> Images = new List<ImageThumbnail>();
       
    }
    [BsonIgnoreExtraElements]
    public class ImageThumbnail
    {
        public string Size { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Source { get; set; }
        public string MimeType { get; set; } = "image/png";
        public DateTime LastLoaded { get; set; } = DateTime.Now;
        public byte[] data { get; set; }
    }
}
