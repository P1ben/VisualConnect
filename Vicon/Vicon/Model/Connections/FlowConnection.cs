using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Viscon.Model.Connectables;

namespace Viscon.Model.Connections
{
    [XmlType("FlowConnection")]
    [XmlInclude(typeof(FlowConnection))]
    public class FlowConnection : Connection
    {
        [XmlAttribute("FlowInID")]
        public long FlowInID;

        [XmlAttribute("FlowOutID")]
        public long FlowOutID;

        [XmlIgnore]
        public FlowParameter In  { get; set; }

        [XmlIgnore]
        public FlowParameter Out { get; set; }


        public FlowConnection()
        {

        }

        public FlowConnection(FlowParameter _in, FlowParameter _out)
        {
            In  = _in;
            Out = _out;
            FlowInID = _in.ID;
            FlowOutID = _out.ID;
            ID = Orchestrator.GetNextId();
        }
    }
}
