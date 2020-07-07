using System;

namespace Comdata.AppSupport.AppTools
{
    public class LogUpdatedEventArgs : EventArgs
    {
        public readonly string Mwssage;

        public LogUpdatedEventArgs(string message)
        {
            this.Mwssage = message;
        }
    }
}
