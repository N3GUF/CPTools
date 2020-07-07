using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CreateRejectFile
{
    public partial class formCreateRejectFile : Form
    {
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
        string[] fileLines;
        string[] na;

        ArrayList al = new ArrayList();


        public formCreateRejectFile()
        {
            InitializeComponent();
        }

        /// <summary>
        /// click handler for Browse button, opens file dialog for loading the reject file, and loads any REGIONS files in the working dir
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        private void btnBrowse_Click(object sender, EventArgs e)
        {            
            openFileDialog1.FileName = string.Empty;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                // populate the tbRejectFile and tbFileName screen objects
                tbRejectFile.Text = string.Empty;
                tbRejectFile.Text = File.ReadAllText(openFileDialog1.FileName, Encoding.Default);
                tbFileName.Text = openFileDialog1.FileName.ToString();

                // populate the tbREGIONSFiles and tbFileDates screen objects
                DirectoryInfo di = new DirectoryInfo(Directory.GetCurrentDirectory().ToString());
                FileInfo[] fInfo = di.GetFiles("REGIONS*");
                foreach (FileInfo fi in fInfo)
                {
                    tbREGIONSFiles.AppendText(fi.Name + Environment.NewLine);
                    tbFileDates.AppendText(fi.LastWriteTime.Date.ToShortDateString() + Environment.NewLine);
                    // append the REGIONS file contents
                    fileContents += File.ReadAllText(fi.FullName, Encoding.Default);
                }

                // split up the REGIONS files contents into a string array
                fileLines = fileContents.Split(Environment.NewLine.ToCharArray());
            }
        }


        /// <summary>
        /// click handler for Create button and main processing logic
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        private void btnCreate_Click(object sender, EventArgs e)
        {
            // get the dates, the contract numbers, and their amounts from the UI
            fileDates = tbFileDates.Text.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            contractNumbers = tbContractNumbers.Text.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            creditAmounts = tbCreditAmounts.Text.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            // copy the file contents that were loaded into the screen's text box control (tbRejectFile) to an ArrayList object
            al.AddRange(tbRejectFile.Lines);

            // insert a blank line before the first "9" row as a flag or placeholder, and while you
            // are looping through the file lines, pick up (and increment) the last sequence number
            // for later use
            foreach (var item in al)
            {
                if (item.ToString().StartsWith("6") && (Int32.TryParse(item.ToString().Substring(item.ToString().Length - 7, 7), out lastCounter)))
                {
                    lastCounter++;
                };
                    
                if (item.ToString().StartsWith("9000")) 
                {
                    // store this position as you will use it later
                    ct = al.IndexOf(item);
                    // insert the blank line and bail now
                    al.Insert(ct, string.Empty);
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

                    // looping through REGIONS file contents array
                    foreach (var lineContents in fileLines)
                    {
                        // store any 5 rows as you scan
                        if (lineContents.StartsWith("5"))
                        {
                            fiveHolder = lineContents;
                        }

                        // does the contract number match?
                        if (lineContents.Contains(contractNumber))
                        {
                            // does the credit amount match?
                            if (lineContents.Substring(29, 10) == creditAmount)
                            {
                                // make sure you are not inserting a duplicate
                                if (al.IndexOf(lineContents) < 0)
                                {
                                    // and insert the appropriate 5 and 6 row from this REGIONS
                                    // file while inserting a dummy 7 and 8 row, plus a blank line
                                    // for readability
                                    al.Insert(ct, "820000000200064206590000000000000000000161222364227403                         064206590000009");
                                    al.Insert(ct, "799R03064206597765322      06420659                                            064206597765322");
                                    al.Insert(ct, lineContents);
                                    al.Insert(ct, fiveHolder);
                                    al.Insert(ct, string.Empty);
                                    // and bail to the next REGIONS file
                                    break;
                                }
                            }
                        }
                    } // foreach line in REGIONS file
                }
            }
            catch (Exception ea)
            {
                ShowDialog("TagOne: " + ea.Message + " : " + ea.ToString());
            }


            // now that all the new credits or debits info has been added, loop through the new contents, 
            // adjusting strings and adding totals as you go
            na = (String[])al.ToArray(typeof(string));
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
                    na[i] = na[i].Substring(0, na[i].Length - 7) + lastCounter.ToString();
                    lastCounter++;

                    // adjust the first three chars for CR or DB
                    if (na[i].StartsWith("622"))
                        na[i] = "621" + na[i].Substring(3);
                    if (na[i].StartsWith("632"))
                        na[i] = "631" + na[i].Substring(3);

                    // change pos. 78 to a "1"
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
                        ShowDialog("TagTwo: " + ex2.Message + " : " + ex2.ToString());
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
                        ShowDialog("TagThree: " + ex3.Message + " : " + ex3.ToString());
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

            
            // refresh tbRejectFile while removing the blank lines that were used as flags or placeholders, 
            // while counting fives, and count every line up to 9000
            tbRejectFile.Clear();          
            foreach (string s in na)
            {
                if (s.Length > 1)
                {
                    tbRejectFile.AppendText(s + Environment.NewLine);
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
                        tbRejectFile.AppendText("9999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999" + Environment.NewLine);
                    }
                }
            }
            catch (Exception ex4)
            {
                ShowDialog("TagFour: " + ex4.Message + " : " + ex4.ToString());
            }

            // the tbRejectFile.Lines array elements are read-only and/or the Text property is not auto-refreshed, 
            // so for easier manipulation, move the contents to the ArrayList object
            al.Clear();
            al.AddRange(tbRejectFile.Lines);

            // and do the final manipulation of the "9000" row...
            try
            {
                for (int i = 0; (i < al.Count); i++)
                {
                    if (al[i].ToString().StartsWith("9000"))
                    {
                        howManyAllNineLinesNeeded = (nines + howManyAllNineLinesNeeded) / 10;
                        al[i] = al[i].ToString().Substring(0, 1) + fives.ToString("D6") + al[i].ToString().Substring(7);
                        al[i] = al[i].ToString().Substring(0, 7) + howManyAllNineLinesNeeded.ToString("D6") + al[i].ToString().Substring(13);
                    }
                }
            }
            catch (Exception ex5)
            {
                ShowDialog("TagFive: " + ex5.Message + " : " + ex5.ToString());
            }

            // finally, do the final refresh of the Text property from the ArrayList, repositioning the cursor at TOF
            tbRejectFile.Clear();
            tbRejectFile.Lines = (String[])al.ToArray(typeof(string));
            tbRejectFile.Text.TrimEnd(Environment.NewLine.ToCharArray());
        }


        /// <summary>
        /// error dialog box
        /// </summary>
        /// <param name="p">string</param>
        private void ShowDialog(string p)
        {
            Console.WriteLine(p);            
            MessageBox.Show(p, "CreateRejectFile Utility",MessageBoxButtons.OK, MessageBoxIcon.Error);
            throw new NotImplementedException();
        }

        #region

        // depricated:
        // commented 11/18/2013 when I changed it to not require 1-to-1 files per contract
        //            string[] fileEntries;
        //            fileEntries = Directory.GetFiles(Directory.GetCurrentDirectory().ToString());

        //// looping through the working directory contents looking for REGIONS files...
        //try 
        //    {
        //        for (int i = 0; i < fileEntries.Length; i++)                    
        //        {
        //            if (fileEntries[i].ToString().Contains("REGIONS"))
        //            {
        //                // get the correct contract number that we will be using to search through this REGIONS file 
        //                contractNumber = contractNumbers[idx].ToString();

        //                // get and reformat the related credit amount for this contract number
        //                creditAmount = creditAmounts[idx].ToString().Trim();

        //                // TODO: fix this:  if they have not formatted as a decimal value...  i.e. "5" vs "5.00"
        //                // if the amount is not expressed as a decimal value, assume it is whole dollars
        //                //if (creditAmount.Contains(".") != true)
        //                //{
        //                //    creditAmount = creditAmount.TrimEnd() + "00";
        //                //}

        //                creditAmount = creditAmount.Replace(".", string.Empty);
        //                creditAmount = creditAmount.PadLeft(10, '0');

        //                // init a var for any "5" rows
        //                fiveHolder = string.Empty;

        //                // load this particular REGIONS file and split into an array
        //                fileLines = File.ReadAllText(fileEntries[i].ToString(), Encoding.Default).Split(Environment.NewLine.ToCharArray());
        //                // DEBUG: TODO: here is where you simply use the fileLines array as loaded in the other method 
        //                // versus ReadAllText/Split here...  fileLines would have all the REGIONS file(s) lines...  and 
        //                // you would not have to have that one-to-ne thing between contracts and REGIONS files...
        //                // so you would comment out this fileLines assignment, and this:
        //                // for (int i = 0; i < fileEntries.Length; i++)
        //                // would now be:
        //                // for (int i = 0; i < contractNumbers.Length; i++)

        //                // looping through array
        //                foreach (var lineContents in fileLines)
        //                {
        //                    // store any 5 rows as you scan
        //                    if (lineContents.StartsWith("5"))
        //                    {
        //                        fiveHolder = lineContents;
        //                    }

        //                    // does the contract number match?
        //                    if (lineContents.Contains(contractNumber))
        //                    {
        //                        // does the credit amount match?
        //                        if (lineContents.Substring(29,10) == creditAmount)
        //                        {
        //                            // make sure you are not inserting a duplicate
        //                            if (al.IndexOf(lineContents) < 0)
        //                            {
        //                                // and insert the appropriate 5 and 6 row from this REGIONS
        //                                // file while inserting a dummy 7 and 8 row, plus a blank line
        //                                // for readability
        //                                al.Insert(ct, "820000000200064206590000000000000000000161222364227403                         064206590000009");
        //                                al.Insert(ct, "799R03064206597765322      06420659                                            064206597765322");
        //                                al.Insert(ct, lineContents);
        //                                al.Insert(ct, fiveHolder);
        //                                al.Insert(ct, string.Empty);
        //                                // and bail to the next REGIONS file
        //                                // TODO: revise this for multiple credits in the same REGIONS file(s)...
        //                                break;
        //                            }
        //                        }
        //                    }
        //                } // foreach line in REGIONS file

        //                // since you are moving to the next REGIONS file,
        //                // increment the index used to get the next contract, dates, and amounts
        //                idx++;
        //            }
        //        }		
        //    }
        //catch (Exception ex)
        //    {
        //        ShowDialog("TagOne: " + ex.Message + " : " + ex.ToString());
        //    };
        // end of 11/18/2013 commmented

        #endregion

    }
}
