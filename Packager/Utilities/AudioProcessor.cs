using System;
using System.Diagnostics;
using System.Drawing.Text;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Packager.Models;

namespace Packager.Utilities
{
    public class AudioProcessor : IProcessor
    {
        private readonly string _bwfMetaEditPath;
        private readonly string _ffmpegPath;
        private readonly string _ffmpegAudioMezzanineArguments;
        private readonly string _ffmpegAudioAccessArguments;

        public AudioProcessor(string ffmpegPath, string bwfMetaEditPath, string ffmpegAudioMezzanineArguments, string ffmpegAudioAccessArguments)
        {
            _ffmpegPath = ffmpegPath;
            _bwfMetaEditPath = bwfMetaEditPath;
            _ffmpegAudioMezzanineArguments = ffmpegAudioMezzanineArguments;
            _ffmpegAudioAccessArguments = ffmpegAudioAccessArguments;
        }

        public void ProcessFile(string targetPath)
        {
            const string standinDescription =
               "Indiana University, Bloomington. William and Gayle Cook Music Library. TP-S .A1828 81-4-17 v. 1. File use:";

            const string standinIARL =
                "Indiana University, Bloomington. William and Gayle Cook Music Library. TP-S .A1828 81-4-17 v. 1. File use:";

            const string standinICMT = "Indiana University, Bloomington. William and Gayle Cook Music Library.";
            
            var data = new BextData
            {
                Description = standinDescription,
                IARL = standinIARL,
                ICMT = standinICMT
            };

            AddMetadata(targetPath,data);
            CreateMezzanine(targetPath, _ffmpegAudioMezzanineArguments);
            CreateAccess(targetPath, _ffmpegAudioAccessArguments);
        }
        
        private void CreateMezzanine(string inputPath, string commandLineArgs)
        {
            var outputPath = Path.Combine(Path.GetDirectoryName(inputPath), Path.GetFileNameWithoutExtension(inputPath) + ".aac");
            var args = string.Format("-i {0} {1} {2}", inputPath, commandLineArgs, outputPath);

            CreateDerivative(args);
        }

        private void CreateAccess(string inputPath, string commandLineArgs)
        {
            var outputPath = Path.Combine(Path.GetDirectoryName(inputPath), Path.GetFileNameWithoutExtension(inputPath) + ".mp4");
            var args = string.Format("-i {0} {1} {2}", inputPath, commandLineArgs, outputPath);

            CreateDerivative(args);
        }
        
        private void CreateDerivative(string args)
        {
            var startInfo = new ProcessStartInfo(_ffmpegPath)
            {
                Arguments = args,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false
            };

            var process = Process.Start(startInfo);
            if (process == null)
            {
                throw new Exception(string.Format("Could not start {0}", _bwfMetaEditPath));
            }

            do
            {
                Application.DoEvents();
                process.WaitForExit(5);
            } while (!process.HasExited);

            string output;
            using (var reader = process.StandardOutput)
            {
                output = reader.ReadToEnd();
            }

            if (process.ExitCode != 0)
            {
                throw new Exception(string.Format("Could not generate derivative: {0}", process.ExitCode));
            }
        }

        private void AddMetadata(string targetPath, BextData data)
        {
            var args = string.Format("--verbose --append {0} {1}", string.Join(" ", data.GenerateCommandArgs()), targetPath); 
            
            var startInfo = new ProcessStartInfo(_bwfMetaEditPath)
            {
                Arguments = args,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false
            };

            var process = Process.Start(startInfo);
            if (process == null)
            {
                throw new Exception(string.Format("Could not start {0}", _bwfMetaEditPath));
            }

            do
            {
                Application.DoEvents();
                process.WaitForExit(5);
            } while (!process.HasExited);


            string output;
            using (var reader = process.StandardOutput)
            {
                output = reader.ReadToEnd();
            }
            
            if (process.ExitCode != 0 || !SuccessMessagePresent(targetPath, output))
            {
                throw new Exception(string.Format("Could not insert BEXT Data: {0}",process.ExitCode));
            }
        }

        private static bool SuccessMessagePresent(string fileName, string output)
        {
            var success = string.Format("{0}: is modified", fileName).ToLowerInvariant();
            var nothingToDo = string.Format("{0}: nothing to do", fileName).ToLowerInvariant();
            return output.ToLowerInvariant().Contains(success) || output.ToLowerInvariant().Contains(nothingToDo);
        }
    }

    
}