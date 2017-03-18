using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownloadFCCFMStationData.Models
{
    public class FCCFMStationInfo
    {
        public string Call { get; set; }
        public string Frequency { get; set; }
        public string Service { get; set; }
        public int Channel { get; set; }
        public string Class { get; set; }
        public string Status { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string FileNumber { get; set; }
        public int FacilityID { get; set; }
        public string LatDirection { get; set; }
        public int LatDegrees { get; set; }
        public int LatMinutes { get; set; }
        public float LatSeconds { get; set; }
        public string LngDirection { get; set; }
        public int LngDegrees { get; set; }
        public int LngMinutes { get; set; }
        public float LngSeconds { get; set; }
        public String Licensee { get; set; }

        public int FCCAppId { get; set; }

        public string CoverageMap { get; set; }
    }


    //CSV Mapper for csv parser

    public sealed class FCCFMStationInfoMap: CsvClassMap<FCCFMStationInfo>
    {
        public FCCFMStationInfoMap()
        {
            Map(m => m.Call).Index(1);
            Map(m => m.Frequency).Index(2);
            Map(m => m.Service).Index(3);
            Map(m => m.Channel).Index(4);
            Map(m => m.Class).Index(7);
            Map(m => m.Status).Index(9);
            Map(m => m.City).Index(10);
            Map(m => m.State).Index(11);
            Map(m => m.Country).Index(12);
            Map(m => m.FileNumber).Index(13);
            Map(m => m.FacilityID).Index(18);
            Map(m => m.LatDirection).Index(19);
            Map(m => m.LatDegrees).Index(20);
            Map(m => m.LatMinutes).Index(21);
            Map(m => m.LatSeconds).Index(22);
            Map(m => m.LngDirection).Index(23);
            Map(m => m.LngDegrees).Index(24);
            Map(m => m.LngMinutes).Index(25);
            Map(m => m.LngSeconds).Index(26);
            Map(m => m.Licensee).Index(27);
            Map(m => m.FCCAppId).Index(37);
        }
    }
}
