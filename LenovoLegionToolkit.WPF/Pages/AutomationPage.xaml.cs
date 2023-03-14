using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Automation;
using LenovoLegionToolkit.Lib.Automation.Pipeline;
using LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers;
using LenovoLegionToolkit.Lib.Automation.Steps;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.WPF.Controls.Automation;
using LenovoLegionToolkit.WPF.Extensions;
using LenovoLegionToolkit.WPF.Resources;
using LenovoLegionToolkit.WPF.Utils;
using LenovoLegionToolkit.WPF.Windows.Automation;
using LenovoLegionToolkit.WPF.Windows.Utils;
using Wpf.Ui.Common;
using MenuItem = Wpf.Ui.Controls.MenuItem;

namespace LenovoLegionToolkit.WPF.Pages;

public partial class AutomationPage
{
    private readonly AutomationProcessor _automationProcessor = IoCContainer.Resolve<AutomationProcessor>();

    private IAutomationStep[] _supportedAutomationSteps = Array.Empty<IAutomationStep>();

    public AutomationPage()
    {
        Initialized += AutomationPage_Initialized;

        InitializeComponent();
    }

    private async void AutomationPage_Initialized(object? sender, EventArgs e)
    {
        await RefreshAsync();
    }

    private async void EnableAutomaticPipelinesToggle_Click(object sender, RoutedEventArgs e)
    {
        var isChecked = _enableAutomaticPipelinesToggle.IsChecked;
        if (isChecked.HasValue)
            await _automationProcessor.SetEnabledAsync(isChecked.Value);
    }

    private void NewAutomaticPipelineButton_Click(object sender, RoutedEventArgs e)
    {
        var existingTriggersTypes = _automaticPipelinesStackPanel.Children.ToArray()
            .OfType<AutomationPipelineControl>()
            .Select(c => c.AutomationPipeline.Trigger)
            .Where(t => t is not null)
            .Select(t => t!.GetType())
            .ToHashSet();

        var window = new CreateAutomationPipelineWindow(existingTriggersTypes, AddAutomaticPipeline) { Owner = Window.GetWindow(this) };
        window.ShowDialog();
    }

    private async void NewManualPipelineButton_Click(object sender, RoutedEventArgs e)
    {
        await AddManualPipelineAsync();
    }

    private async void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            _saveButton.IsEnabled = false;
            _saveButton.Content = Resource.Saving;

            var automaticPipelines = _automaticPipelinesStackPanel.Children.ToArray()
                .OfType<AutomationPipelineControl>()
                .Select(c => c.CreateAutomationPipeline())
                .ToList();

            var manualPipelines = _manualPipelinesStackPanel.Children.ToArray()
                .OfType<AutomationPipelineControl>()
                .Select(c => c.CreateAutomationPipeline())
                .ToList();

            var pipelines = new List<AutomationPipeline>();
            pipelines.AddRange(automaticPipelines);
            pipelines.AddRange(manualPipelines);

            var ap = new AutomationPipeline(new AndAutomationPipelineTrigger(new IAutomationPipelineTrigger[]
            {
                new ACAdapterDisconnectedAutomationPipelineTrigger(),
                new PowerModeAutomationPipelineTrigger(PowerModeState.Quiet),
            }));
            ap.Steps.Add(new SpectrumKeyboardBacklightBrightnessAutomationStep(1));
            // pipelines.Add(ap);

            await _automationProcessor.ReloadPipelinesAsync(pipelines);
            await RefreshAsync();

            await SnackbarHelper.ShowAsync(Resource.AutomationPage_Saved_Title, Resource.AutomationPage_Saved_Message);
        }
        finally
        {
            _saveButton.Content = Resource.Save;
            _saveButton.IsEnabled = true;
        }
    }

    private async void RevertButton_Click(object sender, RoutedEventArgs e)
    {
        await RefreshAsync();

        await SnackbarHelper.ShowAsync(Resource.AutomationPage_Reverted_Title, Resource.AutomationPage_Reverted_Message);
    }

    private async Task RefreshAsync()
    {
        _scrollViewer.ScrollToTop();

        _loaderAutomatic.IsLoading = true;
        _loaderManual.IsLoading = true;

        var initializedTasks = new List<Task> { Task.Delay(TimeSpan.FromMilliseconds(500)) };

        _enableAutomaticPipelinesToggle.IsChecked = _automationProcessor.IsEnabled;

        _automaticPipelinesStackPanel.Children.Clear();
        _manualPipelinesStackPanel.Children.Clear();

        var pipelines = await _automationProcessor.GetPipelinesAsync();

        if (_supportedAutomationSteps.IsEmpty())
            _supportedAutomationSteps = await GetSupportedAutomationStepsAsync();

        foreach (var pipeline in pipelines.Where(p => p.Trigger is not null))
        {
            var control = GenerateControl(pipeline, _automaticPipelinesStackPanel);
            _automaticPipelinesStackPanel.Children.Add(control);
            initializedTasks.Add(control.InitializedTask);
        }

        foreach (var pipeline in pipelines.Where(p => p.Trigger is null))
        {
            var control = GenerateControl(pipeline, _manualPipelinesStackPanel);
            _manualPipelinesStackPanel.Children.Add(control);
            initializedTasks.Add(control.InitializedTask);
        }

        _saveRevertStackPanel.Visibility = Visibility.Collapsed;

        _noAutomaticActionsText.Visibility = _automaticPipelinesStackPanel.Children.Count < 1
            ? Visibility.Visible
            : Visibility.Collapsed;
        _noManualActionsText.Visibility = _manualPipelinesStackPanel.Children.Count < 1
            ? Visibility.Visible
            : Visibility.Collapsed;

        await Task.WhenAll(initializedTasks);

        _loaderAutomatic.IsLoading = false;
        _loaderManual.IsLoading = false;
    }

    private async Task<IAutomationStep[]> GetSupportedAutomationStepsAsync()
    {
        var steps = new IAutomationStep[]
        {
            new AlwaysOnUsbAutomationStep(default),
            new BatteryAutomationStep(default),
            new DeactivateGPUAutomationStep(default),
            new DelayAutomationStep(default),
            new DisplayBrightnessAutomationStep(50),
            new DpiScaleAutomationStep(default),
            new FlipToStartAutomationStep(default),
            new FnLockAutomationStep(default),
            new GodModePresetAutomationStep(default),
            new HDRAutomationStep(default),
            new MicrophoneAutomationStep(default),
            new OneLevelWhiteKeyboardBacklightAutomationStep(default),
            new OverDriveAutomationStep(default),
            new PowerModeAutomationStep(default),
            new RefreshRateAutomationStep(default),
            new ResolutionAutomationStep(default),
            new RGBKeyboardBacklightAutomationStep(default),
            new RunAutomationStep(default, default),
            new SpectrumKeyboardBacklightBrightnessAutomationStep(0),
            new SpectrumKeyboardBacklightProfileAutomationStep(1),
            new TouchpadLockAutomationStep(default),
            new TurnOffMonitorsAutomationStep(),
            new WhiteKeyboardBacklightAutomationStep(default),
            new WinKeyAutomationStep(default),
        };

        var supportedSteps = new List<IAutomationStep>();

        foreach (var step in steps)
            if (await step.IsSupportedAsync())
                supportedSteps.Add(step);

        return supportedSteps.ToArray();
    }

    private AutomationPipelineControl GenerateControl(AutomationPipeline pipeline, StackPanel stackPanel)
    {
        var control = new AutomationPipelineControl(pipeline, _supportedAutomationSteps);
        control.MouseRightButtonUp += (_, e) =>
        {
            ShowPipelineContextMenu(control, stackPanel);
            e.Handled = true;
        };
        control.OnChanged += (_, _) => PipelinesChanged();
        control.OnDelete += (s, _) =>
        {
            if (s is AutomationPipelineControl c)
                DeletePipeline(c, stackPanel);
        };
        return control;
    }

    private void PipelinesChanged()
    {
        _saveRevertStackPanel.Visibility = Visibility.Visible;
    }

    private void ShowPipelineContextMenu(AutomationPipelineControl control, StackPanel stackPanel)
    {
        var menuItems = new List<MenuItem>();

        var index = stackPanel.Children.IndexOf(control);
        var maxIndex = stackPanel.Children.Count - 1;

        var moveUpMenuItem = new MenuItem { SymbolIcon = SymbolRegular.ArrowUp24, Header = Resource.MoveUp };
        if (index > 0)
            moveUpMenuItem.Click += (_, _) => MovePipeline(control, stackPanel, index - 1);
        else
            moveUpMenuItem.IsEnabled = false;
        menuItems.Add(moveUpMenuItem);

        var moveDownMenuItem = new MenuItem { SymbolIcon = SymbolRegular.ArrowDown24, Header = Resource.MoveDown };
        if (index < maxIndex)
            moveDownMenuItem.Click += (_, _) => MovePipeline(control, stackPanel, index + 1);
        else
            moveDownMenuItem.IsEnabled = false;
        menuItems.Add(moveDownMenuItem);

        var renameMenuItem = new MenuItem { SymbolIcon = SymbolRegular.Edit24, Header = Resource.Rename };
        renameMenuItem.Click += async (_, _) => await RenamePipelineAsync(control);
        menuItems.Add(renameMenuItem);

        if (control.AutomationPipeline.Trigger is null)
        {
            var changeIconMenuItem = new MenuItem { SymbolIcon = SymbolRegular.Edit24, Header = Resource.AutomationPage_ChangeIcon };
            changeIconMenuItem.Click += async (_, _) => await ChangePipelineIconAsync(control);
            menuItems.Add(changeIconMenuItem);
        }

        var deleteMenuItem = new MenuItem { SymbolIcon = SymbolRegular.Delete24, Header = Resource.Delete };
        deleteMenuItem.Click += (_, _) => DeletePipeline(control, stackPanel);
        menuItems.Add(deleteMenuItem);

        control.ContextMenu = new();
        control.ContextMenu.Items.AddRange(menuItems);
        control.ContextMenu.IsOpen = true;
    }

    private void MovePipeline(AutomationPipelineControl control, StackPanel stackPanel, int index)
    {
        stackPanel.Children.Remove(control);
        stackPanel.Children.Insert(index, control);

        PipelinesChanged();
    }

    private void AddAutomaticPipeline(IAutomationPipelineTrigger trigger)
    {
        var pipeline = new AutomationPipeline(trigger);
        var control = GenerateControl(pipeline, _automaticPipelinesStackPanel);
        _automaticPipelinesStackPanel.Children.Insert(0, control);

        _noAutomaticActionsText.Visibility = _automaticPipelinesStackPanel.Children.Count < 1
            ? Visibility.Visible
            : Visibility.Collapsed;

        PipelinesChanged();
    }

    private async Task AddManualPipelineAsync()
    {
        var newName = await MessageBoxHelper.ShowInputAsync(this,
            Resource.AutomationPage_AddManualPipeline_Title,
            Resource.AutomationPage_AddManualPipeline_Placeholder);
        if (string.IsNullOrWhiteSpace(newName))
            return;

        var pipeline = new AutomationPipeline(newName);
        var control = GenerateControl(pipeline, _manualPipelinesStackPanel);
        _manualPipelinesStackPanel.Children.Insert(0, control);

        _noManualActionsText.Visibility = _manualPipelinesStackPanel.Children.Count < 1
            ? Visibility.Visible
            : Visibility.Collapsed;

        PipelinesChanged();
    }

    private async Task RenamePipelineAsync(AutomationPipelineControl control)
    {
        var name = control.GetName();
        var newName = await MessageBoxHelper.ShowInputAsync(this,
            Resource.AutomationPage_RenamePipeline_Title,
            Resource.AutomationPage_RenamePipeline_Placeholder,
            name,
            allowEmpty: true);

        if (string.IsNullOrEmpty(newName))
            return;

        control.SetName(newName);
    }

    private async Task ChangePipelineIconAsync(AutomationPipelineControl control)
    {
        try
        {
            var window = new SymbolRegularPicker { Owner = Window.GetWindow(this) };
            window.ShowDialog();

            var icon = await window.SymbolRegularTask;
            control.SetIcon(icon);
        }
        catch (TaskCanceledException) { }
    }

    private void DeletePipeline(AutomationPipelineControl control, StackPanel stackPanel)
    {
        stackPanel.Children.Remove(control);

        _noAutomaticActionsText.Visibility = _automaticPipelinesStackPanel.Children.Count < 1
            ? Visibility.Visible
            : Visibility.Collapsed;
        _noManualActionsText.Visibility = _manualPipelinesStackPanel.Children.Count < 1
            ? Visibility.Visible
            : Visibility.Collapsed;

        PipelinesChanged();
    }
}