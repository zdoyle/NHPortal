using GD.Highcharts.Enums;
using NHPortal.Classes;
using NHPortal.Classes.Charts;
using NHPortal.Classes.Reports.Triggers;
using PortalFramework.ReportModel;
using System;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NHPortal.Charts
{
    public partial class GDCharts : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Redirect("~/Welcome.aspx");
        }

        [WebMethod]
        public static string HandleDrillDown(string testDateVal, string drillVal, string prevDrillValue, int drillLevel)
        {
            IChartReportData reportData = HttpContext.Current.Session[NHChartMaster.CHART_REPORT_DATA] as IChartReportData;

            return reportData.LoadDrillLevel(testDateVal, drillVal, prevDrillValue, drillLevel);
        }

        [WebMethod]
        public static string HandleSIReadinessData()
        {
            string returnVal = string.Empty;
            IChartReportData reportData = HttpContext.Current.Session[NHChartMaster.CHART_REPORT_DATA] as IChartReportData;

            if (reportData is StationInspector)
            {
                StationInspector stationInspector = reportData as StationInspector;
                returnVal = stationInspector.LoadReadinessReport();
            }

            return returnVal;
        }
    }
}