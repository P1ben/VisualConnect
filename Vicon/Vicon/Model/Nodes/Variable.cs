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
    [XmlType("Variable")]
    [XmlInclude(typeof(Variable))]
    public class Variable : Node
    {
        [XmlAttribute("Type")]
        public CDataTypes Type { get; set; }

        [XmlAttribute("Value")]
        public string Value { get; set; } = "";

        [XmlAttribute("IsParam")]
        public bool isFunctionParam = false;

        [XmlAttribute("Name")]
        public string Name = "";

        public DataParameter Connection;

        public Variable()
        {
            ID = Orchestrator.GetNextId();
        }

        public Variable(CDataTypes _type, string _value)
        {
            Type = _type;
            Value = _value;
            Connection = new DataParameter(this, $"VariableParam", Type);
            ID = Orchestrator.GetNextId();
        }

        public override void GenerateGraphicObject(MainWindow window, double x, double y)
        {
            window.variable_create(x, y, this);
        }

        public override void UpdateParams()
        {
            Connection.parent = this;
        }

        public override Connectable GetParameterById(long ID)
        {
            if (Connection.ID == ID) return Connection;
            return null;
        }

        public override FlowParameter getOutputFlowParam()
        {
            return null;
        }

        public override NodeType TypeInformer()
        {
            return NodeType.Variable;
        }

        public override List<string> GenerateCode()
        {
            string name = Name.Length == 0 ? $"variable{ID}" : Name;
            return new List<string>() { name };
        }
    }
}
