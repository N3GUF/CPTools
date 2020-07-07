using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Comdata.AppSupport.UpdateLoadCancelReject.DAL
{
    public interface IDAL
    {
        string Username { set; get; }
        string Password { set; get; }
        bool ConnectionIsReady { get; }

        bool Open();
        void Close();
        void GetParentContractInfo(string cardholderContract, ref string parentContract, ref string parentContractName);
    }
}
