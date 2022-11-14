using System;
using System.Windows;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Extensions;

namespace LenovoLegionToolkit.WPF.Controls.KeyboardBacklight.Spectrum
{
    public partial class SpectrumKeyboardEffectControl
    {
        public new SpectrumKeyboardBacklightEffect Effect { get; }

        public event EventHandler? Edit;
        public event EventHandler? Delete;

        public SpectrumKeyboardEffectControl(SpectrumKeyboardBacklightEffect effect)
        {
            Effect = effect;

            InitializeComponent();

            _cardHeaderControl.Title = effect.Type.GetDisplayName();

            var subtitle = string.Empty;
            if (effect.Keys.All)
                subtitle += "All keys";
            else
                subtitle += $"{effect.Keys.KeyCodes.Length} zones";
            _cardHeaderControl.Subtitle = subtitle;
        }

        private void Edit_Click(object sender, RoutedEventArgs e) => Edit?.Invoke(this, EventArgs.Empty);

        private void Delete_Click(object sender, RoutedEventArgs e) => Delete?.Invoke(this, EventArgs.Empty);
    }
}
