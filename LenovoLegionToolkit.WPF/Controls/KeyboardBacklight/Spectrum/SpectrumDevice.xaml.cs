using System;
using System.Linq;
using System.Windows;
using LenovoLegionToolkit.WPF.Extensions;

namespace LenovoLegionToolkit.WPF.Controls.KeyboardBacklight.Spectrum
{
    public partial class SpectrumDevice
    {
        public SpectrumKeyboardButton[] Buttons { get; private set; } = Array.Empty<SpectrumKeyboardButton>();

        public SpectrumDevice()
        {
            InitializeComponent();

            Loaded += SpectrumDevice_Loaded;
        }

        private void SpectrumDevice_Loaded(object sender, RoutedEventArgs args)
        {
            Buttons = this.GetChildrenOfType<SpectrumKeyboardButton>()
                .Where(b => b.KeyCode > 0)
                .Where(b => b.Visibility == Visibility.Visible)
                .ToArray();
        }
    }
}
