using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ColorPicker;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Listeners;
using LenovoLegionToolkit.Lib.Settings;
using LenovoLegionToolkit.WPF.Utils;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;
using Button = System.Windows.Controls.Button;
using Color = System.Windows.Media.Color;

namespace LenovoLegionToolkit.WPF.Controls
{
    public class ColorCardSystemAwareControl : ColorCardControl
    {
        private readonly SystemThemeListener _listener = IoCContainer.Resolve<SystemThemeListener>();

        private StackPanel? _colorsPanel;

        private readonly CheckBox _syncSystemAccentColorCheckBox = new()
        {
            Margin = new(0, 0, 0, 8),

            Content = new TextBlock
            {
                Text = "Match the system accent color",
                TextWrapping = TextWrapping.Wrap,
            }
        };

        public event EventHandler? OnSyncSystemAccentColorChanged, OnSyncSystemAccentColorChangedByUser;

        public ColorCardSystemAwareControl()
        {
            _listener.Changed += Listener_Changed;
        }

        protected override StackPanel GetCardExpanderPanel()
        {
            _colorsPanel = base.GetCardExpanderPanel();

            var stackPanel = new StackPanel();

            stackPanel.Children.Add(_syncSystemAccentColorCheckBox);
            stackPanel.Children.Add(_colorsPanel);

            _syncSystemAccentColorCheckBox.Checked += SyncSystemAccentColorCheckBox_Changed;
            _syncSystemAccentColorCheckBox.Unchecked += SyncSystemAccentColorCheckBox_Changed;

            return stackPanel;
        }

        private bool _wasSetSyncSystemAccentColorCalled = false;

        private void SyncSystemAccentColorCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            if (GetSyncSystemAccentColor())
                SetColor(SystemTheme.GetAccentColor());

            _colorsPanel!.IsEnabled = !GetSyncSystemAccentColor();

            OnSyncSystemAccentColorChanged?.Invoke(this, EventArgs.Empty);

            if (!_wasSetSyncSystemAccentColorCalled)
                OnSyncSystemAccentColorChangedByUser?.Invoke(this, EventArgs.Empty);
        }

        private void Listener_Changed(object? sender, SystemThemeSettings e) => Dispatcher.Invoke(() =>
        {
            if (GetSyncSystemAccentColor())
                SetColor(e.AccentColor);
        });

        public bool GetSyncSystemAccentColor() => _syncSystemAccentColorCheckBox.IsChecked ?? false;

        public void SetSyncSystemAccentColor(bool state)
        {
            _wasSetSyncSystemAccentColorCalled = true;
            _syncSystemAccentColorCheckBox.IsChecked = state;
            _wasSetSyncSystemAccentColorCalled = false;
        }
    }
}
