using System.Collections.Generic;
using System.Linq;

namespace System.Windows.Controls
{
    public static class ComboBoxExtensions
    {
        public static void SetItems<T>(this ComboBox comboBox, IEnumerable<T> items,
            T selectedItem, Func<T, string> displayValueConverter = null)
        {
            var boxedItems = items.Select(v => new ComboBoxItem<T>(v, displayValueConverter));
            var selectedBoxedItem = boxedItems.FirstOrDefault(bv => EqualityComparer<T>.Default.Equals(bv.Value, selectedItem));

            comboBox.Items.Clear();
            comboBox.Items.AddRange(boxedItems);
            comboBox.SelectedValue = selectedBoxedItem;
        }

        public static bool TryGetSelectedItem<T>(this ComboBox comboBox, out T value)
        {
            if (comboBox.SelectedItem is ComboBoxItem<T> selectedBoxedItem)
            {
                value = selectedBoxedItem.Value;
                return true;
            }

            value = default;
            return false;
        }

        private class ComboBoxItem<T>
        {
            public static bool operator ==(ComboBoxItem<T> left, ComboBoxItem<T> right) => left.Equals(right);

            public static bool operator !=(ComboBoxItem<T> left, ComboBoxItem<T> right) => !(left == right);

            public T Value { get; }
            private readonly Func<T, string> _displayString;

            public ComboBoxItem(T value, Func<T, string> displayString)
            {
                Value = value;
                _displayString = displayString;
            }

            public override bool Equals(object obj) => obj is ComboBoxItem<T> item && EqualityComparer<T>.Default.Equals(Value, item.Value);

            public override int GetHashCode() => HashCode.Combine(Value);

            public override string ToString() => _displayString?.Invoke(Value) ?? Value.ToString();
        }
    }
}
