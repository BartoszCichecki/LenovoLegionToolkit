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

        TryCreate();
    }

    public float NextValue()
    {
        try
        {
            return _performanceCounter?.NextValue() ?? 0f;
        }
        catch
        {
            TryCreate();
            return 0f;
        }
    }

    private void TryCreate()
    {
        try { _performanceCounter = new(_categoryName, _counterName, _instanceName); }
        catch { _performanceCounter = null; }
    }
}
