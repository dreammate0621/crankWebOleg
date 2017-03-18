using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.GeoJsonObjectModel;
using System;

namespace Crankdata.Models
{
    /// <summary>
    /// "id","zip","po_name","state","sumblkpop","pop2007","pop07_sqmi","sqmi","oid","geom"
    /// </summary>
    public class WorldZipCodeBorder
    {
        [BsonId]

        public int Id { get; set; }

        public string Zip { get; set; }

        public string POName { get; set; }

        public string State { get; set; }

        public int SumbLkPop { get; set; }


        public int Pop2007 { get; set; }


        public double Pop07Sqmi { get; set; }


        public double Sqmi { get; set; }


        public int Oid { get; set; }

        public GeoJsonMultiPolygon<GeoJson2DGeographicCoordinates> Geom { set { value = null; } get { return Program.GetMultiPolyGon(st_astext); } }

    }
}
