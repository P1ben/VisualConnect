using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Viscon.Model.Nodes;
using Viscon.Model.Nodes.Enums;

namespace Viscon.Model.Connectables
{
    [XmlType("DataParameter")]
    public class DataParameter : Connectable
    {
        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlAttribute("Type")]
        public CDataTypes Type { get; set; }

        public DataParameter() { }

        public DataParameter(Node _parent, string _name, CDataTypes _type)
        {
            parent = _parent;
            Name   = _name;
            Type   = _type;
            ID     = Orchestrator.GetNextId();
        }
    }
}
