using System;
using System.Collections.Generic;

namespace DownloadSongKickData.AppCode.Models
{
    public class SongKickEvent
    {
        public string Type { get; set; }
        public string Status { get; set; }
        public double? Popularity { get; set; }

        public string DisplayName { get; set; }

        public SKEventDate Start { get; set; }

        public string AgeRestriction { get; set; }

        public SKLocation Location { get; set; }

        public string Uri { get; set; }
        public int Id { get; set; }

        public List<SKPerformance> Performance { get; set; }

        public SKVenue Venue { get; set; }

    }

    public class SKVenue
    {
        public string DisplayName { get; set; }
        public double? Lat { get; set; }
        public double? Lng { get; set; }
        public string Uri { get; set; }
        public int? Id { get; set; }

        public string Street { get; set; }

        public string Phone { get; set; }
        public string Website { get; set; }

        public int? Capacity { get; set; }

        public string Description { get; set; }
        public SKCity City { get; set; }
        public SKMetroArea MetroArea { get; set; }
    }

    public class SKMetroArea
    {
        public string DisplayName { get; set; }
        public string Uri { get; set; }
        public int Id { get; set; }
        public SKCountry Country { get; set; }
        public SKState State { get; set; }
    }

    public class SKState
    {
        public string DisplayName { get; set; }
    }

    public class SKCountry
    {
        public string DisplayName { get; set; }

    }

    public class SKPerformance
    {
        public int billingIndex { get; set; }
        public string DisplayName { get; set; }
        public int Id { get; set; }
        public string Billing { get; set; }

        public SongKickArtist Artist { get; set; }
    }

    public class SKLocation
    {
        public string City { get; set; }
        public double? Lat { get; set; }
        public double? Lng { get; set; }
    }

    public class SKEventDate
    {
        public DateTime? DateTime { get; set; }
        public TimeSpan? Time { get; set; }
        public DateTime? Date { get; set; }
    }

    public class SKCity
    {
        public string DisplayName { get; set; }
        public string Uri { get; set; }
        public int Id { get; set; }
        public SKCountry Country { get; set; }
        public SKState State { get; set; }
    }
}