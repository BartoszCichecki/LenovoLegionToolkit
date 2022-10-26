using System;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LenovoLegionToolkit.WPF.Controls.KeyboardBacklight.Spectrum
{
    public partial class SpectrumKeyboardBacklightControl
    {
        protected override bool DisablesWhileRefreshing => false;

        public SpectrumKeyboardBacklightControl()
        {
            InitializeComponent();

            SizeChanged += SpectrumKeyboardBacklightControl_SizeChanged;
        }

        private void SpectrumKeyboardBacklightControl_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            if (_device.LayoutTransform is not ScaleTransform scaleTransform)
                return;

            var target = (0.75 * ActualWidth) / _device.ActualWidth;
            var scale = Math.Clamp(target, 0.5, 1.5);

            scaleTransform.ScaleX = scale;
            scaleTransform.ScaleY = scale;
        }

        protected override Task OnRefreshAsync() => Task.CompletedTask;

        protected override void OnFinishedLoading()
        {
        }
    }
}
