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
using GDCoreUtilities.Logging;

namespace NHPortal.Reports
{
    public partial class SuspectTestReport : NHPortal.Classes.PortalPage
    {
        private string m_inputId;

        protected void Page_Load(object sender, EventArgs e)
        {
            RedirectOnInvalidPermission(UserPermissions.SuspectTestReport);

            if (IsPostBack)
            {
                LogMessage("IsPostBack - Successful", LogSeverity.Information);
                ProcessPostBack();
            }
            else
            {
                LogMessage("IsPostBack - Else", LogSeverity.Information);
                InitializePage();
                LoadFavorite();
            }
        }

        private void InitializePage()
        {
            Master.SetHeaderText("NHOST Suspect Test Report");
        }

        private void ProcessPostBack()
        {
            string action = Master.MasterPage.HidActionValue;
            SetInputId();

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

        private void RunReport()
        {
            string qry = BuildQuery();
            GDDatabaseClient.Oracle.OracleResponse response = ODAP.GetDataTable(qry, DatabaseTarget.Adhoc);
            if (response.Successful)
            {
                LogMessage("RunReport - Response Successful", LogSeverity.Information);
                Master.UserReport = new Report("NHOST Suspect Test Report", response.ResultsTable);
                SetMetaData();
                Master.SetDataLastUpdate(DateTime.Now);
                if (response.HasResults)
                {
                    LogMessage("RunReport - HasResults", LogSeverity.Information);
                    Master.UserReport.Columns.Add("Percentage", "%", ColumnDataType.Percentage);
                    CalculateReportPercentRowValues();
                    CalculateTotalRowValue();
                    SetColumnTypes();
                    Master.UserReport.Sortable = true;
                    Master.UserReport.FooterVisibility = FooterVisibility.Auto;
                }
            }
            else
            {
                LogMessage("RunReport - UnSuccessful", LogSeverity.Warning);
                Master.UserReport = null;
                Master.SetError(response.Exception);
            }

            LogOracleResponse(response);
            Master.RenderReportToPage();
            SessionHelper.SetCurrentReport(this.Session, Master.UserReport);
        }

        private string BuildQuery()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("      SELECT INSPECTORID AS \"Mechanic ID\" ");
            sb.AppendLine(",            SUM(TEST_COUNT) AS \"Total Test Count\" ");
            sb.AppendLine(",            SUM(SUSPECT_COUNT) AS \"Suspect Test Count\" ");
            sb.AppendLine("        FROM ((SELECT INSPECTORID, 0  AS TEST_COUNT, COUNT(*) AS SUSPECT_COUNT ");
            sb.AppendLine("                 FROM (SELECT * ");
            sb.AppendLine("                         FROM CANDIDATE_OBDVIN_MISMATCH ");
            sb.AppendLine("                UNION SELECT * FROM CANDIDATE_OBDPROTOCOL_MISMATCH ");
            sb.AppendLine("                UNION SELECT * FROM CANDIDATE_READINESS_MISMATCH) ");
            sb.AppendLine("       WHERE STATIONID = '" + m_inputId + "' ");
            sb.AppendLine("         AND TESTDATE >= '" + dateSelector.StartDateControl.GetDateText("yyyyMMdd") + "' AND TESTDATE <= '" + dateSelector.EndDateControl.GetDateText("yyyyMMdd") + "' ");
            sb.AppendLine("    GROUP BY INSPECTORID) ");
            sb.AppendLine(" UNION ");
            sb.AppendLine("      (SELECT NT.INSPECTORID AS \"Mechanic ID\" ");
            sb.AppendLine(",             COUNT(*) AS \"Total Test Count\" ");
            sb.AppendLine(",             0 AS \"Suspect Test Count\" ");
            sb.AppendLine("        FROM NEW_TESTRECORD NT  ");
            sb.AppendLine("       WHERE STATIONID = '" + m_inputId + "' ");
            sb.AppendLine("         AND TESTDATE >= '" + dateSelector.StartDateControl.GetDateText("yyyyMMdd") + "' AND TESTDATE <= '" + dateSelector.EndDateControl.GetDateText("yyyyMMdd") + "' ");
            sb.AppendLine("         AND NT.EMISSTESTTYPE = 'O' AND NT.OVERALLPF <> '6' ");
            sb.AppendLine("    GROUP BY NT.INSPECTORID)) ");
            sb.AppendLine(" GROUP BY INSPECTORID ");
            sb.AppendLine(" ORDER BY \"Suspect Test Count\" DESC ");

            return sb.ToString();
        }

        private void SetMetaData()
        {
            string stationName = GetStationName();
            if (Master.UserReport != null)
            {
                Master.UserReport.MetaData.Add("Start Date", dateSelector.StartDateControl.Text);
                Master.UserReport.MetaData.Add("End Date", dateSelector.EndDateControl.Text);
                Master.UserReport.MetaData.Add("Station Name", stationName, m_inputId);
            }
        }

        // Get station name to display in MetaData
        private string GetStationName()
        {
            string stationName = m_inputId;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(" SELECT NAME ");
            sb.AppendLine("   FROM STATION ");
            sb.AppendLine("  WHERE STATIONID = '" + m_inputId + "' ");

            string nameQry = sb.ToString();
            GDDatabaseClient.Oracle.OracleResponse response = ODAP.GetDataRow(nameQry, DatabaseTarget.Adhoc);

            // Set default station name to station number
            stationName = m_inputId.Trim();
            if (response.Successful && response.HasResults)
            {
                stationName = GDCoreUtilities.StringUtilities.ToTitleCase(response.ResultsRow["NAME"].ToString().Trim());
            }
            else
            {
                LogException(response.Exception);
            }
            return stationName;
        }

        private int CalculateTotalColumn(string rowName)
        {
            int columnTotal = 0;
            foreach (ReportRow row in Master.UserReport.Rows)
            {
                columnTotal += GDCoreUtilities.NullSafe.ToInt(row[rowName].Value, 0);
            }
            return columnTotal;
        }


        // Calculate the percentage of suspect tests for each inspectorID (Mechanic ID)
        private void CalculateReportPercentRowValues()
        {
            foreach (ReportRow row in Master.UserReport.Rows)
            {
                row["Percentage"].Value = CalculatePercentValue(GDCoreUtilities.NullSafe.ToInt(row["Suspect Test Count"].Value), GDCoreUtilities.NullSafe.ToInt(row["Total Test Count"].Value));
            } 
        }

        private string CalculatePercentValue(int num, int den)
        {
            string percent = "0.0%";
            if (den != 0)
            {
                double d = (double)num / (double)den;
                percent = d.ToString("0.00%");
            }
            return percent;
        }

        // Calculate sum of all rows and display in totals row
        private void CalculateTotalRowValue()
        {
            if (Master.UserReport != null)
            {
                Master.UserReport.Rows.Insert(0);

                int[] sums = new int[3];
                foreach (ReportRow row in Master.UserReport.Rows)
                {
                    sums[0] += row[1].ValueAsInt;
                    sums[1] += row[2].ValueAsInt;
                    sums[2] += row[3].ValueAsInt;
                }
                ReportRow totalRow = Master.UserReport.Rows[0];
                totalRow.AddClass("static");
                totalRow.AddClass("total_row");
                totalRow["Mechanic ID"].Value = "Total";
                totalRow[1].Value = sums[0].ToString();
                totalRow[2].Value = sums[1].ToString();
                totalRow[3].Value = sums[2].ToString();

                totalRow[3].Value = CalculatePercentValue(Convert.ToInt32(totalRow[2].Value), Convert.ToInt32(totalRow[1].Value));
            }          
        }

        private void SaveFavorite()
        {
            Master.SaveFavorite(NHPortal.UserControls.RedirectCodes.REPORT_SUSPECT_TEST, UserFavoriteTypes.Report);
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
                        case "STATION NAME":
                            tbStation.Text = c.Value;
                            break;
                    }
                }
            }
        }
        private void SetInputId()
        {
                m_inputId = NHPortalUtilities.ToStationID(tbStation.Text);      
        }
        private void SetColumnTypes()
        {
            for (int i = 1; i < Master.UserReport.ColumnCount; i++)
            {
                Master.UserReport.Columns[i].ColumnDataType = ColumnDataType.Number;
            }
            Master.UserReport.Columns["Mechanic ID"].ColumnDataType = ColumnDataType.String;
            Master.UserReport.Columns["Percentage"].ColumnDataType = ColumnDataType.Percentage;
        }
    }
}