using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crankdata.Models
{
    public class UserImageModel
    {
        public string Id { get; set; }
        public string UserId { get; set; }

        public ImageThumbnail Image { get; set; }
    }
}
