using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Humanizer;

namespace LenovoLegionToolkit.WPF.Windows.Utils
{
    public partial class LanguageSelectorWindow
    {
        private readonly TaskCompletionSource<CultureInfo?> _taskCompletionSource = new();

        public Task<CultureInfo?> ShouldContinue => _taskCompletionSource.Task;

        public LanguageSelectorWindow(IEnumerable<CultureInfo> languages, CultureInfo defaultLanguage)
        {
            InitializeComponent();

            _languageComboBox.SetItems(languages.OrderBy(ci => ci.Name, StringComparer.InvariantCultureIgnoreCase),
                defaultLanguage,
                cc => cc.NativeName.Transform(cc, To.TitleCase));
        }

        private void LanguageSelectorWindow_OnClosed(object? sender, EventArgs e) => _taskCompletionSource.TrySetResult(null);

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            _languageComboBox.TryGetSelectedItem(out CultureInfo? cultureInfo);
            _taskCompletionSource.TrySetResult(cultureInfo);
            Close();
        }
    }
}
