using NHPortal.Classes;
using PortalFramework.ReportModel;
using System;
using System.Text;
using System.Web;
using System.Web.Services;

namespace NHPortal.Charts
{
    /// <summary> Returns JSON data for a chart request such as a drilldown.</summary>
    public class ChartDataHandler : IHttpHandler, System.Web.SessionState.IRequiresSessionState
    {
        [WebMethod]
        public void ProcessRequest(HttpContext context)
        {
            // Get type of request.
            string dataType = context.Request.QueryString["data"];

            try
            {
                HandleRequest(context, dataType);
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
        }

        private static void HandleRequest(HttpContext context, string dataType)
        {
            if (context.Request.UrlReferrer == null)
            {
                throw new Exception("Invalid Handler Request. Url Refferer is null or user does not have permission for this handler.");
            }

            // format result
            context.Response.ContentType = "application/json";
            context.Response.ContentEncoding = Encoding.UTF8;

            // Drill Down
            if (dataType == "dd")
            {
                ProcessDrillDown(context);
                return;
            }

            // Ajax Errors
            if (dataType == "ajaxerr")
            {
                string exception = context.Request.QueryString["ex"];
                throw new Exception("Ajax Error: " + exception);
            }

            // Clear Auxiliary table from session.
            if (dataType == "resetauxreport")
            {
                // Change back aux report to be the main report to be able to return from a detailed report.
                Report report = SessionHelper.GetCurrentReport(HttpContext.Current.Session);
                SessionHelper.SetAuxiliaryReport(HttpContext.Current.Session, report);
                
                context.Response.ContentType = "text/html";
                context.Response.ContentEncoding = Encoding.ASCII;
                context.Response.Write("Aux Table Cleared.");
                return;
            }
        }

        private static void HandleError(Exception exception)
        {

        }

        private static void ProcessDrillDown(HttpContext context)
        {
            string pointName = context.Request.QueryString["pt"];
            string seriesName = context.Request.QueryString["ser"];
            string seriesId = context.Request.QueryString["sid"];
            string chartName = context.Request.QueryString["cid"];
            string response = NHPortal.Classes.Charts.NHChartMaster.GetDrillSeries(pointName, seriesName, seriesId, chartName);

            context.Response.Write(response);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}