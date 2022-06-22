using System;
using System.Threading.Tasks;

namespace LenovoLegionToolkit.WPF
{
    public struct Continuable
    {
        private readonly Func<Task> _action;

        public Continuable(Func<Task> action) => _action = action;

        public Task ContinueAsync() => _action();
    }
}
