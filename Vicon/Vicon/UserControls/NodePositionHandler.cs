using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Viscon.UserControls
{
    [XmlType("Layout")]
    public class NodePositionHandler
    {
        [XmlArray("Nodes")]
        public List<NodePositionContainer> elements = new List<NodePositionContainer>();

        public NodePositionHandler() { }

        public void AddNode(long ID, double X, double Y)
        {
            if(elements.Where(x => x.ID == ID).Count() == 0)
                elements.Add(new NodePositionContainer(ID, X, Y));
        }

        public void DeleteNodeById(long ID)
        {
            if (elements.Where(x => x.ID == ID).Count() > 0)
                elements.Remove(elements.Where(x => x.ID == ID).ToArray()[0]);
        }

        public void UpdateNodeById(long ID, double new_X, double new_Y)
        {
            if (elements.Where(x => x.ID == ID).Count() > 0)
            {
                elements.Where(x => x.ID == ID).ToArray()[0].x = new_X;
                elements.Where(x => x.ID == ID).ToArray()[0].y = new_Y;
            }
        }

        public (double, double) GetPositionById(long ID)
        {
            return elements.Where(x => x.ID == ID).Count() != 0 
                    ? elements.Where(x => x.ID == ID).Select(x => (x: x.x, y: x.y)).First() 
                    : (x: 50, y: 50) ;

        }
    }
}
