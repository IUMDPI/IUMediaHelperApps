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
        private string Barcode { get; }
        
        public OpenObjectLogFileLinkText(string barcode, VisualLine parentVisualLine) : base(parentVisualLine, barcode.Length)
        {
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
            if (!File.Exists(editorPath))
            {
                return;
            }

            var logFolder = Properties.Settings.Default.ReportFolder;
            if (!Directory.Exists(logFolder))
            {
                return;
            }

            foreach (var logFile in Directory.GetFiles(logFolder, $"*{Barcode}.log"))
            {
                OpenLogFile(editorPath, logFile);
            }
        }

        private static void OpenLogFile(string editorPath, string logPath)
        {
            var startInfo = new ProcessStartInfo(editorPath)
            {
                Arguments = logPath
            };

            Process.Start(startInfo);
        }

        private string GetEditorPath()
        {
            var notepadPlusPlusPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Notepad++", "Notepad++.exe");
            var notepadPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "Notepad.exe");

            return File.Exists(notepadPlusPlusPath)
                ? notepadPlusPlusPath
                : notepadPath;
        }
    }
}