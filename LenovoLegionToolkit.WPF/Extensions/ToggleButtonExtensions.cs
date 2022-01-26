namespace System.Windows.Controls.Primitives
{
    internal static class ToggleButtonExtensions
    {
        public static void OnOffContent(this ToggleButton toggleButton)
        {
            Update(toggleButton);

            toggleButton.Checked += ToggleButton_CheckedUnchecked;
            toggleButton.Unchecked += ToggleButton_CheckedUnchecked;
        }

        private static void ToggleButton_CheckedUnchecked(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton toggleButton)
                Update(toggleButton);
        }

        private static void Update(ToggleButton toggleButton)
        {
            if (toggleButton.IsChecked == true)
                toggleButton.Content = "On";
            else
                toggleButton.Content = "Off";

            toggleButton.Width = 82;
        }
    }
}
