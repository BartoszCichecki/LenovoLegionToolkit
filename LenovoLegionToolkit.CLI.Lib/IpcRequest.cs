namespace LenovoLegionToolkit.CLI.Lib;

public class IpcRequest
{
    public enum OperationType
    {
        Unknown,
        ListFeatures,
        ListFeatureValues,
        ListQuickActions,
        GetFeatureValue,
        SetFeatureValue,
        GetSpectrumProfile,
        SetSpectrumProfile,
        GetSpectrumBrightness,
        SetSpectrumBrightness,
        GetRGBPreset,
        SetRGBPreset,
        QuickAction,
    }

    public OperationType? Operation { get; init; }

    public string? Name { get; init; }

    public string? Value { get; init; }
}
