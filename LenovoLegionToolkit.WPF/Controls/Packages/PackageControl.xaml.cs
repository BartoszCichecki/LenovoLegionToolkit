using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;
using LenovoLegionToolkit.WPF.Utils;
using LenovoLegionToolkit.WPF.Windows.Packages;

namespace LenovoLegionToolkit.WPF.Controls.Packages
{
    public partial class PackageControl : UserControl, IProgress<float>
    {
        private readonly PackageDownloader _packageDownloader;
        private readonly Package _package;

        private CancellationTokenSource? _downloadPackageTokenSource;

        public PackageControl(PackageDownloader packageDownloader, Package package)
        {
            _packageDownloader = packageDownloader;
            _package = package;

            InitializeComponent();

            _dateTextBlock.Text = package.ReleaseDate.ToString("d");
            _descriptionTextBlock.Text = package.Description;
            _categoryTextBlock.Text = package.Category;
            _detailTextBlock.Text = $"Version {package.Version}  |  {package.FileSize / 1024.0 / 1024.0:0.00} MB  |  {package.FileName}";

            _readmeButton.Visibility = string.IsNullOrWhiteSpace(package.Readme) ? Visibility.Collapsed : Visibility.Visible;

            var showWarning = package.ReleaseDate < DateTime.Now.AddYears(-1);
            _warningTextBlock.Visibility = showWarning ? Visibility.Visible : Visibility.Collapsed;
        }

        public void CancelDownloads() => _downloadPackageTokenSource?.Cancel();

        public void Report(float value) => Dispatcher.Invoke(() =>
        {
            _downloadProgressRing.IsIndeterminate = !(value > 0);
            _downloadProgressRing.Progress = value * 100;
            _downloadProgressLabel.Content = $"{value * 100:0}%";
        });

        private void ReadmeButton_Click(object sender, RoutedEventArgs e)
        {
            var updateWindow = new ReadmeWindow(_package.Readme)
            {
                Owner = Window.GetWindow(this),
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ShowInTaskbar = false,
            };
            updateWindow.ShowDialog();
        }

        private async void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            var result = false;

            try
            {
                _idleStackPanel.Visibility = Visibility.Collapsed;
                _downloadingStackPanel.Visibility = Visibility.Visible;

                _downloadPackageTokenSource?.Cancel();
                _downloadPackageTokenSource = new CancellationTokenSource();

                var downloadsFolder = KnownFolders.GetPath(KnownFolder.Downloads);
                var token = _downloadPackageTokenSource.Token;

                await _packageDownloader.DownloadPackageFileAsync(_package, downloadsFolder, this, token);

                result = true;
            }
            catch (TaskCanceledException) { }
            finally
            {
                _idleStackPanel.Visibility = Visibility.Visible;
                _downloadingStackPanel.Visibility = Visibility.Collapsed;
                _downloadProgressRing.Progress = 0;
                _downloadProgressLabel.Content = null;
            }

            if (result)
                await SnackbarHelper.ShowAsync("Download complete", $"{_package.FileName} downloaded!");
        }

        private void CancelDownloadButton_Click(object sender, RoutedEventArgs e) => _downloadPackageTokenSource?.Cancel();
    }
}
