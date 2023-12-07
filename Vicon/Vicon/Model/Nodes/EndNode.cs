using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Viscon.Model.Connectables;
using Viscon.Model.Nodes.Enums;

namespace Viscon.Model.Nodes
{
    [XmlType("EndNode")]
    public class EndNode : Node
    {
        public FlowParameter FlowIn;

        public EndNode()
        {
            FlowIn = new FlowParameter(this, "Function End");
            ID = Orchestrator.GetNextId();
        }
        public override void GenerateGraphicObject(MainWindow window, double x, double y)
        {
            window.endbox_create(x, y, this);
        }

        public override void UpdateParams()
        {
            FlowIn.parent = this;
        }

        public override Connectable GetParameterById(long ID)
        {
            if (FlowIn.ID == ID) return FlowIn;
            return null;
        }
        public override FlowParameter getOutputFlowParam()
        {
            return null;
        }
        public override NodeType TypeInformer()
        {
            return NodeType.End;
        }

        public override List<string> GenerateCode()
        {
            return new List<string>() { "return;" };
        }
    }
}
