using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;
using Viscon.Model.Connectables;
using Viscon.Model.Nodes.Enums;

namespace Viscon.Model.Nodes
{
    [XmlType("CompareBox")]
    public class CompareBox : Node
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
        public CompareOperator arithOperator = CompareOperator._GREATERTHAN;

        public CompareBox()
        {
            dataOut     = new DataParameter(this, $"CompareBox_DataOut", CDataTypes.BOOL);
            dataInLeft  = new DataParameter(this, $"CompareBox_DataInLeft", CDataTypes.BOOL);
            dataInRight = new DataParameter(this, $"CompareBox_DataInRight", CDataTypes.BOOL);
            ID = Orchestrator.GetNextId();
        }

        public override void GenerateGraphicObject(MainWindow window, double x, double y)
        {
            window.comparebox_create(x, y, this);
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
            ret = $"({((left != null) ? left.GenerateCode()[0] : "NULL")}" +
                  $" {new string[]{"<", ">", "<=", ">=", "==", "!="}[(int)arithOperator]} " +
                  $"{((right != null) ? right.GenerateCode()[0] : "NULL")})";

            return new List<string>() { ret };
        }
    }
}
