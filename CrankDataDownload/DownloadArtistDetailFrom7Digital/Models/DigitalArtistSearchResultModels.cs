using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloadArtistDetailFrom7Digital
{
    // use for search by name 
    public class DigitalArtistSearchResult
    {
        public string Status { get; set; }
        public string Version { get; set; }
        public ArtistSearchResult SearchResults { get; set; }

    }

    // use for search by id 
    public class DigitalArtistDetailResult
    {
        public string Status { get; set; }
        public string Version { get; set; }
        public DigitalArtistDetail Artist { get; set; }

    }

    public class ArtistSearchResult
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public List<DigitalArtist> SearchResult { get; set; }
    }

    public class DigitalArtist
    {
        public string Type { get; set; }
        public double Score { get; set; }
        public DigitalArtistDetail Artist { get; set; }

    }

    public class DigitalArtistDetail
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SortName { get; set; }
        public string Url { get; set; }
        public string Slug { get; set; }
        public string Image { get; set; }
        public string AppearsAs { get; set; }
        public double Popularity { get; set; }
        public ArtistBio Bio { get; set; }

    }

    public class ArtistBio
    {
       public string Text { get; set; }
    }
}
