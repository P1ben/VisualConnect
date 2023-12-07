using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Viscon.Model.Connectables;
using Viscon.Model.Connections;
using Viscon.Model.Nodes;
using Viscon.Model.Nodes.Enums;

namespace Viscon.Model
{
    [XmlRoot("Workspace")]
    public class Workspace
    {
        public string Name { get; set; } = "lajos";

        [XmlArray("Imports")]
        public List<Import> imports = new List<Import>();

        [XmlArray("NodeArray")]
        public List<Node> connectables = new List<Node>();

        [XmlArray("DataConnectionArray")]
        public List<DataConnection>   dataConnections  = new List<DataConnection>();

        [XmlArray("FlowConnectionArray")]
        public List<FlowConnection>   flowConnections  = new List<FlowConnection>();

        public CDataTypes ReturnValue { get; set; } = CDataTypes.VOID;

        public Workspace() { }
        public Workspace(string name, CDataTypes return_type)
        {
            Name = name;
            ReturnValue = return_type;
        }

        public void AddNode(Node newnode)
        {
            connectables.Add(newnode);

        }

        public void RemoveNode(Node toremove)
        {
            connectables.Remove(toremove);
        }

        public void AddConnection(DataConnection newconnection)
        {
            dataConnections.Add(newconnection);
        }

        public void RemoveConnection(FlowConnection toremove)
        {
            flowConnections.Remove(toremove);
        }

        public bool ConnectionExists(Connectable a, Connectable b)
        {
            foreach (var conn in dataConnections)
            {
                if ((conn.Consumer == a && conn.Producer == b) || (conn.Consumer == b && conn.Producer == a)) return true;
            }

            foreach (var conn in flowConnections)
            {
                if ((conn.In == a && conn.Out == b) || (conn.In == b && conn.Out == a)) return true;
            }
            return false;
        }
    }
}
