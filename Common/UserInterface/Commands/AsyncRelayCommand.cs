using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Common.UserInterface.Commands
{
    public class AsyncRelayCommand : RelayCommand
    {
        private bool _isExecuting;

        public event EventHandler Started;

        public event EventHandler Ended;

        public AsyncRelayCommand(Action<object> execute, Predicate<object> canExecute = null)
            : base(execute, canExecute)
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
