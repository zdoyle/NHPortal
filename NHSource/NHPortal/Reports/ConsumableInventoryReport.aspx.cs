using GDCoreUtilities.Logging;
using NHPortal.Classes;
using NHPortal.Classes.User;
using PortalFramework.ReportModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NHPortal.Reports
{
    public partial class ConsumableInventoryReport : NHPortal.Classes.PortalPage
    {
        public const string ReportTitle = "Consumable Inventory Report";
        public const string CSVFilePrefix = "ConsumableInventoryReport";

        /// <summary>Gets the directory to use for CSV based reports.</summary>
        public static string CsvFilePath
        {
            get { return PortalFramework.PortalIniSettings.Settings.GetValue("DIR_CONSUMABLE_INVENTORY"); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            RedirectOnInvalidPermission(UserPermissions.ConsumableInventory);

            if (IsPostBack)
            {
                ProcessPostBack();
            }
            else
            {
                InitializePage();
                LoadFavorite();
                RunReport();
            }
        }

        private void InitializePage()
        {
            Master.SetHeaderText(ReportTitle);
            this.dpReportDate.Text = DateTime.Now.ToShortDateString();
        }

        private void ProcessPostBack()
        {
            string action = Master.MasterPage.HidActionValue;
            switch (action)
            {
                case "RUN_REPORT":
                    RunReport();
                    break;
                case "SAVE_FAVORITE":
                    SaveFavorite();
                    break;
            }
        }
        private void SaveFavorite()
        {
            Master.SaveFavorite(NHPortal.UserControls.RedirectCodes.REPORT_CONSUMABLE_INVENTORY, UserFavoriteTypes.Report);
        }

        private void LoadFavorite()
        {
            UserFavorite fav = SessionHelper.GetSelectedFavorite(this.Session);
            if (fav != null)
            {
                foreach (var c in fav.Criteria)
                {
                    switch (c.Description.Trim().ToUpper())
                    {
                        case "REPORT DATE":
                            this.dpReportDate.Text = c.Value;
                            break;
                    }
                }
            }
        }

        private void SetMetaData()
        {
            if (Master.UserReport != null)
            {
                if (!String.IsNullOrEmpty(this.dpReportDate.GetDateText("yyyyMMdd")))
                {
                    Master.UserReport.MetaData.Add("Report Date", this.dpReportDate.GetDateText("MM/dd/yyyy"));
                }
            }
        }

        private void RunReport()
        {
            //ReportFromSQL();
            ReportFromCSV();
            SetMetaData();
            SetColumnTypes();
            Master.UserReport.Sortable = true;
            Master.RenderReportToPage();
            SessionHelper.SetCurrentReport(this.Session, Master.UserReport);
        }

        private void ReportFromSQL()
        {
            string qry = BuildQuery();
            GDDatabaseClient.Oracle.OracleResponse response = PortalFramework.Database.ODAP.GetDataTable(qry, PortalFramework.Database.DatabaseTarget.OLTP);
            LogMessage("Attempting to run query...", LogSeverity.Information);
            if (response.Successful)
            {
                LogMessage("Query is successful", LogSeverity.Information);
                Master.UserReport = new Report(ReportTitle, response.ResultsTable);
                Master.SetDataLastUpdate(DateTime.Now);

                if (response.HasResults)
                {
                    LogMessage("Query has results", LogSeverity.Information);
                    LogMessage("Data rendering to page", LogSeverity.Information);
                }
            }
            else
            {
                Master.UserReport = null;
                Master.SetError(response.Exception);
                LogMessage("Query error: " + response.Exception, LogSeverity.Information);
            }

            LogMessage("Query Finshed", LogSeverity.Information);
            LogOracleResponse(response);
            LogMessage("Page Loaded", LogSeverity.Information);
        }

        private void ReportFromCSV()
        {
            string fullPath = NHPortalUtilities.BuildCSVFileName(CsvFilePath, CSVFilePrefix, dpReportDate.Date);
            try
            {
                NHPortalCSVReader csvReader = new NHPortalCSVReader(fullPath);
                csvReader.Process();

                if (csvReader.Table != null)
                {
                    LogMessage("CSV Reader running", LogSeverity.Information);
                    Master.UserReport = new PortalFramework.ReportModel.Report(ReportTitle, csvReader.Table);
                    LogMessage("CSV Reader Complete", LogSeverity.Information);
                }
            }
            catch (IOException ex)
            {
                Master.UserReport = null;
                NHPortalUtilities.LogSessionException(ex);
                // TODO call Master.SetError here... may not want to pass in the exception in case it exposes the file structure
            }
            catch (Exception ex)
            {
                Master.UserReport = null;
                NHPortalUtilities.LogSessionException(ex);
            }
        }

        //set column types so export can read them as a number 
        private void SetColumnTypes()
        {
            for (int i = 1; i < Master.UserReport.ColumnCount; i++)
            {
                Master.UserReport.Columns[i].ColumnDataType = ColumnDataType.Number;
            }
        }

        private string BuildQuery()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("SELECT DISTINCT (man.DESCRIPTION) as \"Component\",");
            sb.AppendLine("SUM(CASE WHEN ad.quantity = 1 then 1 else 0 end) as \"Quantity On Hand\",");

            //this is when the page loads and runs the query, this is the date format
            sb.AppendLine("SUM(CASE WHEN ad.quantity = 1 AND \"DATE\" >= '" + DateTime.Now.Year + "0101" + "' AND \"DATE\" <= '" + this.dpReportDate.GetDateText("yyyyMMdd") + "' AND ad.FROMID LIKE '%GD%SALE%'  then 1 else 0 end) as \"MTD Sales\",");
            sb.AppendLine("--month to date, start date to todays date");
            sb.AppendLine("SUM(CASE WHEN ad.quantity = 1 AND \"DATE\" >= '" + DateTime.Now.Year + "0101" + "' AND \"DATE\" <= '" + this.dpReportDate.GetDateText("yyyyMMdd") + "' AND ad.FROMID LIKE '%GD%SALE%'  then 1 else 0 end) as \"YTD Sales\"");

            sb.AppendLine("FROM  MANUFACTURER man");
            sb.AppendLine("left join ACTIVITYDETAIL ad on ad.typeid = man.typeid ");
            sb.AppendLine("left join TYPEID typ on man.TYPEID = typ.TYPEID ");
            sb.AppendLine("WHERE ad.TYPEID = man.TYPEID");
            sb.AppendLine("AND ad.CHARGABLE = 'Y'");
            sb.AppendLine("AND typ.serialized = 'N'");

            //this is when the page loads and runs the query, this is the date format
            sb.AppendLine("AND ad.\"DATE\" >= '" + DateTime.Now.Year + "0101" + "'");
            sb.AppendLine("AND ad.\"DATE\" <= '" + this.dpReportDate.GetDateText("yyyyMMdd") + "' ");

            sb.AppendLine("GROUP BY man.DESCRIPTION,man.MAKEMODELID,typ.serialized");
            sb.AppendLine("ORDER BY man.DESCRIPTION");
            return sb.ToString();
        }
    }
}