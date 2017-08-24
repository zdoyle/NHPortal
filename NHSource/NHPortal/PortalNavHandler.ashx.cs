using NHPortal.UserControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NHPortal
{
    /// <summary>Handles navigation menu redirects.</summary>
    public class PortalNavHandler : IHttpHandler, System.Web.SessionState.IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            string targetCode = String.Empty;
            if (context.Request.QueryString["code"] != null)
            {
                targetCode = context.Request.QueryString["code"];
            }

            string targetUrl = String.Empty;
            switch (targetCode.Trim().ToUpper())
            {
                case RedirectCodes.MY_HOME:
                    targetUrl = "Welcome.aspx";
                    break;
                case RedirectCodes.MY_FAVORITES:
                    targetUrl = "MyFavorites.aspx";
                    break;
                case RedirectCodes.DOCUMENTS:
                    targetUrl = "Documents.aspx";
                    break;
                case RedirectCodes.REPORT_MAIN:
                    targetUrl = @"Reports\MainReport.aspx";
                    break;
                case RedirectCodes.REPORT_MODEL_YR_REJECTION:
                    targetUrl = @"Reports\InspectionModelYearRejection.aspx";
                    break;
                case RedirectCodes.REPORT_STATION_REJECTION:
                    targetUrl = @"Reports\InspectionStationRejection.aspx";
                    break;
                case RedirectCodes.REPORT_CNTY_ST_REJECTION:
                    targetUrl = @"Reports\InspectionCountyStationRejection.aspx";
                    break;
                case RedirectCodes.REPORT_MECHANIC_REJECTION:
                    targetUrl = @"Reports\InspectionMechanicRejection.aspx";
                    break;
                case RedirectCodes.REPORT_OBD_RESULTS:
                    targetUrl = @"Reports\InspectionOBDResults.aspx";
                    break;
                case RedirectCodes.REPORT_FIRST_TEST_RESET:
                    targetUrl = @"Reports\InspectionFirstTestReset.aspx";
                    break;
                case RedirectCodes.REPORT_SAFETY_DEFICIENCY:
                    targetUrl = @"Reports\SafetyDeficiency.aspx";
                    break;
                case RedirectCodes.REPORT_CALLS_BY_HOUR:
                    targetUrl = @"Reports\CallsByHour.aspx";
                    break;
                case RedirectCodes.REPORT_CALLS_BY_DURATION:
                    targetUrl = @"Reports\CallsByDuration.aspx";
                    break;
                case RedirectCodes.REPORT_REASONS_RESOLUTION:
                    targetUrl = @"Reports\CallsReasonAndResolution.aspx";
                    break;
                case RedirectCodes.REPORT_SUSPECT_TEST:
                    targetUrl = @"Reports\SuspectTestReport.aspx";
                    break;
                case RedirectCodes.REPORT_STATION:
                    targetUrl = @"Reports\StationReport.aspx";
                    break;
                case RedirectCodes.REPORT_AUDIT_HISTORY_INQUIRY:
                    targetUrl = @"Reports\AuditHistoryInquiry.aspx";
                    break;
                case RedirectCodes.REPORT_RESCHEDULED_AUDITS:
                    targetUrl = @"Reports\RescheduledAudits.aspx";
                    break;
                case RedirectCodes.REPORT_STICKER_DENIAL:
                    targetUrl = @"Reports\StickerDenial.aspx";
                    break;
                case RedirectCodes.REPORT_INQ_INSPECTION_BY_STICKER:
                    targetUrl = @"Inquiries\InqInspectionBySticker.aspx";
                    break;
                case RedirectCodes.REPORT_STICKER_REGISTRY:
                    targetUrl = @"Reports\StickerRegistry.aspx";
                    break;
                case RedirectCodes.REPORT_QUESTIONABLE_STICKERS:
                    targetUrl = @"Reports\StickerQuestionableStickers.aspx";
                    break;
                case RedirectCodes.REPORT_REDEEM_SOLD_MISMATCH:
                    targetUrl = @"Reports\StickerRedeemSoldMisMatch.aspx";
                    break;
                case RedirectCodes.REPORT_COMPONENT_PERFORMANCE:
                    targetUrl = @"Reports\ComponentPerformance.aspx";
                    break;
                case RedirectCodes.REPORT_REJECTION_BY_MAKE:
                    targetUrl = @"Reports\RejectionByMake.aspx";
                    break;
                case RedirectCodes.REPORT_REJECTION_BY_MODEL:
                    targetUrl = @"Reports\RejectionByModel.aspx";
                    break;
                case RedirectCodes.REPORT_REJECTION_BY_MODELYEAR:
                    targetUrl = @"Reports\RejectionByModelYear.aspx";
                    break;
                case RedirectCodes.REPORT_REJECTION_BY_COUNTY:
                    targetUrl = @"Reports\RejectionByCounty.aspx";
                    break;
                case RedirectCodes.REPORT_REJECTION_BY_INSPECTOR:
                    targetUrl = @"Reports\RejectionByInspector.aspx";
                    break;
                case RedirectCodes.REPORT_REJECTION_BY_STATION:
                    targetUrl = @"Reports\RejectionByStation.aspx";
                    break;
                case RedirectCodes.REPORT_MONTHLY_REJECTION:
                    targetUrl = @"Reports\MonthlyRejectionReport.aspx";
                    break;
                case RedirectCodes.REPORT_NO_FINAL_OUTCOME:
                    targetUrl = @"Reports\NoFinalOutcomeReport.aspx";
                    break;
                case RedirectCodes.REPORT_STATION_SUPPORT:
                    targetUrl = @"Reports\StationSupportReport.aspx";
                    break;
                case RedirectCodes.REPORT_COMPONENT_INVENTORY:
                    targetUrl = @"Reports\ComponentInventoryReport.aspx";
                    break;
                case RedirectCodes.REPORT_CONSUMABLE_INVENTORY:
                    targetUrl = @"Reports\ConsumableInventoryReport.aspx";
                    break;
                case RedirectCodes.REPORT_HISTORY_REPORT:
                    targetUrl = @"Reports\HistoryReport.aspx";
                    break;
                case RedirectCodes.REPORT_CONSUMABLE_SALES:
                    targetUrl = @"Reports\ConsumableSalesReport.aspx";
                    break;
                case RedirectCodes.REPORT_INQ_MOBILE_TEST_BY_VIN:
                    targetUrl = @"Inquiries\InqMobileTestByVIN.aspx";
                    break;
                case RedirectCodes.REPORT_INQ_INSPECTION_BY_VIN:
                    targetUrl = @"Inquiries\InqInspectionByVIN.aspx";
                    break;
                case RedirectCodes.REPORT_INQ_INSPECTION_BY_PLATE:
                    targetUrl = @"Inquiries\InqInspectionByPlate.aspx";
                    break;
                case RedirectCodes.REPORT_INQ_INSPECTION_BY_STATION:
                    targetUrl = @"Inquiries\InqInspectionByStation.aspx";
                    break;
                case RedirectCodes.GRAPH_EMISSION_TEST_REJECT_RATE:
                    targetUrl = @"Charts\ChartEmissionTestReject.aspx";
                    break;
                case RedirectCodes.GRAPH_OBD_COMMUNICATIONS:
                    targetUrl = @"Charts\ChartOBDComms.aspx";
                    break;
                case RedirectCodes.GRAPH_OBD_DTC_ERR_CODES:
                    targetUrl = @"Charts\ChartOBDDTCCodes.aspx";
                    break;
                case RedirectCodes.GRAPH_OBD_MIL_STATUS:
                    targetUrl = @"Charts\ChartOBDMILStatus.aspx";
                    break;
                case RedirectCodes.GRAPH_OBD_PROTOCOL_USAGE:
                    targetUrl = @"Charts\ChartOBDProtocol.aspx";
                    break;
                case RedirectCodes.GRAPH_OBD_READINESS_MONITORS:
                    targetUrl = @"Charts\ChartOBDIIReadiness.aspx";
                    break;
                case RedirectCodes.TRIGGER_EVIN_MISMATCH:
                    targetUrl = @"Triggers\TriggerEVINMismatch.aspx";
                    break;
                case RedirectCodes.TRIGGER_COMM_PROTOCOL:
                    targetUrl = @"Triggers\TriggerCommProtocol.aspx";
                    break;
                case RedirectCodes.TRIGGER_REJECTION:
                    targetUrl = @"Triggers\TriggerRejection.aspx";
                    break;
                case RedirectCodes.TRIGGER_READINESS_MISMATCH:
                    targetUrl = @"Triggers\TriggerReadinessMismatch.aspx";
                    break;
                case RedirectCodes.TRIGGER_TIME_BEFORE_TESTS:
                    targetUrl = @"Triggers\TriggerTimeBeforeTests.aspx";
                    break;
                case RedirectCodes.TRIGGER_STATION_SAFETY_DEFECT:
                    targetUrl = @"Triggers\TriggerStationSafetyDefect.aspx";
                    break;
                case RedirectCodes.TRIGGER_NO_VOLTAGE:
                    targetUrl = @"Triggers\TriggerNoVoltage.aspx";
                    break;
                case RedirectCodes.TRIGGER_STATION_INSPECTOR:
                    targetUrl = @"Triggers\TriggerStationInspector.aspx";
                    break;
                case RedirectCodes.TRIGGER_WEIGHTED_SCORE:
                    targetUrl = @"Triggers\TriggerWeightedScore.aspx";
                    break;
                case RedirectCodes.USER_ADD_USER:
                    NHPortal.Classes.SessionHelper.SetUserMaintenanceMode(HttpContext.Current.Session, UserMaintenanceMode.Add);
                    targetUrl = "UserMaintenance.aspx";
                    break;
                case RedirectCodes.USER_EDIT_USER:
                    NHPortal.Classes.SessionHelper.SetUserMaintenanceMode(HttpContext.Current.Session, UserMaintenanceMode.Modify);
                    targetUrl = "UserMaintenance.aspx";
                    break;
                case RedirectCodes.USER_VIEW_USER:
                    NHPortal.Classes.SessionHelper.SetUserMaintenanceMode(HttpContext.Current.Session, UserMaintenanceMode.View);
                    targetUrl = "UserMaintenance.aspx";
                    break;
                case RedirectCodes.USER_UPDATE_PASSWORD:
                    targetUrl = "ChangePassword.aspx";
                    break;
                case RedirectCodes.ADHOC_BUILDER:
                    targetUrl = "AdhocBuilder.aspx";
                    break;
                case RedirectCodes.SQL_BUILDER:
                    targetUrl = "SQLEditor.aspx";
                    break;
                case RedirectCodes.LOGOUT:
                    targetUrl = "Logout.aspx";
                    break;
            }

            if (!String.IsNullOrEmpty(targetUrl))
            {
                context.Response.Redirect("~/" + targetUrl);
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}