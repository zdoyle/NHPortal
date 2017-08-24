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
    public partial class CallsByDuration : NHPortal.Classes.PortalPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RedirectOnInvalidPermission(UserPermissions.CallsByDuration);

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
            Master.SetHeaderText("Calls By Duration Report");
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

        private void SaveFavorite()
        {
            Master.SaveFavorite(NHPortal.UserControls.RedirectCodes.REPORT_CALLS_BY_DURATION, UserFavoriteTypes.Report);
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
                Master.UserReport = new Report("Calls By Duration Report");
                SetMetaData();
                Master.SetDataLastUpdate(DateTime.Now);

                if (response.HasResults)
                {
                    LogMessage("Query has results", LogSeverity.Information);
                    Master.UserReport.FromDataTable(response.ResultsTable);
                    AddTotalRow();

                    SetColumnTypes();
                    Master.UserReport.Sortable = true;
                    Master.UserReport.FooterVisibility = FooterVisibility.Auto;
                    LogMessage("Data rendering to page", LogSeverity.Information);
                }
            }
            else
            {
                Master.SetError(response.Exception.Message);
                LogMessage("Query error: " + response.Exception, LogSeverity.Information);
            }
            LogMessage("Query Finshed", LogSeverity.Information);
            LogOracleResponse(response);
            Master.RenderReportToPage();
            LogMessage("Page Loaded", LogSeverity.Information);
            SessionHelper.SetCurrentReport(this.Session, Master.UserReport);
        }

        private string BuildQuery()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("SELECT te.NAME as \"Employee\",");
            sb.AppendLine("sum(case when cc.CALLDURATION >= 0 then 1 else 0 end) as \"Total Calls\", ");
            sb.AppendLine(" sum(case when cc.CALLDURATION >= 0 and cc.CALLDURATION < 200 then 1 else 0 end) as \"<2 Minutes\", ");
            sb.AppendLine(" sum(case when cc.CALLDURATION >= 200 and cc.CALLDURATION < 600 then 1 else 0 end) as \"2-5 Minutes\", ");
            sb.AppendLine("sum(case when cc.CALLDURATION >= 600 and cc.CALLDURATION < 1100 then 1 else 0 end) as \"6-10 Minutes\", ");
            sb.AppendLine("sum(case when cc.CALLDURATION >= 1100 and cc.CALLDURATION < 1600 then 1 else 0 end) as \"11-15 Minutes\",");
            sb.AppendLine("sum(case when cc.CALLDURATION >= 1600 and cc.CALLDURATION < 2100 then 1 else 0 end) as \"16-20 Minutes\", ");
            sb.AppendLine("sum(case when cc.CALLDURATION >= 2100 and cc.CALLDURATION < 2600 then 1 else 0 end) as \"21-25 Minutes\", ");
            sb.AppendLine("sum(case when cc.CALLDURATION >= 2600 and cc.CALLDURATION <= 3000 then 1 else 0 end) as \"26-30 Minutes\", ");
            sb.AppendLine(" sum(case when cc.CALLDURATION > 3000 then 1 else 0 end) as \">30 Minutes\" ");
            sb.AppendLine("FROM CALLCENTER cc, TECHEMPLOYEE te  WHERE (cc.EMPID = te.TECHEMPLOYEEID)");
            sb.AppendLine("AND cc.CALLDATE >= '" + dateSelector.StartDateControl.GetDateText("yyyyMMdd") + "' ");
            sb.AppendLine("AND cc.CALLDATE <= '" + dateSelector.EndDateControl.GetDateText("yyyyMMdd") + "' ");

            if (cboCallType.SelectedValue.Equals("P"))
            {
                sb.AppendLine("        AND     cc.stationid = 'JQPUBLIC'");
            }
            if (cboCallType.SelectedValue.Equals("S"))
            {
                sb.AppendLine("        AND     cc.stationid <> 'JQPUBLIC'");
            }
            sb.AppendLine("group by te.Name");

            return sb.ToString();
        }
        //The total  row (first row)
        private void AddTotalRow()
        {
            if (Master.UserReport != null)
            {
                Master.UserReport.Rows.Insert(0);

                int[] sums = new int[10];
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
                }

                ReportRow totalRow = Master.UserReport.Rows[0];
                totalRow.AddClass("static");
                totalRow.AddClass("total-row");
                totalRow["Employee"].Value = "Total";
                totalRow[1].Value = sums[0].ToString();
                totalRow[2].Value = sums[1].ToString();
                totalRow[3].Value = sums[2].ToString();
                totalRow[4].Value = sums[3].ToString();
                totalRow[5].Value = sums[4].ToString();
                totalRow[6].Value = sums[5].ToString();
                totalRow[7].Value = sums[6].ToString();
                totalRow[8].Value = sums[7].ToString();
                totalRow[9].Value = sums[8].ToString();
            }
        }
        private void SetColumnTypes()
        {
            for (int i = 1; i < Master.UserReport.ColumnCount; i++)
            {
                Master.UserReport.Columns[i].ColumnDataType = ColumnDataType.Number;
            }
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

                //if (cboCallType.ValuesSelected)
                //{
                //    Master.UserReport.MetaData.Add("Call Type", cboCallType.GetDelimitedText(", "), cboCallType.GetDelimitedValues(","));
                //}
            }
        }
    }
}