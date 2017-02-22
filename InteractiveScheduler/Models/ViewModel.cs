using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.DirectoryServices;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security;
using System.Security.Principal;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Common.TaskScheduler.Configurations;
using Common.TaskScheduler.Factory;
using Common.TaskScheduler.Schedulers;
using Common.UserInterface.Commands;
using InteractiveScheduler.ManagedCode;
using InteractiveScheduler.Services;
using Microsoft.Win32.TaskScheduler;

namespace InteractiveScheduler.Models
{
    public class ViewModel : INotifyPropertyChanged, IDisposable
    {
        private const string DefaultPackagerPath = @"c:\dependencies\packager\packager.exe";
        private readonly IFileDialogService _fileDialogService;
        private readonly IUserService _userService;
        private readonly ITaskSchedulerFactory _taskSchedulerFactory;
        public event PropertyChangedEventHandler PropertyChanged;

        private bool _scheduleDaily;
        private string _taskName;
        private string _packagerPath;
        private DateTime _startOn;
        private string _username;

        private bool _mondayChecked;
        private bool _tuesdayChecked;
        private bool _wednesdayChecked;
        private bool _thursdayChecked;
        private bool _fridayChecked;
        private bool _saturdayChecked;
        private bool _sundayChecked;
        private bool _impersonate;
        private bool _showMessage;
        private string _message;
        private string _messageTitle;
        private bool _taskExists;
        private bool _taskRunning;
        private bool _flashMessage;

        private ICommand _browseForApplicationCommand;
        private ICommand _scheduleTaskCommand;
        private ICommand _hideMessageCommand;
        private ICommand _closeWindowCommand;
        private ICommand _removeTaskCommand;

        private ICommand _stopTaskCommand;
        private ICommand _openSchedulerCommand;
        private bool _taskEnabled;
        private bool _taskDisabled;
        private ICommand _enableTaskCommand;
        private ICommand _disableTaskCommand;
        private bool _advancedMenuOpen;
        private ICommand _openPolicyCommand;
        private BitmapSource _uacShield;

        public ViewModel(IFileDialogService fileDialogService, IUserService userService, ITaskSchedulerFactory taskSchedulerFactory)
        {
            _fileDialogService = fileDialogService;
            _userService = userService;
            _taskSchedulerFactory = taskSchedulerFactory;

            Username = WindowsIdentity.GetCurrent().Name;

            DoInitialConfiguration();
        }

        private void DoInitialConfiguration()
        {
            var packagerPath = _fileDialogService.Exists(DefaultPackagerPath)
                ? DefaultPackagerPath
                : string.Empty;

            var scheduler = GetSchedulerForApplication(packagerPath);

            PackagerPath = packagerPath;
            ImportSchedulerDefaults(scheduler);
            ImportFromTask(scheduler.FindExisting());
        }

        public string TaskName
        {
            get { return _taskName; }
            set
            {
                _taskName = value;
                OnPropertyChanged();
            }
        }

        public string PackagerPath
        {
            get { return _packagerPath; }
            set
            {
                _packagerPath = value;
                OnPropertyChanged();
            }
        }

        public DateTime StartOn
        {
            get { return _startOn; }
            set
            {
                _startOn = value;
                OnPropertyChanged();
            }
        }

        public string Username
        {
            get { return _username; }
            set
            {
                _username = value;
                OnPropertyChanged();
            }
        }

        public bool MondayChecked
        {
            get { return _mondayChecked; }
            set
            {
                _mondayChecked = value;
                OnPropertyChanged();
            }
        }

        public bool TuesdayChecked
        {
            get { return _tuesdayChecked; }
            set
            {
                _tuesdayChecked = value;
                OnPropertyChanged();
            }
        }

        public bool WednesdayChecked
        {
            get { return _wednesdayChecked; }
            set
            {
                _wednesdayChecked = value;
                OnPropertyChanged();
            }
        }

        public bool ThursdayChecked
        {
            get { return _thursdayChecked; }
            set
            {
                _thursdayChecked = value;
                OnPropertyChanged();
            }
        }

        public bool FridayChecked
        {
            get { return _fridayChecked; }
            set
            {
                _fridayChecked = value;
                OnPropertyChanged();
            }
        }

        public bool SaturdayChecked
        {
            get { return _saturdayChecked; }
            set
            {
                _saturdayChecked = value;
                OnPropertyChanged();
            }
        }

        public bool SundayChecked
        {
            get { return _sundayChecked; }
            set
            {
                _sundayChecked = value;
                OnPropertyChanged();
            }
        }

        public bool Impersonate
        {
            get { return _impersonate; }
            set
            {
                _impersonate = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ShowUacShield));
            }
        }

        public bool ScheduleDaily
        {
            get { return _scheduleDaily;}
            set { _scheduleDaily = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ShowUacShield)); }
        }

        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                OnPropertyChanged();
            }
        }

        public string MessageTitle
        {
            get { return _messageTitle; }
            set
            {
                _messageTitle = value;
                OnPropertyChanged();
            }
        }

        public bool ToggleMessage
        {
            get { return _showMessage; }
            set
            {
                _showMessage = value;
                OnPropertyChanged();
            }
        }

        public bool TaskEnabled
        {
            get { return _taskEnabled; }
            set { _taskEnabled = value; OnPropertyChanged(); }
        }

        public bool TaskDisabled
        {
            get { return _taskDisabled; }
            set { _taskDisabled = value; OnPropertyChanged(); }
        }

        public bool TaskExists
        {
            get { return _taskExists; }
            set { _taskExists = value; OnPropertyChanged(); }
        }

        public bool AdvancedMenuOpen
        {
            get { return _advancedMenuOpen; }
            set { _advancedMenuOpen = value; OnPropertyChanged(); }
        }

        public bool FlashMessage
        {
            get { return _flashMessage; }
            set { _flashMessage = value; OnPropertyChanged(); }
        }

        public bool ShowUacShield => Impersonate && ScheduleDaily;

        private void OpenFileBrowserDialog()
        {
            var currentScheduler = _taskSchedulerFactory.GetForApplication(PackagerPath);
            var result = _fileDialogService.OpenDialog(DefaultPackagerPath);
            if (string.IsNullOrWhiteSpace(result))
            {
                return;
            }

            PackagerPath = result;
            var newScheduler = _taskSchedulerFactory.GetForApplication(result);
            if (newScheduler == null || newScheduler == currentScheduler)
            {
                return;
            }

            ImportSchedulerDefaults(newScheduler);
            ImportFromTask(newScheduler.FindExisting());
        }

        public ICommand BrowseForPackager
        {
            get
            {
                return _browseForApplicationCommand
                       ?? (_browseForApplicationCommand = new RelayCommand(param => OpenFileBrowserDialog()));
            }
        }

        public ICommand ScheduleTask
        {
            get
            {
                return _scheduleTaskCommand
                       ?? (_scheduleTaskCommand = new AsyncRelayCommand(async param => await DoScheduleTask()));
            }
        }

        public ICommand HideMessage
        {
            get
            {
                return _hideMessageCommand
                       ?? (_hideMessageCommand = new RelayCommand(
                           param => { ToggleMessage = false; },
                           param => ToggleMessage));
            }
        }

        public ICommand CloseWindow
        {
            get
            {
                return _closeWindowCommand ?? (_closeWindowCommand = new RelayCommand(
                           param => { Application.Current.Shutdown(); }));
            }
        }

        public ICommand RemoveTask
        {
            get
            {
                return _removeTaskCommand ?? (_removeTaskCommand = new RelayCommand(
                           param => DoRemoveTask(),
                           param => TaskExists));
            }
        }

        public ICommand OpenPolicy
        {
            get
            {
                return _openPolicyCommand ?? (_openPolicyCommand = new RelayCommand(
                           param => DoOpenPolicy()));
            }
        }

        private void DoOpenPolicy()
        {
            try
            {
                _userService.OpenSecPol();
            }
            catch (Exception e)
            {
                ShowMessage("Could not open Local Security Policy editor", e.Message);
            }
        }

        public ICommand OpenScheduler
        {
            get
            {
                return _openSchedulerCommand ?? (_openSchedulerCommand = new RelayCommand(
                           param => _taskSchedulerFactory
                                .GetDefaultScheduler()
                                .OpenWindowsTaskScheduler()));
            }
        }

        public ICommand StopTask
        {
            get
            {
                return _stopTaskCommand ?? (_stopTaskCommand = new RelayCommand(
                           param => DoStopTask(),
                           param => TaskRunning & TaskExists));
            }
        }

        public ICommand EnableTask
        {
            get
            {
                return _enableTaskCommand ?? (_enableTaskCommand = new RelayCommand(
                           param => DoEnableTask(true),
                           param => TaskDisabled & TaskExists));
            }
        }

        public ICommand DisableTask
        {
            get
            {
                return _disableTaskCommand ?? (_disableTaskCommand = new RelayCommand(
                           param => DoEnableTask(false),
                           param => TaskEnabled & TaskExists));
            }
        }

        public BitmapSource UacShield => _uacShield ?? (_uacShield = UserInterfaceHelper.GetUserAccessControlShield());

        public bool TaskRunning
        {
            get { return _taskRunning; }
            set { _taskRunning = value; OnPropertyChanged(); }
        }

        private void DoRemoveTask()
        {
            var scheduler = GetSchedulerForApplication(PackagerPath);
            scheduler.Remove(TaskName);
            ImportFromTask(scheduler.FindExisting());
        }

        private void DoStopTask()
        {
            var scheduler = GetSchedulerForApplication(PackagerPath);
            scheduler.Stop(TaskName);
            ImportFromTask(scheduler.FindExisting());
        }

        private void DoEnableTask(bool enable)
        {
            var scheduler = GetSchedulerForApplication(PackagerPath);
            scheduler.Enable(TaskName, enable);
            ImportFromTask(scheduler.FindExisting());
        }

        private async System.Threading.Tasks.Task DoScheduleTask()
        {
            if (await TryGrantPermission(Username) == false)
            {
                return;
            }

            TryScheduleTask();
        }

        private async System.Threading.Tasks.Task<bool> TryGrantPermission(string username)
        {
            try
            {
                if (!Impersonate || !ScheduleDaily)
                {
                    return true;
                }

                await _userService.GrantBatchPermissions(username);
                return true;
            }
            catch (Exception e)
            {
                ShowMessage("Could not grant user permissions", e.Message);
                return false;
            }
        }

        private void TryScheduleTask()
        {
            try
            {
                var scheduler = GetSchedulerForApplication(PackagerPath);
                var configuration = GetConfiguration();
                var result = scheduler.Schedule(configuration);
                if (result.Item1 == false)
                {
                    ShowMessage("Could not schedule task", result.Item2.First());
                }
                else
                {
                    ShowMessage("Success!", "Task successfully scheduled!");
                }
                
                ImportFromTask(scheduler.FindExisting());
            }
            catch (Exception e)
            {
                ShowMessage("Could not schedule task", e.Message);
            }
        }

        private AbstractConfiguration GetConfiguration()
        {
            if (ScheduleDaily == false)
            {
                return new StartOnLogonConfiguration
                {
                    ExecutablePath = PackagerPath,
                    TaskName = TaskName
                };
            }

            if (Impersonate == false)
            {
                return new InteractiveDailyConfiguration
                {
                    ExecutablePath = PackagerPath,
                    TaskName = TaskName,
                    StartOn = StartOn,
                    Days = CalculateDays()
                };
            }

            return new NonInteractiveDailyConfiguration
            {
                ExecutablePath = PackagerPath,
                TaskName = TaskName,
                StartOn = StartOn,
                Days = CalculateDays(),
                Username = Username,
                Passphrase = Password
            };
        }

        public SecureString Password { private get; set; }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Dispose()
        {
            Password.Dispose();
        }

        private void ShowMessage(string title, string message)
        {
            if (ToggleMessage)
            {
                FlashMessage = true;
            }
            MessageTitle = title;
            Message = message;
            ToggleMessage = true;
        }

        private DaysOfTheWeek CalculateDays()
        {
            DaysOfTheWeek result = 0;
            if (MondayChecked)
            {
                result = result | DaysOfTheWeek.Monday;
            }

            if (TuesdayChecked)
            {
                result = result | DaysOfTheWeek.Tuesday;
            }

            if (WednesdayChecked)
            {
                result = result | DaysOfTheWeek.Wednesday;
            }

            if (ThursdayChecked)
            {
                result = result | DaysOfTheWeek.Thursday;
            }

            if (FridayChecked)
            {
                result = result | DaysOfTheWeek.Friday;
            }

            if (SaturdayChecked)
            {
                result = result | DaysOfTheWeek.Saturday;
            }

            if (SundayChecked)
            {
                result = result | DaysOfTheWeek.Sunday;
            }

            return result;
        }

        private ITaskScheduler GetSchedulerForApplication(string path)
        {
            return _taskSchedulerFactory.GetForApplication(path) != null
                ? _taskSchedulerFactory.GetForApplication(path)
                : _taskSchedulerFactory.GetDefaultScheduler();
        }

        private void ImportSchedulerDefaults(ITaskScheduler scheduler)
        {
            var config = scheduler.GetDefaultConfiguration() as InteractiveDailyConfiguration;

            if (config == null)
            {
                ScheduleDaily = false;
                return;
            }

            ScheduleDaily = true;
            SetDaysFromEnum(config.Days);
            StartOn = config.StartOn;
            TaskName = config.TaskName;
            
            Impersonate = config is NonInteractiveDailyConfiguration;
        }

        private void ImportFromTask(Task task)
        {
            if (task == null)
            {
                TaskExists = false;
                TaskDisabled = false;
                TaskEnabled = false;
                TaskRunning = false;
                return;
            }

            TaskExists = true;
            TaskName = task.Name;
            TaskRunning = task.State == TaskState.Running;
            TaskDisabled = task.Enabled == false;
            TaskEnabled = task.Enabled;
            AdvancedMenuOpen = false;

            Impersonate = task.Definition.Principal.LogonType == TaskLogonType.Password;

            var trigger = task.Definition.Triggers.FirstOrDefault(tr => tr.TriggerType == TaskTriggerType.Weekly) as WeeklyTrigger;
            if (trigger != null)
            {
                StartOn = trigger.StartBoundary;
                SetDaysFromEnum(trigger.DaysOfWeek);
            }
        }

        private void SetDaysFromEnum(DaysOfTheWeek value)
        {
            MondayChecked = DayChecked(value, DaysOfTheWeek.Monday);
            TuesdayChecked = DayChecked(value, DaysOfTheWeek.Tuesday);
            WednesdayChecked = DayChecked(value, DaysOfTheWeek.Wednesday);
            ThursdayChecked = DayChecked(value, DaysOfTheWeek.Thursday);
            FridayChecked = DayChecked(value, DaysOfTheWeek.Friday);
            SaturdayChecked = DayChecked(value, DaysOfTheWeek.Saturday);
            SundayChecked = DayChecked(value, DaysOfTheWeek.Sunday);
        }

        private static bool DayChecked(DaysOfTheWeek value, DaysOfTheWeek dayValue)
        {
            return (value & dayValue) == dayValue;
        }
    }
}
