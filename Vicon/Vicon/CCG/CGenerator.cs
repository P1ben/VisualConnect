using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Viscon.Model;
using Viscon.Model.Nodes;

namespace Viscon.CCG
{
    public class CGenerator
    {
        /*CHAR,
        INT,
        FLOAT,
        DOUBLE,
        LONG,
        SHORT,
        VOID*/
        public static string[] CTypes = new string[]
        {
            "char", "int", "float", "double", "long", "short", "bool", "const char*", "void"
        };

        public static bool Warmup = false;

        private static List<string> InitFunctions()
        {
            Orchestrator.GetFunctions().ForEach(x => x.AlreadyGenerated = false);
            List<string> ret = new List<string>();
            var funs = Orchestrator.GetFunctions().Where(x => x.Type != Model.Nodes.Enums.CDataTypes.VOID);
            foreach (var function in funs)
            {
                string signat = $"{CTypes[(int)function.Type]} function{function.ID}_retval";
                ret.Add(
                        $"{signat};"
                    );
            }
            ret.Add("");
            return ret;
        }


        private static List<string> InitVariables()
        {
            List<string> ret = new List<string>();
            var vars = Orchestrator.GetVariables().Where(x => !x.isFunctionParam);
            foreach (var variable in vars)
            {
                string defaultVal = "0";
                if (variable.Type == Model.Nodes.Enums.CDataTypes.BOOL) defaultVal = "true";
                string signat = $"{CTypes[(int)variable.Type]} {variable.GenerateCode()[0]}";
                string value;
                if(variable.Value != "")
                {
                    switch (variable.Type)
                    {
                        case Model.Nodes.Enums.CDataTypes.CHAR:
                            value = $"'{variable.Value}'";
                            break;
                        case Model.Nodes.Enums.CDataTypes.STRING:
                            value = $"\"{variable.Value}\"";
                            break;
                        default:
                            value = $"{variable.Value}";
                            break;
                    }
                }
                else
                {
                    switch (variable.Type)
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
                ret.Add(
                        $"{signat} = {value};"
                    );
            }
            ret.Add("");
            return ret;
        }

        private static void ImportDependencies(string directory, List<string> includes)
        {
            foreach (var file in includes)
            {
                if (!File.Exists(directory + "\\" + Path.GetFileName(file)))
                {
                    File.Copy(file, directory + "\\" + Path.GetFileName(file));
                }
            }
        }

        private static void GenerateHeader(string directory, string name, List<string> includes)
        {
            List<string> header_code = new List<string>();

            // Include guard
            header_code.Add($"#ifndef {name.ToUpper()}_H_INCLUDED");
            header_code.Add($"#define {name.ToUpper()}_H_INCLUDED");
            header_code.Add($"");

            // Includes
            header_code.Add("#include <stdbool.h>");
            foreach (var file in includes)
            {
                header_code.Add($"#include \"{Path.GetFileName(file)}\"");
            }
            header_code.Add($"");

            // Function signature
            header_code.Add(Orchestrator.GetFunctionSignature() + ";");
            header_code.Add($"");
            header_code.Add($"#endif");
            File.WriteAllLines(directory + "\\" + name + ".h", header_code);
        }

        private static List<string> GenerateMain()
        {
            List<string> main = new List<string>();
            main.Add("int main() {");
            main.Add($"\t{Orchestrator.GetFunctionFilledWithValues()}");
            main.Add("}");
            return main;
        }

        private static void GenerateSource(string directory, string name, bool gen_main = false)
        {
            var nextNode = Orchestrator.GetBeginNode();
            //nextNode = Orchestrator.workspace.connectables[0];
            List<string> generatedCode = new List<string>();

            Orchestrator.workspace.connectables.ForEach(x => x.Generated = false);

            generatedCode.Add($"#include \"{name}.h\"");
            generatedCode.Add("");
            // TODO: ADD FUNCTION HEADER
            generatedCode.Add(Orchestrator.GetFunctionSignature() + " {");

            generatedCode.AddRange(InitVariables().Select(x => "\t" + x));
            generatedCode.AddRange(InitFunctions().Select(x => "\t" + x));

            // Generator Warmup Round
            Warmup = true;
            while (nextNode != null)
            {
                nextNode.GenerateCode();
                nextNode = Orchestrator.NextFlowItem(nextNode);
            }

            // Reset environment
            Orchestrator.workspace.connectables.ForEach(x => x.Generated = false);
            nextNode = Orchestrator.GetBeginNode();
            Warmup = false;

            // Code Generation
            while (nextNode != null)
            {
                var tmNode = nextNode.GenerateCode();
                if (tmNode != null)
                {
                    generatedCode.AddRange(tmNode.Select(x => "\t" + x));
                }
                nextNode = Orchestrator.NextFlowItem(nextNode);
            }
            generatedCode.Add("}");
            generatedCode.Add("");
            generatedCode.Add("");
            if (gen_main)
                generatedCode.AddRange(GenerateMain());
            File.WriteAllLines(directory + "\\" + name + ".c", generatedCode);
        }

        private static void GenerateMakefile(string directory)
        {
            List<string> generatedCode = new List<string>();
            generatedCode.Add("CC := gcc");
            generatedCode.Add("CFLAGS := -Wall -pedantic -g -std=c99 -O0 ");
            generatedCode.Add("");
            generatedCode.Add("HEADERS := $(wildcard *.h)");
            generatedCode.Add("SOURCES := $(wildcard *.c)");
            generatedCode.Add("OBJECTS := $(SOURCES:%.c=%.o)");
            generatedCode.Add("");
            generatedCode.Add("main: $(OBJECTS)");
            generatedCode.Add("\t$(CC) $^ -g -o $@");
            generatedCode.Add("");
            generatedCode.Add("%.o: %.c $(HEADERS)");
            generatedCode.Add("\t$(CC) $(CFLAGS) -c $<");
            generatedCode.Add("");
            generatedCode.Add("clean:");
            generatedCode.Add("\trm -rf $(OBJECTS) main");
            File.WriteAllLines(directory + "\\Makefile", generatedCode);
        }

        public static void Generate(string directory, string file_name, bool gen_main = false, bool gen_make = false)
        {
            var includes = Orchestrator.GetImportedFiles();
            ImportDependencies(directory, includes);
            GenerateHeader(directory, file_name, includes);
            GenerateSource(directory, file_name, gen_main);
            if (gen_make)
                GenerateMakefile(directory);
        }
    }
}
