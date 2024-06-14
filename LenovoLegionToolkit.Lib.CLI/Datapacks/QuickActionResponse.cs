using ProtoBuf;

namespace LenovoLegionToolkit.Lib.CLI.Datapacks;

[ProtoContract]
public class QuickActionResponse
{
    [ProtoMember(1)]
    public CLIQuickActionRunState State { get; set; }
    [ProtoMember(2)]
    public string? Error { get; set; }
}
