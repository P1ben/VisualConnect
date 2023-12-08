using MaterialDesignThemes.Wpf.Converters.CircularProgressBar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Viscon.Model;
using Viscon.Model.Connectables;
using Viscon.Model.Connections;
using Viscon.Model.Nodes;
using Viscon.UserControls;
using System.Threading;
using System.Windows.Threading;
using System.Windows;
using System.Security.Cryptography.X509Certificates;
using Viscon.Model.Nodes.Enums;
using Viscon.CCG;
using System.Net;

namespace Viscon
{
    public static class Orchestrator
    {
        static long NextID = 0;

        public static string generator_dir = "";
        public static string proj_dir = "";
        public static Workspace workspace;
        static NodePositionHandler layout;
        static List<ComInterface> graphicsElements;
        public static MainWindow mainWindow = null;

        public static List<FunctionSignature> imported_functions = new List<FunctionSignature>();


        public static void NewFunction(string name, CDataTypes ret)
        {
            NextID = 0;
            workspace = new Workspace(name, ret);
            layout    = new NodePositionHandler();
            graphicsElements = new List<ComInterface>();
        }

        private static Connectable GetParamById(long ID)
        {
            Connectable conn = null;
            foreach (var node in workspace.connectables)
            {
                if (node.GetParameterById(ID) != null) conn = node.GetParameterById(ID);
            }
            return conn;
        }

        public static void LoadFunction(string file_path)
        {
            if(File.Exists(file_path) && File.Exists(file_path.Replace(".xml", "_layout.xml")))
            {
                proj_dir = Path.GetDirectoryName(file_path);
                workspace = XMLSerializer.LoadWorkspace(file_path);
                layout = XMLSerializer.LoadLayout(file_path.Replace(".xml", "_layout.xml"));
                foreach (Node node in workspace.connectables)
                {
                    (double x, double y) pos = layout.GetPositionById(node.ID);
                    node.GenerateGraphicObject(mainWindow, pos.x, pos.y);
                    node.UpdateParams();
                }
                // restore connections
                foreach (var conn in workspace.dataConnections)
                {
                    Connectable consumer = GetParamById(conn.ConsumerID);
                    Connectable producer = GetParamById(conn.ProducerID);
                    if (consumer == null || producer == null)
                        workspace.dataConnections.Remove(conn);
                    else
                    {
                        conn.Consumer = (DataParameter)consumer;
                        conn.Producer = (DataParameter)producer;
                    }
                }

                foreach (var conn in workspace.flowConnections)
                {
                    Connectable flowIn = GetParamById(conn.FlowInID);
                    Connectable flowOut = GetParamById(conn.FlowOutID);
                    if (flowIn == null || flowOut == null)
                        workspace.flowConnections.Remove(conn);
                    else
                    {
                        conn.In =  (FlowParameter)flowIn;
                        conn.Out = (FlowParameter)flowOut;
                    }
                }
                LoadId();
                RefreshImpots();
                RequestWindowConnectionReload();
                mainWindow.RefreshImported();
            }
        }

        public static void SaveFunction()
        {
            layout = new NodePositionHandler();
            graphicsElements.ForEach(x => layout.AddNode(x.GetModelId(), x.GetPosX(), x.GetPosY()));
            XMLSerializer.SaveWorkspace(workspace, proj_dir + $"\\{workspace.Name}.xml");
            XMLSerializer.SaveLayout(layout, proj_dir + $"\\{workspace.Name}_layout.xml");
        }

        public static void UpdateGraphics()
        {
            //graphicsElements.ForEach(x => x.Update());
        }

        public static void CreateElement(ComInterface newElement, Node node)
        {
            graphicsElements.Add(newElement);
            if (!workspace.connectables.Contains(node))
                workspace.AddNode(node);
        }

        public static long GetNextId()
        {
            return NextID++;
        }

        public static void ResetId()
        {
            NextID = 0;
        }

        public static string GetFunctionSignature()
        {
            if (workspace == null) return "NULL";
            var input_variables = GetVariables()
                                  .Where(x => x.isFunctionParam)
                                  .Select(x => $"{CGenerator.CTypes[(int)x.Type]} {x.GenerateCode()[0]}");
            return $"{CGenerator.CTypes[(int)workspace.ReturnValue]} " +
                   $"{workspace.Name}({String.Join(", ", input_variables)})";
        }

        public static string GetFunctionFilledWithValues()
        {
            if (workspace == null) return "NULL";
            var input_variables = GetVariables()
                                  .Where(x => x.isFunctionParam);
            List<string> vals = new List<string>();
            foreach ( var x in input_variables )
            {
                string defaultVal = "0";
                if (x.Type == Model.Nodes.Enums.CDataTypes.BOOL) defaultVal = "true";
                string value;
                if (x.Value != "")
                {
                    switch (x.Type)
                    {
                        case Model.Nodes.Enums.CDataTypes.CHAR:
                            value = $"'{x.Value}'";
                            break;
                        case Model.Nodes.Enums.CDataTypes.STRING:
                            value = $"\"{x.Value}\"";
                            break;
                        default:
                            value = $"{x.Value}";
                            break;
                    }
                }
                else
                {
                    switch (x.Type)
                    {
                        case Model.Nodes.Enums.CDataTypes.CHAR:
                            value = $"'{defaultVal}'";
                            break;
                        case Model.Nodes.Enums.CDataTypes.STRING:
                            value = $"\"{defaultVal}\"";
                            break;
                        default:
                            value = $"{defaultVal}";
                            break;
                    }
                }
                vals.Add(value);
            }
            return $"{((workspace.ReturnValue == CDataTypes.VOID) ? "" : "(void)")} " +
                   $"{workspace.Name}({String.Join(", ", vals)});";
        }

        public static void LoadId()
        {
            long max = 0;
            foreach(var elem in workspace.connectables)
            {
                if(elem.ID > max)
                {
                    max = elem.ID;
                }
            }

            foreach (var elem in workspace.dataConnections)
            {
                if (elem.ID > max)
                {
                    max = elem.ID;
                }
            }

            foreach (var elem in workspace.flowConnections)
            {
                if (elem.ID > max)
                {
                    max = elem.ID;
                }
            }
            NextID = max + 1;
        }

        public static void CreateConnection(Connectable first, Connectable second, MainWindow.LineType type)
        {
            // Don't connect a node to itself
            if (first.parent == second.parent) return;

            // Don't allow same connection twice
            if (workspace.ConnectionExists(first, second)) return;

            if (type == MainWindow.LineType.DATA)
            {
                workspace.dataConnections.Add(new DataConnection((DataParameter)first, (DataParameter)second));
            }
            else
            {
                workspace.flowConnections.Add(new FlowConnection((FlowParameter)first, (FlowParameter)second));
            }
            RequestWindowConnectionReload();
        }

        public static void RequestWindowConnectionReload()
        {
            mainWindow.ReloadBorderConnectableMap();
            mainWindow.ReloadLines(workspace.dataConnections, workspace.flowConnections);
        }
        public static Node NextFlowItem(Node currentItem)
        {
            FlowParameter outputParam = currentItem.getOutputFlowParam();
            return NextFlowConnection(outputParam);
        }

        public static Node NextFlowConnection(FlowParameter currentItem)
        {
            if (currentItem == null) return null;
            Node parent = null;
            foreach (var x in workspace.flowConnections)
            {
                if (x.In == currentItem)
                {
                    parent = x.Out.parent;
                    break;
                }
                if (x.Out == currentItem)
                {
                    parent = x.In.parent;
                    break;
                }
            }
            if (parent.Generated)
            {
                parent = NextFlowConnection(parent.getOutputFlowParam());
            }
            return parent;
        }
        

        public static Node GetDataNames(DataParameter param)
        {
            foreach (var x in workspace.dataConnections)
            {
                if (x.Producer == param) return x.Consumer.parent;
                if (x.Consumer == param) return x.Producer.parent;
            }
            return null;
        }
        public static Node GetBeginNode()
        {
            var tmp = workspace.connectables.Where(x => x.TypeInformer() == Model.Nodes.Enums.NodeType.Begin).ToArray();
            if (tmp.Count() == 0 || tmp.Count() > 1) return null;
            return tmp[0];
        }

        public static List<Variable> GetVariables()
        {
            if (workspace == null) return new List<Variable>();
            return workspace.connectables.Where(x => x.TypeInformer() == NodeType.Variable).Cast<Variable>().ToList();
        }

        public static List<Function> GetFunctions()
        {
            if (workspace == null) return new List<Function>();
            return workspace.connectables.Where(x => x.TypeInformer() == NodeType.Function).Cast<Function>().ToList();
        }

        public static void DeleteConnection(Connection conn)
        {
            if (workspace == null) return;
            if (workspace.dataConnections.Contains(conn))
            {
                workspace.dataConnections.Remove((DataConnection)conn);
            }

            if (workspace.flowConnections.Contains(conn))
            {
                workspace.flowConnections.Remove((FlowConnection)conn);
            }
            RequestWindowConnectionReload();
        }

        public static void DeleteNodeWithConnections(Node node)
        {
            if (workspace == null) return;
            bool found = true;
            while (found)
            {
                found = false;
                foreach (var conn in workspace.dataConnections)
                {
                    if (conn.Consumer.parent == node || conn.Producer.parent == node)
                    {
                        workspace.dataConnections.Remove(conn);
                        found = true;
                        break;
                    }
                }

                foreach (var conn in workspace.flowConnections)
                {
                    if (conn.In.parent == node || conn.Out.parent == node)
                    {
                        workspace.flowConnections.Remove(conn);
                        found = true;
                        break;
                    }
                }
            }

            workspace.connectables.Remove(node);
            RequestWindowConnectionReload();
        }

        private static string CopyImport(string file_name)
        {
            if(!Directory.Exists(proj_dir + "\\imports"))
            {
                Directory.CreateDirectory(proj_dir + "\\imports");
            }
            if (!File.Exists(proj_dir + "\\imports\\" + Path.GetFileName(file_name)))
                File.Copy(file_name, proj_dir + "\\imports\\" + Path.GetFileName(file_name));
            return Path.GetFileName(proj_dir + "\\imports\\" + Path.GetFileName(file_name));
        }

        public static void ImportHeader(string file_name)
        {;
            if (file_name.EndsWith(".h"))
            {
                workspace.imports.Add(new Import(CopyImport(file_name), true));
            }
            RefreshImpots();
        }

        public static void RefreshImpots()
        {
            imported_functions.Clear();
            foreach (var imp in workspace.imports)
            {
                string file = imp.Name;
                if (File.Exists(proj_dir + "\\imports\\" + file))
                {
                    var lines = File.ReadAllLines(proj_dir + "\\imports\\" + file).ToList();
                    lines = lines.Where(x => x.StartsWith("//"))
                        .Select(x => x.Replace("//", "").Trim())
                        .Where(x => x.StartsWith("[") && x.EndsWith("]")).ToList();

                    lines.ForEach(x => imported_functions.Add(new FunctionSignature(x)));
                }
                else
                {
                    workspace.imports.Remove(imp);
                }
            }
        }

        public static List<string> GetImportedFiles()
        {
            List<string> imported = new List<string>();
            foreach (var imp in workspace.imports)
            {
                if (File.Exists(proj_dir + "\\imports\\" + imp.Name))
                    imported.Add(proj_dir + "\\imports\\" + imp.Name);
            }
            return imported;
        }
    }
}
