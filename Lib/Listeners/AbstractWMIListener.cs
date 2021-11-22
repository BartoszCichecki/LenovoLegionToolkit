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

        public event EventHandler<T> Changed;
        public AbstractWMIListener(string eventName, string property, int offset)
        {
            _evenName = eventName;
            _property = property;
            _offset = offset;
        }

        public void Start()
        {
            _watcher = new ManagementEventWatcher("ROOT\\WMI", $"SELECT * FROM LENOVO_GAMEZONE_{_evenName}_EVENT");
            _watcher.EventArrived += watcher_EventArrived;
            _watcher.Start();
        }

        public void Stop()
        {
            _watcher.Stop();
            _watcher.Dispose();
            _watcher = null;
        }

        protected abstract void OnChanged(T value);

        private void watcher_EventArrived(object sender, EventArrivedEventArgs e)
        {
            var propertyValue = Convert.ToInt32(e.NewEvent.Properties[_property].Value);
            var value = (T)(object)(propertyValue - _offset);
            OnChanged(value);
            Changed?.Invoke(this, value);
        }
    }
}
