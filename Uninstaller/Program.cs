using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Uninstaller
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            public partial class App : Application
{
  void Application_Startup(object sender,StartupEventArgs e)
  {
    for(int i = 0;i != e.Args.Length;++i)
    {
      if(e.Args[i].Split('=')[0].ToLower() == "/u")
      {
        string guid = e.Args[i].Split('=')[1];
        string path = Environment.GetFolderPath(Environment.SpecialFolder.System);
        ProcessStartInfo uninstallProcess = new ProcessStartInfo(path+"\\msiexec.exe","/x "+guid);
        Process.Start(uninstallProcess);
        System.Windows.Application.Current.Shutdown();
      }
    }
  }
}

        }
    }
}
