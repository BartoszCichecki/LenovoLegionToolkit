using System;

namespace LenovoLegionToolkit.Lib.Features.Hybrid;

public class IGPUModeChangeException : Exception
{
    public IGPUModeState IGPUMode { get; }

    public IGPUModeChangeException(IGPUModeState igpuMode)
    {
        IGPUMode = igpuMode;
    }
}
