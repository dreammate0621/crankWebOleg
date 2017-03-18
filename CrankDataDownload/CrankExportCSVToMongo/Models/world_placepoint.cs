using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.GeoJsonObjectModel;
using System.Data.Spatial;
using System;


namespace CrankExportCSVToMongo.Models
{
    /// <summary>
    /// "id","name","place_class","st","stfips","placefip","houseunits","pop2000","pop_class","arealand","areawater","oid","geom"
    /// </summary>
    public class world_placepoint
    {
        [BsonId]
        [BsonElement("Id")]
        public int id { get; set; }

        [BsonElement("Name")]
        public string name { get; set; }


        [BsonElement("PlaceClass")]
        public string place_class { get; set; }

        [BsonElement("St")]
        public string st { get; set; }


        [BsonElement("StFips")]
        public string stfips { get; set; }

        [BsonElement("PlaceFip")]
        public string placefip { get; set; }


        [BsonElement("HouseUnits")]
        public int houseunits { get; set; }


        [BsonElement("Pop2000")]
        public int pop2000 { get; set; }


        [BsonElement("PopClass")]
        public int pop_class { get; set; }


        [BsonElement("AreaLand")]
        public double arealand { get; set; }


        [BsonElement("AreaWater")]
        public double areawater { get; set; }


        [BsonElement("OId")]
        public int oid { get; set; }
        [BsonIgnore]
        public string st_asewkt { get; set; }


        [BsonElement("Geom")]
        public GeoJson2DGeographicCoordinates geom { set { value = new GeoJson2DGeographicCoordinates(0,0); } get { return new GeoJson2DGeographicCoordinates(Program.GetLatitude(st_asewkt), Program.GetLongitude(st_asewkt)); } }
    }
}
