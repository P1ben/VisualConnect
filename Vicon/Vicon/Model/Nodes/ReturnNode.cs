using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Xml.Serialization;
using Viscon.Model.Connectables;
using Viscon.Model.Nodes.Enums;

namespace Viscon.Model.Nodes
{
    [XmlType("ReturnNode")]
    public class ReturnNode : Node
    {
        public FlowParameter FlowIn;

        public DataParameter ReturnValue;

        public ReturnNode()
        {
            FlowIn = new FlowParameter(this, "Function Flow End");
            ReturnValue = new DataParameter(this, $"ReturnNode_ReturnValue", CDataTypes.BOOL);
            ID = Orchestrator.GetNextId();
        }

        public override void GenerateGraphicObject(MainWindow window, double x, double y)
        {
            window.returnbox_create(x, y, this);
        }

        public override void UpdateParams()
        {
            FlowIn.parent = this;
            ReturnValue.parent = this;
        }

        public override Connectable GetParameterById(long ID)
        {
            if (FlowIn.ID == ID) return FlowIn;
            if (ReturnValue.ID == ID) return ReturnValue;
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
            Node returnval = Orchestrator.GetDataNames(ReturnValue);
            string ret = $"return " +
                         $"{((returnval != null) ? returnval.GenerateCode()[0] : "NULL")};";

            return new List<string>() { ret };
        }
    }
}
