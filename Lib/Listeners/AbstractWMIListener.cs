using System;
using System.Management;

namespace LenovoLegionToolkit.Lib.Listeners
{
    public abstract class AbstractWMIListener<T> where T : struct, IComparable
    {
        private readonly string _evenName;
        private readonly string _property;
        private readonly int _offset;

        private ManagementEventWatcher _watcher;

        public AbstractWMIListener(string eventName, string property, int offset)
        {
            _evenName = eventName;
            _property = property;
            _offset = offset;
        }

        public void Start()
        {
            var scope = new ManagementScope("ROOT\\WMI");
            scope.Connect();
            var eventQuery = new EventQuery($"SELECT * FROM LENOVO_GAMEZONE_{_evenName}_EVENT");
            _watcher = new ManagementEventWatcher(scope, eventQuery);
            _watcher.EventArrived += _watcher_EventArrived;
            _watcher.Start();
        }

        public void Stop()
        {
            _watcher.Stop();
            _watcher.Dispose();
            _watcher = null;
        }

        private void _watcher_EventArrived(object sender, EventArrivedEventArgs e)
        {
            var propertyValue = Convert.ToInt32(e.NewEvent.Properties[_property].Value);
            var value = (T)(object)(propertyValue - _offset);
            OnChanged(value);
        }

        protected abstract void OnChanged(T value);
    }
}
