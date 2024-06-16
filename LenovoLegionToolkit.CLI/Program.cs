using System;
using System.Threading.Tasks;
using LenovoLegionToolkit.CLI.Lib;

namespace LenovoLegionToolkit.CLI;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        var flags = Flags.Create(args);

        try
        {
            if (flags.Help)
                PrintHelp(flags);
            else if (flags.QuickActionRunName is { } name)
                await IpcClient.RunQuickActionAsync(name);
            else
                throw new InvalidOperationException();

            return 0;
        }
        catch (IpcException ex)
        {
            PrintLine(flags, ex.Message);
            return 3;
        }
        catch (InvalidOperationException)
        {
            PrintHelp(flags);
            return 2;
        }
        catch (Exception ex)
        {
            PrintLine(flags, ex.Message);
            return 1;
        }
    }

    private static void PrintHelp(Flags flags)
    {
        PrintLine(flags, [
            "Lenovo Legion Toolkit CLI",
            "",
            "Usage:",
            "  --quickAction=<name>, -qa=<name> - run Quick Action with specified name",
            "  --silent, -s                     - suppress output",
            "  --help, -h                       - display this help",
            ""
        ]);
    }

    private static void PrintLine(Flags flags, params string[] messages)
    {
        if (flags.Silent)
            return;

        foreach (var message in messages)
            Console.WriteLine(message);
    }
}
