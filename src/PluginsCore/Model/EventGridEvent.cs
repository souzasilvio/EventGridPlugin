using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PluginsCore.Model
{

    [KnownType(typeof(Entidade))]
    public class EventGridEvent
    {
        public EventGridEvent()
        {
        }
        public EventGridEvent(string id, string subject, object data, string eventType, DateTime eventTime, string dataVersion, string topic = null, string metadataVersion = null)
        {
            Id = id;
            Subject = subject;
            Data = data;
            EventType = eventType;
            EventTime = eventTime;
            DataVersion = dataVersion;
            Topic = topic;
            MetadataVersion = metadataVersion;
        }

        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "topic")]
        public string Topic { get; set; }

        [DataMember(Name = "subject")]
        public string Subject { get; set; }


        [DataMember(Name = "data")]
        public object Data { get; set; }

        [DataMember(Name = "eventtype")]
        public string EventType { get; set; }


        [DataMember(Name = "eventtime")]
        public DateTime EventTime { get; set; }

        [DataMember(Name = "metadataversion")]
        public string MetadataVersion { get; }

        [DataMember(Name = "dataversion")]
        public string DataVersion { get; set; }

    }
}
