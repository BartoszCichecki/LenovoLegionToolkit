using System;
using System.Reflection;
using System.Threading.Tasks;
using LenovoLegionToolkit.CLI.Resources;
using LenovoLegionToolkit.CLI.Utils;
using LenovoLegionToolkit.Lib.Automation;
using LenovoLegionToolkit.Lib.Automation.CmdLine;
using LenovoLegionToolkit.Lib.Extensions;

namespace LenovoLegionToolkit.Cmdline;

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
        Console.WriteLine($" * --help\t\t{Resource.HelpMessage_Argument_Help}");
        Console.WriteLine($" * --silent\t\t{Resource.HelpMessage_Argument_Silent}");
        Console.WriteLine($" * --run={{Quick Action}}\t{Resource.HelpMessage_Argument_Run}\n");
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
