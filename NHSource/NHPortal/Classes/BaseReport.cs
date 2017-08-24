using GDCoreUtilities.Logging;
using GDDatabaseClient.Oracle;
using NHPortal.Classes.Charts;
using NHPortal.Classes.Reports.InspectionReports;
using NHPortal.Classes.User;
using NHPortal.UserControls;
using Oracle.DataAccess.Client;
using PortalFramework.Database;
using PortalFramework.ReportModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web;

namespace NHPortal.Classes
{
    /// <summary>Contains static methods for handling reports and stores a list of static report data.</summary>
    public static class BaseReportMaster
    {
        static BaseReportMaster()
        {
            reports = new List<BaseReport>();

            MainReport = new BaseReport(true, RedirectCodes.REPORT_MAIN, "Main Report", UserPermissions.MainReport);
            MainReport.FooterNote = "* Percentages apply to columns." + Environment.NewLine
                                  + "** Total Tests includes Completed tests, Aborted tests and Administrative Certificates." + Environment.NewLine
                                  + "*** Aborts and Administrative Certificates are not included in Inspection Results." + Environment.NewLine
                                  + "Please reference the document entitled Notes for Interpreting Report Data for further information.";
            MainReport.ReportColumnData = new Dictionary<string, ReportColumnInfo>();
            MainReport.ReportColumnData.Add("desc", new ReportColumnInfo("Description"));
            MainReport.ReportColumnData.Add("total_tests", new ReportColumnInfo("Total"));
            MainReport.ReportColumnData.Add("total_tests_percent", new ReportColumnInfo("%*"));
            MainReport.ReportColumnData.Add("init_tests", new ReportColumnInfo("Initial Tests"));
            MainReport.ReportColumnData.Add("init_tests_percent", new ReportColumnInfo("IT %*"));
            MainReport.ReportColumnData.Add("init_retests", new ReportColumnInfo("IT %*"));
            MainReport.ReportColumnData.Add("init_retests_percent", new ReportColumnInfo("IR %*"));
            MainReport.ReportColumnData.Add("other_retests", new ReportColumnInfo("Other Retests"));
            MainReport.ReportColumnData.Add("other_retests_percent", new ReportColumnInfo("OR %*"));
            reports.Add(MainReport);

            ModelYearRejection = new BaseReport(true, RedirectCodes.REPORT_MODEL_YR_REJECTION, "Model Year Rejection Report", UserPermissions.ModelYearRejection);
            ModelYearRejection.FooterNote = "* Test Count includes Completed tests, Aborted tests and Administrative Certificates." + Environment.NewLine + "** Deficiency = Rejected + Corrected. ";
            ModelYearRejection.ReportColumnData = new Dictionary<string, ReportColumnInfo>();
            ModelYearRejection.ReportColumnData.Add("MODEL_YEAR", new ReportColumnInfo("Model Year"));
            ModelYearRejection.ReportColumnData.Add("TEST_COUNT", new ReportColumnInfo("Test*  Count", ColumnDataType.Number));
            ModelYearRejection.ReportColumnData.Add("DEFICIENCY_CNT", new ReportColumnInfo("Deficiency**  Count", ColumnDataType.Number));
            ModelYearRejection.ReportColumnData.Add("DEFICIENCY_PCT", new ReportColumnInfo("%  Deficiency", ColumnDataType.Percentage));
            ModelYearRejection.ReportColumnData.Add("REJECT_CNT", new ReportColumnInfo("Reject Count", ColumnDataType.Number));
            ModelYearRejection.ReportColumnData.Add("REJECT_PCT", new ReportColumnInfo("% Reject", ColumnDataType.Percentage));
            ModelYearRejection.ReportColumnData.Add("SAFETY_COUNT", new ReportColumnInfo("Safety Count", ColumnDataType.Number));
            ModelYearRejection.ReportColumnData.Add("SAFETY_REJ", new ReportColumnInfo("Safety Reject", ColumnDataType.Number));
            ModelYearRejection.ReportColumnData.Add("SAFETY_REJ_PCT", new ReportColumnInfo("Safety % Reject", ColumnDataType.Percentage));
            ModelYearRejection.ReportColumnData.Add("SAFETY_CORRECTED", new ReportColumnInfo("Safety Corrected", ColumnDataType.Number));
            ModelYearRejection.ReportColumnData.Add("SAFETY_CORRECTED_PCT", new ReportColumnInfo("Safety % Corrected", ColumnDataType.Percentage));
            ModelYearRejection.ReportColumnData.Add("SAFETY_DEFICIENCY", new ReportColumnInfo("Safety Deficiency**", ColumnDataType.Number));
            ModelYearRejection.ReportColumnData.Add("SAFETY_DEFICIENCY_PCT", new ReportColumnInfo("Safety % Deficiency**", ColumnDataType.Percentage));
            ModelYearRejection.ReportColumnData.Add("OBD_COUNT", new ReportColumnInfo("OBD Count", ColumnDataType.Number));
            ModelYearRejection.ReportColumnData.Add("OBD_REJ", new ReportColumnInfo("OBD Reject", ColumnDataType.Number));
            ModelYearRejection.ReportColumnData.Add("OBD_REJ_PCT", new ReportColumnInfo("OBD % Reject", ColumnDataType.Percentage));
            ModelYearRejection.ReportColumnData.Add("OBD_NOCOM", new ReportColumnInfo("OBD NoCom", ColumnDataType.Number));
            ModelYearRejection.ReportColumnData.Add("OBD_NOCOM_PCT", new ReportColumnInfo("OBD % NoCom", ColumnDataType.Percentage));
            ModelYearRejection.ReportColumnData.Add("VISUAL_COUNT", new ReportColumnInfo("Visual Count", ColumnDataType.Number));
            ModelYearRejection.ReportColumnData.Add("VISUAL_REJ", new ReportColumnInfo("Visual Reject", ColumnDataType.Number));
            ModelYearRejection.ReportColumnData.Add("VISUAL_REJ_PCT", new ReportColumnInfo("Visual % Reject", ColumnDataType.Percentage));
            ModelYearRejection.ReportColumnData.Add("VISUAL_CORR", new ReportColumnInfo("Visual Corrected", ColumnDataType.Number));
            ModelYearRejection.ReportColumnData.Add("VISUAL_CORR_PCT", new ReportColumnInfo("Visual % Corrected", ColumnDataType.Percentage));
            ModelYearRejection.ReportColumnData.Add("VISUAL_DEFICIENCY", new ReportColumnInfo("Visual Deficiency**", ColumnDataType.Number));
            ModelYearRejection.ReportColumnData.Add("VISUAL_DEFICIENCY_PCT", new ReportColumnInfo("Visual % Deficiency**", ColumnDataType.Percentage));
            ModelYearRejection.ReportColumnData.Add("ABORT_CNT", new ReportColumnInfo("Abort Count", ColumnDataType.Number));
            ModelYearRejection.ReportColumnData.Add("ABORT_PCT", new ReportColumnInfo(" % Abort"));
            ModelYearRejection.ReportColumnData.Add("ADMIN_CERT_CNT", new ReportColumnInfo("Admin Cert Count", ColumnDataType.Number));
            ModelYearRejection.ReportColumnData.Add("ADMIN_CERT_PCT", new ReportColumnInfo(" % Admin Cert"));
            reports.Add(ModelYearRejection);

            StationRejection = new BaseReport(true, RedirectCodes.REPORT_STATION_REJECTION, "Rejection Rates By Station ", UserPermissions.StationRejection);
            StationRejection.FooterNote = "* Aborts are not included in Total Test Count." + Environment.NewLine + "** OBD Advisory Rejections not included in Total Rejections. ";
            StationRejection.ReportColumnData = new Dictionary<string, ReportColumnInfo>();
            StationRejection.ReportColumnData.Add("STATION_ID", new ReportColumnInfo("Station ID"));
            StationRejection.ReportColumnData.Add("TEST_COUNT", new ReportColumnInfo("Test Count*", ColumnDataType.Number));
            StationRejection.ReportColumnData.Add("REJECT_CNT", new ReportColumnInfo("Reject Count", ColumnDataType.Number));
            StationRejection.ReportColumnData.Add("REJECT_PCT", new ReportColumnInfo("% Reject", ColumnDataType.Percentage));
            StationRejection.ReportColumnData.Add("SAFETY_COUNT", new ReportColumnInfo("Safety Count", ColumnDataType.Number));
            StationRejection.ReportColumnData.Add("SAFETY_REJ", new ReportColumnInfo("Safety Reject", ColumnDataType.Number));
            StationRejection.ReportColumnData.Add("SAFETY_REJ_PCT", new ReportColumnInfo("Safety % Reject", ColumnDataType.Percentage));
            StationRejection.ReportColumnData.Add("OBD_COUNT", new ReportColumnInfo("OBD Count", ColumnDataType.Number));
            StationRejection.ReportColumnData.Add("OBD_REJ", new ReportColumnInfo("OBD Reject", ColumnDataType.Number));
            StationRejection.ReportColumnData.Add("OBD_REJ_PCT", new ReportColumnInfo("OBD % Reject", ColumnDataType.Percentage));
            StationRejection.ReportColumnData.Add("OBD_NOCOM", new ReportColumnInfo("OBD NoCom", ColumnDataType.Number));
            StationRejection.ReportColumnData.Add("OBD_NOCOM_PCT", new ReportColumnInfo("OBD %  NoCom", ColumnDataType.Percentage));
            StationRejection.ReportColumnData.Add("VISUAL_COUNT", new ReportColumnInfo("Visual Count", ColumnDataType.Number));
            StationRejection.ReportColumnData.Add("VISUAL_REJ", new ReportColumnInfo("Visual Reject", ColumnDataType.Number));
            StationRejection.ReportColumnData.Add("VISUAL_REJ_PCT", new ReportColumnInfo("Visual % Reject", ColumnDataType.Percentage));
            StationRejection.ReportColumnData.Add("ABORT_CNT", new ReportColumnInfo("Abort Count", ColumnDataType.Number));
            StationRejection.ReportColumnData.Add("ABORT_PCT", new ReportColumnInfo(" % Abort", ColumnDataType.Percentage));
            reports.Add(StationRejection);

            CountyStationRejection = new BaseReport(true, RedirectCodes.REPORT_CNTY_ST_REJECTION, "Rejection Rate By County And Station", UserPermissions.CountyStationRejection);
            CountyStationRejection.FooterNote = "* Aborts are not included in Total Test Count." + Environment.NewLine + "** OBD Advisory Rejections not included in Total Rejections. ";
            CountyStationRejection.ReportColumnData = new Dictionary<string, ReportColumnInfo>();
            CountyStationRejection.ReportColumnData.Add("COUNTY", new ReportColumnInfo("County"));
            CountyStationRejection.ReportColumnData.Add("STATION_ID", new ReportColumnInfo("Station ID"));
            CountyStationRejection.ReportColumnData.Add("TEST_COUNT", new ReportColumnInfo("Test * Count", ColumnDataType.Number));
            CountyStationRejection.ReportColumnData.Add("REJECT_CNT", new ReportColumnInfo("Reject Count", ColumnDataType.Number));
            CountyStationRejection.ReportColumnData.Add("REJECT_PCT", new ReportColumnInfo("% Reject", ColumnDataType.Percentage));
            CountyStationRejection.ReportColumnData.Add("SAFETY_COUNT", new ReportColumnInfo("Safety Count", ColumnDataType.Number));
            CountyStationRejection.ReportColumnData.Add("SAFETY_REJ", new ReportColumnInfo("Safety Reject", ColumnDataType.Number));
            CountyStationRejection.ReportColumnData.Add("SAFETY_REJ_PCT", new ReportColumnInfo("Safety % Reject", ColumnDataType.Percentage));
            CountyStationRejection.ReportColumnData.Add("OBD_COUNT", new ReportColumnInfo("OBD Count", ColumnDataType.Number));
            CountyStationRejection.ReportColumnData.Add("OBD_REJ", new ReportColumnInfo("OBD Reject", ColumnDataType.Number));
            CountyStationRejection.ReportColumnData.Add("OBD_REJ_PCT", new ReportColumnInfo("OBD % Reject", ColumnDataType.Percentage));
            CountyStationRejection.ReportColumnData.Add("OBD_NOCOM", new ReportColumnInfo("OBD NoCom", ColumnDataType.Number));
            CountyStationRejection.ReportColumnData.Add("OBD_NOCOM_PCT", new ReportColumnInfo("OBD % NoCom", ColumnDataType.Percentage));
            CountyStationRejection.ReportColumnData.Add("VISUAL_COUNT", new ReportColumnInfo("Visual Count", ColumnDataType.Number));
            CountyStationRejection.ReportColumnData.Add("VISUAL_REJ", new ReportColumnInfo("Visual Reject", ColumnDataType.Number));
            CountyStationRejection.ReportColumnData.Add("VISUAL_REJ_PCT", new ReportColumnInfo("Visual % Reject", ColumnDataType.Percentage));
            CountyStationRejection.ReportColumnData.Add("ABORT_CNT", new ReportColumnInfo("Abort Count", ColumnDataType.Number));
            CountyStationRejection.ReportColumnData.Add("ABORT_PCT", new ReportColumnInfo(" % Abort", ColumnDataType.Percentage));
            reports.Add(CountyStationRejection);

            MechanicRejection = new BaseReport(true, RedirectCodes.REPORT_MECHANIC_REJECTION, "Rejection Rates By Mechanic ", UserPermissions.MechanicRejection);
            MechanicRejection.FooterNote = "* Aborts are not included in Total Test Count." + Environment.NewLine + "** OBD Advisory Rejections not included in Total Rejections. ";
            MechanicRejection.ReportColumnData = new Dictionary<string, ReportColumnInfo>();
            MechanicRejection.ReportColumnData.Add("MECHANIC_ID", new ReportColumnInfo("Mechanic ID"));
            MechanicRejection.ReportColumnData.Add("TEST_COUNT", new ReportColumnInfo("Test * Count", ColumnDataType.Number));
            MechanicRejection.ReportColumnData.Add("REJECT_CNT", new ReportColumnInfo("Reject Count", ColumnDataType.Number));
            MechanicRejection.ReportColumnData.Add("REJECT_PCT", new ReportColumnInfo("% Reject", ColumnDataType.Percentage));
            MechanicRejection.ReportColumnData.Add("SAFETY_COUNT", new ReportColumnInfo("Safety Count", ColumnDataType.Number));
            MechanicRejection.ReportColumnData.Add("SAFETY_REJ", new ReportColumnInfo("Safety Reject", ColumnDataType.Number));
            MechanicRejection.ReportColumnData.Add("SAFETY_REJ_PCT", new ReportColumnInfo("Safety % Reject", ColumnDataType.Percentage));
            MechanicRejection.ReportColumnData.Add("OBD_COUNT", new ReportColumnInfo("OBD Count", ColumnDataType.Number));
            MechanicRejection.ReportColumnData.Add("OBD_REJ", new ReportColumnInfo("OBD Reject", ColumnDataType.Number));
            MechanicRejection.ReportColumnData.Add("OBD_REJ_PCT", new ReportColumnInfo("OBD % Reject", ColumnDataType.Percentage));
            MechanicRejection.ReportColumnData.Add("OBD_NOCOM", new ReportColumnInfo("OBD NoCom", ColumnDataType.Number));
            MechanicRejection.ReportColumnData.Add("OBD_NOCOM_PCT", new ReportColumnInfo("OBD % NoCom", ColumnDataType.Percentage));
            MechanicRejection.ReportColumnData.Add("VISUAL_COUNT", new ReportColumnInfo("Visual Count", ColumnDataType.Number));
            MechanicRejection.ReportColumnData.Add("VISUAL_REJ", new ReportColumnInfo("Visual Reject", ColumnDataType.Number));
            MechanicRejection.ReportColumnData.Add("VISUAL_REJ_PCT", new ReportColumnInfo("Visual % Reject", ColumnDataType.Percentage));
            MechanicRejection.ReportColumnData.Add("ABORT_CNT", new ReportColumnInfo("Abort Count", ColumnDataType.Number));
            MechanicRejection.ReportColumnData.Add("ABORT_PCT", new ReportColumnInfo(" %  Abort", ColumnDataType.Percentage));
            reports.Add(MechanicRejection);

            OBDResults = new BaseReport(true, RedirectCodes.REPORT_OBD_RESULTS, "OBD Report", UserPermissions.OBDResults);
            OBDResults.FooterNote = String.Empty;
            OBDResults.ReportColumnData = new Dictionary<string, ReportColumnInfo>();
            OBDResults.ReportColumnData.Add("STATION_ID", new ReportColumnInfo("Station ID"));
            OBDResults.ReportColumnData.Add("TOTAL", new ReportColumnInfo("Total", ColumnDataType.Number));
            OBDResults.ReportColumnData.Add("INIT_STEP", new ReportColumnInfo("Init Step", ColumnDataType.Number));
            OBDResults.ReportColumnData.Add("INIT_PCT", new ReportColumnInfo("% Init", ColumnDataType.Percentage));
            OBDResults.ReportColumnData.Add("VOLT_STEP", new ReportColumnInfo("Voltage Step", ColumnDataType.Number));
            OBDResults.ReportColumnData.Add("VOLT_PCT", new ReportColumnInfo("% Voltage", ColumnDataType.Percentage));
            OBDResults.ReportColumnData.Add("TEST_STEP", new ReportColumnInfo("Test Step", ColumnDataType.Number));
            OBDResults.ReportColumnData.Add("TEST_PCT", new ReportColumnInfo("% Test", ColumnDataType.Percentage));
            OBDResults.ReportColumnData.Add("EXTRA_INFO_STEP", new ReportColumnInfo("Extra Info Step", ColumnDataType.Number));
            OBDResults.ReportColumnData.Add("EXTRA_INFO_PCT", new ReportColumnInfo("% Extra Info", ColumnDataType.Percentage));
            OBDResults.ReportColumnData.Add("MIL_OK", new ReportColumnInfo("MIL OK", ColumnDataType.Number));
            OBDResults.ReportColumnData.Add("MIL_OK_PCT", new ReportColumnInfo("% MIL OK", ColumnDataType.Percentage));
            OBDResults.ReportColumnData.Add("NO_COM", new ReportColumnInfo("No Com", ColumnDataType.Number));
            OBDResults.ReportColumnData.Add("NO_COM_PCT", new ReportColumnInfo("% No Com", ColumnDataType.Percentage));
            OBDResults.ReportColumnData.Add("UNREADY", new ReportColumnInfo("Unready", ColumnDataType.Number));
            OBDResults.ReportColumnData.Add("UNREADY_PCT", new ReportColumnInfo("% Unready", ColumnDataType.Percentage));
            OBDResults.ReportColumnData.Add("MIL_ON", new ReportColumnInfo("MIL ON", ColumnDataType.Number));
            OBDResults.ReportColumnData.Add("MIL_ON_PCT", new ReportColumnInfo(" % MIL ON", ColumnDataType.Percentage));
            OBDResults.ReportColumnData.Add("READY_EXEMPT", new ReportColumnInfo("Ready Exempt", ColumnDataType.Number));
            OBDResults.ReportColumnData.Add("READY_EXCEMPT_PCT", new ReportColumnInfo("% Ready Exempt", ColumnDataType.Percentage));
            OBDResults.ReportColumnData.Add("COM_EXEMPT", new ReportColumnInfo("Com Exempt", ColumnDataType.Number));
            OBDResults.ReportColumnData.Add("COM_EXEMPT_PCT", new ReportColumnInfo("% Com Exempt", ColumnDataType.Percentage));
            reports.Add(OBDResults);

            FirstTestReset = new BaseReport(true, RedirectCodes.REPORT_FIRST_TEST_RESET, "First Test Reset", UserPermissions.FirstTestReset);
            FirstTestReset.FooterNote = String.Empty;
            FirstTestReset.SQLTable = "FIRSTTESTRESET";
            FirstTestReset.DatabaseTarget = PortalFramework.Database.DatabaseTarget.OLTP;
            FirstTestReset.ReportColumnData = new Dictionary<string, ReportColumnInfo>();
            FirstTestReset.ReportColumnData.Add("VIN", new ReportColumnInfo("VIN"));
            FirstTestReset.ReportColumnData.Add("DATE", new ReportColumnInfo("Date"));
            FirstTestReset.ReportColumnData.Add("TIME", new ReportColumnInfo("Time"));
            FirstTestReset.ReportColumnData.Add("USER", new ReportColumnInfo("User"));
            FirstTestReset.ReportColumnData.Add("REASON", new ReportColumnInfo("Reason"));
            reports.Add(FirstTestReset);

            EmissionTestRejectionRates = new BaseReport(false, RedirectCodes.GRAPH_EMISSION_TEST_REJECT_RATE, "Emission Test Rejection Rates", UserPermissions.EmissionTestRejectionRates);
            EmissionTestRejectionRates.FooterNote = String.Empty;
            EmissionTestRejectionRates.DatabaseTarget = PortalFramework.Database.DatabaseTarget.Adhoc;
            EmissionTestRejectionRates.ReportColumnData = new Dictionary<string, ReportColumnInfo>();
            EmissionTestRejectionRates.ReportColumnData.Add("TBL_OVERALL_COUNT", new ReportColumnInfo("Total Tests", ColumnDataType.Number));
            EmissionTestRejectionRates.ReportColumnData.Add("TBL_OVERALL_REJECT_COUNT", new ReportColumnInfo("Rejected Tests", ColumnDataType.Number));
            EmissionTestRejectionRates.ReportColumnData.Add("TBL_OVERALL_REJECT_PCT", new ReportColumnInfo("Rejected Tests %", ColumnDataType.Percentage));
            EmissionTestRejectionRates.ReportColumnData.Add("TBL_OBD_COUNT", new ReportColumnInfo("Total OBD Tests", ColumnDataType.Number));
            EmissionTestRejectionRates.ReportColumnData.Add("TBL_OBD_REJECT_COUNT", new ReportColumnInfo("Rejected OBD Tests", ColumnDataType.Number));
            EmissionTestRejectionRates.ReportColumnData.Add("TBL_OBD_REJECT_PCT", new ReportColumnInfo("Rejected OBD Tests %", ColumnDataType.Percentage));
            EmissionTestRejectionRates.ReportColumnData.Add("TBL_VIS_COUNT", new ReportColumnInfo("Total Visual Tests", ColumnDataType.Number));
            EmissionTestRejectionRates.ReportColumnData.Add("TBL_VIS_REJECT_COUNT", new ReportColumnInfo("Rejected Visual Tests", ColumnDataType.Number));
            EmissionTestRejectionRates.ReportColumnData.Add("TBL_VIS_REJECT_PCT", new ReportColumnInfo("Rejected Visual Tests %", ColumnDataType.Percentage));
            EmissionTestRejectionRates.ReportColumnData.Add("TBL_SAF_COUNT", new ReportColumnInfo("Total Safety Tests", ColumnDataType.Number));
            EmissionTestRejectionRates.ReportColumnData.Add("TBL_SAF_REJECT_COUNT", new ReportColumnInfo("Rejected Safety Tests", ColumnDataType.Number));
            EmissionTestRejectionRates.ReportColumnData.Add("TBL_SAF_REJECT_PCT", new ReportColumnInfo("Rejected Safety Tests %", ColumnDataType.Percentage));
            reports.Add(EmissionTestRejectionRates);

            OBDIIReadinessMonitors = new BaseReport(false, RedirectCodes.GRAPH_OBD_READINESS_MONITORS, "OBD II Readinesss Monitors", UserPermissions.OBDIIReadinessMonitor);
            OBDIIReadinessMonitors.FooterNote = String.Empty;
            OBDIIReadinessMonitors.DatabaseTarget = PortalFramework.Database.DatabaseTarget.Adhoc;
            OBDIIReadinessMonitors.ReportColumnData = new Dictionary<string, ReportColumnInfo>();
            OBDIIReadinessMonitors.ReportColumnData.Add("TBL_OVERALL_COUNT", new ReportColumnInfo("Total Tests", ColumnDataType.Number));
            OBDIIReadinessMonitors.ReportColumnData.Add("TBL_READY_COUNT", new ReportColumnInfo("Ready Status", ColumnDataType.Number));
            OBDIIReadinessMonitors.ReportColumnData.Add("TBL_READY_PCT", new ReportColumnInfo("Ready %", ColumnDataType.Percentage));
            OBDIIReadinessMonitors.ReportColumnData.Add("TBL_NOTREADY_COUNT", new ReportColumnInfo("Not Ready Status", ColumnDataType.Number));
            OBDIIReadinessMonitors.ReportColumnData.Add("TBL_NOTREADY_PCT", new ReportColumnInfo("Not Ready %", ColumnDataType.Percentage));
            reports.Add(OBDIIReadinessMonitors);

            OBDIIMILStatus = new BaseReport(false, RedirectCodes.GRAPH_OBD_MIL_STATUS, "OBD II MIL Status", UserPermissions.OBDIIMILStatus);
            OBDIIMILStatus.FooterNote = String.Empty;
            OBDIIMILStatus.DatabaseTarget = PortalFramework.Database.DatabaseTarget.Adhoc;
            OBDIIMILStatus.ReportColumnData = new Dictionary<string, ReportColumnInfo>();
            OBDIIMILStatus.ReportColumnData.Add("TBL_OVERALL_COUNT", new ReportColumnInfo("Total Tests", ColumnDataType.Number));
            OBDIIMILStatus.ReportColumnData.Add("TBL_MILON_COUNT", new ReportColumnInfo("MIL On", ColumnDataType.Number));
            OBDIIMILStatus.ReportColumnData.Add("TBL_MILON_PCT", new ReportColumnInfo("MIL On %", ColumnDataType.Percentage));
            OBDIIMILStatus.ReportColumnData.Add("TBL_MILOFF_COUNT", new ReportColumnInfo("MIL Off", ColumnDataType.Number));
            OBDIIMILStatus.ReportColumnData.Add("TBL_MILOFF_PCT", new ReportColumnInfo("MIL Off %", ColumnDataType.Percentage));
            OBDIIMILStatus.ReportColumnData.Add("TBL_MILNOTREADY_COUNT", new ReportColumnInfo("MIL Not Ready", ColumnDataType.Number));
            OBDIIMILStatus.ReportColumnData.Add("TBL_MILNOTREADY_PCT", new ReportColumnInfo("MIL Not Ready %", ColumnDataType.Percentage));
            reports.Add(OBDIIMILStatus);

            OBDIICommunications = new BaseReport(false, RedirectCodes.GRAPH_OBD_COMMUNICATIONS, "OBD II Communications", UserPermissions.OBDIICommunications);
            OBDIICommunications.FooterNote = String.Empty;
            OBDIICommunications.DatabaseTarget = PortalFramework.Database.DatabaseTarget.Adhoc;
            OBDIICommunications.ReportColumnData = new Dictionary<string, ReportColumnInfo>();
            OBDIICommunications.ReportColumnData.Add("TBL_OVERALL_COUNT", new ReportColumnInfo("Total Tests", ColumnDataType.Number));
            OBDIICommunications.ReportColumnData.Add("TBL_NOCOM_COUNT", new ReportColumnInfo("No Comm", ColumnDataType.Number));
            OBDIICommunications.ReportColumnData.Add("TBL_NOCOM_PCT", new ReportColumnInfo("No Comm %", ColumnDataType.Percentage));
            OBDIICommunications.ReportColumnData.Add("TBL_COM_COUNT", new ReportColumnInfo("Comm", ColumnDataType.Number));
            OBDIICommunications.ReportColumnData.Add("TBL_COM_PCT", new ReportColumnInfo("Comm %", ColumnDataType.Percentage));
            OBDIICommunications.ReportColumnData.Add("TBL_COMEXEMPT_COUNT", new ReportColumnInfo("Comm Exempt", ColumnDataType.Number));
            OBDIICommunications.ReportColumnData.Add("TBL_COMEXEMPT_PCT", new ReportColumnInfo("Comm Exempt %", ColumnDataType.Percentage));
            reports.Add(OBDIICommunications);

            OBDIIDTCErrorCodes = new BaseReport(true, RedirectCodes.GRAPH_OBD_DTC_ERR_CODES, "OBD II DTC Error Codes", UserPermissions.OBDIIDTCErrorCodes);
            OBDIIDTCErrorCodes.FooterNote = String.Empty;
            OBDIIDTCErrorCodes.DatabaseTarget = PortalFramework.Database.DatabaseTarget.Adhoc;
            OBDIIDTCErrorCodes.ReportColumnData = new Dictionary<string, ReportColumnInfo>();
            OBDIIDTCErrorCodes.ReportColumnData.Add("MODEL_YEAR", new ReportColumnInfo("Model Year"));
            OBDIIDTCErrorCodes.ReportColumnData.Add("MAKE", new ReportColumnInfo("Make"));
            OBDIIDTCErrorCodes.ReportColumnData.Add("MODEL", new ReportColumnInfo("Model"));
            OBDIIDTCErrorCodes.ReportColumnData.Add("DTC", new ReportColumnInfo("DTC"));
            OBDIIDTCErrorCodes.ReportColumnData.Add("QUANTITY", new ReportColumnInfo("Quantity", ColumnDataType.Number));
            reports.Add(OBDIIDTCErrorCodes);

            OBDIIProtocolUsage = new BaseReport(true, RedirectCodes.GRAPH_OBD_PROTOCOL_USAGE, "OBD II Protocol Usage", UserPermissions.OBDIIProtocolUsage);
            OBDIIProtocolUsage.FooterNote = String.Empty;
            OBDIIProtocolUsage.DatabaseTarget = PortalFramework.Database.DatabaseTarget.Adhoc;
            OBDIIProtocolUsage.ReportColumnData = new Dictionary<string, ReportColumnInfo>();
            OBDIIProtocolUsage.ReportColumnData.Add("MODEL_YEAR", new ReportColumnInfo("Model Year"));
            OBDIIProtocolUsage.ReportColumnData.Add("MAKE", new ReportColumnInfo("Make"));
            OBDIIProtocolUsage.ReportColumnData.Add("MODEL", new ReportColumnInfo("Model"));
            OBDIIProtocolUsage.ReportColumnData.Add("QUANTITY", new ReportColumnInfo("Quantity", ColumnDataType.Number));
            reports.Add(OBDIIProtocolUsage);

            EVINMismatch = new BaseReport(true, RedirectCodes.TRIGGER_EVIN_MISMATCH, "eVIN Mismatch Trigger", UserPermissions.eVINMismatch);
            EVINMismatch.FooterNote = String.Empty;
            EVINMismatch.DatabaseTarget = PortalFramework.Database.DatabaseTarget.Adhoc;
            EVINMismatch.ReportColumnData = new Dictionary<string, ReportColumnInfo>();
            EVINMismatch.ReportColumnData.Add("TEST_COUNT", new ReportColumnInfo("Test Count", ColumnDataType.Number));
            EVINMismatch.ReportColumnData.Add("MATCH_COUNT", new ReportColumnInfo("Match Count", ColumnDataType.Number));
            EVINMismatch.ReportColumnData.Add("MATCH_PCT", new ReportColumnInfo("Match %", ColumnDataType.Percentage));
            EVINMismatch.ReportColumnData.Add("NO_MATCH_COUNT", new ReportColumnInfo("No Match Count", ColumnDataType.Number));
            EVINMismatch.ReportColumnData.Add("NO_MATCH_PCT", new ReportColumnInfo("No Match %", ColumnDataType.Percentage));
            reports.Add(EVINMismatch);

            CommProtocol = new BaseReport(true, RedirectCodes.TRIGGER_COMM_PROTOCOL, "Communication Protocol Mismatch Trigger", UserPermissions.CommunicationProtocol);
            CommProtocol.FooterNote = TRIGGER_INVALID_EVINS;
            CommProtocol.DatabaseTarget = PortalFramework.Database.DatabaseTarget.Adhoc;
            CommProtocol.ReportColumnData = new Dictionary<string, ReportColumnInfo>();
            CommProtocol.ReportColumnData.Add("TEST_COUNT", new ReportColumnInfo("Test Count", ColumnDataType.Number));
            CommProtocol.ReportColumnData.Add("MATCH_COUNT", new ReportColumnInfo("Match Count", ColumnDataType.Number));
            CommProtocol.ReportColumnData.Add("MATCH_PCT", new ReportColumnInfo("Match %", ColumnDataType.Percentage));
            CommProtocol.ReportColumnData.Add("NO_MATCH_COUNT", new ReportColumnInfo("No Match Count", ColumnDataType.Number));
            CommProtocol.ReportColumnData.Add("NO_MATCH_PCT", new ReportColumnInfo("No Match %", ColumnDataType.Percentage));
            CommProtocol.ReportColumnData.Add("UNKNOWN_COUNT", new ReportColumnInfo("Unknown Count", ColumnDataType.Number));
            CommProtocol.ReportColumnData.Add("UNKNOWN_PCT", new ReportColumnInfo("Unknown %", ColumnDataType.Percentage));
            reports.Add(CommProtocol);

            Rejection = new BaseReport(true, RedirectCodes.TRIGGER_REJECTION, "Rejection Trigger", UserPermissions.Rejection);
            Rejection.FooterNote = TRIGGER_INVALID_EVINS;
            Rejection.DatabaseTarget = PortalFramework.Database.DatabaseTarget.Adhoc;
            Rejection.ReportColumnData = new Dictionary<string, ReportColumnInfo>();
            Rejection.ReportColumnData.Add("TEST_COUNT", new ReportColumnInfo("Test Count", ColumnDataType.Number));
            Rejection.ReportColumnData.Add("REJECTION_COUNT", new ReportColumnInfo("Rejection Count", ColumnDataType.Number));
            Rejection.ReportColumnData.Add("REJECTION_PCT", new ReportColumnInfo("Rejection %", ColumnDataType.Percentage));
            reports.Add(Rejection);

            ReadinessMismatch = new BaseReport(true, RedirectCodes.TRIGGER_READINESS_MISMATCH, "Readiness Mismatch Trigger", UserPermissions.ReadinessPatternMismatch);
            ReadinessMismatch.FooterNote = TRIGGER_INVALID_EVINS;
            ReadinessMismatch.DatabaseTarget = PortalFramework.Database.DatabaseTarget.Adhoc;
            ReadinessMismatch.ReportColumnData = new Dictionary<string, ReportColumnInfo>();
            ReadinessMismatch.ReportColumnData.Add("TEST_COUNT", new ReportColumnInfo("Test Count", ColumnDataType.Number));
            ReadinessMismatch.ReportColumnData.Add("MATCH_COUNT", new ReportColumnInfo("Match Count", ColumnDataType.Number));
            ReadinessMismatch.ReportColumnData.Add("MATCH_PCT", new ReportColumnInfo("Match %", ColumnDataType.Percentage));
            ReadinessMismatch.ReportColumnData.Add("NO_MATCH_COUNT", new ReportColumnInfo("No Match Count", ColumnDataType.Number));
            ReadinessMismatch.ReportColumnData.Add("NO_MATCH_PCT", new ReportColumnInfo("No Match %", ColumnDataType.Percentage));
            ReadinessMismatch.ReportColumnData.Add("UNKNOWN_COUNT", new ReportColumnInfo("Unknown Count", ColumnDataType.Number));
            ReadinessMismatch.ReportColumnData.Add("UNKNOWN_PCT", new ReportColumnInfo("Unknown %", ColumnDataType.Percentage));
            reports.Add(ReadinessMismatch);

            SafetyDefect = new BaseReport(true, RedirectCodes.TRIGGER_STATION_SAFETY_DEFECT, "Safety Defect Trigger", UserPermissions.StationSafetyDefect);
            SafetyDefect.FooterNote = TRIGGER_INVALID_EVINS;
            SafetyDefect.DatabaseTarget = PortalFramework.Database.DatabaseTarget.Adhoc;
            SafetyDefect.ReportColumnData = new Dictionary<string, ReportColumnInfo>();
            SafetyDefect.ReportColumnData.Add("TEST_COUNT", new ReportColumnInfo("Test Count", ColumnDataType.Number));
            SafetyDefect.ReportColumnData.Add("DEFECT_COUNT", new ReportColumnInfo("Defect Count", ColumnDataType.Number));
            SafetyDefect.ReportColumnData.Add("DEFECT_PCT", new ReportColumnInfo("Defect %", ColumnDataType.Percentage));
            reports.Add(SafetyDefect);

            NoVoltage = new BaseReport(true, RedirectCodes.TRIGGER_NO_VOLTAGE, "No Voltage Trigger", UserPermissions.NoVoltage);
            NoVoltage.FooterNote = TRIGGER_INVALID_EVINS;
            NoVoltage.DatabaseTarget = PortalFramework.Database.DatabaseTarget.Adhoc;
            NoVoltage.ReportColumnData = new Dictionary<string, ReportColumnInfo>();
            NoVoltage.ReportColumnData.Add("TEST_COUNT", new ReportColumnInfo("Test Count", ColumnDataType.Number));
            NoVoltage.ReportColumnData.Add("VOLT_COUNT", new ReportColumnInfo("Voltage Count", ColumnDataType.Number));
            NoVoltage.ReportColumnData.Add("VOLT_PCT", new ReportColumnInfo("Voltage %", ColumnDataType.Percentage));
            NoVoltage.ReportColumnData.Add("NOVOLT_COUNT", new ReportColumnInfo("No Voltage Count", ColumnDataType.Number));
            NoVoltage.ReportColumnData.Add("NOVOLT_PCT", new ReportColumnInfo("No Voltage %", ColumnDataType.Percentage));
            reports.Add(NoVoltage);

            TimeBeforeTest = new BaseReport(true, RedirectCodes.TRIGGER_TIME_BEFORE_TESTS, "Time Before Test Trigger", UserPermissions.TimeBeforeTests);
            TimeBeforeTest.FooterNote = TRIGGER_INVALID_EVINS;
            TimeBeforeTest.DatabaseTarget = PortalFramework.Database.DatabaseTarget.Adhoc;
            TimeBeforeTest.ReportColumnData = new Dictionary<string, ReportColumnInfo>();
            TimeBeforeTest.ReportColumnData.Add("TEST_COUNT", new ReportColumnInfo("Test Count", ColumnDataType.Number));
            TimeBeforeTest.ReportColumnData.Add("TOTAL_SHORT", new ReportColumnInfo("Total Tests with Short Times", ColumnDataType.Number));
            TimeBeforeTest.ReportColumnData.Add("TOTAL_SHORT_PCT", new ReportColumnInfo("Tests with Short Times %", ColumnDataType.Percentage));
            TimeBeforeTest.ReportColumnData.Add("NETWK_AVG_TEST_PCT", new ReportColumnInfo("Percent Network Average Tests with Short Times", ColumnDataType.Percentage));
            TimeBeforeTest.ReportColumnData.Add("NETWK_AVG_TBT", new ReportColumnInfo("Network Average Time Before Test"));
            reports.Add(TimeBeforeTest);

            StationIpectorTrigger = new BaseReport(true, RedirectCodes.TRIGGER_STATION_INSPECTOR, "Station Inspector Trigger Overview", UserPermissions.StationInspectionOverview);
            StationIpectorTrigger.FooterNote = TRIGGER_INVALID_EVINS;
            StationIpectorTrigger.DatabaseTarget = PortalFramework.Database.DatabaseTarget.Adhoc;
            StationIpectorTrigger.ReportColumnData = new Dictionary<string, ReportColumnInfo>();
            StationIpectorTrigger.ReportColumnData.Add("TRIGGER_IQ", new ReportColumnInfo("Trigger IQ"));
            StationIpectorTrigger.ReportColumnData.Add("TRIGGER", new ReportColumnInfo("Trigger"));
            StationIpectorTrigger.ReportColumnData.Add("DECILES", new ReportColumnInfo("Deciles", ColumnDataType.Number));
            reports.Add(StationIpectorTrigger);

            WeightedTriggerScore = new BaseReport(true, RedirectCodes.TRIGGER_WEIGHTED_SCORE, "Weighted Trigger Score", UserPermissions.WeightedTriggerScore);
            WeightedTriggerScore.FooterNote = TRIGGER_INVALID_EVINS;
            WeightedTriggerScore.DatabaseTarget = PortalFramework.Database.DatabaseTarget.Adhoc;
            WeightedTriggerScore.ReportColumnData = new Dictionary<string, ReportColumnInfo>();
            WeightedTriggerScore.ReportColumnData.Add("OBDPDA", new ReportColumnInfo("Weighted OBD Protocol Score({0}%)", ColumnDataType.Percentage));
            WeightedTriggerScore.ReportColumnData.Add("OBDRDA", new ReportColumnInfo("Weighted OBD Rejection Score({0}%)", ColumnDataType.Percentage));
            WeightedTriggerScore.ReportColumnData.Add("OBDIDA", new ReportColumnInfo("Weighted OBD Readiness Score({0}%)", ColumnDataType.Percentage));
            WeightedTriggerScore.ReportColumnData.Add("EVINDA", new ReportColumnInfo("eVIN Score({0}%)", ColumnDataType.Percentage));
            WeightedTriggerScore.ReportColumnData.Add("TBTDA", new ReportColumnInfo("TBT Score({0}%)", ColumnDataType.Percentage));
            WeightedTriggerScore.ReportColumnData.Add("SDDA", new ReportColumnInfo("Safety Defect Score({0}%)", ColumnDataType.Percentage));
            WeightedTriggerScore.ReportColumnData.Add("NOVDA", new ReportColumnInfo("No Voltage Score({0}%)", ColumnDataType.Percentage));
            reports.Add(WeightedTriggerScore);

            QuestionableStickers = new BaseReport(false, RedirectCodes.REPORT_QUESTIONABLE_STICKERS, "Questionable Stickers", UserPermissions.QuestionableStickers);
            QuestionableStickers.FooterNote = "\"D\" = \"Sticker issued multiple times. Verify stub is correctly completed by hand in the sticker book.\""
                                            + Environment.NewLine
                                            + " \"U\" = \"Sticker not issued through the NHOST unit. Verify stub is correctly completed by hand in the sticker book.\"";
            QuestionableStickers.DatabaseTarget = PortalFramework.Database.DatabaseTarget.Adhoc;
            QuestionableStickers.ProcedureName = "NH_STIK_QUESTIONABLE";
            QuestionableStickers.ReportColumnData = new Dictionary<string, ReportColumnInfo>();
            QuestionableStickers.ReportColumnData.Add("STICKERNUMBER", new ReportColumnInfo("Sticker Number"));
            QuestionableStickers.ReportColumnData.Add("FONREASON", new ReportColumnInfo("Reason"));
            QuestionableStickers.ReportColumnData.Add("VIN", new ReportColumnInfo("VIN"));
            QuestionableStickers.ReportColumnData.Add("TESTDATE", new ReportColumnInfo("Test Date"));
            QuestionableStickers.ReportColumnData.Add("TESTENDTIME", new ReportColumnInfo("Test Time"));
            QuestionableStickers.ReportColumnData.Add("INSPECTORID", new ReportColumnInfo("Mechanic ID"));
            QuestionableStickers.ReportColumnData.Add("MODELYEAR", new ReportColumnInfo("Year"));
            QuestionableStickers.ReportColumnData.Add("MAKE", new ReportColumnInfo("Make"));
            QuestionableStickers.ReportColumnData.Add("MODEL", new ReportColumnInfo("Model"));
            QuestionableStickers.ReportColumnData.Add("ODOMETER", new ReportColumnInfo("Odometer"));
            reports.Add(QuestionableStickers);

            StickerRegistry = new BaseReport(false, RedirectCodes.REPORT_STICKER_REGISTRY, "Sticker Registry", UserPermissions.StickerRegistry);
            StickerRegistry.FooterNote = "\"+\" = \"Sticker issued multiple times. Verify stub is correctly completed by hand in the sticker book.\""
                                            + Environment.NewLine
                                            + " \"*\" = \"Sticker not issued through the NHOST unit. Verify stub is correctly completed by hand in the sticker book.\"";
            StickerRegistry.DatabaseTarget = PortalFramework.Database.DatabaseTarget.Adhoc;
            StickerRegistry.ProcedureName = "NH_STIK_REGISTER";
            StickerRegistry.ReportColumnData = new Dictionary<string, ReportColumnInfo>();
            StickerRegistry.ReportColumnData.Add("STICKERNUM", new ReportColumnInfo("Sticker Number"));
            StickerRegistry.ReportColumnData.Add("VIN", new ReportColumnInfo("VIN"));
            StickerRegistry.ReportColumnData.Add("TESTDATE", new ReportColumnInfo("Test Date"));
            StickerRegistry.ReportColumnData.Add("INSPECTORID", new ReportColumnInfo("Mechanic ID"));
            StickerRegistry.ReportColumnData.Add("MODELYEAR", new ReportColumnInfo("Year"));
            StickerRegistry.ReportColumnData.Add("MAKE", new ReportColumnInfo("Make"));
            StickerRegistry.ReportColumnData.Add("MODEL", new ReportColumnInfo("Model"));
            StickerRegistry.ReportColumnData.Add("ODOMETER", new ReportColumnInfo("Odometer"));
            reports.Add(StickerRegistry);

            StickerDenial = new BaseReport(true, RedirectCodes.REPORT_STICKER_DENIAL, "Sticker Denial Report", UserPermissions.StickerDenialReport);
            StickerDenial.FooterNote = "NOTE: No Reports Prior To April 15, 2011 Are Currently Available ";
            StickerDenial.DatabaseTarget = PortalFramework.Database.DatabaseTarget.Adhoc;
            StickerDenial.ProcedureName = "NH_STIK_DENIAL";
            StickerDenial.ReportColumnData = new Dictionary<string, ReportColumnInfo>();
            StickerDenial.ReportColumnData.Add("STATIONID", new ReportColumnInfo("Station Id"));
            StickerDenial.ReportColumnData.Add("STATION_CERT", new ReportColumnInfo("Station Cert"));
            StickerDenial.ReportColumnData.Add("TYPE_DENIED", new ReportColumnInfo("Type Denied"));
            StickerDenial.ReportColumnData.Add("NAME", new ReportColumnInfo("Name"));
            StickerDenial.ReportColumnData.Add("CITY", new ReportColumnInfo("City"));
            StickerDenial.ReportColumnData.Add("REASON_CODE", new ReportColumnInfo("Reason Code"));
            StickerDenial.ReportColumnData.Add("DATE STATION ADDED TO SD", new ReportColumnInfo("Date Station Added to SD"));
            StickerDenial.ReportColumnData.Add("#_OF DAYS_ CONSECUTIVELY_ON_SD", new ReportColumnInfo("# Days Consecutively On SD", ColumnDataType.Number));
            StickerDenial.ReportColumnData.Add("DATE_STKRS_LAST_PURCHASED", new ReportColumnInfo("Date Stickers Last Purchased"));
            StickerDenial.ReportColumnData.Add("#_OF_STKRS_LAST_PURCHASED", new ReportColumnInfo("# Stickers Last Purchased", ColumnDataType.Number));

            StickerDenial.ReportColumnData.Add("STKRS_SOLD_LAST_12MONTHS", new ReportColumnInfo("Stickers Sold in Last 12 Mo.", ColumnDataType.Number));
            StickerDenial.ReportColumnData.Add("STKRS_ISSUED_LAST_12MONTHS", new ReportColumnInfo("Stickers Issues in Last 12 Mo.", ColumnDataType.Number));
            StickerDenial.ReportColumnData.Add("STATION_TYPE", new ReportColumnInfo("Station Type"));
            StickerDenial.ReportColumnData.Add("ADDRESS", new ReportColumnInfo("Address"));
            StickerDenial.ReportColumnData.Add("ZIP", new ReportColumnInfo("ZIP"));
            StickerDenial.ReportColumnData.Add("COUNTY", new ReportColumnInfo("County"));
            StickerDenial.ReportColumnData.Add("CONTACT", new ReportColumnInfo("Contact"));
            StickerDenial.ReportColumnData.Add("TELEPHONE", new ReportColumnInfo("Telephone"));
            StickerDenial.ReportColumnData.Add("# OF UNITS", new ReportColumnInfo("# Of Units", ColumnDataType.Number));
            StickerDenial.ReportColumnData.Add("AGREEMENTSTATUS", new ReportColumnInfo("Agreement Status"));

            StickerDenial.ReportColumnData.Add("GDSTATUS", new ReportColumnInfo("GD Status"));
            StickerDenial.ReportColumnData.Add("GDINACTIVE", new ReportColumnInfo("GD Inactive"));
            StickerDenial.ReportColumnData.Add("DATETIME_REPORT_GENERATED", new ReportColumnInfo("Datetime Report Generated"));
            StickerDenial.ReportColumnData.Add("DMV_FTP FILE NAME", new ReportColumnInfo("DMV FTP File Name"));
            StickerDenial.ReportColumnData.Add("D01", new ReportColumnInfo("D01"));
            StickerDenial.ReportColumnData.Add("D02", new ReportColumnInfo("D02"));
            StickerDenial.ReportColumnData.Add("D03", new ReportColumnInfo("D03"));
            StickerDenial.ReportColumnData.Add("D05", new ReportColumnInfo("D05"));
            StickerDenial.ReportColumnData.Add("D06", new ReportColumnInfo("D06"));
            StickerDenial.ReportColumnData.Add("D07", new ReportColumnInfo("D07"));

            StickerDenial.ReportColumnData.Add("D08", new ReportColumnInfo("D08"));
            StickerDenial.ReportColumnData.Add("D09", new ReportColumnInfo("D09"));
            StickerDenial.ReportColumnData.Add("B01", new ReportColumnInfo("B01"));
            StickerDenial.ReportColumnData.Add("B02", new ReportColumnInfo("B02"));
            StickerDenial.ReportColumnData.Add("B03", new ReportColumnInfo("B03"));
            StickerDenial.ReportColumnData.Add("T01", new ReportColumnInfo("T01"));
            StickerDenial.ReportColumnData.Add("B04", new ReportColumnInfo("B04"));
            StickerDenial.ReportColumnData.Add("T02", new ReportColumnInfo("T02"));
            StickerDenial.ReportColumnData.Add("B05", new ReportColumnInfo("B05"));
            StickerDenial.ReportColumnData.Add("B06", new ReportColumnInfo("B06"));

            StickerDenial.ReportColumnData.Add("B07", new ReportColumnInfo("B07"));
            StickerDenial.ReportColumnData.Add("B08", new ReportColumnInfo("B08"));
            StickerDenial.ReportColumnData.Add("B09", new ReportColumnInfo("B09"));
            StickerDenial.ReportColumnData.Add("B10", new ReportColumnInfo("B10"));
            StickerDenial.ReportColumnData.Add("B11", new ReportColumnInfo("B11"));
            StickerDenial.ReportColumnData.Add("T03", new ReportColumnInfo("T03"));
            StickerDenial.ReportColumnData.Add("T04", new ReportColumnInfo("T04"));

            reports.Add(StickerDenial);

            RejectionByMake = new BaseReport(true, RedirectCodes.REPORT_REJECTION_BY_MAKE, "Rejection Rate By Make", UserPermissions.RejectionRateByMake);
            RejectionByMake.DatabaseTarget = PortalFramework.Database.DatabaseTarget.Adhoc;
            RejectionByMake.ReportColumnData = new Dictionary<string, ReportColumnInfo>();
            RejectionByMake.ReportColumnData.Add("MAIN_COLUMN", new ReportColumnInfo("Make"));
            reports.Add(RejectionByMake);

            RejectionByModel = new BaseReport(true, RedirectCodes.REPORT_REJECTION_BY_MODEL, "Rejection Rate By Model", UserPermissions.RejectionRateByModel);
            RejectionByModel.DatabaseTarget = PortalFramework.Database.DatabaseTarget.Adhoc;
            RejectionByModel.ReportColumnData = new Dictionary<string, ReportColumnInfo>();
            RejectionByModel.ReportColumnData.Add("MAIN_COLUMN", new ReportColumnInfo("Model"));
            reports.Add(RejectionByModel);

            RejectionByModelYear = new BaseReport(true, RedirectCodes.REPORT_REJECTION_BY_MODELYEAR, "Rejection Rate By Model Year", UserPermissions.RejectionRateByModelYear);
            RejectionByModelYear.DatabaseTarget = PortalFramework.Database.DatabaseTarget.Adhoc;
            RejectionByModelYear.ReportColumnData = new Dictionary<string, ReportColumnInfo>();
            RejectionByModelYear.ReportColumnData.Add("MAIN_COLUMN", new ReportColumnInfo("Model Year"));
            reports.Add(RejectionByModelYear);

            RejectionByCounty = new BaseReport(true, RedirectCodes.REPORT_REJECTION_BY_COUNTY, "Rejection Rate By County", UserPermissions.RejectionRateByCounty);
            RejectionByCounty.DatabaseTarget = PortalFramework.Database.DatabaseTarget.Adhoc;
            RejectionByCounty.ReportColumnData = new Dictionary<string, ReportColumnInfo>();
            RejectionByCounty.ReportColumnData.Add("MAIN_COLUMN", new ReportColumnInfo("County"));
            reports.Add(RejectionByCounty);

            RejectionByInspector = new BaseReport(true, RedirectCodes.REPORT_REJECTION_BY_INSPECTOR, "Rejection Rate By Inspector", UserPermissions.RejectionRateByInspector);
            RejectionByInspector.DatabaseTarget = PortalFramework.Database.DatabaseTarget.Adhoc;
            RejectionByInspector.ReportColumnData = new Dictionary<string, ReportColumnInfo>();
            RejectionByInspector.ReportColumnData.Add("MAIN_COLUMN", new ReportColumnInfo("Inspector ID"));
            reports.Add(RejectionByInspector);

            RejectionByStation = new BaseReport(true, RedirectCodes.REPORT_REJECTION_BY_STATION, "Rejection Rate By Station", UserPermissions.RejectionRateByStation);
            RejectionByStation.DatabaseTarget = PortalFramework.Database.DatabaseTarget.Adhoc;
            RejectionByStation.ReportColumnData = new Dictionary<string, ReportColumnInfo>();
            RejectionByStation.ReportColumnData.Add("MAIN_COLUMN", new ReportColumnInfo("Station ID"));
            reports.Add(RejectionByStation);

            MonthlyRejection = new BaseReport(true, RedirectCodes.REPORT_MONTHLY_REJECTION, "Monthly Rejection Report", UserPermissions.MonthlyRejectionReport);
            MonthlyRejection.DatabaseTarget = PortalFramework.Database.DatabaseTarget.Adhoc;
            MonthlyRejection.ReportColumnData = MonthlyRejectionHeaders.MainColumns;
            reports.Add(MonthlyRejection);
        }

        /// <summary>A list of all base reports.</summary>
        private static List<BaseReport> reports;
        public static BaseReport[] Reports
        {
            get { return reports.ToArray(); }
        }

        // Inspection Reports
        public static BaseReport MainReport { get; private set; }
        public static BaseReport ModelYearRejection { get; private set; }
        public static BaseReport StationRejection { get; private set; }
        public static BaseReport CountyStationRejection { get; private set; }
        public static BaseReport MechanicRejection { get; private set; }
        public static BaseReport OBDResults { get; private set; }
        public static BaseReport FirstTestReset { get; private set; }

        // Charts (Graphs)
        public static BaseReport EmissionTestRejectionRates { get; private set; }
        public static BaseReport OBDIIReadinessMonitors { get; private set; }
        public static BaseReport OBDIIMILStatus { get; private set; }
        public static BaseReport OBDIICommunications { get; private set; }
        public static BaseReport OBDIIDTCErrorCodes { get; private set; }
        public static BaseReport OBDIIProtocolUsage { get; private set; }

        // Triggers
        public static BaseReport EVINMismatch { get; private set; }
        public static BaseReport CommProtocol { get; private set; }
        public static BaseReport Rejection { get; private set; }
        public static BaseReport ReadinessMismatch { get; private set; }
        public static BaseReport SafetyDefect { get; private set; }
        public static BaseReport NoVoltage { get; private set; }
        public static BaseReport TimeBeforeTest { get; private set; }
        public static BaseReport StationIpectorTrigger { get; private set; }
        public static BaseReport WeightedTriggerScore { get; private set; }

        // Sticker Reports
        public static BaseReport QuestionableStickers { get; private set; }
        public static BaseReport StickerRegistry { get; private set; }
        public static BaseReport StickerDenial { get; private set; }

        // Predefined Queries
        public static BaseReport RejectionByMake { get; private set; }
        public static BaseReport RejectionByModel { get; private set; }
        public static BaseReport RejectionByModelYear { get; private set; }
        public static BaseReport RejectionByCounty { get; private set; }
        public static BaseReport RejectionByInspector { get; private set; }
        public static BaseReport RejectionByStation { get; private set; }
        public static BaseReport MonthlyRejection { get; private set; }

        /// <summary>Returns a Base Report based on a string code.</summary>
        /// <param name="code">The nav code of which base report you would like returned.</param>
        public static BaseReport Find(string code)
        {
            BaseReport baseReport = null;

            foreach (BaseReport rpt in reports)
            {
                if (rpt.PageCode == code)
                {
                    baseReport = rpt;
                    break;
                }
            }

            return baseReport;
        }

        /// <summary>Returns a string formatted with a percent sign for the ratio of 2 numbers.</summary>
        internal static string CalculatePercentString(int numerator, int denominator)
        {
            return CalculatePercent(numerator, denominator).ToString() + "%";
        }

        /// <summary>Returns the ratio of 2 numbers as a percent value</summary>
        internal static double CalculatePercent(int numerator, int denominator)
        {
            decimal returnVal = 0;

            if (denominator != 0)
            {
                returnVal = ((decimal)numerator / (decimal)denominator) * 100m;
            }

            return (double)Math.Round(returnVal, 2);
        }

        public static OracleResponse GetOracleResponse(string sql)
        {
            GDDatabaseClient.Oracle.OracleResponse response = ODAP.GetDataTable(sql, DatabaseTarget.Adhoc);
            return response;
        }

        public static DataTable GetProcedureDataTable(string procName, OracleParameter[] oParams, DatabaseTarget dbTarget = DatabaseTarget.Adhoc)
        {
            DataTable dt = new DataTable();
            ILogger Logger = SessionHelper.GetSessionLogger(System.Web.HttpContext.Current.Session);
            
            try
            {
                Logger.Log("BaseReportMaster.GetProcedureDataTable Method initializing call to ODAP.GetDataTableFromProcedure.", LogSeverity.Information);
                Logger.Info("Procedure: " + procName + " Target DB: " + dbTarget.ToString() + Environment.NewLine + SerializeOracleParameters(oParams));

                GDDatabaseClient.Oracle.OracleResponse response = ODAP.GetDataTableFromProcedure(procName, oParams, dbTarget);
                if (response.Successful)
                {
                    if (response.HasResults)
                    {
                        dt = response.ResultsTable;
                    }
                }
                else
                {                   
                    Logger.Error("There was a problem loading a data table for the procedure: " + procName);
                    Logger.Log("Unsuccsessful OracleResponse in BaseReportMaster.GetProcedureDataTable" + Environment.NewLine + "Error Code: " + response.ErrorCode + Environment.NewLine + response.ErrorMessage, LogSeverity.Error);
                    System.Diagnostics.Debug.WriteLine("Unsuccsessful OracleResponse returning DataTable from BaseReportMaster.GetProcedureDataTable. TargetDB= " + dbTarget.ToString());
                }
            }
            catch(Exception ex)
            {
                Logger.Error("There was a problem loading a data table for the procedure: " + procName);
                Logger.Log("Exception thrown in BaseReportMaster.GetProcedureDataTable" + Environment.NewLine + "Exception: " + ex.ToString() + Environment.NewLine + ex.Message, LogSeverity.Error);
                System.Diagnostics.Debug.WriteLine("Exception thrown while returning DataTable from BaseReportMaster.GetProcedureDataTable. TargetDB= " + dbTarget.ToString());
            }

            Logger.Info("Returning DataTable from BaseReportMaster.GetProcedureDataTable Method.");
            return dt;
        }

        private static string SerializeOracleParameters(OracleParameter[] oParams)
        {
            string returnVal = " Oracle Params: ";

            foreach (OracleParameter oParam in oParams)
            {
                returnVal += oParam.ParameterName + ": " + (oParam.Value == null? String.Empty : oParam.Value.ToString()) + Environment.NewLine;
            }

            return returnVal;
        }

        public static readonly string QUERY_REPORT =
        " sum(case when OVERALLPF = '1' then 1 else 0    end) over(partition by {0} ) OVERALL_PASS, " + Environment.NewLine +
        " sum(case when OVERALLPF = '2' then 1 else 0    end) over(partition by {0} ) OVERALL_REJECT, " + Environment.NewLine +
        " sum(case when OVERALLPF = '3' then 1 else 0    end) over(partition by {0} ) OVERALL_CORRECTED, " + Environment.NewLine +
        " sum(case when OVERALLPF = '5' then 1 else 0    end) over(partition by {0} ) OVERALL_ADMIN, " + Environment.NewLine +
        " sum(case when OVERALLPF not in ('1','2','3','5') then 1 else 0    end) over(partition by {0} ) OVERALL_ABORT, " + Environment.NewLine +
        " sum(case when EMISSTESTTYPE = 'O' and OBDPF = '0' then 1 else 0    end) over(partition by {0} ) OBD_ABORT, " + Environment.NewLine +
        " sum(case when EMISSTESTTYPE = 'O' and OBDPF = '1' then 1 else 0    end) over(partition by {0} ) OBD_PASS, " + Environment.NewLine +
        " sum(case when EMISSTESTTYPE = 'O' and OBDPF = '2' then 1 else 0    end) over(partition by {0} ) OBD_REJECT, " + Environment.NewLine +
        " sum(case when EMISSTESTTYPE = 'O' and OBDPF = '2' and substr(OBDMILCMDON,2,1) = '3' then 1 else 0    end) over(partition by {0} ) OBD_NO_COM, " + Environment.NewLine +
        " sum(case when EMISSTESTTYPE = 'O' and OBDPF = '9' then 1 else 0    end) over(partition by {0} ) OBD_UNTESTED, " + Environment.NewLine +
        " sum(case when EMISSTESTTYPE = 'V' and VISUALPF = '0' then 1 else 0    end) over(partition by {0} ) VISUAL_ABORT, " + Environment.NewLine +
        " sum(case when EMISSTESTTYPE = 'V' and VISUALPF = '1' then 1 else 0    end) over(partition by {0} ) VISUAL_PASS, " + Environment.NewLine +
        " sum(case when EMISSTESTTYPE = 'V' and VISUALPF = '2' then 1 else 0    end) over(partition by {0} ) VISUAL_REJECT, " + Environment.NewLine +
        " sum(case when EMISSTESTTYPE = 'V' and VISUALPF = '3' then 1 else 0    end) over(partition by {0} ) VISUAL_CORRECTED, " + Environment.NewLine +
        " sum(case when EMISSTESTTYPE = 'V' and VISUALPF = '9' then 1 else 0    end) over(partition by {0} ) VISUAL_UNTESTED, " + Environment.NewLine +
        " sum(case when SAFETYPF = '0' then 1 else 0    end) over(partition by {0} ) SAFETY_ABORT, " + Environment.NewLine +
        " sum(case when SAFETYPF = '1' then 1 else 0    end) over(partition by {0} ) SAFETY_PASS, " + Environment.NewLine +
        " sum(case when SAFETYPF = '2' then 1 else 0    end) over(partition by {0} ) SAFETY_REJECT, " + Environment.NewLine +
        " sum(case when SAFETYPF = '3' then 1 else 0    end) over(partition by {0} ) SAFETY_CORRECTED, " + Environment.NewLine +
        " sum(case when SAFETYPF = '9' then 1 else 0    end) over(partition by {0} ) SAFETY_UNTESTED " + Environment.NewLine +
            "from new_testrecord " + Environment.NewLine;

        public static readonly string QUERY_OBD =
        " select distinct stationid, " +
        " sum(case when obdstep = '0' then 1 else 0 end) over(partition by stationid) INIT_STEP, " + Environment.NewLine +
        " sum(case when obdstep = '2' then 1 else 0 end) over(partition by stationid) VOLTAGE_STEP, " + Environment.NewLine +
        " sum(case when obdstep in ('3','4','5') then 1 else 0 end) over(partition by stationid) TEST_STEP, " + Environment.NewLine +
        " sum(case when obdstep in ('1','6','7','8','9') then 1 else 0 end) over(partition by stationid) EXTRAINFO_STEP, " + Environment.NewLine +
        " sum(case when substr(OBDMILCMDON,2,1) = '0' then 1 else 0 end) over(partition by stationid) MIL_OK, " + Environment.NewLine +
        " sum(case when substr(OBDMILCMDON,2,1) = '3' then 1 else 0 end) over(partition by stationid) NO_COM, " + Environment.NewLine +
        " sum(case when substr(OBDMILCMDON,2,1) = '4' then 1 else 0 end) over(partition by stationid) UNREADY, " + Environment.NewLine +
        " sum(case when substr(OBDMILCMDON,2,1) = '5' then 1 else 0 end) over(partition by stationid) MIL_ON, " + Environment.NewLine +
        " sum(case when substr(OBDMILCMDON,2,1) = '6' then 1 else 0 end) over(partition by stationid) READY_EXEMPT, " + Environment.NewLine +
        " sum(case when substr(OBDMILCMDON,2,1) = '7' then 1 else 0 end) over(partition by stationid) COM_EXEMPT " + Environment.NewLine +
        " from new_testrecord " + Environment.NewLine;

        public const string BASE_REPORT_DATA = "BASE_REPORT_DATA";

        public const string TRIGGER_INVALID_EVINS = "* Invalid eVINs are represented as hexadecimal values in the format: 0x(INVALID EVIN).";
    }

    /// <summary>Defines basic functionality for a report.</summary>
    public interface IReportData
    {
        /// <summary>Takes data from a datarow and places it into the properties of an instance report class.</summary>
        void SetPropertyValues(DataRow dataRow);
        /// <summary>Takes data from report class properties and places them into a report row object.</summary>
        void AddDataToReportRow(ReportRow currentRow);
        /// <summary>Sets the grand total values for report class properties.</summary>
        void SetTotalRowValues(ReportRow totalRow);
        /// <summary>Creates a sql string for running a report.</summary>
        string BuildSQL();
        /// <summary>Static information about a report.</summary>
        BaseReport BaseReport { get; set; }
    }

    /// <summary>Contains static information about reports such as column definitions, target db, permission code, etc.</summary>
    public class BaseReport
    {
        /// <summary>Creates a default BaseReport object.</summary>
        public BaseReport()
        {
            this.RunReportOnInitialLoad = false;
            this.ReportTitle = String.Empty;
            this.DatabaseTarget = PortalFramework.Database.DatabaseTarget.Adhoc;
            this.SQLTable = "TESTRECORD";
        }

        /// <summary>Creates a BaseReport object based on parameters.</summary>
        /// <param name="runReportOnInitialLoad">Whether or not a report should be ran when the page first loads.</param>
        /// <param name="navCode">The nav code for a report.</param>
        /// <param name="pageCode">The code for identifying the asp page.</param>
        /// <param name="reportTitle">The title of the report.</param>
        /// <param name="userPermission">The User permission info for the report.</param>
        public BaseReport(bool runReportOnInitialLoad, string pageCode, string reportTitle, UserPermission userPermission)
        {
            this.RunReportOnInitialLoad = runReportOnInitialLoad;
            this.PageCode = pageCode;
            this.ReportTitle = reportTitle;
            this.DatabaseTarget = PortalFramework.Database.DatabaseTarget.Adhoc;
            this.SQLTable = "TESTRECORD";
            this.UserPermission = userPermission;
        }

        public string ReportTitle { get; private set; }
        public string PageCode { get; private set; }
        public string FooterNote { get; set; }
        public string SQLTable { get; set; }
        public string ProcedureName { get; set; }
        public bool RunReportOnInitialLoad { get; private set; }
        public Dictionary<string, ReportColumnInfo> ReportColumnData { get; set; }
        public PortalFramework.Database.DatabaseTarget DatabaseTarget { get; set; }
        public UserPermission UserPermission { get; set; }
    }

    /// <summary>Contains information about a report column such as data type, display name, DB column(attribute) name.</summary>
    public class ReportColumnInfo
    {
        public ReportColumnInfo()
        {
            DisplayName ="Header";
            ColumnDataType = ColumnDataType.String;
        }

        public ReportColumnInfo(string displayName)
        {
            DisplayName = displayName;
            ColumnDataType = ColumnDataType.String;
        }
        
        public ReportColumnInfo(string displayName, ColumnDataType columnDataType)
        {
            DisplayName = displayName;
            ColumnDataType = columnDataType;
        }

        /// <summary>The string that is displayed in the rendered report header for each column.  </summary>
        public string DisplayName { get; set; }
        /// <summary>The value of the attribute(column) from the database, or otherwise the backing value of the display name.  </summary>
        public string AttributeName { get; set; }
        /// <summary>The Data Type of the report column.  </summary>
        public ColumnDataType ColumnDataType { get; set; }
    }
}