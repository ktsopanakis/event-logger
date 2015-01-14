using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventLogger
{
    public class UnreadErrorMessages
    {
        List<Message> unread { get; set; } 
    }

    public class Message
    {
        public int _id { get; set; }
        public Payload payload { get; set; }
        public Identifier identifier { get; set; }
        public string timestamp { get; set; }
        public string receivedtime { get; set; }
        public string title { get; set; }
        public bool isRead { get; set; }
        public string origin { get; set; }
    }

    public class Payload
    { }

    public class Identifier
    {
        public string type { get; set; }
        public string value { get; set; }
    }
}
