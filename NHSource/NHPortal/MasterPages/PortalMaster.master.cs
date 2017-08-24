using NHPortal.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NHPortal.MasterPages
{
    public partial class PortalMaster : System.Web.UI.MasterPage
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            RedirectOnInvalidSession();
            RedirectIfPasswordRequired();
            RegisterTimeoutFunction();

            Page.LoadComplete += Page_LoadComplete;
            Menu.Visible = ShowMenu;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // Data binds the jquery include in the head tags
            // Which uses "#ResolveClientID" instead of "=ResolveClientID"
            // The former is a databinding expression while the latter calls Response.Write
            Page.Header.DataBind();

            if (String.IsNullOrEmpty(lblPageHeader.Text))
            {
                pnlHeader.Visible = false;
            }
            lblCopywrite.Text = String.Format("Copyright © 2003-{0}, Gordon-Darby Systems, Inc.", DateTime.Now.Year);
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            hidAction.Value = String.Empty;
        }

        //public void RedirectOnInvalidPermission(int permissionCode)
        //{
        //    NHPortal.Classes.User.PortalUser currentUser = SessionHelper.GetPortalUser(this.Session);
        //    if (currentUser == null || (permissionCode > 0 && !currentUser.HasReadAccess(permissionCode)))
        //    {
        //        Response.Redirect("~/InvalidPermissions.aspx");
        //    }
        //}

        private void RedirectOnInvalidSession()
        {
            NHPortal.Classes.User.PortalUser currentUser = SessionHelper.GetPortalUser(this.Session);
            if (currentUser == null)
            {
                if (ActiveSessionPage)
                {
                    Response.Redirect("~/Login.aspx");
                }
            }
        }

        private void RedirectIfPasswordRequired()
        {
            NHPortal.Classes.User.PortalUser currentUser = SessionHelper.GetPortalUser(this.Session);
            if (currentUser != null && currentUser.PasswordUpdateRequired &&
                ActiveSessionPage && !(this.Page is ChangePassword))
            {
                Response.Redirect("~/PortalNavHandler.ashx?code=" + NHPortal.UserControls.RedirectCodes.USER_UPDATE_PASSWORD);
            }
        }

        private void RegisterTimeoutFunction()
        {
            if (ActiveSessionPage)
            {
                string key = "page_timeout_function";
                if (!Page.ClientScript.IsClientScriptBlockRegistered(key))
                {
                    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), key, RenderTimeoutFunction(), true);
                }
            }
        }

        /// <summary>Returns a JavaScript function called using the setTimeout function to redirect the user to the SessionExpired page upon being idle for too long.</summary>
        /// <returns>Function to render to the page.</returns>
        private string RenderTimeoutFunction()
        {
            int timeout = PortalFramework.PortalIniSettings.Values.PageTimeout * 60 * 1000;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("setTimeout( 'userIdle()', " + timeout + " )");
            sb.AppendLine("function userIdle() {");
            sb.AppendLine("    document.location = '" + ResolveClientUrl("~/SessionExpired.aspx") + "';");
            sb.AppendLine("}");

            return sb.ToString();
        }

        /// <summary>Sets the value of the hidden value to display a message to the user upon page load.</summary>
        /// <param name="msg">Message to display.</param>
        public void SetMessagePrompt(string msg)
        {
            hidMsg.Value = msg;
        }

        /// <summary>Sets the value of the hidden action value.</summary>
        /// <param name="action">Value to set.</param>
        public void SetHiddenAction(string action)
        {
            hidAction.Value = action;
        }

        /// <summary>Sets the text of the page's header label.</summary>
        /// <param name="text">Text to set to the header label.</param>
        public void SetHeaderText(string text)
        {
            lblPageHeader.Text = text;
        }




        ///// <summary>Gets the current user logged into the Portal.</summary>
        //public PortalUser PortalUser { get; private set; }

        ///// <summary>Gets the text logger used for writing log files.</summary>
        //public FileLogger Logger { get; private set; }

        /// <summary>Gets a reference to the page's menu control.</summary>
        public NHPortal.UserControls.PortalMenu Menu
        {
            get { return this.menu; }
        }

        /// <summary>Gets a reference to the page's Data Last Updated Label control.</summary>
        public Label DataLastUpdateLabel
        {
            get { return this.lblDataLastUpdate; }
        }

        ///// <summary>Gets a reference to the page's hidden action HiddenField control..</summary>
        //public HiddenField HiddenAction
        //{
        //    get { return this.hidAction; }
        //}

        /// <summary>Gets the value stored in the HiddenField control, trimmed and in uppercase.</summary>
        public string HidActionValue
        {
            get
            {
                return this.hidAction.Value.Trim().ToUpper();
            }
        }

        ///// <summary>Gets a reference to the page's hidden message HiddenField control.</summary>
        //public HiddenField HiddenMessage
        //{
        //    get { return this.hidMsg; }
        //}

        /// <summary>Gets whether or not the current content page does not need timeout or session checks.</summary>
        private bool ActiveSessionPage
        {
            get
            {
                return !(this.Page is Login || this.Page is Logout || this.Page is SessionExpired);
            }
        }

        /// <summary>Gets whether or not the show the menu on the page.</summary>
        protected bool ShowMenu
        {
            get { return (ActiveSessionPage && !(this.Page is InvalidPermissions || this.Page is ApplicationError)); }
        }

        /// <summary>Gets or sets the async post back timeout value of the script manager.</summary>
        public int AsyncPostBackTimeoutValue
        {
            get { return ScriptManager.AsyncPostBackTimeout; }
            set { this.ScriptManager.AsyncPostBackTimeout = value; }
        }
    }
}