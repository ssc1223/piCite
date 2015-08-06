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
    public partial class TemplateSelectionDialog : Form
    {
        public enum CustomDialogResult
        {
            Insert = 0,
            OpenNew = 1,
            Abort = 2,
        }

        public CustomDialogResult strBehavior;

        public TemplateSelectionDialog()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            strBehavior = CustomDialogResult.Abort;
        }

        public CustomDialogResult GetBehavior
        {
            get{ return strBehavior; }
        }
        #region ButtonClick
        private void btnInsert_Click(object sender, EventArgs e)
        {
            DialogResult dlgResult;
            dlgResult = MessageBox.Show("Insert template might change the format of your document, would you want to continue?", "Confirm Insert?",MessageBoxButtons.YesNo);
            if (dlgResult == DialogResult.Yes)
            {
                strBehavior = CustomDialogResult.Insert;//插入Template
                Close();
            }
            else if(dlgResult == DialogResult.No)
            {
                Close();
            }
        }

        private void btnOpenNew_Click(object sender, EventArgs e)
        {
            strBehavior = CustomDialogResult.OpenNew;//開啟Template於新檔
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            strBehavior = CustomDialogResult.Abort;//取消
            Close();
        }
        #endregion

    }
}
