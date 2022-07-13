using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Threading;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;
using LenovoLegionToolkit.WPF.Controls.Packages;
using LenovoLegionToolkit.WPF.Utils;

namespace LenovoLegionToolkit.WPF.Pages
{
    public partial class PackagesPage : Page, IProgress<float>
    {
        private readonly PackageDownloader _packageDownloader = IoCContainer.Resolve<PackageDownloader>();

        private CancellationTokenSource? _getPackagesTokenSource;

        private List<Package>? _packages;

        public PackagesPage()
        {
            Initialized += PackagesPage_Initialized;
            InitializeComponent();
        }

        private async void PackagesPage_Initialized(object? sender, EventArgs e)
        {
            var mi = await Compatibility.GetMachineInformation();
            var os = Environment.OSVersion;

            _machineTypeTextBox.Text = mi.MachineType;
            _osComboBox.SelectedIndex = os.Version >= new Version(10, 0, 22000, 0) ? 0 : 1;
            _downloadToText.PlaceholderText = _downloadToText.Text = KnownFolders.GetPath(KnownFolder.Downloads);

            _downloadPackagesButton.IsEnabled = true;
            _cancelDownloadPackagesButton.IsEnabled = true;
        }

        public void Report(float value) => Dispatcher.Invoke(() =>
        {
            _loader.IsIndeterminate = !(value > 0);
            _loader.Progress = value;
        });

        private void OpenDownloadToButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start("explorer.exe", GetDownloadLocation());
            }
            catch { }
        }

        private void DownloadToButton_Click(object sender, RoutedEventArgs e)
        {
            using var ofd = new FolderBrowserDialog
            {
                InitialDirectory = _downloadToText.Text,
            };
            if (ofd.ShowDialog() != DialogResult.OK)
                return;

            _downloadToText.Text = ofd.SelectedPath;
        }

        private async void DownloadPackagesButton_Click(object sender, RoutedEventArgs e)
        {
            var errored = false;
            try
            {
                _downloadPackagesButton.Visibility = Visibility.Collapsed;
                _cancelDownloadPackagesButton.Visibility = Visibility.Visible;
                _loader.Visibility = Visibility.Visible;
                _loader.IsLoading = true;
                _packages = null;

                _packagesStackPanel.Children.Clear();
                _scrollViewer.ScrollToHome();

                var machineType = _machineTypeTextBox.Text.Trim();
                var os = _osComboBox.SelectedIndex switch
                {
                    0 => "win11",
                    1 => "win10",
                    _ => null,
                };

                if (string.IsNullOrWhiteSpace(machineType) || machineType.Length != 4 || string.IsNullOrWhiteSpace(os))
                {
                    await SnackbarHelper.ShowAsync("Something went wrong", "Check if Machine Type and OS are set correctly.");
                    return;
                }

                _getPackagesTokenSource?.Cancel();
                _getPackagesTokenSource = new CancellationTokenSource();

                var token = _getPackagesTokenSource.Token;

                var packages = await _packageDownloader.GetPackagesAsync(machineType, os, this, token);

                _packages = packages;

                packages = Sort(packages);

                foreach (var package in packages)
                {
                    var control = new PackageControl(_packageDownloader, package, GetDownloadLocation);
                    _packagesStackPanel.Children.Add(control);
                }
            }
            catch (TaskCanceledException)
            {
                errored = true;
            }
            catch (HttpRequestException ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Error occured when downloading packages: {ex.Demystify()}");

                SnackbarHelper.Show("Something went wrong", "Check if your internet connection is up and running.", true);

                errored = true;
            }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Error occured when downloading packages: {ex.Demystify()}");

                SnackbarHelper.Show("Something went wrong", ex.Message, true);

                errored = true;
            }
            finally
            {
                _downloadPackagesButton.Visibility = Visibility.Visible;
                _cancelDownloadPackagesButton.Visibility = Visibility.Collapsed;
                _loader.IsLoading = false;
                _loader.Progress = 0;
                _loader.IsIndeterminate = true;

                if (errored)
                {
                    _packagesStackPanel.Children.Clear();
                    _loader.Visibility = Visibility.Collapsed;
                    _loader.IsLoading = true;
                }
            }
        }

        private void CancelDownloadPackagesButton_Click(object sender, RoutedEventArgs e) => _getPackagesTokenSource?.Cancel();

        private void SortingComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_packages is null)
                return;

            _packagesStackPanel.Children.Clear();
            _scrollViewer.ScrollToHome();

            var packages = Sort(_packages);

            foreach (var package in packages)
            {
                var control = new PackageControl(_packageDownloader, package, GetDownloadLocation);
                _packagesStackPanel.Children.Add(control);
            }
        }

        private string GetDownloadLocation()
        {
            var location = _downloadToText.Text.Trim();
            if (!Directory.Exists(location))
                return KnownFolders.GetPath(KnownFolder.Downloads);
            return location;
        }

        private List<Package> Sort(List<Package> packages)
        {
            return _sortingComboBox.SelectedIndex switch
            {
                0 => packages.OrderBy(p => p.Description).ToList(),
                1 => packages.OrderBy(p => p.Category).ToList(),
                2 => packages.OrderByDescending(p => p.ReleaseDate).ToList(),
                _ => packages,
            };
        }
    }
}
