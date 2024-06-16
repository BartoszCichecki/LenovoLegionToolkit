using System;

namespace LenovoLegionToolkit.CLI.Lib;

public class IpcException : Exception
{
    public IpcException() { }

    public IpcException(string? name) : base(name) { }

    public IpcException(string? name, Exception innerException) : base(name, innerException) { }
}
