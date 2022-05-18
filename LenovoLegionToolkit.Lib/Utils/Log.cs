using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

namespace LenovoLegionToolkit.Lib.Utils
{
    public class Log
    {
        private static Log? _instance;
        public static Log Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Log();
                return _instance;
            }
        }

        private readonly object _lock = new();
        private readonly string _folderPath;
        private readonly string _logPath;

        public bool IsTraceEnabled { get; set; } = false;

        public string LogPath => _logPath;

        public Log()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            _folderPath = Path.Combine(appData, "LenovoLegionToolkit", "log");
            Directory.CreateDirectory(_folderPath);
            _logPath = Path.Combine(_folderPath, $"log_{DateTime.Now:yyyy_MM_dd_HH_mm_ss}.txt");
        }

        public void Trace(FormattableString message,
            [CallerFilePath] string? file = null,
            [CallerLineNumber] int lineNumber = -1,
            [CallerMemberName] string? caller = null)
        {
            if (!IsTraceEnabled)
                return;

            lock (_lock)
            {
                var date = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                var fileName = Path.GetFileName(file);
                var line = $"[{date}] [{Environment.CurrentManagedThreadId}] [{fileName}#{lineNumber}:{caller}] {message}";
                File.AppendAllLines(_logPath, new[] { line });
            }
        }

        public void ErrorReport(Exception ex)
        {
            var errorReportPath = Path.Combine(_folderPath, $"error_{DateTime.Now:yyyy_MM_dd_HH_mm_ss}.txt");
            File.AppendAllLines(errorReportPath, new[] { ex.ToString() });
        }
    }
}