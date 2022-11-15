using System;
using System.Windows.Input;
using System.Windows.Media;

namespace LenovoLegionToolkit.WPF.Controls
{
    public partial class MultiColorPickerItemControl
    {
        public Color SelectedColor
        {
            get => _picker.SelectedColor;
            set => _picker.SelectedColor = value;
        }

        public event EventHandler? ColorChangedContinuous
        {
            add => _picker.ColorChangedContinuous += value;
            remove => _picker.ColorChangedContinuous -= value;
        }

        public event EventHandler? ColorChangedDelayed
        {
            add => _picker.ColorChangedDelayed += value;
            remove => _picker.ColorChangedDelayed -= value;
        }

        public event EventHandler<MouseButtonEventArgs>? Delete;

        public MultiColorPickerItemControl() => InitializeComponent();

        private void Delete_Click(object sender, MouseButtonEventArgs e) => Delete?.Invoke(this, e);
    }
}
