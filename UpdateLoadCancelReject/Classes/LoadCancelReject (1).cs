using Comdata.AppSupport.AppTools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Comdata.AppSupport.UpdateLoadCancelReject.Classes
{
    class LoadCancelReject
    {
        public string RejectData { get; private set; }
        public string ProcessedPath { get; set; }
        public string PostingDates { get; set; }
        public List<Credit> CreditList = new List<Credit>();

        private string[] _achRecords;

        int ct = 0;
        int lastCounter = 0;
        long totalDebits = 0;
        long totalCredits = 0;
        int numberOfEntries = 0;
        int numberOfSixes = 0;
        int howManyAllNineLinesNeeded = 0;
        int nines = 0;
        int fives = 0;

        bool isNew = false;

        string lastThree = string.Empty;
        string taxID = string.Empty;
        string lastSeven = string.Empty;
        string firstEightOfLastGroup = string.Empty;
        string cancelAmount = string.Empty;
        string contractNumber = string.Empty;
        string creditAmount = string.Empty;
        string fiveHolder = string.Empty;
        string fileContents = string.Empty;

        string[] fileDates;
        string[] contractNumbers;
        string[] creditAmounts;
        string[] na;
        ArrayList _rejectRecords = new ArrayList();
        ILog log;

        public LoadCancelReject(ILog log)
        {
            this.log = log;
        }

        public void LoadRejectData(string rejectFilename)
        {
            // populate the _rejectRecords and tbFileName screen objects
            this.log.Write("Loading reject data from {0}", Path.GetFileName(rejectFilename));
            this.RejectData = File.ReadAllText(rejectFilename, Encoding.Default);
        }

        public void LoadAchData(string postingDates, out string achFiles)
        {
            achFiles = "";
            var fileContents = "";

            // populate the the REGIONS files and tbFileDates screen objects
            DirectoryInfo di = new DirectoryInfo(this.ProcessedPath);
            FileInfo[] fInfo = di.GetFiles("REGIONS.PRD.ACH.*");

            foreach (FileInfo fi in fInfo)
            {
                if (!this.PostingDates.Contains(fi.LastWriteTime.Date.ToShortDateString()))
                    continue;

                achFiles += fi.Name + Environment.NewLine;
                // append the REGIONS file contents
                this.log.Write("Loading ACH data for {0} from {1}", fi.LastWriteTime.Date.ToShortDateString(), fi.Name);
                fileContents += File.ReadAllText(fi.FullName, Encoding.Default);
            }

            // split up the REGIONS files contents into a string array
            this._achRecords = fileContents.Split(Environment.NewLine.ToCharArray());
        }

        public int Update(string postingDatesList, string contractNumbersList
                        , string creditAmountsList, ref string rejectData)
        {
            this.CreditList.Clear();
             // get the dates, the contract numbers, and their amounts from the UI
            fileDates = postingDatesList.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            contractNumbers = contractNumbersList.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            creditAmounts = creditAmountsList.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            this.log.Write("\nThe following Credits need to be added:");
            this.log.Write("\tPostng Date\tCH Contract\tAmount");

            for (int i = 0; i <= contractNumbers.Length - 1; i++)
            {
                this.log.Write("\t{0}\t{1}\t{2}", fileDates[i], contractNumbers[i], creditAmounts[i]);
                this.CreditList.Add(new Credit(fileDates[i], contractNumbers[i], creditAmounts[i]));
            }

            this.log.Write("");

            // copy the file contents that were loaded into the screen's text box control (_rejectRecords) to an ArrayList object
            _rejectRecords.AddRange(rejectData.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries));

            // insert a blank line before the first "9" row as a flag or placeholder, and while you
            // are looping through the file lines, pick up (and increment) the last sequence number
            // for later use
            foreach (var item in _rejectRecords)
            {
                if (item.ToString().StartsWith("6") 
                    && (Int32.TryParse(item.ToString().Substring(item.ToString().Length - 7, 7), out lastCounter)))
                {
                    lastCounter++;
                };

                if (item.ToString().StartsWith("9000"))
                {
                    // store this position as you will use it later
                    ct = _rejectRecords.IndexOf(item);
                    // insert the blank line and bail now
                    _rejectRecords.Insert(ct, string.Empty);
                    break;
                }
            }

            try
            {
                // loop through all the contract numbers
                for (int i = 0; i < contractNumbers.Length; i++)
                {
                    // get the correct contract number that we will be using to search through the REGIONS files contents 
                    contractNumber = contractNumbers[i].ToString();

                    // get and reformat the related credit amount for this contract number
                    creditAmount = creditAmounts[i].ToString().Trim();
                    creditAmount = creditAmount.Replace(".", string.Empty);
                    creditAmount = creditAmount.PadLeft(10, '0');

                    // init a var for any "5" rows
                    fiveHolder = string.Empty;
                    var foundAchData = false;

                    // looping through REGIONS file contents array
                    foreach (var achRecord in this._achRecords)
                    {
                        // store any 5 rows as you scan
                        if (achRecord.StartsWith("5"))
                        {
                            fiveHolder = achRecord;
                        }

                        // does the contract number match?
                        if (achRecord.Contains(contractNumber))
                        {
                            // does the credit amount match?
                            if (achRecord.Substring(29, 10) == creditAmount)
                            {
                                // make sure you are not inserting a duplicate
                                if (_rejectRecords.IndexOf(achRecord) < 0)
                                {
                                    foundAchData = true;
                                    this.log.Write("Adding corresponding ACH data for Cardholder {0} and credit {1}", contractNumber, creditAmount);
                                    this.CreditList[i].Added = true;                                    // and insert the appropriate 5 and 6 row from this REGIONS
                                    // file while inserting a dummy 7 and 8 row, plus a blank line
                                    // for readability
                                    _rejectRecords.Insert(ct, "820000000200064206590000000000000000000161222364227403                         064206590000009");
                                    _rejectRecords.Insert(ct, "799R03064206597765322      06420659                                            064206597765322");
                                    _rejectRecords.Insert(ct, achRecord);
                                    _rejectRecords.Insert(ct, fiveHolder);
                                    _rejectRecords.Insert(ct, string.Empty);
                                    // and bail to the next REGIONS file
                                    break;
                                }
                            }
                        }
                    } // foreach line in REGIONS file

                    if (!foundAchData)
                    {
                        this.log.Write(Severity.Error, "Unable to find corresponding ACH data for Cardholder {0} and credit {1}", contractNumber, creditAmount);
                    }
                }

                 this.CreditList.RemoveAll(c => c.Added==true);

                if (this.CreditList.Count > 0)
                    return this.CreditList.Count;
            }
            catch (Exception ea)
            {
                throw new Exception("TagOne: " + ea.Message + " : " + ea.ToString());
            }


            // now that all the new credits or debits info has been added, loop through the new contents, 
            // adjusting strings and adding totals as you go
            na = (String[])_rejectRecords.ToArray(typeof(string));
            for (int i = 0; i < na.Length; i++)
            {
                // if you hit a blank line, you know that this is a newly inserted entry
                if (na[i].ToString().Length < 1)
                {
                    isNew = true;
                }

                // if a five record, get the last three chars and the tax id 
                if (isNew && na[i].StartsWith("5"))
                {
                    lastThree = na[i].Substring(na[i].Length - 3, 3);
                    taxID = na[i].Substring(40, 10);
                }

                // adjust the 6's
                if (isNew && na[i].StartsWith("6"))
                {
                    // get the last seven chars
                    lastSeven = na[i].Substring(na[i].Length - 7);

                    // update the sequence
                    na[i] = na[i].Substring(0, na[i].Length - 7) + lastCounter.ToString("D7");
                    lastCounter++;

                    // adjust the first three chars for CR or DB
                    if (na[i].StartsWith("622"))                    // Credit (deposit) to checking account Credit 
                        na[i] = "621" + na[i].Substring(3);
                    if (na[i].StartsWith("632"))                    // Credit to savings account 
                        na[i] = "631" + na[i].Substring(3);

                    // change pos. 78 to a "1"                      // Addenda Record Indicator
                    na[i] = na[i].Substring(0, 78) + "1" + na[i].Substring(79);

                    // get the cancel amount
                    cancelAmount = na[i].Substring(29, 10);
                }

                // adjust the 7's
                if (isNew && na[i].StartsWith("7"))
                {
                    // get the first eight chars of the last group
                    firstEightOfLastGroup = na[i].Substring(79, 8);

                    // use it to update the 6 and 5 lines
                    na[i - 1] = na[i - 1].Substring(0, 79) + firstEightOfLastGroup + na[i - 1].Substring(87);
                    na[i - 2] = na[i - 2].Substring(0, 79) + firstEightOfLastGroup + na[i - 2].Substring(87);

                    // adjust the first and last group of digits with lastSeven
                    na[i] = na[i].Substring(0, 14) + lastSeven + na[i].Substring(21);
                    na[i] = na[i].Substring(0, na[i].Length - 7) + lastSeven;
                }

                // adjust the 8's
                if (isNew && na[i].StartsWith("8"))
                {
                    // replace the last three chars
                    na[i] = na[i].Substring(0, na[i].Length - 3) + lastThree;

                    // insert the taxID
                    na[i] = na[i].Substring(0, 44) + taxID + na[i].Substring(54);

                    // zero out between 20 and 34
                    na[i] = na[i].Substring(0, 20) + "00000000000000" + na[i].Substring(34);

                    // insert the cancel amount
                    na[i] = na[i].Substring(0, 34) + cancelAmount + na[i].Substring(44);
                }

                // and regardless of isNew or not, total the credits and debits on the 6's
                // while counting them for use later
                if (na[i].StartsWith("6"))
                {
                    try
                    {
                        long amt = 0;
                        long.TryParse(na[i].Substring(29, 10), out amt);
                        if (na[i].StartsWith("621") || na[i].StartsWith("631"))
                            totalCredits = totalCredits + amt;
                        else
                            if (na[i].StartsWith("626") || na[i].StartsWith("636"))
                                totalDebits = totalDebits + amt;
                    }
                    catch (Exception ex2)
                    {
                        throw new Exception("TagTwo: " + ex2.Message + " : " + ex2.ToString());
                    }

                    numberOfSixes++;
                }

                // regardless of isNew or not, count the number of entries by counting 8's
                if (na[i].StartsWith("8"))
                {
                    try
                    {
                        int amt = 0;
                        Int32.TryParse(na[i].Substring(2, 8), out amt);
                        numberOfEntries += amt;
                    }
                    catch (Exception ex3)
                    {
                        throw new Exception("TagThree: " + ex3.Message + " : " + ex3.ToString());
                    }
                }

                // adjust the 9000 record
                if (na[i].StartsWith("9000"))
                {
                    // fix the total debit and credit fields                    
                    na[i] = na[i].Substring(0, 31) + totalDebits.ToString("D12") + na[i].Substring(43);
                    na[i] = na[i].Substring(0, 43) + totalCredits.ToString("D12") + na[i].Substring(55);

                    // fix the number of entries
                    na[i] = na[i].Substring(0, 13) + numberOfEntries.ToString("D8") + na[i].Substring(21);

                    // 6420659 * numberOfSixes and 
                    // check for zero before multiply
                    if (numberOfSixes == 0) numberOfSixes = 1;
                    na[i] = na[i].Substring(0, 21) + (numberOfSixes * 6420659).ToString("D10") + na[i].Substring(31);
                }
            }


            // refresh rejectData while removing the blank lines that were used as flags or placeholders, 
            // while counting fives, and count every line up to 9000
            rejectData = "";
            foreach (string s in na)
            {
                if (s.Length > 1)
                {
                    rejectData += s + Environment.NewLine;
                    nines++;
                    if (s.StartsWith("5"))
                        fives++;
                    if (s.StartsWith("9000"))
                    {
                        break;
                    }
                }
            }

            // now, start on the final adjustments...

            // first, how many "All Nines" rows will be needed? If any, add as many "all nines lines" to the text as calcd
            try
            {
                for (int i = nines; (i % 10) != 0; i++)
                {
                    howManyAllNineLinesNeeded = i;
                }
                // append them to the text box array contents
                if (howManyAllNineLinesNeeded > 0)
                {
                    // if there are 9999999 rows needed, the prior loop 
                    // will not have incremented howManyAllNineLinesNeeded in its final iteration,
                    // so adjust for that now...
                    howManyAllNineLinesNeeded++;
                    // now, subtract the nines count, giving the true number of rows needed
                    howManyAllNineLinesNeeded = howManyAllNineLinesNeeded - nines;
                    // and finally append them
                    for (int i = howManyAllNineLinesNeeded; i != 0; i--)
                    {
                        rejectData += "9999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999" + Environment.NewLine;
                    }
                }
            }
            catch (Exception ex4)
            {
                throw new Exception("TagFour: " + ex4.Message + " : " + ex4.ToString());
            }

            // the _rejectRecords.Lines array elements are read-only and/or the Text property is not auto-refreshed, 
            // so for easier manipulation, move the contents to the ArrayList object
            _rejectRecords.Clear();
            _rejectRecords.AddRange(rejectData.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries));

            // and do the final manipulation of the "9000" row...
            try
            {
                for (int i = 0; (i < _rejectRecords.Count); i++)
                {
                    if (_rejectRecords[i].ToString().StartsWith("9000"))
                    {
                        howManyAllNineLinesNeeded = (nines + howManyAllNineLinesNeeded) / 10;
                        _rejectRecords[i] = _rejectRecords[i].ToString().Substring(0, 1) + fives.ToString("D6") + _rejectRecords[i].ToString().Substring(7);
                        _rejectRecords[i] = _rejectRecords[i].ToString().Substring(0, 7) + howManyAllNineLinesNeeded.ToString("D6") + _rejectRecords[i].ToString().Substring(13);
                    }
                }
            }
            catch (Exception ex5)
            {
                throw new Exception("TagFive: " + ex5.Message + " : " + ex5.ToString());
            }

            StringBuilder sb = new StringBuilder();
            var count = 1;

            foreach (string record in this._rejectRecords)
                if (count++ == this._rejectRecords.Count)
                    sb.Append(record);
                else
                    sb.AppendLine(record);

            this.RejectData = sb.ToString();
            return 0;
        }
       
        public class Credit
        {
            private string _amount;
            public string PostingDate{ get; set; }
            public string CardholderContract { get; set; }
            public bool Added { set; get; }

            public string Amount 
            {
                get
                { 
                    return _amount; 
                }
                set 
                {
                    this._amount = value;
                    this.DecimalAmount = decimal.Parse(value);
                    this.DecimalAmount = Math.Round(this.DecimalAmount / 100, 2);
                } 
            }
            public decimal DecimalAmount { private set; get; }

            public Credit(string postingDate, string cardHolderContract, string amount)
            {
                this.PostingDate = postingDate;
                this.CardholderContract = cardHolderContract;
                this.Amount = amount;
                this.Added = false;
            }
        }
    }
}
