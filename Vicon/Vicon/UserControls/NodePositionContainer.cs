using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Viscon.UserControls
{
    [XmlType("NodePosition")]
    public class NodePositionContainer
    {
        [XmlAttribute("ID")]
        public long ID;

        [XmlAttribute("X")]
        public double x;

        [XmlAttribute("Y")]
        public double y;

        public NodePositionContainer()
        {

        }

        public NodePositionContainer(long ID, double X, double Y)
        {
            this.ID = ID;
            this.x = X;
            this.y = Y;
        }
    }
}
