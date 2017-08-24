using GDCoreUtilities;
using NHPortal.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NHPortal
{
    public partial class ChangePassword : PortalPage
    {
        private const int MINIMUM_LENGTH = 8;
        private List<string> m_errors;

        protected void Page_Load(object sender, EventArgs e)
        {
            ResetErrors();

            if (IsPostBack)
            {

            }
            else
            {
                Master.SetHeaderText("Update Password");
                if (PortalUser.PasswordUpdateRequired)
                {
                    // Do not display the menu if the user needs to update their password
                    LogMessage("User password update required; hiding menu", GDCoreUtilities.Logging.LogSeverity.Information);
                    Master.Menu.Visible = false;
                }
            }
        }

        protected void btnUpdatePassword_Click(object sender, EventArgs e)
        {
            LogMessage("btnUpdatePassword_Click event", GDCoreUtilities.Logging.LogSeverity.Trace);

            ValidateFields();
            if (m_errors.Count > 0)
            {
                ApplyErrors();
                lblErrors.Visible = true;
            }
            else
            {
                PerformUpdate();
            }
        }

        private void PerformUpdate()
        {
            bool pwdReqd = PortalUser.PasswordUpdateRequired;
            LogMessage("pwdReqd: [" + pwdReqd + "]", GDCoreUtilities.Logging.LogSeverity.Debug);

            bool success = PortalUser.UpdatePassword(tbNewPassword.Text, PortalUser.UserName);
            if (success)
            {
                Master.SetMessagePrompt("Password updated successfully.");
                if (pwdReqd)
                {
                    Response.Redirect("~/PortalNavHandler.ashx?code=" + NHPortal.UserControls.RedirectCodes.MY_HOME);
                }
            }
            else
            {
                Master.SetMessagePrompt("There was an error updating the password.");
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            LogMessage("btnCancel_Click event", GDCoreUtilities.Logging.LogSeverity.Trace);

            string navCode = NHPortal.UserControls.RedirectCodes.MY_HOME;
            if (PortalUser.PasswordUpdateRequired)
            {
                navCode = NHPortal.UserControls.RedirectCodes.LOGOUT;
            }

            LogMessage("Redirecting to navCode [" + navCode + "]", GDCoreUtilities.Logging.LogSeverity.Debug);
            Response.Redirect("~/PortalNavHandler.ashx?code=" + navCode);
        }

        private void ValidateFields()
        {
            ValidateCurrentPassword();
            ValidateNewPassword();
        }

        private void ResetErrors()
        {
            m_errors = new List<string>();
            lblErrors.Text = String.Empty;
        }

        private void ApplyErrors()
        {
            foreach (string error in m_errors)
            {
                lblErrors.Text += error;
                if (error != m_errors.Last())
                {
                    lblErrors.Text += "<br />";
                }
            }

            LogMessage("Error label text set to " + lblErrors.Text, GDCoreUtilities.Logging.LogSeverity.Information);
        }

        private void ValidateCurrentPassword()
        {
            if (String.IsNullOrEmpty(tbCrntPassword.Text))
            {
                m_errors.Add("The current password field cannot be blank.");
            }
            else if (!tbCrntPassword.Text.Equals(PortalUser.Password))
            {
                m_errors.Add("The current password does not match the password on record.");
            }
        }

        private void ValidateNewPassword()
        {
            string newPassword = tbNewPassword.Text;
            if (String.IsNullOrWhiteSpace(newPassword))
            {
                m_errors.Add("The new password field cannot be blank.");
            }
            else
            {
                if (StringUtilities.AreEqual(newPassword, PortalUser.UserName))
                {
                    m_errors.Add("The new password cannot be the same as the user's ID.");
                }

                if (newPassword.Length < MINIMUM_LENGTH)
                {
                    m_errors.Add("The new password must be at least eight (8) characters.");
                }

                if (!StringUtilities.AreEqual(newPassword, tbConfirmPassword.Text))
                {
                    m_errors.Add("The new password fields do not match.");
                }

                if (StringUtilities.AreEqual(newPassword, PortalUser.Password))
                {
                    m_errors.Add("The new password must be different than the user's current password.");
                }

                CheckPasswordStrength(newPassword);
            }
        }

        private void CheckPasswordStrength(string password)
        {
            int reqCount = 0;
            if (StringUtilities.ContainsUppercaseAlpha(password))
            {
                reqCount++;
            }

            if (StringUtilities.ContainsDigit(password))
            {
                reqCount++;
            }

            if (password.Length >= MINIMUM_LENGTH)
            {
                reqCount++;
            }

            if (reqCount < 3)
            {
                m_errors.Add("The new password does not meet the strength requirements.");
            }
        }
    }
}