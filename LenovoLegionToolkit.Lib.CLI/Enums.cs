namespace LenovoLegionToolkit.Lib.CLI;

public enum CLIQuickActionRunState
{
    Undefined,
    Ok,
    ActionNotFound,
    ActionRunFailed,
    DeserializeFailed,
    Status_ServerNotRunning,
    Status_PipeConnectFailed
}
