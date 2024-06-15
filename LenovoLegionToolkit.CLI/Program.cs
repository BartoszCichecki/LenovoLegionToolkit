using System;
using System.Reflection;
using System.Threading.Tasks;
using LenovoLegionToolkit.CLI.Resources;
using LenovoLegionToolkit.CLI.Utils;
using LenovoLegionToolkit.Lib.CLI;
using LenovoLegionToolkit.Lib.Extensions;

namespace LenovoLegionToolkit.CLI;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        var flags = new Flags(args);
        if (flags.Error)
        {
            Console.WriteLine(Resource.Error_IllegalCommandLineArgument_Text);
            return 1;
        }

        await LocalizationHelper.SetLanguageAsync();

        if (flags.ShowHelp)
        {
            ShowHelpMessage();
            return 0;
        }

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

    private static void ShowHelpMessage()
    {
        Console.WriteLine($"llt.exe - {Resource.HelpMessage_ExeDescription}");
        Console.WriteLine($"{Resource.HelpMessage_Version} {GetVersion()}\n");
        Console.WriteLine($"{Resource.HelpMessage_AvailableArguments}");
        Console.WriteLine($" * --help\t\t\t{Resource.HelpMessage_Argument_Help}");
        Console.WriteLine($" * --silent\t\t\t{Resource.HelpMessage_Argument_Silent}");
        Console.WriteLine($" * --quickAction={{Quick Action}}\t{Resource.HelpMessage_Argument_QuickAction}\n");
    }

    private static string GetVersion()
    {
        var version = Assembly.GetEntryAssembly()?.GetName().Version;
        if (version is null)
        {
            return string.Empty;
        }
        else if (version.IsBeta())
        {
            return "BETA";
        }
        else
        {
            return version.ToString(3);
        }
    }

    private static async Task ExecuteQuickActionRunAsync(string quickAcionName, bool silent)
    {
        var client = new CmdLineIPCClient();
        await client.RunQuickActionAsync(quickAcionName);

        if (silent)
        {
            return;
        }

        Console.WriteLine(client.QuickActionState switch
        {
            CLIQuickActionRunState.Ok => Resource.QuickActionRun_Ok_Text,
            CLIQuickActionRunState.ActionRunFailed => (Resource.QuickActionRun_Error_ActionRunFailed_Text, client.Errmsg ?? string.Empty),
            CLIQuickActionRunState.ActionNotFound => Resource.QuickActionRun_Error_ActionNotFound_Text,
            CLIQuickActionRunState.DeserializeFailed => Resource.QuickActionRun_Error_DeserializeFailed_Text,
            CLIQuickActionRunState.Status_ServerNotRunning => Resource.QuickActionRun_Error_ServerNotRunning_Text,
            CLIQuickActionRunState.Status_PipeConnectFailed => (Resource.QuickActionRun_Error_PipeConnectFailed_Text, client.Errmsg ?? string.Empty),
            _ => Resource.QuickActionRun_Error_Undefined_Text
        });
    }
}
