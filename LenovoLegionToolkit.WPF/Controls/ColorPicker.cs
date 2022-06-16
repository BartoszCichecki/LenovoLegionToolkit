using System;
using System.Windows;
using UserControl = System.Windows.Controls.UserControl;
using WPFUI.Controls;
using System.Windows.Media;
using LenovoLegionToolkit.WPF.Controls;
using System.Windows.Controls;

namespace LenovoLegionToolkit.WPF.Controls
{
    public class ColorPicker : UserControl
    {

        public event EventHandler OnColorChange;
        ColorTable colorTable = new ColorTable();
        Grid grid = new Grid();
        Slider slider = new Slider();

        public Color color   // property
        {
            get {
                return colorTable.color; 
            }
            set {
                colorTable.color = value;
            } 
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            colorTable.OnColorChange += ColorChangedEvent;
            Grid grid = new Grid();
            slider.TickFrequency = 0.05;
            slider.Maximum = 1;
            slider.Minimum = 0;
            slider.Value = 1;
            colorTable.Value = (slider.Value);
            slider.ToolTip = "Value (HSV ColorSpace)";
            slider.ValueChanged += onSaturationChange;
            grid.Width = 256;
            grid.Height = 300;
            grid.HorizontalAlignment = HorizontalAlignment.Left;
            grid.VerticalAlignment = VerticalAlignment.Top;
            grid.ShowGridLines = false;

            // Define the Columns
            ColumnDefinition colDef1 = new ColumnDefinition();
            grid.ColumnDefinitions.Add(colDef1);

            RowDefinition rowDef1 = new RowDefinition();
            rowDef1.Height = new GridLength( 256);
            RowDefinition rowDef2 = new RowDefinition();
            grid.RowDefinitions.Add(rowDef1);
            grid.RowDefinitions.Add(rowDef2);
            Grid.SetRow(colorTable, 0);
            Grid.SetColumn(colorTable, 0);

            Grid.SetRow(slider, 1);
            Grid.SetColumn(slider, 0);

            grid.Children.Add(colorTable);
            grid.Children.Add(slider);

            Content = grid;


        }

        private void ColorChangedEvent(object? sender, EventArgs e)
        {
            EventHandler handler = OnColorChange;
            handler?.Invoke(this, e);
        }

        private void onSaturationChange(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            colorTable.Value=(slider.Value);
        }
    }
}
