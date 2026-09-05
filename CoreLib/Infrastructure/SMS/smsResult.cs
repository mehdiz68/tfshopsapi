using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Infrastructure.SMS
{
    public class smsResult
    {
        public string Mobile { get; set; }
        public string Message { get; set; }
        public string Error { get; set; }
        public bool ReturnType { get; set; }
    }

    public class sr
    {
        public string status { get; set; }
        public string data { get; set; }
        public bool ReturnType { get; set; }
    }
}
