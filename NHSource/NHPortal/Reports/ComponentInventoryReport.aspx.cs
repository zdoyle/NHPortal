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
    public partial class ComponentInventoryReport : NHPortal.Classes.PortalPage
    {
        public const string ReportTitle = "Component Inventory Report";
        public const string CSVFilePrefix = "ComponentInventoryReport";

        /// <summary>Gets the directory to use for CSV based reports.</summary>
        public static string CsvFilePath
        {
            get { return PortalFramework.PortalIniSettings.Settings.GetValue("DIR_COMPONENT_INVENTORY"); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            RedirectOnInvalidPermission(UserPermissions.ComponentInventory);

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
            Master.SaveFavorite(NHPortal.UserControls.RedirectCodes.REPORT_COMPONENT_INVENTORY, UserFavoriteTypes.Report);
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
            //Hold off on sql for now.
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
                    Master.UserReport.Columns.Insert("TOTAL", "Total", 1);
                    CalculateTotalColumn();
                    LogMessage("Data rendering to page", LogSeverity.Information);
                }
            }
            else
            {
                Master.UserReport = null;
                Master.SetError(response.Exception);
                LogMessage("Query error: " + response.Exception, LogSeverity.Information);
            }

            LogMessage("Query Finished", LogSeverity.Information);
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

                    if (Master.UserReport.Columns.Count() >= 9)
                    {
                        Master.UserReport.Columns[4].Text = "Wait Test Field Use";
                        Master.UserReport.Columns[5].Text = "Wait Test Return";
                        Master.UserReport.Columns[9].Text = "Sent To KY Eval";
                    }

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
            //sb.AppendLine("SELECT distinct (man.DESCRIPTION)  as \"Component\",");
            //sb.AppendLine("sum ( case when pui.DescriptionCode = 'NHOST' then 1 else 0 end) as \"Field Unit\",");
            //sb.AppendLine("sum (case when pui.DescriptionCode = 'WHSTR' then 1 else 0 end) as \"Warehouse Gen Store\",");
            //sb.AppendLine("sum (case when pui.DescriptionCode = 'WHTFU' then 1 else 0 end) as \"Wait Test Field Use\",");
            //sb.AppendLine("sum (case when pui.DescriptionCode = 'WHTMF' then 1 else 0 end) as \"Wait Test Return\",");
            //sb.AppendLine("sum (case when pui.DescriptionCode = 'WHRMF' then 1 else 0 end) as \"Wait Return Manufacturer\",");
            //sb.AppendLine("sum (case when pui.DescriptionCode = 'WHBYR' then 1 else 0 end) as \"Beyond Repair\",");
            //sb.AppendLine("sum (case when pui.DescriptionCode = 'WHSMF' then 1 else 0 end) as \"Sent To Manufacturer\",");
            //sb.AppendLine("sum (case when pui.DescriptionCode = 'KYEVAL' then 1 else 0 end) as \"Sent To KY Eval\",");
            //sb.AppendLine("sum (case when pui.DescriptionCode = 'KYRTN' then 1 else 0 end) as \"Returned To KY\"");
            //sb.AppendLine("from INVENTORY inv,MANUFACTURER man, UNIT un, TYPEID typ, POSSIBLEUNITIDS pui");
            //sb.AppendLine("WHERE inv.MAKEMODELID = man.MAKEMODELID");
            //sb.AppendLine("AND inv.UnitID = un.UNITID ");
            //sb.AppendLine("AND un.DescriptionCode = pui.DescriptionCode");
            //sb.AppendLine("--AND man.TYPEID = typ.TYPEID");
            //sb.AppendLine("AND inv.MAKEMODELID IN (SELECT man.MAKEMODELID FROM MANUFACTURER WHERE man.ACTIVESTATUS = '1')");
            //sb.AppendLine("AND typ.serialized = 'Y'");
            //sb.AppendLine("and inv.receiveddate >= '" + DateTime.Now.Year + "0101" + "'");
            //sb.AppendLine("and inv.receiveddate <= '" + this.dpReportDate.GetDateText("yyyyMMdd") + "'");
            //sb.AppendLine("GROUP BY man.DESCRIPTION");
            //sb.AppendLine("ORDER BY man.DESCRIPTION");

            sb.AppendLine(" SELECT M.DESCRIPTION AS \"Component\", ");
            sb.AppendLine(" SUM (CASE WHEN U.DescriptionCode = 'NHOST'  THEN 1 ELSE 0 END) AS \"Field Unit\", ");
            sb.AppendLine(" SUM (CASE WHEN U.DescriptionCode = 'WHSTR'  THEN 1 ELSE 0 END) AS \"Warehouse Gen Store\", ");
            sb.AppendLine(" SUM (CASE WHEN U.DescriptionCode = 'WHTFU'  THEN 1 ELSE 0 END) AS \"Wait Test Field Use\", ");
            sb.AppendLine(" SUM (CASE WHEN U.DescriptionCode = 'WHTMF'  THEN 1 ELSE 0 END) AS \"Wait Test Return\", ");
            sb.AppendLine(" SUM (CASE WHEN U.DescriptionCode = 'WHRMF'  THEN 1 ELSE 0 END) AS \"Wait Return Manufacturer\", ");
            sb.AppendLine(" SUM (CASE WHEN U.DescriptionCode = 'WHBYR'  THEN 1 ELSE 0 END) AS \"Beyond Repair\", ");
            sb.AppendLine(" SUM (CASE WHEN U.DescriptionCode = 'WHSMF'  THEN 1 ELSE 0 END) AS \"Sent To Manufacturer\", ");
            sb.AppendLine(" SUM (CASE WHEN U.DescriptionCode = 'KYEVAL' THEN 1 ELSE 0 END) AS \"Sent To KY Eval\", ");
            sb.AppendLine(" SUM (CASE WHEN U.DescriptionCode = 'KYRTN'  THEN 1 ELSE 0 END) AS \"Returned To KY\" ");
            sb.AppendLine(" FROM INVENTORY I ");
            sb.AppendLine(" JOIN UNIT U         ON U.UNITID      = I.UNITID ");
            sb.AppendLine(" JOIN MANUFACTURER M ON M.MAKEMODELID = I.MAKEMODELID ");
            sb.AppendLine(" JOIN TYPEID T       ON T.TYPEID      = M.TYPEID ");
            sb.AppendLine(" WHERE M.ACTIVESTATUS = '1' ");
            sb.AppendLine(" AND   T.SERIALIZED   = 'Y' ");
            sb.AppendLine(" GROUP BY M.DESCRIPTION ");
            sb.AppendLine(" ORDER BY M.DESCRIPTION ");

            return sb.ToString();
        }
        //creates a total column
        private void CalculateTotalColumn()
        {
            foreach (ReportRow row in Master.UserReport.Rows)
            {
                int total = 0;
                for (int i = 2; i < row.Cells.Count; i++)
                {
                    total += row.Cells[i].ValueAsInt;
                }
                row["TOTAL"].Value = total.ToString();
            }
        }
    }
}