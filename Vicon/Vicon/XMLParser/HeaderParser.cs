using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Viscon.Model.Connectables;
using Viscon.Model.Nodes;

namespace Viscon.XMLParser
{
    public class HeaderParser
    {
        public List<Function> Parse(string path)
        {
            if (!File.Exists(path))
            {
                Console.WriteLine($"File {path} cannot be found.");
                return null;
            }
            string[] lines = File.ReadAllLines(path);

            // Exclude lines with ';'
            lines = lines
                     .Where(x => !x.Contains(';'))
                     .ToArray();

            // Exclude lines starting with ' '
            lines = lines
                     .Where(x => !x.StartsWith(" "))
                     .ToArray();

            // Exclude defines
            lines = lines
                     .Where(x => !x.Contains('#'))
                     .ToArray();

            // Only 1 parentheses allowed
            lines = lines
                     .Where(x => x.Count(y => y == '(') == 1)
                     .Where(x => x.Count(y => y == ')') == 1)
                     .ToArray();

            // Put space in front of '('
            lines = lines
                     .Select(x => x[x.IndexOf('(') - 1] != ' ' ? x.Insert(x.IndexOf('('), " ") : x)
                     .ToArray();

            // lines.ToList().ForEach(x => Console.WriteLine(x));

            List<Function> functions = new List<Function>();

            //foreach (string line in lines)
            //{
            //    Function temp = new Function(GetName(line), GetType(line),
            //        GetParams(line).Select(x => new DataParameter(x.Name, x.Type)).ToList());
            //    functions.Add(temp);
            //}
            return functions;
        }

        string GetType(string line)
        {
            return line.Split(' ')[0];
        }

        string GetName(string line)
        {
            return line.Split(' ')[1];
        }

        List<DataParameter> GetParams(string line)
        {
            List<DataParameter> parameters = new List<DataParameter>();
            string param_str = line
                                .Substring(line.IndexOf('('))
                                .Replace("(", "")
                                .Replace(")", "");
            string[] parsed = param_str.Split(',').Select(x => x.Trim()).ToArray();
            foreach (string str in parsed)
            {
                
                if(str == "...")
                {
                    string name = "...";
                    string type = "...";
                    //parameters.Add(new DataParameter(name, type));
                }
                else
                {
                    string type = str.Substring(0, str.LastIndexOf(' '));
                    string name = str.Substring(str.LastIndexOf(" ")).Trim();

                    while (name.StartsWith("*"))
                    {
                        name = name.Substring(1);
                        type += '*';
                    }
                    //parameters.Add(new DataParameter(name, type));
                }
            }

            return parameters;
        }
    }
}
