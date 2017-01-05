using System;
using System.Runtime.InteropServices;
using System.Text;

namespace InteractiveScheduler.ManagedCode
{
    public static class LsaUtilities
    {
        // Import the LSA functions

        [DllImport("advapi32.dll", PreserveSig = true)]
        private static extern uint LsaOpenPolicy(
            ref LsaUnicodeString systemName,
            ref LsaObjectAttributes objectAttributes,
            uint desiredAccess,
            out IntPtr policyHandle
        );

        [DllImport("advapi32.dll", SetLastError = true, PreserveSig = true)]
        private static extern uint LsaAddAccountRights(
            IntPtr policyHandle,
            IntPtr accountSid,
            LsaUnicodeString[] userRights,
            uint countOfRights);

        [DllImport("advapi32")]
        private static extern void FreeSid(IntPtr pSid);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true, PreserveSig = true)]
        private static extern bool LookupAccountName(
            string lpSystemName, string lpAccountName,
            IntPtr psid,
            ref int cbsid,
            StringBuilder domainName, ref int cbdomainLength, ref int use);
        
        [DllImport("advapi32.dll")]
        private static extern long LsaClose(IntPtr objectHandle);

        [DllImport("kernel32.dll")]
        private static extern int GetLastError();

        [DllImport("advapi32.dll")]
        private static extern int LsaNtStatusToWinError(uint status);

        // define the structures

        [Flags]
        private enum LsaAccessPolicy : long
        {
            PolicyViewLocalInformation = 0x00000001L,
            PolicyViewAuditInformation = 0x00000002L,
            PolicyGetPrivateInformation = 0x00000004L,
            PolicyTrustAdmin = 0x00000008L,
            PolicyCreateAccount = 0x00000010L,
            PolicyCreateSecret = 0x00000020L,
            PolicyCreatePrivilege = 0x00000040L,
            PolicySetDefaultQuotaLimits = 0x00000080L,
            PolicySetAuditRequirements = 0x00000100L,
            PolicyAuditLogAdmin = 0x00000200L,
            PolicyServerAdmin = 0x00000400L,
            PolicyLookupNames = 0x00000800L,
            PolicyNotification = 0x00001000L
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct LsaObjectAttributes
        {
            public int Length;
            public IntPtr RootDirectory;
            public readonly LsaUnicodeString ObjectName;
            public uint Attributes;
            public IntPtr SecurityDescriptor;
            public IntPtr SecurityQualityOfService;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct LsaUnicodeString
        {
            public ushort Length;
            public ushort MaximumLength;
            public IntPtr Buffer;
        }

        /// 
        //Adds a privilege to an account

        /// Name of an account - "domain\account" or only "account"
        /// Name ofthe privilege
        /// The windows error code returned by LsaAddAccountRights
        public static int SetRight(string accountName, string privilegeName)
        {
            var winErrorCode = 0; //contains the last error

            //pointer an size for the SID
            var sid = IntPtr.Zero;
            var sidSize = 0;
            //StringBuilder and size for the domain name
            var domainName = new StringBuilder();
            var nameSize = 0;
            //account-type variable for lookup
            var accountType = 0;

            //get required buffer size
            LookupAccountName(string.Empty, accountName, sid, ref sidSize, domainName, ref nameSize, ref accountType);

            //allocate buffers
            domainName = new StringBuilder(nameSize);
            sid = Marshal.AllocHGlobal(sidSize);

            //lookup the SID for the account
            var result = LookupAccountName(string.Empty, accountName, sid, ref sidSize, domainName, ref nameSize,
                ref accountType);

            if (!result)
            {
                winErrorCode = GetLastError();
               
            }
            else
            {
                //initialize an empty unicode-string
                var systemName = new LsaUnicodeString();
                //combine all policies
                const uint access = (uint) (
                    LsaAccessPolicy.PolicyAuditLogAdmin |
                    LsaAccessPolicy.PolicyCreateAccount |
                    LsaAccessPolicy.PolicyCreatePrivilege |
                    LsaAccessPolicy.PolicyCreateSecret |
                    LsaAccessPolicy.PolicyGetPrivateInformation |
                    LsaAccessPolicy.PolicyLookupNames |
                    LsaAccessPolicy.PolicyNotification |
                    LsaAccessPolicy.PolicyServerAdmin |
                    LsaAccessPolicy.PolicySetAuditRequirements |
                    LsaAccessPolicy.PolicySetDefaultQuotaLimits |
                    LsaAccessPolicy.PolicyTrustAdmin |
                    LsaAccessPolicy.PolicyViewAuditInformation |
                    LsaAccessPolicy.PolicyViewLocalInformation
                );
                //initialize a pointer for the policy handle
                IntPtr policyHandle;

                //these attributes are not used, but LsaOpenPolicy wants them to exists
                var objectAttributes = new LsaObjectAttributes
                {
                    Length = 0,
                    RootDirectory = IntPtr.Zero,
                    Attributes = 0,
                    SecurityDescriptor = IntPtr.Zero,
                    SecurityQualityOfService = IntPtr.Zero
                };

                //get a policy handle
                var resultPolicy = LsaOpenPolicy(ref systemName, ref objectAttributes, access, out policyHandle);
                winErrorCode = LsaNtStatusToWinError(resultPolicy);

                if (winErrorCode == 0)
                {
                    //Now that we have the SID an the policy,
                    //we can add rights to the account.

                    //initialize an unicode-string for the privilege name
                    var userRights = new LsaUnicodeString[1];
                    userRights[0] = new LsaUnicodeString
                    {
                        Buffer = Marshal.StringToHGlobalUni(privilegeName),
                        Length = (ushort) (privilegeName.Length*UnicodeEncoding.CharSize),
                        MaximumLength = (ushort) ((privilegeName.Length + 1)*UnicodeEncoding.CharSize)
                    };

                    //add the right to the account
                    var res = LsaAddAccountRights(policyHandle, sid, userRights, 1);
                    winErrorCode = LsaNtStatusToWinError(res);

                    LsaClose(policyHandle);
                }
                FreeSid(sid);
            }

            return winErrorCode;
        }
    }
}
