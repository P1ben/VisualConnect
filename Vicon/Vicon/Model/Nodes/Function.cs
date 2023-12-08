using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Xml.Serialization;
using Viscon.Model.Connectables;
using Viscon.Model.Nodes.Enums;

namespace Viscon.Model.Nodes
{
    [XmlType("Function")]
    public class Function : Node
    {
        [XmlAttribute("Name")]
        public string      Name { get; set; }

        [XmlAttribute("Type")]
        public CDataTypes  Type { get; set; }

        public FlowParameter FlowIn;

        public FlowParameter FlowOut;

        [XmlElement(ElementName = "DataParams")]
        public List<DataParameter> DataParams = new List<DataParameter>();

        [XmlIgnore]
        public bool AlreadyGenerated = false;

        public DataParameter Output;

        public Function() 
        {
            ID = Orchestrator.GetNextId();
        }

        public Function(string _name, CDataTypes _type, List<DataParameter> _parameters, DataParameter output)
        {
            Name   = _name;
            Type   = _type;
            DataParams = _parameters;
            Output = output;
            FlowIn = new FlowParameter(this, "Flow In");
            FlowOut = new FlowParameter(this, "Flow Out");
            ID = Orchestrator.GetNextId();
        }

        public static Function Replicate(FunctionSignature sign)
        {
            var parameters = new List<DataParameter>();

            foreach (var parameter in sign.Parameters)
            {
                parameters.Add(new DataParameter(null, parameter.name, parameter.type));
            }

            Function func;
            if (sign.ReturnType != CDataTypes.VOID)
            {
                func = new Function(sign.Name, sign.ReturnType, parameters, new DataParameter(null, "Output", sign.ReturnType));
            }
            else
            {
                func = new Function(sign.Name, sign.ReturnType, parameters, null);
            }

            func.UpdateParams();
            return func;
        }

        [Obsolete("Method is deprecated, don't use it")]
        public void AddParam(DataParameter param)
        {
            DataParams.Add(param);
        }

        public override void GenerateGraphicObject(MainWindow window, double x, double y)
        {
            window.function_create(x, y, this, null);
        }

        public override void UpdateParams()
        {
            FlowIn.parent = this;
            FlowOut.parent = this;
            if (DataParams != null)
            {
                DataParams.ForEach(x => x.parent = this);
            }
            if (Output != null)
                Output.parent = this;
        }

        public override Connectable GetParameterById(long ID)
        {
            if (FlowIn.ID == ID) return FlowIn;
            if (FlowOut.ID == ID) return FlowOut;
            if (DataParams != null)
            {
                foreach (DataParameter param in DataParams)
                {
                    if (param.ID == ID) return param;
                }
            }
            return null;
        }
        public override FlowParameter getOutputFlowParam()
        {
            return FlowOut;
        }

        public override List<string> GenerateCode()
        {
            if (AlreadyGenerated && Type != CDataTypes.VOID)
            {
                return new List<string>() { $"function{ID}_retval" };
            }
            else
            {
                string function_call = "";
                if (Type != CDataTypes.VOID)
                {
                    function_call += $"function{ID}_retval = ";
                }
                function_call += Name + "(";
                bool first = true;
                foreach(DataParameter param in DataParams)
                {
                    if (!first)
                    {
                        function_call += ", ";
                    }
                    Node data = Orchestrator.GetDataNames(param);
                    if (data != null)
                    {
                        function_call += data.GenerateCode()[0];
                    }
                    else
                    {
                        function_call += "0";
                    }
                    first = false;
                }
                function_call += ");";
                return new List<string>() { function_call };
            }
        }

        public override NodeType TypeInformer()
        {
            return NodeType.Function;
        }
    }
}
