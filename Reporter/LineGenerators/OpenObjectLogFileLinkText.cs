using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.TextFormatting;
using ICSharpCode.AvalonEdit.Rendering;

namespace Reporter.LineGenerators
{
    public class OpenObjectLogFileLinkText : VisualLineText
    {
        private string ReportFolder { get; }
        private string Barcode { get; }
        
        public OpenObjectLogFileLinkText(string reportFolder, string barcode, VisualLine parentVisualLine) : base(parentVisualLine, barcode.Length)
        {
            ReportFolder = reportFolder;
            Barcode = barcode;
        }

        protected override void OnQueryCursor(QueryCursorEventArgs e)
        {
            e.Cursor = Cursors.Hand;
            e.Handled = true;
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            OpenLogFiles();
            e.Handled = true;
        }
        
        public override TextRun CreateTextRun(int startVisualColumn, ITextRunConstructionContext context)
        {
            TextRunProperties.SetForegroundBrush(context.TextView.LinkTextForegroundBrush);
            TextRunProperties.SetBackgroundBrush(context.TextView.LinkTextBackgroundBrush);
            TextRunProperties.SetTextDecorations(TextDecorations.Underline);
            
            return base.CreateTextRun(startVisualColumn, context);
        }

        private void OpenLogFiles()
        {
            var editorPath = GetEditorPath();
            
            if (!Directory.Exists(ReportFolder))
            {
                return;
            }

            foreach (var logFile in Directory.GetFiles(ReportFolder, $"*{Barcode}.log"))
            {
                OpenLogFile(editorPath, logFile);
            }
        }

        private static void OpenLogFile(string editorPath, string logPath)
        {
            try
            {
                var startInfo = new ProcessStartInfo(editorPath)
                {
                    Arguments = logPath
                };

                Process.Start(startInfo);
            }
            catch
            {
                // ignore
            }
        }

        private static string GetEditorPath()
        {
            var notepadPlusPlusPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), 
                "Notepad++", "Notepad++.exea");

            return File.Exists(notepadPlusPlusPath)
                ? notepadPlusPlusPath
                : "Notepad.exe";
        }
    }
}