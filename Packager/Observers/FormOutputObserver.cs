using System.Windows.Forms;

namespace Packager.Observers
{
    public class TextBoxOutputObserver:IObserver
    {
        private readonly TextBox _target;


        private delegate void LogCallback(string baseMessage, params object[] elements);

        public TextBoxOutputObserver(TextBox target)
        {
            _target = target;
        }

        public void Log(string baseMessage, params object[] elements)
        {
            if (_target.InvokeRequired)
            {
                var d = new LogCallback(Log);
                _target.Parent.Invoke(d, baseMessage, elements);
                return;
            }

            _target.AppendText(string.Format(baseMessage, elements));
            _target.AppendText("\n\n");
        }

        public void LogHeader(string baseMessage, params object[] elements)
        {
            Log("");
            Log(string.Format(baseMessage, elements).ToUpperInvariant());
            Log("------------------------------------------------------------------------------------------");
        }
    }
}
