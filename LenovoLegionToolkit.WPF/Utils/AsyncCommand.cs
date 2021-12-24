using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LenovoLegionToolkit.WPF.Utils
{
    public interface IAsyncCommand : ICommand
    {
        bool Enabled { get; set; }
    }

    public class AsyncCommand : IAsyncCommand
    {
        public event EventHandler CanExecuteChanged;

        public bool Enabled
        {
            get => _canExecute && _canExecuteInternal;
            set
            {
                _canExecute = value;
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private readonly Func<Task> _action;
        private bool _canExecute = true;
        private bool _canExecuteInternal = true;

        public AsyncCommand(Func<Task> action)
        {
            _action = action;
        }

        public bool CanExecute(object parameter) => _canExecute && _canExecuteInternal;

        public async void Execute(object parameter)
        {
            _canExecuteInternal = false;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);

            await _action();

            _canExecuteInternal = true;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
