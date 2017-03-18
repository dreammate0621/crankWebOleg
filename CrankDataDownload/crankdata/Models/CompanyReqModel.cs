using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crankdata.Models
{
   public class CompanyReqModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<string> OtherNames = new List<string>();
        public List<Address> Addresses = new List<Address>();
        public List<Email> Emails = new List<Email>();
        public List<Phone> Phones = new List<Phone>();
        public List<Phone> Faxes = new List<Phone>();
        public List<string> Admins = new List<string>();
        public List<string> References = new List<string>();
        public bool isActive { get; set; }
        public bool HasApiKeySent { get; set; } = false;
        public DateTime CreatedDate { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public List<ImageThumbnail> Images = new List<ImageThumbnail>();
    }
}
