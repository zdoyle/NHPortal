using NHPortal.Classes;
using NHPortal.Classes.User;
using PortalFramework.ReportModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Threading;
using System.IO;
using GDCoreUtilities;
using GDCoreUtilities.Logging;

namespace NHPortal.Reports
{
    public partial class StationSupportReport : NHPortal.Classes.PortalPage
    {
        public const string ReportTitle = "Station Support Report";
        public const string CSVFilePrefix = "StationSupportReport";

        /// <summary>Gets the directory to use for CSV based reports.</summary>
        public static string CsvFilePath
        {
            get { return PortalFramework.PortalIniSettings.Settings.GetValue("DIR_STATION_SUPPORT"); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            RedirectOnInvalidPermission(UserPermissions.StationSupport);

            if (IsPostBack)
            {
                ProcessPostBack();
            }
            else
            {
                InitializePage();
                ParseQueryStrings();
                LoadFavorite();
                RunReport();
            }
        }

        private void InitializePage()
        {
            Master.SetHeaderText(ReportTitle);
            cboCounty.Initialize();
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
        private void ParseQueryStrings()
        {
            if (Request.QueryString["cnty"] != null)
            {
                string cnty = Request.QueryString["cnty"];
                //lstCounty.SetSelectedValue(cnty);
            }
        }
        private void SaveFavorite()
        {
            Master.SaveFavorite(NHPortal.UserControls.RedirectCodes.REPORT_STATION_SUPPORT, UserFavoriteTypes.Report);
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
                            dpReportDate.Text = c.Value;
                            break;
                        case "COUNTY":
                            cboCounty.SelectedValue = c.Value;
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
                Master.UserReport.MetaData.Add("County ", cboCounty.SelectedItem.ToString(), cboCounty.SelectedValue);
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
            int[] totalValues = new int[] { 0, 0, 0, 0, 0, 0 };

            GDDatabaseClient.Oracle.OracleResponse response = PortalFramework.Database.ODAP.GetDataTable(qry, PortalFramework.Database.DatabaseTarget.OLTP);
            LogMessage("Attempting to run query...", LogSeverity.Information);
            if (response.Successful)
            {
                LogMessage("Query is successful", LogSeverity.Information);
                Master.UserReport = new Report(ReportTitle, response.ResultsTable);
                Master.SetDataLastUpdate(DateTime.Now);

                if (response.HasResults)
                {
                    foreach (DataRow dRow in response.ResultsTable.Rows)
                    {
                        for (int i = 0; i < response.ResultsTable.Columns.Count; i++)
                        {
                            if (i > 0 && i <= totalValues.Count())
                            {
                                totalValues[i - 1] += NullSafe.ToInt(dRow[i]);
                            }
                        }
                    }

                    ReportRow totalRow = new ReportRow(Master.UserReport);
                    totalRow.AddClass("static");
                    totalRow.AddClass("total-row");

                    ReportCell rptCell = new ReportCell("ALL");
                    totalRow.Cells.Add(rptCell);

                    for (int i = 0; i < totalValues.Count(); i++)
                    {
                        rptCell = new ReportCell(totalValues[i].ToString());
                        totalRow.Cells.Add(rptCell);
                    }

                    Master.UserReport.Rows.Insert(totalRow, 0);

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
            string fullPath = NHPortalUtilities.BuildCSVFileName(CsvFilePath, CSVFilePrefix, 
                dpReportDate.Date, cboCounty.SelectedItem.Text);
            try
            {
                NHPortalCSVReader csvReader = new NHPortalCSVReader(fullPath);
                csvReader.Process();

                if (csvReader.Table != null)
                {
                    LogMessage("CSV Reader running", LogSeverity.Information);

                    //if (csvReader.Table.Columns.Count >= 7)
                    //{
                    //    csvReader.Table.Columns.RemoveAt(7);
                    //}

                    Master.UserReport = new PortalFramework.ReportModel.Report(ReportTitle, csvReader.Table);

                    if (Master.UserReport.Rows.Count() >= 1)
                    {
                        Master.UserReport.Rows[0].AddClass("static");
                        Master.UserReport.Rows[0].AddClass("total-row");
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

        //sets column types to numbers so exports will read them in xlsx
        private void SetColumnTypes()
        {
            for (int i = 1; i < Master.UserReport.ColumnCount; i++)
            {
                Master.UserReport.Columns[i].ColumnDataType = ColumnDataType.Number;
            }
        }

        private string BuildQuery()
        {
            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            TextInfo textInfo = cultureInfo.TextInfo;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("select Distinct(cc.stationid ) as \"Station ID\",");
            sb.AppendLine("sum (case when reasonforcall IS NOT NULL then 1 else 0 end) as \"Total Inquiries To Call Center\",");
            sb.AppendLine("sum (case when sa.activitynumber IS NOT NULL then 1 else 0 end) as \"Total Support Activities\",");
            sb.AppendLine("sum (case when sa.supportmethod = 'A' OR  sa.supportmethod = 'B' then 1 else 0 end )  as \"Fixed Over Phone\",");

            sb.AppendLine("sum (case when sa.supportmethod = 'D' then 1 else 0 end)  as \"Service Via Depot\",");
            sb.AppendLine("sum (case when sa.supportmethod = 'O' then 1 else 0 end)  as \"Fixed Onsite\",");
            sb.AppendLine("sum (case when sa.supportmethod = 'S' then 1 else 0 end ) as \"Parts Shipped\"");

            sb.AppendLine("from callcenter cc");
            sb.AppendLine("left join supportactivity sa on sa.activitynumber = cc.activitynumber");
            sb.AppendLine("left join station st on st.stationid = cc.stationid");
            sb.AppendLine("where calldate >= '" + DateTime.Now.Year + "0101" + "' ");
            sb.AppendLine("AND calldate <= '" + this.dpReportDate.GetDateText("yyyyMMdd") + "' ");

            if (!String.IsNullOrEmpty(cboCounty.SelectedValue))
            {
                sb.AppendLine("and ( st.county ='" + cboCounty.SelectedValue.ToUpper() + "')");
                // sb.AppendLine("and  UPPER(st.county) ='" + lstCounty.SelectedValue + "'");
            }

            sb.AppendLine("group by cc.stationid");
            sb.AppendLine("order by cc.stationid asc");
            return sb.ToString();
        }
    }
}