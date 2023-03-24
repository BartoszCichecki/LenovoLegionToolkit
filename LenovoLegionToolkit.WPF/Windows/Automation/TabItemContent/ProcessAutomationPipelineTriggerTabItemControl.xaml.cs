using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Automation.Pipeline.Triggers;
using LenovoLegionToolkit.Lib.Utils;
using LenovoLegionToolkit.WPF.Extensions;
using Microsoft.Win32;
using Wpf.Ui.Common;
using Button = Wpf.Ui.Controls.Button;

namespace LenovoLegionToolkit.WPF.Windows.Automation.TabItemContent;

public partial class ProcessAutomationPipelineTriggerTabItemControl : IAutomationPipelineTriggerTabItemContent<IProcessesAutomationPipelineTrigger>
{
    private readonly IProcessesAutomationPipelineTrigger _trigger;
    private readonly List<ProcessInfo> _processes;

    public ProcessAutomationPipelineTriggerTabItemControl(IProcessesAutomationPipelineTrigger trigger)
    {
        _trigger = trigger;
        _processes = trigger.Processes.ToList();

        InitializeComponent();
    }

    public IProcessesAutomationPipelineTrigger GetTrigger() => _trigger.DeepCopy(_processes.ToArray());

    private void ProcessAutomationPipelineTriggerTabItemControl_Initialized(object? sender, EventArgs e)
    {
        var copyCommand = new RoutedCommand();
        copyCommand.InputGestures.Add(new KeyGesture(Key.C, ModifierKeys.Control));
        CommandBindings.Add(new CommandBinding(copyCommand, CopyShortcut));

        var pasteCommand = new RoutedCommand();
        pasteCommand.InputGestures.Add(new KeyGesture(Key.V, ModifierKeys.Control));
        CommandBindings.Add(new CommandBinding(pasteCommand, PasteShortcut));

        Refresh();
    }

    private void Item_OnDelete(object? sender, EventArgs e)
    {
        if (sender is not ListItem listItem)
            return;

        _processes.Remove(listItem.Process);
        Refresh();
    }

    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
        var ofd = new OpenFileDialog
        {
            InitialDirectory = "::{20D04FE0-3AEA-1069-A2D8-08002B30309D}",
            Filter = "Exe Files (.exe)|*.exe",
            CheckFileExists = true,
        };
        var result = ofd.ShowDialog();

        if (!result.HasValue || !result.Value)
            return;

        var processInfo = ProcessInfo.FromPath(ofd.FileName);
        if (_processes.Contains(processInfo))
            return;

        _processes.Add(processInfo);
        Refresh();
    }

    private void DeleteAllButton_Click(object sender, RoutedEventArgs e)
    {
        _processes.Clear();
        Refresh();
    }

    private void CopyShortcut(object sender, RoutedEventArgs e)
    {
        try
        {
            ClipboardExtensions.SetProcesses(_processes);
        }
        catch (Exception ex)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Couldn't copy to clipboard", ex);
        }
    }

    private void PasteShortcut(object sender, RoutedEventArgs e)
    {
        try
        {
            var processes = ClipboardExtensions.GetProcesses()
                .Where(p => !_processes.Contains(p))
                .ToArray();
            if (!processes.Any())
                return;

            _processes.AddRange(processes);
            Refresh();
        }
        catch (Exception ex)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Couldn't paste from clipboard", ex);
        }
    }

    private void Refresh()
    {
        _list.Items.Clear();
        foreach (var process in _processes.OrderBy(p => p))
        {
            var item = new ListItem(process);
            item.OnDelete += Item_OnDelete;
            _list.Items.Add(item);
        }
    }

    private class ListItem : UserControl
    {
        private readonly Grid _grid = new()
        {
            Margin = new(8, 4, 0, 16),
            ColumnDefinitions =
            {
                new() { Width = GridLength.Auto },
                new() { Width = new(1, GridUnitType.Star) },
                new() { Width = GridLength.Auto },
            },
            RowDefinitions =
            {
                new() { Height = GridLength.Auto },
                new() { Height = GridLength.Auto },
            },
        };

        private readonly Image _icon = new()
        {
            Width = 24,
            Height = 24,
            Margin = new(0, 0, 8, 0),
        };

        private readonly TextBlock _nameTextBox = new()
        {
            TextTrimming = TextTrimming.CharacterEllipsis,
        };

        private readonly TextBlock _pathTextBox = new()
        {
            TextTrimming = TextTrimming.CharacterEllipsis,
            Margin = new(0, 4, 0, 0),
        };

        private readonly Button _deleteButton = new()
        {
            Icon = SymbolRegular.Delete24,
            FontSize = 18,
            Margin = new(8, 0, 0, 0),
        };

        public ProcessInfo Process { get; }

        public event EventHandler? OnDelete;

        public ListItem(ProcessInfo process)
        {
            Process = process;

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            _icon.Source = ImageSourceExtensions.ApplicationIcon(Process.ExecutablePath) ?? ImageSourceExtensions.FromResource("Assets/default_exe.png");
            _nameTextBox.Text = Process.Name;
            _pathTextBox.Text = Process.ExecutablePath;

            ToolTipService.SetToolTip(_pathTextBox, Process.ExecutablePath);

            _pathTextBox.SetResourceReference(ForegroundProperty, "TextFillColorSecondaryBrush");

            _deleteButton.Click += (_, _) => OnDelete?.Invoke(this, EventArgs.Empty);

            Grid.SetColumn(_icon, 0);
            Grid.SetRow(_icon, 0);
            Grid.SetRowSpan(_icon, 2);

            Grid.SetColumn(_nameTextBox, 1);
            Grid.SetRow(_nameTextBox, 0);

            Grid.SetColumn(_pathTextBox, 1);
            Grid.SetRow(_pathTextBox, 1);

            Grid.SetColumn(_deleteButton, 2);
            Grid.SetRow(_deleteButton, 0);
            Grid.SetRowSpan(_deleteButton, 2);

            _grid.Children.Add(_icon);
            _grid.Children.Add(_nameTextBox);
            _grid.Children.Add(_pathTextBox);
            _grid.Children.Add(_deleteButton);

            Content = _grid;
        }
    }
}
