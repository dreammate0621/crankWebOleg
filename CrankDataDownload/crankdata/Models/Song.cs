using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crankdata.Models
{
    public class Song
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public ObjectId ArtistId { get; set; }
        public string Charts { get; set; }
        public string OrigTitle { get; set; }
        public string OrigLabels { get; set; }

        public SortedSet<string> Labels { get; set; } = new SortedSet<string>();

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public SortedSet<string> SubArtists { get; set; } = new SortedSet<string>();

        public SortedSet<SongStats> Stats { get; set; } = new SortedSet<SongStats>();
    }

    public class SongStats: IComparable<SongStats>
    {
        public string Format { get; set; }
        public SpinStats SpinStat { get; set; } = new SpinStats();

        public int CompareTo(SongStats obj)
        {
            return this.Format.CompareTo(obj.Format);
        }
    }

    public class SpinStats
    {
        public int Rank { get; set; }
        public int PeakRank { get; set; }
        public int RankLastWeek { get; set; }
        public int PeakSpins { get; set; }
        public int Spins { get; set; }
        public int OverNightSpins { get; set; }
        public int AMSpins { get; set; }
        public int MidSpins { get; set; }
        public int PMSpins { get; set; }
        public int EveningSpins { get; set; }
        public int StationsOn { get; set; }
        public int NewStations { get; set; }

    }
}
