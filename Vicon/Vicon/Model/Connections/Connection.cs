using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Viscon.Model.Connections
{
    [XmlType("Connection")]
    [XmlInclude(typeof(DataConnection)), XmlInclude(typeof(FlowConnection))]
    public abstract class Connection
    {
        [XmlAttribute("ID")]
        public long ID { get; set; }
    }
}
