using ProtoBuf;

namespace LenovoLegionToolkit.Lib.CLI.Datapacks;

[ProtoContract]
public class QuickActionRequest
{
    [ProtoMember(1)]
    public string? Name { get; set; }
}
