using ProtoBuf;

namespace LenovoLegionToolkit.CLI.Lib;

[ProtoContract]
public class IpcRequest
{
    [ProtoMember(1, IsRequired = true)]
    public string Name { get; init; } = "";
}
