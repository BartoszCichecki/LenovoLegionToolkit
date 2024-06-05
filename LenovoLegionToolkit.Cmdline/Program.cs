using LenovoLegionToolkit.Cmdline.Resources;
using LenovoLegionToolkit.Cmdline.Utils;
using LenovoLegionToolkit.Lib.Automation;
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
            await ExecuteQuickActionRunAsync(flags.QuickActionRunName ?? string.Empty, flags.Silent);
            return 0;
        }

        if (!flags.Silent)
        {
            Console.WriteLine(Resource.Error_IllegalCommandLineArgument_Text);
        }
        return 1;
    }

    private static async Task ExecuteQuickActionRunAsync(string quickAcionName, bool silent)
    {
        var client = new CmdLineIPCClient();
        await client.RunQuickActionAsync(quickAcionName);

        if (!silent)
        {
            if (client.State == CmdLineQuickActionRunState.ActionRunFailed)
            {
                Console.WriteLine(Resource.QuickActionRun_Error_ActionRunFailed_Text, client.Errmsg ?? string.Empty);
            }
            else if (client.State == CmdLineQuickActionRunState.ActionNotFound)
            {
                Console.WriteLine(Resource.QuickActionRun_Error_ActionNotFound_Text);
            }
            else if (client.State == CmdLineQuickActionRunState.DeserializeFailed)
            {
                Console.WriteLine(Resource.QuickActionRun_Error_DeserializeFailed_Text);
            }
            else if (client.State == CmdLineQuickActionRunState.ServerNotRunning)
            {
                Console.WriteLine(Resource.QuickActionRun_Error_ServerNotRunning_Text);
            }
            else if (client.State == CmdLineQuickActionRunState.PipeConnectFailed)
            {
                Console.WriteLine(Resource.QuickActionRun_Error_PipeConnectFailed_Text, client.Errmsg ?? string.Empty);
            }    
            else if (client.State == CmdLineQuickActionRunState.Ok)
            {
                Console.WriteLine(Resource.QuickActionRun_Ok_Text);
            }
            else
            {
                Console.WriteLine(Resource.QuickActionRun_Error_Undefined_Text);
            }
        }
    }
}
