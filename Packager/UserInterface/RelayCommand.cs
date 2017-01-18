using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Packager.UserInterface
{
    public class RelayCommand : ICommand
    {
        #region Fields

        protected readonly Action<object> ToExecute;
        readonly Predicate<object> _canExecute;

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

        public virtual void Execute(object parameter)
        {
            ToExecute(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
        
        #endregion // ICommand Members
    }

    public class AsyncRelayCommand : RelayCommand
    {
        private bool _isExecuting;

        public event EventHandler Started;

        public event EventHandler Ended;

        public bool IsExecuting => _isExecuting;

        public AsyncRelayCommand(Action<object> toExecute, Predicate<object> canExecute = null)
            : base(toExecute, canExecute)
        {
        }

        public override bool CanExecute(object parameter)
        {
            return base.CanExecute(parameter) && !_isExecuting;
        }

        public override void Execute(object parameter)
        {
            try
            {
                _isExecuting = true;
                Started?.Invoke(this, EventArgs.Empty);

                var task = Task.Factory.StartNew(() =>
                {
                    ToExecute(parameter);
                });
                task.ContinueWith(t =>
                {
                    OnRunWorkerCompleted(EventArgs.Empty);
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch (Exception ex)
            {
                OnRunWorkerCompleted(new RunWorkerCompletedEventArgs(null, ex, true));
            }
        }

        private void OnRunWorkerCompleted(EventArgs e)
        {
            _isExecuting = false;
            Ended?.Invoke(this, e);
            RaiseCanExecuteChanged();
        }
    }
}