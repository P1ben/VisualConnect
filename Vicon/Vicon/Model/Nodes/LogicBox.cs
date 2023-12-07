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
    [XmlType("LogicBox")]
    public class LogicBox : Node
    {
        // Negated
        [XmlAttribute("Negated")]
        public bool negated = false;

        // Data output
        public DataParameter dataOut;

        // Input variables
        public DataParameter dataInLeft;
        public DataParameter dataInRight;

        // Operator
        public LogicOperator arithOperator = LogicOperator._XOR;

        public LogicBox()
        {
            dataOut     = new DataParameter(this, $"LogicBox_DataOut", CDataTypes.BOOL);
            dataInLeft  = new DataParameter(this, $"LogicBox_DataInLeft", CDataTypes.BOOL);
            dataInRight = new DataParameter(this, $"LogicBox_DataInRight", CDataTypes.BOOL);
            ID = Orchestrator.GetNextId();
        }

        public override void GenerateGraphicObject(MainWindow window, double x, double y)
        {
            window.logicbox_create(x, y, this);
        }

        public override void UpdateParams()
        {
            dataOut.parent = this;
            dataInLeft.parent = this;
            dataInRight.parent = this;
        }

        public override Connectable GetParameterById(long ID)
        {
            if (dataOut.ID == ID) return dataOut;
            if (dataInLeft.ID == ID) return dataInLeft;
            if (dataInRight.ID == ID) return dataInRight;
            return null;
        }
        public override FlowParameter getOutputFlowParam()
        {
            return null;
        }

        public override List<string> GenerateCode()
        {
            Node left = Orchestrator.GetDataNames(dataInLeft);
            Node right = Orchestrator.GetDataNames(dataInRight);

            string ret = $"{(negated ? "!" : "")}";

            if (arithOperator == LogicOperator._XOR)
            {
                ret =   $"((!{((left != null) ? left.GenerateCode()[0] : "NULL")}" +
                        $" && " +
                        $"!{((right != null) ? right.GenerateCode()[0] : "NULL")}) && " +
                        $"!(!{((left != null) ? left.GenerateCode()[0] : "NULL")}" +
                        $" && " +
                        $"!{((right != null) ? right.GenerateCode()[0] : "NULL")}))";
            }
            else
            {
                ret =   $"({((left != null) ? left.GenerateCode()[0] : "NULL")}" +
                        $" {new string[] { "&&", "||" }[(int)arithOperator]} " +
                        $"{((right != null) ? right.GenerateCode()[0] : "NULL")})";
            }

            return new List<string>() { ret };
        }
    }
}
