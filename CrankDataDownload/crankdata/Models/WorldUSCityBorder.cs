using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.GeoJsonObjectModel;
using System;

namespace Crankdata.Models
{
    /// <summary>
    /// "id","feature","name","st_abbrev","fips","place_fips","pop_2000","pop2007","status","oid","geom"
    /// </summary>
    public class WorldUSCityBorder
    {
        [BsonId]
        public int Id { get; set; }


        public string Feature { get; set; }


        public string Name { get; set; }


        public string StAbbrev { get; set; }


        public string Fips { get; set; }


        public string PlaceFips { get; set; }


        public int Pop2000 { get; set; }


        public int Pop2007 { get; set; }


        public string Status { get; set; }

        [BsonElement("Oid")]
        public int Oid { get; set; }




        public GeoJson2DGeographicCoordinates Geom { set { value = new GeoJson2DGeographicCoordinates(0, 0); } get { return new GeoJson2DGeographicCoordinates(Program.GetLatitude(st_asewkt), Program.GetLongitude(st_asewkt)); } }

    }
}
