using System;
using System.Diagnostics;
using System.Windows.Input;

namespace InteractiveScheduler.Commands
{
    public class RelayCommand : ICommand
    {
        #region Fields

        protected Action<object> ToExecute { get; }
        private readonly Predicate<object> _canExecute;

        #endregion // Fields

        #region Constructors

        public RelayCommand(Action<object> toExecute, Predicate<object> canExecute = null)
        {
            if (toExecute == null)
                throw new ArgumentNullException(nameof(toExecute));

            ToExecute = toExecute;
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

        protected void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }

        public virtual void Execute(object parameter)
        {
            ToExecute(parameter);
        }

        #endregion // ICommand Members
    }
}