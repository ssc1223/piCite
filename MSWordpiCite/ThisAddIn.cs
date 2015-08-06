using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Windows;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO;
using Microsoft.Office.Tools;
using Microsoft.Office.Tools.Word;
using Microsoft.Office.Tools.Word.Extensions;
using Word = Microsoft.Office.Interop.Word;
using Office = Microsoft.Office.Core;
using MSWordpiCite.Entities;
using MSWordpiCite.Controls;
using MSWordpiCite.Classes;
using MSWordpiCite.Forms;
using MSWordpiCite.Formatter;
using MSWordpiCite.Tools;

namespace MSWordpiCite
{
    public partial class ThisAddIn
    {
        #region Variables

        private CustomTaskPane myCustomTaskPane;
        private Office.MsoCTPDockPosition DockPosition;
        public Logger log;
        public int iPaneMaxWidth;
        public int iPaneMaxHeight;
        public int iTempWidth;
        public int iTempHeight;
        public User user;
        public event EventHandler DockPositionChanged;
        private bool bLoading = false;
        private string _panetitle;
        public bool DebugMode = false;

        #endregion

        #region Public Functions

        // Show or Hide the CustomPane (If never loaded before: load the resource)
        public void StartpiCitePane()
        {
            if (bLoading)
                return;

            bLoading = true;
            if (myCustomTaskPane != null)
            {
                try
                {
                    this.CustomTaskPanes.Remove(myCustomTaskPane);
                    myCustomTaskPane = null;
                }
                catch { }
            }
            myCustomTaskPane = this.CustomTaskPanes.Add(new System.Windows.Forms.UserControl(), _panetitle);
            myCustomTaskPane.DockPosition = Office.MsoCTPDockPosition.msoCTPDockPositionBottom;
            myCustomTaskPane.Height = 5;
            iPaneMaxWidth = myCustomTaskPane.Width;
            myCustomTaskPane.DockPosition = Office.MsoCTPDockPosition.msoCTPDockPositionRight;
            myCustomTaskPane.Width = 5;
            iPaneMaxHeight = myCustomTaskPane.Height;
            this.CustomTaskPanes.Remove(myCustomTaskPane);
            ShowLoginControl();
            ShowCustomPanel();
            bLoading = false;
        }        
        // Display login control in CustomPane
        public void ShowLoginControl()
        {
            LoginControl loginControl = new LoginControl();
            try
            {
                this.CustomTaskPanes.Remove(myCustomTaskPane);
            }
            catch { }
            myCustomTaskPane = this.CustomTaskPanes.Add(loginControl, _panetitle);
            myCustomTaskPane.DockPosition = Office.MsoCTPDockPosition.msoCTPDockPositionFloating;
            myCustomTaskPane.Width = Convert.ToInt32(Properties.Settings.Default.CUSTOMPANE_MINIMUM_WIDTH);
            myCustomTaskPane.Height = Convert.ToInt32(0.7 * iPaneMaxHeight);
            preparePaneUI();
            if (DockPosition == Office.MsoCTPDockPosition.msoCTPDockPositionBottom || DockPosition == Office.MsoCTPDockPosition.msoCTPDockPositionTop)
                loginControl.Height = myCustomTaskPane.Height;
            else if (DockPosition == Office.MsoCTPDockPosition.msoCTPDockPositionRight || DockPosition == Office.MsoCTPDockPosition.msoCTPDockPositionLeft)
                loginControl.Width = myCustomTaskPane.Width;            
        }
        // Display Master control in CustomPane
        public void ShowMasterControl()
        {
            MasterControl masterControl = new MasterControl();
            try
            {
                this.CustomTaskPanes.Remove(myCustomTaskPane);
            }
            catch { }
            myCustomTaskPane = this.CustomTaskPanes.Add(masterControl, _panetitle);
            myCustomTaskPane.DockPosition = Office.MsoCTPDockPosition.msoCTPDockPositionFloating;
            myCustomTaskPane.Width = Convert.ToInt32(Properties.Settings.Default.CUSTOMPANE_MINIMUM_WIDTH);
            myCustomTaskPane.Height = Convert.ToInt32(0.7 * iPaneMaxHeight);
            preparePaneUI();            
        }        
        // Display Citation style chooser Window
        public void ShowStyleChooserForm(DocumentFormatter formatter)
        {
            CitationStyleWindow styleWindow = new CitationStyleWindow(formatter);
            styleWindow.ShowDialog();
        }
        // Change Visible property of current CustomPane
        public void ShowCustomPanel()
        {
            myCustomTaskPane.Visible = true;
        }
        // Delete Stored authentication string
        public void Logout()
        {
            Properties.Settings.Default.USER_REMEMBERME = false;
            Properties.Settings.Default.USER_AUTHENTICATION = string.Empty;
            Properties.Settings.Default.Save();
            user = null;
            ShowLoginControl();
            ShowCustomPanel();
        }
        // Remember logged-in email & authentication string in this comp
        public void RememberMe()
        {
            Properties.Settings.Default.USER_REMEMBERME = true;
            Properties.Settings.Default.USER_AUTHENTICATION = user.Authentication;
        }
        // Return current Custompane width
        public int GetCurrentCustomPaneWidth()
        {
            return myCustomTaskPane.Width;
        }
        // Return current Custompane height
        public int GetCurrentCustomPaneHeight()
        {
            return myCustomTaskPane.Height;
        }
        // Get current Custompane Dock position: Top, Bottom, Left, Right or Floating
        public Office.MsoCTPDockPosition GetCurrentDockPosition()
        {
            return DockPosition;
        }
        // Return true when Dock Repositioning Finished - by checking Width, Height
        public bool DockRepositionFinished()
        {   
            bool bRet = true;
            if(this.CustomTaskPanes.Count > 0)
            {
                if (myCustomTaskPane.DockPosition == Office.MsoCTPDockPosition.msoCTPDockPositionBottom || myCustomTaskPane.DockPosition == Office.MsoCTPDockPosition.msoCTPDockPositionTop)                
                    bRet = myCustomTaskPane.Width == iPaneMaxWidth;
                else if (myCustomTaskPane.DockPosition == Office.MsoCTPDockPosition.msoCTPDockPositionLeft || myCustomTaskPane.DockPosition == Office.MsoCTPDockPosition.msoCTPDockPositionRight)
                    bRet = myCustomTaskPane.Height == iPaneMaxHeight;
                else
                    bRet = true;
            }
            return bRet;
        }
        // Reset minimize Width after Dock Repositioning
        public void ResetPaneWidth(int iWidth)
        {
            if (this.CustomTaskPanes.Count > 0)
                if (myCustomTaskPane.Width < iWidth)
                    myCustomTaskPane.Width = iWidth + 5;
        }
        // Get Item data from current Master Control
        public bool GetItem(ref ItemMasterRow item, int ItemID)
        {
            try
            {
                return ((MasterControl)this.CustomTaskPanes[0].Control).GetItem(ref item, ItemID);
            }
            catch { }
            return false;
        }
        
        #endregion

        #region Event Handlers

        private void myCustomTaskPane_DockPositionChanged(object sender, EventArgs e)
        {
            DockPosition = myCustomTaskPane.DockPosition;
            Properties.Settings.Default.DOCK_POSITION = (int)DockPosition;
            DockPositionChanged(null, null);            
        }
        private void Application_WindowSize(Microsoft.Office.Interop.Word.Document doc, Microsoft.Office.Interop.Word.Window win)
        {
            CustomTaskPane taskPane = this.CustomTaskPanes.Add(new System.Windows.Forms.UserControl(), "temp");
            taskPane.DockPosition = Office.MsoCTPDockPosition.msoCTPDockPositionBottom;
            taskPane.Height = 5;
            iPaneMaxWidth = taskPane.Width;
            taskPane.DockPosition = Office.MsoCTPDockPosition.msoCTPDockPositionRight;
            taskPane.Width = 5;
            iPaneMaxHeight = taskPane.Height;
            this.CustomTaskPanes.Remove(taskPane);
        }
        
        // Start-up process (started right after Word is started)
        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            try
            {
                // For the 1st time running
                onetimeRunning();
                // Prepare shortcut keys
                //prepareShortcutKeys();
                // Prepare other application settings
                prepareAppSettings();
            }
            catch
            {}
        }
        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
            // Save the last configuration
            Properties.Settings.Default.CUSTOMPANE_WIDTH_DEFAULT = iTempWidth;
            Properties.Settings.Default.CUSTOMPANE_HEIGHT_DEFAULT = iTempHeight;
            Properties.Settings.Default.Save();
        }
               
        #endregion

        #region Private Functions

        private void prepareShortcutKeys()
        {
            try
            {
                object okey = Word.WdKey.wdKeyW;
                Application.KeyBindings.Add(Word.WdKeyCategory.wdKeyCategoryCommand, "piCite_InsertCitation", Application.BuildKeyCode(Word.WdKey.wdKeyControl, ref okey, ref missing, ref missing), ref missing, ref missing);
                okey = Word.WdKey.wdKey1;
                Application.KeyBindings.Add(Word.WdKeyCategory.wdKeyCategoryCommand, "piCite_InsertCitation", Application.BuildKeyCode(Word.WdKey.wdKeyControl, ref okey, ref missing, ref missing), ref missing, ref missing);
                okey = Word.WdKey.wdKey2;
                Application.KeyBindings.Add(Word.WdKeyCategory.wdKeyCategoryCommand, "piCite_ChangeStyle", Application.BuildKeyCode(Word.WdKey.wdKeyControl, ref okey, ref missing, ref missing), ref missing, ref missing);
                okey = Word.WdKey.wdKey3;
                Application.KeyBindings.Add(Word.WdKeyCategory.wdKeyCategoryCommand, "piite_ReferenceList", Application.BuildKeyCode(Word.WdKey.wdKeyControl, ref okey, ref missing, ref missing), ref missing, ref missing);
                okey = Word.WdKey.wdKey4;
                Application.KeyBindings.Add(Word.WdKeyCategory.wdKeyCategoryCommand, "piCite_FindReference", Application.BuildKeyCode(Word.WdKey.wdKeyControl, ref okey, ref missing, ref missing), ref missing, ref missing);
            }
            catch{}            
        }
        private void onetimeRunning()
        {
            Properties.Settings.Default.DEFAULT_RESET = true;
            if (Properties.Settings.Default.DEFAULT_RESET)
            {
                try
                {
                    //string strLocalFileStoragePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + @"\WizFolio";
                    string strLocalFileStoragePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + @"\πFolio";
                    if (!System.IO.Directory.Exists(strLocalFileStoragePath))
                    {
                        System.IO.Directory.CreateDirectory(strLocalFileStoragePath);
                        Properties.Settings.Default.DEFAULT_FILES_FOLDER = strLocalFileStoragePath;
                        Properties.Settings.Default.Save();
                    }
                    //string strLogFilePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + @"\WizFolio\log";
                    string strLogFilePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + @"\πFolio\log";
                    if (!System.IO.Directory.Exists(strLogFilePath))
                    {
                        System.IO.Directory.CreateDirectory(strLogFilePath);
                    }
                    //Templates資料夾
                    //string strTemplateFilePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + @"\WizFolio\Templates";
                    string strTemplateFilePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + @"\πFolio\Templates";
                    if (!System.IO.Directory.Exists(strTemplateFilePath))
                    {
                        System.IO.Directory.CreateDirectory(strTemplateFilePath);
                    }

                    Properties.Settings.Default.DefaultLogFilePath = strLogFilePath + @"\error.log";
                    Properties.Settings.Default.DEFAULT_RESET = false;
                    Properties.Settings.Default.Save();
                }
                catch
                { }
            }
        }                
        private void preparePaneUI()
        {
            myCustomTaskPane.DockPosition = DockPosition;
            if (DockPosition == Office.MsoCTPDockPosition.msoCTPDockPositionLeft || DockPosition == Office.MsoCTPDockPosition.msoCTPDockPositionRight || DockPosition == Office.MsoCTPDockPosition.msoCTPDockPositionFloating)
                myCustomTaskPane.Width = iTempWidth < Properties.Settings.Default.CUSTOMPANE_MINIMUM_WIDTH ? Properties.Settings.Default.CUSTOMPANE_MINIMUM_WIDTH : iTempWidth;            
            if (DockPosition == Office.MsoCTPDockPosition.msoCTPDockPositionTop || DockPosition == Office.MsoCTPDockPosition.msoCTPDockPositionBottom || DockPosition == Office.MsoCTPDockPosition.msoCTPDockPositionFloating)
                myCustomTaskPane.Height = iTempHeight;
            myCustomTaskPane.DockPositionRestrict = Office.MsoCTPDockPositionRestrict.msoCTPDockPositionRestrictNone;
            myCustomTaskPane.DockPositionChanged += new EventHandler(myCustomTaskPane_DockPositionChanged);
        }
        private void prepareAppSettings()
        {
            // Set Docking position of custom task pane
            DockPosition = (Office.MsoCTPDockPosition)Properties.Settings.Default.DOCK_POSITION;
            // Set the Width and Height
            iTempHeight = Properties.Settings.Default.CUSTOMPANE_HEIGHT_DEFAULT;
            iTempWidth = Properties.Settings.Default.CUSTOMPANE_WIDTH_DEFAULT;
            // Create new OR open the old logger
            log = new Logger("piCite", Properties.Settings.Default.DefaultLogFilePath, true);

            _panetitle = string.Format(Properties.Settings.Default.CUSTOMPANE_TITLE, Properties.Settings.Default.VERSION);
            // Attach handler for event Window Resizing
            this.Application.WindowSize += new Microsoft.Office.Interop.Word.ApplicationEvents4_WindowSizeEventHandler(this.Application_WindowSize);
        }       
        
        #endregion

        #region VSTO generated code

        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }
        
        #endregion
    }
}
