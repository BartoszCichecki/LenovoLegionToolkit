using System.Collections.Generic;
using System.Linq;
using System.Windows;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.WPF.Extensions;

namespace LenovoLegionToolkit.WPF.Controls.KeyboardBacklight.Spectrum
{
    public partial class SpectrumDeviceControl
    {
        public SpectrumDeviceControl()
        {
            InitializeComponent();
        }

        public void SetLayout(KeyboardLayout layout, bool isExtended)
        {
            _keyboard.SetLayout(layout);

            foreach (var button in GetButtons().Where(b => b.IsExtended))
                button.Visibility = isExtended ? Visibility.Visible : Visibility.Hidden;
        }

        public IEnumerable<SpectrumKeyboardButtonControl> GetVisibleButtons() =>
            GetButtons().Where(b => b.Visibility == Visibility.Visible);

        private IEnumerable<SpectrumKeyboardButtonControl> GetButtons() =>
            this.GetVisibleChildrenOfType<SpectrumKeyboardButtonControl>().Where(b => b.KeyCode > 0);
    }
}
