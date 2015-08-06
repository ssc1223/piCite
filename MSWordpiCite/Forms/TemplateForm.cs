using MSWordpiCite.Tools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace MSWordpiCite.Forms
{
    public partial class TemplateForm : Form, IMessageFilter
    {
        #region Variables
        private Logger log = Globals.ThisAddIn.log;
        private Cursor NoDropCursor;
        private Cursor NormalCursor;
        public string strOrgNode;
        private static object g_objPictureBoxLock = new object();
        #endregion
        //public static void AddMessageFilter(IMessageFilter value);
        /// <summary>
        /// Template開啟列表
        /// </summary>
        public TemplateForm()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;//視窗起始點為螢幕中央
            LoadTemplateFromSource();
            Application.AddMessageFilter(this);
        }

        private void LoadTemplateFromSource()
        {
            try
            {
                if (!System.IO.Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + Properties.Settings.Default.DEFAULT_TEMPLATE_FOLDER))
                {
                    System.IO.Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + Properties.Settings.Default.DEFAULT_TEMPLATE_FOLDER);
                }
                TextInfo txtInfo = new CultureInfo("en-US", false).TextInfo;//將每個單字字首轉為大寫
                WebClient WebClient = new WebClient();
                byte[] bWebClient = WebClient.DownloadData(Properties.Settings.Default.URL_TEMPLATE + "/all");//取得byte array避免中文亂碼問題
                string strJson = System.Text.Encoding.UTF8.GetString(bWebClient);//轉成string，準備Json的反序列化
                JObject restoredObject = JsonConvert.DeserializeObject<JObject>(strJson);

                string[] strDepartment = new string[restoredObject.Count];//網址取得的資料夾宣告

                TreeNode TreeNodeRoot = new TreeNode("Templates");//根節點名稱
                for (int i = 0; i < restoredObject.Count; i++)
                {
                    var q = from p in restoredObject.Properties() where p.Name == (i + 1).ToString() select p;
                    strDepartment[i] = q.First().Value.ToString();
                    //TreeNodeRoot.Nodes.Add(strDepartment[i]).Tag = restoredObject.Count;
                    TreeNodeRoot.Nodes.Add(txtInfo.ToTitleCase(strDepartment[i]));
                }
                treeTemplate.SelectedNode = TreeNodeRoot.Nodes[0];
                strOrgNode = TreeNodeRoot.Nodes[0].Text;
                treeTemplate.Nodes.Add(TreeNodeRoot);
                treeTemplate.ExpandAll();
            }
            catch (WebException ex)
            {
                log.WriteLine(LogType.Error, "TemplateForm::LoadTemplateFromSource", ex.ToString());
            }
        }

        /// <summary>
        /// 判斷滑鼠是否位於Word文件中
        /// </summary>
        private bool isInsideDocument()
        {
            bool b = false;
            if (this.pnlPreview.Focused || this.grpTemplatePreview.Focused || this.Focused)
                return b;
            Point p = Cursor.Position;
            int left = Globals.ThisAddIn.Application.ActiveWindow.Left;
            int top = Globals.ThisAddIn.Application.ActiveWindow.Top + 198;
            int width = Globals.ThisAddIn.Application.ActiveWindow.Width;
            int height = Globals.ThisAddIn.Application.ActiveWindow.Height;
            if (p.X > left && p.X < (left + width))
                if (p.Y > top && p.Y < (top + height))
                    b = true;
            return b;
        }

        #region Template Mouse Event
        /// <summary>
        /// 滑鼠點選右側Template事件
        /// </summary>
        private void TemplateForm_Click(object sender, EventArgs e)
        {
            try
            {
                string strFileType = "dotx";//檔案類型-將使用於以url取得檔案時
                PictureBox picTemplate = (PictureBox)sender;
                TemplateSelectionDialog tmpSelectionDialog = new TemplateSelectionDialog();//使用者可選擇插入及開啟新檔案之對話視窗

                tmpSelectionDialog.ShowDialog();
                if (tmpSelectionDialog.GetBehavior == TemplateSelectionDialog.CustomDialogResult.OpenNew)
                {
                    string strRequestTemplateURL = Properties.Settings.Default.URL_TEMPLATE
                                                    + treeTemplate.SelectedNode.Text
                                                    + "/"
                                                    + strFileType
                                                    + "/"
                                                    + picTemplate.Tag.ToString().Split('.')[0];
                    Globals.ThisAddIn.user.DownloadFile(picTemplate.Tag.ToString(), null, strRequestTemplateURL);
                }
                else if (tmpSelectionDialog.GetBehavior == TemplateSelectionDialog.CustomDialogResult.Insert)
                {
                    WebClient WebClient = new WebClient();
                    string strRequestTemplateURL = Properties.Settings.Default.URL_TEMPLATE
                                                    + treeTemplate.SelectedNode.Text
                                                    + "/dotx/"
                                                    + picTemplate.Tag.ToString().Split('.')[0];
                    string strTemplateFileLocation = Environment.GetFolderPath(Environment.SpecialFolder.Personal)
                                                    + Properties.Settings.Default.DEFAULT_TEMPLATE_FOLDER
                                                    + picTemplate.Tag.ToString();
                    if (!File.Exists(strTemplateFileLocation))
                    {
                        WebClient.DownloadFile(strRequestTemplateURL, strTemplateFileLocation);
                    }
                    Globals.ThisAddIn.Application.ActiveWindow.Selection.InsertNewPage();//加入一新空白頁
                    Globals.ThisAddIn.Application.ActiveWindow.Selection.InsertFile(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + @"\πFolio\Templates\" + picTemplate.Tag.ToString());
                }
            }
            catch (Exception ex)
            {
                log.WriteLine(LogType.Error, "TemplateForm::TemplateForm_Click", ex.ToString());
            }
        }

        /// <summary>
        /// 滑鼠拖曳點擊右側Template事件
        /// </summary>
        private void TemplateForm_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Left)
                {
                    NormalCursor = new Cursor("HandClench.cur");
                    NoDropCursor = new Cursor("nodrop.cur");
                    PictureBox picTemplate = (PictureBox)sender;
                    picTemplate.DoDragDrop(picTemplate.Tag, DragDropEffects.Copy);
                    if (isInsideDocument())
                    {
                        WebClient WebClient = new WebClient();
                        string strRequestTemplateURL = Properties.Settings.Default.URL_TEMPLATE
                                                        + treeTemplate.SelectedNode.Text
                                                        + "/dotx/"
                                                        + picTemplate.Tag.ToString().Split('.')[0];
                        string strTemplateFileLocation = Environment.GetFolderPath(Environment.SpecialFolder.Personal)
                                                        + Properties.Settings.Default.DEFAULT_TEMPLATE_FOLDER
                                                        + picTemplate.Tag.ToString();
                        if (!File.Exists(strTemplateFileLocation))
                        {
                            WebClient.DownloadFile(strRequestTemplateURL, strTemplateFileLocation);
                        }
                        Globals.ThisAddIn.Application.ActiveWindow.Selection.InsertFile(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + Properties.Settings.Default.DEFAULT_TEMPLATE_FOLDER + picTemplate.Tag.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                log.WriteLine(LogType.Error, "TemplateForm::TemplateForm_Drag", ex.ToString());
            }
        }

        /// <summary>
        /// 滑鼠進入Picturebox事件 - 秀出OpenFileIcon
        /// </summary>
        private void TemplateForm_MouseEnter(object sender, EventArgs e)
        {
            try
            {
                PictureBox picTemplate = (PictureBox)sender;
                //var picOpenFileIcon = picTemplate.Controls.OfType<PictureBox>().F
                System.Diagnostics.Debug.WriteLine("TemplateForm.MouseEnter");
                var picOpenFileIcon = picTemplate.Controls.OfType<PictureBox>().ElementAt(0);
                picOpenFileIcon.Visible = true;
                picOpenFileIcon = picTemplate.Controls.OfType<PictureBox>().ElementAt(2);
                picOpenFileIcon.Visible = true;
                picOpenFileIcon.BringToFront();
            }
            catch (Exception ex)
            {
                log.WriteLine(LogType.Error, "TemplateForm::TemplateForm_MouseEnter", ex.ToString());
            }
        }

        /// <summary>
        /// 滑鼠離開Picturebox事件 - 秀出OpenFileIcon
        /// </summary>
        private void TemplateForm_MouseLeave(object sender, EventArgs e)
        {
            try
            {
                PictureBox picTemplate = (PictureBox)sender;
                var picOpenFileIcon = picTemplate.Controls.OfType<PictureBox>().ElementAt(0);
                picOpenFileIcon.Visible = false;
                picOpenFileIcon = picTemplate.Controls.OfType<PictureBox>().ElementAt(1);
                picOpenFileIcon.Visible = false;

            }
            catch (Exception ex)
            {
                log.WriteLine(LogType.Error, "TemplateForm::TemplateForm_MouseLeave", ex.ToString());
            }
        }

        private bool MouseIsOverButton(Point location)
        {
            // Make sure the location is over the image.
            if (location.X < 0) return false;
            if (location.Y < 0) return false;
            if (location.X >= Properties.Resources.blackmask.Width) return false;
            if (location.Y >= Properties.Resources.blackmask.Height) return false;
            // See if the mask pixel at this position is black.
            Color color =
            Properties.Resources.blackmask.GetPixel(
            location.X, location.Y);
            return ((color.A == 255) &&
            (color.R == 0) &&
            (color.G == 0) &&
            (color.B == 0));
        }

        /// <summary>
        /// 滑鼠點選資料樹事件
        /// </summary>
        private void treeTemplate_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            try
            {
                if (e.Node.Text != "Templates" && e.Node.Text != strOrgNode)
                {
                    pnlPreview.Controls.Clear();//清除Panel中的所有物件
                    try
                    {
                        strOrgNode = e.Node.Text; //記錄當前為哪個node
                        WebClient WebClient = new WebClient();
                        byte[] bWebClient = WebClient.DownloadData(Properties.Settings.Default.URL_TEMPLATE + e.Node.Text + "/all");//取得byte array避免中文亂碼問題
                        string strJson = System.Text.Encoding.UTF8.GetString(bWebClient);//轉成string，準備Json的反序列化

                        // 寫法 1
                        // JSON 字串也可還原回JObject，動態存取
                        JObject restoredObject = JsonConvert.DeserializeObject<JObject>(strJson);
                        if (restoredObject.Count != 0)
                        {
                            System.Diagnostics.Debug.WriteLine(restoredObject.First.ToString());
                            //錯誤訊息狀態處理
                            //if (restoredObject["Status"].ToString() == "Failed")
                            {
                                //if (restoredObject["Message"].ToString() == "File not found")
                                //{
                                //    log.WriteLine(LogType.Warning, "TemplateForm::treeTemplate_NodeMouseClick", "File not found");
                                //}
                                //else if (restoredObject["Message"].ToString() == "API not found")
                                //{
                                //    log.WriteLine(LogType.Warning, "TemplateForm::treeTemplate_NodeMouseClick", "API not found");
                                //}
                            }
                            //else
                            {
                                PictureBox[] picTempPreview;
                                //PictureBox[] picNewFileIcon;
                                //PictureBox[] picInsertFileIcon;
                                Label[] labTempPreview;
                                picTempPreview = new PictureBox[restoredObject.Count];
                                //picNewFileIcon = new PictureBox[restoredObject.Count];
                                //picInsertFileIcon = new PictureBox[restoredObject.Count];
                                labTempPreview = new Label[restoredObject.Count];
                                for (int i = 0; i < restoredObject.Count; i++)
                                {
                                    //JObject可使用LINQ方式存取
                                    var q = from p in restoredObject.Properties() where p.Name == (i + 1).ToString() select p;

                                    System.Diagnostics.Debug.WriteLine(q.First().Value.ToString());

                                    labTempPreview[i] = new Label();
                                    labTempPreview[i].Location = new Point(i % 3 * 203 + 27, i / 3 * 255 + 228);
                                    labTempPreview[i].Text = q.First().Value.ToString().Split('.')[0];
                                    labTempPreview[i].AutoSize = true;
                                    labTempPreview[i].TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                                    labTempPreview[i].MaximumSize = new System.Drawing.Size(156, 0);
                                    labTempPreview[i].BringToFront();
                                    pnlPreview.Controls.Add(labTempPreview[i]);

                                    string strName = String.Format("Picturebox {0}", i);
                                    string strIconName = String.Format("FileIcon {0}", i);
                                    string strRequestTemplatePicURL = Properties.Settings.Default.URL_TEMPLATE
                                                                        + e.Node.Text
                                                                        + "/gif/"
                                                                        + q.First().Value.ToString().Split('.')[0];
                                    string strGifLocation = Environment.GetFolderPath(Environment.SpecialFolder.Personal)
                                                            + Properties.Settings.Default.DEFAULT_TEMPLATE_FOLDER
                                                            + Regex.Replace(q.First().Value.ToString(), "dotx", "gif");
                                    picTempPreview[i] = new PictureBox();
                                    picTempPreview[i].Location = new Point(i % 3 * 203 + 27, i / 3 * 255 + 15);
                                    picTempPreview[i].Size = new Size(156, 209);
                                    picTempPreview[i].BorderStyle = BorderStyle.FixedSingle;
                                    picTempPreview[i].Name = strName;
                                    picTempPreview[i].Tag = q.First().Value;
                                    picTempPreview[i].SizeMode = PictureBoxSizeMode.StretchImage;
                                    picTempPreview[i].AllowDrop = true;
                                    FileInfo fileinfo = new FileInfo(strGifLocation);
                                    if (!File.Exists(strGifLocation) || fileinfo.Length < 2048)//File doesn't exist or size too small
                                    {
                                        WebClient.DownloadFile(strRequestTemplatePicURL, strGifLocation);
                                    }
                                    picTempPreview[i].ImageLocation = strGifLocation;
                                    //if (picTempPreview[i].Image != null)
                                    //{
                                    //    picTempPreview[i].Image.Dispose();
                                    //}
                                    //picTempPreview[i].Image = Image.FromFile(strGifLocation);
                                    //picTempPreview[i].MouseEnter += new EventHandler(TemplateForm_MouseEnter);
                                    //picTempPreview[i].MouseLeave += new EventHandler(TemplateForm_MouseLeave);
                                    picTempPreview[i].MouseDown += new MouseEventHandler(TemplateForm_Click);//定義每個button事件左鍵點擊事件

                                    //picNewFileIcon[i] = new PictureBox();
                                    //picNewFileIcon[i].Location = new Point(136, 189);
                                    //picNewFileIcon[i].Size = new Size(18, 18);
                                    //picNewFileIcon[i].Image = Image.FromFile(@"D:\A-Cite for iGroup\WizCite 4.0\MSWordWizCite\MSWordWizCite\Resources\ic_launch_black_18dp.png");
                                    //picNewFileIcon[i].BackColor = Color.Transparent;
                                    //picNewFileIcon[i].Parent = picTempPreview[i];
                                    //picNewFileIcon[i].Name = strIconName;
                                    //picNewFileIcon[i].Visible = false;
                                    //picNewFileIcon[i].BringToFront();

                                    //picInsertFileIcon[i] = new PictureBox();
                                    //picInsertFileIcon[i].Location = new Point(116, 189);
                                    //picInsertFileIcon[i].Size = new Size(18, 18);
                                    //picInsertFileIcon[i].Image = Image.FromFile(@"D:\A-Cite for iGroup\WizCite 4.0\MSWordWizCite\MSWordWizCite\Resources\ic_system_update_tv_black_18dp.png");
                                    //picInsertFileIcon[i].BackColor = Color.Transparent;
                                    //picInsertFileIcon[i].Parent = picTempPreview[i];
                                    //picInsertFileIcon[i].Visible = false;
                                    //picInsertFileIcon[i].Name = strIconName;

                                    #region drag and drop
                                    //picTempPreview[i].GiveFeedback += new GiveFeedbackEventHandler(TemplateForm_GiveFeedback);
                                    //picTempPreview[i].MouseDown += new MouseEventHandler(TemplateForm_MouseDown);//拖曳PictureBox裡面的Template
                                    #endregion
                                    pnlPreview.Controls.Add(picTempPreview[i]);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        //MessageBox.Show(Lang.en_US.Login_NoInternet_Msg, Lang.en_US.Login_NoInternet_Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        log.WriteLine(LogType.Error, "TemplateForm::treeTemplate_NodeMouseClick - NoInternet", ex.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                log.WriteLine(LogType.Error, "TemplateForm::treeTemplate_NodeMouseClick", ex.ToString());
            }
        }



        #endregion
        private void TemplateForm_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            e.UseDefaultCursors = false;
            //if ((e.Effect & DragDropEffects.Copy) == DragDropEffects.Copy)
            Cursor.Current = NormalCursor;
            //else
            //    Cursor.Current = NoDropCursor;
        }

        private void treeTemplate_AfterExpand(object sender, TreeViewEventArgs e)
        {
            try
            {
                int iCount = 0;
                WebClient WebClient = new WebClient();
                byte[] bWebClient = WebClient.DownloadData(Properties.Settings.Default.URL_TEMPLATE + "General//all");//取得byte array避免中文亂碼問題
                string strJson = System.Text.Encoding.UTF8.GetString(bWebClient);//轉成string，準備Json的反序列化

                // 寫法 1
                // JSON 字串也可還原回JObject，動態存取
                JObject restoredObject = JsonConvert.DeserializeObject<JObject>(strJson);
                iCount = restoredObject.Count;
                System.Diagnostics.Debug.WriteLine(restoredObject.First.ToString());
                PictureBox[] picTempPreview;
                PictureBox[] picNewFileIcon;
                PictureBox[] picInsertFileIcon;
                PictureBox[] picMask;
                Label[] labTempPreview;
                picTempPreview = new PictureBox[restoredObject.Count];
                picNewFileIcon = new PictureBox[restoredObject.Count];
                picInsertFileIcon = new PictureBox[restoredObject.Count];
                picMask = new PictureBox[restoredObject.Count];
                labTempPreview = new Label[restoredObject.Count];
                for (int i = 0; i < restoredObject.Count; i++)
                {
                    //JObject可使用LINQ方式存取
                    var q = from p in restoredObject.Properties() where p.Name == (i + 1).ToString() select p;

                    System.Diagnostics.Debug.WriteLine(q.First().Value.ToString());

                    labTempPreview[i] = new Label();
                    labTempPreview[i].Location = new Point(i % 3 * 203 + 27, i / 3 * 255 + 228);
                    labTempPreview[i].Text = q.First().Value.ToString().Split('.')[0];
                    labTempPreview[i].AutoSize = true;
                    labTempPreview[i].TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                    labTempPreview[i].BringToFront();

                    string strName = String.Format("Picturebox {0}", i);
                    string strIconName = String.Format("FileIcon {0}", i);
                    string strRequestTemplatePicURL = Properties.Settings.Default.URL_TEMPLATE
                                                        + "General//gif//"
                                                        + q.First().Value.ToString().Split('.')[0];
                    string strGifLocation = Environment.GetFolderPath(Environment.SpecialFolder.Personal)
                                            + Properties.Settings.Default.DEFAULT_TEMPLATE_FOLDER
                                            + Regex.Replace(q.First().Value.ToString(), "dotx", "gif");
                    picTempPreview[i] = new PictureBox();
                    picTempPreview[i].Location = new Point(i % 3 * 203 + 27, i / 3 * 255 + 15);
                    picTempPreview[i].Size = new Size(156, 209);
                    picTempPreview[i].BorderStyle = BorderStyle.FixedSingle;
                    picTempPreview[i].Name = strName;
                    picTempPreview[i].Tag = q.First().Value;
                    picTempPreview[i].SizeMode = PictureBoxSizeMode.StretchImage;
                    picTempPreview[i].AllowDrop = true;
                    picTempPreview[i].InitialImage = MSWordpiCite.Properties.Resources.loading;
                    FileInfo fileinfo = new FileInfo(strGifLocation);
                    if (!File.Exists(strGifLocation) || fileinfo.Length < 2048)//File doesn't exist or size too small
                    {
                        WebClient.DownloadFile(strRequestTemplatePicURL, strGifLocation);
                    }
                    picTempPreview[i].ImageLocation = strGifLocation;
                    //picTempPreview[i].MouseEnter += new EventHandler(TemplateForm_MouseEnter);
                    //picTempPreview[i].MouseLeave += new EventHandler(TemplateForm_MouseLeave);
                    //picTempPreview[i].Visible = false;//先看不到
                    picTempPreview[i].MouseDown += new MouseEventHandler(TemplateForm_Click);//定義每個button事件左鍵點擊事件

                    //picNewFileIcon[i] = new PictureBox();
                    //picNewFileIcon[i].Location = new Point(136, 189);
                    //picNewFileIcon[i].Size = new Size(18, 18);
                    //picNewFileIcon[i].Image = Image.FromFile(@"D:\Pi-Cite for iGroup\WizCite 4.0\MSWordWizCite\MSWordWizCite\Resources\ic_launch_black_18dp.png");
                    //picNewFileIcon[i].BackColor = Color.Transparent;
                    //picNewFileIcon[i].Parent = picTempPreview[i];
                    //picNewFileIcon[i].Name = strIconName;
                    //picNewFileIcon[i].Visible = false;
                    //picNewFileIcon[i].BringToFront();

                    //picInsertFileIcon[i] = new PictureBox();
                    //picInsertFileIcon[i].Location = new Point(116, 189);
                    //picInsertFileIcon[i].Size = new Size(18, 18);
                    //picInsertFileIcon[i].Image = Image.FromFile(@"D:\Pi-Cite for iGroup\WizCite 4.0\MSWordWizCite\MSWordWizCite\Resources\ic_system_update_tv_black_18dp.png");
                    //picInsertFileIcon[i].BackColor = Color.Transparent;
                    //picInsertFileIcon[i].Parent = picTempPreview[i];
                    //picInsertFileIcon[i].Visible = false;
                    //picInsertFileIcon[i].Name = strIconName;

                    pnlPreview.Controls.Add(picTempPreview[i]);
                    pnlPreview.Controls.Add(labTempPreview[i]);
                }
            }
            catch(Exception ex)
            {
                log.WriteLine(LogType.Error, "TemplateForm::treeTemplate_AfterExpand", ex.ToString());
            }
        }

        #region MouseWheel event relative functions

        [DllImport("user32.dll")]
        private static extern IntPtr WindowFromPoint(Point pt);
        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);
        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == 0x20a)
            {
                // WM_MOUSEWHEEL, find the control at screen position m.LParam
                Point pos = new Point(m.LParam.ToInt32() & 0xffff, m.LParam.ToInt32() >> 16);
                IntPtr hWnd = WindowFromPoint(pos);
                if (hWnd != IntPtr.Zero && hWnd != m.HWnd && Control.FromHandle(hWnd) != null)
                {
                    SendMessage(hWnd, m.Msg, m.WParam, m.LParam);
                    return true;
                }
            }
            return false;
        }

        #endregion
    }
}
