using System;
using System.Diagnostics.Eventing.Reader;
using System.Threading.Tasks;
using LenovoLegionToolkit.Lib.Utils;

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
        }

        public event EventHandler<EventArgs>? Changed;

        public void Start()
        {
            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Starting... [listener={GetType().Name}]");

            _watcher.Enabled = true;
        }

        public void Stop()
        {
            _watcher.Enabled = false;

            if (Log.Instance.IsTraceEnabled)
                Log.Instance.Trace($"Stopped [listener={GetType().Name}]");
        }

        protected abstract Task OnChangedAsync();

        private async void Watcher_EventRecordWritten(object? sender, EventRecordWrittenEventArgs e)
        {
            await OnChangedAsync().ConfigureAwait(false);
            Changed?.Invoke(this, EventArgs.Empty);
        }
    }
}
