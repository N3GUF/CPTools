using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.ComponentModel;

namespace AppTools.Classes
{
    public static class Email
    {
        static bool mailSent;
        private static void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            // Get the unique identifier for this asynchronous operation.
            String token = (string)e.UserState;

            if (e.Cancelled)
            {
                Console.WriteLine("[{0}] Send canceled.", token);
            }
            if (e.Error != null)
            {
                Console.WriteLine("[{0}] {1}", token, e.Error.ToString());
            }
            else
            {
                Console.WriteLine("Message sent.");
            }
            mailSent = true;
        }

        public static void Test()
        {
            SmtpClient client       = new SmtpClient("mailhost");
            MailAddress from        = new MailAddress("IFPMonitoring@Emdeon.com");
            MailAddress to          = new MailAddress("dbernhardy@Emdeon.com");
            MailMessage message     = new MailMessage(from, to);
            message.Body            = "This is a test e-mail message sent by an application. ";
            message.BodyEncoding    = System.Text.Encoding.UTF8;
            message.Subject         = "test message 1";
            message.SubjectEncoding = System.Text.Encoding.UTF8;
            client.SendCompleted   += new SendCompletedEventHandler(SendCompletedCallback);
            string userState        = "test message1";
            client.SendAsync(message, userState);
            message.Dispose();
            Console.WriteLine("Goodbye.");
        }
    }
}
