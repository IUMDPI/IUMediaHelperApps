using System.Drawing;
using System.Windows.Forms;
using FastColoredTextBoxNS;

namespace Packager.Observers
{
    public class TextBoxOutputObserver:IObserver
    {
        private readonly FastColoredTextBox _target;
        
        private delegate void LogCallback(string baseMessage, params object[] elements);

        public TextBoxOutputObserver(FastColoredTextBox target)
        {
            _target = target;
        }
        
        public void Log(string message, string exception)
        {
            
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
            _target.AppendText("\n");
        }

        public void LogHeader(string baseMessage, params object[] elements)
        {
            if (_target.InvokeRequired)
            {
                var d = new LogCallback(Log);
                _target.Parent.Invoke(d, baseMessage, elements);
                return;
            }


            //var startPosition = _target..TextLength;
            var message = string.Format(baseMessage, elements).ToUpperInvariant();
            _target.AppendText(message);
            _target.AppendText("\n\n");
            //BoldSelection(startPosition, message.Length);
            
        }

        public void LogError(string baseMessage, object[] elements)
        {
            if (_target.InvokeRequired)
            {
                var d = new LogCallback(Log);
                _target.Parent.Invoke(d, baseMessage, elements);
                return;
            }

            //va//startPosition = _target.TextLength;

            var message = (string.Format(baseMessage, elements));
            _target.TextChanged += new ColorTextEventHandler(Color.Red).Handler;
            _target.AppendText(message);
            _target.AppendText("\n\n");

            
            //ColorSelection(startPosition, message.Length, Color.DarkRed);
        }

        private void ColorSelection(int start, int length, Color color)
        {
            var start = new Place()
            
            //_target.Select(start, length);
            //_target.SelectionColor = color;
        }

        private void BoldSelection(int start, int length)
        {
            //_target.Select(start, length);
            //_target.SelectionFont = new Font(_target.Font, FontStyle.Bold);
        }

        private class ColorTextEventHandler
        {
            private readonly Color _color;
            private bool _handled;

            public ColorTextEventHandler(Color color)
            {
                _color = color;
            }

            public void Handler(object sender, TextChangedEventArgs e)
            {
                if (_handled)
                {
                    return;
                }

                e.ChangedRange.SetStyle(new TextStyle(new SolidBrush(_color), new SolidBrush(Color.White), FontStyle.Regular));
                _handled = true;
            }
        }
    }
}
