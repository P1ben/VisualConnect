using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Viscon.Model;
using Viscon.Model.Connectables;
using Viscon.Model.Connections;
using Viscon.Model.Nodes;

namespace Viscon.Program
{
    public class Program
    {
        [System.STAThreadAttribute()]
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public static void Main()
        {

            //Workspace ws = XMLSerializer.LoadWorkspace("stdio_xml.xml");
            //ws.connectables.ForEach(x => Console.WriteLine(x.ID));
            // Init application
            //////// Workspace Load Test ////////
            //Workspace ws = XMLSerializer.LoadWorkspace("stdio_xml.xml");
            //ws.connectables.ForEach(x => Console.WriteLine(x.ID));
            ////////////////////////////////////

            //////// Load Application GUI ////////
            Orchestrator.NewFunction("test_function", Model.Nodes.Enums.CDataTypes.VOID);

            Viscon.App app = new Viscon.App();
            app.InitializeComponent();
            app.Run();
            //////////////////////////////////////

            //////// Header Parser Test ////////
            //HeaderParser hp = new HeaderParser();
            //var asd = hp.Parse("C:/msys64/mingw64/include/stdio.h");
            //asd.ForEach(x => Console.WriteLine($"Func: {x.Name}"));
            ////////////////////////////////////

            //////// Workspace Save Test ////////
            //Workspace fl = new Workspace();
            //Function f1 = new Function("printf",
            //                    "bool",
            //                    new List<DataParameter>
            //                    {
            //                        new DataParameter("asd", "char"),
            //                        new DataParameter("xd", "int")
            //                    });

            //Function f2 = new Function("scanf",
            //                    "bool",
            //                    new List<DataParameter>
            //                    {
            //                        new DataParameter("asdd", "char"),
            //                        new DataParameter("xdd", "int")
            //                    });

            //Function f3 = new Function("igne",
            //                    "bool",
            //                    new List<DataParameter>
            //                    {
            //                        new DataParameter("asdd", "char"),
            //                        new DataParameter("xdd", "int")
            //                    });

            //Variable cond = new Variable("bool", "true");
            //Branch branch = new Branch();

            //DataConnection dc = new DataConnection(f1.DataParams[0], f2.DataParams[1]);
            //DataConnection var_branch = new DataConnection(cond.Connection, branch.Condition);

            //FlowConnection f1_branch = new FlowConnection(f1.FlowOut, branch.FlowIn);
            //FlowConnection branchTrue_f2 = new FlowConnection(branch.FlowOutTrue, f2.FlowIn);
            //FlowConnection branchFalse_f3 = new FlowConnection(branch.FlowOutFalse, f3.FlowIn);

            //fl.connectables.Add(f1);
            //fl.connectables.Add(f2);
            //fl.connectables.Add(f3);
            //fl.connectables.Add(cond);
            //fl.connectables.Add(branch);

            //fl.connections.Add(dc);
            //fl.connections.Add(var_branch);
            //fl.connections.Add(f1_branch);
            //fl.connections.Add(branchTrue_f2);
            //fl.connections.Add(branchFalse_f3);

            //XMLSerializer.SaveWorkspace(fl, Directory.GetCurrentDirectory() + "\\test_save.xml");
            /////////////////////////////////////
        }
    }
}
