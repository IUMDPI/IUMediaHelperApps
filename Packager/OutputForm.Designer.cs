namespace Packager
{
    partial class OutputForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.OutputBox = new FastColoredTextBoxNS.FastColoredTextBox();
            this.SuspendLayout();
            // 
            // OutputBox
            // 
            this.OutputBox.AutoScrollMinSize = new System.Drawing.Size(0, 15);
            this.OutputBox.BackBrush = null;
            this.OutputBox.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.OutputBox.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.OutputBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.OutputBox.LeftPadding = 8;
            this.OutputBox.Location = new System.Drawing.Point(0, 0);
            this.OutputBox.Name = "OutputBox";
            this.OutputBox.Paddings = new System.Windows.Forms.Padding(0);
            this.OutputBox.ReadOnly = true;
            this.OutputBox.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.OutputBox.ShowLineNumbers = false;
            this.OutputBox.Size = new System.Drawing.Size(535, 390);
            this.OutputBox.TabIndex = 0;
            this.OutputBox.Text = "OutputBox";
            this.OutputBox.WordWrap = true;
            // 
            // OutputForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(535, 390);
            this.Controls.Add(this.OutputBox);
            this.Name = "OutputForm";
            this.Text = "Output";
            this.Load += new System.EventHandler(this.FormLoadHandler);
            this.ResumeLayout(false);

        }

        #endregion

        private FastColoredTextBoxNS.FastColoredTextBox OutputBox;


    }
}

