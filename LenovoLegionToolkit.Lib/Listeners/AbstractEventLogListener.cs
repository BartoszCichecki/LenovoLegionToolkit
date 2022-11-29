﻿using System;
using System.Diagnostics.Eventing.Reader;
using System.Threading.Tasks;

namespace LenovoLegionToolkit.Lib.Listeners;

public abstract class AbstractEventLogListener : IListener<EventArgs>
{
    private readonly EventLogWatcher _watcher;

    public AbstractEventLogListener(string path, string query)
    {
        var eventLogQuery = new EventLogQuery(path, PathType.LogName, query);
        _watcher = new EventLogWatcher(eventLogQuery);
        _watcher.EventRecordWritten += Watcher_EventRecordWritten;
    }

    public event EventHandler<EventArgs>? Changed;

    public Task StartAsync()
    {
        if (!_watcher.Enabled)
            _watcher.Enabled = true;

        return Task.CompletedTask;
    }

    public Task StopAsync()
    {
        _watcher.Enabled = false;
        return Task.CompletedTask;
    }

    protected abstract Task OnChangedAsync();

    private async void Watcher_EventRecordWritten(object? sender, EventRecordWrittenEventArgs e)
    {
        await OnChangedAsync().ConfigureAwait(false);
        Changed?.Invoke(this, EventArgs.Empty);
    }
}