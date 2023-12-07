using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Serialization;
using Viscon.Model.Connectables;
using Viscon.Model.Nodes.Enums;

namespace Viscon.Model.Nodes
{
    [XmlType("Arithmetic")]
    public class Arithmetic : Node
    {
        // Data output
        public DataParameter dataOut;

        // Input variables
        public DataParameter dataInLeft;
        public DataParameter dataInRight;

        // Operator
        public ArithmeticOperator arithOperator = ArithmeticOperator._PLUS;

        public Arithmetic() {
            dataOut     = new DataParameter(this, $"Aritmetic_DataOut", CDataTypes.BOOL);
            dataInLeft  = new DataParameter(this, $"Aritmetic_DataInLeft", CDataTypes.BOOL);
            dataInRight = new DataParameter(this, $"Aritmetic_DataInRight", CDataTypes.BOOL);
            ID = Orchestrator.GetNextId();
        }

        public override void GenerateGraphicObject(MainWindow window, double x, double y)
        {
            window.arithmetic_create(x, y, this);
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

            string ret = $"({((left != null) ? left.GenerateCode()[0] : "NULL")}" +
                         $" {"+-*%/"[(int)arithOperator]} " +
                         $"{((right != null) ? right.GenerateCode()[0] : "NULL")})";

            return new List<string>() { ret };
        }
    }
}
