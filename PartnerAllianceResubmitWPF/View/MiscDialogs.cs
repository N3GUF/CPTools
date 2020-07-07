using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace PartnerAllianceResubmitWPF.View
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
