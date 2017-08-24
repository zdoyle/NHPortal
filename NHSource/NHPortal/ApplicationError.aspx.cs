using NHPortal.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NHPortal
{
    public partial class ApplicationError : PortalPage
    {
        private Exception m_exception;

        protected void Page_Load(object sender, EventArgs e)
        {
            m_exception = SessionHelper.GetSessionException(this.Session);
            Process();
        }

        private void Process()
        {
            if (m_exception != null)
            {
                if (m_exception.InnerException != null)
                {
                    DisplayError(m_exception.InnerException);
                }
                else
                {
                    DisplayError(m_exception);
                }

                SendAlert();
            }
            else
            {
                lblErrorMessage.Text = "N/A";
            }
        }

        private void SendAlert()
        {
            try
            {
                GDCoreUtilities.Email.EmailUtilities.SendEmail(Global.AlertSettings,
                    "NH Portal Application Error",
                    "An unhandled exception was encountered.  Exception Details: " + Environment.NewLine + m_exception.ToString(),
                    true);
            }
            catch (Exception ex)
            {
                NHPortalUtilities.LogSessionException(ex, "In ApplicationError.SendAlert");
            }
        }

        private void DisplayError(Exception ex)
        {
            if (ex is System.IO.IOException)
            {
                lblErrorMessage.Text = "An I/O exception has occurred.";
            }
            else
            {
                lblErrorMessage.Text = ex.Message;
            }
        }
    }
}