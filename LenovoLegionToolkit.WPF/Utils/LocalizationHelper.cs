using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using LenovoLegionToolkit.Lib.Utils;
using LenovoLegionToolkit.WPF.Resources;
using LenovoLegionToolkit.WPF.Windows.Utils;

namespace LenovoLegionToolkit.WPF.Utils
{
    public static class LocalizationHelper
    {
        private static readonly string LanguagePath = Path.Combine(Folders.AppData, "lang");

        public static readonly CultureInfo DefaultLanguage = new("en");

        public static readonly CultureInfo[] Languages = {
            DefaultLanguage,
            //new("ar"),
            new("cs"),
            new("de"),
            new("el"),
            new("es"),
            new("fr"),
            new("it"),
            new("nl-nl"),
            new("pt"),
            new("pt-br"),
            new("ro"),
            new("ru"),
            new("tr"),
            new("uk"),
            new("vi"),
            new("zh-hans"),
        };

        public static FlowDirection Direction => Resource.Culture.TextInfo.IsRightToLeft
            ? FlowDirection.RightToLeft
            : FlowDirection.LeftToRight;

        public static HorizontalAlignment ReverseHorizontalAlignment => Resource.Culture.TextInfo.IsRightToLeft
            ? HorizontalAlignment.Left
            : HorizontalAlignment.Right;

        public static async Task SetLanguageAsync(bool interactive = false)
        {
            CultureInfo? cultureInfo = null;

            if (interactive && await GetLanguageFromFile() is null)
            {
                var window = new LanguageSelectorWindow(Languages, DefaultLanguage);
                window.Show();
                cultureInfo = await window.ShouldContinue;
                if (cultureInfo is not null)
                    await SaveLanguageToFileAsync(cultureInfo);
            }

            cultureInfo ??= await GetLanguageAsync();

            SetLanguageInternal(cultureInfo);
        }

        public static async Task SetLanguageAsync(CultureInfo cultureInfo)
        {
            await SaveLanguageToFileAsync(cultureInfo);
            SetLanguageInternal(cultureInfo);
        }

        public static async Task<CultureInfo> GetLanguageAsync()
        {
            var cultureInfo = await GetLanguageFromFile();
            if (cultureInfo is null)
            {
                cultureInfo = DefaultLanguage;
                await SaveLanguageToFileAsync(cultureInfo);
            }
            return cultureInfo;
        }

        private static async Task<CultureInfo?> GetLanguageFromFile()
        {
            try
            {
                var name = await File.ReadAllTextAsync(LanguagePath);
                var cultureInfo = new CultureInfo(name);
                if (!Languages.Contains(cultureInfo))
                    throw new InvalidOperationException("Unknown language.");
                return cultureInfo;
            }
            catch
            {
                return null;
            }
        }

        private static Task SaveLanguageToFileAsync(CultureInfo cultureInfo) => File.WriteAllTextAsync(LanguagePath, cultureInfo.Name);

        private static void SetLanguageInternal(CultureInfo cultureInfo)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en");
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en");

            Thread.CurrentThread.CurrentUICulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            Resource.Culture = cultureInfo;
            Lib.Resources.Resource.Culture = cultureInfo;
            Lib.Automation.Resources.Resource.Culture = cultureInfo;
        }
    }
}
