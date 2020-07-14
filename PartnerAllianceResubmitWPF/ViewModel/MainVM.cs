using Comdata.AppSupport.AppTools;
using Comdata.AppSupport.PartnerAllianceResubmit;
using System;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace PartnerAllianceResubmitWPF.ViewModel 
{
    class MainVM : INotifyPropertyChanged 
    {
        #region Private Properties
        private string _ps14Pathname;
        private string _ps15Pathname;
        private string _status;
        private ICommand _checkForErrorsCommand;
        private ICommand _prepareForResubmitCommand;
        private ICommand _browseForPS14FileCommand;
        private ICommand _browseForPS15FileCommand;
        private DataTable _acctCustChangesDT;
        private Visibility _checkForErrorsVisibility;
        private Visibility _prepareForResubitVisibility;
        Logger _log;
        PartnerAllianceResubmit _resub;
        #endregion

        #region Public Properties
        public ICommand BrowseForPS14FileCommand
        {
            get { return _browseForPS14FileCommand; }
            set
            {
                _browseForPS14FileCommand = value;
                RaisePropertyChanged("BrowseForPS14FileCommand");
            }
        }
        public ICommand BrowseForPS15FileCommand
        {
            get { return _browseForPS15FileCommand; }
            set
            {
                _browseForPS15FileCommand = value;
                RaisePropertyChanged("BrowseForPS15FileCommand");
            }
        }
        public ICommand CheckForErrorsCommand
        {
            get { return _checkForErrorsCommand; }
            set
            {
                _checkForErrorsCommand = value;
                RaisePropertyChanged("CheckForErrorsCommand");
            }
        }
        public ICommand PrepareForResubmitCommand
        {
            get { return _prepareForResubmitCommand; }
            set
            {
                _prepareForResubmitCommand = value;
                RaisePropertyChanged("PrepareForResubmitCommand");
            }
        }
        public string PS14Pathname
        {
            get { return _ps14Pathname; }
            set
            {
                _ps14Pathname = value;
                this.Status = "";

                if (!File.Exists(_ps14Pathname) || !_ps14Pathname.Contains("PS00014"))
                    this.Status = "Not a valid PS00014 file";

                RaisePropertyChanged("PS14Pathname");
            }
        }
        public string PS15Pathname
        {
            get { return _ps15Pathname; }
            set
            {
                _ps15Pathname = value;
                this.Status = "";

                if (File.Exists(_ps15Pathname) && _ps15Pathname.Contains("PS00015"))
                    CheckForErrorsVisibility = Visibility.Visible;
                else
                { 
                    CheckForErrorsVisibility = Visibility.Hidden;
                    this.Status = "Not a valid PS00015 file";
                }

                PrepareForResubitVisibility = Visibility.Hidden;
                RaisePropertyChanged("PS15Pathname");
            }
        }
        public string Status
        {
            get { return _status; }
            set
            {
                _status = value;
                RaisePropertyChanged("Status");
            }
        }
        public DataTable AcctCustChangesDT
        {
            get { return _acctCustChangesDT; }
            set
            {
                _acctCustChangesDT = value;
                RaisePropertyChanged("AcctCustChangesDT");
            }
        }
        public Visibility CheckForErrorsVisibility
        {
            get
            {
                return _checkForErrorsVisibility;
            }
            set
            {
                _checkForErrorsVisibility = value;
                RaisePropertyChanged("CheckForErrorsVisibility");
            }
        }
        public Visibility PrepareForResubitVisibility
        {
            get
            {
                return _prepareForResubitVisibility;
            }
            set
            {
                _prepareForResubitVisibility = value;
                RaisePropertyChanged("PrepareForResubitVisibility");
            }
        }
        #endregion

        #region Public Methods
        public MainVM()
        {
            BrowseForPS14FileCommand    = new ActionCommand(e => browseForPS14File());
            BrowseForPS15FileCommand    = new ActionCommand(e => browseForPS15File());
            CheckForErrorsCommand       = new ActionCommand(e => checkForErrors(), e => canCheckForErrors());
            PrepareForResubmitCommand   = new ActionCommand(e => prepareForResubmit());
            this.AcctCustChangesDT      = new DataTable();
            this.AcctCustChangesDT.Columns.Add(new DataColumn("OrigAcct", typeof(String)));
            this.AcctCustChangesDT.Columns.Add(new DataColumn("OrigCust", typeof(String)));
            this.AcctCustChangesDT.Columns.Add(new DataColumn("ReqAcct", typeof(String)));
            this.AcctCustChangesDT.Columns.Add(new DataColumn("ReqCust", typeof(String)));
            this.CheckForErrorsVisibility = Visibility.Hidden;
            this.PrepareForResubitVisibility = Visibility.Hidden;
        }
        #endregion

        #region Private Methods
        private void browseForPS14File()
        {
            this.PS14Pathname = View.MiscDialogs.OpenFile(this.PS14Pathname);
        }

        private void browseForPS15File()
        {
            this.PS15Pathname = View.MiscDialogs.OpenFile(this.PS15Pathname);
        }

        private bool canCheckForErrors()
        {
            return false;
        }

        private void checkForErrors()
        {
            var workingDir = Path.GetDirectoryName(this._ps15Pathname);
            _log = new Logger(workingDir);
            _log.AddSeverityLevel = true;
            _log.AddTimeStamp = true;

            _resub = new PartnerAllianceResubmit();
            _resub.Log = _log;
            _resub.PS14Pathname = this._ps14Pathname;
            _resub.PS15Pathname = this._ps15Pathname;
            _resub.AcctCustChangesDT = this.AcctCustChangesDT;
            _log.Write("PS14 Filename: {0}", Path.GetFileName(_resub.PS14Pathname));
            _log.Write("PS15 Filename: {0}", Path.GetFileName(_resub.PS15Pathname));

            try
            {
                if (_resub.CheckForErrors())
                {
                    CheckForErrorsVisibility = Visibility.Hidden;

                    if (File.Exists(_ps14Pathname))
                    {
                        PrepareForResubitVisibility = Visibility.Visible;
                    }
                    
                    this.Status = String.Format("{0} cards were not assigned in: {1}.", _resub.CardsNotAssigned, Path.GetFileName(_resub.PS15Pathname));
                }
                else
                {
                    _log.Write("No Errors found in: {0}.", _resub.PS15Pathname);
                    this.Status = String.Format("No Errors found in: {0}.", Path.GetFileName(_resub.PS15Pathname));
                }
            }
            catch (Exception ex)
            {
                AppTools.Utilities.ReportException(ex, _log);
            }

        }
        private void prepareForResubmit()
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.RunWorkerAsync();
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Status = String.Format("Extracted {0} cards into: {1}.", _resub.CardsExtracted
                                                                        , Path.GetFileName(_resub.ResubmitFilename));
            CheckForErrorsVisibility = Visibility.Hidden;
            PrepareForResubitVisibility = Visibility.Hidden;
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                _resub.ExtractErrors();
            }
            catch (Exception ex)
            {
                AppTools.Utilities.ReportException(ex, _log);
            }
        }
        #endregion

        #region INPC
        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion  
    }

    public class ActionCommand : ICommand
    {
        private readonly Action<string> _codeToExecute;
        private readonly Action<string> _canExecteCodeToExecute;
                     
        public bool CanExecute(object parameter)
        {
            return true;        
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            _codeToExecute(null);
        }

        public ActionCommand(Action<string> codeToExecute)
        {
            _codeToExecute = codeToExecute;
        }

        public ActionCommand(Action<string> codeToExecute, Action<string> canExecteCodeToExecute)
        {
            _codeToExecute = codeToExecute;
            _canExecteCodeToExecute = canExecteCodeToExecute;
        }
    }
 
    public class EnumMatchToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return false;

            string checkValue = value.ToString();
            string targetValue = parameter.ToString();
            return checkValue.Equals(targetValue,
                     StringComparison.InvariantCultureIgnoreCase);
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return null;

            bool useValue = (bool)value;
            string targetValue = parameter.ToString();
            if (useValue)
                return Enum.Parse(targetType, targetValue);

            return null;
        }
    } 
}
