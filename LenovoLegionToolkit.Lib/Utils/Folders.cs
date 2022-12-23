using System;
using System.IO;

namespace LenovoLegionToolkit.Lib.Utils;

public static class Folders
{
    public static string AppData
    {
        get
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var folderPath = Path.Combine(appData, "LenovoLegionToolkit");
            Directory.CreateDirectory(folderPath);
            return folderPath;
        }
    }

    public static string Temp
    {
        get
        {
            var appData = Path.GetTempPath();
            var folderPath = Path.Combine(appData, "LenovoLegionToolkit");
            Directory.CreateDirectory(folderPath);
            return folderPath;
        }
    }
}