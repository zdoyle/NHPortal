using GDCoreUtilities.Logging;
using NHPortal.Classes;
using NHPortal.Classes.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NHPortal
{
    public partial class Login : PortalPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            lblError.Text = String.Empty;

            if (IsPostBack)
            {

            }
            else
            {
                Master.Menu.Visible = false;
            }

            tbUsername.Focus();
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(DateTime.Now + " - Attempting User Login...");

            PortalUser usr;
            if (PortalUser.TryLogin(tbUsername.Text, tbPassword.Text, out usr))
            {
                PortalUser = usr;
                EstablishSession();
                LogMessage("User " + PortalUser.UserName + " successfully logged in!", LogSeverity.Information);
                DetermineRedirect();
            }
            else
            {
                UpdateAttempts();
                string msg = "Invalid user name or password";
                LogMessage(msg, LogSeverity.Information);
                lblError.Text = msg;
            }
        }

        private void UpdateAttempts()
        {
            PortalUser usr = PortalUsers.Find(tbUsername.Text);
            if (usr != null)
            {
                usr.LoginAttemptCount++;
                if (usr.LoginAttemptCount >= 3 && !usr.IsLocked)
                {
                    if (usr.UpdateLockedStatus(true))
                    {
                        string msg = String.Format("Maximum attempt count for user {0} reached. Locking user account.", usr.UserName);
                        Master.SetMessagePrompt(msg);
                    }
                }
            }
        }

        private void EstablishSession()
        {
            string filename = PortalUser.UserName + "_" + Guid.NewGuid().ToString() + ".log";
            string logPath = System.IO.Path.Combine(Server.MapPath("~"), PortalFramework.PortalIniSettings.Values.Directories.Logs);
            FileLogger logger = new FileLogger(logPath, filename);

            PortalUser.LoginAttemptCount = 0;
            SessionHelper.SetPortalUser(this.Session, PortalUser);
            SessionHelper.SetSessionLogger(this.Session, logger);

            Master.Menu.EvaluateMenuVisibility(PortalUser);
        }

        private void DetermineRedirect()
        {
            if (PortalUser.PasswordUpdateRequired)
            {
                AlertAndRedirect("Password has expired. You must update your password.", "ChangePassword.aspx");
            }
            else if (PortalUser.DaysUntilPasswordExpires <= 7)
            {
                AlertAndRedirect("Password expires in " + PortalUser.DaysUntilPasswordExpires + " days.", "Welcome.aspx");
            }
            else
            {
                Response.Redirect("Welcome.aspx");
            }
        }

        private void AlertAndRedirect(string msg, string target)
        {
            string key = "password_redirect";
            if (!Page.ClientScript.IsClientScriptBlockRegistered(key))
            {
                string appPath = Request.ApplicationPath;
                if (!appPath.EndsWith("/"))
                {
                    appPath += "/";
                }
                appPath += target;

                string script = String.Format("alert('{0}'); window.location='{1}';",
                    msg, appPath);
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), key, script, true);

                NHPortalUtilities.LogSessionMessage("AlertAndRedirect script: [" + script + "]", LogSeverity.Debug);
            }
        }
    }
}