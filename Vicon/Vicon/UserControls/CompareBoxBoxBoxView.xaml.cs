using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Viscon.Model.Connectables;
using Viscon.Model.Nodes;
using Viscon.Model.Nodes.Enums;

namespace Viscon.UserControls
{
    /// <summary>
    /// Interaction logic for CompareBoxBoxBox.xaml
    /// </summary>
    public partial class CompareBoxBoxBoxView : UserControl, ComInterface
    {
        List<LineEndpoint> Connections = new List<LineEndpoint>();
        List<Border> LineOwners = new List<Border>();
        List<Border> borders = new List<Border>();
        MainWindow main_window;
        int zIndex = 0;
        double x = -1, y = -1;
        Border active;
        CompareBox node;

        public long GetModelId()
        {
            return node.ID;
        }

        public CompareBoxBoxBoxView(MainWindow _main_window)
        {
            InitializeComponent();
            main_window = _main_window;
            zIndex = Panel.GetZIndex(this);
            borders.Add(DataInLeft);
            borders.Add(DataInRight);
            borders.Add(DataOut);
        }

        public void SetNode(CompareBox node)
        {
            this.node = node;
            if (node != null)
            {
                this.type.SelectedItem = this.type.Items.GetItemAt((int)node.arithOperator);
                this.Negate.IsChecked = node.negated;
            }
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

        private void VariableA_MouseDown(object sender, MouseButtonEventArgs e)
        {
            main_window.RegisterEndpoint(node.dataInLeft, ConnectionType.Data);
        }

        private void VariableB_MouseDown(object sender, MouseButtonEventArgs e)
        {
            main_window.RegisterEndpoint(node.dataInRight, ConnectionType.Data);
        }

        private void DataOut_MouseDown(object sender, MouseButtonEventArgs e)
        {
            main_window.RegisterEndpoint(node.dataOut, ConnectionType.Data);
        }

        public List<(Border border, Connectable connectable, ConnectionType conn_type)> GetParamsMap()
        {
            List<(Border border,
                  Connectable connectable,
                  ConnectionType conn_type)> paramsMap = new List<(Border, Connectable, ConnectionType)>();

            if (node == null) return paramsMap;

            paramsMap.Add((DataInLeft, node.dataInLeft, ConnectionType.Data));
            paramsMap.Add((DataInRight, node.dataInRight, ConnectionType.Data));
            paramsMap.Add((DataOut, node.dataOut, ConnectionType.Data));

            return paramsMap;
        }

        public void setLine(LineEndpoint line, FlowParameter f)
        {
            if (node.dataInLeft.ID == f.ID) { active = borders[0]; }
            else if (node.dataInRight.ID == f.ID) { active = borders[1]; }
            else if (node.dataOut.ID == f.ID) { active = borders[2]; }
            LineOwners.Add(active); Connections.Add(line);
            active.Background = line.Line.Stroke;
        }
        public void setLine(LineEndpoint line, DataParameter f)
        {
            if (node.dataInLeft.ID == f.ID) { active = borders[0]; }
            else if (node.dataInRight.ID == f.ID) { active = borders[1]; }
            else if (node.dataOut.ID == f.ID) { active = borders[2]; }
            LineOwners.Add(active); Connections.Add(line);
            active.Background = line.Line.Stroke;
        }
        public Point getActiveLocation() { return active.TransformToAncestor(main_window).Transform(new Point(25, 25)); }

        public double GetPosX()
        {
            return this.Margin.Left;
        }

        private void type_Selected(object sender, RoutedEventArgs e)
        {
            if(node != null) node.arithOperator = (CompareOperator)Enum.Parse(typeof(CompareOperator), (string)((ComboBoxItem)(sender)).Tag);
        }

        public double GetPosY()
        {
            return this.Margin.Top;
        }

        private void negate_checked(object sender, RoutedEventArgs e)
        {
            if (node != null)
            {
                node.negated = true;
            }
        }

        private void negate_unchecked(object sender, RoutedEventArgs e)
        {
            if (node != null)
            {
                node.negated = false;
            }
        }
    }
}
