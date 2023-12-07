using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Viscon.Model.Nodes;

namespace Viscon.Model.Connectables
{
    [XmlType("FlowParameter")]
    public class FlowParameter : Connectable
    {
        [XmlAttribute("Description")]
        public string Description { get; set; }

        public FlowParameter() { }

        public FlowParameter(Node _parent, string _description)
        {
            parent = _parent;
            Description = _description;
            ID = Orchestrator.GetNextId();
        }
    }
}
