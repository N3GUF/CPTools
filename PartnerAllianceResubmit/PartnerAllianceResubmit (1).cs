using System;
using System.Collections.Generic;
using Comdata.AppSupport.AppTools;
using System.IO;
using System.Data;


namespace Comdata.AppSupport.PartnerAllianceResubmit
{
    class PartnerAllianceResubmit
    {
        private int cardsAssigned = 0;
        private int cardsNotAssigned = 0;
        private List<VCardAssignment> vCardAssignments;
        private string resubmitFilename = "";

        public string PS14Pathname { get; set; }
        public string PS15Pathname { get; set; }
        public DataTable AcctCustChangesDT { get; set; }
        public Logger Log { get; set; }
        public int CardsAssigned { get { return cardsAssigned; } }
        public int CardsNotAssigned { get { return cardsNotAssigned; } }

        internal PartnerAllianceResubmit()
        {
            this.vCardAssignments = new List<VCardAssignment>();
            this.AcctCustChangesDT = new DataTable();
            this.AcctCustChangesDT.Columns.Add(new DataColumn("OrigAcct", typeof(String)));
            this.AcctCustChangesDT.Columns.Add(new DataColumn("OrigCust", typeof(String)));
            this.AcctCustChangesDT.Columns.Add(new DataColumn("RequestedAcct", typeof(String)));
            this.AcctCustChangesDT.Columns.Add(new DataColumn("RequestedCust", typeof(String)));
        }

        internal bool CheckForErrors()
        {
            this.Log.Write(Severity.Debug, "Checking for Errors in: {0}", this.PS14Pathname);
            this.vCardAssignments = new List<VCardAssignment>();
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
                        var token = line.Substring(63, 14);
                        var message = line.Substring(920).Trim();
                        this.vCardAssignments.Add(new VCardAssignment(account, cust, token, message));
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error has occured while checking for errors.", ex);
            }

            if (this.cardsNotAssigned > 0)
            {
                this.listErrorsFound();
                return true;
            }
            else
                return false;
        }

        private void listErrorsFound()
        {
            Log.Write("");
            Log.Write("{0,-5} {1,-10} {2,-10} {3,-20} {4,-30}", " ", "Acct", "Cust", "Token", "Error Message");
            Log.Write("{0,-5} {1,-10} {2,-10} {3,-20} {4,-30}", " ", String.Empty.PadRight(5, '-')
                                                                   , String.Empty.PadRight(5, '-')
                                                                   , String.Empty.PadRight(14, '-')
                                                                   , String.Empty.PadRight(25, '-'));

            foreach (VCardAssignment error in this.vCardAssignments)
                Log.Write("{0,-5} {1,-10} {2,-10} {3,-20} {4,-30}", " ", error.Account
                                                                       , error.Customer
                                                                       , error.Token
                                                                       , error.Message);

            Log.Write("");
            Log.Write("Cards Assigned:     {0,10:n0}", this.CardsAssigned);
            Log.Write("Cards Not Assigned: {0,10:n0}", this.CardsNotAssigned);
            Log.Write("");
        }
 
        internal void ExtractErrors()
        {
            try
            {
                var tokensWritten = 0;
                var idx = 0;
                this.resubmitFilename = Path.GetFileName(this.PS14Pathname).Substring(0,20);
                this.resubmitFilename += DateTime.Now.ToString("MMddyyyy.hhmmssffff");
                this.resubmitFilename = Path.Combine(Path.GetDirectoryName(this.PS14Pathname), this.resubmitFilename);
                this.Log.Write("Creating: {0}", this.resubmitFilename);

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
                        var token = line.Substring(63, 14);

                        if (this.vCardAssignments.FindIndex(x => x.Token == token) > -1)    // Select only cards that have not been assigned
                        {
                            var result = this.AcctCustChangesDT.Select(string.Format("OrigAcct = '{0}' and OrigCust = '{1}'", account, cust));

                            if (result.Length > 0)                                           // Copy the record with the requested customer changes
                            {
                                this.Log.Write("Cust Id {0} will be changed to {1} for token {2}."
                                                                        , cust
                                                                        , result[0]["RequestedCust"]
                                                                        , token);
                                writter.WriteLine("{0}{1}{2}{3}{4}{5}", line.Substring(0, 3)
                                                                        , result[0]["RequestedAcct"]
                                                                        , result[0]["RequestedCust"]
                                                                        , line.Substring(13, 25)
                                                                        , result[0]["RequestedAcct"]
                                                                        , line.Substring(43));
                                tokensWritten++;
                            }
                            else                                                            // Copy the record with no changes
                            {
                                writter.WriteLine(line);
                                tokensWritten++;
                            }
                        }
                    }

                    reader.Close();
                    writter.Close();

                    if (tokensWritten != this.cardsNotAssigned)
                        throw new Exception("Records extrcted does not equal the unassigned Cards");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error has occured while extracting errors.", ex);
            }
        }

        private class VCardAssignment
        {
            public string Account { get; set; }
            public string Customer { get; set; }
            public string Token { get; set; }
            public string Message { get; set; }

            public VCardAssignment(string acct, string cust, string token, string message)
            {
                this.Account = acct;
                this.Customer = cust;
                this.Token = token;
                this.Message = message;
            }
        }
    }
}
