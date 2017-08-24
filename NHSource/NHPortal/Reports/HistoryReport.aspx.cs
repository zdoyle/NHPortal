using GDCoreUtilities.Logging;
using NHPortal.Classes;
using NHPortal.Classes.User;
using NHPortal.MasterPages;
using PortalFramework.ReportModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NHPortal.Reports
{
    public partial class HistoryReport : NHPortal.Classes.PortalPage
    {
        private string m_inputId;
        private string m_stationId;
        

        protected void Page_Load(object sender, EventArgs e)
        {
            RedirectOnInvalidPermission(UserPermissions.HistoryReport);

            if (IsPostBack)
            {
                Page.Validate( /*Control Validation Group Name Optional*/ );
                if (Page.IsValid)
                {
                    ProcessPostBack();
                }
            }
            else
            {
                InitializePage();
                lstInquiryType.Items.RemoveAt(0);
                lstInquiryType.SelectedIndex = 0;
                // set to start of month for month to date report
                dateSelector.StartDateControl.Text = DateTime.Now.ToString("M/1/yyyy");
                LoadFavorite();
                Master.SetButtonVisibility(ReportButton.Run, false);
            }
            InitializeInputs();

        }
        private void InitializeInputs()
        {
            string onchange = String.Format("javascript: inquiryChange(this, '{0}'); changeValue();", tbInquiry.ClientID);
            lstInquiryType.Attributes.Add("onchange", onchange);
            string onload = String.Format("javascript: inquiryChange(this, '{0}');", tbInquiry.ClientID);
            lstInquiryType.Attributes.Add("onload", onload);
            if (lstInquiryType.SelectedValue == "S")
            {
                tbInquiry.MaxLength = 8;
            }
            else
            {
                tbInquiry.MaxLength = 10;
            }
        }
        private void InitializePage()
        {
            Master.SetHeaderText("History Report");
            lstInquiryType.Initialize();
        }
        private void ProcessPostBack()
        {
            string action = Master.MasterPage.HidActionValue;

            if (lstInquiryType.SelectedValue.ToString()[0] == 'S')
            {
                m_inputId = NHPortalUtilities.ToStationID(tbInquiry.Text);
            }
            else if (lstInquiryType.SelectedValue.ToString()[0] == 'O')
            {
                m_inputId = NHPortalUtilities.ToOfficerID(tbInquiry.Text);
            }
            else
            {
                m_inputId = tbInquiry.Text.Trim().ToUpper();
            }
            switch (action)
            {
                //support history buttons 
                case "SUPPORTHISTORY":
                    RunQuerySupportSummary();
                    break;
                case "SUPPORTHISTORYDETAILS":
                    RunQuerySupportSummaryfromMech();
                    break;
                case "NOTES":
                    RunNotes();
                    break;
                case "CONSUMABLE":
                    RunConsumable();
                   
                    break;
                case "COMPONENT":
                    RunComponent();

                    break;
                case "MECHANIC":
                    RunMechanic();
                    break;
                //call history buttons 
                case "CALLHISTORY":
                    RunReportCall();
                    break;
                //billing history buttons
                case "BILLINGHISTORY":
                    RunQueryBillingSummary();
                    break;
                case "BILLINGHISTORYDETAILS":
                    RunReportBillingDetails();
                    break;
                //case "NOTESBILLING":
                //    RunNotesBilling();
                //    break;
                //other buttons 
                case "SAVE_FAVORITE":
                    SaveFavorite();
                    break;
            }
        }
        private void SaveFavorite()
        {
            Master.SaveFavorite(NHPortal.UserControls.RedirectCodes.REPORT_HISTORY_REPORT, UserFavoriteTypes.Report);
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

                        case "ID/OLN":
                            tbInquiry.Text = c.Value;
                            break;
                        case "INQUIRY TYPE":
                            lstInquiryType.SelectedValue = c.Value;
                            break;
                    }
                }
            }
        }

        private void SetMetaData()
        {
            if (Master.UserReport != null)
            {
                Master.UserReport.MetaData.Add("Start Date", dateSelector.StartDateControl.Text);
                Master.UserReport.MetaData.Add("End Date", dateSelector.EndDateControl.Text);
                Master.UserReport.MetaData.Add("Inquiry Type", lstInquiryType.SelectedItem.Text,
                         lstInquiryType.SelectedValue);
                Master.UserReport.MetaData.Add("ID/OLN", m_inputId);
            }
        }
        private void SetColumnTypes()
        {
            for (int i = 1; i < Master.UserReport.ColumnCount; i++)
            {
                Master.UserReport.Columns[i].ColumnDataType = ColumnDataType.Number;
            }
        }
        private void RunReportSupport()
        {
            string qry = BuildQuerySupportContact();
            GDDatabaseClient.Oracle.OracleResponse response = PortalFramework.Database.ODAP.GetDataTable(qry, PortalFramework.Database.DatabaseTarget.Adhoc);
            LogMessage("Attempting to run query for Support History Contact information...", LogSeverity.Trace);

            if (response.Successful)
            {
                LogMessage("Query for Support History Contact information is successful", LogSeverity.Trace);
                if ((lstInquiryType.SelectedValue.ToString() == "R"))
                {
                    var table = MechanicInfoToReportTable(response.ResultsTable);
                    Master.UserReport.Tables.Add(table);
                }
                else
                {
                    var table = StationInfoToReportTable(response.ResultsTable);
                    Master.UserReport.Tables.Add(table);
                }
            }
            else
            {
                Master.UserReport = null;
                Master.SetError(response.Exception);
                LogMessage("Query error for Support History Contact information: " + response.Exception, LogSeverity.Error);
            }
            LogMessage("Query for Support History Contact information Finshed", LogSeverity.Trace);
            LogOracleResponse(response);
            LogMessage("Support History Contact information Loaded", LogSeverity.Trace);

        }
        //runs support history summary
        private void RunQuerySupportSummary()
        {
            string qrysumm = BuildQuerySupportSummary();
            GDDatabaseClient.Oracle.OracleResponse response2 = PortalFramework.Database.ODAP.GetDataTable(qrysumm, PortalFramework.Database.DatabaseTarget.Adhoc);
            LogMessage("Attempting to run query for Support History Summary information...", LogSeverity.Trace);

            SetMetaData();
            if (response2.Successful)
            {
                LogMessage("Query for Support History Summary information is successful", LogSeverity.Trace);
                if ((lstInquiryType.SelectedValue.ToString() == "R"))
                {
                    Master.UserReport = new Report("Support History for Mechanic " + m_inputId, response2.ResultsTable);
                }
                else
                {
                    Master.UserReport = new Report("Support History for Station " + m_inputId, response2.ResultsTable);
                }
                SetMetaData();
                Master.UserReport.FooterVisibility = FooterVisibility.Auto;
                if (response2.HasResults)
                {
                    LogMessage("Query for Support History Summary information has results", LogSeverity.Trace);
                    ConvertReportColumns();
                    LogMessage("Support History Summary information Data rendering to page", LogSeverity.Trace);
                    RunReportSupport();
                }
            }
            else
            {
                Master.UserReport = null;
                Master.SetError(response2.Exception);
                LogMessage("Query error for Support History Summary information: " + response2.Exception, LogSeverity.Error);
            }
            LogMessage("Query Finshed for Support History Summary information", LogSeverity.Trace);
            LogOracleResponse(response2);
            Master.RenderReportToPage();
            SessionHelper.SetCurrentReport(this.Session, Master.UserReport);
            LogMessage("Support History Summary information Loaded", LogSeverity.Trace);
        }
        //runs support history summary
        private void RunQuerySupportSummaryfromMech()
        {
            string qrysumm = BuildQuerySupportSummaryForMech();
            GDDatabaseClient.Oracle.OracleResponse response2 = PortalFramework.Database.ODAP.GetDataTable(qrysumm, PortalFramework.Database.DatabaseTarget.Adhoc);
            LogMessage("Attempting to run query for Support History Summary information...", LogSeverity.Trace);

            if (response2.Successful)
            {
                LogMessage("Query for Support History Summary information is successful", LogSeverity.Trace);

                if (response2.HasResults)
                {
                    Master.UserReport.Tables.Remove(1);

                    var table = MechanicStationDetailsInfoToReportTable(response2.ResultsTable);
                    Master.UserReport.Tables.Add(table);
                }
                else
                {
                   // Master.UserReport = null;
                   // Master.SetError(response2.Exception);
                   Master.UserReport.Tables.Remove(1);
                }
                
            }
            else
            {
                Master.UserReport = null;
                Master.UserReport.Tables.Remove(1);
                 Master.SetError(response2.Exception);
                LogMessage("Query error for Support History Summary information: " + response2.Exception, LogSeverity.Error);
            }
            LogMessage("Query Finshed for Support History Summary information", LogSeverity.Trace);
            LogOracleResponse(response2);
            Master.RenderReportToPage();
            SessionHelper.SetCurrentReport(this.Session, Master.UserReport);
            LogMessage("Support History Summary information Loaded", LogSeverity.Trace);
        }
        //Support History if Station ID is selected
        private string BuildQuerySupportContact()
        {
            StringBuilder sb = new StringBuilder();
            if (lstInquiryType.SelectedValue.ToString() == "S")
            {
                sb.AppendLine(" SELECT S.NAME as CompanyName, ");
                sb.AppendLine(" S.ADDRESS as Address,");
                sb.AppendLine(" S.CITY as City, ");
                sb.AppendLine(" S.PHONE as Phone, ");
                sb.AppendLine(" S.CONTACTNAME as ContactName, ");
                sb.AppendLine(" gd.DESCRIPTION as Description, ");
                sb.AppendLine("  ( ");
                sb.AppendLine(" SELECT DISABLEDPAYMENT + DISABLEDENFORCE + DISABLEDOUTOFBUSINESS ");
                sb.AppendLine("    FROM ");
                sb.AppendLine("     ( ");
                sb.AppendLine("     SELECT ");
                sb.AppendLine("     CASE MAX(DISABLEDPAYMENT) WHEN '0' THEN 0 ELSE 1 END DISABLEDPAYMENT, ");
                sb.AppendLine("     CASE MAX(DISABLEDENFORCE) WHEN '0' THEN 0 ELSE 2 END DISABLEDENFORCE, ");
                sb.AppendLine("     CASE MAX(DISABLEDOUTOFBUSINESS) WHEN '0' THEN 0 ELSE 4 END DISABLEDOUTOFBUSINESS ");
                sb.AppendLine("     FROM UNIT U ");
                sb.AppendLine("     WHERE U.STATIONID = '" + m_inputId + "' ");
                sb.AppendLine("     ) ");
                sb.AppendLine("  )  DISABLED ");
                sb.AppendLine(" FROM STATION S ");
                sb.AppendLine(" INNER JOIN R_GD_STATUS gd ON gd.code_value = s.gdstatus");
                sb.AppendLine(" WHERE S.STATIONID ='" + m_inputId + "' ");
            }
            if (lstInquiryType.SelectedValue.ToString() == "R")
            {
                sb.AppendLine(" SELECT ");
                sb.AppendLine(" NAME AS \"Mechanic Name\", ");
                sb.AppendLine(" SECURITYLEVEL AS \"Security Level\", ");
                sb.AppendLine(" CERTLEVEL AS \"Cert Level\", ");
                sb.AppendLine(" CITY AS \"Cert #\", ");
                sb.AppendLine(" CURRENTSTATUS AS \"Status\", ");
                sb.AppendLine(" CBTPASSED AS \"CBT\" ");
                sb.AppendLine(" FROM INSPECTOR ");
                sb.AppendLine(" WHERE INSPECTORID = '" + m_inputId + "' ");
            }
            return sb.ToString();
        }
        private string BuildQuerySupportSummary()
        {
            string activitynum = hidSelectedIndex.Value;
            string activity = activitynum;

            StringBuilder sb = new StringBuilder();
            if (lstInquiryType.SelectedValue.ToString() == "S")
            {
                sb.AppendLine(" SELECT SA.ACTIVITYNUMBER || '-' || SA.ACTIVITYATTEMPT AS \"Activity Number\", ");
                sb.AppendLine(" SA.DATECALLINITIATED as \"Opened\", ");
                sb.AppendLine(" SA.DATECLOSED AS \"Closed\", ");
                sb.AppendLine(" SA.ORIGINATORID AS \"Mech ID\", ");
                sb.AppendLine(" SA.UNITID AS \"Unit ID\", ");
                sb.AppendLine(" SA.SUPPORTTYPE as \"Support Type\", ");
                sb.AppendLine(" SA.SUPPORTMETHOD as \"Support Method\", ");
                sb.AppendLine(" NULL AS \"Consumable\", ");
                sb.AppendLine(" (SELECT COUNT(*) FROM ACTIVITYDETAIL AD WHERE AD.ACTIVITYNUMBER = SA.ACTIVITYNUMBER AND AD.ACTIVITYATTEMPT = SA.ACTIVITYATTEMPT)AS \"Component\",  ");
                sb.AppendLine("NOTES as \"Notes\",");
                sb.AppendLine("NOTES as \"Notes_Exports\"");
                sb.AppendLine(" FROM SUPPORTACTIVITY SA JOIN UNIT U ON SA.UNITID = U.UNITID ");
                sb.AppendLine(" WHERE U.STATIONID ='" + m_inputId + "' ");
                if (dateSelector.StartDateControl.GetDateText().ToString() != null && dateSelector.EndDateControl.GetDateText() != null)
                {
                    sb.AppendLine(" AND SA.DATECALLINITIATED BETWEEN '" + dateSelector.StartDateControl.GetDateText() + "' AND '" + dateSelector.EndDateControl.GetDateText() + "' ");
                }
                sb.AppendLine(" ORDER BY SA.DATECALLINITIATED ");
            }
            if (lstInquiryType.SelectedValue.ToString() == "R")
            {
                sb.AppendLine(" SELECT SI1.STATIONID as \"Station ID\", ");
                sb.AppendLine(" (  ");
                sb.AppendLine(" SELECT COUNT(*) FROM NEW_TESTRECORD TR  ");
                sb.AppendLine(" WHERE INSPECTORID = '" + m_inputId + "'  ");
                sb.AppendLine(" AND TESTDATE BETWEEN '" + dateSelector.StartDateControl.GetDateText() + "' AND '" + dateSelector.EndDateControl.GetDateText() + "'  ");
                sb.AppendLine(" AND SI1.STATIONID = TR.STATIONID  ");
                sb.AppendLine(" ) AS \"Inspections\"  ");
                sb.AppendLine(" FROM STATINSPCOMBOHIST SI1  ");
                sb.AppendLine(" LEFT JOIN STATINSPCOMBOHIST SI2 ON SI1.STATIONID = SI2.STATIONID AND SI1.INSPECTORID = SI2.INSPECTORID AND SI2.ACTION = 'D' ");
                sb.AppendLine(" WHERE SI1.INSPECTORID ='" + m_inputId + "' ");
                sb.AppendLine(" AND SI1.ACTION = 'A'  ");
                sb.AppendLine(" AND SI1.ALTERDATE <= '" + dateSelector.EndDateControl.GetDateText() + "' ");
                sb.AppendLine(" AND (SI2.ALTERDATE >= '" + dateSelector.StartDateControl.GetDateText() + "' OR SI2.ALTERDATE IS NULL) ");
                sb.AppendLine(" GROUP BY SI1.STATIONID ");
            }
            return sb.ToString();
        }

        private string BuildQuerySupportSummaryForMech()
        {
            string stationid = hidSelectedIndex.Value;
            string station = stationid;

            StringBuilder sb = new StringBuilder();

            sb.AppendLine(" SELECT SA.ACTIVITYNUMBER || '-' || SA.ACTIVITYATTEMPT AS \"Activity Number\", ");
            sb.AppendLine(" SA.DATECALLINITIATED as \"Opened\", ");
            sb.AppendLine(" SA.DATECLOSED AS \"Closed\", ");
            sb.AppendLine(" SA.ORIGINATORID AS \"Mech ID\", ");
            sb.AppendLine(" SA.UNITID AS \"Unit ID\", ");
            sb.AppendLine(" SA.SUPPORTTYPE as \"Support Type\", ");
            sb.AppendLine(" SA.SUPPORTMETHOD as \"Support Method\", ");
            sb.AppendLine(" NULL AS \"Consumable\", ");
            sb.AppendLine(" (SELECT COUNT(*) FROM ACTIVITYDETAIL AD WHERE AD.ACTIVITYNUMBER = SA.ACTIVITYNUMBER AND AD.ACTIVITYATTEMPT = SA.ACTIVITYATTEMPT)AS \"Component\",  ");
            sb.AppendLine("NOTES as \"Notes\"");
            sb.AppendLine(" FROM SUPPORTACTIVITY SA JOIN UNIT U ON SA.UNITID = U.UNITID ");
            sb.AppendLine(" WHERE U.STATIONID ='" + station + "' ");
            sb.AppendLine("AND SA.ORIGINATORID ='" + m_inputId + "'");
            if (dateSelector.StartDateControl.GetDateText().ToString() != null && dateSelector.EndDateControl.GetDateText() != null)
            {
                sb.AppendLine(" AND SA.DATECALLINITIATED BETWEEN '" + dateSelector.StartDateControl.GetDateText() + "' AND '" + dateSelector.EndDateControl.GetDateText() + "' ");
            }
            sb.AppendLine(" ORDER BY SA.DATECALLINITIATED ");
            return sb.ToString();
        }
        //sql for mechanic
        private string BuildMechanic()
        {
            string activitynum = hidSelectedIndex.Value;
            string activity = activitynum;
            string stationId = activity.Substring(activity.IndexOf(",") + 1);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(" SELECT '" + stationId + "' as stationid, '" + m_inputId + "'  as INSPECTORID,  COUNT(*) TOTAL_TESTS, SUSPECT_COUNT.TOTAL_SUSPECT, '%' ");
            sb.AppendLine(" FROM NEW_TESTRECORD TR, ");
            sb.AppendLine(" ( ");
            sb.AppendLine("     SELECT COUNT(*) AS TOTAL_SUSPECT ");
            sb.AppendLine("     FROM ");
            sb.AppendLine("     ( ");
            sb.AppendLine("       SELECT * FROM CANDIDATE_OBDVIN_MISMATCH EVIN ");
            sb.AppendLine("       UNION SELECT * FROM CANDIDATE_OBDPROTOCOL_MISMATCH ");
            sb.AppendLine("       UNION SELECT * FROM CANDIDATE_READINESS_MISMATCH ");
            sb.AppendLine("     ) ");
            sb.AppendLine("     WHERE  stationid  = '" + stationId + "' AND TESTDATE >= '" + dateSelector.StartDateControl.GetDateText() + "' AND TESTDATE <= '" + dateSelector.EndDateControl.GetDateText() + "' ");
            sb.AppendLine(" ) SUSPECT_COUNT ");
            sb.AppendLine(" WHERE  stationid  = '" + stationId + "' AND TESTDATE >= '" + dateSelector.StartDateControl.GetDateText() + "' AND TESTDATE <= '" + dateSelector.EndDateControl.GetDateText() + "' ");
            sb.AppendLine(" GROUP BY TOTAL_SUSPECT, stationid ");

            return sb.ToString();
        }
        //station
        private string BuildComponentSQL()
        {
            string activitynum = hidSelectedIndex.Value;
            string activity = activitynum;

            string[] parts = activity.Split('-');

            StringBuilder builder = new StringBuilder();

            builder.AppendLine(" SELECT ");
            builder.AppendLine("     A.TYPEID as \"Component\", ");
            builder.AppendLine("     MAX(FROM_NO) AS OLD_SERIAL, ");
            builder.AppendLine("     MAX(TO_NO) AS NEW_SERIAL, ");
            builder.AppendLine("     A.SHIPTRACKINGNUM as SHIPTRACKINGNUM, ");
            builder.AppendLine("     A.RETURNTRACKINGNUM as RETURNTRACKINGNUM, ");
            builder.AppendLine("     A.ACTIVITYNUMBER || '-' || A.ACTIVITYATTEMPT AS ACTIVITYNUM ");
            builder.AppendLine(" FROM ");
            builder.AppendLine("     ( ");
            builder.AppendLine("         SELECT ");
            builder.AppendLine("             SA.UNITID, ");
            builder.AppendLine("             SA.SHIPTRACKINGNUM, ");
            builder.AppendLine("             SA.RETURNTRACKINGNUM, ");
            builder.AppendLine("  SA.ACTIVITYNUMBER, ");
            builder.AppendLine("  SA.ACTIVITYATTEMPT, ");
            builder.AppendLine("             I.MAKEMODELID, ");
            builder.AppendLine("             M.TYPEID, ");
            builder.AppendLine("             AD.INVENTORYNUMBER, ");
            builder.AppendLine("             AD.FROMID, ");
            builder.AppendLine("             AD.TOID, ");
            builder.AppendLine("             CASE WHEN (FROMID = SA.UNITID) THEN AD.INVENTORYNUMBER ELSE NULL END AS FROM_NO, ");
            builder.AppendLine("             CASE WHEN (TOID = SA.UNITID) THEN AD.INVENTORYNUMBER  ELSE NULL END AS TO_NO ");
            builder.AppendLine("         FROM ");
            builder.AppendLine("             SUPPORTACTIVITY SA ");
            builder.AppendLine("             JOIN ACTIVITYDETAIL AD ON (AD.ACTIVITYNUMBER = SA.ACTIVITYNUMBER) ");
            builder.AppendLine("                                       AND (AD.ACTIVITYATTEMPT = SA.ACTIVITYATTEMPT) ");
            builder.AppendLine("                                       AND (AD.TYPEID='00000000') ");
            builder.AppendLine("             JOIN INVENTORY I ON (I.INVENTORYNUMBER = AD.INVENTORYNUMBER) ");
            builder.AppendLine("             JOIN MANUFACTURER M ON (M.MAKEMODELID = I.MAKEMODELID) ");
            builder.AppendLine("         WHERE ");
            builder.AppendLine("             SA.ACTIVITYNUMBER = '" + parts[0] + "' AND SA.ACTIVITYATTEMPT='" + parts[1] + "' ");
            builder.AppendLine("     ) A ");
            builder.AppendLine(" GROUP BY ");
            builder.AppendLine("     A.TYPEID, ");
            builder.AppendLine("     A.SHIPTRACKINGNUM, ");
            builder.AppendLine("     A.RETURNTRACKINGNUM ,");
            builder.AppendLine("A.ACTIVITYNUMBER, ");
            builder.AppendLine("A.ACTIVITYATTEMPT");

            return builder.ToString();
        }
        //station
        private string BuildConsumableSQL()
        {
            string activitynum = hidSelectedIndex.Value;
            string activity = activitynum;

            string[] parts = activity.Split('-');

            StringBuilder builder = new StringBuilder();

            builder.AppendLine(" SELECT ");
            builder.AppendLine(" TI.DESCRIPTION, ");
            builder.AppendLine(" AD.QUANTITY, ");
            builder.AppendLine(" SA.SHIPTRACKINGNUM, ");
            builder.AppendLine(" SA.ACTIVITYNUMBER || '-' || SA.ACTIVITYATTEMPT AS ACTIVITYNUM ");
            builder.AppendLine(" FROM ACTIVITYDETAIL AD ");
            builder.AppendLine(" JOIN SUPPORTACTIVITY SA ON SA.ACTIVITYNUMBER = AD.ACTIVITYNUMBER AND SA.ACTIVITYATTEMPT = AD.ACTIVITYATTEMPT ");
            builder.AppendLine(" LEFT JOIN TYPEID TI ON AD.TYPEID = TI.TYPEID ");
            builder.AppendLine(" WHERE AD.ACTIVITYNUMBER = '" + parts[0] + "' ");
            builder.AppendLine(" AND AD.ACTIVITYATTEMPT = '" + parts[1] + "' ");
            return builder.ToString();
        }
        private string BuildNotesSQL()
        {
            string activitynum = hidSelectedIndex.Value;
            string activity = activitynum;

            string[] parts = activity.Split('-');

            StringBuilder builder = new StringBuilder();
            builder.AppendLine(" SELECT ");
            builder.AppendLine(" SA.ACTIVITYNUMBER || '-' || SA.ACTIVITYATTEMPT AS ACTIVITYNUM, ");
            builder.AppendLine("NOTES");
            builder.AppendLine(" FROM SUPPORTACTIVITY SA JOIN UNIT U ON SA.UNITID = U.UNITID ");
            builder.AppendLine(" WHERE U.STATIONID ='" + m_inputId + "' ");
            builder.AppendLine("AND SA.ACTIVITYNUMBER = '" + parts[0] + "' ");
            builder.AppendLine(" AND SA.ACTIVITYATTEMPT = '" + parts[1] + "' ");

            if (dateSelector.StartDateControl.GetDateText().ToString() != null && dateSelector.EndDateControl.GetDateText().ToString() != null)
            {
                builder.AppendLine(" AND SA.DATECALLINITIATED BETWEEN '" + dateSelector.StartDateControl.GetDateText() + "' AND '" + dateSelector.EndDateControl.GetDateText() + "' ");
            }
            builder.AppendLine(" ORDER BY SA.DATECALLINITIATED ");

            return builder.ToString();

        }
        private void RunNotes()
        {
            string qrysumm3 = BuildNotesSQL();
            GDDatabaseClient.Oracle.OracleResponse response3 = PortalFramework.Database.ODAP.GetDataRow(qrysumm3, PortalFramework.Database.DatabaseTarget.Adhoc);
            if (response3.HasResults)
            {
                LogMessage("Query for Support History Notes has results", LogSeverity.Trace);
                //Master.UserReport.Tables.Remove(1);
                ltrlPopUpRpt.Text = PopUp(response3.ResultsRow);
                hidShowOverlay.Value = "TRUE";
                LogMessage("Support History Notes Data rendering to page", LogSeverity.Trace);
            }
            else
            {
                LogMessage("Query error for Support History Notes: " + response3.Exception, LogSeverity.Error);
            }
            LogMessage("Support History Notes Query Finshed", LogSeverity.Trace);
            LogOracleResponse(response3);
            Master.RenderReportToPage();
            LogMessage("Support History Notes Loaded", LogSeverity.Trace);

        }
        //this runs if Consumable is YES in Support History Summary table
        private void RunConsumable()
        {
            string qrysumm3 = BuildConsumableSQL();
            GDDatabaseClient.Oracle.OracleResponse response3 = PortalFramework.Database.ODAP.GetDataTable(qrysumm3, PortalFramework.Database.DatabaseTarget.Adhoc);
            LogMessage("Attempting to run query for Consumable YES...", LogSeverity.Trace);

            if (response3.HasResults)
            {
                LogMessage("Query for Consumable YES has results", LogSeverity.Trace);
                Master.UserReport.Tables.Remove(1);
                var table = ConsumableInfoToReportTable(response3.ResultsTable);
                Master.UserReport.Tables.Add(table);
                
                LogMessage("Consumable YES Data rendering to page", LogSeverity.Trace);
            }
            else
            {
                LogMessage("Consumable No Data rendering to page", LogSeverity.Error);
            }
            LogMessage("Query for Consumable YES Finshed", LogSeverity.Trace);
            LogOracleResponse(response3);
            Master.RenderReportToPage();
            SessionHelper.SetCurrentReport(this.Session, Master.UserReport);           
            LogMessage("Consumable YES Loaded", LogSeverity.Trace);
        }
        //this runs if Component is YES in Support History Summary table
        private void RunComponent()
        {
            string qrysumm3 = BuildComponentSQL();
            GDDatabaseClient.Oracle.OracleResponse response3 = PortalFramework.Database.ODAP.GetDataTable(qrysumm3, PortalFramework.Database.DatabaseTarget.Adhoc);
            LogMessage("Attempting to run query for Component YES...", LogSeverity.Trace);
            if (response3.Successful)
            {
                LogMessage("Query for Component YES has results", LogSeverity.Trace);
                Master.UserReport.Tables.Remove(1);
                var table = ComponentInfoToReportTable(response3.ResultsTable);
                Master.UserReport.Tables.Add(table);
               
                LogMessage("Component YES Data rendering to page", LogSeverity.Trace);
            }
            else
            {
                LogMessage("Query error for Component YES: " + response3.Exception, LogSeverity.Error);
            }
            LogMessage("Query for Component YES Finshed", LogSeverity.Trace);
            LogOracleResponse(response3);
            Master.RenderReportToPage();
            LogMessage("Component YES Loaded", LogSeverity.Trace);
        }
        //this runs when user selects mechanic OLN as inquiry type and selects inspections YES
        private void RunMechanic()
        {
            string qrysumm = BuildMechanic();
            GDDatabaseClient.Oracle.OracleResponse response2 = PortalFramework.Database.ODAP.GetDataTable(qrysumm, PortalFramework.Database.DatabaseTarget.Adhoc);
            LogMessage("Attempting to run Mechanic query...", LogSeverity.Trace);
            if (response2.Successful && response2.HasResults)
            {
                LogMessage("Mechanic Query is successful and has results", LogSeverity.Trace);
                Master.UserReport.Tables.Remove(1);
                var table = MechanicDetailsInfoToReportTable(response2.ResultsTable);
                Master.UserReport.Tables.Add(table);
                LogMessage("Mechanic Data rendering to page", LogSeverity.Trace);
            }
            else
            {
                Master.UserReport = null;
                Master.SetError(response2.Exception);
                LogMessage("Query error for Mechanic: " + response2.Exception, LogSeverity.Error);
            }
            LogOracleResponse(response2);
            Master.RenderReportToPage();
            LogMessage("Mechanic Query Finshed", LogSeverity.Trace);
        }
        //Click Station -> Notes -> Pop up table with Notes Data
        private string PopUp(DataRow row)
        {
            string html = "<div class=\"details\" style=\"visibility:visible\" runat=\"server\">";
            html += "<table class=\"expandedDetails\">";
            html += "<tr><td style=\"text-align:center;\" class=\"table_header\"><h2>" + row["ACTIVITYNUM"].ToString() + " NOTES </h2></td></tr>";
            html += "<tr><td>";
            html += row["NOTES"].ToString();
            html += "</td></tr>";
            html += "</table>";
            html += "</div>";
            return html;
        }
        //Click Station -> Consumable -> Pop up table with Consumable Data
        private ReportTable ConsumableInfoToReportTable(DataTable dt)
        {
            string activitynum = hidSelectedIndex.Value;
            string activity = activitynum;

            ReportTable tbl = new ReportTable();
            tbl.RenderLocation = RenderLocation.Above;
            tbl.Title = activity + " CONSUMABLE";
            tbl.HeaderNote = activity + " CONSUMABLE";
            tbl.Columns.Add("DESCRIPTION", "Consumable Item", ColumnDataType.String);
            tbl.Columns.Add("QUANTITY", "Quantity", ColumnDataType.Number);
            tbl.Columns.Add("SHIPTRACKINGNUM", "Tracking Number", ColumnDataType.String);

            foreach (DataRow rows in dt.Rows)
            {
                ReportRow rptRow = new ReportRow(tbl);
                rptRow.Cells.Add(rows["DESCRIPTION"].ToString().Trim());
                rptRow.Cells.Add(rows["QUANTITY"].ToString().Trim());
                rptRow.Cells.Add(rows["SHIPTRACKINGNUM"].ToString().Trim());
                tbl.Rows.Add(rptRow);
            }
            return tbl;
        }
        //Click Station -> Component -> Pop up table with Component Data
        private ReportTable ComponentInfoToReportTable(DataTable dt)
        {
            string activitynum = hidSelectedIndex.Value;         

            ReportTable tbl = new ReportTable();
            tbl.RenderLocation = RenderLocation.Above;
            tbl.HeaderNote = activitynum + " COMPONENT";
            tbl.Title = activitynum + " COMPONENT";
            tbl.Columns.Add("Component", "Component", ColumnDataType.String);
            tbl.Columns.Add("OLD_SERIAL", "Old Serial", ColumnDataType.String);
            tbl.Columns.Add("NEW_SERIAL", "New Serial", ColumnDataType.String);
            tbl.Columns.Add("SHIPTRACKINGNUM", "Tracking Number", ColumnDataType.String);
            tbl.Columns.Add("RETURNTRACKINGNUM", "Return Tracking", ColumnDataType.String);

            foreach (DataRow rows in dt.Rows)
            {
                ReportRow rptRow = new ReportRow(tbl);
                rptRow.Cells.Add(rows["Component"].ToString().Trim());
                rptRow.Cells.Add(rows["OLD_SERIAL"].ToString().Trim());
                rptRow.Cells.Add(rows["NEW_SERIAL"].ToString().Trim());
                rptRow.Cells.Add(rows["SHIPTRACKINGNUM"].ToString().Trim());
                rptRow.Cells.Add(rows["RETURNTRACKINGNUM"].ToString().Trim());
                tbl.Rows.Add(rptRow);
            }
            return tbl;
        }
        //Station Name and Information
        private void ConvertReportColumns()
        {
            if (Master.UserReport != null)
            {
                foreach (ReportRow row in Master.UserReport.Rows)
                {
                    try
                    {
                        if ((lstInquiryType.SelectedValue.ToString() == "R"))
                        {
                            string inspectorid = m_inputId;
                            m_stationId = row["Station ID"].Value;
                            if (row["Inspections"].Value != "0")
                            {
                                row["Inspections"].Value = "YES";
                                row["Inspections"].OnClick = "javascript:RunMechanic('" + inspectorid + "," + m_stationId + "')";
                                row["Inspections"].Href = "#/";
                            }
                            else
                            {
                                row["Inspections"].Value = "No";
                            }
                            if (!String.IsNullOrEmpty(row["Station ID"].ToString()))
                            {
                                row["Station ID"].OnClick = "javascript:RunReportSupportForMech('" + m_stationId + "')";
                                row["Station ID"].Href = "#/" + m_stationId + "_Mechanic";
                            }
                        }
                        else
                        {
                            string strOpenDT, strClosedDT, x;
                            strOpenDT = row["Opened"].ToString().Trim();
                            strClosedDT = row["Closed"].ToString().Trim();
                            if (NHPortal.Classes.NHPortalUtilities.TryConvertDateTimeFormat(strOpenDT, "yyyyMMdd", "M/d/yyyy", out x))
                            {
                                string msg = String.Format("Converted {0} to {1}", strOpenDT, x);
                                System.Diagnostics.Debug.WriteLine(msg);
                                row["OPENED"].Value = x;
                            }
                            if (NHPortal.Classes.NHPortalUtilities.TryConvertDateTimeFormat(strClosedDT, "yyyyMMdd", "M/d/yyyy", out x))
                            {
                                row["CLOSED"].Value = x;
                            }
                            row["Support Type"].Value = Classes.Reference.SupportTypes.GetDescription(row["Support Type"].ToString().Trim()[0]);
                            if (!String.IsNullOrEmpty(row["SUPPORT METHOD"].ToString()))
                            {
                                String strSUPPORTMETHOD = row["SUPPORT METHOD"].ToString();
                                switch (strSUPPORTMETHOD)
                                {
                                    case "A":
                                        strSUPPORTMETHOD = "No - Ship";
                                        break;
                                    case "O":
                                        strSUPPORTMETHOD = "On-Site";
                                        break;
                                    case "S":
                                        strSUPPORTMETHOD = "Ship";
                                        break;
                                    case "B":
                                        strSUPPORTMETHOD = "No - Pend";
                                        break;
                                    case "D":
                                        strSUPPORTMETHOD = "Depot";
                                        break;
                                    case "N":
                                        strSUPPORTMETHOD = "None";
                                        break;
                                    case "P":
                                        strSUPPORTMETHOD = "Pending";
                                        break;
                                    case "C":
                                        strSUPPORTMETHOD = "Callback";
                                        break;
                                    default:
                                        strSUPPORTMETHOD = "UNKNOWN";
                                        break;
                                }
                                row["SUPPORT METHOD"].Value = strSUPPORTMETHOD.ToString().Trim();
                            }
                            string activityNum = row["Activity Number"].ToString();
                            //consumable
                            if (row["SUPPORT TYPE"].ToString() == "Sales" && row["SUPPORT METHOD"].ToString() == "Ship")
                            {
                                row["Consumable"].Value = "YES";
                                row["Consumable"].OnClick = "javascript:RunConsumable('" + activityNum + "')";
                                row["Consumable"].ID = activityNum;
                                row["Consumable"].Href = "#/" + activityNum + "_Consumable";
                            }
                            else
                            {
                                row["Consumable"].Value = "NO";
                            }
                            // If support type is sales and (support method is ship or on-site) and there is a activitydetail record
                            if (row["COMPONENT"].ToString() != "0" && (row["SUPPORT TYPE"].ToString() == "Support") && (row["SUPPORT METHOD"].ToString() == "Ship" || row["SUPPORT METHOD"].ToString() == "On-Site"))
                            {
                                row["Component"].Value = "YES";
                                row["Component"].OnClick = "javascript:RunComponent('" + activityNum + "')";
                                row["Component"].Href = "#/" + activityNum + "_Component";
                            }
                            else
                            {
                                row["Component"].Value = "NO";
                            }
                            //station notes
                            if (!String.IsNullOrEmpty(row["Notes"].ToString()))
                            {
                                if (row["Notes"].ToString() != " ")
                                    row["Notes"].Value = "NOTES";
                                row["Notes"].OnClick = "javascript:RunNotes('" + activityNum + "')";
                                row["Notes"].Href = "#/" + activityNum + "_Notes";
                                Master.UserReport.Columns["Notes"].Exportable = false;
                                row["Notes_Exports"].Value = row["Notes_Exports"].Value.Trim();
                                
                            }
                        }
                        if ((lstInquiryType.SelectedValue.ToString() == "S"))
                        {
                            //if (!String.IsNullOrEmpty(Master.UserReport.Columns["Notes_Exports"].ToString()))
                            //{
                                Master.UserReport.Columns["Notes_Exports"].ToString();
                                Master.UserReport.Columns["Notes_Exports"].Visible = false;
                                Master.UserReport.Columns["Notes_Exports"].Width = 800;
                                Master.UserReport.Columns["Notes_Exports"].Text = "Notes";
                            //}
                        }
                       
                    }
                    catch (FormatException ex)
                    {
                        LogException(ex);
                    }
                }
            }
        }
        //support History -- Company Information Table for Station ID
        private ReportTable StationInfoToReportTable(DataTable dt)
        {
            ReportTable tbl = new ReportTable();
            tbl.RenderLocation = RenderLocation.Above;
            tbl.Title = "Customer Information";
            tbl.Columns.Add("CompanyName", "Company Name", ColumnDataType.String);
            tbl.Columns.Add("Address", "Address", ColumnDataType.String);
            tbl.Columns.Add("City", "City", ColumnDataType.String);
            tbl.Columns.Add("Phone", "Phone", ColumnDataType.String);
            tbl.Columns.Add("ContactName", "Contact Name", ColumnDataType.String);
            tbl.Columns.Add("Description", "Description", ColumnDataType.String);
            tbl.Columns.Add("DISABLED", "Disabled", ColumnDataType.String);
            foreach (DataRow dr in dt.Rows)
            {
                ReportRow rptRow = new ReportRow(tbl);

                rptRow.Cells.Add(dr["CompanyName"].ToString().Trim());
                rptRow.Cells.Add(dr["Address"].ToString().Trim());
                rptRow.Cells.Add(dr["City"].ToString().Trim());
                rptRow.Cells.Add(dr["Phone"].ToString().Trim());
                rptRow.Cells.Add(dr["ContactName"].ToString().Trim());
                rptRow.Cells.Add(dr["Description"].ToString().Trim());
                string strDisabled = "";

                if (!String.IsNullOrEmpty(dr["DISABLED"].ToString()))
                {
                    strDisabled = dr["DISABLED"].ToString();
                    switch (strDisabled)
                    {
                        case "1":
                            strDisabled = "Payment";
                            break;
                        case "2":
                            strDisabled = "Enforcement";
                            break;
                        case "3":
                            strDisabled = "Payment/Enforcement";
                            break;
                        case "4":
                            strDisabled = "Out of Business";
                            break;
                        case "5":
                            strDisabled = "Payment/Out of Bus.";
                            break;
                        case "6":
                            strDisabled = "Enforcement/Out of Bus.";
                            break;
                        case "7":
                            strDisabled = "Payment/Enforce/Out of Bus.";
                            break;
                        case "0":
                            strDisabled = "";
                            break;
                        default:
                            strDisabled = "";
                            break;
                    }

                }
                else
                {
                    rptRow.Cells.Add(String.Empty);
                }
                rptRow.Cells.Add(strDisabled.Trim());
                tbl.Rows.Add(rptRow);
            }
            return tbl;
        }
        //support History -- Company Information Table for Mechanic ID
        private ReportTable MechanicInfoToReportTable(DataTable dt)
        {

            ReportTable tbl = new ReportTable();
            tbl.RenderLocation = RenderLocation.Above;
            tbl.Title = "Mechanic Information";
            tbl.Columns.Add("MECH_NAME", "Mechanic Name", ColumnDataType.String);
            tbl.Columns.Add("SECURITY", "Security", ColumnDataType.String);
            tbl.Columns.Add("CERT_LEVEL", "Cert Level", ColumnDataType.String);
            tbl.Columns.Add("CERT_NUM", "Cert #", ColumnDataType.Number);
            tbl.Columns.Add("STATUS", "Status", ColumnDataType.String);
            tbl.Columns.Add("CBT", "CBT", ColumnDataType.String);

            foreach (DataRow dr in dt.Rows)
            {
                ReportRow rptRow = new ReportRow(tbl);
                rptRow.Cells.Add(dr["Mechanic Name"].ToString().Trim());
                if (!String.IsNullOrEmpty(dr["Security Level"].ToString()))
                {
                    rptRow.Cells.Add(Classes.Reference.SecurityLevelTypes.GetDescription(dr["Security Level"].ToString()[0]));
                }
                else
                {
                    rptRow.Cells.Add(String.Empty);
                }

                rptRow.Cells.Add(dr["Cert Level"].ToString().Trim());
                rptRow.Cells.Add(dr["Cert #"].ToString().Trim());

                if (!String.IsNullOrEmpty(dr["Status"].ToString()))
                {
                    rptRow.Cells.Add(Classes.Reference.MechanicStatusTypes.GetDescription(dr["Status"].ToString()[0]));
                }
                else
                {
                    rptRow.Cells.Add(String.Empty);
                }

                rptRow.Cells.Add(dr["CBT"].ToString().Trim());
                tbl.Rows.Add(rptRow);
            }

            return tbl;
        }
        private ReportTable MechanicDetailsInfoToReportTable(DataTable dt)
        {
            ReportTable tbl = new ReportTable();
            tbl.RenderLocation = RenderLocation.Below;
            //tbl.Title = "Customer Information";
            tbl.Columns.Add("STATIONID", "Station ID", ColumnDataType.String);
            tbl.Columns.Add("INSPECTORID", "Mechanic ID", ColumnDataType.String);
            tbl.Columns.Add("TOTAL_TESTS", "Total Test Count", ColumnDataType.Number);
            tbl.Columns.Add("TOTAL_SUSPECT", "Suspect Test Count", ColumnDataType.Number);
            tbl.Columns.Add("%", "%", ColumnDataType.Percentage);

            foreach (DataRow dr in dt.Rows)
            {
                ReportRow rptRow = new ReportRow(tbl);
                rptRow.Cells.Add(dr["STATIONID"].ToString().Trim());
                rptRow.Cells.Add(dr["INSPECTORID"].ToString().Trim());
                rptRow.Cells.Add(dr["TOTAL_TESTS"].ToString().Trim());
                rptRow.Cells.Add(dr["TOTAL_SUSPECT"].ToString().Trim());
                double formatPercent = Math.Round((Convert.ToInt64(dr["TOTAL_SUSPECT"]) * 1.0) / Convert.ToInt64(dr["TOTAL_TESTS"]), 4, MidpointRounding.AwayFromZero);
                string percent = formatPercent.ToString("0.00%");
                rptRow.Cells.Add(percent);
                tbl.Rows.Add(rptRow);
            }

            return tbl;
        }
        private ReportTable MechanicStationDetailsInfoToReportTable(DataTable dt)
        {
            string activitynum = hidSelectedIndex.Value;
            string activity = activitynum;

            ReportTable tbl = new ReportTable();
            tbl.RenderLocation = RenderLocation.Below;
            //tbl.HeaderNote = activity + " CONSUMABLE";
            tbl.Title = activity + " STATION INFORMATION";
            tbl.Columns.Add("Activity Number", "Activity Number", ColumnDataType.String);
            tbl.Columns.Add("Opened", "Opened", ColumnDataType.String);
            tbl.Columns.Add("Closed", "Closed", ColumnDataType.String);
            tbl.Columns.Add("Mech ID", "Mech ID", ColumnDataType.String);
            tbl.Columns.Add("Unit ID", "Unit ID", ColumnDataType.String);
            tbl.Columns.Add("Support Type", "Support Type", ColumnDataType.String);
            tbl.Columns.Add("Support Method", "Support Method", ColumnDataType.String);
            tbl.Columns.Add("Consumable", "Consumable", ColumnDataType.String);
            tbl.Columns.Add("Component", "Component", ColumnDataType.String);
            tbl.Columns.Add("Notes", "Notes", ColumnDataType.String);

            foreach (DataRow rows in dt.Rows)
            {
                ReportRow rptRow = new ReportRow(tbl);
                rptRow.Cells.Add(rows["Activity Number"].ToString().Trim());

                string strOpenDT, strClosedDT, x;
                strOpenDT = rows["Opened"].ToString();
                strClosedDT = rows["Closed"].ToString();

                if (NHPortal.Classes.NHPortalUtilities.TryConvertDateTimeFormat(strOpenDT, "yyyyMMdd", "M/d/yyyy", out x))
                {
                    string msg = String.Format("Converted {0} to {1}", strOpenDT, x);
                    System.Diagnostics.Debug.WriteLine(msg);
                    rows["OPENED"] = x;
                    rptRow.Cells.Add(rows["Opened"].ToString().Trim());
                }
                if (NHPortal.Classes.NHPortalUtilities.TryConvertDateTimeFormat(strClosedDT, "yyyyMMdd", "M/d/yyyy", out x))
                {
                    rows["CLOSED"] = x;
                    rptRow.Cells.Add(rows["Closed"].ToString().Trim());
                }

                rptRow.Cells.Add(rows["Mech ID"].ToString().Trim());
                rptRow.Cells.Add(rows["Unit ID"].ToString().Trim());
                if (!String.IsNullOrEmpty(rows["Support Type"].ToString()))
                {
                    rows["Support Type"] = Classes.Reference.SupportTypes.GetDescription(rows["Support Type"].ToString()[0]);
                    rptRow.Cells.Add(rows["Support Type"].ToString().Trim());
                }
                if (!String.IsNullOrEmpty(rows["SUPPORT METHOD"].ToString()))
                {
                    String strSUPPORTMETHOD = rows["SUPPORT METHOD"].ToString();
                    switch (strSUPPORTMETHOD)
                    {
                        case "A":
                            strSUPPORTMETHOD = "No - Ship";
                            break;
                        case "O":
                            strSUPPORTMETHOD = "On-Site";
                            break;
                        case "S":
                            strSUPPORTMETHOD = "Ship";
                            break;
                        case "B":
                            strSUPPORTMETHOD = "No - Pend";
                            break;
                        case "D":
                            strSUPPORTMETHOD = "Depot";
                            break;
                        case "N":
                            strSUPPORTMETHOD = "None";
                            break;
                        case "P":
                            strSUPPORTMETHOD = "Pending";
                            break;
                        case "C":
                            strSUPPORTMETHOD = "Callback";
                            break;
                        default:
                            strSUPPORTMETHOD = "UNKNOWN";
                            break;
                    }
                    rows["Support Method"] = strSUPPORTMETHOD.ToString();
                    rptRow.Cells.Add(rows["Support Method"].ToString().Trim());
                }
                string activityNum = rows["Activity Number"].ToString();
                //consumable
                if (rows["SUPPORT TYPE"].ToString() == "Sales" && rows["SUPPORT METHOD"].ToString() == "Ship")
                {
                    ReportCell rc = new ReportCell(rows["Consumable"]);
                    rc.Value = "YES";
                    rc.OnClick = "javascript:RunConsumable('" + activityNum + "')";
                    rc.Href = "#/";
                    rptRow.Cells.Add(rc);

                }
                else
                {
                    ReportCell rc = new ReportCell(rows["Consumable"]);
                    rc.Value = "NO";
                    rptRow.Cells.Add(rc);
                }

                //If support type is sales and (support method is ship or on-site) and there is a activitydetail record
                if (rows["COMPONENT"].ToString() != "0" && (rows["SUPPORT TYPE"].ToString() == "Support") && (rows["SUPPORT METHOD"].ToString() == "Ship" || rows["SUPPORT METHOD"].ToString() == "On-Site"))
                {
                    ReportCell rc = new ReportCell(rows["Component"]);
                    rc.Value = "YES";
                    rc.OnClick = "javascript:RunComponent('" + activityNum + "')";
                    rc.Href = "#/";
                    rptRow.Cells.Add(rc);
                    rows["Component"] = "YES";
                }
                else
                {
                    ReportCell rc = new ReportCell(rows["Component"]);
                    rc.Value = "NO";
                    rptRow.Cells.Add(rc);
                }
                //station notes
                if (!String.IsNullOrEmpty(rows["Notes"].ToString()))
                {
                    if (rows["Notes"].ToString() != " ")
                    {
                        ReportCell rc = new ReportCell(rows["Component"]);
                        rc.Value = "NOTES";
                        rc.OnClick = "javascript:RunNotes('" + activityNum + "')";
                        rc.Href = "#/";
                        rptRow.Cells.Add(rc);
                    }
                    else
                    {
                        rptRow.Cells.Add(rows["Notes"].ToString().Trim());
                    }
                }

                tbl.Rows.Add(rptRow);
            }
            return tbl;
        }

        //runs call history basic information
        private void RunReportCallSupport()
        {
            string qry = BuildQuerySupportContact();
            GDDatabaseClient.Oracle.OracleResponse response = PortalFramework.Database.ODAP.GetDataTable(qry, PortalFramework.Database.DatabaseTarget.Adhoc);
            LogMessage("Attempting to run query for Call History Contact information...", LogSeverity.Trace);

            if (response.Successful)
            {
                LogMessage("Query for Call History Contact information is successful", LogSeverity.Trace);
               
                if (response.HasResults)
                {
                    var table = StationInfoToReportTable(response.ResultsTable);
                Master.UserReport.Tables.Add(table);
                    LogMessage("Query for Call History Contact information has results", LogSeverity.Trace);
                }
            }
            else
            {
                Master.UserReport = null;
                Master.SetError(response.Exception);
                LogMessage("Query error for Support History Contact information: " + response.Exception, LogSeverity.Error);
            }
            LogMessage("Query for Support History Contact information Finshed", LogSeverity.Trace);
            LogOracleResponse(response);
            LogMessage("Support History Contact information Loaded", LogSeverity.Trace);

        }
        //runs call history
        private void RunReportCall()
        {
            string qry = BuildQueryCallHistory();
            GDDatabaseClient.Oracle.OracleResponse responseCall = PortalFramework.Database.ODAP.GetDataTable(qry, PortalFramework.Database.DatabaseTarget.Adhoc);
            LogMessage("Attempting to run BuildQueryCallHistory query...", LogSeverity.Trace);
            if (responseCall.Successful)
            {
                LogMessage("Call History Query is successful", LogSeverity.Trace);
                Master.UserReport = new Report("Call History for Station " + m_inputId, responseCall.ResultsTable);
                SetMetaData();
                Master.SetDataLastUpdate(DateTime.Now);
                if (responseCall.HasResults)
                {
                    LogMessage("Call History Query has results", LogSeverity.Trace);
                    FormatDate();
                    RunReportCallSupport();
                    Master.UserReport.FooterVisibility = FooterVisibility.Auto;
                    Master.UserReport.Sortable = true;
                    LogMessage("Call History Data rendering to page", LogSeverity.Trace);
                }
            }
            else
            {
                Master.UserReport = null;
                Master.SetError(responseCall.Exception);
                LogMessage("Query error for Call History: " + responseCall.Exception, LogSeverity.Error);
            }
            LogMessage("Call History Query Finshed", LogSeverity.Trace);
            LogOracleResponse(responseCall);
            Master.RenderReportToPage();

            LogMessage("Call History Loaded", LogSeverity.Trace);
            SessionHelper.SetCurrentReport(this.Session, Master.UserReport);
        }
        //builds call history summary (table 1)
        private string BuildQueryCallHistory()
        {
            string activitynum = hidSelectedIndex.Value;
            string activity = activitynum;

            StringBuilder sb = new StringBuilder();
            if (lstInquiryType.SelectedValue.ToString() == "S")
            {
                sb.AppendLine(" SELECT C.CALLNUMBER AS \"Call Number\" , ");
                sb.AppendLine(" C.CALLERNAME AS \"Caller\", ");
                sb.AppendLine(" C.CALLDATE as \"Date\", ");
                sb.AppendLine(" C.NOTES  as \"Notes\" ");
                sb.AppendLine(" FROM CALLCENTER C ");
                sb.AppendLine(" WHERE C.STATIONID ='" + m_inputId + "' ");
                sb.AppendLine(" AND C.CALLDATE BETWEEN '" + dateSelector.StartDateControl.GetDateText() + "' AND '" + dateSelector.EndDateControl.GetDateText() + "' ");
                sb.AppendLine(" ORDER BY C.CALLDATE ");
            }
            return sb.ToString();
        }
        private void FormatDate()
        {
            foreach (ReportRow row in Master.UserReport.Rows)
            {
                string strDate, x;
                strDate = row["Date"].Value;
                if (NHPortal.Classes.NHPortalUtilities.TryConvertDateTimeFormat(strDate, "yyyyMMdd", "M/d/yyyy", out x))
                {
                    string msg = String.Format("Converted {0} to {1}", strDate, x);
                    System.Diagnostics.Debug.WriteLine(msg);
                    row["Date"].Value = x;
                }
                row["Notes"].Value = row["Notes"].ToString().Trim();
                Master.UserReport.Columns["Notes"].Width = 500;
            }
        }
        // Runs Billing History (table 1)
        private void RunReportBilling()
        {
            string qry = BuildQueryBilling();
            GDDatabaseClient.Oracle.OracleResponse responseBilling = PortalFramework.Database.ODAP.GetDataTable(qry, PortalFramework.Database.DatabaseTarget.Adhoc);
            LogMessage("Attempting to run Billing History query...", LogSeverity.Trace);
            if (responseBilling.Successful && responseBilling.HasResults)
            {
                LogMessage("Billing History Query is successful", LogSeverity.Trace);
                var table = BillingCustomerInfoToReportTable(responseBilling.ResultsTable);
                Master.UserReport.Tables.Add(table);

                // Master.UserReport.HeaderContent = ConvertBillingCustomerToHTML(responseBilling.ResultsTable) + Environment.NewLine;
            }
            else
            {
                Master.UserReport = null;
                Master.SetError(responseBilling.Exception);
                LogMessage("Query error for Billing History: " + responseBilling.Exception, LogSeverity.Error);
            }
            LogMessage("Billing History Query Finshed", LogSeverity.Trace);
            LogOracleResponse(responseBilling);
            LogMessage("Billing History Loaded", LogSeverity.Trace);
        }
        //sql for for billing history (table 1)
        private string BuildQueryBilling()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(" SELECT ");
            sb.AppendLine(" C.CUSTOMERID AS \"Cust ID\", ");
            sb.AppendLine(" C.CUSTOMERNAME AS \"Customer Name\", ");
            // nai - sb.AppendLine(" TO_CHAR(C.AMOUNTCURRENT, '$99,990.99') AMOUNTCURRENT ");
            sb.AppendLine(" CASE WHEN (C.AMOUNTCURRENT = '      ' ) THEN '$0.00' ELSE TO_CHAR(C.AMOUNTCURRENT, '$99,990.99') END  AS \"Current Balance\"");
            sb.AppendLine(" FROM CUSTOMER C ");
            sb.AppendLine(" JOIN STATION S ON S.CUSTOMERID = C.CUSTOMERID ");
            sb.AppendLine(" WHERE S.STATIONID ='" + m_inputId + "' ");
            return sb.ToString();
        }
        //billing history summary (2nd table)
        private string BuildQueryBillingSummary()
        {
            string activitynum = hidSelectedIndex.Value;
            string activity = activitynum;
            string statementNum = hidSelectedIndex.Value;
            string statement = hidstatement.Value;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(" SELECT STATEMENTNUMBER as \"Statement Number\", ");
            sb.AppendLine(" STATEMENTSTATUS as \"Status\", ");
            sb.AppendLine(" SENTDATE as \"Sent Date\", ");
            sb.AppendLine(" STATEMENTDATE as \"Due Date\", ");
            sb.AppendLine(" SATISFIEDDATE as \"Satisfied Date\", ");
            sb.AppendLine(" TO_CHAR(PREVIOUSCHARGES, '$99,990.99') AS \"Prev. Charges\", ");
            sb.AppendLine(" TO_CHAR(NEWCHARGES, '$99,990.99') AS  \"New Charges\", ");
            sb.AppendLine(" TO_CHAR(TOTALCHARGES, '$99,990.99') AS \"Total Charges\", ");
            sb.AppendLine(" NULL AS \"Details\" ");
            sb.AppendLine(" FROM NHSTATEMEMT NS ");
            sb.AppendLine(" JOIN STATION S ON S.CUSTOMERID = NS.CUSTOMERID ");
            sb.AppendLine(" WHERE S.STATIONID ='" + m_inputId + "' ");
            sb.AppendLine(" AND STATEMENTDATE BETWEEN '" + dateSelector.StartDateControl.GetDateText() + "' AND '" + dateSelector.EndDateControl.GetDateText() + "' ");
            sb.AppendLine(" ORDER BY SENTDATE ");
            return sb.ToString();
        }
        //billing history details, pop ups when you click Details
        private string BuildQueryBillingDetails(DataTable dt)
        {
            string activitynum = hidSelectedIndex.Value;
            string activity = activitynum;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(" SELECT P.POSTINGNUMBER AS ACTIVITYNUM, ");
            sb.AppendLine("S.STATEMENTNUMBER as statement,");
            sb.AppendLine(" P.TYPE, ");
            sb.AppendLine(" TO_CHAR(P.CHARGES, '$99,990.99') CHARGES, ");
            sb.AppendLine(" POSTINGDATE, ");
            sb.AppendLine(" P.UNITID, ");
            sb.AppendLine(" SUPPORTACTIVITY || CASE WHEN TRIM(SUPPORTATTEMPT) IS NULL THEN NULL ELSE '-' END || SUPPORTATTEMPT AS SUPPORTID, ");
            sb.AppendLine(" NOTES ");
            sb.AppendLine(" FROM NHPOSTING P JOIN UNIT U ON P.UNITID = U.UNITID ");
            sb.AppendLine(" JOIN NHSTATEMEMT S ON S.STATEMENTNUMBER = P.STATEMENTNUMBER ");
            sb.AppendLine(" WHERE P.STATEMENTNUMBER = '" + activity + "' ");
            sb.AppendLine(" ORDER BY POSTINGDATE ");
            return sb.ToString();
        }
        //runs summary (2nd table)
        private void RunQueryBillingSummary()
        {
            string qrysumm = BuildQueryBillingSummary();
            GDDatabaseClient.Oracle.OracleResponse billingresponse2 = PortalFramework.Database.ODAP.GetDataTable(qrysumm, PortalFramework.Database.DatabaseTarget.Adhoc);
            LogMessage("Attempting to run Billing History Summary query...", LogSeverity.Trace);
            if (billingresponse2.Successful)
            {
                Master.UserReport = new Report("Billing History for Station " + m_inputId, billingresponse2.ResultsTable);
                SetMetaData();
                Master.SetDataLastUpdate(DateTime.Now);

                if (billingresponse2.HasResults)
                {
                    LogMessage("Billing History Summary Query is successful and has results", LogSeverity.Trace);
                    ConvertBillingtReportColumns();
                    RunReportBilling();
                }
            }
            else
            {
                Master.UserReport = null;
                Master.SetError(billingresponse2.Exception);
                LogMessage("Query error for Billing History Summary: " + billingresponse2.Exception, LogSeverity.Error);
            }
            LogMessage("Billing History Summary Query Finshed", LogSeverity.Trace);
            LogOracleResponse(billingresponse2);
            Master.RenderReportToPage();
            SessionHelper.SetCurrentReport(this.Session, Master.UserReport);
            LogMessage("Billing History Summary Loaded", LogSeverity.Trace);
        }
        //display billing customer information
        private ReportTable BillingCustomerInfoToReportTable(DataTable dt)
        {
            ReportTable tbl = new ReportTable();
            tbl.RenderLocation = RenderLocation.Above;
            tbl.Title = "Customer Information";
            tbl.Columns.Add("Cust ID", "Cust ID", ColumnDataType.String);
            tbl.Columns.Add("Customer Name", "Customer Name", ColumnDataType.String);
            tbl.Columns.Add("Current Balance", "Current Balance", ColumnDataType.String);
            foreach (DataRow dr in dt.Rows)
            {
                ReportRow rptRow = new ReportRow(tbl);
                rptRow.Cells.Add(dr["Cust ID"].ToString().Trim());
                rptRow.Cells.Add(dr["Customer Name"].ToString().Trim());
                rptRow.Cells.Add(dr["Current Balance"].ToString().Trim());
                tbl.Rows.Add(rptRow);
            }
            return tbl;
        }
        private void ConvertBillingtReportColumns()
        {
            if (Master.UserReport != null)
            {
                foreach (ReportRow row in Master.UserReport.Rows)
                {
                    try
                    {
                        string strSentDateDT, strDueDateDT, strsatisfiedDateDT, x;
                        strSentDateDT = row["Sent Date"].ToString();
                        strDueDateDT = row["Due Date"].ToString();
                        strsatisfiedDateDT = row["Satisfied Date"].ToString();
                        if (NHPortal.Classes.NHPortalUtilities.TryConvertDateTimeFormat(strSentDateDT, "yyyyMMdd", "M/d/yyyy", out x))
                        {
                            string msg = String.Format("Converted {0} to {1}", strSentDateDT, x);
                            System.Diagnostics.Debug.WriteLine(msg);
                            row["Sent Date"].Value = x;
                        }
                        if (NHPortal.Classes.NHPortalUtilities.TryConvertDateTimeFormat(strDueDateDT, "yyyyMMdd", "M/d/yyyy", out x))
                        {
                            row["Due Date"].Value = x;
                        }
                        if (NHPortal.Classes.NHPortalUtilities.TryConvertDateTimeFormat(strsatisfiedDateDT, "yyyyMMdd", "M/d/yyyy", out x))
                        {
                            row["Satisfied Date"].Value = x;
                        }
                        if (!String.IsNullOrEmpty(row["Status"].ToString()))
                        {
                            String strSTATEMENTSTATUS = row["Status"].ToString();
                            switch (strSTATEMENTSTATUS)
                            {
                                case "D":
                                    strSTATEMENTSTATUS = "Due";
                                    break;
                                case "S":
                                    strSTATEMENTSTATUS = "Satisfied";
                                    break;
                                case "B":
                                    strSTATEMENTSTATUS = "Returned";
                                    break;
                                case "L":
                                    strSTATEMENTSTATUS = "Late";
                                    break;
                                case "P":
                                    strSTATEMENTSTATUS = "Past due";
                                    break;
                                case "I":
                                    strSTATEMENTSTATUS = "Invoiced";
                                    break;
                                default:
                                    strSTATEMENTSTATUS = "UNKNOWN";
                                    break;
                            }
                            row["Status"].Value = strSTATEMENTSTATUS.ToString();
                            string activityNum = row["Statement Number"].ToString();
                            if (String.IsNullOrEmpty(row["Details"].Value))
                            {
                                row["Details"].Value = "DETAILS";
                                row["Details"].OnClick = "javascript:RunReportBillingDetails('" + activityNum + "')";
                                row["Details"].Href = "#/" + activityNum + "_Details";
                            }
                            Master.UserReport.Columns["Details"].Exportable = false;
                        }
                    }
                    catch (FormatException ex)
                    {
                        LogException(ex);
                    }
                }
            }
        }
        //runs billing details
        private void RunReportBillingDetails()
        {
            string qrysumm = BuildQueryBillingSummary();
            GDDatabaseClient.Oracle.OracleResponse billingresponse2 = PortalFramework.Database.ODAP.GetDataTable(qrysumm, PortalFramework.Database.DatabaseTarget.Adhoc);
            LogMessage("Attempting to run Billing History Summary query...", LogSeverity.Trace);
            if (billingresponse2.Successful)
            {
                LogMessage("Billing History Summary Query is successful and has results", LogSeverity.Trace);
                string qryBilling3 = BuildQueryBillingDetails(billingresponse2.ResultsTable);
                GDDatabaseClient.Oracle.OracleResponse billingresponse3 = PortalFramework.Database.ODAP.GetDataTable(qryBilling3, PortalFramework.Database.DatabaseTarget.Adhoc);
                LogMessage("Attempting to run Billing History Details query...", LogSeverity.Trace);
                if (billingresponse2.Successful)
                {
                    LogMessage("Billing History Summary Query is successful and has results", LogSeverity.Trace);
                    ltrlPopUpRpt.Text = PopUpBillingDetails(billingresponse3.ResultsTable);
                    hidShowOverlay.Value = "TRUE";
                    //SetColumnTypes();
                    LogMessage("Billing History Details Data rendering to page", LogSeverity.Trace);
                }
            }
            else
            {
                Master.SetError(billingresponse2.Exception);
                LogMessage("Query error for Billing History Details: " + billingresponse2.Exception, LogSeverity.Error);
            }
            LogMessage("Billing History Details Query Finshed", LogSeverity.Trace);
            LogOracleResponse(billingresponse2);
            //Master.RenderReportToPage();
        }
        //display of details in a pop up (table 3)
        private string PopUpBillingDetails(DataTable dt)
        {
            string html = "<div class=\"details\" style=\"visibility:visible\" runat=\"server\">";
            html += "<table class=\"expandedDetails\" >";
            html += "<tr><td colspan=\"7\" style=\"text-align:center;\" class=\"table_header\"><h2>" + dt.Rows[0]["statement"].ToString() + " Statement Details</h2></td></tr>";
            html += "<tr class=\"table_header\"><th>Posting Number</th><th>Type</th><th>Charges</th><th>Posting Date</th><th>Unit</th><th>Support ID</th><th>Notes</th></tr>";
            foreach (DataRow row in dt.Rows)
            {
                string strPostDateDT, x;
                strPostDateDT = row["POSTINGDATE"].ToString();

                if (NHPortal.Classes.NHPortalUtilities.TryConvertDateTimeFormat(strPostDateDT, "yyyyMMdd", "M/d/yyyy", out x))
                {
                    string msg = String.Format("Converted {0} to {1}", strPostDateDT, x);
                    System.Diagnostics.Debug.WriteLine(msg);
                    row["POSTINGDATE"] = x;
                }
                if (!String.IsNullOrEmpty(row["TYPE"].ToString()))
                {
                    String strType = row["TYPE"].ToString();
                    switch (strType)
                    {
                        case "D":
                            strType = "Credit Card Payment";
                            break;
                        case "P":
                            strType = "ACH Push Payment";
                            break;
                        case "L":
                            strType = "ACH Pull Payment";
                            break;
                        case "O":
                            strType = "Other Payment";
                            break;
                        case "R":
                            strType = "ACH Return";
                            break;
                        case "A":
                            strType = "Sales Charge";
                            break;
                        case "S":
                            strType = "Service Charge";
                            break;
                        case "H":
                            strType = "	Handling Fee";
                            break;
                        case "I":
                            strType = "	Inspection Charge";
                            break;
                        case "C":
                            strType = "	Admin Credit";
                            break;
                        case "M":
                            strType = "	Misc. Adjustment";
                            break;
                        case "F":
                            strType = "	Late Fee";
                            break;
                        case "E":
                            strType = "	Processing Fee";
                            break;
                        default:
                            strType = "UNKNOWN";
                            break;
                    }
                    row["TYPE"] = strType.ToString();
                }
                html += "<tr><td>";
                html += row["ACTIVITYNUM"];
                html += "</td>";
                html += "<td>" + row["TYPE"] + "</td>";
                html += "<td>" + row["CHARGES"] + "</td>";
                html += "<td>" + row["POSTINGDATE"] + "</td>";
                html += "<td>" + row["UNITID"] + "</td>";
                html += "<td>" + row["SUPPORTID"] + "</td>";
                html += "<td>" + row["Notes"] + "</td>";

                html += "</tr>";
            }
            html += "</table>";
            html += "</div>";
            return html;
        }
        //builds notes for billing history (table 4)
        private string BuildNotesHistorySQL(DataTable dt)
        {
            string activitynum = hidSelectedIndex.Value;
            string activity = activitynum;

            string[] parts = activity.Split(',');
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("SELECT P.POSTINGNUMBER AS ACTIVITYNUM, ");
            builder.AppendLine("S.STATEMENTNUMBER as statement,");
            builder.AppendLine("NOTES");
            builder.AppendLine("FROM NHPOSTING P JOIN UNIT U ON P.UNITID = U.UNITID ");
            builder.AppendLine(" JOIN NHSTATEMEMT S ON S.STATEMENTNUMBER = P.STATEMENTNUMBER ");
            builder.AppendLine(" WHERE P.POSTINGNUMBER ='" + parts[0] + "' ");
            builder.AppendLine("AND S.STATEMENTNUMBER = '" + parts[1] + "' ");
            return builder.ToString();

        }
    }
}

