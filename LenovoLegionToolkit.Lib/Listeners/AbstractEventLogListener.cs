using System;
using System.Diagnostics.Eventing.Reader;

namespace LenovoLegionToolkit.Lib.Listeners
{

    public abstract class AbstractEventLogListener : IListener<EventArgs>
    {
        private readonly EventLogWatcher _watcher;

        public AbstractEventLogListener(string path, string query)
        {
            var eventLogQuery = new EventLogQuery(path, PathType.LogName, query);
            _watcher = new EventLogWatcher(eventLogQuery);
            _watcher.EventRecordWritten += Watcher_EventRecordWritten;
            _watcher.Enabled = true;
        }

        public event EventHandler<EventArgs>? Changed;

        protected abstract void OnChanged();

        private void Watcher_EventRecordWritten(object? sender, EventRecordWrittenEventArgs e)
        {
            OnChanged();
            Changed?.Invoke(this, EventArgs.Empty);
        }
    }
}
