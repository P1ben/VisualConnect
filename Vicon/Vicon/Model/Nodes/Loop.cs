using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Annotations;
using System.Xml.Serialization;
using Viscon.Model.Connectables;
using Viscon.Model.Nodes.Enums;

namespace Viscon.Model.Nodes
{
    [XmlType("Loop")]
    public class Loop : Node
    {
        // Node flow in
        public FlowParameter FlowIn;

        // Node flow out
        public FlowParameter FlowOut;

        // Loop body end
        public FlowParameter LoopFlowIn;

        // Loop body start
        public FlowParameter LoopFlowOut;

        // Loop termination condition
        public DataParameter LoopCondition;

        public Loop()
        {
            FlowIn = new FlowParameter(this, "Flow In");
            FlowOut = new FlowParameter(this, "Flow Out");
            LoopFlowIn = new FlowParameter(this, "Loop Flow In");
            LoopFlowOut = new FlowParameter(this, "Loop Flow Out");
            LoopCondition = new DataParameter(this, $"Loop_Condition", CDataTypes.BOOL);
            ID = Orchestrator.GetNextId();
        }

        public override void GenerateGraphicObject(MainWindow window, double x, double y)
        {
            window.loop_create(x, y, this);
        }

        public override void UpdateParams()
        {
            FlowIn.parent = this;
            FlowOut.parent = this;
            LoopFlowIn.parent = this;
            LoopFlowOut.parent = this;
            LoopCondition.parent = this;
        }

        public override Connectable GetParameterById(long ID)
        {
            if (FlowIn.ID == ID) return FlowIn;
            if (FlowOut.ID == ID) return FlowOut;
            if (LoopFlowIn.ID == ID) return LoopFlowIn;
            if (LoopFlowOut.ID == ID) return LoopFlowOut;
            if (LoopCondition.ID == ID) return LoopCondition;
            return null;
        }
        public override FlowParameter getOutputFlowParam()
        {
            return FlowOut;
        }

        public override List<string> GenerateCode()
        {
            this.Generated = true;
            var outputLista= new List<string>();

            var cond = Orchestrator.GetDataNames(LoopCondition);
            outputLista.Add($"while({((cond != null) ? cond.GenerateCode()[0] : "NULL")})");
            outputLista.Add($"{{");
            var tmpNode = Orchestrator.NextFlowConnection(LoopFlowOut);
            while(tmpNode != null && tmpNode != this && tmpNode.TypeInformer() != NodeType.End)
            {
                var lista = tmpNode.GenerateCode();
                foreach (string codeLine in lista)
                {
                    string indentedLine = "\t" + codeLine;
                    outputLista.Add(indentedLine);
                }
                tmpNode = Orchestrator.NextFlowItem(tmpNode);
                
            }
            outputLista.Add("}");
            return outputLista;
        }
    }
}
