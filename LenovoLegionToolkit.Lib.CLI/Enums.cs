namespace LenovoLegionToolkit.Lib.CLI;

public enum QuickActionResponseState
{
    Undefined,
    Ok,
    ActionNotFound,
    ActionRunFailed,
    DeserializeFailed,
    Status_ServerNotRunning,
    Status_PipeConnectFailed
}
