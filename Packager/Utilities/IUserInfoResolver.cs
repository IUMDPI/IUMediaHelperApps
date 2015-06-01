using System;
using System.DirectoryServices.AccountManagement;
using Packager.Models;

namespace Packager.Utilities
{
    public interface IUserInfoResolver
    {
        UserInfo Resolve(string username);
    }

    internal class DomainUserResolver : IUserInfoResolver
    {
        public UserInfo Resolve(string username)
        {
            var context = new PrincipalContext(ContextType.Domain);
            var user = UserPrincipal.FindByIdentity(context, username);
            if (user == null)
            {
                throw new Exception(string.Format("Could not resolve file creator from username {0}", username));
            }

            return new UserInfo
            {
                Username = user.Name,
                DisplayName = string.Format("{0} {1}", user.GivenName, user.Surname),
                Email = user.EmailAddress
            };
        }
    }
}