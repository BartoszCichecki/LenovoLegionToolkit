using System.Diagnostics;

namespace LenovoLegionToolkit.Lib.Utils;

public class SafePerformanceCounter
{
    private readonly string _categoryName;
    private readonly string _counterName;
    private readonly string _instanceName;

    private PerformanceCounter? _performanceCounter;

    public SafePerformanceCounter(string categoryName, string counterName, string instanceName)
    {
        _categoryName = categoryName;
        _counterName = counterName;
        _instanceName = instanceName;

        _ = TryCreate();
    }

    public float NextValue() => _performanceCounter?.NextValue() ?? 0f;

    private PerformanceCounter? TryCreate()
    {
        try { _performanceCounter = new(_categoryName, _counterName, _instanceName); }
        catch { _performanceCounter = null; }
        return _performanceCounter;
    }
}
