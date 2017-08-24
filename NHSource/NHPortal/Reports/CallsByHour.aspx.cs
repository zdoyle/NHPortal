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
    public partial class CallsByHour : NHPortal.Classes.PortalPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RedirectOnInvalidPermission(UserPermissions.CallsByHour);

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

        private void SaveFavorite()
        {
            Master.SaveFavorite(NHPortal.UserControls.RedirectCodes.REPORT_CALLS_BY_HOUR, UserFavoriteTypes.Report);
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
                            //if (c.Value == "Public")
                            //{
                            //    cboCallType.SelectedValue = "P";
                            //}
                            //if (c.Value == "Station")
                            //{
                            //    cboCallType.SelectedValue = "S";
                            //}
                            break;
                    }
                }
            }
        }

        private void RunReport()
        {
            string qry = BuildQuery();
            GDDatabaseClient.Oracle.OracleResponse response = ODAP.GetDataTable(qry, DatabaseTarget.OLTP);
            if (response.Successful)
            {
                Master.UserReport = new Report("Calls By Hour Report");
                SetMetaData();

                if (response.HasResults)
                {
                    Master.UserReport.FromDataTable(response.ResultsTable);
                    ConvertHoursColumn();
                    AddTotalRow();
                    SetColumnTypes();
                }
            }
            else
            {
                Master.SetError(response.Exception);
            }

            LogOracleResponse(response);
            Master.RenderReportToPage();
            SessionHelper.SetCurrentReport(this.Session, Master.UserReport);
        }

        private string BuildQuery()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("select  a.call_hour as \"Call Hour\"");
            sb.AppendLine("        , nvl( b.all_sum, 0 ) as \"All\"");
            sb.AppendLine("        , nvl( b.public_sum, 0 ) as \"Public\"");
            sb.AppendLine("        , nvl( b.station_sum, 0 ) as \"Station\"");
            sb.AppendLine("from");
            sb.AppendLine("(");
            sb.AppendLine("    SELECT  lpad( rownum - 1, 2, '0' ) as call_hour,");
            sb.AppendLine("            0 as all_sum, 0 as public_sum, 0 as station_sum");
            sb.AppendLine("    from    dual");
            sb.AppendLine("    connect by level <= 24");
            sb.AppendLine(") a");
            sb.AppendLine("left join");
            sb.AppendLine("(");
            sb.AppendLine("    SELECT  call_hour,");
            sb.AppendLine("            sum( 1 ) as all_sum,");
            sb.AppendLine("            sum( case when stationid = 'JQPUBLIC' THEN 1 ELSE 0 END ) AS public_sum,");
            sb.AppendLine("            sum( case when stationid <> 'JQPUBLIC' THEN 1 ELSE 0 END ) AS station_sum");
            sb.AppendLine("    FROM");
            sb.AppendLine("    (");
            sb.AppendLine("        SELECT  cc.stationid,");
            sb.AppendLine("                SUBSTR( LPAD( cc.calltime, 6, '0' ), 0, 2 ) AS call_hour");
            sb.AppendLine("        FROM    callcenter cc");
            sb.AppendLine("        WHERE   cc.callnumber > 0");
            sb.AppendLine("        AND     cc.calldate >= '" + dateSelector.StartDateControl.GetDateText() + "'");
            sb.AppendLine("        AND     cc.calldate <= '" + dateSelector.EndDateControl.GetDateText() + "'");
            if (cboCallType.SelectedValue.Equals("P"))
            {
                sb.AppendLine("        AND     cc.stationid = 'JQPUBLIC'");
            }

            if (cboCallType.SelectedValue.Equals("S"))
            {
                sb.AppendLine("        AND     cc.stationid <> 'JQPUBLIC'");
            }
            sb.AppendLine("    )");
            sb.AppendLine("    GROUP BY call_hour");
            sb.AppendLine(") b");
            sb.AppendLine("on a.call_hour = b.call_hour");
            sb.AppendLine("ORDER BY a.call_hour");
            return sb.ToString();
        }

        private void AddTotalRow()
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
                totalRow.AddClass("total-row");
                totalRow["Call Hour"].Value = "ALL";
                totalRow[1].Value = sums[0].ToString();
                totalRow[2].Value = sums[1].ToString();
                totalRow[3].Value = sums[2].ToString();
            }
        }

        private void ConvertHoursColumn()
        {
            if (Master.UserReport != null)
            {
                string callText;
                DateTime hour;
                foreach (ReportRow row in Master.UserReport.Rows)
                {
                    try
                    {
                        if (!String.IsNullOrEmpty(row["Call Hour"].Value))
                        {
                            hour = DateTime.ParseExact(row["Call Hour"].Value, "HH", System.Globalization.CultureInfo.InvariantCulture);
                            callText = hour.ToString("hh:mm") + " - " + hour.AddMinutes(59).ToString("h:mm tt");
                            row["Call Hour"].Value = callText;
                        }
                    }
                    catch (FormatException ex)
                    {
                        LogException(ex);
                    }
                }
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

                Master.UserReport.MetaData.Add("Call Type", cboCallType.SelectedItem.Text,
                     cboCallType.SelectedValue);

                //if (cboCallType.SelectedIndex > 0)
                //{
                //    Master.UserReport.MetaData.Add("Call Type", cboCallType.SelectedItem.ToString());
                //}
            }
        }
    }
}