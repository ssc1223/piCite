using System.Runtime.InteropServices;

namespace MSWordWizCite.Classes
{
    //Expose COM Add-in VSTO method to VBA Macro

    [ComVisible(true)]
    public interface IVbaAccess
    {
        //void InsertCitation();
        //void ChangeCitationStyle();
        //void ReferenceList();
        //void FindReference();
    }

    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    public class VbaAccess : IVbaAccess
    {
        //public void InsertCitation()
        //{
        //    Globals.ThisAddIn.ActivateInsertCitation();
        //}
        //public void ChangeCitationStyle()
        //{
        //    Globals.ThisAddIn.ActivateChangeStyle();
        //}
        //public void ReferenceList()
        //{
        //    Globals.ThisAddIn.ActivateReferenceList();
        //}
        //public void FindReference()
        //{
        //    Globals.ThisAddIn.ActivateFindReference();
        //}
    }
}
