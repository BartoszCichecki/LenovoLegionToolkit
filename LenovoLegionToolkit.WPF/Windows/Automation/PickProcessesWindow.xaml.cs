using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.WPF.Extensions;
using Microsoft.Win32;
using Wpf.Ui.Common;
using Button = Wpf.Ui.Controls.Button;
using Image = System.Windows.Controls.Image;

namespace LenovoLegionToolkit.WPF.Windows.Automation
{
    public partial class PickProcessesWindow
    {
        public List<ProcessInfo> Processes { get; }

        public event EventHandler<ProcessInfo[]>? OnSave;

        public PickProcessesWindow(ProcessInfo[] processes)
        {
            Processes = processes.ToList();

            InitializeComponent();

            ResizeMode = ResizeMode.CanMinimize;

            _titleBar.UseSnapLayout = false;
            _titleBar.CanMaximize = false;

            Loaded += PickProcessesWindow_Loaded;
            IsVisibleChanged += PickProcessesWindow_IsVisibleChanged;
        }

        private void PickProcessesWindow_Loaded(object sender, RoutedEventArgs e) => Refresh();

        private void PickProcessesWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsLoaded && IsVisible)
                Refresh();
        }

        private void Item_OnDelete(object? sender, EventArgs e)
        {
            if (sender is not ListItem listItem)
                return;

            Processes.Remove(listItem.Process);

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

            Processes.Add(ProcessInfo.FromPath(ofd.FileName));

            Refresh();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            OnSave?.Invoke(this, Processes.ToArray());
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public void Refresh()
        {
            _loader.IsLoading = true;

            var processes = Processes;

            _list.Items.Clear();
            foreach (var process in processes.OrderBy(p => p))
            {
                var item = new ListItem(process);
                item.OnDelete += Item_OnDelete;
                _list.Items.Add(item);
            }

            _loader.IsLoading = false;
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

                _pathTextBox.SetResourceReference(ForegroundProperty, "TextFillColorTertiaryBrush");

                _deleteButton.Click += (s, e) => OnDelete?.Invoke(this, EventArgs.Empty);

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
}
