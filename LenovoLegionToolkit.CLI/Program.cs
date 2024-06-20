using System;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using System.CommandLine.Parsing;
using System.Threading.Tasks;
using LenovoLegionToolkit.CLI.Lib;

namespace LenovoLegionToolkit.CLI;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        var root = new RootCommand("Control LLT");

        var builder = new CommandLineBuilder(root)
            .UseDefaults()
            .UseExceptionHandler(OnException);

        var quickActionNameArgument = new Argument<string>("name", "Name of the QuickAction") { Arity = ArgumentArity.ExactlyOne };
        var featureArgument = new Argument<string>("feature", "Name of the feature") { Arity = ArgumentArity.ExactlyOne };
        var valueArgument = new Argument<string>("value", "Value of the feature") { Arity = ArgumentArity.ExactlyOne };

        var runQuickActionCommand = new Command("quick-action", "Run Quick Action with specified name");
        runQuickActionCommand.AddAlias("qa");
        runQuickActionCommand.AddArgument(quickActionNameArgument);
        runQuickActionCommand.SetHandler(IpcClient.RunQuickActionAsync, quickActionNameArgument);
        root.AddCommand(runQuickActionCommand);

        var listFeaturesCommand = new Command("list-features", "List supported features");
        listFeaturesCommand.AddAlias("lf");
        listFeaturesCommand.SetHandler(async _ =>
        {
            var value = await IpcClient.ListFeatures();
            Console.WriteLine(value);
        });
        root.AddCommand(listFeaturesCommand);

        var listFeatureValuesCommand = new Command("list-feature-values", "List values supported by a feature");
        listFeatureValuesCommand.AddAlias("lfv");
        listFeatureValuesCommand.SetHandler(async name =>
        {
            var value = await IpcClient.ListFeatureValues(name);
            Console.WriteLine(value);
        }, featureArgument);
        listFeatureValuesCommand.AddArgument(featureArgument);
        root.AddCommand(listFeatureValuesCommand);

        var setFeatureCommand = new Command("set-feature-value", "Set feature with value");
        setFeatureCommand.AddAlias("sfv");
        setFeatureCommand.AddArgument(featureArgument);
        setFeatureCommand.AddArgument(valueArgument);
        setFeatureCommand.SetHandler(IpcClient.SetFeature, featureArgument, valueArgument);
        root.AddCommand(setFeatureCommand);

        var getFeatureCommand = new Command("get-feature-value", "Get feature value");
        getFeatureCommand.AddAlias("gfv");
        getFeatureCommand.AddArgument(featureArgument);
        getFeatureCommand.SetHandler(async name =>
        {
            var value = await IpcClient.GetFeature(name);
            Console.WriteLine(value);
        }, featureArgument);
        root.AddCommand(getFeatureCommand);

        return await builder.Build().InvokeAsync(args);
    }

    private static void OnException(Exception ex, InvocationContext context)
    {
        var message = ex switch
        {
            IpcException => ex.Message,
            _ => ex.ToString()
        };
        var exitCode = ex switch
        {
            IpcException => -1,
            _ => -99
        };

        context.Console.Error.WriteLine(message);
        context.ExitCode = exitCode;
    }
}
