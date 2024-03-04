﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Automation;
using LenovoLegionToolkit.Lib.Automation.Pipeline;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.Settings;
using LenovoLegionToolkit.WPF.Resources;

namespace LenovoLegionToolkit.WPF.Windows.Settings;

public partial class SelectSmartKeyPipelinesWindow
{
    private readonly AutomationProcessor _automationProcessor = IoCContainer.Resolve<AutomationProcessor>();
    private readonly ApplicationSettings _settings = IoCContainer.Resolve<ApplicationSettings>();

    private readonly bool _isDoublePress;

    private Guid? SettingsStoreGuid
    {
        get => _isDoublePress ? _settings.Store.SmartKeyDoublePressActionId : _settings.Store.SmartKeySinglePressActionId;
        set
        {
            if (_isDoublePress)
                _settings.Store.SmartKeyDoublePressActionId = value;
            else
                _settings.Store.SmartKeySinglePressActionId = value;
        }
    }

    private List<Guid> SettingsStoreList => _isDoublePress ? _settings.Store.SmartKeyDoublePressActionList : _settings.Store.SmartKeySinglePressActionList;

    public SelectSmartKeyPipelinesWindow(bool isDoublePress = false)
    {
        _isDoublePress = isDoublePress;

        InitializeComponent();

        Title = _title.Text = isDoublePress
            ? Resource.SettingsPage_SmartKeyDoublePressAction_Title
            : Resource.SettingsPage_SmartKeySinglePressAction_Title;

        IsVisibleChanged += SelectSmartKeyPipelinesWindow_IsVisibleChanged;
    }

    private async void SelectSmartKeyPipelinesWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (IsVisible)
            await RefreshAsync();
    }

    private async Task RefreshAsync()
    {
        _loader.IsLoading = true;

        _showThisAppToggle.IsChecked = SettingsStoreGuid is null;

        var allPipelines = await _automationProcessor.GetPipelinesAsync();
        var pipelines = allPipelines.Where(p => p.Trigger is null).OrderBy(p => p.Name).ToArray();

        _list.Items.Clear();

        if (pipelines.IsEmpty())
            _list.Items.Add(Resource.SelectSmartKeyPipelinesWindow_List_Empty);

        foreach (var pipeline in pipelines)
        {
            var item = new ListItem(pipeline)
            {
                IsChecked = SettingsStoreList.Exists(x => x == pipeline.Id) || pipeline.Id == SettingsStoreGuid
            };
            _list.Items.Add(item);
        }

        EnableListIfPossible();

        _loader.IsLoading = false;
    }

    private void ShowThisAppToggle_Click(object sender, RoutedEventArgs e) => EnableListIfPossible();

    private void EnableListIfPossible() => _list.IsEnabled = !(_showThisAppToggle.IsChecked ?? false);

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        var selectedPipelines = _list.Items.OfType<ListItem>()
            .Where(li => li.IsChecked)
            .Select(li => li.Pipeline.Id)
            .ToArray();

        SettingsStoreList.Clear();

        if (_showThisAppToggle.IsChecked ?? false)
            SettingsStoreGuid = null;
        else
        {
            SettingsStoreList.AddRange(selectedPipelines);
            SettingsStoreGuid = SettingsStoreList.FirstOrDefault();
        }

        _settings.SynchronizeStore();

        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private class ListItem : UserControl
    {
        private readonly Grid _grid = new()
        {
            Margin = new(8, 4, 0, 16),
            ColumnDefinitions =
            {
                new() { Width = new(32, GridUnitType.Pixel) },
                new() { Width = new(1, GridUnitType.Star) },
            },
        };

        private readonly CheckBox _checkBox = new();

        private readonly TextBlock _nameTextBox = new()
        {
            TextAlignment = TextAlignment.Left,
            VerticalAlignment = VerticalAlignment.Center,
        };

        public AutomationPipeline Pipeline { get; }

        public bool IsChecked
        {
            get => _checkBox.IsChecked ?? false;
            set => _checkBox.IsChecked = value;
        }

        public ListItem(AutomationPipeline pipeline)
        {
            Pipeline = pipeline;

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            _nameTextBox.Text = Pipeline.Name;

            Grid.SetColumn(_checkBox, 0);
            Grid.SetColumn(_nameTextBox, 1);

            _grid.Children.Add(_checkBox);
            _grid.Children.Add(_nameTextBox);

            AutomationProperties.SetLabeledBy(_checkBox, _nameTextBox);

            Content = _grid;
        }
    }
}
