using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.GeoJsonObjectModel;
using System;

namespace Crankdata.Models
{
    class WorldWorldBorder
    {
        [BsonId]

        public int Id { get; set; }



        public string Fips { get; set; }



        public string ISO2 { get; set; }



        public string ISO3 { get; set; }


        public int UN { get; set; }

        public string Name { get; set; }


        public int Area { get; set; }



        public int Pop2005 { get; set; }


        public int Region { get; set; }


        public int SubRegion { get; set; }


        public double Lon { get; set; }

        public double Lat { get; set; }



        public GeoJsonMultiPolygon<GeoJson2DGeographicCoordinates> Geom { set { value = null; } get { return Program.GetMultiPolyGon(geomaterypolygon); } }

    }
}
