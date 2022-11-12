using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace LenovoLegionToolkit.WPF.Controls
{
    public partial class MultiColorPickerControl
    {
        private const int MAX_ITEMS = 3;

        public Color[] SelectedColors
        {
            get => _buttons.Children.OfType<MultiColorPickerItemControl>()
                    .Select(c => c.SelectedColor)
                    .ToArray();
            set
            {
                _buttons.Children.Clear();
                foreach (var color in value.Take(MAX_ITEMS))
                {
                    var picker = CreateColorPicker();
                    picker.SelectedColor = color;
                    _buttons.Children.Add(picker);
                }

                Update();
            }
        }

        public event EventHandler? ColorsChangedContinuous;
        public event EventHandler? ColorsChangedDelayed;

        public MultiColorPickerControl() => InitializeComponent();

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedColors.Length >= MAX_ITEMS)
                return;

            var picker = CreateColorPicker();
            _buttons.Children.Add(picker);
            Update();
            picker.Open();
        }

        private MultiColorPickerItemControl CreateColorPicker()
        {
            var picker = new MultiColorPickerItemControl
            {
                Margin = new(0, 0, 8, 0),
            };
            picker.ColorChangedContinuous += (s, e) => ColorsChangedContinuous?.Invoke(this, EventArgs.Empty);
            picker.ColorChangedDelayed += (s, e) => ColorsChangedDelayed?.Invoke(this, EventArgs.Empty);
            picker.Delete += (s, e) =>
            {
                _buttons.Children.Remove(picker);
                Update();
                e.Handled = true;
            };
            return picker;
        }

        private void Update()
        {
            _addButton.IsEnabled = SelectedColors.Length < MAX_ITEMS;

            ColorsChangedContinuous?.Invoke(this, EventArgs.Empty);
            ColorsChangedDelayed?.Invoke(this, EventArgs.Empty);
        }
    }
}
