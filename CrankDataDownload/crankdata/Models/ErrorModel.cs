using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crankdata.Models
{
    public class ErrorModel
    {
        public string errorMessage { get; set; }
        public string errorDesc { get; set; }
        public int errorCode { get; set; }

        public ErrorModel(string msg, string desc)
        {
            errorMessage = msg;
            errorDesc = desc;
            errorCode = -1;
        }
        public ErrorModel(string msg, string desc, int eCode)
        {
            errorMessage = msg;
            errorDesc = desc;
            errorCode = eCode;
        }
    }

}
