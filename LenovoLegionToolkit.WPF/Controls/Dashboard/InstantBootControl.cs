using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.WPF.Extensions;
using LenovoLegionToolkit.WPF.Resources;
using Wpf.Ui.Controls;

namespace LenovoLegionToolkit.WPF.Controls.Dashboard;

public class InstantBootControl : AbstractComboBoxFeatureCardControl<InstantBootState>
{
    public InstantBootControl()
    {
        Icon = SymbolRegular.PlugDisconnected24.GetIcon();
        Title = Resource.InstantBootControl_Title;
        Subtitle = Resource.InstantBootControl_Message;
    }
}
