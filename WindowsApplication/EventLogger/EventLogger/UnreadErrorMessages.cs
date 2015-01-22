using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EventLogger
{
    [DataContract]
    public class UnreadErrorMessages
    {
        [DataMember(Name="unread")]
        public List<Message> Unread { get; set; } 
    }

    [DataContract]
    public class Message : INotifyPropertyChanged
    {
        [DataMember(Name="_id")]
        public string Id { get; set; }

        [DataMember(Name="payload")]
        public string Payload { get; set; }

        [DataMember(Name = "identifier")]
        public Identifier Identifier { get; set; }

        [DataMember(Name = "timestamp")]
        public string Timestamp { get { return null; } set { Date = (value!=null ? UnixTimeStampToDateTime(Double.Parse(value)): new DateTime()) ; }}

        public DateTime Date { get; set; }

        [DataMember(Name = "receivedtime")]
        public string Receivedtime { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        private bool _isRead;

        [DataMember(Name = "isRead")]
        public bool IsRead { get { return this._isRead; } set { this._isRead = value; OnPropertyChanged("IsRead"); } }

        [DataMember(Name = "origin")]
        public string Origin { get; set; }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(String name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }

    [DataContract]
    public class Payload
    {
        
    }

    [DataContract]
    public class Identifier
    {
        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "value")]
        public string Value { get; set; }
    }

}
