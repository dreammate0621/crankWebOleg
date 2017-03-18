using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.GeoJsonObjectModel;



namespace Crankdata.Models
{
    /// <summary>
    /// "id","name","place_class","st","stfips","placefip","houseunits","pop2000","pop_class","arealand","areawater","oid","geom"
    /// </summary>
    public class WorldPlacepoint
    {
        [BsonId]        
        public int Id { get; set; }

        
        public string Name { get; set; }


        
        public string PlaceClass { get; set; }

        
        public string St { get; set; }


        
        public string StFips { get; set; }

        
        public string PlaceFip { get; set; }


        
        public int HouseUnits { get; set; }


        
        public int Pop2000 { get; set; }


        
        public int PopClass { get; set; }


        
        public double AreaLand { get; set; }


        
        public double AreaWater { get; set; }


        
        public int OId { get; set; }
        
        
        public GeoJson2DGeographicCoordinates Geom { set { value = new GeoJson2DGeographicCoordinates(0,0); } get { return new GeoJson2DGeographicCoordinates(Program.GetLatitude(st_asewkt), Program.GetLongitude(st_asewkt)); } }
    }
}
