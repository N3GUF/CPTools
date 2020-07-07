using System.Collections.Generic;
using System.Linq;
using System.Text;
using Comdata.AppSupport.AppTools;
using System.IO;
using System;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace Comdata.AppSupport.UpdateLoadCancelReject.Classes
{
    class DailyLCRReport
    {
        public string BINPathname { get; set; }
        public string EmailFileName
        {
            get { return this._emailFilename; }
        }

        private string _emailFilename;
        private List<string> _binNumbers = new List<string>();
        private DAL.IDAL _db;
        private ILog _log;

        public DailyLCRReport(ILog log, DAL.IDAL db, string binNumbersPathname, string emailPathname)
        {
            this._db = db;
            this.BINPathname = binNumbersPathname;

            if (this.BINPathname.StartsWith(@".\"))
                this.BINPathname=Path.Combine(Environment.CurrentDirectory, Path.GetFileName(this.BINPathname));

            this._emailFilename = emailPathname;

            if (this._emailFilename.StartsWith(@".\"))
                this._emailFilename = Path.Combine(Environment.CurrentDirectory, Path.GetFileName(this._emailFilename));

            this._log = log;
        }

        internal void CreateReport(string [] records)
        {
            decimal totalCredits = 0;
            decimal totalDebits = 0;
            List<LoadCancel> credits = new List<LoadCancel>();
            List<LoadCancel> debits = new List<LoadCancel>();
            var prevParentContractName = "";
  
            loadBinNumbers();
            var parentContract = "";
            var parentContractName = "";
            this._db.Open();

            if (File.Exists(_emailFilename))
                File.Delete(_emailFilename);

            foreach (var record in records)
            {
                if (record.Length == 0)
                    continue;

                var recordType = record.Substring(0, 3);

                if (recordType == "520")                         // Save accout Name
                {
                    parentContractName = record.Substring(4, 35).Trim();
                    continue;
                }

                if (recordType != "621" && recordType != "631" && recordType != "626" && recordType != "636") // Skip all except credit or debits
                    continue;

                var cardholderContract = record.Substring(12, 17).Trim();
                var amount = Math.Round(decimal.Parse(record.Substring(29, 10)) / 100, 2);
                var empid = record.Substring(39, 15).Trim();
                var name = record.Substring(54, 24).TrimEnd();

                if (parentContractName != prevParentContractName)
                {
                    this._db.GetParentContractInfo(cardholderContract, ref parentContract, ref parentContractName);
                    prevParentContractName = parentContractName;
                }

                if (amount == 0)
                    continue;

                if (recordType == "621" || recordType == "631") // This is a Credit
                {
                    credits.Add(new LoadCancel(parentContract, parentContractName, cardholderContract, empid, name, amount));
                    totalCredits += amount;
                }
                else                                             // This is a Debit
                {
                    debits.Add(new LoadCancel(parentContract, parentContractName, cardholderContract, empid, name, amount));
                    totalDebits += amount;
                }
            }

            this._db.Close();
            debits.Sort();
            credits.Sort();

            using (StreamWriter sw = new StreamWriter(_emailFilename))
            {
                sw.WriteLine("These are the amounts for the Payroll Reject File sent on {0:MM/dd/yyyy}.", DateTime.Today);
                this._log.Write("\r\nThese are the amounts for the Payroll Reject File sent on {0:MM/dd/yyyy}.", DateTime.Today);

                if (credits.Count > 0)
                {
                    sw.WriteLine("\r\nCredits:");
                    this._log.Write("\r\nCredits:");
                    prevParentContractName = "";

                    foreach (LoadCancel cancel in credits)
                    {
                        if (prevParentContractName.CompareTo(cancel.ParentContractName) != 0)
                        {
                            sw.WriteLine("\n{0}\t{1}", cancel.ParentContract, cancel.ParentContractName);
                            this._log.Write("\n{0}\t{1}", cancel.ParentContract, cancel.ParentContractName);
                        }

                        prevParentContractName = cancel.ParentContractName;
                        sw.WriteLine("\t{0,-17}\t{1,13:n}\t{2,-20}\t{3,-25}", mask(cancel.CardholderContract)
                                                                            , cancel.Amount
                                                                            , cancel.EmployeeId
                                                                            , cancel.Name);
                        this._log.Write("\t{0,-17}\t{1,13:n}\t{2,-20}\t{3,-25}", cancel.CardholderContract
                                                                            , cancel.Amount
                                                                            , cancel.EmployeeId
                                                                            , cancel.Name);
                    }

                    sw.WriteLine("\r\nTotal Credits:\t\t\t{0,13:n}", totalCredits);
                    this._log.Write("\r\nTotal Credits:\t\t\t{0,13:n}", totalCredits);
                }

                if (debits.Count > 0)
                {
                    sw.WriteLine("\r\nDebits:");
                    this._log.Write("\r\nDebits:");
                    prevParentContractName = "";

                    foreach (LoadCancel cancel in debits)
                    {
                        if (prevParentContractName.CompareTo(cancel.ParentContractName) != 0)
                        {
                            sw.WriteLine("\n{0}\t{1}", cancel.ParentContract, cancel.ParentContractName);
                            this._log.Write("\n{0}\t{1}", cancel.ParentContract, cancel.ParentContractName);
                        }

                        prevParentContractName = cancel.ParentContractName;
                        sw.WriteLine("\t{0,-17}\t{1,13:n}\t{2,-20}\t{3,-25}", mask(cancel.CardholderContract)
                                                                            , cancel.Amount
                                                                            , cancel.EmployeeId
                                                                            , cancel.Name);
                        this._log.Write("\t{0,-17}\t{1,13:n}\t{2,-20}\t{3,-25}", cancel.CardholderContract
                                                                            , cancel.Amount
                                                                            , cancel.EmployeeId
                                                                            , cancel.Name);
                    }

                    sw.WriteLine("\r\nTotal Debits:\t\t\t{0,13:n}", totalDebits);
                    this._log.Write("\r\nTotal Debits:\t\t\t{0,13:n}", totalDebits);
                }

                if (totalCredits >= totalDebits)
                {
                    sw.WriteLine("\r\nFile Total Credits:\t\t{0,13:n}", totalCredits - totalDebits);
                    this._log.Write("\r\nFile Total Credits:\t\t{0,13:n}", totalCredits - totalDebits);
                }
                else
                {
                    sw.WriteLine("\r\nFile Total Debits:\t\t{0,13:n}", totalDebits - totalCredits);
                    this._log.Write("\r\nFile Total Debits:\t\t{0,13:n}", totalDebits - totalCredits);
                }
            }
        }

        private string mask(string card)
        {
            if (card.Length==16)
                if (this._binNumbers.Contains(card.Substring(0,6)))
                    card = card.Substring(0,4) + "********" + card.Substring(12,4);

            return card;
        }

        private void loadBinNumbers()
        {
            this._binNumbers.Clear();

            if (this.BINPathname == "")
                return;

            if (!File.Exists(this.BINPathname))
            {
                this._log.Write(Severity.Debug, "{0} doesn't exist.  No BIN ranges to load.");
                return;
            }

            this._log.Write(Severity.Debug, "\r\nLoading BIN numbers from {0}.", this.BINPathname);
            var bin = "";

            using (StreamReader sr = new StreamReader(this.BINPathname))
                while ((bin = sr.ReadLine()) != null)
                {
                    bin = bin.Trim();
                    this._binNumbers.Add(bin);
                    _log.Write(Severity.Debug, "Added {0} to BIN list.", bin);
                }

            this._log.Write("Loaded {0} BIN numbers from {1}.", this._binNumbers.Count, this.BINPathname);
        }

        class LoadCancel : IComparable<LoadCancel>
        {
            public string ParentContract { get; set; }
            public string ParentContractName { get; set; }
            public string CardholderContract { get; set; }
            public string EmployeeId { get; set; }
            public string Name { get; set; }
            public decimal Amount { get; set; }

            public LoadCancel(string parentContract, string parentContractName, string contract, string employeeId, string name, decimal amount)
            {
                this.ParentContract = parentContract;
                this.ParentContractName = parentContractName;
                this.CardholderContract = contract;
                this.EmployeeId = employeeId;
                this.Name = name;
                this.Amount = amount;
            }

            public int CompareTo(LoadCancel other)
            {
                var thisOne = this.ParentContract + this.CardholderContract;
                var otherOne = other.ParentContract + other.CardholderContract;
                return thisOne.CompareTo(otherOne);
            }
        }
    }
}
