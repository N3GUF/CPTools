using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Comdata.AppSupport.UpdateLoadCancelReject.DAL
{
    public class OpenWayDB :IDAL
    {
        private string _connectionString;
        OracleConnection _connection;

        public string Username { set; get; }
        public string Password { set; get; }

        public bool ConnectionIsReady 
        {
            get
            {
                if (this.Open())
                {
                    this.Close();
                    return true;
                }
                else
                    return false;
            }
        }

        public OpenWayDB(string connectionString)
        {
            this._connectionString = connectionString;
        }

        public bool Open()
        {
            var connectionString = _connectionString;
            connectionString = connectionString.Replace("myUsername", this.Username);
            connectionString = connectionString.Replace("myPassword", this.Password);
            this._connection=new OracleConnection(connectionString); 
            this._connection.Open();

            if (_connection.State == ConnectionState.Open)
                return true;
            else
            {
                this._connection = null;
                return false;
            }
        }

        public void Close()
        {
            this._connection.Close();
        }

        public void GetParentContractInfo(string cardholderContract, ref string parentContract, ref string parentContractName)
        {
            parentContract = "";
            var sb = new StringBuilder();

            if (cardholderContract.StartsWith("5") && cardholderContract.Length == 16)
            {
                sb.Append("SELECT contract_number, contract_name ");
                sb.Append("FROM OWS.ACNT_CONTRACT ");
                sb.Append("where id in (SELECT LIAB_CONTRACT ");
                sb.Append("FROM OWS.ACNT_CONTRACT ");
                sb.Append("where contract_number = (SELECT contract_number ");
                sb.Append("FROM OWS.ACNT_CONTRACT ");
                sb.Append("where id = (SELECT ACNT_CONTRACT__OID ");
                sb.Append("FROM OWS.ACNT_CONTRACT ");
                sb.AppendFormat("where contract_number = '{0}' ", cardholderContract);
                sb.Append("and AMND_STATE = 'A')) ");
                sb.Append("and AMND_STATE = 'A') ");
                sb.Append("and AMND_STATE = 'A'");
            }
            else
            {
                sb.Append("SELECT contract_number, contract_name ");
                sb.Append("FROM OWS.ACNT_CONTRACT ");
                sb.Append("where id in (SELECT LIAB_CONTRACT FROM OWS.ACNT_CONTRACT ");
                sb.AppendFormat("where contract_number = '{0}' and AMND_STATE = 'A') ", cardholderContract);
                sb.Append("and AMND_STATE = 'A'");
            }

            

            try
            {
                OracleCommand command = new OracleCommand(sb.ToString(), this._connection);
                command.CommandType = CommandType.Text;

                if (this._connection.State== ConnectionState.Closed)
                    this._connection.Open();

                using (OracleDataReader reader = command.ExecuteReader())
                    while (reader.Read())
                    {
                        parentContract = reader.GetString(0);
                        parentContractName = reader.GetString(1);
                    }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("An error has occurred while getting parent contract for {0}.", cardholderContract), ex);
            }
        }
    }
}
