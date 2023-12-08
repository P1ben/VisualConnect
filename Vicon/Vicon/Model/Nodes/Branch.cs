using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Viscon.CCG;
using Viscon.Model.Connectables;
using Viscon.Model.Nodes.Enums;

namespace Viscon.Model.Nodes
{
    [XmlType("Branch")]
    [XmlInclude(typeof(Branch))]
    public class Branch : Node
    {
        // Flow in
        public FlowParameter FlowIn;

        // Flow out if condition evaluates true
        public FlowParameter FlowOutTrue;

        // Flow out if condition is false
        public FlowParameter FlowOutFalse;

        [XmlIgnore]
        public FlowParameter commonEndPoint = null;
        // Condition Parameter
        public DataParameter Condition;

        public Branch() 
        {
            Condition    = new DataParameter(this, $"Branch_Condition", CDataTypes.BOOL);
            FlowIn       = new FlowParameter(this, $"Branch_FlowIn");
            FlowOutTrue  = new FlowParameter(this, $"Branch_FlowOutTrue");
            FlowOutFalse = new FlowParameter(this, $"Branch_FlowOutFalse");
            ID = Orchestrator.GetNextId();
        }

        public override void GenerateGraphicObject(MainWindow window, double x, double y)
        {
            window.branch_create(x, y, this);
        }

        public override void UpdateParams()
        {
            FlowIn.parent = this;
            FlowOutTrue.parent = this;
            FlowOutFalse.parent = this;
            Condition.parent = this;
        }

        public override Connectable GetParameterById(long ID)
        {
            if (FlowIn.ID == ID) return FlowIn;
            if (FlowOutTrue.ID == ID) return FlowOutTrue;
            if (FlowOutFalse.ID == ID) return FlowOutFalse;
            if (Condition.ID == ID) return Condition;
            return null;
        }
        public override FlowParameter getOutputFlowParam()
        {
            return commonEndPoint;
        }



        public override List<string> GenerateCode()
        {
            var lista= new List<string>();
            Node endOfIfNode=null;

            //Az igaz ágon lévő kód listája
            var trueNodeLista = new List<Node>();
            Node tmpNode = Orchestrator.NextFlowConnection(FlowOutTrue);
            while (tmpNode != null)
            {
                trueNodeLista.Add(tmpNode);
                tmpNode = Orchestrator.NextFlowItem(tmpNode);
            }

            var falseLista = new List<string>();
            var falseNodeLista = new List<Node>();
            tmpNode = Orchestrator.NextFlowConnection(FlowOutFalse);
            if (tmpNode == null) commonEndPoint = FlowOutFalse;
            else
            {
                while (tmpNode != null)
                {
                    //Ha megvan az első közös elem akkor megáll.
                    if (trueNodeLista.Contains(tmpNode))
                    {
                        endOfIfNode = tmpNode;
                        commonEndPoint = tmpNode.getOutputFlowParam();
                        Generated = true;
                        break;
                    }
                    falseNodeLista.Add(tmpNode);
                    tmpNode = Orchestrator.NextFlowItem(tmpNode);
                }
                if (!CGenerator.Warmup && tmpNode == null)
                {
                    falseNodeLista.RemoveAt(falseNodeLista.Count - 1);
                }

                foreach (var node in falseNodeLista)
                {
                    falseLista.AddRange(node.GenerateCode());
                }
            }


            List<string> outputLista = new List<string>();

            var cond = Orchestrator.GetDataNames(Condition);

            outputLista.Add($"if({((cond != null) ? cond.GenerateCode()[0] : "NULL")})");
            outputLista.Add($"{{");



            tmpNode = Orchestrator.NextFlowConnection(FlowOutTrue);
            if (tmpNode == null) return null;
            while (tmpNode != endOfIfNode && tmpNode!=this)
            {
                foreach (string codeLine in tmpNode.GenerateCode())
                {
                    string indentedLine = "\t" + codeLine;
                    outputLista.Add(indentedLine);
                }



                tmpNode = Orchestrator.NextFlowItem(tmpNode) ;
            }
            outputLista.Add("}");
            if (falseLista.Count > 0)
            {
                outputLista.Add("else");
                outputLista.Add("{");
                outputLista.AddRange(falseLista.Select(x => x = "\t" + x));
                outputLista.Add("}");
            }
            
            if (endOfIfNode!=null) outputLista.AddRange(endOfIfNode.GenerateCode());
            return outputLista;
        }
    }
}
