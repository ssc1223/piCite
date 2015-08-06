using Microsoft.Office.Tools.Ribbon;
using MSWordpiCite.Lang;

namespace MSWordpiCite.RibbonButtons
{
    public partial class ribpiCiteTab : RibbonBase
    {
        public ribpiCiteTab() : base(Globals.Factory.GetRibbonFactory())
        {
            InitializeComponent();
        }

        #region Event Handlers

        private void ribpiCiteTab_Load(object sender, RibbonUIEventArgs e)
        {
            btnpiCite.ScreenTip = string.Format(Lang.en_US.piCite_Ribbon_Button_ScreenTip, Properties.Settings.Default.VERSION);
            btnpiCite.SuperTip = Lang.en_US.piCite_Ribbon_Button_SuperTip;
            btnpiCite.Label = string.Format(Lang.en_US.piCite_Ribbon_Button_Label, Properties.Settings.Default.VERSION);
        }
        private void btnpiCite_Click(object sender, RibbonControlEventArgs e)
        {
            Globals.ThisAddIn.StartpiCitePane();
        }

        #endregion
    }
}
