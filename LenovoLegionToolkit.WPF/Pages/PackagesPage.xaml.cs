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
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Extensions;
using LenovoLegionToolkit.Lib.PackageDownloader;
using LenovoLegionToolkit.Lib.Settings;
using LenovoLegionToolkit.Lib.System;
using LenovoLegionToolkit.Lib.Utils;
using LenovoLegionToolkit.WPF.Controls.Packages;
using LenovoLegionToolkit.WPF.Resources;
using LenovoLegionToolkit.WPF.Utils;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;
using MenuItem = Wpf.Ui.Controls.MenuItem;

namespace LenovoLegionToolkit.WPF.Pages
{
    public partial class PackagesPage : Page, IProgress<float>
    {
        private readonly PackageDownloaderSettings _packageDownloaderSettings = IoCContainer.Resolve<PackageDownloaderSettings>();
        private readonly PackageDownloaderFactory _packageDownloaderFactory = IoCContainer.Resolve<PackageDownloaderFactory>();

        private IPackageDownloader? _packageDownloader;

        private CancellationTokenSource? _getPackagesTokenSource;

        private CancellationTokenSource? _filterDebounceCancellationTokenSource;

        private List<Package>? _packages;

        public PackagesPage()
        {
            Initialized += PackagesPage_Initialized;

            InitializeComponent();
        }

        private async void PackagesPage_Initialized(object? sender, EventArgs e)
        {
            _machineTypeTextBox.Text = (await Compatibility.GetMachineInformationAsync()).MachineType;
            _osComboBox.SetItems(Enum.GetValues<OS>(), OSExtensions.GetCurrent(), os => os.GetDisplayName());
            _downloadToText.PlaceholderText = _downloadToText.Text = KnownFolders.GetPath(KnownFolder.Downloads);

            _downloadPackagesButton.IsEnabled = true;
            _cancelDownloadPackagesButton.IsEnabled = true;

            _sourcePrimaryRadio.Tag = PackageDownloaderFactory.Type.PCSupport;
            _sourceSecondaryRadio.Tag = PackageDownloaderFactory.Type.Commercial;
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
            if (!await ShouldInterruptDownloadsIfRunning())
                return;

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

                _filterTextBox.Text = string.Empty;
                _sortingComboBox.SelectedIndex = 2;

                var machineType = _machineTypeTextBox.Text.Trim();

                if (string.IsNullOrWhiteSpace(machineType) || machineType.Length != 4 || !_osComboBox.TryGetSelectedItem(out OS os))
                {
                    await SnackbarHelper.ShowAsync(Resource.PackagesPage_DownloadFailed_Title, Resource.PackagesPage_DownloadFailed_Message);
                    return;
                }

                _getPackagesTokenSource?.Cancel();
                _getPackagesTokenSource = new CancellationTokenSource();

                var token = _getPackagesTokenSource.Token;

                var packageDownloaderType = new[] {
                    _sourcePrimaryRadio,
                    _sourceSecondaryRadio,
                }.Where(r => r.IsChecked == true)
                 .Select(r => (PackageDownloaderFactory.Type)r.Tag)
                 .First();

                _packageDownloader = _packageDownloaderFactory.GetInstance(packageDownloaderType);
                var packages = await _packageDownloader.GetPackagesAsync(machineType, os, this, token);

                _packages = packages;

                Reload();
            }
            catch (TaskCanceledException)
            {
                errored = true;
            }
            catch (HttpRequestException ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Error occured when downloading packages.", ex);

                SnackbarHelper.Show("Something went wrong", "Check if your internet connection is up and running.", true);

                errored = true;
            }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Error occured when downloading packages.", ex);

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

        private async void FilterTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!await ShouldInterruptDownloadsIfRunning())
                return;

            try
            {
                if (_packages is null)
                    return;

                _filterDebounceCancellationTokenSource?.Cancel();
                _filterDebounceCancellationTokenSource = new();

                await Task.Delay(500, _filterDebounceCancellationTokenSource.Token);

                _packagesStackPanel.Children.Clear();
                _scrollViewer.ScrollToHome();

                Reload();
            }
            catch (TaskCanceledException) { }
        }

        private async void SortingComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!await ShouldInterruptDownloadsIfRunning())
                return;

            if (_packages is null)
                return;

            _packagesStackPanel.Children.Clear();
            _scrollViewer.ScrollToHome();

            Reload();
        }

        private string GetDownloadLocation()
        {
            var location = _downloadToText.Text.Trim();
            if (!Directory.Exists(location))
                return KnownFolders.GetPath(KnownFolder.Downloads);
            return location;
        }

        private ContextMenu? GetContextMenu(Package package, IEnumerable<Package> packages)
        {
            if (_packageDownloaderSettings.Store.HiddenPackages.Contains(package.Id))
                return null;

            var hideMenuItem = new MenuItem
            {
                SymbolIcon = SymbolRegular.EyeOff24,
                Header = "Hide",
            };
            hideMenuItem.Click += (s, e) =>
            {
                _packageDownloaderSettings.Store.HiddenPackages.Add(package.Id);
                _packageDownloaderSettings.SynchronizeStore();

                Reload();
            };

            var hideAllMenuItem = new MenuItem
            {
                SymbolIcon = SymbolRegular.EyeOff24,
                Header = "Hide all",
            };
            hideAllMenuItem.Click += (s, e) =>
            {
                foreach (var id in packages.Select(p => p.Id))
                    _packageDownloaderSettings.Store.HiddenPackages.Add(id);
                _packageDownloaderSettings.SynchronizeStore();

                Reload();
            };

            var cm = new ContextMenu();
            cm.Items.Add(hideMenuItem);
            cm.Items.Add(hideAllMenuItem);
            return cm;
        }

        private async Task<bool> ShouldInterruptDownloadsIfRunning()
        {
            if (_packagesStackPanel?.Children is null)
                return true;

            if (_packagesStackPanel.Children.ToArray().OfType<PackageControl>().Where(pc => pc.IsDownloading).IsEmpty())
                return true;

            return await MessageBoxHelper.ShowAsync(this, Resource.PackagesPage_DownloadInProgress_Title, Resource.PackagesPage_DownloadInProgress_Message);
        }

        private void Reload()
        {
            if (_packageDownloader is null)
                return;

            _packagesStackPanel.Children.Clear();

            if (_packages is null || !_packages.Any())
                return;

            var packages = SortAndFilter(_packages);

            foreach (var package in packages)
            {
                var control = new PackageControl(_packageDownloader, package, GetDownloadLocation)
                {
                    ContextMenu = GetContextMenu(package, packages)
                };
                _packagesStackPanel.Children.Add(control);
            }

            if (_packageDownloaderSettings.Store.HiddenPackages.Any())
            {
                var clearHidden = new Hyperlink
                {
                    Icon = SymbolRegular.Eye24,
                    Content = "Show hidden downloads",
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Right,
                };
                clearHidden.Click += (s, e) =>
                {
                    _packageDownloaderSettings.Store.HiddenPackages.Clear();
                    _packageDownloaderSettings.SynchronizeStore();

                    Reload();
                };
                _packagesStackPanel.Children.Add(clearHidden);
            }
        }

        private List<Package> SortAndFilter(List<Package> packages)
        {
            var result = _sortingComboBox.SelectedIndex switch
            {
                0 => packages.OrderBy(p => p.Title),
                1 => packages.OrderBy(p => p.Category),
                2 => packages.OrderByDescending(p => p.ReleaseDate),
                _ => packages.AsEnumerable(),
            };

            result = result.Where(p => !_packageDownloaderSettings.Store.HiddenPackages.Contains(p.Id));

            if (!string.IsNullOrWhiteSpace(_filterTextBox.Text))
                result = result.Where(p => p.Index.Contains(_filterTextBox.Text, StringComparison.InvariantCultureIgnoreCase));

            return result.ToList();
        }
    }
}
