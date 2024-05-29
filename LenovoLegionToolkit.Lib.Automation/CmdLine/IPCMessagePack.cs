using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LenovoLegionToolkit.Lib.Automation.CmdLine;

[ProtoContract]
internal class IPCMessagePack
{
    [ProtoMember(1)]
    public string? QuickActionName { get; set; }
    [ProtoMember(2)]
    public CmdLineQuickActionRunState State { get; set; }
    [ProtoMember(3)]
    public string? Error { get; set; }
}
