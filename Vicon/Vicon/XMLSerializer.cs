using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Viscon
{
    public static class XMLSerializer
    {
        public static void SaveWorkspace(Workspace workspace, string fileName)
        {
            XmlSerializer ser = new XmlSerializer(typeof(Workspace));
            //string serialized;
            using (StreamWriter sw = new StreamWriter(fileName))
                ser.Serialize(sw, workspace);
        }

        public static Workspace LoadWorkspace(string fileName)
        {
            XmlSerializer ser = new XmlSerializer(typeof(Workspace));
            using (StreamReader reader = new StreamReader(fileName))
            {
                return (Workspace)ser.Deserialize(reader);
            }
        }
    }
}
