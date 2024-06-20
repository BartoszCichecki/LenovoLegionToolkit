namespace LenovoLegionToolkit.CLI.Lib;

public class IpcRequest
{
    public enum OperationType
    {
        QuickAction,
        ListFeatures,
        ListFeatureValues,
        SetFeature,
        GetFeature,
    }

    public OperationType? Operation { get; init; }

    public string? Name { get; init; }

    public string? Value { get; init; }
}
