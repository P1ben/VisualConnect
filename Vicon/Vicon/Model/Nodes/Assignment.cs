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
    [XmlType("Assignment")]
    public class Assignment : Node
    {
        // Flow in
        public FlowParameter FlowIn;

        // Flow out
        public FlowParameter FlowOut;

        // Left data (Variable)
        public DataParameter DataInLeft;

        // Right data
        public DataParameter DataInRight;

        public Assignment()
        {
            FlowOut = new FlowParameter(this, "Flow Out");
            FlowIn = new FlowParameter(this, "Flow In");
            DataInLeft  = new DataParameter(this, $"Assignment_DataInLeft", CDataTypes.BOOL);
            DataInRight = new DataParameter(this, $"Assignment_DataInRight", CDataTypes.BOOL);
            ID = Orchestrator.GetNextId();
        }

        public override void GenerateGraphicObject(MainWindow window, double x, double y)
        {
            window.assignment_create(x, y, this);
        }

        public override void UpdateParams()
        {
            FlowIn.parent = this;
            FlowOut.parent = this;
            DataInLeft.parent = this;
            DataInRight.parent = this;
        }

        public override Connectable GetParameterById(long ID)
        {
            if (FlowIn.ID == ID) return FlowIn;
            if (FlowOut.ID == ID) return FlowOut;
            if (DataInLeft.ID == ID) return DataInLeft;
            if (DataInRight.ID == ID) return DataInRight;
            return null;
        }


        public override FlowParameter getOutputFlowParam()
        {
            return FlowOut;
        }

        public override List<string> GenerateCode()
        {
            Node left = Orchestrator.GetDataNames(DataInLeft);
            Node right = Orchestrator.GetDataNames(DataInRight);

            string ret = $"{((left != null) ? left.GenerateCode()[0] : "NULL")}" +
                         $" = " +
                         $"{((right != null) ? right.GenerateCode()[0] : "NULL")};";

            return new List<string>() { ret };
        }
    }
}
