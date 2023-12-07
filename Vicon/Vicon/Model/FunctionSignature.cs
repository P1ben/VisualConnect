using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Markup;
using Viscon.CCG;
using Viscon.Model.Nodes.Enums;

namespace Viscon.Model
{
    public class FunctionSignature
    {
        public string Name { get; set; }
        public CDataTypes ReturnType { get; set; }
        public List<(string name, CDataTypes type)> Parameters { get;}

        public FunctionSignature(string annot)
        {
            try
            {
                Parameters = new List<(string name, CDataTypes type)>();
                annot = annot.Replace(" ", "");
                annot = annot.Replace("[", "");
                annot = annot.Replace("]", "");

                // Parsing annotation
                var separated = annot.Split(new string[] { "::" }, StringSplitOptions.None);

                var name_split = separated[0].Split(':');
                Name = name_split[0];
                ReturnType = (CDataTypes)CGenerator.CTypes.ToList().IndexOf(name_split[1]);

                var params_split = separated[1].Split(',');
                foreach ( var param in params_split )
                {
                    var type_and_name = param.Split(':');
                    string name = type_and_name[0];
                    CDataTypes type = GetVariableType(type_and_name[1].ToUpper());
                    Parameters.Add((name, type));
                }
            }
            catch { /* Parse error */ }
        }

        CDataTypes GetVariableType(string type)
        {
            return (CDataTypes)Enum.Parse(typeof(CDataTypes), type);
        }
    }
}
