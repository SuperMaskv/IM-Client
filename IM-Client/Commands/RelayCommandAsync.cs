using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace IM_Client.Commands
{
    public class RelayCommandAsync : ICommand
    {
        private readonly Func<Task> _execute;
        private readonly Predicate<object> _canExecute;
        private bool isExecuting;

        public RelayCommandAsync(Func<Task> execute) : this(execute, null)
        {
        }

        public RelayCommandAsync(Func<Task> execute, Predicate<object> canExecute)
        {
            if (execute == null) { throw new ArgumentNullException("execute"); }
            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }

            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }

        public async void Execute(object parameter)
        {
            isExecuting = true;
            try { await _execute(); }
            finally { isExecuting = false; }
        }
    }
}