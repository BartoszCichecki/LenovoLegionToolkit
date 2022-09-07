using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace LenovoLegionToolkit.Lib.Utils
{
    public class Log
    {
        private static Log? _instance;
        public static Log Instance
        {
            get
            {
                _instance ??= new Log();
                return _instance;
            }
        }

        private readonly object _lock = new();
        private readonly string _folderPath;
        private readonly string _logPath;

        public bool IsTraceEnabled { get; set; }

        private Log()
        {
            _folderPath = Path.Combine(Folders.AppData, "log");
            Directory.CreateDirectory(_folderPath);
            _logPath = Path.Combine(_folderPath, $"log_{DateTime.UtcNow:yyyy_MM_dd_HH_mm_ss}.txt");
        }

        public void ErrorReport(Exception ex)
        {
            var errorReportPath = Path.Combine(_folderPath, $"error_{DateTime.UtcNow:yyyy_MM_dd_HH_mm_ss}.txt");
            File.AppendAllLines(errorReportPath, new[] { Serialize(ex) });
        }

        public void Trace(FormattableString message,
            Exception? ex = null,
            [CallerFilePath] string? file = null,
            [CallerLineNumber] int lineNumber = -1,
            [CallerMemberName] string? caller = null)
        {
            if (!IsTraceEnabled)
                return;

            LogInternal(_logPath, message, ex, file, lineNumber, caller);
        }

        public void Debug(string fileName,
            FormattableString message,
            Exception? ex = null,
            [CallerFilePath] string? file = null,
            [CallerLineNumber] int lineNumber = -1,
            [CallerMemberName] string? caller = null)
        {
            var logPath = Path.Combine(_folderPath, $"{fileName}.txt");
            LogInternal(logPath, message, ex, file, lineNumber, caller);
        }

        private void LogInternal(string path,
            FormattableString message,
            Exception? ex,
            string? file,
            int lineNumber,
            string? caller)
        {
            lock (_lock)
            {
                var lines = new List<string>
                {
                    $"[{DateTime.UtcNow:dd/MM/yyyy HH:mm:ss}] [{Environment.CurrentManagedThreadId}] [{Path.GetFileName(file)}#{lineNumber}:{caller}] {message}"
                };
                if (ex is not null)
                    lines.Add(Serialize(ex));
                File.AppendAllLines(path, lines);
            }
        }

        private static string Serialize(Exception ex) => new StringBuilder()
            .AppendLine("=== Exception ===")
            .AppendLine(ex.ToString())
            .AppendLine()
            .AppendLine("=== Exception demystified ===")
            .AppendLine(ex.ToStringDemystified())
            .ToString();
    }
}