using NHPortal.Classes;
using NHPortal.Classes.Charts;
using NHPortal.Classes.Reports.InspectionReports;
using System;
using System.Web;

namespace NHPortal
{
    public partial class InspectionModelYearRejection : PortalPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Master.ReportData = new ModelYearReportData();
            RedirectOnInvalidPermission(Master.ReportData.BaseReport.UserPermission);
        }
    }
}