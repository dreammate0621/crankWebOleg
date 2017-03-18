using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crankdata.Models
{
    public class MultipleArtistImagesModel
    {
        public List<string> artistIds = new List<string>();
        public string imageType { get; set; }
    }
}
