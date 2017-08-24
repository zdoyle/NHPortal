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
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using NHPortal.Classes.Reference;
using System.Globalization;
using GDCoreUtilities.Logging;

namespace NHPortal
{
    public partial class InqInspectionByVIN : NHPortal.Classes.PortalPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RedirectOnInvalidPermission(UserPermissions.InspectionInquiryByVIN);
            if (IsPostBack)
            {
                LogMessage("IsPostBack Successful", LogSeverity.Information);
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
            Master.SetHeaderText("Inspection History by VIN");
        }

        private void ProcessPostBack()
        {
            string action = Master.MasterPage.HidActionValue;
            switch (action)
            {
                case "RUN_REPORT":
                    RunReport();
                    break;

                case "SELECT_ROW":
                    RunDetailsReport();
                    break;

                case "SAVE_FAVORITE":
                    SaveFavorite();
                    break;
            }
        }

        private void SetMetaData()
        {
            if (Master.UserReport != null)
            {
                Master.UserReport.MetaData.Add("VIN", tbVIN.Text);
            }
        }

        private void RunReport()
        {
            string qry = BuildQuery();
            GDDatabaseClient.Oracle.OracleResponse response = ODAP.GetDataTable(qry, DatabaseTarget.OLTP);
            if (response.Successful)
            {
                LogMessage("Run Report - Response Successful", LogSeverity.Information);
                Master.UserReport = new Report("Inspection History by VIN", response.ResultsTable);
                SetMetaData();
                Master.SetDataLastUpdate(DateTime.Now);
                if (response.HasResults)
                {
                    Master.UserReport.HeaderNote = "Vehicle Inspection History";
                    LogMessage("Run Report - Response HasResults", LogSeverity.Information);
                    Master.UserReport.Columns.Insert("Details", "Select", 0);
                    AddDetailSelectColumn();
                    FormatReportValues();
                    SetColumnTypes();
                    Master.UserReport.Sortable = true;
                    Master.UserReport.FooterVisibility = FooterVisibility.Auto;
                    RunBuildLastTestQuery(response.ResultsTable.Rows[0]);
                }
            }
            else
            {
                LogMessage("Run Report - Response UnSuccessful", LogSeverity.Warning);
                Master.UserReport = null;
                Master.SetError(response.Exception);
            }

            LogOracleResponse(response);
            Master.RenderReportToPage();
            SessionHelper.SetCurrentReport(this.Session, Master.UserReport);
        }

        private string BuildQuery()
        {
            LogMessage("BuildQuery - Start", LogSeverity.Information);
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("   SELECT ");
            sb.AppendLine("           T.TESTSEQUENCE AS \"Test Sequence\" ");
            sb.AppendLine(",          T.TESTDATE || T.TESTENDTIME AS \"Test Date\" ");
            sb.AppendLine(",          T.OBDPF AS \"Emis Result\" ");
            sb.AppendLine(",          T.SAFETYPF AS \"Safety Result\" ");
            sb.AppendLine(",          T.OVERALLPF AS \"Overall Result\" ");
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
            sb.AppendLine("                               ELSE CASE NVL(PR.READINESSPATTERN,'NOPATTERN') ");
            sb.AppendLine("                                         WHEN 'NOPATTERN' THEN 'UNKNOWN' ");
            sb.AppendLine("                                    ELSE CASE PR.READINESSPATTERN ");
            sb.AppendLine("                                              WHEN NULL THEN 'UNKNOWN' ");
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
            sb.AppendLine(",          S.STATIONID AS \"Station\" ");
            sb.AppendLine("      FROM TESTRECORD T ");
            sb.AppendLine(" LEFT OUTER JOIN STATION S ON S.STATIONID = T.STATIONID ");
            sb.AppendLine(" LEFT OUTER JOIN OBDSTUDYTABLE O ON T.VEHICLESEQUENCENUM = O.VSN AND T.TESTDATE = O.TESTDATE AND T.TESTENDTIME = O.TESTTIME ");
            sb.AppendLine(" LEFT OUTER JOIN PMV_OBDPROTOCOL_TRUTHTABLE@NH_ADHOC_PRIMARY PV ON PV.VIN10 = (SUBSTR(T.VIN,1,8) || SUBSTR(T.VIN,10,2)) ");
            sb.AppendLine(" LEFT OUTER JOIN PMV_OBDREADINESS_TRUTHTABLE@NH_ADHOC_PRIMARY PR ON PR.VIN10 = (SUBSTR(T.VIN,1,8) || SUBSTR(T.VIN,10,2)) ");
            sb.AppendLine("     WHERE T.VEHICLESEQUENCENUM IN (SELECT V.VEHICLESEQUENCENUMBER ");
            sb.AppendLine("                                      FROM VEHICLEMASTERTBL V ");
            sb.AppendLine("                                     WHERE V.VIN = '" + tbVIN.Text + "') ");
            sb.AppendLine("  ORDER BY T.TESTDATE DESC, T.TESTENDTIME DESC ");

            LogMessage("BuildQuery SQL:" + sb.ToString(), LogSeverity.Information);
            return sb.ToString();
        }

        private string BuildLastTestQuery(DataRow row)
        {
            LogMessage("BuildLastTestQuery Start", LogSeverity.Information);
            string lastTestDateTime = Master.UserReport.Rows[0].Cells[2].ToString();
            string strDetailsTestDate = GDCoreUtilities.StringUtilities.GetSubstring(row[1].ToString(), 0, 8);
            string strDetailsTestEndTime = GDCoreUtilities.StringUtilities.GetSubstring(row[1].ToString(), 8, 6);

            StringBuilder sb = new StringBuilder();
            sb = Classes.Reference.InqInspectionBy.InqInspectionBySQLbyOLTP(sb);

            sb.AppendLine("    WHERE T.VIN = '" + tbVIN.Text + "' ");
            sb.AppendLine("      AND T.TESTDATE = '" + strDetailsTestDate + "' ");
            sb.AppendLine("      AND T.TESTENDTIME = '" + strDetailsTestEndTime + "' ");
            sb.AppendLine(" ORDER BY T.TESTDATE DESC, T.TESTENDTIME DESC");

            LogMessage("BuildLastTestQuery SQL:" + sb.ToString(), LogSeverity.Information);
            return sb.ToString();
        }

        private void RunBuildLastTestQuery(DataRow row)
        {
            string qry = BuildLastTestQuery(row);
            GDDatabaseClient.Oracle.OracleResponse response = PortalFramework.Database.ODAP.GetDataRow(qry, PortalFramework.Database.DatabaseTarget.OLTP);
            if (response.Successful)
            {
                LogMessage("Run BuildLastTestQuery - Response Successful", LogSeverity.Information);
                if (response.HasResults)
                {
                    DataRow rowLastTest = response.ResultsRow;
                    LogMessage("Run BuildLastTestQuery - Response HasResults", LogSeverity.Information);
                    AddLastRowDetailsTable(rowLastTest);
                }
            }
        }

        private void RunDetailsReport()
        {
            string qry = BuildDetailsQuery();
            GDDatabaseClient.Oracle.OracleResponse response = PortalFramework.Database.ODAP.GetDataRow(qry, PortalFramework.Database.DatabaseTarget.OLTP);
            if (response.Successful)
            {
                LogMessage("Run DetailsReport - Response Successful", LogSeverity.Information);
                if (response.HasResults)
                {
                    DataRow row = response.ResultsRow;
                    DataRow rawRow = response.ResultsRow;
                    ltrlInqInspectionByVIN.Text = Classes.Reference.InqInspectionBy.GenerateDetailsTable(row);
                    if ((rawRow["Safety Result"].ToString()[0] == 'C') || (rawRow["Safety Result"].ToString()[0] == 'R'))
                    {
                        string qrySafety = BuildSafetyBreakdownQuery();
                        GDDatabaseClient.Oracle.OracleResponse responseSafety = PortalFramework.Database.ODAP.GetDataRow(qrySafety, PortalFramework.Database.DatabaseTarget.Adhoc);
                        if ((responseSafety.Successful) && (responseSafety.HasResults))
                        {
                            DataRow rowSafety = responseSafety.ResultsRow;
                            ltrlInqInspectionByVIN.Text += Classes.Reference.InqInspectionBy.GenerateSafetyBreakdownTable(rowSafety);
                        }
                    }
                    hidShowOverlay.Value = "TRUE";
                }
            }
            else
            {
                LogMessage("Run BuildDetailsReport - Response UnSuccessful", LogSeverity.Warning);
                Master.UserReport = null;
                Master.SetError(response.Exception);
            }

            LogOracleResponse(response);
            Master.RenderReportToPage();
            SessionHelper.SetCurrentReport(this.Session, Master.UserReport);
        }

        private string BuildDetailsQuery()
        {
            int idx = GDCoreUtilities.NullSafe.ToInt(hidSelectedIndex.Value);
            var row = Master.UserReport.Rows[idx];

            string strDetailsTestDate;
            string strDetailsTestEndTime;

            NHPortal.Classes.NHPortalUtilities.TryConvertDateTimeFormat(row[2].Value, "M/d/yyyy hh:mm:ss tt", "yyyyMMdd", out strDetailsTestDate);
            NHPortal.Classes.NHPortalUtilities.TryConvertDateTimeFormat(row[2].Value, "M/d/yyyy hh:mm:ss tt", "HHmmss", out strDetailsTestEndTime);

            StringBuilder builder = new StringBuilder();

            builder = Classes.Reference.InqInspectionBy.InqInspectionBySQLbyOLTP(builder);
            builder.AppendLine("    WHERE T.VIN = '" + tbVIN.Text + "' ");
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

            NHPortal.Classes.NHPortalUtilities.TryConvertDateTimeFormat(row[2].Value, "M/d/yyyy hh:mm:ss tt", "yyyyMMdd", out strDetailsTestDate);
            NHPortal.Classes.NHPortalUtilities.TryConvertDateTimeFormat(row[2].Value, "M/d/yyyy hh:mm:ss tt", "HHmmss", out strDetailsTestEndTime);

            StringBuilder builder = new StringBuilder();
            builder = Classes.Reference.InqInspectionBy.InqInspectionSafetyBreakdownAdhoc(builder);
            builder.AppendLine("    WHERE T.VIN = '" + tbVIN.Text + "' ");
            builder.AppendLine("      AND T.TESTDATE = '" + strDetailsTestDate + "' ");
            builder.AppendLine("      AND T.TESTENDTIME = '" + strDetailsTestEndTime + "' ");
            builder.AppendLine(" ORDER BY T.TESTDATE DESC, T.TESTENDTIME DESC");

            LogMessage("BuildSafetyBreakdownQuery SQL:" + builder.ToString(), LogSeverity.Information);
            return builder.ToString();
        }

        private void AddLastRowDetailsTable(DataRow row)
        {
            VehicleRegistrationInformationTable(row);
            LastInspectionInformationTable(row);
        }

        private void VehicleRegistrationInformationTable(DataRow row)
        {
            ReportTable tbl = new ReportTable();
            tbl.Title = "Vehicle and Inspection Information";
            tbl.HeaderVisible = false;
            tbl.HeaderNote = "Vehicle and Inspection Information";
            tbl.RenderLocation = RenderLocation.Above;
            tbl.Columns.Add("DataTitleColumn1", "", ColumnDataType.String);
            tbl.Columns.Add("DataValueColumn1", "", ColumnDataType.String);
            tbl.Columns.Add("DataTitleColumn2", "", ColumnDataType.String);
            tbl.Columns.Add("DataValueColumn2", "", ColumnDataType.String);

            ReportRow vriRow1 = new ReportRow(tbl);
            vriRow1.Cells.Add("VIN");
            vriRow1.Cells.Add(row["VIN"].ToString());
            vriRow1.Cells.Add("Registration Number");
            vriRow1.Cells.Add(row["VMT Registration Number"].ToString());
            tbl.Rows.Add(vriRow1);

            row["Fuel Code"] = Classes.Reference.FuelCodeTypes.GetDescription(row["Fuel Code"].ToString()[0]);
            ReportRow vr1Row2 = new ReportRow(tbl);
            vr1Row2.Cells.Add("Model Year");
            vr1Row2.Cells.Add(row["Model Year"].ToString());
            vr1Row2.Cells.Add("Fuel Code");
            vr1Row2.Cells.Add(row["Fuel Code"].ToString());
            tbl.Rows.Add(vr1Row2);

            ReportRow vr1Row3 = new ReportRow(tbl);
            vr1Row3.Cells.Add("Make");
            vr1Row3.Cells.Add(row["Make"].ToString());
            vr1Row3.Cells.Add("Body Style");
            vr1Row3.Cells.Add(row["Body Style"].ToString());
            tbl.Rows.Add(vr1Row3);

            ReportRow vr1Row4 = new ReportRow(tbl);
            vr1Row4.Cells.Add("Model");
            vr1Row4.Cells.Add(row["Model"].ToString());
            vr1Row4.Cells.Add("License");
            vr1Row4.Cells.Add(row["License"].ToString());
            tbl.Rows.Add(vr1Row4);

            ReportRow vr1Row5 = new ReportRow(tbl);
            vr1Row5.Cells.Add("Transmission");
            vr1Row5.Cells.Add(row["Transmission"].ToString());
            vr1Row5.Cells.Add("Plate Type");
            vr1Row5.Cells.Add(row["Plate Type"].ToString());
            tbl.Rows.Add(vr1Row5);

            row["Engine Size"] = GDCoreUtilities.NullSafe.ToDecimal(row["Engine Size"]).ToString("F1", CultureInfo.InvariantCulture);
            ReportRow vr1Row6 = new ReportRow(tbl);
            vr1Row6.Cells.Add("Number of Cylinders");
            vr1Row6.Cells.Add(row["Number Cylinders"].ToString());
            vr1Row6.Cells.Add("Engine Size (ltr)");
            vr1Row6.Cells.Add(row["Engine Size"].ToString());
            tbl.Rows.Add(vr1Row6);

            string strDateTimeFormattedOut;
            NHPortal.Classes.NHPortalUtilities.TryConvertDateTimeFormat(row["VMT Registration Expiration"].ToString(), "yyyyMMdd", "M/d/yyyy", out strDateTimeFormattedOut);
            row["VMT Registration Expiration"] = strDateTimeFormattedOut.ToString();
            NHPortal.Classes.NHPortalUtilities.TryConvertDateTimeFormat(row["VMT Registration Date"].ToString(), "yyyyMMdd", "M/d/yyyy", out strDateTimeFormattedOut);
            row["VMT Registration Date"] = strDateTimeFormattedOut.ToString();

            ReportRow vr1Row7 = new ReportRow(tbl);
            vr1Row7.Cells.Add("Registration Date");
            vr1Row7.Cells.Add(row["VMT Registration Date"].ToString());
            vr1Row7.Cells.Add("Registration Expiration");
            vr1Row7.Cells.Add(row["VMT Registration Expiration"].ToString());
            tbl.Rows.Add(vr1Row7);

            row["Tested As Vehicle Type"] = Classes.Reference.SafetyTypes.GetDescription(row["Tested As Vehicle Type"].ToString()[0]);
            ReportRow vr1Row8 = new ReportRow(tbl);
            vr1Row8.Cells.Add("Odometer");
            vr1Row8.Cells.Add(row["Tested As Odometer"].ToString());
            vr1Row8.Cells.Add("Vehicle Type");
            vr1Row8.Cells.Add(row["Tested As Vehicle Type"].ToString());
            tbl.Rows.Add(vr1Row8);

            if (row["Tested As GVW"].ToString() == "H") row["Tested As GVW"] = "GREATER THAN 10000 LBS";
            if (row["Tested As GVW"].ToString() == "M") row["Tested As GVW"] = "BETWEEN 8501 AND 10000 LBS";
            if (row["Tested As GVW"].ToString() == "L") row["Tested As GVW"] = "LESS THAN 8501 LBS";
            ReportRow vr1Row9 = new ReportRow(tbl);
            vr1Row9.Cells.Add("GVW");
            vr1Row9.Cells.Add(row["Tested As GVW"].ToString());
            vr1Row9.Cells.Add("");
            vr1Row9.Cells.Add("");
            tbl.Rows.Add(vr1Row9);

            Master.UserReport.Tables.Add(tbl);
        }

        private void LastInspectionInformationTable(DataRow row)
        {
            ReportTable tbl = new ReportTable();
            tbl.Title = "Last Inspection Information";
            tbl.HeaderVisible = false;
            tbl.HeaderNote = "Last Inspection Information";
            tbl.RenderLocation = RenderLocation.Above;
            tbl.Columns.Add("DataTitleColumn1", "", ColumnDataType.String);
            tbl.Columns.Add("DataValueColumn1", "", ColumnDataType.String);
            tbl.Columns.Add("DataTitleColumn2", "", ColumnDataType.String);
            tbl.Columns.Add("DataValueColumn2", "", ColumnDataType.String);

            string strDateTimeFormattedOut;
            NHPortal.Classes.NHPortalUtilities.TryConvertDateTimeFormat(row["Init Test Date"].ToString(), "yyyyMMdd", "M/d/yyyy", out strDateTimeFormattedOut);
            row["Init Test Date"] = strDateTimeFormattedOut.ToString();
            NHPortal.Classes.NHPortalUtilities.TryConvertDateTimeFormat(row["Init Test Time"].ToString(), "HHmmss", "hh:mm:ss tt", out strDateTimeFormattedOut);
            row["Init Test Time"] = strDateTimeFormattedOut.ToString();
            ReportRow vriRow1 = new ReportRow(tbl);
            vriRow1.Cells.Add("Initial Test Date");
            vriRow1.Cells.Add(row["Init Test Date"].ToString());
            vriRow1.Cells.Add("Initial Test Time");
            vriRow1.Cells.Add(row["Init Test Time"].ToString());
            tbl.Rows.Add(vriRow1);

            NHPortal.Classes.NHPortalUtilities.TryConvertDateTimeFormat(row["Test Date"].ToString(), "yyyyMMdd", "M/d/yyyy", out strDateTimeFormattedOut);
            row["Test Date"] = strDateTimeFormattedOut.ToString();
            NHPortal.Classes.NHPortalUtilities.TryConvertDateTimeFormat(row["Test Time"].ToString(), "HHmmss", "hh:mm:ss tt", out strDateTimeFormattedOut);
            row["Test Time"] = strDateTimeFormattedOut.ToString();
            ReportRow vr1Row2 = new ReportRow(tbl);
            vr1Row2.Cells.Add("Test Date");
            vr1Row2.Cells.Add(row["Test Date"].ToString());
            vr1Row2.Cells.Add("Test Time");
            vr1Row2.Cells.Add(row["Test Time"].ToString());
            tbl.Rows.Add(vr1Row2);

            row["Test Type"] = Classes.Reference.EmissionTypes.GetDescription(row["Test Type"].ToString()[0]);
            ReportRow vr1Row3 = new ReportRow(tbl);
            vr1Row3.Cells.Add("Test Type");
            vr1Row3.Cells.Add(row["Test Type"].ToString());
            vr1Row3.Cells.Add("Test Sequence");
            vr1Row3.Cells.Add(row["Test Sequence"].ToString());
            tbl.Rows.Add(vr1Row3);

            row["Visual Result"] = Classes.Reference.InqInspectionBy.GetOverallResultDescription(row["Visual Result"].ToString()[0]);
            row["OBD Result"] = Classes.Reference.InqInspectionBy.GetOverallResultDescription(row["OBD Result"].ToString()[0]);
            ReportRow vr1Row4 = new ReportRow(tbl);
            vr1Row4.Cells.Add("Visual Result");
            vr1Row4.Cells.Add(row["Visual Result"].ToString());
            vr1Row4.Cells.Add("OBD Result");
            vr1Row4.Cells.Add(row["OBD Result"].ToString());
            tbl.Rows.Add(vr1Row4);

            row["Overall Result"] = Classes.Reference.InqInspectionBy.GetOverallResultDescription(row["Overall Result"].ToString()[0]);
            row["Safety Result"] = Classes.Reference.InqInspectionBy.GetOverallResultDescription(row["Safety Result"].ToString()[0]);
            ReportRow vr1Row5 = new ReportRow(tbl);
            vr1Row5.Cells.Add("Safety Result");
            vr1Row5.Cells.Add(row["Safety Result"].ToString());
            vr1Row5.Cells.Add("Overall Result");
            vr1Row5.Cells.Add(row["Overall Result"].ToString());
            tbl.Rows.Add(vr1Row5);

            ReportRow vr1Row6 = new ReportRow(tbl);
            vr1Row6.Cells.Add("Station ID");
            vr1Row6.Cells.Add(row["Station ID"].ToString());
            vr1Row6.Cells.Add("Inspector ID");
            vr1Row6.Cells.Add(row["Inspector ID"].ToString());
            tbl.Rows.Add(vr1Row6);

            ReportRow vr1Row7 = new ReportRow(tbl);
            vr1Row7.Cells.Add("Unit ID");
            vr1Row7.Cells.Add(row["Unit ID"].ToString());
            vr1Row7.Cells.Add("Sticker Number");
            vr1Row7.Cells.Add(row["Sticker Number"].ToString());
            tbl.Rows.Add(vr1Row7);

            Master.UserReport.Tables.Add(tbl);
        }

        private void FormatReportValues()
        {
            string strDateTimeFormatOut;
            foreach (ReportRow row in Master.UserReport.Rows)
            {
                LogMessage("FormatReportValues - Start", LogSeverity.Information);
                NHPortal.Classes.NHPortalUtilities.TryConvertDateTimeFormat(row["Test Date"].Value, "yyyyMMddHHmmss", "M/d/yyyy hh:mm:ss tt", out strDateTimeFormatOut);
                row["Test Date"].Value = strDateTimeFormatOut;
                row["Safety Result"].Value = InqInspectionBy.GetOverallResultDescription(row["Safety Result"].Value[0]);
                row["Emis Result"].Value = InqInspectionBy.GetOverallResultDescription(row["Emis Result"].Value[0]);
                
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

                if (row["Protocol"].Value == "MATCH READINESS") row["Protocol"].Value = row["Readiness"].Value;
                if ((row["Protocol"].Value == "NO MATCH") && (row["Readiness"].Value == "UNKNOWN")) row["Protocol"].Value = row["Readiness"].Value;

                row["Overall Result"].Value = InqInspectionBy.GetOverallResultDescription(row["Overall Result"].Value[0]);                
                row["Test Type"].Value = Classes.Reference.EmissionTypes.GetDescription(row["Test Type"].Value[0]);
            }
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
            Master.SaveFavorite(NHPortal.UserControls.RedirectCodes.REPORT_INQ_INSPECTION_BY_VIN, UserFavoriteTypes.Report);
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
                        case "VIN":
                            tbVIN.Text = c.Value;
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