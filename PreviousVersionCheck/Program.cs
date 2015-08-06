using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Win32;   
using System.Threading;

namespace PreviousVersionCheck
{
    static class Program
    {


        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            RegistryKey regKey = Registry.LocalMachine;
            regKey = regKey.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\WizCite");
            string strUninstaller = (string) regKey.GetValue("UninstallString");
            if (strUninstaller != null && strUninstaller.Length > 0)
            {
                DialogResult result = MessageBox.Show("There is a previous version of WizCite installed in this computer.\nDo you want to remove it and install the newer version of WizCite?", "WizCite Setup", MessageBoxButtons.YesNo);
                if(result == DialogResult.Yes)
                {
                    System.Diagnostics.ProcessStartInfo procFormsBuilderStartInfo = new System.Diagnostics.ProcessStartInfo();
                    procFormsBuilderStartInfo.FileName = strUninstaller;
                    procFormsBuilderStartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Maximized;
                    System.Diagnostics.Process procFormsBuilder = new System.Diagnostics.Process();
                    procFormsBuilder.StartInfo = procFormsBuilderStartInfo;
                    procFormsBuilder.Exited += new EventHandler(procFormsBuilder_Exited);
                    procFormsBuilder.Start();                 
                }
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        private static void procFormsBuilder_Exited(object sender, EventArgs e)
        {
            MessageBox.Show("Done");
            Application.Exit();
        }
    }
}
