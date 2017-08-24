using GDCoreUtilities.Logging;
using NHPortal.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NHPortal
{
    public partial class Logout : PortalPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Master.Menu.Visible = false;
            LogMessage("User [" + PortalUserName + "] logged out. Cleaning up the user's session.", LogSeverity.Information);
            SessionHelper.CleanupUserSession(this.Session);
        }
    }
}