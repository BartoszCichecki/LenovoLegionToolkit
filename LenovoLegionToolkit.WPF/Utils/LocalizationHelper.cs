using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Utils;
using LenovoLegionToolkit.WPF.Resources;

namespace LenovoLegionToolkit.WPF.Utils
{
    public static class LocalizationHelper
    {
        private static readonly string LanguagePath = Path.Combine(Folders.AppData, "lang");

        public static readonly CultureInfo[] Languages = {
            new("en"),
            new("zh-hans"),
            new("es"),
            new("nl-nl"),
            new("pt"),
            new("pt-br"),
            new("ro"),
            new("ru"),
            new("tr"),
            new("ukr"),
            new("vi"),
        };

        public static async Task SetLanguageAsync()
        {
            var cultureInfo = await GetLanguageAsync();
            SetLanguageInternal(cultureInfo);
        }

        public static async Task SetLanguageAsync(CultureInfo cultureInfo)
        {
            await File.WriteAllTextAsync(LanguagePath, cultureInfo.Name);
            SetLanguageInternal(cultureInfo);
        }

        public static async Task<CultureInfo> GetLanguageAsync()
        {
            var defaultCulture = new CultureInfo("en");
            CultureInfo? cultureInfo = null;

            try
            {
                var name = await File.ReadAllTextAsync(LanguagePath);
                cultureInfo = new CultureInfo(name);
                if (!Languages.Contains(cultureInfo))
                    throw new InvalidOperationException("Unknown language.");
            }
            catch
            {
                await File.WriteAllTextAsync(LanguagePath, defaultCulture.Name);
            }

            return cultureInfo ?? defaultCulture;
        }

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
