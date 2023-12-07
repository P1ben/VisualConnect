using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Viscon.Model.Nodes;

namespace Viscon.Model.Connectables
{
    [XmlType("Connectable")]
    [XmlInclude(typeof(DataParameter)), XmlInclude(typeof(FlowParameter))]
    public abstract class Connectable
    {
        [XmlAttribute("ID")]
        public long ID { get; set; }

        [XmlIgnore]
        public Node parent = null;
    }
}
