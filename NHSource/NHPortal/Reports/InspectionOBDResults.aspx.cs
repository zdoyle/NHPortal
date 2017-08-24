using NHPortal.Classes;
using NHPortal.Classes.Charts;
using NHPortal.Classes.Reports.InspectionReports;
using System;
using System.Web;

namespace NHPortal
{
    public partial class InspectionOBDResults : PortalPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Master.ReportData = new OBDResultsData();
            RedirectOnInvalidPermission(Master.ReportData.BaseReport.UserPermission);
        }
    }
}