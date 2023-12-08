using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Viscon.Model.Connectables;
using Viscon.Model.Nodes.Enums;

namespace Viscon.Model.Nodes
{
    [XmlType("Node")]
    [XmlInclude(typeof(Arithmetic)), XmlInclude(typeof(Assignment)), XmlInclude(typeof(Branch)),
     XmlInclude(typeof(CompareBox)), XmlInclude(typeof(Function)),   XmlInclude(typeof(LogicBox)),
     XmlInclude(typeof(Loop)),       XmlInclude(typeof(Variable)),   XmlInclude(typeof(BeginNode)),
     XmlInclude(typeof(EndNode)),    XmlInclude(typeof(ReturnNode))]
    public abstract class Node
    {
        [XmlAttribute("ID")]
        public long ID { get; set; }

        public abstract void GenerateGraphicObject(MainWindow window, double x, double y);
        public abstract void UpdateParams();
        public abstract Connectable GetParameterById(long ID);

        public abstract FlowParameter getOutputFlowParam();

        public abstract List<string> GenerateCode();

        [XmlIgnore]
        public bool Generated { get; set; } = false;

        public virtual NodeType TypeInformer()
        {
            return NodeType.None;
        }

    }
}
