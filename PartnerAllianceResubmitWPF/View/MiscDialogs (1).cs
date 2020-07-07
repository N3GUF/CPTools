using Microsoft.Win32;
using System;

namespace Comdata.AppSupport.PartnerAllianceResubmit.View
{
    static class MiscDialogs
    {
        public static string OpenFile(string path)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.InitialDirectory = path;
            Nullable<bool> dr = dialog.ShowDialog();

            if (dr == true)
                return dialog.FileName;
            else
                return null;
        }
    }
}
