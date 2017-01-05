using System;
using System.Diagnostics;
using System.Windows.Input;

namespace InteractiveScheduler.Commands
{
    public class RelayCommand : ICommand
    {
        #region Fields

        private readonly Action<object> _toExecute;
        private readonly Predicate<object> _canExecute;

        #endregion // Fields

        #region Constructors

        public RelayCommand(Action<object> toExecute, Predicate<object> canExecute = null)
        {
            if (toExecute == null)
                throw new ArgumentNullException(nameof(toExecute));

            _toExecute = toExecute;
            _canExecute = canExecute;
        }
        #endregion // Constructors

        #region ICommand Members

        [DebuggerStepThrough]
        public virtual bool CanExecute(object parameter)
        {
            return _canExecute?.Invoke(parameter) ?? true;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public virtual void Execute(object parameter)
        {
            _toExecute(parameter);
        }

        #endregion // ICommand Members
    }
}