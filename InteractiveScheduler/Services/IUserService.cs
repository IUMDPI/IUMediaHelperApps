using System.Security;
using System.Threading.Tasks;

namespace InteractiveScheduler.Services
{
    public interface IUserService
    {
        bool CredentialsValid(string username, SecureString password);
        Task GrantBatchPermissions(string username);
        void OpenSecPol();
    }
}
