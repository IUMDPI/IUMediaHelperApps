using Packager.Models.EmailMessageModels;

namespace Packager.Utilities.Email
{
    public interface IEmailSender
    {
        void Send(AbstractEmailMessage message);
    }
}