using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ParseMediabaseData.AppCode.Utils
{
    public class ProcessHelper
    {
        Process _process = new Process();

        public Process Process
        {
            get
            {
                return _process;
            }
        }


        public ProcessHelper()
        {

        }
    }
}
