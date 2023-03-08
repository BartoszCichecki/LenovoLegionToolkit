using System.Linq;
using System.Windows.Controls;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Controllers;
using LenovoLegionToolkit.WPF.Extensions;

namespace LenovoLegionToolkit.WPF.Controls.Settings;

public partial class CPUBoostModeControl
{
    private readonly CPUBoostModeSettings setting;

    private readonly CPUBoostModeController _cpuBoostController = IoCContainer.Resolve<CPUBoostModeController>();

    public CPUBoostModeControl(CPUBoostModeSettings setting)
    {
        this.setting = setting;

        InitializeComponent();

        _headerControl.Title = setting.PowerPlan.Name;

        var selectedItem = setting.CPUBoostModes.First(cbm => cbm.Value == setting.ACSettingValue);
        _comboBoxAC.SetItems(setting.CPUBoostModes, selectedItem, s => s.Name);

        selectedItem = setting.CPUBoostModes.First(cbm => cbm.Value == setting.DCSettingValue);
        _comboBoxDC.SetItems(setting.CPUBoostModes, selectedItem, s => s.Name);
    }

    private async void ComboBoxAC_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!_comboBoxAC.TryGetSelectedItem(out CPUBoostMode cpuBoostMode))
            return;

        await _cpuBoostController.SetSettingAsync(setting.PowerPlan, cpuBoostMode, true);
    }

    private async void ComboBoxDC_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!_comboBoxDC.TryGetSelectedItem(out CPUBoostMode cpuBoostMode))
            return;

        await _cpuBoostController.SetSettingAsync(setting.PowerPlan, cpuBoostMode, false);
    }
}