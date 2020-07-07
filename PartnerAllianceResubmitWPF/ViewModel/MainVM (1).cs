using Comdata.AppSupport.AppTools;
using Comdata.AppSupport.PartnerAllianceResubmit.MVVM;
using System;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Windows;

namespace Comdata.AppSupport.PartnerAllianceResubmit.ViewModel
{
    class MainVM : MVVM.ViewModel
    {
        #region Private Properties
        private Model.PartnerAllianceResubmit _model;
        View.ExceptionDlg _exceptionDlg;
        ViewModel.ExceptionDlgVM _exceptionVM;
        private string _status;
        private RelayCommand _checkForErrorsCommand;
        private RelayCommand _prepareForResubmitCommand;
        private RelayCommand _browseForPS14FileCommand;
        private RelayCommand _browseForPS15FileCommand;
        private Visibility _checkForErrorsVisibility;
        private Visibility _prepareForResubitVisibility;
        Logger _log;
        #endregion

        #region Public Properties
        public Model.PartnerAllianceResubmit Model
        {
            get
            {
                return _model;
            }

            set
            {
                _model = value;
                NotifyPropertyChanged("Model");

            }
        }
        public RelayCommand BrowseForPS14FileCommand
        {
            get { return _browseForPS14FileCommand; }
            set
            {
                _browseForPS14FileCommand = value;
                NotifyPropertyChanged("BrowseForPS14FileCommand");
            }
        }
        public RelayCommand BrowseForPS15FileCommand
        {
            get { return _browseForPS15FileCommand; }
            set
            {
                _browseForPS15FileCommand = value;
                NotifyPropertyChanged("BrowseForPS15FileCommand");
            }
        }
        public RelayCommand CheckForErrorsCommand
        {
            get { return _checkForErrorsCommand; }
            set
            {
                _checkForErrorsCommand = value;
                NotifyPropertyChanged("CheckForErrorsCommand");
            }
        }
        public RelayCommand PrepareForResubmitCommand
        {
            get { return _prepareForResubmitCommand; }
            set
            {
                _prepareForResubmitCommand = value;
                NotifyPropertyChanged("PrepareForResubmitCommand");
            }
        }
        public string Status
        {
            get { return _status; }
            set
            {
                _status = value;
                NotifyPropertyChanged("Status");
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
                NotifyPropertyChanged("CheckForErrorsVisibility");
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
                NotifyPropertyChanged("PrepareForResubitVisibility");
            }
        }
        #endregion

        #region Public Methods
        public MainVM()
        {
            this._exceptionDlg = new View.ExceptionDlg();
            this._exceptionVM = new ViewModel.ExceptionDlgVM();
            this._exceptionDlg.DataContext = this._exceptionVM;
            this.Model = new Model.PartnerAllianceResubmit();
            //this.Model.Reset();

            BrowseForPS14FileCommand = new RelayCommand(e => browseForPS14File());
            BrowseForPS15FileCommand    = new RelayCommand(e => browseForPS15File());
            CheckForErrorsCommand       = new RelayCommand(e => checkForErrors(), e => canCheckForErrors());
            PrepareForResubmitCommand   = new RelayCommand(e => prepareForResubmit(), e => canPrepareForResubmit());
            this.CheckForErrorsVisibility = Visibility.Hidden;
            this.PrepareForResubitVisibility = Visibility.Hidden;
        }
        #endregion

        #region Private Methods
        private void browseForPS14File()
        {
           Model.PS14Pathname = View.MiscDialogs.OpenFile(_model.PS14Pathname);
        }

        private void browseForPS15File()
        {
            _model.PS15Pathname = View.MiscDialogs.OpenFile(_model.PS15Pathname);
        }

        private bool canCheckForErrors()
        {
            return true;
        }

        private void checkForErrors()
        {
            var workingDir = Path.GetDirectoryName(_model.PS15Pathname);
            _log = new Logger(workingDir);
            _log.AddSeverityLevel = true;
            _log.AddTimeStamp = true;

            _log.Write("PS14 Filename: {0}", Path.GetFileName(_model.PS14Pathname));
            _log.Write("PS15 Filename: {0}", Path.GetFileName(_model.PS15Pathname));

            try
            {
                if (_model.CheckForErrors())
                {
                    CheckForErrorsVisibility = Visibility.Hidden;

                    if (File.Exists(_model.PS14Pathname))
                    {
                        PrepareForResubitVisibility = Visibility.Visible;
                    }
                    
                    this.Status = String.Format("{0} cards were not assigned in: {1}.", _model.CardsNotAssigned, Path.GetFileName(_model.PS15Pathname));
                }
                else
                {
                    _log.Write("No Errors found in: {0}.", _model.PS15Pathname);
                    this.Status = String.Format("No Errors found in: {0}.", Path.GetFileName(_model.PS15Pathname));
                }
            }
            catch (Exception ex)
            {
                AppTools.Utilities.ReportException(ex, _model.Log);
            }

        }

        private bool canPrepareForResubmit()
        {
            return true;
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
            this.Status = String.Format("Extracted {0} cards into: {1}.", _model.CardsExtracted
                                                                        , Path.GetFileName(_model.ResubmitFilename));
            CheckForErrorsVisibility = Visibility.Hidden;
            PrepareForResubitVisibility = Visibility.Hidden;
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                _model.ExtractErrors();
            }
            catch (Exception ex)
            {
                AppTools.Utilities.ReportException(ex, _model.Log);
            }
        }
        #endregion
    }
}
