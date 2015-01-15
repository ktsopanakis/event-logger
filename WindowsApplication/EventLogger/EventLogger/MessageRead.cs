using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EventLogger
{
    public class MessageRead
    {
        public ReadPayLoad payload { get; set; }
    }

    public class ReadPayLoad
    {
        public string origin { get; set; }

        public string readFrom { get; set; }

        public string id { get; set; }
    }
}
