using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Crankdata.Models
{
    public class MetroAreaViewModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string State { get; set; }

        public string Country { get; set; }
    }
}
