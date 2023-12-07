using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Viscon.Model.Connectables;
using Viscon.Model.Nodes.Enums;

namespace Viscon.Model.Nodes
{
    [XmlType("BeginNode")]
    public class BeginNode : Node
    {
        // Function flow start
        public FlowParameter FlowOut;

        public BeginNode()
        {
            FlowOut = new FlowParameter(this, "Begin Flow Out");
            ID = Orchestrator.GetNextId();
        }

        public override void GenerateGraphicObject(MainWindow window, double x, double y)
        {
            window.beginbox_create(x, y, this);
        }

        public override void UpdateParams()
        {
            FlowOut.parent = this;
        }

        public override Connectable GetParameterById(long ID)
        {
            if (FlowOut.ID == ID) return FlowOut;
            return null;
        }
        public override FlowParameter getOutputFlowParam()
        {
            return FlowOut;
        }
        public override NodeType TypeInformer()
        {
            return NodeType.Begin;
        }

        public override List<string> GenerateCode()
        {
            return null;
        }
    }
}
