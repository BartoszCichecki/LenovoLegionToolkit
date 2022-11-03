using System.Linq;
using System.Windows;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.WPF.Extensions;

namespace LenovoLegionToolkit.WPF.Controls.KeyboardBacklight.Spectrum
{
    public partial class SpectrumDevice
    {
        public SpectrumDevice()
        {
            InitializeComponent();
        }

        public void SetLayout(KeyboardLayout layout) => _keyboard.SetLayout(layout);

        public SpectrumKeyboardButton[] GetButtons()
        {
            return this.GetVisibleChildrenOfType<SpectrumKeyboardButton>()
                    .Where(b => b.KeyCode > 0)
                    .Where(b => b.Visibility == Visibility.Visible)
                    .ToArray();
        }
    }
}
