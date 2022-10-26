using System.Threading.Tasks;

namespace LenovoLegionToolkit.WPF.Controls.KeyboardBacklight.Spectrum
{
    public partial class SpectrumKeyboardBacklightControl
    {
        protected override bool DisablesWhileRefreshing => false;

        public SpectrumKeyboardBacklightControl()
        {
            InitializeComponent();
        }

        protected override Task OnRefreshAsync() => Task.CompletedTask;

        protected override void OnFinishedLoading()
        {
        }
    }
}
