using LenovoLegionToolkit.Cmdline.Resources;
using LenovoLegionToolkit.Cmdline.Utils;
using LenovoLegionToolkit.Lib.Utils;
using LenovoLegionToolkit.Lib.Automation.CmdLine;

namespace LenovoLegionToolkit.Cmdline;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        var flags = new Flags(args);

        LocalizationHelper.SetLanguageAsync().Wait();

        if (flags.QuickActionRunName is not null)
        {
            await ExecuteQuickActionRunAsync(flags.QuickActionRunName ?? string.Empty);
            return 0;
        }

        if (!flags.Silent)
        {
            Console.WriteLine(Resource.Error_IllegalCommandLineArgument_Text);
        }
        return 1;
    }

    private static async Task ExecuteQuickActionRunAsync(string quickAcionName)
    {
        var client = new CmdLineIPCClient();
        await client.RunQuickActionAsync(quickAcionName);

        return;
    }
}
