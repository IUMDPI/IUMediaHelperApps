using System.Threading.Tasks;
using Packager.Models.EmailMessageModels;

namespace Packager.Utilities
{
    public interface IEmailSender
    {
        void Send(AbstractEmailMessage message);
    }
}