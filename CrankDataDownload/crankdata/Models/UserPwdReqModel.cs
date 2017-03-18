using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crankdata.Models
{
    public class UserPwdReqModel
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        public DateTime updatedDate { get; set; }
    }
}
