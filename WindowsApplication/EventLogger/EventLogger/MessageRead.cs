using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EventLogger
{
    [DataContract]
    public class MessageRead
    {
        [DataMember(Name="payload")]
        public ReadPayLoad Payload { get; set; }
    }

    [DataContract]
    public class ReadPayLoad
    {
        [DataMember(Name="origin")]
        public string Origin { get; set; }

        [DataMember(Name="readFrom")]
        public string ReadFrom { get; set; }

        [DataMember(Name="id")]
        public string Id { get; set; }
    }
}
