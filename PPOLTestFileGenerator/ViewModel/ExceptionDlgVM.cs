using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Comdata.AppSupport.PPOLTestFileGenerator.ViewModel
{
    class ExceptionDlgVM : INotifyPropertyChanged
    {
        #region Private Properties
        private string _errorMsg;
        private string _exceptionMsg;
        #endregion

        #region Public Properties
        public string ErrorMsg
        {
            get { return _errorMsg; }
            set
            {
                _errorMsg = value;
                NotifyPropertyChanged("ErrorMsg");
            }
        }
        public string ExceptionMsg
        {
            get { return _exceptionMsg; }
            set
            {
                _exceptionMsg = value;
                NotifyPropertyChanged("ExceptionMsg");
            }
        }
        #endregion

        #region INPC
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the property changed event.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        public virtual void NotifyPropertyChanged(string propertyName)
        {
            //  Store the event handler - in case it changes between
            //  the line to check it and the line to fire it.
            PropertyChangedEventHandler propertyChanged = PropertyChanged;

            //  If the event has been subscribed to, fire it.
            if (propertyChanged != null)
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
