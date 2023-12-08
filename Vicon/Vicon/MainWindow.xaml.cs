using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Viscon.CCG;
using Viscon.Model;
using Viscon.Model.Connectables;
using Viscon.Model.Connections;
using Viscon.Model.Nodes;
using Viscon.Model.Nodes.Enums;
using Viscon.UserControls;
using static MaterialDesignThemes.Wpf.Theme;

namespace Viscon
{
    /*
     * Szinek:
     *  FlowIn:  Background="#FF633c3c"
     *  FlowOut: Background="#FF4d0f0f"
     *  Data:    Background="#FF326e8a"
     */

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public enum LineType{
            DATA,
            FLOW
        }

        string fileName = "";

        private static SolidColorBrush flowInColor = new SolidColorBrush();
        private static SolidColorBrush flowOutColor = new SolidColorBrush();
        private static SolidColorBrush dataColor = new SolidColorBrush();


        List<ComInterface>  controls = new List<ComInterface>();
        List<(Line line, Border border1, Border border2)> lines = new List<(Line, Border, Border)>();
        List<(Connectable connectable,
              ConnectionType type)>   registered_params = new List<(Connectable, ConnectionType)> ();
        List<(Border border,
              Connectable connectable,
              ConnectionType conn_type)> borderConnectableMap = new List<(Border, Connectable, ConnectionType)>();
        List<Line> Datalines = new List<Line>();
        List<Line> Controllines = new List<Line>();
        LineType   currentLineType = LineType.DATA;
        public UserControl currentlyDragged = null;
        private int ZIndex = 0;
        private bool gen_main = false;
        private bool gen_make = false;
        public MainWindow()
        {
            InitializeComponent();

            // Param default colors
            flowInColor.Color = (Color)ColorConverter.ConvertFromString("#633c3c");
            flowOutColor.Color = (Color)ColorConverter.ConvertFromString("#4d0f0f");
            dataColor.Color = (Color)ColorConverter.ConvertFromString("#326e8a");


            Orchestrator.mainWindow = this;
            //Orchestrator.LoadFunction(Directory.GetCurrentDirectory() + "\\test_save\\test.xml");
        }
        #region Addolók
        private void generate_Click(object sender, RoutedEventArgs e)
        {
            if (Orchestrator.generator_dir == "")
            {
                string dir_path = "";
                OpenFileDialog folderBrowser = new OpenFileDialog();
                folderBrowser.ValidateNames = false;
                folderBrowser.CheckFileExists = false;
                folderBrowser.CheckPathExists = true;
                folderBrowser.FileName = "Folder Selection";
                if (folderBrowser.ShowDialog() == true)
                {
                    dir_path = System.IO.Path.GetDirectoryName(folderBrowser.FileName);
                    Orchestrator.generator_dir = dir_path;
                    CGenerator.Generate(Orchestrator.generator_dir, Orchestrator.workspace.Name, gen_main, gen_make);
                }
            }
            else
            {
                CGenerator.Generate(Orchestrator.generator_dir, Orchestrator.workspace.Name, gen_main, gen_make);
            }
        }

        private void loadFunction_Click(object sender, RoutedEventArgs e)
        {
            Orchestrator.mainWindow = this;
            Orchestrator.LoadFunction(Directory.GetCurrentDirectory() + "\\test_save\\test.xml");
        }

        private void addBranch_click(object sender, RoutedEventArgs e)
        {
            branch_create(225, 75, null);
        }

        private void addVariable_Click(object sender, RoutedEventArgs e)
        {
            variable_create(225, 75, null);
        }

        private void addAssignment_Click(object sender, RoutedEventArgs e)
        {
            assignment_create(225, 75, null);
        }

        [Obsolete("Do not the function")]
        private void addFunction_Click(object sender, RoutedEventArgs e)
        {
        }

        private void addAritmetic_Click(object sender, RoutedEventArgs e)
        {
            arithmetic_create(225, 75, null);
        }

        private void addCompareBox_Click(object sender, RoutedEventArgs e)
        {
            comparebox_create(225, 75, null);
        }

        private void addLogicConn_Click(object sender, RoutedEventArgs e)
        {
            logicbox_create(225, 75, null);
        }

        private void addLoop_Click(object sender, RoutedEventArgs e)
        {
            loop_create(225, 75, null);
        }
        private void addBegin_Click(object sender, RoutedEventArgs e)
        {
            beginbox_create(225, 75, null);
        }
        private void addReturnNode_Click(object sender, RoutedEventArgs e)
        {
            returnbox_create(225, 75, null);
        }
        private void addEnd_Click(object sender, RoutedEventArgs e)
        {
            endbox_create(225, 75, null);
        }
        #endregion
        #region Ujabb_addolok

        public void loop_create(double x, double y, Loop node)
        {
            var control = new UserControls.LoopView(this);
            if (node == null)
            {
                node = new Loop();
            }
            control.MouseRightButtonDown += (a, b) =>
            {
                Orchestrator.DeleteNodeWithConnections(node);
                workSheet.Children.Remove(control);
                controls.Remove(control);
                registered_params.Clear();
                UpdateBorderSelectedColor();
            };
            Orchestrator.CreateElement(control, node);
            control.SetNode(node);
            control.Margin = new Thickness(x, y, 0, 0);
            workSheet.Children.Add(control);
            controls.Add(control);
            ReloadBorderConnectableMap();
        }
        public void branch_create(double x, double y, Branch node)
        {
            var control = new UserControls.BranchView(this);
            if (node == null)
            {
                node = new Branch();
            }
            control.MouseRightButtonDown += (a, b) =>
            {
                Orchestrator.DeleteNodeWithConnections(node);
                workSheet.Children.Remove(control);
                controls.Remove(control);
                registered_params.Clear();
                UpdateBorderSelectedColor();
            };
            Orchestrator.CreateElement(control, node);
            control.SetNode(node);
            control.Margin = new Thickness(x, y, 0, 0);
            workSheet.Children.Add(control);
            controls.Add(control);
            ReloadBorderConnectableMap();
        }
        public void variable_create(double x, double y, Variable node)
        {
            var control = new UserControls.VariableView(this);
            if (node == null)
            {
                node    = new Variable(Model.Nodes.Enums.CDataTypes.INT, "");
            }
            control.MouseRightButtonDown += (a, b) =>
            {
                Orchestrator.DeleteNodeWithConnections(node);
                workSheet.Children.Remove(control);
                controls.Remove(control);
                registered_params.Clear();
                UpdateBorderSelectedColor();
            };
            Orchestrator.CreateElement(control, node);
            control.SetNode(node);
            control.Margin = new Thickness(x, y, 0, 0);
            workSheet.Children.Add(control);
            controls.Add(control);
            ReloadBorderConnectableMap();
        }
        public void assignment_create(double x, double y, Assignment node)
        {
            var control = new UserControls.AssignmentView(this);
            if (node == null)
            {
                node    = new Assignment();
            }
            control.MouseRightButtonDown += (a, b) =>
            {
                Orchestrator.DeleteNodeWithConnections(node);
                workSheet.Children.Remove(control);
                controls.Remove(control);
                registered_params.Clear();
                UpdateBorderSelectedColor();
            };
            Orchestrator.CreateElement(control, node);
            control.SetNode(node);
            control.Margin = new Thickness(x, y, 0, 0);
            workSheet.Children.Add(control);
            controls.Add(control);
            ReloadBorderConnectableMap();
        }
        public void function_create(double x, double y, Function node, FunctionSignature sign)
        {
            var control = new UserControls.FunctionView(this);
            if (node == null && sign != null)
            {
                node = Function.Replicate(sign);
            }
            control.MouseRightButtonDown += (a, b) =>
            {
                Orchestrator.DeleteNodeWithConnections(node);
                workSheet.Children.Remove(control);
                controls.Remove(control);
                registered_params.Clear();
                UpdateBorderSelectedColor();
            };
            Orchestrator.CreateElement(control, node);
            control.SetNode(node);
            control.Margin = new Thickness(x, y, 0, 0);
            workSheet.Children.Add(control);
            controls.Add(control);
            ReloadBorderConnectableMap();
        }
        public void arithmetic_create(double x, double y, Arithmetic node)
        {
            var control = new UserControls.AritmeticView(this);
            if (node == null)
            {
                node    = new Arithmetic();
            }
            control.MouseRightButtonDown += (a, b) =>
            {
                Orchestrator.DeleteNodeWithConnections(node);
                workSheet.Children.Remove(control);
                controls.Remove(control);
                registered_params.Clear();
                UpdateBorderSelectedColor();
            };
            Orchestrator.CreateElement(control, node);
            control.SetNode(node);
            control.Margin = new Thickness(x, y, 0, 0);
            workSheet.Children.Add(control);
            controls.Add(control);
            ReloadBorderConnectableMap();
        }
        public void comparebox_create(double x, double y, CompareBox node)
        {
            var control = new UserControls.CompareBoxBoxBoxView(this);
            if (node == null)
            {
                node    = new CompareBox();
            }
            control.MouseRightButtonDown += (a, b) =>
            {
                Orchestrator.DeleteNodeWithConnections(node);
                workSheet.Children.Remove(control);
                controls.Remove(control);
                registered_params.Clear();
                UpdateBorderSelectedColor();
            };
            Orchestrator.CreateElement(control, node);
            control.SetNode(node);
            control.Margin = new Thickness(x, y, 0, 0);
            workSheet.Children.Add(control);
            controls.Add(control);
            ReloadBorderConnectableMap();
        }
        public void logicbox_create(double x, double y, LogicBox node)
        {
            var control = new UserControls.LogicConnectionView(this);
            if (node == null)
            {
                node    = new LogicBox();
            }
            control.MouseRightButtonDown += (a, b) =>
            {
                Orchestrator.DeleteNodeWithConnections(node);
                workSheet.Children.Remove(control);
                controls.Remove(control);
                registered_params.Clear();
                UpdateBorderSelectedColor();
            };
            Orchestrator.CreateElement(control, node);
            control.SetNode(node);
            control.Margin = new Thickness(x, y, 0, 0);
            workSheet.Children.Add(control);
            controls.Add(control);
            ReloadBorderConnectableMap();
        }
        public void beginbox_create(double x, double y, BeginNode node) {
            var control = new UserControls.BeginView(this);
            if (node == null)
            {
                node = new BeginNode();
            }
            control.MouseRightButtonDown += (a, b) =>
            {
                Orchestrator.DeleteNodeWithConnections(node);
                workSheet.Children.Remove(control);
                controls.Remove(control);
                registered_params.Clear();
                UpdateBorderSelectedColor();
            };
            Orchestrator.CreateElement(control, node);
            control.SetNode(node);
            control.Margin = new Thickness(x, y, 0, 0);
            workSheet.Children.Add(control);
            controls.Add(control);
            ReloadBorderConnectableMap();
        }
        public void returnbox_create(double x, double y, ReturnNode node)
        {
            var control = new UserControls.ReturnNodeView(this);
            if (node == null)
            {
                node = new ReturnNode();
            }
            control.MouseRightButtonDown += (a, b) =>
            {
                Orchestrator.DeleteNodeWithConnections(node);
                workSheet.Children.Remove(control);
                controls.Remove(control);
                registered_params.Clear();
                UpdateBorderSelectedColor();
            };
            Orchestrator.CreateElement(control, node);
            control.SetNode(node);
            control.Margin = new Thickness(x, y, 0, 0);
            workSheet.Children.Add(control);
            controls.Add(control);
            ReloadBorderConnectableMap();
        }
        public void endbox_create(double x, double y, EndNode node)
        {
            var control = new UserControls.EndNodeView(this);
            if (node == null)
            {
                node = new EndNode();
            }
            control.MouseRightButtonDown += (a, b) =>
            {
                Orchestrator.DeleteNodeWithConnections(node);
                workSheet.Children.Remove(control);
                controls.Remove(control);
                registered_params.Clear();
                UpdateBorderSelectedColor();
            };
            Orchestrator.CreateElement(control, node);
            control.SetNode(node);
            control.Margin = new Thickness(x, y, 0, 0);
            workSheet.Children.Add(control);
            controls.Add(control);
            ReloadBorderConnectableMap();
        }
        #endregion
        #region Endpoint_Handler
        public void RegisterEndpoint(Connectable endpoint, ConnectionType connType)
        {
            var lineType = connType == ConnectionType.Data ? LineType.DATA : LineType.FLOW;

            // Second click on the same parameter clears selection
            if (registered_params.Count > 0 && registered_params[0].connectable == endpoint)
            {
                registered_params.Clear();
                UpdateBorderSelectedColor();
                return;
            }

            if (currentLineType != lineType)
            {
                registered_params.Clear();
                currentLineType = lineType;
            }

            if (registered_params.Count() == 0 || 
                ((lineType == LineType.DATA && endpoint != registered_params[0].connectable) ||
                registered_params[0].type != connType))
            {
                registered_params.Add(( endpoint, connType ));
            } 

            if (registered_params.Count() == 2)
            {
                Orchestrator.CreateConnection(registered_params[0].connectable, registered_params[1].connectable, currentLineType);
                registered_params.Clear();
            }
            UpdateBorderSelectedColor();
        }

        // Update parameter selection colors
        private void UpdateBorderSelectedColor()
        {
            foreach (var elem in borderConnectableMap.Select(x => x.border))
            {
                elem.BorderBrush = Brushes.Black;
            }

            foreach (var param in registered_params)
            {
                var border = borderConnectableMap
                                .Where(x => x.connectable == param.connectable)
                                .Select(x => x.border)
                                .FirstOrDefault();
                if (border != null)
                {
                    border.BorderBrush = Brushes.Yellow;
                } 
            }
        }

        public void ReloadBorderConnectableMap()
        {
            borderConnectableMap.Clear();
            foreach (var a in controls)
            {
                foreach (var b in a.GetParamsMap())
                {
                    borderConnectableMap.Add(b);
                }
            }
        }

        private Point GetBorderCenter(Border border)
        {
            return border.TransformToAncestor(this).Transform(new Point(12.5, 12.5));
        }

        [STAThread]
        public void ReloadLines(List<DataConnection> dataConnections, List<FlowConnection> flowConnections)
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Render, new Action(() =>
            {
                foreach (var a in lines)
                {
                    workSheet.Children.Remove(a.line);
                }
                lines.Clear();

                foreach (var dC in dataConnections)
                {
                    Border first = borderConnectableMap.Where(x => x.connectable == dC.Consumer).FirstOrDefault().border;
                    Border second = borderConnectableMap.Where(x => x.connectable == dC.Producer).FirstOrDefault().border;

                    Line conn = new Line();
                    SolidColorBrush brush = new SolidColorBrush();

                    brush.Color = (Color)ColorConverter.ConvertFromString("#144b9f");
                    conn.StrokeThickness = 3;
                    conn.Stroke = brush;
                    Point first_point = GetBorderCenter(first);
                    Point second_point = GetBorderCenter(second);

                    conn.X1 = first_point.X;
                    conn.Y1 = first_point.Y;

                    conn.MouseRightButtonDown += (x, y) =>
                    {
                        Orchestrator.DeleteConnection(dC);
                        registered_params.Clear();
                        Orchestrator.RequestWindowConnectionReload();
                        UpdateBorderSelectedColor();
                    };

                    conn.X2 = second_point.X;
                    conn.Y2 = second_point.Y;
                    Panel.SetZIndex(conn, 150);
                    lines.Add((conn, first, second));
                    workSheet.Children.Add(conn);
                    first.Background = second.Background = conn.Stroke;
                }

                foreach (var fC in flowConnections)
                {
                    Border first = borderConnectableMap.Where(x => x.connectable == fC.In).FirstOrDefault().border;
                    Border second = borderConnectableMap.Where(x => x.connectable == fC.Out).FirstOrDefault().border;

                    Line conn = new Line();
                    SolidColorBrush brush = new SolidColorBrush();

                    brush.Color = (Color)ColorConverter.ConvertFromString("#c70e20"); //GM Laser Blue
                    conn.StrokeThickness = 3;
                    conn.Stroke = brush;
                    Point first_point = GetBorderCenter(first);
                    Point second_point = GetBorderCenter(second);

                    conn.X1 = first_point.X;
                    conn.Y1 = first_point.Y;

                    conn.X2 = second_point.X;
                    conn.Y2 = second_point.Y;

                    conn.MouseRightButtonDown += (x, y) =>
                    {
                        Orchestrator.DeleteConnection(fC);
                        registered_params.Clear();
                        Orchestrator.RequestWindowConnectionReload();
                        UpdateBorderSelectedColor();
                    };

                    Panel.SetZIndex(conn, 150);
                    lines.Add((conn, first, second));
                    workSheet.Children.Add(conn);
                    first.Background = second.Background = conn.Stroke;
                }
            }));
        }
        public void ReloadLinesSoft()
        {
            foreach (var line in lines)
            {
                Point first_point = GetBorderCenter(line.border1);
                Point second_point = GetBorderCenter(line.border2);

                line.line.X1 = first_point.X;
                line.line.Y1 = first_point.Y;

                line.line.X2 = second_point.X;
                line.line.Y2 = second_point.Y;
            }
        }
        #endregion

        private void dataLine_Click(object sender, RoutedEventArgs e)
        {
            dataLine.Visibility = Visibility.Hidden;
            signalLine.Visibility = Visibility.Visible;
            currentLineType = LineType.DATA;
        }
        private void signalLine_Click(object sender, RoutedEventArgs e)
        {
            signalLine.Visibility = Visibility.Hidden;
            dataLine.Visibility = Visibility.Visible;
            currentLineType = LineType.FLOW;
        }
        public int GetZIndex()
        {
            return ZIndex;
        }
        public int getNextZIndex()
        {
            return ++ZIndex;
        }
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var a = sender as System.Windows.Controls.TextBox;
            n.Visibility = Visibility.Hidden;
        }
        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var a = sender as System.Windows.Controls.TextBox;
            if (a.Text == "") n.Visibility = Visibility.Visible;
        }

        private void create_Click(object sender, RoutedEventArgs e)
        {
            if (nev.Text.ToString() != "")
            {
                Orchestrator.NewFunction(nev.Text, GetFunctionType());
                projName.Text = nev.Text;
                menu.Visibility = Visibility.Hidden;
                sideMenu.Visibility = Visibility.Visible;
                workSheet.Visibility = Visibility.Visible;
                Storyboard sb = (Storyboard)FindResource("MenuIn");
                sb.Begin();
            }
            else MessageBox.Show("Please enter a name!");
        }

        private void load_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == true)
            {
                fileName = ofd.FileName;

                Orchestrator.LoadFunction(fileName);

                menu.Visibility = Visibility.Hidden;
                sideMenu.Visibility = Visibility.Visible;
                workSheet.Visibility = Visibility.Visible;
                Storyboard sb = (Storyboard)FindResource("MenuIn");
                sb.Begin();
            }
        }

        CDataTypes GetFunctionType()
        {
            return (CDataTypes)Enum.Parse(typeof(CDataTypes), ((ComboBoxItem)(type.SelectedItem)).Tag.ToString());
        }

        public void RefreshImported()
        {
            Orchestrator.RefreshImpots();
            var functions = Orchestrator.imported_functions;
            funcMenu.Children.Clear();
            foreach ( var f in functions )
            {
                Program.CustomMenu customMenu = new Program.CustomMenu();
                customMenu.Name.Text = f.Name;
                customMenu.button.Click += (x, y) =>
                {
                    function_create(225, 75, null, f);
                };

                funcMenu.Children.Add(customMenu);
            }
        }

        private void import_Click(object sender, RoutedEventArgs e)
        {
            if (Orchestrator.proj_dir == "")
            {
                // The user should save the project first
                return;
            }
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == true)
            {
                fileName = ofd.FileName;

                Orchestrator.ImportHeader(fileName);
                RefreshImported();
            }
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            if (Orchestrator.proj_dir == "")
            {
                string dir_path = "";
                OpenFileDialog folderBrowser = new OpenFileDialog();
                folderBrowser.ValidateNames = false;
                folderBrowser.CheckFileExists = false;
                folderBrowser.CheckPathExists = true;
                folderBrowser.FileName = "Folder Selection";
                if (folderBrowser.ShowDialog() == true)
                {
                    dir_path = System.IO.Path.GetDirectoryName(folderBrowser.FileName);
                    Orchestrator.proj_dir = dir_path;
                    RefreshImported();
                    Orchestrator.SaveFunction();
                }
            }
            else
            {
                RefreshImported();
                Orchestrator.SaveFunction();
            }
        }

        private void makeFile_Checked(object sender, RoutedEventArgs e)
        {
            gen_make = true;
        }

        private void mainGen_Checked(object sender, RoutedEventArgs e)
        {
            gen_main = true;
        }

        private void mainGen_Unchecked(object sender, RoutedEventArgs e)
        {
            gen_main = false;
        }

        private void makeFile_Unchecked(object sender, RoutedEventArgs e)
        {
            gen_make = false;
        }
    }
}
