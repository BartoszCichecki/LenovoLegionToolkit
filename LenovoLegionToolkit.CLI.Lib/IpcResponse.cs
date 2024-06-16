using ProtoBuf;

namespace LenovoLegionToolkit.CLI.Lib;

[ProtoContract]
public class IpcResponse
{
    [ProtoMember(1, IsRequired = true)]
    public bool Success { get; init; }

    [ProtoMember(2)]
    public string? Message { get; init; }
}
