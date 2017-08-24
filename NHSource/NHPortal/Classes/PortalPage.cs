using GDCoreUtilities.Logging;
using NHPortal.Classes.User;
using PortalFramework.ReportModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NHPortal.Classes
{
    /// <summary>Defines a page used for containing common methods and properties across web pages for the Portal.</summary>
    public abstract class PortalPage : System.Web.UI.Page
    {
        /// <summary>Gets the current user logged into the Portal.</summary>
        public PortalUser PortalUser { get; protected set; }

        /// <summary>Gets the user name of the current user logged into the Portal, or "NULL" if the PortalUser object is null.</summary>
        public string PortalUserName
        {
            get { return (PortalUser == null ? "NULL" : PortalUser.UserName); }
        }

        ///// <summary>Gets the text logger used for writing log files.</summary>
        //public FileLogger Logger { get; protected set; }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PortalUser = SessionHelper.GetPortalUser(this.Session);
            //Logger = SessionHelper.GetSessionLogger(this.Session);

            if (IsPostBack)
            {
                NHPortalUtilities.LogSessionMessage(String.Format("User [{0}] PostBack to [{1}]", 
                    PortalUserName, this.GetType().Name), LogSeverity.Information);
            }
            else
            {
                NHPortalUtilities.LogSessionMessage(String.Format("User [{0}] Navigate to [{1}]",
                    PortalUserName, this.GetType().Name), LogSeverity.Information);
            }
        }

        /// <summary>Redirects the user to an invalid permission page if the user does not the required permission.</summary>
        /// <param name="permission">Permission to check.</param>
        public void RedirectOnInvalidPermission(UserPermission permission)
        {
            //NHPortal.Classes.User.PortalUser currentUser = SessionHelper.GetPortalUser(this.Session);
            if (PortalUser == null || permission != null)
            {
                RedirectOnInvalidPermission(permission.Code);
            }
        }

        /// <summary>Redirects the user to an invalid permission page if the user does not the required permission.</summary>
        /// <param name="permissionCode">Permission code to check.</param>
        public void RedirectOnInvalidPermission(int permissionCode)
        {
            //NHPortal.Classes.User.PortalUser currentUser = SessionHelper.GetPortalUser(this.Session);
            if (PortalUser == null || (permissionCode > 0 && !PortalUser.HasReadAccess(permissionCode)))
            {
                Response.Redirect("~/InvalidPermissions.aspx");
            }
        }

        /// <summary>Writes a message to the session log file.</summary>
        /// <param name="msg">Message to log.</param>
        /// <param name="severity">Severity of the log entry.</param>
        public void LogMessage(string msg, LogSeverity severity)
        {
            NHPortalUtilities.LogSessionMessage(msg, severity);
        }

        /// <summary>Logs an exception to the session log file.</summary>
        /// <param name="ex">Exception to log.</param>
        public void LogException(Exception ex)
        {
            NHPortalUtilities.LogSessionException(ex);
        }

        /// <summary>Logs an OracleResponse to the session log file.</summary>
        /// <param name="response"></param>
        public void LogOracleResponse(GDDatabaseClient.Oracle.OracleResponse response)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine("Logging Oracle Response");
            sb.AppendLine("Success:   " + response.Successful);
            sb.AppendLine("Code:      " + response.ErrorCode);
            sb.AppendLine("Message:   " + response.ErrorMessage);
            sb.AppendLine("Exception: " + (response.Exception == null ? "NULL" : response.Exception.ToString()));
            NHPortalUtilities.LogSessionMessage(sb.ToString(), LogSeverity.Information);
        }
    }
}