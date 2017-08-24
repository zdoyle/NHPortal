using GDCoreUtilities;
using GDCoreUtilities.Logging;
using NHPortal.Classes;
using PortalFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace NHPortal
{
    public class Global : System.Web.HttpApplication
    {
        /// <summary>Gets the settings for sending alert emails.</summary>
        public static GDCoreUtilities.Email.EmailSettings AlertSettings { get; private set; }

        protected void Application_Start(object sender, EventArgs e)
        {
            PortalIniSettings.Initialize("NHPortal.ini");
            NHPortalUtilities.LogApplicationMessage("Application_Start event", LogSeverity.Information);

            //GDCoreUtilities.StringUtilities.SetLogger(PortalIniSettings.Values.Directories.Logs);
            //GDDatabaseClient.Oracle.OracleWorker.SetLogger(PortalIniSettings.Values.Directories.Logs);
            //GDCoreUtilities.Email.EmailUtilities.SetLogger(PortalIniSettings.Values.Directories.Logs);

            PortalFramework.Utilities.Stylesheets.Generate(Server.MapPath("~/Style"));
            MapJQueryToScriptManager();
            SetEmailSettings();
        }

        private void MapJQueryToScriptManager()
        {
            string JQueryVer = "1.10.2";
            System.Web.UI.ScriptManager.ScriptResourceMapping.AddDefinition("jquery", new System.Web.UI.ScriptResourceDefinition
            {
                Path = "~/Scripts/jquery-" + JQueryVer + ".min.js",
                DebugPath = "~/Scripts/jquery-" + JQueryVer + ".js",
                CdnPath = "http://ajax.aspnetcdn.com/ajax/jQuery/jquery-" + JQueryVer + ".min.js",
                CdnDebugPath = "http://ajax.aspnetcdn.com/ajax/jQuery/jquery-" + JQueryVer + ".js",
                CdnSupportsSecureConnection = true,
                LoadSuccessExpression = "window.jQuery"
            });
        }

        private void SetEmailSettings()
        {
            AlertSettings = new GDCoreUtilities.Email.EmailSettings();
            try
            {
                AlertSettings.Host = PortalIniSettings.Settings.GetValue("EMAIL_HOST");
                AlertSettings.Sender = PortalIniSettings.Settings.GetValue("EMAIL_SENDER");
                AlertSettings.Port = GDCoreUtilities.NullSafe.ToInt(PortalIniSettings.Settings.GetValue("EMAIL_PORT"));
                foreach (var to in PortalIniSettings.Settings.GetValues("EMAIL_TO"))
                {
                    AlertSettings.AddEmail(to, GDCoreUtilities.Email.EmailSendTarget.To);
                }
                foreach (var cc in PortalIniSettings.Settings.GetValues("EMAIL_CC"))
                {
                    AlertSettings.AddEmail(cc, GDCoreUtilities.Email.EmailSendTarget.Cc);
                }
                foreach (var bcc in PortalIniSettings.Settings.GetValues("EMAIL_BCC"))
                {
                    AlertSettings.AddEmail(bcc, GDCoreUtilities.Email.EmailSendTarget.Bcc);
                }

                NHPortalUtilities.LogApplicationMessage("Using AlertSettings: " + Environment.NewLine + AlertSettings.GetLogString(),
                    LogSeverity.Information);
            }
            catch (Exception ex)
            {
                NHPortalUtilities.LogApplicationException(ex, "In Global.SetEmailSettings");
            }
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            // Store a dummy value in session to help keep sessions alive
            Session["DUMMY_SESSION_VALUE"] = DateTime.Now;
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                //NHPortalUtilities.GetRequestLength(sender);
            }
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();
            NHPortalUtilities.LogSessionException(ex, "Unhandled exception in Global.Application_Error");
            SessionHelper.SetSessionException(this.Session, ex);
            Server.ClearError();

            if (ex.GetType() == typeof(HttpException))
            {
                if (ex.Message.Contains("maxUrlLength"))
                {
                    // TODO special logic for http errors ? redirect to a different error page?
                }
            }     

            Response.Redirect("~/ApplicationError.aspx");
        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {
            NHPortalUtilities.LogApplicationMessage("Application_End event", LogSeverity.Information);
        }
    }
}