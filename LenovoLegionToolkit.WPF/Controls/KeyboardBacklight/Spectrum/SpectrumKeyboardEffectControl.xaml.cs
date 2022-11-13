using System;
using System.Windows;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Extensions;

namespace LenovoLegionToolkit.WPF.Controls.KeyboardBacklight.Spectrum
{
    public partial class SpectrumKeyboardEffectControl
    {
        public SpectrumKeyboardBacklightEffect Effect { get; }

        public event EventHandler? Delete;

        public SpectrumKeyboardEffectControl(SpectrumKeyboardBacklightEffect effect)
        {
            Effect = effect;

            InitializeComponent();

            _cardHeaderControl.Title = effect.Type.GetDisplayName();
        }

        private void Delete_Click(object sender, RoutedEventArgs e) => Delete?.Invoke(this, EventArgs.Empty);
    }
}
