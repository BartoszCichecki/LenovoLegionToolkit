using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.WPF.Resources;
using Wpf.Ui.Common;

namespace LenovoLegionToolkit.WPF.Controls.Dashboard;

public class InstantBootControl : AbstractComboBoxFeatureCardControl<InstantBootState>
{
    public InstantBootControl()
    {
        Icon = SymbolRegular.PlugDisconnected24;
        Title = Resource.InstantBootControl_Title;
        Subtitle = Resource.InstantBootControl_Message;
    }
}
