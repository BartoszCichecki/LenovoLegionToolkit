using System;
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
    }

    public float NextValue()
    {
        try
        {
            TryCreateIfNeeded();
            return _performanceCounter?.NextValue() ?? 0f;
        }
        catch
        {
            return 0f;
        }
    }

    public void Reset()
    {
        _performanceCounter = null;
        _ = NextValue();
    }

    private void TryCreateIfNeeded()
    {
        if (_performanceCounter is not null)
            return;

        try
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Creating performance counter. [categoryName={_categoryName}, counterName={_counterName}, instanceName={_instanceName}]");

            _performanceCounter = new(_categoryName, _counterName, _instanceName);
        }
        catch (Exception ex)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Failed to create performance counter. [categoryName={_categoryName}, counterName={_counterName}, instanceName={_instanceName}]", ex);

            _performanceCounter = null;
        }
    }
}
