using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Viscon.Model.Connectables;
using Viscon.Model.Nodes;

namespace Viscon.UserControls
{
    /// <summary>
    /// Interaction logic for Assignment.xaml
    /// </summary>
    public partial class AssignmentView : UserControl, ComInterface
    {
        List<LineEndpoint> Connections = new List<LineEndpoint>();
        List<Border> LineOwners = new List<Border>();
        List<Border> borders = new List<Border>();
        MainWindow main_window;
        int zIndex = 0;
        double x = -1, y = -1;
        Border active;
        public AssignmentView(MainWindow _main_window)
        {
            InitializeComponent();
            main_window = _main_window;
            zIndex = Panel.GetZIndex(this);
            borders.Add(FlowIn);
            borders.Add(VariableA);
            borders.Add(VariableB);
            borders.Add(FlowOut);
        }

        Assignment node;

        public long GetModelId()
        {
            return node.ID;
        }

        public void SetNode(Assignment node)
        {
            this.node = node;
        }

        private void Border_MouseMove_1(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && (main_window.currentlyDragged == null || main_window.currentlyDragged == this))
            {
                if (x == -1)
                {
                    x = e.GetPosition(this).X;
                    y = e.GetPosition(this).Y;
                }
                Mouse.OverrideCursor = Cursors.Hand;
                Panel.SetZIndex(this, 100);
                Canvas c = this.Parent as Canvas;
                this.MaxHeight = this.ActualHeight;
                this.MaxWidth = this.ActualWidth;
                Point mousePoint = e.GetPosition(main_window);
                this.Margin = new Thickness((mousePoint.X - x), (mousePoint.Y - y), 0, 0);
                main_window.ReloadLinesSoft();
                if (main_window.currentlyDragged == null)
                    main_window.currentlyDragged = this;
            }
            else
            {
                if (main_window.currentlyDragged == this)
                {
                    Panel.SetZIndex(this, main_window.getNextZIndex());
                    main_window.currentlyDragged = null;
                }

                x = -1;
                y = -1;




            }
        }

        public object getParent() { return this.Parent; }

        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Arrow;
            x = -1;
            y = -1;

        }

        private void SignalIn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            main_window.RegisterEndpoint(node.FlowIn, ConnectionType.FlowIn);
        }

        private void VariableA_MouseDown(object sender, MouseButtonEventArgs e)
        {
            main_window.RegisterEndpoint(node.DataInLeft, ConnectionType.Data);
        }

        private void VariableB_MouseDown(object sender, MouseButtonEventArgs e)
        {
            main_window.RegisterEndpoint(node.DataInRight, ConnectionType.Data);
        }

        private void SignalOut_MouseDown(object sender, MouseButtonEventArgs e)
        {
            main_window.RegisterEndpoint(node.FlowOut, ConnectionType.FlowOut);
        }

        public List<(Border border, Connectable connectable, ConnectionType conn_type)> GetParamsMap()
        {
            List<(Border border,
                  Connectable connectable,
                  ConnectionType conn_type)> paramsMap = new List<(Border, Connectable, ConnectionType)>();

            if (node == null) return paramsMap;

            paramsMap.Add((FlowIn, node.FlowIn, ConnectionType.FlowIn));
            paramsMap.Add((FlowOut, node.FlowOut, ConnectionType.FlowOut));

            paramsMap.Add((VariableA, node.DataInLeft, ConnectionType.Data));
            paramsMap.Add((VariableB, node.DataInRight, ConnectionType.Data));

            return paramsMap;
        }

        public void setLine(LineEndpoint line, FlowParameter f)
        {
            if (node.FlowIn.ID == f.ID) { active = borders[0]; }
            else if (node.DataInLeft.ID == f.ID) { active = borders[1]; }
            else if (node.DataInRight.ID == f.ID) { active = borders[2]; }
            else if (node.FlowOut.ID == f.ID) { active = borders[3]; }
            LineOwners.Add(active); Connections.Add(line);
            active.Background = line.Line.Stroke;
        }
        public void setLine(LineEndpoint line, DataParameter f)
        {
            if (node.FlowIn.ID == f.ID) { active = borders[0]; }
            else if (node.DataInLeft.ID == f.ID) { active = borders[1]; }
            else if (node.DataInRight.ID == f.ID) { active = borders[2]; }
            else if (node.FlowOut.ID == f.ID) { active = borders[3]; }
            LineOwners.Add(active); Connections.Add(line);
            active.Background = line.Line.Stroke;
        }
        public Point getActiveLocation() { return active.TransformToAncestor(main_window).Transform(new Point(25, 25)); }
        public double GetPosX()
        {
            return this.Margin.Left;
        }

        public double GetPosY()
        {
            return this.Margin.Top;
        }
    }

}
