using Comdata.AppSupport.UpdateLoadCancelReject.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Comdata.AppSupport.UpdateLoadCancelReject.Forms
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            Session.Dal.Username = this.tbUsername.Text;
            Session.Dal.Password = this.tbPassword.Text;

            try
            {
                if (Session.Dal.ConnectionIsReady)
                {
                   // Session.MainForm.Message = "Login as " + this.tbUsername.Text;
                    this.Dispose();
                }
            }
            catch (Exception ex)
            {
                this.statusLabel.Text = ex.Message;
            }
        }
    }
}
