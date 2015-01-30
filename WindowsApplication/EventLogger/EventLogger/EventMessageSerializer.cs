using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventLogger
{
    public static class EventMessageSerializer
    {
        public static string MessageToString(this Message eventMessage )
        {
            string serializedMessage = "";

            serializedMessage += eventMessage.Date;

            if (!String.IsNullOrEmpty(eventMessage.Origin))
                serializedMessage += " " + eventMessage.Origin;
            if (!String.IsNullOrEmpty(eventMessage.Payload))
            {
                serializedMessage += " " + eventMessage.Payload.Replace("\r\n"," ");
            }
            if (eventMessage.Identifier != null)
            {
                if (!String.IsNullOrEmpty(eventMessage.Identifier.Type))
                    serializedMessage += " " + eventMessage.Identifier.Type;
                if (!String.IsNullOrEmpty(eventMessage.Identifier.Value))
                    serializedMessage += " " + eventMessage.Identifier.Value;
            }
            return serializedMessage;
        }
    }
}
