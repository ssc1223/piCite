using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MSWordpiCite.Forms
{    
    public partial class ProgressForm : Form
    {
        public ProgressForm(string title, string message, bool loading)
        {
            InitializeComponent();
            this.Text = title;
            this.lblMessage.Text = message;
            this.imgLoading.Visible = loading;
        }

        private void initializeUI()
        {
            this.Width = Properties.Settings.Default.DEFAULT_PROGRESS_WIDTH;
            this.Height = Properties.Settings.Default.DEFAULT_PROGRESS_HEIGHT;

            Graphics g = lblMessage.CreateGraphics();
            SizeF s = g.MeasureString(lblMessage.Text, lblMessage.Font);
            if (s.Width > this.Width - 100)
                this.Width = Convert.ToInt32(s.Width + 100);

            this.Location = new Point(Convert.ToInt32((SystemInformation.PrimaryMonitorSize.Width - this.Width) / 2), Convert.ToInt32((SystemInformation.PrimaryMonitorSize.Height - this.Height) / 2));
        }

        private void ProgressForm_Load(object sender, EventArgs e)
        {
            initializeUI();
        }
    }
}
