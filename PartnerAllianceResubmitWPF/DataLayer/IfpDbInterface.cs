using System;
using System.Data;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace PartnerAllianceResubmitWPF.DataLayer
{
    class IfpDbInterface
    {
        private static string _connString = null;

        public IfpDbInterface(string locationOfConnString)
        {
            if (_connString != null)
                return;

            if (System.IO.File.Exists(locationOfConnString))
                using (System.IO.StreamReader sr = new System.IO.StreamReader(locationOfConnString))
                    while ((_connString = sr.ReadLine()) != null)
                        return;
        }

        public string GetArchiveNameFromAudit(long jobId)
        {
            string filename = "";
            DataTable messagesDT = new DataTable();
            StringBuilder sb = new StringBuilder();
            sb.Append("select message from audit with (nolock)");
            sb.AppendFormat(@" where fileid = {0} and (message like 'Archiving P:\\INPUT\CLAIMS%'", jobId);
            sb.Append(@" or message like 'Creating P:\Output\Archive% for archival purposes')");

            using (OdbcConnection conn = new OdbcConnection(_connString))
            using (OdbcDataAdapter da = new OdbcDataAdapter(sb.ToString(), conn))
            {
                da.SelectCommand.CommandTimeout = 180;

                try
                {
                    da.Fill(messagesDT);
                }

                catch (Exception ex)
                {
                    throw new Exception("An error has Occrured while getting the archive filename.", ex);
                }
            }

            string message = "";
            string[] items = { "" };

            foreach (DataRow dr in messagesDT.Rows)
            {
                message = dr["message"].ToString().Trim();
                items = dr["message"].ToString().Split(new char[] { ' ' });
                break;
            }

            if (message.StartsWith(@"Archiving P:\\INPUT\CLAIMS\") && items.Count() > 4)
            {
                filename = items[3].Trim();
            }
            else
            if (message.StartsWith(@"Creating P:\Output\Archive") && items.Count() > 2)
            {
                filename = items[1].Trim();
            }

            return filename;
        }

        public IList<long> GetDistnctJobs(IList<string> corns)
        {
            IList<long> jobs = new List<long>();            
            DataTable jobsDT = new DataTable();
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT DISTINCT f.IFPJobID FROM  Claims c with (nolock) ");
            sb.Append("JOIN SubmitterClaimFiles f with (nolock) on c.SubmitterFileID = f.ID ");
            sb.Append("WHERE c.Corn in ");
            int count = 0;

            foreach (string corn in corns)
                if (count++ == 0)
                    sb.AppendFormat("('{0}'", corn);
                else
                    sb.AppendFormat(", '{0}'", corn);

            sb.Append(")");
            using (OdbcConnection conn = new OdbcConnection(_connString))
            using (OdbcDataAdapter da = new OdbcDataAdapter(sb.ToString(), conn))
            {
                da.SelectCommand.CommandTimeout = 180;

                try
                {
                    da.Fill(jobsDT);
                }

                catch (Exception ex)
                {
                    throw new Exception("An error has Occrured while getting the job ID's.", ex);
                }
            }

            foreach (DataRow dr in jobsDT.Rows)
                jobs.Add(long.Parse(dr["IFPJobID"].ToString().Trim()));
 
            return jobs;
        }
    }
}
