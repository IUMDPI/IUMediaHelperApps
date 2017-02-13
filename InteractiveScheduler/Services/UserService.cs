using System.ComponentModel;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Reflection;
using System.Security;
using System.Threading.Tasks;
using InteractiveScheduler.Extensions;

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

        public async Task GrantBatchPermissions(string username)
        {
            var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var path = string.IsNullOrWhiteSpace(location)
                ? "SetLogonAsBatchRight.exe"
                : Path.Combine(location, "SetLogonAsBatchRight.exe");
            
            var startInfo = new ProcessStartInfo(path)
            {
                Arguments = username,
                Verb = "runas",
                UseShellExecute = true,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };

            using (var process = Process.Start(startInfo))
            {
                if (process == null)
                {
                    return;
                }

                await process.WaitForExitAsync();
                if (process.ExitCode != 0)
                {
                    throw new Win32Exception(process.ExitCode);    
                }
            }
        }
        
        public void OpenSecPol()
        {
            var startInfo = new ProcessStartInfo("mmc.exe", "secpol.msc");
            Process.Start(startInfo);
        }
    }
}