using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crankdata.Models
{
    public class SongViewModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public string ArtistId { get; set; }
        public string Charts { get; set; }
        public string OrigTitle { get; set; }
        public string OrigLabels { get; set; }

        public List<string> Labels { get; set; } = new List<string>();
        public string CreatedDate { get; set; }
        public List<string> SubArtists { get; set; } = new List<string>();
        public SongStats Stats { get; set; } = new SongStats();
    }
}