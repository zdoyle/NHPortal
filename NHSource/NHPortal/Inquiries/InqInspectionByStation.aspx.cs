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
using System.Globalization;
using System.Web.UI.HtmlControls;
using System.Web.Mvc;
using NHPortal.Classes.Reference;
using GDCoreUtilities.Logging;

namespace NHPortal
{
    public partial class InqInspectionByStation : NHPortal.Classes.PortalPage
    {
        private string m_Station;

        protected void Page_Load(object sender, EventArgs e)
        {
            RedirectOnInvalidPermission(UserPermissions.InspectionInquiryByStation);
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
            Master.SetHeaderText("Inspection History by Station and Date");

            // Set date to today's date
            dpInqDate.Text = DateTime.Now.ToString("M/d/yyyy");
        }

        private void ProcessPostBack()
        {
            string action = Master.MasterPage.HidActionValue;
            m_Station = NHPortalUtilities.ToStationID(tbStation.Text);
            switch (action)
            {
                case "RUN_REPORT":
                    RunReport();
                    break;

                case "SAVE_FAVORITE":
                    SaveFavorite();
                    break;

                case "SELECT_ROW":
                    RunDetailsReport();
                    break;
            }
        }

        private void SetMetaData()
        {
            if (Master.UserReport != null)
            {
                Master.UserReport.MetaData.Add("Station", m_Station);
                Master.UserReport.MetaData.Add("Date", dpInqDate.Text);
            }
        }

        private void RunReport()
        {
            string qry = BuildQuery();
            GDDatabaseClient.Oracle.OracleResponse response = ODAP.GetDataTable(qry, DatabaseTarget.OLTP);
            if (response.Successful)
            {
                LogMessage("RunReport - Response Successful", LogSeverity.Information);
                Master.UserReport = new Report("Inspection History by Station and Date", response.ResultsTable);
                SetMetaData();
                Master.SetDataLastUpdate(DateTime.Now);
                if (response.HasResults)
                {
                    LogMessage("RunReport - HasResults", LogSeverity.Information);
                    Master.UserReport.Columns.Insert("Details", "Select", 0);
                    AddDetailSelectColumn();
                    FormatReportValues();
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

        private void RunDetailsReport()
        {
            string qry = BuildDetailsQuery();
            GDDatabaseClient.Oracle.OracleResponse response = PortalFramework.Database.ODAP.GetDataRow(qry, PortalFramework.Database.DatabaseTarget.OLTP);
            if (response.Successful)
            {
                LogMessage("RunDetailsReport - Successful", LogSeverity.Information);
                if (response.HasResults)
                {
                    LogMessage("RunDetailsReport - HasResults", LogSeverity.Information);
                    DataRow row = response.ResultsRow;
                    DataRow rawRow = response.ResultsRow;
                    ltrlDetailsRpt.Text = Classes.Reference.InqInspectionBy.GenerateDetailsTable(row);
                    if ((rawRow["Safety Result"].ToString()[0] == 'C') || (rawRow["Safety Result"].ToString()[0] == 'R'))
                    {
                        string qrySafety = BuildSafetyBreakdownQuery();
                        GDDatabaseClient.Oracle.OracleResponse responseSafety = PortalFramework.Database.ODAP.GetDataRow(qrySafety, PortalFramework.Database.DatabaseTarget.Adhoc);
                        if ((responseSafety.Successful) && (responseSafety.HasResults))
                        {
                            DataRow rowSafety = responseSafety.ResultsRow;
                            ltrlDetailsRpt.Text += Classes.Reference.InqInspectionBy.GenerateSafetyBreakdownTable(rowSafety);
                        }
                    }
                    hidShowOverlay.Value = "TRUE";
                }
            }
            LogOracleResponse(response);
        }

        private string BuildQuery()
        {
            string sqlInqDate = dpInqDate.GetDateText();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("   SELECT ");
            sb.AppendLine("           T.TESTDATE || T.TESTENDTIME AS \"Test Date\" ");
            sb.AppendLine(",          T.VIN AS \"VIN\" ");
            sb.AppendLine(",          T.TESTASMAKE AS \"Make\" ");
            sb.AppendLine(",          T.TESTASMODELYR AS \"Model Year\" ");
            sb.AppendLine(",          T.EMISSTESTTYPE AS \"Test Type\" ");
            sb.AppendLine(",          CASE T.OVERALLPF ");
            sb.AppendLine("                WHEN '6' THEN 'ABORT' ");
            sb.AppendLine("                WHEN '0' THEN 'ABORT' ");
            sb.AppendLine("                WHEN '9' THEN 'N/A' ");
            sb.AppendLine("           ELSE CASE SUBSTR(T.REPAIRAMOUNTDURINGTEST,1,1) ");
            sb.AppendLine("                     WHEN 'N' THEN 'NO VOLT' ");
            sb.AppendLine("                ELSE CASE SUBSTR(T.OBDMILCMDON,2,1) ");
            sb.AppendLine("                          WHEN '3' THEN 'NO COMM' ");
            sb.AppendLine("                          WHEN '7' THEN 'N/A' ");
            sb.AppendLine("                          WHEN '6' THEN 'N/A' ");
            sb.AppendLine("                     ELSE CASE SUBSTR(T.REPAIRAMOUNTDURINGTEST,5,1) ");
            sb.AppendLine("                               WHEN 'V' THEN 'N/A' ");
            sb.AppendLine("                          ELSE CASE T.OBDLOCATABLE ");
            sb.AppendLine("                                    WHEN '3' THEN 'N/A' ");
            sb.AppendLine("                               ELSE CASE T.OBDPF ");
            sb.AppendLine("                                         WHEN '9' THEN 'UNKNOWN' ");
            sb.AppendLine("                                    ELSE CASE PR.READINESSPATTERN ");
            sb.AppendLine("                                              WHEN CASE WHEN T.OBDMISFIRE IN ('1','2') THEN 'S' ");
            sb.AppendLine("                                                        WHEN T.OBDMISFIRE = '4' THEN 'U' END||  ");
            sb.AppendLine("                                                   CASE WHEN T.OBDFUELSYSTEM IN ('1','2') THEN 'S'  ");
            sb.AppendLine("                                                        WHEN T.OBDFUELSYSTEM = '4' THEN 'U' END||  ");
            sb.AppendLine("                                                   CASE WHEN T.OBDCOMPONENT IN ('1','2') THEN 'S'  ");
            sb.AppendLine("                                                        WHEN T.OBDCOMPONENT = '4' THEN 'U' END||  ");
            sb.AppendLine("                                                   CASE WHEN T.OBDCATALYST IN ('1','2') THEN 'S'  ");
            sb.AppendLine("                                                        WHEN T.OBDCATALYST = '4' THEN 'U' END||  ");
            sb.AppendLine("                                                   CASE WHEN T.OBDHEATEDCATALYST IN ('1','2') THEN 'S'  ");
            sb.AppendLine("                                                        WHEN T.OBDHEATEDCATALYST = '4' THEN 'U' END||  ");
            sb.AppendLine("                                                   CASE WHEN T.OBDEVAP IN ('1','2') THEN 'S'  ");
            sb.AppendLine("                                                        WHEN T.OBDEVAP = '4' THEN 'U' END||  ");
            sb.AppendLine("                                                   CASE WHEN T.OBDAIRSYSTEM IN ('1','2') THEN 'S'  ");
            sb.AppendLine("                                                        WHEN T.OBDAIRSYSTEM = '4' THEN 'U' END||  ");
            sb.AppendLine("                                                   CASE WHEN T.OBDAC IN ('1','2') THEN 'S'  ");
            sb.AppendLine("                                                        WHEN T.OBDAC = '4' THEN 'U' END||  ");
            sb.AppendLine("                                                   CASE WHEN T.OBDOXYGEN IN ('1','2') THEN 'S'  ");
            sb.AppendLine("                                                        WHEN T.OBDOXYGEN = '4' THEN 'U' END||  ");
            sb.AppendLine("                                                   CASE WHEN T.OBDHEATEDOXYGEN IN ('1','2') THEN 'S'  ");
            sb.AppendLine("                                                        WHEN T.OBDHEATEDOXYGEN = '4' THEN 'U' END||  ");
            sb.AppendLine("                                                   CASE WHEN T.OBDEGR IN ('1','2') THEN 'S'  ");
            sb.AppendLine("                                                        WHEN T.OBDEGR = '4' THEN 'U' END THEN 'MATCH' ");
            sb.AppendLine("                                                   ELSE 'NO MATCH' ");
            sb.AppendLine("                                         END    ");
            sb.AppendLine("                                    END    ");
            sb.AppendLine("                               END    ");
            sb.AppendLine("                          END    ");
            sb.AppendLine("                     END    ");
            sb.AppendLine("                END    ");
            sb.AppendLine("           END AS \"Readiness\" ");
            sb.AppendLine(",          CASE NVL(SUBSTR(O.OBDDATA,1,2),'NOPATTERN') ");
            sb.AppendLine("              WHEN PV.PROTOCOLID THEN 'MATCH' ");
            sb.AppendLine("              WHEN 'NOPATTERN' THEN 'UNKNOWN' ");
            sb.AppendLine("           ELSE CASE SUBSTR(O.OBDDATA,1,1) ");
            sb.AppendLine("                     WHEN 'T' THEN 'MATCH READINESS' ");
            sb.AppendLine("                ELSE 'NO MATCH' ");
            sb.AppendLine("                END ");
            sb.AppendLine("           END AS \"Protocol\" ");
            sb.AppendLine(",          T.OVERALLPF AS \"Overall Result\" ");
            sb.AppendLine("      FROM TESTRECORD T ");
            sb.AppendLine(" LEFT OUTER JOIN OBDSTUDYTABLE O ON T.VEHICLESEQUENCENUM = O.VSN AND T.TESTDATE = O.TESTDATE AND T.TESTENDTIME = O.TESTTIME ");
            sb.AppendLine(" LEFT OUTER JOIN PMV_OBDPROTOCOL_TRUTHTABLE@NH_ADHOC_PRIMARY PV ON PV.VIN10 = (SUBSTR(T.VIN,1,8) || SUBSTR(T.VIN,10,2)) ");
            sb.AppendLine(" LEFT OUTER JOIN PMV_OBDREADINESS_TRUTHTABLE@NH_ADHOC_PRIMARY PR ON PR.VIN10 = (SUBSTR(T.VIN,1,8) || SUBSTR(T.VIN,10,2)) ");
            sb.AppendLine("     WHERE T.STATIONID = '" + m_Station + "' ");
            sb.AppendLine("       AND T.TESTDATE = '" + sqlInqDate.ToString() + "' ");
            sb.AppendLine("       AND T.OBDPF != '6' ");
            sb.AppendLine("  ORDER BY T.TESTDATE DESC, T.TESTENDTIME DESC ");

            LogMessage("BuildQuery SQL:" + sb.ToString(), LogSeverity.Information);
            return sb.ToString();
        }

        private string BuildDetailsQuery()
        {
            int idx = GDCoreUtilities.NullSafe.ToInt(hidSelectedIndex.Value);
            var row = Master.UserReport.Rows[idx];

            string strDetailsTestDate;
            string strDetailsTestEndTime;

            NHPortal.Classes.NHPortalUtilities.TryConvertDateTimeFormat(row[1].Value, "M/d/yyyy hh:mm:ss tt", "yyyyMMdd", out strDetailsTestDate);
            NHPortal.Classes.NHPortalUtilities.TryConvertDateTimeFormat(row[1].Value, "M/d/yyyy hh:mm:ss tt", "HHmmss", out strDetailsTestEndTime);

            StringBuilder builder = new StringBuilder();

            builder = Classes.Reference.InqInspectionBy.InqInspectionBySQLbyOLTP(builder);
            builder.AppendLine("    WHERE T.STATIONID = '" + m_Station + "' ");
            builder.AppendLine("      AND T.TESTDATE = '" + strDetailsTestDate + "' ");
            builder.AppendLine("      AND T.TESTENDTIME = '" + strDetailsTestEndTime + "' ");
            builder.AppendLine(" ORDER BY T.TESTDATE DESC, T.TESTENDTIME DESC");

            LogMessage("BuildDetailsQuery SQL:" + builder.ToString(), LogSeverity.Information);
            return builder.ToString();
        }

        private string BuildSafetyBreakdownQuery()
        {
            int idx = GDCoreUtilities.NullSafe.ToInt(hidSelectedIndex.Value);
            var row = Master.UserReport.Rows[idx];

            string strDetailsTestDate;
            string strDetailsTestEndTime;

            NHPortal.Classes.NHPortalUtilities.TryConvertDateTimeFormat(row[1].Value, "M/d/yyyy hh:mm:ss tt", "yyyyMMdd", out strDetailsTestDate);
            NHPortal.Classes.NHPortalUtilities.TryConvertDateTimeFormat(row[1].Value, "M/d/yyyy hh:mm:ss tt", "HHmmss", out strDetailsTestEndTime);

            string strVIN = row[2].Value;

            StringBuilder builder = new StringBuilder();
            builder = Classes.Reference.InqInspectionBy.InqInspectionSafetyBreakdownAdhoc(builder);
            builder.AppendLine("    WHERE T.VIN = '" + strVIN + "' ");
            builder.AppendLine("      AND T.TESTDATE = '" + strDetailsTestDate + "' ");
            builder.AppendLine("      AND T.TESTENDTIME = '" + strDetailsTestEndTime + "' ");
            builder.AppendLine(" ORDER BY T.TESTDATE DESC, T.TESTENDTIME DESC");

            LogMessage("BuildSafetyBreakdownQuery SQL:" + builder.ToString(), LogSeverity.Information);
            return builder.ToString();
        }

        private void FormatReportValues()
        {
            LogMessage("FormatReportValues - Start", LogSeverity.Information);
            string strDateTimeFormatIn, strDateTimeFormatOut;
            foreach (ReportRow row in Master.UserReport.Rows)
            {
                strDateTimeFormatIn = row["Test Date"].Value;
                if (NHPortal.Classes.NHPortalUtilities.TryConvertDateTimeFormat(strDateTimeFormatIn, "yyyyMMddHHmmss", "M/d/yyyy hh:mm:ss tt", out strDateTimeFormatOut))
                {
                    row["Test Date"].Value = strDateTimeFormatOut;
                }

                if (row["Test Type"].Value != "O")
                {
                    row["Protocol"].Value = "N/A";
                    row["Readiness"].Value = "N/A";
                }
                else if ((row["Overall Result"].Value == "9") || (row["Overall Result"].Value == "6") ||
                  (row["Overall Result"].Value == "0") || (row["Readiness"].Value == "NO COMM") ||
                    (row["Readiness"].Value == "NO VOLT") || (row["Readiness"].Value == "N/A"))
                {
                    row["Protocol"].Value = row["Readiness"].Value;
                }

                if (row["Protocol"].Value == "MATCH READINESS")
                {
                    row["Protocol"].Value = row["Readiness"].Value;
                }

                if (row["Protocol"].Value == "MATCH READINESS") row["Protocol"].Value = row["Readiness"].Value;
                if ((row["Protocol"].Value == "NO MATCH") && (row["Readiness"].Value == "UNKNOWN")) row["Protocol"].Value = row["Readiness"].Value;

                row["Overall Result"].Value = InqInspectionBy.GetOverallResultDescription(row["Overall Result"].Value[0]);  
                row["Test Type"].Value = Classes.Reference.EmissionTypes.GetDescription(row["Test Type"].Value[0]);
            }
            LogMessage("FormatReportValues - End", LogSeverity.Information);
        }

        private void AddDetailSelectColumn()
        {
            int i = 0;
            foreach (ReportRow row in Master.UserReport.Rows)
            {
                row[0].Value = "Select";
                row[0].OnClick = String.Format("javascript: selectRow({0})", i);
                row[0].Href = "#/";
                i += 1;
            }
        }

        private void SaveFavorite()
        {
            Master.SaveFavorite(NHPortal.UserControls.RedirectCodes.REPORT_INQ_INSPECTION_BY_STATION, UserFavoriteTypes.Report);
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
                        case "STATION":
                            tbStation.Text = c.Value;
                            break;

                        case "DATE":
                            dpInqDate.Text= c.Value;
                            break;
                    }
                }
            }
        }

        private void SetColumnTypes()
        {
            for (int i = 1; i < Master.UserReport.ColumnCount; i++)
            {
                Master.UserReport.Columns[i].ColumnDataType = ColumnDataType.String;
            }
            Master.UserReport.Columns["Details"].Exportable = false;
        }
    }
}