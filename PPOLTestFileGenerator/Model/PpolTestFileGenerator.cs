using Comdata.AppSupport.AppTools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Threading;

namespace Comdata.AppSupport.PPOLTestFileGenerator.Model
{
    class PpolTestFileGenerator : IDataErrorInfo, INotifyPropertyChanged
    {
        #region Private Properties
        ILog _log;
        string _binRange;
        string _customCode;
        ObservableCollection<ShippingMethod> _shippingMethods;
        string _shippingAddress;
        string _status;
        DataTable _cardsToCreateDT;
        #endregion

        #region Public Properties
        public DataTable CardsToCreateDT
        {
            get
            {
                return _cardsToCreateDT;
            }

            set
            {
                _cardsToCreateDT = value;
            }
        }

        public string BinRange
        {
            get { return _binRange; }
            set
            {
                if (_binRange == value)
                    return;

                _binRange = value;
               NotifyPropertyChanged("BinRange");
            }
        }

        public string CustomCode
        {
            get { return _customCode; }
            set
            {
                if (_customCode == value)
                    return;

                    _customCode = value;
                    NotifyPropertyChanged("CustomCode");
            }
        }

        public ObservableCollection<ShippingMethod> ShippingMethods
        {
            get
            {
                return _shippingMethods;
            }

            set
            {
                _shippingMethods = value;
                NotifyPropertyChanged("ShippingMethods");
            }
        }

        public string ShippingAddress
        {
            get { return _shippingAddress; }
            set
            {
                _shippingAddress = value;
                NotifyPropertyChanged("ShippingAddress");
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

        public string TestFileDestinationBeforeOvernightCutOff { get; internal set; }

        public string TestFileDestinationAfterOvernightCutOff { get; internal set; }

        public string OvernightCutOffHHMM { get; internal set; }

        public string TestFileCopyDestination { get; internal set; }

        public string SendScript { get; internal set; }
        #endregion

        #region Validation
        string IDataErrorInfo.Error
        {
            get
            {
                return null;
            }
        }

        static readonly string[] PropertiesToValidate =
        {
            "BinRange",
            "CustomCode",
            "ShippingNethods"
        };

        public bool IsValid
        {
            get
            {
                foreach (string property in PropertiesToValidate)
                    if (GetValidationError(property) != null)
                        return false;

                return true;
            }
        }

        string IDataErrorInfo.this[string propertyName]
        {
            get
            {
                return GetValidationError(propertyName);
            }
        }

        public string GetValidationError(String propertyName)
        {
            string error = null;

            switch (propertyName)
            {
                case "BinRange":
                    error = validateBinRange();
                    break;

                case "CustomCode":
                    error = validateCustomCode();
                    break;

                case "ShippingNethods":
                    error = validateShippingMethods();
                    break;
            }

            return error;
        }

        private string validateBinRange()
        {
            if (this.BinRange == null)
                return null;

            string error = null;
            var bin = 0;
            this._binRange = this._binRange.Trim();
  
            if (!this._binRange.StartsWith("5"))
            {
                error = "Bin range must be a 6 digit integer beginning with 5.";
                return error;
            }

            if (this._binRange.Length != 6)

            {
                error = "Bin range must be a 6 digit integer beginning with 5.";
                return error;
            }

            if (!Int32.TryParse(this._binRange, out bin))
            {
                error = "Bin range must be a 6 digit integer beginning with 5.";
                return error;
            }

            return error;
        }

        private string validateCustomCode()
        {
            if (this.CustomCode == null)
                return null;

            string error = null;
         //   var Custom = 0;
            this._customCode = this._customCode.Trim();

            if (this._customCode.Length != 2)
            {
                error = "Custom code must be 2 characters.";
                return error;
            }

            // Custom codes can now be alphanumeric.
            //if (!Int32.TryParse(this._customCode, out Custom))
            //{
            //    error = "Custom code must be a 2 digit integer.";
            //    return error;
            //}

           // this._CustomCode = string.Format("0:##", this._CustomCode);
            return error;
        }

        private string validateShippingMethods()
        {
            var error = "Please select at least 1 shipping method.";

            foreach (ShippingMethod method in this._shippingMethods)
                if (method.IsSelected)
                    return null;

            return error;
        }
        #endregion

        #region Public Methods
        public PpolTestFileGenerator(ILog log)
        {
            this._log = log;
            createCardsTable();
            loadShippingMethods();
            Reset();
        }

        public void AddCard()
        {
            foreach (ShippingMethod method in this.ShippingMethods)
            {
                if (method.IsSelected)
                {
                    DataRow row = this._cardsToCreateDT.NewRow();
                    row["CustomCd"] = this.CustomCode;
                    row["ShippingAddr"] = this.ShippingAddress;
                    row["CardNbr"] = generateCardNumber(this.BinRange);
                    row["ShippingMthd"] = method.Description;
                    this.CardsToCreateDT.Rows.Add(row);
                    this.CardsToCreateDT.AcceptChanges();
                    this._log.Write("Ordering card {0} with a Custom Code {1} via {2}.", mask((string)row["CardNbr"]), this.CustomCode, method.Description);
                }
            }
        }

        public string CreateTestFile()
        {
            try
            {
                return createTestFile();
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to create test file.", ex);
            }
        }

        public void CopyTestFile(string filename)
        {
            var source = Path.Combine(this.TestFileDestinationAfterOvernightCutOff, filename);
            var dest = Path.Combine(this.TestFileCopyDestination, filename);
            this._log.Write("Copying: {0} to {1}...", filename, this.TestFileCopyDestination);
            this.Status = String.Format("Copying: {0} to {1}...", filename, this.TestFileCopyDestination);
            File.Copy(source, dest);
        }

        public string SendTestFile(string filename)
        {
            this._log.Write("");
            this._log.Write("Sending: {0} to Oberthur.", filename);
            this.Status = String.Format("Sending: {0} to Oberthur.", filename);
            Thread.Sleep(1000);

            try
            {
                System.Diagnostics.Process.Start(this.SendScript);
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to send test file.", ex);
            }

            return String.Format("Sent: {0} to Oberthur.", filename);
        }

        public void Reset()
        {
            this.BinRange = null;
            this.CustomCode = null;
            this.ShippingAddress = "Card Services\n5301 Maryland Way\nBrentwood, TN 37027 USA";
            this.CardsToCreateDT.Clear();
            this.ShippingMethods[0].IsSelected = false;
            this.ShippingMethods[1].IsSelected = true;
            this.ShippingMethods[2].IsSelected = false;
            this.ShippingMethods[3].IsSelected = false;
        }

        #endregion

        #region Private Methods
        private void createCardsTable()
        {
            this.CardsToCreateDT = new DataTable();
            this.CardsToCreateDT.Columns.Add("CardNbr", typeof(string));
            this.CardsToCreateDT.Columns.Add("CustomCd", typeof(string));
            this.CardsToCreateDT.Columns.Add("ShippingMthd", typeof(string));
            this.CardsToCreateDT.Columns.Add("ShippingAddr", typeof(string));
        }

        private void loadShippingMethods()
        {
            this._shippingMethods = new ObservableCollection<ShippingMethod>();
            this._shippingMethods.Add(new Model.ShippingMethod("Fedex Ground", "06B", false));
            this._shippingMethods.Add(new Model.ShippingMethod("Fedex Overnight", "10B", false));
            this._shippingMethods.Add(new Model.ShippingMethod("Fedex Two Day", "13B", false));
            this._shippingMethods.Add(new Model.ShippingMethod("USPS", "47C", false));
            this._shippingMethods[0].IsSelected = true;
        }

        private string generateCardNumber(string binRange)
        {
            var cardNumber = binRange;
            Thread.Sleep(100);
            Random rnd = new Random();

            for (int i = 0; i < 10; i++)
                cardNumber += rnd.Next(0,9).ToString();

             return cardNumber;
         }

        private string mask(string card)
        {
            return card.Substring(0, 4) + " "
                 + card.Substring(4, 2)
                 + "** **** "
                 + card.Substring(12, 4);
        }

        private string createTestFile()
        {
            this._log.Write("Generating Prepaid Test Card Creation File for Oberthur...");
            var dateTime = DateTime.Now;
            var header = createHeader(dateTime);
            var cardsCreated = 0;
            List<string> details = new List<string>();

            foreach (DataRow row in this.CardsToCreateDT.Rows)
            {
                var cardNbr = (string)row["CardNbr"];
                var customCd = (string)row["CustomCd"];
                var binRange = cardNbr.Substring(0,6);
                var shippingCd = GetShippingCode((string)row["ShippingMthd"]);
                var shippingAddr = (string)row["ShippingAddr"];
                createCard(cardNbr, binRange, customCd, shippingCd, shippingAddr, ++cardsCreated, ref details);
             }

            var footer = createFooter(dateTime, cardsCreated);
            var filname = writeTestFile(header, details, footer, dateTime);
            this._log.Write("Prepaid Test Card Creation Completed.");
            return filname;
        }

        private string GetShippingCode(string description)
        {
            foreach (ShippingMethod method in this.ShippingMethods)
                if (method.Description == description)
                    return method.Code;

            return null;
        }

        private string createHeader(DateTime dateTime)
        {
            var format = "1{0:yyyyMMddhhmm}COMDATA PLSD 00000000000000000000000000000000000000000000000000000000000000";
            var header = string.Format(format, dateTime);
            return header;
        }

        private void createCard(string cardNbr, string binRange, string customCd, string shippingCd, string shippingAddr, int cardsCreated, ref List<string> details)
        {
            Address.Parse(shippingAddr);
            var format = "2{0:000000}{1}   PREPAID OPERATIONS            PREPAID OPERATIONS/       {2}{3}                           {2}    {3}{4}130812110000100005764658693120815        {5,-100}{6,-100}{7,-32}{8,2}{9,-15}{10,-3}{11,-48}31600010        DUMMY ACCOUNT                   100167717                                           ";
            var detail = string.Format(format, cardsCreated, cardNbr, binRange, customCd, shippingCd
                                              , Address.Line1, Address.Line2, Address.City, Address.State, Address.PostalCd, Address.Country, Address.Name);
            details.Add(detail);
        }

        private string createFooter(DateTime dateTime, int cardsCreated)
        {
            var format = "3{0:yyyyMMddhhmm}{1:000000}COMDATA PLSD 00000000000000000000000000000000000000000000000000000000";
            var footer = string.Format(format, dateTime, cardsCreated);
            return footer;
        }

        private string writeTestFile(string header, List<string> details, string footer, DateTime dateTime)
        {
            var format = "";

            if (dateTime.Hour >= 12)
                format = "PPDPM_{0:yyyyMMdd_hhmm}";
            else
                format = "PPDAM_{0:yyyyMMdd_hhmm}";

            var filename = "";
            
            var overnightCutoffTS = DateTime.Parse(DateTime.Now.ToString("MM/dd/yyyy ") + OvernightCutOffHHMM);

            if (DateTime.Now < overnightCutoffTS)
                filename = Path.Combine(this.TestFileDestinationBeforeOvernightCutOff, string.Format(format, dateTime));
            else
                filename = Path.Combine(this.TestFileDestinationAfterOvernightCutOff, string.Format(format, dateTime));

            this._log.Write("Creating: {0}...", filename);
            this.Status = String.Format("Creating: {0}...", filename);

            using (StreamWriter sw = new StreamWriter(filename))
            {
                sw.WriteLine(header);

                foreach (string detail in details)
                    sw.WriteLine(detail);

                sw.WriteLine(footer);
            }

            return Path.GetFileName(filename);
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
