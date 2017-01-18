using Common.UserInterface.Commands;

namespace Recorder.ViewModels
{
    public class IssueNotifyModel : AbstractNotifyModel
    {
        public IssueNotifyModel()
        {
            YesText = "Ok";
            YesCommand = new RelayCommand(param =>
            {
                ShowPanel = false;
                Question = string.Empty;
            });
        }


        public void Notify(string message)
        {
            FlashPanel = false;
            Question = message;
            ShowPanel = true;
            FlashPanel = true;
        }
    }
}