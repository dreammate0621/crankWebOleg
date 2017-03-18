﻿using MongoDB.Bson;
using MongoDB.Driver.GeoJsonObjectModel;
using MongoDB.Bson.Serialization.Attributes;

namespace Crankdata.Models
{
    public class Venue
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public string Name { get; set; }

        public int? VenueSKId { get; set; }

        public string Street { get; set; }

        public GeoJson2DCoordinates LngLat { get; set; }

        public string Phone { get; set; }
        public string Website { get; set; }

        public string City { get; set; }

        public string State { get; set; }
        public string Country { get; set; }


        public int? Capacity { get; set; }

        public string Description { get; set; }

        public MetroArea MetroArea { get; set; }

    }
}