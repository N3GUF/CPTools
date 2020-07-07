using System;
using System.Collections.Generic;
using AppTools;
using System.IO;
using System.Data;


namespace Comdata.AppSupport.PartnerAllianceResubmit
{
    class PartnerAllianceResubmit
    {
        #region Private Properties
        private int cardsAssigned = 0;
        private int cardsNotAssigned = 0;
        private int cardsExtracted = 0;
        private List<VCardAssignment> vCardAssignments;
        private string resubmitFilename = "";
        #endregion

        #region Public Properties
        public string PS14Pathname { get; set; }
        public string PS15Pathname { get; set; }
        public DataTable AcctCustChangesDT { get; set; }
        public Logger Log { get; set; }
        public int CardsAssigned { get { return cardsAssigned; } }
        public int CardsNotAssigned { get { return cardsNotAssigned; } }
        public int CardsExtracted { get { return cardsExtracted; } }
        public string ResubmitFilename { get { return resubmitFilename; } }
        #endregion

        #region Public Methods
        internal PartnerAllianceResubmit()
        {
            this.vCardAssignments = new List<VCardAssignment>();
            this.AcctCustChangesDT = new DataTable();
        }

        internal bool CheckForErrors()
        {
            this.Log.Write(Severity.Debug, "Checking for Errors in: {0}", Path.GetFileName(this.PS14Pathname));
            this.vCardAssignments.Clear();
            this.AcctCustChangesDT.Clear();
            this.cardsAssigned = 0;
            this.cardsNotAssigned = 0;

            try
            {
                using (StreamReader reader = new StreamReader(this.PS15Pathname))
                {
                    string line;

                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.Contains("HEADER") || line.Contains("TRAILER"))
                            continue;

                        if (line.Contains("CARD ASSIGNED"))
                        {
                            this.cardsAssigned++;
                            continue;
                        }

                        this.cardsNotAssigned++;
                        var account = line.Substring(3, 5);
                        var cust = line.Substring(8, 5);
                        var codeword = line.Substring(38, 10);
                        var token = line.Substring(63, 14);
                        var message = line.Substring(920).Trim();
                        this.vCardAssignments.Add(new VCardAssignment(account, cust, token, message, codeword));
                        var result = this.AcctCustChangesDT.Select(string.Format("OrigAcct = '{0}' and OrigCust = '{1}'", account, cust));

                       if (result.Length == 0)                                           // Copy the record with the requested customer changes
                           this.AcctCustChangesDT.Rows.Add(account, cust, "", "");
                    }

                    reader.Close();
                    this.AcctCustChangesDT.AcceptChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error has occurred while checking for errors.", ex);
            }

            if (this.cardsNotAssigned > 0)
            {
                this.listErrorsFound();
                return true;
            }
            else
                return false;
        }
 
        internal void ExtractErrors()
        {
            try
            {
                editAccountCustChanges();
                cardsExtracted = 0;
                this.resubmitFilename = Path.GetFileName(this.PS14Pathname).Substring(0,20);
                this.resubmitFilename += DateTime.Now.ToString("MMddyyyy.HHmmssffff");
                this.resubmitFilename = Path.Combine(Path.GetDirectoryName(this.PS14Pathname), this.resubmitFilename);
                this.Log.Write("Creating: {0}", Path.GetFileName(this.resubmitFilename));

                using (StreamReader reader = new StreamReader(this.PS14Pathname))
                using (StreamWriter writter = new StreamWriter(this.resubmitFilename))
                {
                    string line;


                    while ((line = reader.ReadLine()) != null)                          // Lood through all PS14 records
                    {
                        if (line.StartsWith("H") || line.StartsWith("**FTP"))           // Copy the header record or FTP record     
                        {
                            writter.WriteLine(line);
                            continue;
                        }

                        if (line.StartsWith("T"))                                       // Create a new trailer record
                        {
                            line = String.Format("{0}{1:D7}", line.Substring(0, 37), this.cardsNotAssigned + 2);
                            writter.WriteLine(line);
                            continue;
                        }

                        var account = line.Substring(3, 5);
                        var cust = line.Substring(8, 5);
                        var codeword = line.Substring(38, 10); 
                        var token = line.Substring(63, 14);

                        if (this.vCardAssignments.FindIndex(x => x.Token == token) > -1)    // Select only cards that have not been assigned
                        {
                            var result = this.AcctCustChangesDT.Select(string.Format("OrigAcct = '{0}' and OrigCust = '{1}'", account, cust));

                            if (result.Length > 0)                                           // Copy the record with the requested customer changes
                            {
                                this.Log.Write("Acct/Cust Id {0}/{1} will be changed to {2}/{3} for token {4}." , account
                                                                                                                , cust
                                                                                                                , result[0]["ReqAcct"]
                                                                                                                , result[0]["ReqCust"]
                                                                                                                , token);
                                account = (string) result[0]["ReqAcct"];
                                cust = (string) result[0]["ReqCust"];
                            }

                            if (codeword != account + "VCARD")
                            {
                                this.Log.Write("Codeword {0} will be changed to {1} for token {2}.", codeword
                                                                                                   , account + "VCARD"
                                                                                                   , token);
                                codeword = account + "VCARD";
                            }

                            writter.WriteLine("{0}{1}{2}{3}{4}{5}{6}{7}", line.Substring(0, 3)
                                            , account
                                            , cust
                                            , line.Substring(13, 25)
                                            , codeword
                                            , line.Substring(48, 15)
                                            , token
                                            , line.Substring(77));
                            cardsExtracted++;
                        }
                    }

                    reader.Close();
                    writter.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error has occurred while extracting errors.", ex);
            }
        }
        #endregion

        #region Private Methods
        private void editAccountCustChanges()
        {
            foreach (DataRow row in this.AcctCustChangesDT.Rows)
            {
                if (row["ReqAcct"].ToString() == "" && row["ReqCust"].ToString() == "")
                {
                    row.Delete();
                    continue;
                }

                if (row["ReqAcct"].ToString() == "" && row["ReqCust"].ToString() != "")
                    row["ReqAcct"] = row["OrigAcct"];

                if (row["ReqAcct"].ToString() != "" && row["ReqCust"].ToString() == "")
                    row["ReqCust"] = row["OrigCust"];
            }

            this.AcctCustChangesDT.AcceptChanges();
        }

        private void listErrorsFound()
        {
            Log.Write("");
            Log.Write("{0,-5} {1,-10} {2,-10} {3,-20} {4,-15} {5,-30}", " ", "Acct", "Cust", "Token", "Code Word", "Error Message");
            Log.Write("{0,-5} {1,-10} {2,-10} {3,-20} {4,-15} {5,-30}", " ", String.Empty.PadRight(5, '-')
                                                                           , String.Empty.PadRight(5, '-')
                                                                           , String.Empty.PadRight(14, '-')
                                                                           , String.Empty.PadRight(11, '-')
                                                                           , String.Empty.PadRight(25, '-'));

            foreach (VCardAssignment error in this.vCardAssignments)
                Log.Write("{0,-5} {1,-10} {2,-10} {3,-20} {4,-15} {5,-30}", " ", error.Account
                                                                               , error.Customer
                                                                               , error.Token
                                                                               , error.CodeWord
                                                                               , error.Message);

            Log.Write("");
            Log.Write("Cards Assigned:     {0,10:n0}", this.CardsAssigned);
            Log.Write("Cards Not Assigned: {0,10:n0}", this.CardsNotAssigned);
            Log.Write("");
        }
        #endregion

        private class VCardAssignment
        {
            public string Account { get; set; }
            public string Customer { get; set; }
            public string Token { get; set; }
            public string Message { get; set; }
            public string CodeWord { get; set; }

            public VCardAssignment(string acct, string cust, string token, string message, string cordWord)
            {
                this.Account = acct;
                this.Customer = cust;
                this.Token = token;
                this.Message = message;
                this.CodeWord = cordWord;
            }
        }
    }
}
