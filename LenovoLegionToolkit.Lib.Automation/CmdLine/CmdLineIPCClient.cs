using LenovoLegionToolkit.Lib.Utils;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LenovoLegionToolkit.Lib.Automation.CmdLine;

public class CmdLineIPCClient
{
    private NamedPipeClientStream _pipe = new(".", "LenovoLegionToolkit-IPC-0", PipeDirection.InOut, PipeOptions.None);

    public void RunQuickAction(string quickActionName)
    {
        try
        {
            _pipe.Connect(1000);
        }
        catch (Exception ex)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Pipe connect failed.", ex);

            return;
        }

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Pipe connected, sending Quick Action call: \"{quickActionName}\"...");

        var omsgpack = new IPCMessagePack()
        {
            QuickActionName = quickActionName
        };
        Serializer.SerializeWithLengthPrefix(_pipe, omsgpack, PrefixStyle.Base128);

        var imsgpack = Serializer.DeserializeWithLengthPrefix<IPCMessagePack>(_pipe, PrefixStyle.Base128);

        if (Log.Instance.IsTraceEnabled)
            Log.Instance.Trace($"Response received");

        if (imsgpack is null)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Deserialize failed.");
        }
        else if (imsgpack.State == CmdLineQuickActionRunState.ActionRunFailed)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Run Quick Action failed due to following reason: {imsgpack.Error ?? string.Empty}");
        }
        else if (imsgpack.State == CmdLineQuickActionRunState.ActionNotFound)
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Quick Action not found.");
        }
        else
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Run Quick Action successfully.");
        }
    }
}
