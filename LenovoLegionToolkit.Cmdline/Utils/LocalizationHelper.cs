using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LenovoLegionToolkit.Cmdline.Resources;
using LenovoLegionToolkit.Lib.Utils;

namespace LenovoLegionToolkit.Cmdline.Utils;

public class LocalizationHelper
{
    private static readonly string LanguagePath = Path.Combine(Folders.AppData, "lang");

    private static readonly CultureInfo DefaultLanguage = new("en");

    public static readonly CultureInfo[] Languages = [
        DefaultLanguage,
        new("ar"),
        new("bg"),
        new("cs"),
        new("de"),
        new("el"),
        new("es"),
        new("fr"),
        new("it"),
        new("ja"),
        new("lv"),
        new("nl-nl"),
        new("pt"),
        new("pt-br"),
        new("ro"),
        new("ru"),
        new("sk"),
        new("tr"),
        new("uk"),
        new("vi"),
        new("zh-hans"),
        new("zh-hant"),
        new("uz-latn-uz") // HACK: Karakalpak is not a recognized culture by msbuild, so we use this one as workaround instead.
    ];

    public static async Task SetLanguageAsync()
    {
        CultureInfo cultureInfo = await GetLanguageAsync();
        SetLanguageInternal(cultureInfo);
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

    public static async Task<CultureInfo> GetLanguageAsync()
    {
        var cultureInfo = await GetLanguageFromFile() ?? DefaultLanguage;
        return cultureInfo;
    }

    private static void SetLanguageInternal(CultureInfo cultureInfo)
    {
        Thread.CurrentThread.CurrentCulture = new CultureInfo("en");
        CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en");

        Thread.CurrentThread.CurrentUICulture = cultureInfo;
        CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

        Resource.Culture = cultureInfo;
    }
}
