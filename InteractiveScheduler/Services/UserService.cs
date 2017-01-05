using System.ComponentModel;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
using System.Security;
using InteractiveScheduler.Extensions;
using InteractiveScheduler.ManagedCode;

namespace InteractiveScheduler.Services
{
    public class UserService : IUserService
    {
        public bool CredentialsValid(string username, SecureString password)
        {
            if (password == null || password.Length == 0)
            {
                return false;
            }

            using (var context = new PrincipalContext(ContextType.Domain))
            {
                return context.ValidateCredentials(username, password.ToUnsecureString());
            }
        }

        public void GrantBatchPermissions(string username)
        {
            var result = LsaUtilities.SetRight(username, "SeBatchLogonRight");
            if (result != 0)
            {
                throw new Win32Exception(result);
            }
        }

        public void OpenSecPol()
        {
            var startInfo = new ProcessStartInfo("mmc.exe", "secpol.msc");
            Process.Start(startInfo);
        }
    }
}