using GDCoreUtilities.Logging;
using NHPortal.Classes;
using NHPortal.Classes.User;
using PortalFramework.ReportModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NHPortal.Reports
{
    public partial class ComponentPerformance : PortalPage
    {
        public const string ReportTitle = "Component Performance Report";
        public const string CSVFilePrefix = "PerformanceReport";

        /// <summary>Gets the directory to use for CSV based reports.</summary>
        public static string CsvFilePath
        {
            get { return PortalFramework.PortalIniSettings.Settings.GetValue("DIR_COMPONENT_PERFORMANCE"); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            RedirectOnInvalidPermission(UserPermissions.ComponentPerformance);

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
            Master.SetHeaderText("Component Performance Report");
            dpReportDate.Text = DateTime.Today.ToShortDateString();
            cboCounty.Initialize();
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
               // lstCounty.SetSelectedValue(cnty);
            }
        }
        private void SaveFavorite()
        {
            Master.SaveFavorite(NHPortal.UserControls.RedirectCodes.REPORT_COMPONENT_PERFORMANCE, UserFavoriteTypes.Report);
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

        private void RunReport()
        {
            //ReportFromSQL();
            ReportFromCSV();            
            SetMetaData();
            Master.RenderReportToPage();
            SessionHelper.SetCurrentReport(this.Session, Master.UserReport);
        }

        private void ReportFromSQL()
        {
            //runs query
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
                    AddTotalRow();
                    Master.UserReport.Columns.Insert("TOTAL", "TOTAL", 1);
                    CalculateTotalColumn();
                    SetColumnTypes();
                    Master.UserReport.FooterVisibility = FooterVisibility.Auto;
                    Master.UserReport.Sortable = true;
                    Master.UserReport.FooterNote = "* Total support calls; Total complaints per component.	 " + Environment.NewLine
                              + "Data grouped column-wise.";
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
            string file = NHPortalUtilities.BuildCSVFileName(CsvFilePath, CSVFilePrefix, dpReportDate.Date, cboCounty.SelectedItem.Text);
            try
            {
                NHPortalCSVReader csvReader = new NHPortalCSVReader(file);
                csvReader.Process();

                if (csvReader.Table != null)
                {
                    LogMessage("CSV Reader running", LogSeverity.Information);
                    Master.UserReport = new PortalFramework.ReportModel.Report("Component Performance Report", csvReader.Table);
                    Master.RenderReportToPage();
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
            sb.AppendLine("select");
            sb.AppendLine("(case when sm.code_value in ( 'A', 'B' ) then 'Fixed Over Phone'");
            sb.AppendLine("when sm.code_value in ('P', 'N','C' ) then 'All*'");
            sb.AppendLine("when sm.code_value in ('O') then 'Fixed Onsite' ");
            sb.AppendLine("when sm.code_value in ('S') then 'Parts Shipped' ");
            sb.AppendLine("when sm.code_value in ('D') then 'Service Via Depot' else sm.description end )as \"SUPPORT METHOD\",");
            sb.AppendLine("sum(case when cunknownfailure = 1 then 1 else 0 end) as \"UNKNOWN FAILURE\",");
            sb.AppendLine("sum(case when cpc = 1 then 1 else 0 end) as pc,");
            sb.AppendLine("sum(case when cmonitor = 1 then 1 else 0 end) as monitor,");
            sb.AppendLine("sum(case when ckeyboard = 1 then 1 else 0 end) as keyboard,");
            sb.AppendLine("sum(case when cmouse = 1 then 1 else 0 end) as mouse,");
            sb.AppendLine("sum(case when cbarcodescanner = 1 then 1 else 0 end) as \"BARCODE SCANNER\",");
            sb.AppendLine("sum(case when cprinter = 1 then 1 else 0 end) as printer,");
            sb.AppendLine("sum(case when cobdunit = 0 then 1 else 0 end) AS \"OBD Unit\",");
            sb.AppendLine("sum(case when corangeobdcable = 0 then 1 else 0 end) AS \"ORANGE CABLE\",");
            sb.AppendLine("sum(case when cblackobdcable = 0 then 1 else 0 end) AS \"BLACK CABLE\",");
            sb.AppendLine("sum(case when cpower = 0 then 1 else 0 end) power,");
            sb.AppendLine("sum(case when ccabinet = 0 then 1 else 0 end) cabinet,");
            sb.AppendLine("sum(case when cotherfailure = 0 then 1 else 0 end) AS \"OTHER FAILURE\",");
            sb.AppendLine("sum(case when cnoproblem = 0 then 1 else 0 end) AS \"NO PROBLEM\",");
            sb.AppendLine("sum(case when cblackink = 0 then 1 else 0 end) AS \"BLACK INK\",");
            sb.AppendLine("sum(case when ccolorink = 0 then 1 else 0 end) AS \"COLOR INK\",");
            sb.AppendLine("sum(case when cscannercable = 0 then 1 else 0 end) AS \"SCANNER CABLE\",");
            sb.AppendLine("sum(case when cprintercable = 0 then 1 else 0 end) AS \"PRINTER CABLE\",");
            sb.AppendLine("sum(case when cmodemcable = 0 then 1 else 0 end) AS \"MODEM CABLE\"");
            sb.AppendLine("from R_SUPPORT_METHOD sm");
            sb.AppendLine("left join supportactivity sa  on sm.CODE_VALUE = sa.supportmethod");
            sb.AppendLine("AND STATUS = 'C'");
            sb.AppendLine("AND (sa.SUPPORTTYPE = 'S') ");
            sb.AppendLine("left join unit un on sa.UNITID = un.UNITID");
            sb.AppendLine("AND (un.stationid>='00000000') ");
            sb.AppendLine("AND (un.stationid<='99999999') ");
            sb.AppendLine("left join station st on st.STATIONID = un.STATIONID");

            // TODO update this logic -- used to have a date range, now there is a single date field
            //if (!String.IsNullOrEmpty(dpStart.GetDateText("yyyyMMdd")))
            //{
            //    sb.AppendLine("Where sa.DATECALLINITIATED > '" + dpStart.GetDateText("yyyyMMdd") + "'");
            //    sb.AppendLine("AND sa.DATECLOSED < '" + dpEnd.GetDateText("yyyyMMdd") + "' ");
            //}
            //else
            //{
            //    sb.AppendLine("Where sa.DATECALLINITIATED > '" + cboRptPeriod.GetStartDateText("yyyyMMdd") + "'");//
            //    sb.AppendLine("AND sa.DATECLOSED < '" + cboRptPeriod.GetEndDateText("yyyyMMdd") + "' ");//
            //}

            if (!String.IsNullOrEmpty(cboCounty.SelectedValue))
            {
                sb.AppendLine("and ( st.county ='" + cboCounty.SelectedValue.ToUpper() + "')");
            }
            sb.AppendLine("group by (case when sm.code_value in ( 'A', 'B' ) then 'Fixed Over Phone' ");
            sb.AppendLine("when sm.code_value in ( 'P', 'N','C' ) then 'All*' ");
            sb.AppendLine("when sm.code_value in ('O') then 'Fixed Onsite' ");
            sb.AppendLine("when sm.code_value in ('S') then 'Parts Shipped' ");
            sb.AppendLine("when sm.code_value in ('D') then 'Service Via Depot'else sm.description end) ");
            sb.AppendLine("order by \"SUPPORT METHOD\" ASC ");
            return sb.ToString();
        }

        //total row for overall data 
        private void AddTotalRow()
        {
            if (Master.UserReport != null)
            {
                Master.UserReport.Rows.Insert(0);
                int[] sums = new int[19];
                foreach (ReportRow row in Master.UserReport.Rows)
                {
                    sums[0] += row[1].ValueAsInt;
                    sums[1] += row[2].ValueAsInt;
                    sums[2] += row[3].ValueAsInt;
                    sums[3] += row[4].ValueAsInt;
                    sums[4] += row[5].ValueAsInt;
                    sums[5] += row[6].ValueAsInt;
                    sums[6] += row[7].ValueAsInt;
                    sums[7] += row[8].ValueAsInt;
                    sums[8] += row[9].ValueAsInt;
                    sums[9] += row[10].ValueAsInt;
                    sums[10] += row[11].ValueAsInt;
                    sums[11] += row[12].ValueAsInt;
                    sums[12] += row[13].ValueAsInt;
                    sums[13] += row[14].ValueAsInt;
                    sums[14] += row[15].ValueAsInt;
                    sums[15] += row[16].ValueAsInt;
                    sums[16] += row[17].ValueAsInt;
                    sums[17] += row[18].ValueAsInt;
                    sums[18] += row[19].ValueAsInt;

                }

                ReportRow totalRow = Master.UserReport.Rows[0];
                totalRow.AddClass("static");
                totalRow["SUPPORT METHOD"].Value = "Total";

                totalRow[1].Value = sums[0].ToString();
                totalRow[2].Value = sums[1].ToString();
                totalRow[3].Value = sums[2].ToString();
                totalRow[4].Value = sums[3].ToString();
                totalRow[5].Value = sums[4].ToString();
                totalRow[6].Value = sums[5].ToString();
                totalRow[7].Value = sums[6].ToString();
                totalRow[8].Value = sums[7].ToString();
                totalRow[9].Value = sums[8].ToString();
                totalRow[10].Value = sums[9].ToString();
                totalRow[11].Value = sums[10].ToString();
                totalRow[12].Value = sums[11].ToString();
                totalRow[13].Value = sums[12].ToString();
                totalRow[14].Value = sums[13].ToString();
                totalRow[15].Value = sums[14].ToString();
                totalRow[16].Value = sums[15].ToString();
                totalRow[17].Value = sums[16].ToString();
                totalRow[18].Value = sums[17].ToString();
                totalRow[19].Value = sums[18].ToString();
            }
        }

        //this makes a Total column in the 2nd column 
        private void CalculateTotalColumn()
        {
            foreach (ReportRow row in Master.UserReport.Rows)
            {
                int total = 0;
                for (int i = 2; i < row.Cells.Count; i++)
                {
                    total += row.Cells[i].ValueAsInt;
                }
                row["Total"].Value = total.ToString();
            }
        }
        //this is where the data is displayed
        private string ConvertToHTML(DataTable dt)
        {
            string html = "<table>";
            html += "<tr><td>SUPPORT METHOD</td>";
            html += "<td>TOTAL</td>";
            html += "<td>UNKNOWN FAILURE</td>";
            html += "<td>PC</td>";
            html += "<td>MONITOR</td>";
            html += "<td>MOUSE</td>";
            html += "<td>BARCODE SCANNER</td>";
            html += "<td>PRINTER</td>";
            html += "<td>OBD UNIT</td>";
            html += "<td>ORANGE CABLE</td>";
            html += "<td>BLACK CABLE</td>";
            html += "<td>POWER</td>";
            html += "<td>CABINET</td>";
            html += "<td>OTHER FAILURE</td>";
            html += "<td>NO PROBLEM</td>";
            html += "<td>BLACK INK</td>";
            html += "<td>COLOR INK</td>";
            html += "<td>SCANNER CABLE</td>";
            html += "<td>PRINTER CABLE</td>";
            html += "<td>MODEM CABLE</td>";
            html += "</tr>";

            foreach (DataRow row in dt.Rows)
            {

                html += "<tr>";
                html += "<td>" + row[0] + "</td>";
                html += "<td>" + row[1] + "</td>";
                html += "<td>" + row[2] + "</td>";
                html += "<td>" + row[3] + "</td>";
                html += "<td>" + row[4] + "</td>";
                html += "<td>" + row[5] + "</td>";
                html += "<td>" + row[6] + "</td>";
                html += "<td>" + row[7] + "</td>";
                html += "<td>" + row[8] + "</td>";
                html += "<td>" + row[9] + "</td>";
                html += "<td>" + row[10] + "</td>";
                html += "<td>" + row[11] + "</td>";
                html += "<td>" + row[13] + "</td>";
                html += "<td>" + row[14] + "</td>";
                html += "<td>" + row[15] + "</td>";
                html += "<td>" + row[16] + "</td>";
                html += "<td>" + row[17] + "</td>";
                html += "<td>" + row[18] + "</td>";
                html += "<td>" + row[19] + "</td>";
                html += "</tr>";
            }
            html += "</table>";

            return html;
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
    }
}