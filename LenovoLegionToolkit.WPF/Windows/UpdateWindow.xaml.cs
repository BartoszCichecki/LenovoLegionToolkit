using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.WPF.Windows
{
    public partial class UpdateWindow
    {
        private class DownloadProgress : IProgress<float>
        {
            private readonly ProgressBar _progressBar;

            public DownloadProgress(ProgressBar progressBar)
            {
                _progressBar = progressBar;
            }

            public void Report(float value)
            {
                _progressBar.IsIndeterminate = !(value > 0);
                _progressBar.Value = value;
            }
        }

        private readonly UpdateChecker _updateChecker = DIContainer.Resolve<UpdateChecker>();

        private CancellationTokenSource? _downloadCancellationTokenSource;

        public UpdateWindow()
        {
            InitializeComponent();

            ResizeMode = ResizeMode.CanMinimize;

            _titleBar.UseSnapLayout = false;
            _titleBar.CanMaximize = false;

            Loaded += UpdateWindow_Loaded;
            Closing += UpdateWindow_Closing;
        }

        private async void UpdateWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var updates = await _updateChecker.GetUpdates();

            var stringBuilder = new StringBuilder();
            foreach (var update in updates)
            {
                stringBuilder.AppendLine("**" + update.Title + "**")
                    .AppendLine()
                    .AppendLine(update.Description)
                    .AppendLine();
            }

            _markdownViewer.Markdown = stringBuilder.ToString();

            _downloadButton.IsEnabled = true;
        }

        private void UpdateWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e) => _downloadCancellationTokenSource?.Cancel();

        private async void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _downloadCancellationTokenSource?.Cancel();
                _downloadCancellationTokenSource = new CancellationTokenSource();

                SetDownloading(true);

                var path = await _updateChecker.DownloadLatestUpdate(new DownloadProgress(_downloadProgressBar), _downloadCancellationTokenSource.Token);

                _downloadCancellationTokenSource = null;

                Process.Start(path);
                Application.Current.Shutdown();
            }
            catch (OperationCanceledException)
            {
                SetDownloading(false);
            }
            catch
            {
                SetDownloading(false);

                Process.Start(new ProcessStartInfo("https://github.com/BartoszCichecki/LenovoLegionToolkit/releases/latest") { UseShellExecute = true });
                Close();
            }
        }

        private void CancelDownloadButton_Click(object sender, RoutedEventArgs e) => _downloadCancellationTokenSource?.Cancel();

        private void SetDownloading(bool isDownloading)
        {
            if (isDownloading)
            {
                _downloadProgressBar.Visibility = Visibility.Visible;

                _downloadButton.Visibility = Visibility.Collapsed;
                _downloadButton.IsEnabled = false;

                _cancelDownloadButton.Visibility = Visibility.Visible;
                _cancelDownloadButton.IsEnabled = true;
            }
            else
            {
                _downloadProgressBar.Value = 0;
                _downloadProgressBar.IsIndeterminate = true;
                _downloadProgressBar.Visibility = Visibility.Hidden;

                _downloadButton.Visibility = Visibility.Visible;
                _downloadButton.IsEnabled = true;

                _cancelDownloadButton.Visibility = Visibility.Collapsed;
                _cancelDownloadButton.IsEnabled = false;
            }
        }
    }
}
