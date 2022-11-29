using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using LenovoLegionToolkit.Lib;

namespace LenovoLegionToolkit.WPF.Extensions;

public class ClipboardExtensions
{
    public static void SetProcesses(IEnumerable<ProcessInfo> processes)
    {
        var sb = new StringBuilder();
        foreach (var process in processes)
            sb.AppendLine(process.ExecutablePath);
        Clipboard.SetText(sb.ToString());
    }

    public static IEnumerable<ProcessInfo> GetProcesses()
    {
        var text = Clipboard.GetText();
        return text.Split(Environment.NewLine)
            .Select(l => l.Trim('"'))
            .Where(File.Exists)
            .Distinct()
            .Select(ProcessInfo.FromPath);
    }
}