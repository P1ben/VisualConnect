using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Viscon.Model
{
    [XmlType("Import")]
    public class Import
    {
        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlAttribute("IsExternal")]
        public bool External { get; set; } = true;

        public Import() { }
        public Import(string name, bool ext)
        {
            Name = name;
            External = ext;
        }
    }
}
