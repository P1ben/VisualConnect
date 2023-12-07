using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Viscon.Model.Connectables;

namespace Viscon.Model.Connections
{
    [XmlType("DataConnection")]
    [XmlInclude(typeof(DataConnection))]
    public class DataConnection : Connection
    {
        [XmlAttribute("FlowInID")]
        public long ProducerID;

        [XmlAttribute("FlowOutID")]
        public long ConsumerID;

        [XmlIgnore]
        public DataParameter Producer { get; set; }

        [XmlIgnore]
        public DataParameter Consumer { get; set; }

        public DataConnection()
        {
            ID = Orchestrator.GetNextId();
        }

        public DataConnection(DataParameter _producer, DataParameter _consumer)
        {
            Producer = _producer;
            Consumer = _consumer;
            ProducerID = _producer.ID;
            ConsumerID = _consumer.ID;
            ID = Orchestrator.GetNextId();
        }
    }
}
