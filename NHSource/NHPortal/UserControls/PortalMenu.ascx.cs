using NHPortal.Classes.User;
using PortalFramework.MenuModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NHPortal.UserControls
{
    public partial class PortalMenu : System.Web.UI.UserControl
    {
        private PortalMenuItemCollection m_topLevelItems;
        
        protected void Page_Init(object sender, EventArgs e)
        {
            GetPortalMenuItems();
        }

        protected void Page_Load(object sender, EventArgs e)
        {            
            portalMenu.Items.Clear();
            if (m_topLevelItems == null)
            {
                BuildPortalMenu();
                SetPortalMenu();
            }
            ApplyItemsToMenu();
        }

        private void GetPortalMenuItems()
        {
            m_topLevelItems = NHPortal.Classes.SessionHelper.GetPortalMenuItems(this.Session);
        }

        private void SetPortalMenu()
        {
            NHPortal.Classes.SessionHelper.SetPortalMenuItems(this.Session, m_topLevelItems);
        }

        private void BuildPortalMenu()
        {
            m_topLevelItems = new PortalMenuItemCollection();
            m_topLevelItems.AddItem(new PortalMenuItem("My Home", 0, RedirectCodes.MY_HOME));
            m_topLevelItems.AddItem(new PortalMenuItem("My Favorites", 0, RedirectCodes.MY_FAVORITES));
            m_topLevelItems.AddItem(BuildReportsMenu());
            m_topLevelItems.AddItem(BuildTestInquiryMenu());
            m_topLevelItems.AddItem(BuildGraphMenu());
            m_topLevelItems.AddItem(BuildTriggerMenu());
            m_topLevelItems.AddItem(BuildAdhocMenu());
            m_topLevelItems.AddItem(BuildAdminMenu());
            m_topLevelItems.AddItem(new PortalMenuItem("Update Password", 0, RedirectCodes.USER_UPDATE_PASSWORD));
            m_topLevelItems.AddItem(new PortalMenuItem("Logout", 0, RedirectCodes.LOGOUT));
        }

        private PortalMenuItem BuildReportsMenu()
        {
            PortalMenuItem reportMenu = new PortalMenuItem("Data Reports", 0);
            reportMenu.AddSubItem(BuildInspectionReportsMenu());
            reportMenu.AddSubItem(BuildSafetyReportsMenu());
            reportMenu.AddSubItem(BuildCallCenterReportsMenu());
            reportMenu.AddSubItem(BuildAuditReportsMenu());
            reportMenu.AddSubItem(BuildStickerReportsMenu());
            reportMenu.AddSubItem(BuildServiceReportsMenu());
            reportMenu.AddSubItem(BuildSalesReportsMenu());

            return reportMenu;
        }

        private PortalMenuItem BuildInspectionReportsMenu()
        {
            PortalMenuItem inspectionReports = new PortalMenuItem("Inspection Reports", 0);
            inspectionReports.AddSubItem(new PortalMenuItem("Main Report", UserPermissions.MainReport.Code, RedirectCodes.REPORT_MAIN));
            inspectionReports.AddSubItem(new PortalMenuItem("Model Year Rejection", UserPermissions.ModelYearRejection.Code, RedirectCodes.REPORT_MODEL_YR_REJECTION));
            inspectionReports.AddSubItem(new PortalMenuItem("Station Rejection", UserPermissions.StationRejection.Code, RedirectCodes.REPORT_STATION_REJECTION));
            inspectionReports.AddSubItem(new PortalMenuItem("County Station Rejection", UserPermissions.CountyStationRejection.Code, RedirectCodes.REPORT_CNTY_ST_REJECTION));
            inspectionReports.AddSubItem(new PortalMenuItem("Mechanic Rejection", UserPermissions.MechanicRejection.Code, RedirectCodes.REPORT_MECHANIC_REJECTION));
            inspectionReports.AddSubItem(new PortalMenuItem("OBD Results", UserPermissions.OBDResults.Code, RedirectCodes.REPORT_OBD_RESULTS));
            inspectionReports.AddSubItem(new PortalMenuItem("First Test Reset", UserPermissions.FirstTestReset.Code, RedirectCodes.REPORT_FIRST_TEST_RESET));

            return inspectionReports;
        }

        private PortalMenuItem BuildSafetyReportsMenu()
        {
            PortalMenuItem safetyMenu = new PortalMenuItem("Safety Statistic Reports", 0);
            safetyMenu.AddSubItem(new PortalMenuItem("Safety Deficiency", UserPermissions.SafetyDeficiency.Code, RedirectCodes.REPORT_SAFETY_DEFICIENCY));

            return safetyMenu;
        }

        private PortalMenuItem BuildCallCenterReportsMenu()
        {
            PortalMenuItem callCenterReports = new PortalMenuItem("Call Center Reports", 0);
            callCenterReports.AddSubItem(new PortalMenuItem("Calls By Hour", UserPermissions.CallsByHour.Code, RedirectCodes.REPORT_CALLS_BY_HOUR));
            callCenterReports.AddSubItem(new PortalMenuItem("Calls By Duration", UserPermissions.CallsByDuration.Code, RedirectCodes.REPORT_CALLS_BY_DURATION));
            callCenterReports.AddSubItem(new PortalMenuItem("Reasons & Resolutions", UserPermissions.ReasonsAndResolutions.Code, RedirectCodes.REPORT_REASONS_RESOLUTION));

            return callCenterReports;
        }

        private PortalMenuItem BuildAuditReportsMenu()
        {
            PortalMenuItem auditReports = new PortalMenuItem("Audit Reports", 0);
            auditReports.AddSubItem(new PortalMenuItem("Suspect Test Report", UserPermissions.SuspectTestReport.Code, RedirectCodes.REPORT_SUSPECT_TEST));
            auditReports.AddSubItem(new PortalMenuItem("Station Report", UserPermissions.StationReport.Code, RedirectCodes.REPORT_STATION));
            auditReports.AddSubItem(new PortalMenuItem("Audit History Inquiry", UserPermissions.AuditHistoryInquiry.Code, RedirectCodes.REPORT_AUDIT_HISTORY_INQUIRY));
            auditReports.AddSubItem(new PortalMenuItem("Rescheduled Audits", UserPermissions.RescheduledAudits.Code, RedirectCodes.REPORT_RESCHEDULED_AUDITS));

            return auditReports;
        }

        private PortalMenuItem BuildStickerReportsMenu()
        {
            PortalMenuItem stickerReports = new PortalMenuItem("Sticker Reports", 0);
            stickerReports.AddSubItem(new PortalMenuItem("Sticker Denial Report", UserPermissions.StickerDenialReport.Code, RedirectCodes.REPORT_STICKER_DENIAL));
            stickerReports.AddSubItem(new PortalMenuItem("Sticker Registry", UserPermissions.StickerRegistry.Code, RedirectCodes.REPORT_STICKER_REGISTRY));
            stickerReports.AddSubItem(new PortalMenuItem("Questionable Stickers", UserPermissions.QuestionableStickers.Code, RedirectCodes.REPORT_QUESTIONABLE_STICKERS));
            stickerReports.AddSubItem(new PortalMenuItem("Redeem/Sold Mismatch", UserPermissions.RedeemSoldMismatch.Code, RedirectCodes.REPORT_REDEEM_SOLD_MISMATCH));

            return stickerReports;
        }

        private PortalMenuItem BuildServiceReportsMenu()
        {
            PortalMenuItem serviceReports = new PortalMenuItem("Service Reports", 0);
            serviceReports.AddSubItem(new PortalMenuItem("Component Performance", UserPermissions.ComponentPerformance.Code, RedirectCodes.REPORT_COMPONENT_PERFORMANCE));
            serviceReports.AddSubItem(new PortalMenuItem("Station Support", UserPermissions.StationSupport.Code, RedirectCodes.REPORT_STATION_SUPPORT));
            serviceReports.AddSubItem(new PortalMenuItem("Component Inventory", UserPermissions.ComponentInventory.Code, RedirectCodes.REPORT_COMPONENT_INVENTORY));
            serviceReports.AddSubItem(new PortalMenuItem("Consumable Inventory", UserPermissions.ConsumableInventory.Code, RedirectCodes.REPORT_CONSUMABLE_INVENTORY));
            serviceReports.AddSubItem(new PortalMenuItem("History Report", UserPermissions.HistoryReport.Code, RedirectCodes.REPORT_HISTORY_REPORT));

            return serviceReports;
        }
        private PortalMenuItem BuildSalesReportsMenu()
        {
            PortalMenuItem salesReports = new PortalMenuItem("Predefined Queries", 0);
            salesReports.AddSubItem(new PortalMenuItem("Consumable Sales", UserPermissions.ConsumableSales.Code, RedirectCodes.REPORT_CONSUMABLE_SALES));
            salesReports.AddSubItem(new PortalMenuItem("Rejection Rate by Make", UserPermissions.RejectionRateByMake.Code, RedirectCodes.REPORT_REJECTION_BY_MAKE));
            salesReports.AddSubItem(new PortalMenuItem("Rejection Rate by Model", UserPermissions.RejectionRateByModel.Code, RedirectCodes.REPORT_REJECTION_BY_MODEL));
            salesReports.AddSubItem(new PortalMenuItem("Rejection Rate by Model Year", UserPermissions.RejectionRateByModelYear.Code, RedirectCodes.REPORT_REJECTION_BY_MODELYEAR));
            salesReports.AddSubItem(new PortalMenuItem("Rejection Rate by County", UserPermissions.RejectionRateByCounty.Code, RedirectCodes.REPORT_REJECTION_BY_COUNTY));
            salesReports.AddSubItem(new PortalMenuItem("Rejection Rate by Inspector", UserPermissions.RejectionRateByInspector.Code, RedirectCodes.REPORT_REJECTION_BY_INSPECTOR));
            salesReports.AddSubItem(new PortalMenuItem("Rejection Rate by Station", UserPermissions.RejectionRateByStation.Code, RedirectCodes.REPORT_REJECTION_BY_STATION));
            salesReports.AddSubItem(new PortalMenuItem("Monthly Rejection Report", UserPermissions.MonthlyRejectionReport.Code, RedirectCodes.REPORT_MONTHLY_REJECTION));
            salesReports.AddSubItem(new PortalMenuItem("No Final Outcome Report", UserPermissions.NoFinalOutcomeReport.Code, RedirectCodes.REPORT_NO_FINAL_OUTCOME));

            return salesReports;
        }

        private PortalMenuItem BuildTriggerMenu()
        {
            PortalMenuItem triggersMenu = new PortalMenuItem("Triggers", 0);
            triggersMenu.AddSubItem(new PortalMenuItem("eVIN Mismatch", UserPermissions.eVINMismatch.Code, RedirectCodes.TRIGGER_EVIN_MISMATCH));
            triggersMenu.AddSubItem(new PortalMenuItem("Communication Protocol", UserPermissions.CommunicationProtocol.Code, RedirectCodes.TRIGGER_COMM_PROTOCOL));
            triggersMenu.AddSubItem(new PortalMenuItem("Rejection", UserPermissions.Rejection.Code, RedirectCodes.TRIGGER_REJECTION));
            triggersMenu.AddSubItem(new PortalMenuItem("Readiness Pattern Mismatch", UserPermissions.ReadinessPatternMismatch.Code, RedirectCodes.TRIGGER_READINESS_MISMATCH));
            triggersMenu.AddSubItem(new PortalMenuItem("Time Before Tests", UserPermissions.TimeBeforeTests.Code, RedirectCodes.TRIGGER_TIME_BEFORE_TESTS));
            triggersMenu.AddSubItem(new PortalMenuItem("Station Safety Defect", UserPermissions.StationSafetyDefect.Code, RedirectCodes.TRIGGER_STATION_SAFETY_DEFECT));
            triggersMenu.AddSubItem(new PortalMenuItem("No Voltage", UserPermissions.NoVoltage.Code, RedirectCodes.TRIGGER_NO_VOLTAGE));
            triggersMenu.AddSubItem(new PortalMenuItem("Station-Inspector Trigger Overview", UserPermissions.StationInspectionOverview.Code, RedirectCodes.TRIGGER_STATION_INSPECTOR));
            triggersMenu.AddSubItem(new PortalMenuItem("Weighted Trigger Score (WTS™)", UserPermissions.WeightedTriggerScore.Code, RedirectCodes.TRIGGER_WEIGHTED_SCORE));

            return triggersMenu;
        }

        private PortalMenuItem BuildTestInquiryMenu()
        {
            PortalMenuItem TestInquiryMenu = new PortalMenuItem("Test Inquiry Reports", 0);
            TestInquiryMenu.AddSubItem(new PortalMenuItem("Mobile Testing Tablet Report Inquiry by VIN", UserPermissions.MobileTestingTabletByVIN.Code, RedirectCodes.REPORT_INQ_MOBILE_TEST_BY_VIN));
            TestInquiryMenu.AddSubItem(new PortalMenuItem("Inspection Inquiry by VIN", UserPermissions.InspectionInquiryByVIN.Code, RedirectCodes.REPORT_INQ_INSPECTION_BY_VIN));
            TestInquiryMenu.AddSubItem(new PortalMenuItem("Inspection Inquiry by Plate Number", UserPermissions.InspectionInquiryByPlate.Code, RedirectCodes.REPORT_INQ_INSPECTION_BY_PLATE));
            TestInquiryMenu.AddSubItem(new PortalMenuItem("Inspection Inquiry by Station and Date", UserPermissions.InspectionInquiryByStation.Code, RedirectCodes.REPORT_INQ_INSPECTION_BY_STATION));
            TestInquiryMenu.AddSubItem(new PortalMenuItem("Inspection Inquiry by Sticker", UserPermissions.InspectionInquiryByStation.Code, RedirectCodes.REPORT_INQ_INSPECTION_BY_STICKER));

            return TestInquiryMenu;
        }

        private PortalMenuItem BuildGraphMenu()
        {
            PortalMenuItem graphsMenu = new PortalMenuItem("Graphs", 0);
            graphsMenu.AddSubItem(new PortalMenuItem(NHPortal.Classes.BaseReportMaster.EmissionTestRejectionRates.ReportTitle,
                UserPermissions.EmissionTestRejectionRates.Code, RedirectCodes.GRAPH_EMISSION_TEST_REJECT_RATE));
            graphsMenu.AddSubItem(new PortalMenuItem(NHPortal.Classes.BaseReportMaster.OBDIIReadinessMonitors.ReportTitle,
                UserPermissions.OBDIIReadinessMonitor.Code, RedirectCodes.GRAPH_OBD_READINESS_MONITORS));
            graphsMenu.AddSubItem(new PortalMenuItem(NHPortal.Classes.BaseReportMaster.OBDIIMILStatus.ReportTitle,
                UserPermissions.OBDIIMILStatus.Code, RedirectCodes.GRAPH_OBD_MIL_STATUS));
            graphsMenu.AddSubItem(new PortalMenuItem(NHPortal.Classes.BaseReportMaster.OBDIICommunications.ReportTitle,
                UserPermissions.OBDIICommunications.Code, RedirectCodes.GRAPH_OBD_COMMUNICATIONS));
            graphsMenu.AddSubItem(new PortalMenuItem(NHPortal.Classes.BaseReportMaster.OBDIIDTCErrorCodes.ReportTitle,
                UserPermissions.OBDIIDTCErrorCodes.Code, RedirectCodes.GRAPH_OBD_DTC_ERR_CODES));
            graphsMenu.AddSubItem(new PortalMenuItem(NHPortal.Classes.BaseReportMaster.OBDIIProtocolUsage.ReportTitle,
                UserPermissions.OBDIIProtocolUsage.Code, RedirectCodes.GRAPH_OBD_PROTOCOL_USAGE));

            return graphsMenu;
        }

        private PortalMenuItem BuildAdminMenu()
        {
            PortalMenuItem adminMenu = new PortalMenuItem("Admin", 0);
            adminMenu.AddSubItem(new PortalMenuItem("Add User", UserPermissions.UserMaintenance.Code, RedirectCodes.USER_ADD_USER));
            adminMenu.AddSubItem(new PortalMenuItem("Update User", UserPermissions.UserMaintenance.Code, RedirectCodes.USER_EDIT_USER));
            adminMenu.AddSubItem(new PortalMenuItem("View User", UserPermissions.UserMaintenance.Code, RedirectCodes.USER_VIEW_USER));
            adminMenu.AddSubItem(new PortalMenuItem("Documents", UserPermissions.Documents.Code, RedirectCodes.DOCUMENTS));

            return adminMenu;
        }

        private PortalMenuItem BuildAdhocMenu()
        {
            PortalMenuItem adhocMenu = new PortalMenuItem("Adhoc", 0);
            adhocMenu.AddSubItem(new PortalMenuItem("Report Builder Tool", UserPermissions.ReportBuilderTool.Code, RedirectCodes.ADHOC_BUILDER));
            adhocMenu.AddSubItem(new PortalMenuItem("SQL Report Builder", UserPermissions.SQLReportBuilder.Code, RedirectCodes.SQL_BUILDER));

            return adhocMenu;
        }

        private void ApplyItemsToMenu()
        {
            if (m_topLevelItems != null)
            {
                MenuItem[] menuItems = m_topLevelItems.ToMenuItems();
                foreach (MenuItem item in menuItems)
                {
                    portalMenu.Items.Add(item);
                }
            }
        }

        /// <summary>Evaluates the visibility of each menu item for a user based on the user's permissions.</summary>
        /// <param name="user">The user to evaluate the menu visibility for.</param>
        public void EvaluateMenuVisibility(PortalUser user)
        {
            if (user != null && m_topLevelItems != null)
            {
                foreach (PortalMenuItem item in m_topLevelItems)
                {
                    EvaluateItemVisibility(user, item);
                }
            }
        }

        private void EvaluateItemVisibility(PortalUser user, PortalMenuItem item)
        {
            if (item.PermissionCode == 0 || user.HasReadAccess(item.PermissionCode))
            {
                foreach (PortalMenuItem childItem in item.SubItems)
                {
                    EvaluateItemVisibility(user, childItem);
                }

                item.IsVisible = item.SubItemsVisible;
            }            
        }

        protected void RenderJSRelativeTargetURL()
        {
            Response.Write(this.ResolveClientUrl("~/PortalNavHandler.ashx"));
        }
    }

    /// <summary>Stores static redirect codes for the navigation menu.</summary>
    public static class RedirectCodes
    {
        public const string MY_HOME = "MY_HOME";
        public const string MY_FAVORITES = "MY_FAVORITES";
        public const string DOCUMENTS = "DOCUMENTS";

        public const string REPORT_MAIN = "RPT_MAIN_RPT";
        public const string REPORT_MODEL_YR_REJECTION = "RPT_MDLYR_REJECT";
        public const string REPORT_STATION_REJECTION = "RPT_ST_REJECT";
        public const string REPORT_CNTY_ST_REJECTION = "RPT_CNTY_ST_REJECT";
        public const string REPORT_MECHANIC_REJECTION = "RPT_MECHANIC_REJECT";
        public const string REPORT_OBD_RESULTS = "RPT_OBD_RESULTS";
        public const string REPORT_FIRST_TEST_RESET = "RPT_FIRST_TEST_RESET";
        public const string REPORT_SAFETY_DEFICIENCY = "RPT_SAFETY_DEF";
        public const string REPORT_CALLS_BY_HOUR = "RPT_CALLS_BY_HOUR";
        public const string REPORT_CALLS_BY_DURATION = "RPT_CALLS_BY_DURATION";
        public const string REPORT_REASONS_RESOLUTION = "RPT_RSN_RES";
        public const string REPORT_SUSPECT_TEST = "RPT_SUSPECT_TEST";
        public const string REPORT_STATION = "RPT_ST_REPORT";
        public const string REPORT_AUDIT_HISTORY_INQUIRY = "RPT_AUDIT_HIST";
        public const string REPORT_RESCHEDULED_AUDITS = "RPT_RESCH_AUDITS";
        public const string REPORT_STICKER_DENIAL = "RPT_STICKER_DENIAL";
        public const string REPORT_STICKER_REGISTRY = "RPT_STICKER_REGSTRY";
        public const string REPORT_QUESTIONABLE_STICKERS = "RPT_QUESTIONABLE_STICKERS";
        public const string REPORT_REDEEM_SOLD_MISMATCH = "RPT_REDEEMSOLD_MISMATCH";
        public const string REPORT_COMPONENT_PERFORMANCE = "RPT_COMP_PERF";
        public const string REPORT_STATION_SUPPORT = "RPT_ST_SUPPORT";
        public const string REPORT_COMPONENT_INVENTORY = "RPT_COMP_INV";
        public const string REPORT_CONSUMABLE_INVENTORY = "RPT_CONSUME_INV";
        public const string REPORT_HISTORY_REPORT = "RPT_HIST_RPT";
        public const string REPORT_CONSUMABLE_SALES = "RPT_CONSUME_SALE_RPT";
        public const string REPORT_REJECTION_BY_MAKE = "REPORT_REJECTION_BY_MAKE";
        public const string REPORT_REJECTION_BY_MODEL = "REPORT_REJECTION_BY_MODEL";
        public const string REPORT_REJECTION_BY_MODELYEAR = "REPORT_REJECTION_BY_MODELYEAR";
        public const string REPORT_REJECTION_BY_COUNTY = "REPORT_REJECTION_BY_COUNTY";
        public const string REPORT_REJECTION_BY_INSPECTOR = "REPORT_REJECTION_BY_INSPECTOR";
        public const string REPORT_REJECTION_BY_STATION = "REPORT_REJECTION_BY_STATION";
        public const string REPORT_MONTHLY_REJECTION = "REPORT_MONTHLY_REJECTION";
        public const string REPORT_NO_FINAL_OUTCOME = "REPORT_NO_FINAL_OUTCOME";

        public const string TRIGGER_EVIN_MISMATCH = "TRIGGER_EVIN_MISMATCH";
        public const string TRIGGER_REJECTION = "TRIGGER_REJECTION";
        public const string TRIGGER_READINESS_MISMATCH = "TRIGGER_READINESS_MISMATCH";
        public const string TRIGGER_TIME_BEFORE_TESTS = "TRIGGER_TIME_BEFORE_TESTS";
        public const string TRIGGER_STATION_SAFETY_DEFECT = "TRIGGER_STATION_SAFETY_DEFECT";
        public const string TRIGGER_NO_VOLTAGE = "TRIGGER_NO_VOLTAGE";
        public const string TRIGGER_STATION_INSPECTOR = "TRIGGER_STATION_INSPECTOR";
        public const string TRIGGER_COMM_PROTOCOL = "TRIGGER_COMM_PROTOCOL";
        public const string TRIGGER_WEIGHTED_SCORE = "TRIGGER_WEIGHTED_SCORE";

        public const string REPORT_INQ_MOBILE_TEST_BY_VIN = "RPT_INQ_MOBILE_TEST_BY_VIN";
        public const string REPORT_INQ_INSPECTION_BY_VIN = "RPT_INQ_INSPECTION_BY_VIN";
        public const string REPORT_INQ_INSPECTION_BY_PLATE = "RPT_INQ_INSPECTION_BY_PLATE";
        public const string REPORT_INQ_INSPECTION_BY_STATION = "RPT_INQ_INSPECTION_BY_STATION";
        public const string REPORT_INQ_INSPECTION_BY_STICKER = "RPT_INQ_INSPECTION_BY_STICKER";

        public const string GRAPH_EMISSION_TEST_REJECT_RATE = "GRPH_EM_REJ_RATE";
        public const string GRAPH_OBD_READINESS_MONITORS = "GRPH_OBD_READINESS";
        public const string GRAPH_OBD_MIL_STATUS = "GRPH_OBD_MIL";
        public const string GRAPH_OBD_COMMUNICATIONS = "GRPH_OBD_COMM";
        public const string GRAPH_OBD_DTC_ERR_CODES = "GRPH_OBD_DTC";
        public const string GRAPH_OBD_PROTOCOL_USAGE = "GRPH_OBD_PROTOCOL";

        public const string USER_ADD_USER = "USR_ADD_USER";
        public const string USER_EDIT_USER = "USR_UPDATE_USER";
        public const string USER_VIEW_USER = "USR_VIEW_USER";
        public const string USER_UPDATE_PASSWORD = "USR_UPDT_PASSWORD";

        public const string ADHOC_BUILDER = "ADHOC_BUILDER";
        public const string SQL_BUILDER = "SQL_EDITOR";

        public const string LOGOUT = "LOGOUT";
    }
}