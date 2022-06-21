using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;

namespace LenovoLegionToolkit.WPF.Controls.KeyboardBacklight
{
    public abstract class AbstractComboBoxKeyboardBacklightCardControl<T> : UserControl
    {
        private readonly CardControl _cardControl = new();

        private readonly CardHeaderControl _cardHeaderControl = new();

        private readonly ComboBox _comboBox = new();

        public SymbolRegular Icon
        {
            get => _cardControl.Icon;
            set => _cardControl.Icon = value;
        }

        public string Title
        {
            get => _cardHeaderControl.Title;
            set => _cardHeaderControl.Title = value;
        }

        public string Subtitle
        {
            get => _cardHeaderControl.Subtitle;
            set => _cardHeaderControl.Subtitle = value;
        }

        public T? SelectedItem
        {
            get
            {
                if (_comboBox.TryGetSelectedItem(out T? item))
                    return item;
                return default;
            }
        }

        public event EventHandler? OnChanged;

        public AbstractComboBoxKeyboardBacklightCardControl() => InitializeComponent();

        private void InitializeComponent()
        {
            IsEnabledChanged += CardControl_IsEnabledChanged;

            _comboBox.SelectionChanged += ComboBox_SelectionChanged;
            _comboBox.Width = 150;

            _cardHeaderControl.Accessory = _comboBox;
            _cardControl.Header = _cardHeaderControl;
            _cardControl.Margin = new(0, 0, 0, 8);

            Content = _cardControl;
        }

        private void CardControl_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsEnabled)
                return;

            _comboBox.SelectionChanged -= ComboBox_SelectionChanged;
            _comboBox.ClearItems();
            _comboBox.SelectionChanged += ComboBox_SelectionChanged;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) => OnChanged?.Invoke(this, EventArgs.Empty);

        public void SetItems(IEnumerable<T> items, T selectedItem, Func<T, string>? displayValueConverter)
        {
            _comboBox.SelectionChanged -= ComboBox_SelectionChanged;
            _comboBox.SetItems(items, selectedItem, displayValueConverter);
            _comboBox.SelectionChanged += ComboBox_SelectionChanged;
        }
    }
}
