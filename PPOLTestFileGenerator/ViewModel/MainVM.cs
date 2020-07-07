using Comdata.AppSupport.AppTools;
using Comdata.AppSupport.PPOLTestFileGenerator.Model;
using Comdata.AppSupport.PPOLTestFileGenerator.MVVM;
using System;
using System.ComponentModel;

namespace Comdata.AppSupport.PPOLTestFileGenerator.ViewModel
{
    class MainVM : MVVM.ViewModel
    {
        #region Private Properties
        Model.PpolTestFileGenerator _model;
        View.ExceptionDlg _exceptionDlg;
        ViewModel.ExceptionDlgVM _exceptionVM;

        ILog _log;
        string _status;

        private RelayCommand _addCmd;
        private RelayCommand _cancelCmd;
        private RelayCommand _createCmd;

        #endregion

        #region Public Properties
        public string Status
        {
            get { return _status; }
            set
            {
                _status = value;
                NotifyPropertyChanged("Status");
            }
        }

        public RelayCommand AddCmd
        {
            get
            {
                return _addCmd;
            }

            set
            {
                _addCmd = value;
                NotifyPropertyChanged("AddCmd");
            }
        }
        public RelayCommand CancelCmd
        {
            get
            {
                return _cancelCmd;
            }

            set
            {
                _cancelCmd = value;
                NotifyPropertyChanged("CancelCmd");
            }
        }
        public RelayCommand CreateCmd
        {
            get
            {
                return _createCmd;
            }

            set
            {
                _createCmd = value;
                NotifyPropertyChanged("CreateCmd");
            }
        }

        public PpolTestFileGenerator Model
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

        #endregion
        
        #region Public Methods
        public MainVM()
        {
            this._exceptionDlg = new View.ExceptionDlg();
            this._exceptionVM = new ViewModel.ExceptionDlgVM();
            this._exceptionDlg.DataContext = this._exceptionVM;
            this._log = new TextLogger(Properties.Settings.Default.LogPath
                                     , Properties.Settings.Default.LoggingThreshold);
            
            this.Model = new Model.PpolTestFileGenerator(_log);
            this.Model.TestFileDestinationBeforeOvernightCutOff = Properties.Settings.Default.TesFileDestinationBeforeOvernightCutOff;
            this.Model.TestFileDestinationAfterOvernightCutOff = Properties.Settings.Default.TesFileDestinationAfterOvernightCutOff;
            this.Model.OvernightCutOffHHMM = Properties.Settings.Default.OvernightCutOffHHMM;
            this.Model.TestFileCopyDestination = Properties.Settings.Default.TesFileCopyDestination;
            this.Model.SendScript = Properties.Settings.Default.SendScript;

            this.Model.Reset();
            this.AddCmd = new RelayCommand(e => this.addCard(), e => this.canAddCard());
            this.CancelCmd = new RelayCommand(e => this.resetForm(), e => this.canResetForm());
            this.CreateCmd = new RelayCommand(e => this.createTestFile(), e => this.canCreateTestFile());
            this.AddCmd.CanExecuteChanged += AddCmd_CanExecuteChanged;
        }

        private void AddCmd_CanExecuteChanged(object sender, EventArgs e)
        {
        }
        #endregion

        #region Private Methods
 
        private bool canAddCard()
        {
            return _model.IsValid; 
        }

        private void addCard()
        {
            _model.Status = "Adding Card";
            _model.AddCard();
        }

        private bool canResetForm()
        {
            if (_model.CardsToCreateDT.Rows.Count > 0)
                return true;

            return false;
        }

        private void resetForm()
        {
            _model.Reset();
            NotifyPropertyChanged(string.Empty);

        }

        private bool canCreateTestFile()
        {
            if (_model.CardsToCreateDT.Rows.Count > 0)
                return true;

            return false;
        }

        private void createTestFile()
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.RunWorkerAsync();
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                var filename = _model.CreateTestFile();
                _model.CopyTestFile(filename);
                e.Result = _model.SendTestFile(filename);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                AppTools.Utilities.ReportException(e.Error, _log);
                _exceptionVM.ErrorMsg = e.Error.Message;
                _exceptionVM.ExceptionMsg = e.Error.InnerException.Message;
                _exceptionDlg.ShowDialog();
            }
            else
                _model.Status = (string)e.Result;

            _model.Reset();
        }
        #endregion
    }
}

