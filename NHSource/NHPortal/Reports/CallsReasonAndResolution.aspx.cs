using GDCoreUtilities.Logging;
using NHPortal.Classes;
using NHPortal.Classes.User;
using PortalFramework.Database;
using PortalFramework.ReportModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NHPortal.Reports
{
    public partial class CallsReasonAndResolution : NHPortal.Classes.PortalPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RedirectOnInvalidPermission(UserPermissions.ReasonsAndResolutions);

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
            Master.SetHeaderText("Reason and Resolution Report");
            cboCallType.Initialize();
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
            // set to start of month for month to date report
            //dateSelector.StartDateControl.Text = DateTime.Now.ToString("M/1/yyyy");

        }
        private void SetMetaData()
        {
            if (Master.UserReport != null)
            {
                Master.UserReport.MetaData.Add("Start Date", dateSelector.StartDateControl.Text);
                Master.UserReport.MetaData.Add("End Date", dateSelector.EndDateControl.Text);

                if (!String.IsNullOrEmpty(cboCallType.SelectedValue))
                {
                    Master.UserReport.MetaData.Add("Call Type", cboCallType.SelectedItem.ToString());
                }
            }
        }

        private void SaveFavorite()
        {
            Master.SaveFavorite(NHPortal.UserControls.RedirectCodes.REPORT_REASONS_RESOLUTION, UserFavoriteTypes.Report);
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
                        case "START DATE":
                            dateSelector.StartDateControl.Text = c.Value;
                            break;
                        case "END DATE":
                            dateSelector.EndDateControl.Text = c.Value;
                            break;
                        case "CALL TYPE":
                            cboCallType.SelectedValue = c.Value;
                            if (c.Value == "Public")
                            {
                                cboCallType.SelectedValue = "P";
                            }
                            if (c.Value == "Station")
                            {
                                cboCallType.SelectedValue = "S";
                            }
                            break;
                    }
                }
            }
        }

        private void RunReport()
        {
            string qry = BuildQuery();
            GDDatabaseClient.Oracle.OracleResponse response = ODAP.GetDataTable(qry, DatabaseTarget.OLTP);
            LogMessage("Attempting to run query...", LogSeverity.Information);
            if (response.Successful)
            {
                LogMessage("Query is successful", LogSeverity.Information);
                Master.UserReport = new Report("Reason and Resolution Report");
                SetMetaData();
                Master.SetDataLastUpdate(DateTime.Now);

                if (response.HasResults)
                {
                    LogMessage("Query has results", LogSeverity.Information);
                    Master.UserReport.FromDataTable(response.ResultsTable);
                    AddTotalRow();
                    SetColumnTypes();
                    Master.UserReport.Columns.Insert("Total", "Total", 1);
                    CalculateTotalColumn();
                    Master.UserReport.Sortable = true;
                    Master.UserReport.FooterVisibility = FooterVisibility.Auto;
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
            Master.RenderReportToPage();
            LogMessage("Page Loaded", LogSeverity.Information);
            SessionHelper.SetCurrentReport(this.Session, Master.UserReport);
        }
        //this is the column for total (row[Total])
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
            sb.AppendLine("SELECT RC.DESCRIPTION as \"Call Reason\", ");
            sb.AppendLine("SUM(case when callresolution = 'B' THEN 1  else 0 END) as \"Refer To Billing\",");
            sb.AppendLine("SUM(case when callresolution = 'C' THEN 1  else 0  END) as \"Refer To Call Manager\",");
            sb.AppendLine("SUM(case when callresolution = 'D' THEN 1   else 0 END) as \"Refer To State\",");
            sb.AppendLine("SUM(case when callresolution = 'F' THEN 1   else 0 END) as \"Resolved Flow Chart\",");
            sb.AppendLine("SUM(case when callresolution = 'I' THEN 1   else 0 END) as \"Info Provided\",");
            sb.AppendLine("SUM(case when callresolution = 'M' THEN 1   else 0 END) as \"Refer To Prgm Mngr\",");
            sb.AppendLine("SUM(case when callresolution = 'P' THEN 1   else 0 END) as \"Issue Corrected\",");
            sb.AppendLine("SUM(case when callresolution = 'R' THEN 1   else 0 END) as \"Reset Password\",");
            sb.AppendLine("SUM(case when callresolution = 'S' THEN 1   else 0 END) as \"Shipping Parts\",");
            sb.AppendLine("SUM(case when callresolution = 'T' THEN 1   else 0 END) as \"Escalate To Tech\",");
            sb.AppendLine("SUM(case when callresolution = 'X' THEN 1   else 0 END) as \"Other\"");
            sb.AppendLine("FROM R_REASONFORCALL RC");
            //sb.AppendLine("INNER JOIN TECHEMPLOYEE TE ON CC.EMPID = TE.TECHEMPLOYEEID");
            sb.AppendLine("LEFT OUTER JOIN CALLCENTER CC on RC.CODE_VALUE = CC.REASONFORCALL");
            sb.AppendLine("AND (CALLDATE >= '" + dateSelector.StartDateControl.GetDateText("yyyyMMdd") + "' ");
            sb.AppendLine("AND CALLDATE<= '" + dateSelector.EndDateControl.GetDateText("yyyyMMdd") + "' )");
            sb.AppendLine("AND (cc.CallNumber>0)");
            if (cboCallType.SelectedValue.Equals("P"))
            {
                sb.AppendLine("        AND     cc.stationid = 'JQPUBLIC'");
            }
            if (cboCallType.SelectedValue.Equals("S"))
            {
                sb.AppendLine("        AND     cc.stationid <> 'JQPUBLIC'");
            }
            sb.AppendLine("group by RC.CODE_VALUE,  DESCRIPTION");
            sb.AppendLine("order by RC.CODE_VALUE ASC");
            return sb.ToString();
        }

        //Total row (total under call reason)
        private void AddTotalRow()
        {
            if (Master.UserReport != null)
            {
                Master.UserReport.Rows.Insert(0);

                int[] sums = new int[11];
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
                }

                ReportRow totalRow = Master.UserReport.Rows[0];
                totalRow.AddClass("static");
                totalRow.AddClass("total-row");
                totalRow["Call Reason"].Value = "Total";
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
            }
        }
    }
}


