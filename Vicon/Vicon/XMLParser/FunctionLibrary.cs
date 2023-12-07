using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Viscon.Model.Nodes;

namespace Viscon.XMLParser
{
    [XmlRootAttribute("File")]
    public class FunctionLibrary
    {
        [XmlAttribute("Name")]
        public string         Name { get; set; }

        [XmlAttribute("Location")]
        public string         Path { get; set; }

        [XmlElement(ElementName = "FunctionView")]
        public List<Function> functions;

        public FunctionLibrary() { }

        public FunctionLibrary(string name, string path)
        {
            Name = name;
            Path = path;
            functions = new List<Function>();
        }
    }
}
