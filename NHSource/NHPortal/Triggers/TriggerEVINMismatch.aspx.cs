using NHPortal.Classes;
using NHPortal.Classes.Charts;
using NHPortal.Classes.Reports.Triggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NHPortal
{
    public partial class TriggerEVINMismatch : PortalPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                SetReportData();
            }
            else
            {
                InitReportType();
            }

            RedirectOnInvalidPermission(Master.ReportData.BaseReport.UserPermission);
        }

        private void SetReportData()
        {
            Master.ReportData = HttpContext.Current.Session[NHChartMaster.CHART_REPORT_DATA] as BaseTrigger;

            if (Master.ReportData == null)
            {
                InitReportType();
            }
        }

        private void InitReportType()
        {
            Master.ReportData = new EVINMismatch();
        }

    }
}