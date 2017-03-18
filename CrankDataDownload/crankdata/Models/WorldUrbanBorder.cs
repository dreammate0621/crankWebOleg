using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.GeoJsonObjectModel;
using System;


namespace Crankdata.Models
{
    /// <summary>
    /// "id","ua_id","name","lsad","lsad_desc","pop2000","pop00_sqmi","households","hse_units","sqmi","oid","geom"
    /// </summary>
    public class WorldUrbanBorder
    {
        [BsonId]

        public int Id { get; set; }


        public string UaId { get; set; }


        public string Name { get; set; }


        public string Lsad { get; set; }


        public string LsadDesc { get; set; }


        public int Pop2000 { get; set; }


        public double Pop00Sqmi { get; set; }


        public int Households { get; set; }


        public int HseUnits { get; set; }


        public double Sqmi { get; set; }


        public int Oid { get; set; }

        public GeoJsonMultiPolygon<GeoJson2DGeographicCoordinates> Geom { set { value = null; } get { return Program.GetMultiPolyGon(geomaterypolygon); } }



    }
}
