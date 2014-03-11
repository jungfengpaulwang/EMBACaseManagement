using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using FISCA.UDT;

namespace CaseManagement.Forms
{
    public partial class frmOAuth_Management : BaseForm
    {
        private AccessHelper Access;
        private ErrorProvider ErrorProvider1;
        private UDT.oAuthAccount OAuthAccount;

        public frmOAuth_Management()
        {
            InitializeComponent();

            Access = new AccessHelper();
            ErrorProvider1 = new ErrorProvider();
            OAuthAccount = new UDT.oAuthAccount();
            this.Load += new EventHandler(frmOAuth_Management_Load);
        }

        private void frmOAuth_Management_Load(object sender, EventArgs e)
        {
            List<UDT.oAuthAccount> oAuths = Access.Select<UDT.oAuthAccount>();
            if (oAuths.Count > 0)
            {
                this.OAuthAccount = oAuths.ElementAt(0);

                this.txtClientID.Text = this.OAuthAccount.ClientID;
                this.txtSecret.Text = this.OAuthAccount.Secret;
            }
        }            

        private void Save_Click(object sender, EventArgs e)
        {
            bool is_validated = true;
            ErrorProvider1.Clear();
            if (string.IsNullOrWhiteSpace(this.txtClientID.Text))
            {
                is_validated = false;
                ErrorProvider1.SetError(this.txtClientID, "必填");
            }
            if (string.IsNullOrWhiteSpace(this.txtSecret.Text))
            {
                is_validated = false;
                ErrorProvider1.SetError(this.txtSecret, "必填");
            }
            if (!is_validated)
                return;

            this.OAuthAccount.ClientID = this.txtClientID.Text.Trim();
            this.OAuthAccount.Secret = this.txtSecret.Text.Trim();

            this.OAuthAccount.Save();
            MessageBox.Show("儲存成功。");
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
