using System;
using System.Windows.Forms;
using Packager.Engine;
using Packager.Observers;

namespace Packager
{
    public partial class OutputForm : Form
    {
        private readonly IEngine _engine;

        public OutputForm(IEngine engine)
        {
            // needs to come first
            InitializeComponent();

            // assign engine to variable
            _engine = engine;
        }

        private async void FormLoadHandler(object sender, EventArgs e)
        {
           _engine.AddObserver(new TextBoxOutputObserver(OutputBox));
           OutputBox.Cursor = Cursors.WaitCursor;
           await _engine.Start();
           OutputBox.Cursor = Cursors.Default;
        }
    }
}