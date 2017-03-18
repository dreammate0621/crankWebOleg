using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.GeoJsonObjectModel;
using System;


namespace Crankdata.Models
{

    public class WorldCityBorder
    {
        [BsonId]
        public int Id { get; set; }

        public string Name { get; set; }


        public string Country { get; set; }

        public double Population { get; set; }

        public string Capital { get; set; }

        public GeoJson2DGeographicCoordinates Geom { set { value = new GeoJson2DGeographicCoordinates(0, 0); } get { return new GeoJson2DGeographicCoordinates(Program.GetLatitude(st_asewkt), Program.GetLongitude(st_asewkt)); } }


    }
}
