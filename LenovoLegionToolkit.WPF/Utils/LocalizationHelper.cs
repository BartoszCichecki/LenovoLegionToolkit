using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using System.Threading;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Utils;
using LenovoLegionToolkit.WPF.Resources;

namespace LenovoLegionToolkit.WPF.Utils
{
    public static class LocalizationHelper
    {
        private static readonly string LanguagePath = Path.Combine(Folders.AppData, "lang");

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
                if (!GetLanguages().Contains(cultureInfo))
                    throw new InvalidOperationException("Unknown language.");
            }
            catch
            {
                await File.WriteAllTextAsync(LanguagePath, defaultCulture.Name);
            }

            return cultureInfo ?? defaultCulture;
        }

        public static IEnumerable<CultureInfo> GetLanguages()
        {
            foreach (var culture in CultureInfo.GetCultures(CultureTypes.AllCultures))
            {
                ResourceSet? rs = null;

                try { rs = Resource.ResourceManager.GetResourceSet(culture, true, false); }
                catch { }

                if (rs is null)
                    continue;

                if (culture.Equals(CultureInfo.InvariantCulture))
                    continue;

                yield return culture;
            }
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
