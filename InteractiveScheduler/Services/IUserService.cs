using System.Security;

namespace InteractiveScheduler.Services
{
    public interface IUserService
    {
        bool CredentialsValid(string username, SecureString password);
        void GrantBatchPermissions(string username);
        void OpenSecPol();
    }
}
