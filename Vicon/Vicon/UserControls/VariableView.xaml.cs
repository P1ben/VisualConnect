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
    /// Interaction logic for VariableView.xaml
    /// </summary>
    public partial class VariableView : UserControl, ComInterface
    {
        List<LineEndpoint> Connections = new List<LineEndpoint>();
        List<Border> LineOwners = new List<Border>();
        List<Border> borders = new List<Border>();
        MainWindow main_window;
        int zIndex = 0;
        double x = -1, y = -1;
        Border active;
        Variable node;

        public long GetModelId()
        {
            return node.ID;
        }

        public VariableView(MainWindow _main_window)
        {   
            InitializeComponent();
            main_window = _main_window;
            zIndex = Panel.GetZIndex(this);
            borders.Add(Data);
        }

        public void SetNode(Variable node)
        {
            this.node = node;
            if (node != null)
            {
                this.value.Text = node.Value;
                this.name.Text = node.Name;
                this.IsParam.IsChecked = node.isFunctionParam;
                this.type.SelectedItem = this.type.Items.GetItemAt((int)node.Type);
            }
            if (this.value.Text != "") v.Visibility = Visibility.Hidden;
            if (this.name.Text != "")  n.Visibility = Visibility.Hidden;

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

        private void DataIn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            main_window.RegisterEndpoint(node.Connection, ConnectionType.Data);
        }

        public List<(Border border, Connectable connectable, ConnectionType conn_type)> GetParamsMap()
        {
            List<(Border border,
                  Connectable connectable,
                  ConnectionType conn_type)> paramsMap = new List<(Border, Connectable, ConnectionType)>();

            if (node == null) return paramsMap;

            paramsMap.Add((Data, node.Connection, ConnectionType.Data));

            return paramsMap;
        }

        public void setLine(LineEndpoint line, FlowParameter f)
        {
            if (node.Connection.ID == f.ID)
            {
                active = borders[0];
                LineOwners.Add(active); Connections.Add(line);
                active.Background = line.Line.Stroke;
            }
        }
        public void setLine(LineEndpoint line, DataParameter f)
        {
            if (node.Connection.ID == f.ID)
            {
                active = borders[0];
                LineOwners.Add(active); Connections.Add(line);
                active.Background = line.Line.Stroke;
            }
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
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var a = sender as System.Windows.Controls.TextBox;
            if(a.Name == "value") v.Visibility = Visibility.Hidden;
            if(a.Name == "name") n.Visibility = Visibility.Hidden;
        }

        private void IsParam_Checked(object sender, RoutedEventArgs e)
        {
            asd.Width = 300;
            v.Width = 280;
            value.Width = 280;
            node.isFunctionParam = true;
        }

        private void IsParam_Unchecked(object sender, RoutedEventArgs e)
        {
            asd.Width = 180;
            v.Width = 160;
            value.Width = 160;
            node.isFunctionParam = false;
        }
        
        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var a = sender as System.Windows.Controls.TextBox;
            if (a.Name == "value")
            {
                if (a.Text == "")
                    v.Visibility = Visibility.Visible;
                node.Value = a.Text;
            }
            else if (a.Name == "name")
            {
                if (a.Text == "")
                    n.Visibility = Visibility.Visible;
                node.Name = a.Text;
            }
        }

        private void Selected_LostFocus(object sender, RoutedEventArgs e)
        {
            if (node != null)
            {
                node.Type = GetVariableType();
            }
        }

        CDataTypes GetVariableType() {
            return (CDataTypes)Enum.Parse(typeof(CDataTypes), ((ComboBoxItem)(type.SelectedItem)).Tag.ToString());
        }
    }
}
