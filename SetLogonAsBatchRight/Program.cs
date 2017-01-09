using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SetLogonAsBatchRight
{
    internal static class Program
    {
        public static int Main(string[] args)
        {
            var assembly = Assembly.GetExecutingAssembly();
            WriteHeader(assembly);
            if (ShowHelp(assembly, args))
            {
                return 160; // bad arguments
            }

            Console.WriteLine("");
            Console.WriteLine("Granting logon-as-batch permission to account {0}.", args[0]);
            Console.WriteLine("");
            var result = LsaUtilities.SetRight(args[0], "SeBatchLogonRight");
            if (result == 0)
            {
                Console.WriteLine("{0} successfully granted logon-as-batch permissions.", args[0]);
            }
            else
            {
                var issue = new Win32Exception(result);
                Console.WriteLine("Could not grant logon-as-batch permissions to {0}.", args[0]);
                Console.WriteLine("Issue: {0} ({1}).", issue.Message, issue.NativeErrorCode);
            }

            return result;
        }

        private static void WriteHeader(Assembly assembly)
        {
            Console.WriteLine("");
            Console.WriteLine("Logon-As-Batch Rights Helper ({0})", assembly.GetName().Version);
            Console.WriteLine("---------------------------------------");
        }

        private static bool ShowHelp(Assembly assembly, string[] args)
        {
            if (args.Any())
            {
                return false;
            }

            Console.WriteLine("");
            Console.WriteLine("Grants logon-as-batch permissions to a specified account");
            Console.WriteLine("");
            Console.WriteLine("---------------------------------------");
            Console.WriteLine("Usage:");
            Console.WriteLine("");
            
            Console.WriteLine("  {0} [Account Name]", Path.GetFileNameWithoutExtension(assembly.Location));
            Console.WriteLine("");
            Console.WriteLine("  For example: {0} ads\\dvader", Path.GetFileNameWithoutExtension(assembly.Location));

            return true;
        }
    }
}
