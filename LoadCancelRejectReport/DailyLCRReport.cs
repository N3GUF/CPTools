using System.Collections.Generic;
using System.Linq;
using System.Text;
using Comdata.AppSupport.AppTools;
using System.IO;
using System;

namespace Comdata.AppSupport.LoadCancelRejectReport
{
    class DailyLCRReport
    {
        public string BINPathname { get; set; }
        public string InputPathname { get; set; }
        public Logger Log { get; set; }
        public string EmailFileName
        {
            get { return this._emailFilename; }
        }

        private string _emailFilename;
        private List<string> _binNumbers = new List<string>();

        internal void CreateReport()
        {
            decimal totalCredits = 0;
            decimal totalDebits = 0;
            List<LoadCancel> credits = new List<LoadCancel>();
            List<LoadCancel> debits = new List<LoadCancel>();

            loadBinNumbers();
            var record = "";
            var parentContractName = "";
            _emailFilename = Path.Combine(Path.GetDirectoryName(this.InputPathname), "PPOLEmail.txt");

            if (File.Exists(_emailFilename))
                File.Delete(_emailFilename);

            using (StreamReader sr = new StreamReader(this.InputPathname))
                while ((record = sr.ReadLine()) != null)
                {
                    var recordType = record.Substring(0, 3);

                    if (recordType == "520")                         // Save accout Name
                        parentContractName = record.Substring(4, 35).Trim();

                    if (recordType != "621" && recordType != "631" && recordType != "626" && recordType != "636") // Skip all except credit or debits
                        continue;

                    var cardholderContract = record.Substring(12, 17).Trim();

                    if (cardholderContract.Length == 16 && this._binNumbers.Contains(cardholderContract.Substring(0, 6)))
                        cardholderContract = cardholderContract.Substring(0, 6) + "******" + cardholderContract.Substring(12, 4);

                    var amount = Math.Round(decimal.Parse(record.Substring(29, 10)) / 100, 2);
                    var empid = record.Substring(39, 15).Trim();
                    var name = record.Substring(54, 24).TrimEnd();

                    if (amount == 0)
                        continue;

                    if (recordType == "621" || recordType == "631") // This is a Credit
                    {
                        credits.Add(new LoadCancel(parentContractName, cardholderContract, empid, name, amount));
                        totalCredits += amount;
                    }
                    else                                             // This is a Debit
                    {
                        debits.Add(new LoadCancel(parentContractName, cardholderContract, empid, name, amount));
                        totalDebits += amount;
                    }
                }

            using (StreamWriter sw = new StreamWriter(_emailFilename))
            {
                sw.WriteLine("These are the amounts for the Payroll Reject File sent on {0:MM/dd/yyyy}.", DateTime.Today);
                this.Log.Write("These are the amounts for the Payroll Reject File sent on {0:MM/dd/yyyy}.", DateTime.Today);

                if (credits.Count > 0)
                {
                    sw.WriteLine("\r\nCredits:");
                    this.Log.Write("\r\nCredits:");

                    foreach (LoadCancel cancel in credits)
                    {
                        sw.WriteLine("\t{0,-17}\t{1,13:n}\t{2,-20}\t{3,-25}", cancel.CardholdrContract
                                                                            , cancel.Amount
                                                                            , cancel.EmployeeId
                                                                            , cancel.Name);
                        this.Log.Write("\t{0,-17}\t{1,13:n}\t{2,-20}\t{3,-25}", cancel.CardholdrContract
                                                                            , cancel.Amount
                                                                            , cancel.EmployeeId
                                                                            , cancel.Name);
                    }

                    sw.WriteLine("\r\nTotal Credits:\t\t{0,13:n}", totalCredits);
                    this.Log.Write("\r\nTotal Credits:\t\t{0,13:n}", totalCredits);
                }

                if (debits.Count > 0)
                {
                    sw.WriteLine("\r\nDebits:");
                    this.Log.Write("\r\nDebits:");

                    foreach (LoadCancel cancel in debits)
                    {
                        sw.WriteLine("\n{0}", cancel.ParentContractName);
                        this.Log.Write("\n{0}", cancel.ParentContractName);
                        sw.WriteLine("\t{0,-17}\t{1,13:n}\t{2,-20}\t{3,-25}", cancel.CardholdrContract
                                                                            , cancel.Amount
                                                                            , cancel.EmployeeId
                                                                            , cancel.Name);
                        this.Log.Write("\t{0,-17}\t{1,13:n}\t{2,-20}\t{3,-25}", cancel.CardholdrContract
                                                                            , cancel.Amount
                                                                            , cancel.EmployeeId
                                                                            , cancel.Name);
                    }

                    sw.WriteLine("\r\nTotal Debits:\t\t{0,13:n}", totalDebits);
                    this.Log.Write("\r\nTotal Debits:\t\t{0,13:n}", totalDebits);
                }

                if (totalCredits >= totalDebits)
                {
                    sw.WriteLine("\r\nFile Total Credits:\t{0,13:n}", totalCredits - totalDebits);
                    this.Log.Write("\r\nFile Total Credits:\t{0,13:n}", totalCredits - totalDebits);
                }
                else
                {
                    sw.WriteLine("\r\nFile Total Debits:\t{0,13:n}", totalDebits - totalCredits);
                    this.Log.Write("\r\nFile Total Debits:\t{0,13:n}", totalDebits - totalCredits);
                }
            }
        }

        private void loadBinNumbers()
        {
            if (this.BINPathname == "")
                return;

            this.Log.Write(Severity.Debug, "Loading BIN numbers from {0}.", this.BINPathname);
            var bin = "";

            using (StreamReader sr = new StreamReader(this.BINPathname))
                while ((bin = sr.ReadLine()) != null)
                {
                    bin = bin.Trim();
                    this._binNumbers.Add(bin);
                    Log.Write(Severity.Debug, "Added {0} to BIN list.", bin);
                }

            this.Log.Write("Loaded {0} BIN numbers from {1}.", this._binNumbers.Count, this.BINPathname);
        }

        class LoadCancel
        {
            public string  ParentContractName { get; set; }
            public string CardholdrContract { get; set; }
            public string EmployeeId { get; set; }
            public string Name { get; set; }
            public decimal Amount { get; set; }

            public LoadCancel(string parentContractName, string contract, string employeeId, string name, decimal amount)
            {
                this.ParentContractName = parentContractName;
                this.CardholdrContract = contract;
                this.EmployeeId = employeeId;
                this.Name = name;
                this.Amount = amount;
            }
        }
    }
}
