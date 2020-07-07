using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace AppTools
{
    class SmtpMail
    {
        #region Fields

        SmtpClient mailClient;
        NetworkCredential credentials;
        MailMessage message;
        string mailHost;
        int mailPort;

        #endregion

        #region Properties

        public string MailHost
        {
            get { return mailHost; }
            set { mailHost = value; }
        }

        public int MailPort
        {
            get { return mailPort; }
            set { mailPort = value; }
        }

        #endregion

        #region Public Methods

        public void Send()
        {
            credentials    = new NetworkCredential();
            mailHost       = "mailHost";
            mailClient     = new SmtpClient(mailHost);
            message.Sender = new MailAddress("dbernhardy@emdeon.com");
            message.To.Add(new MailAddress("dbernhardy@emdeon.com"));
            message.Subject ="Test Message";
            message.Body    = "yada yada yada";
            mailClient.Send(message);

        }
        #endregion

        #region Private Methods
        #endregion

        #region Event Handlers
        #endregion
    }
}
