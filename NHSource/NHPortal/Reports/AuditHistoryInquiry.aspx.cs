using NHPortal.Classes;
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
using GDCoreUtilities.Logging;
using NHPortal.Classes.User;

namespace NHPortal.Reports
{
    public partial class AuditHistoryInquiry : PortalPage 
    {
        private string m_inputId;

        protected void Page_Load(object sender, EventArgs e)
        {
            RedirectOnInvalidPermission(UserPermissions.AuditHistoryInquiry);
            if (IsPostBack)
            {
                ProcessPostBack();
            }
            else
            {
                InitializePage();
                LoadFavorite();
            }

            InitializeInputs();
        }

        private void InitializeInputs()
        {
            string onchange = String.Format("javascript: inquiryChange(this, '{0}');", tbInquiry.ClientID);
            cboInquiryType.Attributes.Add("onchange", onchange);
            if (cboInquiryType.SelectedValue == "S")
            {
                tbInquiry.MaxLength = 8;
            }
        }

        private void InitializePage()
        {
            Master.SetHeaderText("Audit History Inquiry Report");
            cboInquiryType.Initialize();
            dateSelector.StartDateControl.Text = DateTime.Now.ToString("M/1/yyyy");
            //tbInquiry.Text = tbInquiry.Text.ToUpper().Trim();
        }

        private void ProcessPostBack()
        {
            string action = Master.MasterPage.HidActionValue;
            SetInputId();

            switch (action)
            {
                case "RUN_REPORT":
                    RunReports();
                    break;
                case "SELECT_ROW":
                    RunDetailsReport();
                    break;
                case "SAVE_FAVORITE":
                    SaveFavorite();
                    break;
            }

        }

        private void SetInputId()
        {
            if (cboInquiryType.SelectedValue == "S")
            {
                m_inputId = NHPortalUtilities.ToStationID(tbInquiry.Text);
            }
            else if (cboInquiryType.SelectedValue == "O")
            {
                m_inputId = NHPortalUtilities.ToOfficerID(tbInquiry.Text);
            }
            else
            {
                m_inputId = tbInquiry.Text.Trim().ToUpper();
            }
        }

        private void SaveFavorite()
        {
            Master.SaveFavorite(NHPortal.UserControls.RedirectCodes.REPORT_AUDIT_HISTORY_INQUIRY, UserFavoriteTypes.Report);
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
                        case "INQUIRY TYPE":
                            cboInquiryType.SelectedValue = c.Value;
                            break;
                        case "ID/OLN":
                            tbInquiry.Text = c.Value;
                            break;
                    }

                }
            } 
        }
        private void SetColumnTypes()
        {
            if (Master.UserReport != null && Master.UserReport.ColumnCount > 2)
            {
                // Mechanic/Station/Inspector ID
                Master.UserReport.Columns[2].ColumnDataType = ColumnDataType.Number;

                if ((cboInquiryType.SelectedValue == "R") || (cboInquiryType.SelectedValue == "O"))
                {
                    Master.UserReport.Columns[2].ColumnDataType = ColumnDataType.String;
                }
            }
        }
        private void RunReports()
        {
            string qry = BuildQuery();
            GDDatabaseClient.Oracle.OracleResponse response = PortalFramework.Database.ODAP.GetDataTable(qry, PortalFramework.Database.DatabaseTarget.Adhoc);
            LogMessage("Attempting to run query...", LogSeverity.Information);
            if (response.Successful)
            {
                LogMessage("Query is successful", LogSeverity.Information);
                Master.UserReport = new Report("Audit History Inquiry", response.ResultsTable);
                SetMetaData();
                Master.SetDataLastUpdate(DateTime.Now);
                if (response.HasResults)
                {
                    LogMessage("Query has results", LogSeverity.Information);
                    ConvertReportColumns();
                    ReportColumn col = Master.UserReport.Columns.Insert("Details", "Select", 0);
                    col.Exportable = false;
                    Master.UserReport.Columns["NOTES"].Width = 500;
                    AddDetailSelectRow();
                    SetColumnTypes();
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

        private void RunDetailsReport()
        {
            string qry = BuildDetailsQuery();
            GDDatabaseClient.Oracle.OracleResponse response = PortalFramework.Database.ODAP.GetDataRow(qry, PortalFramework.Database.DatabaseTarget.Adhoc);
            LogMessage("Attempting to run additional detail query...", LogSeverity.Information);
            if (response.Successful)
            {
                LogMessage("Additiontal Query is successful", LogSeverity.Information);
                ltrlDetailsRpt.Text = ConvertTabletoHtml(response.ResultsRow);
                LogMessage("Additional data displayed", LogSeverity.Information);
                hidShowOverlay.Value = "TRUE";
            }
            else
            {
                Master.UserReport = null;
                Master.SetError(response.Exception);
                LogMessage("Query error: " + response.Exception, LogSeverity.Information);
            }
            LogMessage("Additional Query Finshed and page loaded", LogSeverity.Information);
            LogOracleResponse(response);
        }

        //html for the expanded data table so it doesn't look the same as the rendered
        public string ConvertTabletoHtml(DataRow row)
        {
            if (!String.IsNullOrEmpty(row["Reschedule Date"].ToString()))
            {
                String strRescheduledDate = row["Reschedule Date"].ToString();
                if (strRescheduledDate == "00000000")
                {
                    row["Reschedule Date"] = "None";
                }
                else
                {
                    DateTime dateRescheduledDate = DateTime.ParseExact(strRescheduledDate, "yyyyMMdd", CultureInfo.InvariantCulture);
                    row["Reschedule Date"] = dateRescheduledDate.ToShortDateString().ToString();
                }
            }
            string strPriorAuditDT, x;
            strPriorAuditDT = row["Date"].ToString();

            if (NHPortal.Classes.NHPortalUtilities.TryConvertDateTimeFormat(strPriorAuditDT, "yyyyMMdd", "M/d/yyyy", out x))
            {
                string msg = String.Format("Converted {0} to {1}", strPriorAuditDT, x);
                System.Diagnostics.Debug.WriteLine(msg);
                row["Date"] = x;
            }
            string strPriorAuditDTT, y;
            strPriorAuditDTT = row["Time"].ToString();

            if (NHPortal.Classes.NHPortalUtilities.TryConvertDateTimeFormat(strPriorAuditDTT, "HHmmss", "hh:mm:ss tt", out y))
            {
                string msg = String.Format("Converted {0} to {1}", strPriorAuditDTT, y);
                System.Diagnostics.Debug.WriteLine(msg);
                row["Time"] = y;
            }

            row["Overall Result"] = Classes.Reference.AuditResultTypes.GetDescription(row["Overall Result"].ToString()[0]);
            //row["Station Audit Type"] = Classes.Reference.StationAuditTypes.GetDescription(row["Station Audit Type"].ToString()[0]);
            row["Emission Type"] = Classes.Reference.EmissionTypes.GetDescription(row["Emission Type"].ToString()[0]);
            //changing none to N/A until told otherwise

            if (!String.IsNullOrEmpty(row["Emission Type"].ToString()))
            {
                String strEmisType = row["Emission Type"].ToString();
                switch (strEmisType)
                {
                    case "NONE":
                        strEmisType = "N/A";
                        break;
                }
                row["Emission Type"] = strEmisType.ToString();
            }

            if (!String.IsNullOrEmpty(row["Mechanic OLN"].ToString()))
            {
                if (row["Mechanic OLN"].ToString() != "N/A")
                    row["Audit Type"] = "MECHANIC";
                else
                {
                    row["Audit Type"] = "STATION";
                }
            }

            row["Safety Type"] = Classes.Reference.SafetyTypes.GetDescription(row["Safety Type"].ToString()[0]);

            if (!String.IsNullOrEmpty(row["Safety Type"].ToString()))
            {
                String strSafetyType = row["Safety Type"].ToString();

                switch (strSafetyType)
                {
                    case "N":
                        strSafetyType = "N/A";
                        break;
                    //default:
                    //    strSafetyType = "N/A";
                    //    break;
                }
                row["Safety Type"] = strSafetyType.ToString();
            }

            //temporary until we can get station audit type class to work
            if (!String.IsNullOrEmpty(row["Station Audit Type"].ToString()))
            {
                String strStationAuditType = row["Station Audit Type"].ToString().Trim();
                switch (strStationAuditType)
                {
                    case "A":
                        strStationAuditType = "AUTOMOBILE";
                        break;
                    case "M":
                        strStationAuditType = "MOTORCYCLE";
                        break;
                    case "I":
                        strStationAuditType = "MOTORCYCLE";
                        break;
                    case "F":
                        strStationAuditType = "FLEET";
                        break;
                    case "U":
                        strStationAuditType = "MUNICIPLE";
                        break;
                    default:
                        strStationAuditType = "N/A";
                        break;
                }
                row["Station Audit Type"] = strStationAuditType.ToString().Trim();
            }

            string html = "<div class=\"details\" style=\"visibility:visible\">";
            html += "<table class=\"expandedDetails\" >";
            html += "<tr><td colspan =\"4\" style=\"text-align:center;\" class=\"table_header\"><h2 id=\"dialogHeader\">Audit History Inquiry Details</h2></td></tr>";
            html += "<tr>";
            html += " <td class=\"detail_title\">Audit Date</td>";
            html += "<td>";
            html += row["DATE"];
            html += "</td>";
            html += " <td class=\"detail_title\">Audit Time</td>";
            html += "<td>";
            html += row["TIME"];
            html += "</td>";
            html += "</tr>";
            html += "<tr>";
            html += " <td class=\"detail_title\">Audit Type</td>";
            html += "<td>";
            html += row["Audit Type"];
            html += "</td>";
            html += " <td class=\"detail_title\">Station Audit Type</td>";
            html += "<td>";
            html += row["Station Audit Type"];
            html += "</td>";
            html += "</tr>";
            html += "<tr>";
            html += " <td class=\"detail_title\">Safety Type</td>";
            html += "<td>";
            html += row["Safety Type"];
            html += "</td>";
            html += " <td class=\"detail_title\">Emission Type</td>";
            html += "<td>";
            html += row["Emission Type"];
            html += "</td>";
            html += "</tr>";
            html += "<tr>";
            html += " <td class=\"detail_title\">Station ID</td>";
            html += "<td>";
            html += row["Station ID"];
            html += "</td>";
            html += " <td class=\"detail_title\">Station Name</td>";
            html += "<td>";
            html += row["Station Name"];
            html += "</td>";
            html += "</tr>";
            html += "<tr>";
            html += " <td class=\"detail_title\">Mechanic OLN</td>";
            html += "<td>";
            html += row["Mechanic OLN"];
            html += "</td>";
            html += " <td class=\"detail_title\">Mechanic Name</td>";
            html += "<td>";
            html += row["Mechanic Name"];
            html += "</td>";
            html += "</tr>";
            html += "<tr>";
            html += "<td class=\"detail_title\">Audit Seq.</td>";
            html += "<td>";
            html += row["Audit Seq."];
            html += "</td>";
            html += " <td class=\"detail_title\">Audit Result</td>";
            html += "<td>";
            html += row["Overall Result"];
            html += "</td>";
            html += "</tr>";
            html += "<tr>";
            html += " <td class=\"detail_title\">Reschedule Date</td>";
            html += "<td>";
            html += row["Reschedule Date"];
            html += "</td>";
            html += " <td class=\"detail_title\">Officer ID</td>";
            html += "<td>";
            html += row["Officer ID"];
            html += "</td>";
            html += "</tr>";
            html += "<tr>";
            html += " <td class=\"detail_title\">Notes</td>";
            html += "<td colspan=\"3\">";
            html += row["NOTES"].ToString().Trim();
            html += "</td>";
            html += "</tr>";

            var i = 0;
            foreach (DataColumn col in row.Table.Columns)
            {
                if (col.ColumnName.StartsWith("CODE") && row[col].Equals("2"))
                {
                    i = i + 1;
                    if (i == 1)
                    {
                        html += "<tr><td colspan =\"4\"class=\"detail_def\">Breakdown By Deficient Items (Red)</td></tr>";
                        html += "<tr><td colspan=\"2\"class=\"detail_def\">Category</td>";
                        html += "<td colspan=\"2\"class=\"detail_def\">Item</td></tr>";
                    }

                    //this is for the break down deficiency
                    #region station code values
                    string category = "";
                    string item = "";

                    if (col.ColumnName.Equals("CODE1") || col.ColumnName.Equals("CODE2") || col.ColumnName.Equals("CODE3") || col.ColumnName.Equals("CODE4"))
                    {
                        category = "CERTIFICATION";
                        item = "STATION";
                        if (col.ColumnName.Equals("CODE1"))
                        {
                            if (row["Emission Type"].Equals("Visual"))
                            {
                                category = "VISUAL";
                                item = "CRANKCASE VENTILATION";
                            }
                            else if (row["Emission Type"].Equals("OBD"))
                            {
                                category = "OBD";
                                item = "OBD BULB CHECK/ENGINE OFF";
                            }
                        }
                        if (col.ColumnName.Equals("CODE2"))
                        {
                            if (row["Emission Type"].Equals("Visual"))
                            {
                                category = "VISUAL";
                                item = "AIR INJECTION PUMP/PULSE";
                            }
                            else if (row["Emission Type"].Equals("OBD"))
                            {
                                category = "OBD";
                                item = "OBD MIL CHECK/ENGINE RUNNING";
                            }
                        }
                        if (col.ColumnName.Equals("CODE3"))
                        {
                            if (row["Emission Type"].Equals("Visual"))
                            {
                                category = "VISUAL";
                                item = "FUEL SYSTEM";
                            }
                            else if (row["Emission Type"].Equals("OBD"))
                            {
                                category = "OBD";
                                item = "OBD CONNECTOR LOCATION";
                            }
                        }
                        if (col.ColumnName.Equals("CODE4"))
                        {
                            if (row["Emission Type"].Equals("Visual"))
                            {
                                category = "VISUAL";
                                item = "EVAP CANISTER";
                            }
                            else if (row["Emission Type"].Equals("OBD"))
                            {
                                category = "OBD";
                                item = "OBD CONNECTOR CHECK";
                            }
                        }
                    }
                    if (col.ColumnName.Equals("CODE5"))
                    {
                        category = "CERTIFICATION";
                        item = "MECHANICS";

                        if (row["Emission Type"].Equals("Visual"))
                        {
                            category = "VISUAL";
                            item = "EXHAUST SYSTEM";
                        }
                        else if (row["Emission Type"].Equals("OBD"))
                        {
                            category = "OBD";
                            item = "OBD ELECTRONIC CHECK";
                        }
                    }
                    if (col.ColumnName.Equals("CODE6"))
                    {
                        category = "GENERAL";
                        item = "HOURS DISPLAYED";
                        if (row["Emission Type"].Equals("OBD"))
                        {
                            category = "OBD";
                            item = "OBD READINESS CHECK";
                        }
                    }
                    if (col.ColumnName.Equals("CODE7"))
                    {
                        category = "GENERAL";
                        item = "STATION SIGN";
                        if (row["Emission Type"].Equals("OBD"))
                        {
                            category = "OBD";
                            item = "OBD TEST REPORT";
                        }
                    }
                    if (col.ColumnName.Equals("CODE8"))
                    {
                        category = "GENERAL";
                        item = "FEE SIGNS";
                    }
                    if (col.ColumnName.Equals("CODE9"))
                    {
                        category = "GENERAL";
                        item = "MOTORCYCLE SIGN";
                    }
                    if (col.ColumnName.Equals("CODE10"))
                    {
                        category = "GENERAL";
                        item = "INSPECTION AREA";
                    }
                    if (col.ColumnName.Equals("CODE11"))
                    {
                        category = "GENERAL";
                        item = "INSPECTION SPACE";
                    }
                    if (col.ColumnName.Equals("CODE12"))
                    {
                        category = "GENERAL";
                        item = "REJECTION FORM";
                    }
                    if (col.ColumnName.Equals("CODE13"))
                    {
                        category = "GENERAL";
                        item = "MECHANICS ON DUTY";
                    }
                    if (col.ColumnName.Equals("CODE14"))
                    {
                        category = "STICKER";
                        item = "PROPERLY SECURED";
                    }
                    if (col.ColumnName.Equals("CODE15"))
                    {
                        category = "STICKER";
                        item = "PROPERLY RETURNED";
                    }
                    if (col.ColumnName.Equals("CODE17"))
                    {
                        category = "NHOST UNIT";
                        item = "";
                    }
                    if (col.ColumnName.Equals("CODE18"))
                    {
                        category = "MANUALS";
                        item = "";
                    }
                    if (col.ColumnName.Equals("CODE19"))
                    {
                        category = "TOOLS/EQUIPMENT";
                        item = "BRAKE DRUM GAUGE";
                    }
                    if (col.ColumnName.Equals("CODE20"))
                    {
                        category = "TOOLS/EQUIPMENT";
                        item = "BRAKE PAD GAUGE";
                    }
                    if (col.ColumnName.Equals("CODE21"))
                    {
                        category = "TOOLS/EQUIPMENT";
                        item = "BALL JOINT GAUGE";
                    }
                    if (col.ColumnName.Equals("CODE22"))
                    {
                        category = "TOOLS/EQUIPMENT";
                        item = "LIFT OR JACK";
                    }
                    if (col.ColumnName.Equals("CODE23"))
                    {
                        category = "TOOLS/EQUIPMENT";
                        item = "HAND TOOLS";
                    }
                    if (col.ColumnName.Equals("CODE24"))
                    {
                        category = "TOOLS/EQUIPMENT";
                        item = "HEADLIGHT TOOLS";
                    }
                    if (col.ColumnName.Equals("CODE25"))
                    {
                        category = "TOOLS/EQUIPMENT";
                        item = "TINT METER";
                    }
                    if (col.ColumnName.Equals("CODE26"))
                    {
                        category = "TOOLS/EQUIPMENT";
                        item = "DECIBEL METER";
                    }
                    if (col.ColumnName.Equals("CODE27"))
                    {
                        category = "STICKER";
                        item = "STICKER STUB COMPLETION";
                    }
                    if (col.ColumnName.Equals("CODE28"))
                    {
                        category = "STICKER";
                        item = "STICKER BACK COMPLETION";
                    }
                    if (col.ColumnName.Equals("CODE51"))
                    {
                        if (row["Safety Type"].Equals("BASIC"))
                        {
                            category = "SAFETY/BASIC";
                            item = "VERIFY VEHICLE INFO";
                        }
                        else if (row["Safety Type"].Equals("BUS") || row["Safety Type"].Equals("TRUCK"))
                        {
                            category = "SAFETY/TRUCK";
                            item = "VERIFIED VEHICLE INFO";
                        }
                        else if (row["Safety Type"].Equals("TRAILER"))
                        {
                            category = "SAFETY/TRAILER";
                            item = "VERIFIED VEHICLE INFO";
                        }
                        else if (row["Safety Type"].Equals("AGRICULTURE"))
                        {
                            category = "SAFETY/AGRICULTURE";
                            item = "VERIFIED VEHICLE INFO";
                        }
                        else if (row["Safety Type"].Equals("MOTORCYCLE"))
                        {
                            category = "SAFETY/MOTORCYCLE";
                            item = "VERIFIED VEHICLE INFO";
                        }
                        //category = "STICKER";
                        //item = "STICKER BACK COMPLETION";
                    }
                    if (col.ColumnName.Equals("CODE52"))
                    {
                        if (row["Safety Type"].Equals("BASIC"))
                        {
                            category = "SAFETY/BASIC";
                            item = "CORRECT VEHICLE INFO";
                        }
                        else if (row["Safety Type"].Equals("BUS") || row["Safety Type"].Equals("TRUCK"))
                        {
                            category = "SAFETY/TRUCK";
                            item = "CORRECT VEHICLE INFO";
                        }
                        else if (row["Safety Type"].Equals("TRAILER"))
                        {
                            category = "SAFETY/TRAILER";
                            item = "CORRECT VEHICLE INFO";
                        }
                        else if (row["Safety Type"].Equals("AGRICULTURE"))
                        {
                            category = "SAFETY/AGRICULTURE";
                            item = "CORRECT VEHICLE INFO";
                        }
                        else if (row["Safety Type"].Equals("MOTORCYCLE"))
                        {
                            category = "SAFETY/MOTORCYCLE";
                            item = "CORRECT VEHICLE INFO";
                        }
                    }
                    if (col.ColumnName.Equals("CODE53"))
                    {
                        if (row["Safety Type"].Equals("BASIC"))
                        {
                            category = "SAFETY/BASIC";
                            item = "LICENSE REJECTS";
                        }
                        else if (row["Safety Type"].Equals("BUS") || row["Safety Type"].Equals("TRUCK"))
                        {
                            category = "SAFETY/TRUCK";
                            item = "LICENSE REJECTS";
                        }
                        else if (row["Safety Type"].Equals("TRAILER"))
                        {
                            category = "SAFETY/TRAILER";
                            item = "LICENSE REJECTS";
                        }
                        else if (row["Safety Type"].Equals("AGRICULTURE"))
                        {
                            category = "SAFETY/AGRICULTURE";
                            item = "LICENSE REJECTS";
                        }
                        else if (row["Safety Type"].Equals("MOTORCYCLE"))
                        {
                            category = "SAFETY/MOTORCYCLE";
                            item = "LICENSE REJECTS";
                        }
                    }
                    if (col.ColumnName.Equals("CODE54"))
                    {
                        if (row["Safety Type"].Equals("BASIC"))
                        {
                            category = "SAFETY/BASIC";
                            item = "IFTA STICKERS";
                        }
                        else if (row["Safety Type"].Equals("BUS") || row["Safety Type"].Equals("TRUCK"))
                        {
                            category = "SAFETY/TRUCK";
                            item = "IFTA STICKERS";
                        }
                        else if (row["Safety Type"].Equals("TRAILER"))
                        {
                            category = "SAFETY/TRAILER";
                            item = "TIRES";
                        }
                        else if (row["Safety Type"].Equals("AGRICULTURE"))
                        {
                            category = "SAFETY/AGRICULTURE";
                            item = "STEERING WHEEL";
                        }
                        else if (row["Safety Type"].Equals("MOTORCYCLE"))
                        {
                            category = "SAFETY/MOTORCYCLE";
                            item = "WHEELS";
                        }
                    }
                    if (col.ColumnName.Equals("CODE55"))
                    {
                        if (row["Safety Type"].Equals("BASIC"))
                        {
                            category = "SAFETY/BASIC";
                            item = "WHEELS";
                        }
                        else if (row["Safety Type"].Equals("BUS") || row["Safety Type"].Equals("TRUCK"))
                        {
                            category = "SAFETY/TRUCK";
                            item = "WHEELS";
                        }
                        else if (row["Safety Type"].Equals("TRAILER"))
                        {
                            category = "SAFETY/TRAILER";
                            item = "MAIN BRAKES";
                        }
                        else if (row["Safety Type"].Equals("AGRICULTURE"))
                        {
                            category = "SAFETY/AGRICULTURE";
                            item = "STOP LIGHTS";
                        }
                        else if (row["Safety Type"].Equals("MOTORCYCLE"))
                        {
                            category = "SAFETY/MOTORCYCLE";
                            item = "TIRES";
                        }
                    }
                    if (col.ColumnName.Equals("CODE56"))
                    {
                        if (row["Safety Type"].Equals("BASIC"))
                        {
                            category = "SAFETY/BASIC";
                            item = "TIRES";
                        }
                        else if (row["Safety Type"].Equals("BUS") || row["Safety Type"].Equals("TRUCK"))
                        {
                            category = "SAFETY/TRUCK";
                            item = "TIRES";
                        }
                        else if (row["Safety Type"].Equals("TRAILER"))
                        {
                            category = "SAFETY/TRAILER";
                            item = "PARKING BRAKES (AIR)";
                        }
                        else if (row["Safety Type"].Equals("AGRICULTURE"))
                        {
                            category = "SAFETY/AGRICULTURE";
                            item = "EXHAUST SYSTEM";
                        }
                        else if (row["Safety Type"].Equals("MOTORCYCLE"))
                        {
                            category = "SAFETY/MOTORCYCLE";
                            item = "STEERING/FRONT END";
                        }
                    }
                    if (col.ColumnName.Equals("CODE57"))
                    {
                        if (row["Safety Type"].Equals("BASIC"))
                        {
                            category = "SAFETY/BASIC";
                            item = "STEERING/FRONT END";
                        }
                        else if (row["Safety Type"].Equals("BUS") || row["Safety Type"].Equals("TRUCK"))
                        {
                            category = "SAFETY/TRUCK";
                            item = "STEERING/FRONT END";
                        }
                        else if (row["Safety Type"].Equals("TRAILER"))
                        {
                            category = "SAFETY/TRAILER";
                            item = "EMERGENCY BRAKES";
                        }
                        else if (row["Safety Type"].Equals("AGRICULTURE"))
                        {
                            category = "SAFETY/AGRICULTURE";
                            item = "HEADLIGHTS";
                        }
                        else if (row["Safety Type"].Equals("MOTORCYCLE"))
                        {
                            category = "SAFETY/MOTORCYCLE";
                            item = "FOOT BRAKE";
                        }
                    }
                    if (col.ColumnName.Equals("CODE58"))
                    {
                        if (row["Safety Type"].Equals("BASIC"))
                        {
                            category = "SAFETY/BASIC";
                            item = "FOOT BRAKE";
                        }
                        else if (row["Safety Type"].Equals("BUS") || row["Safety Type"].Equals("TRUCK"))
                        {
                            category = "SAFETY/TRUCK";
                            item = "FOOT BRAKE";
                        }
                        else if (row["Safety Type"].Equals("TRAILER"))
                        {
                            category = "SAFETY/TRAILER";
                            item = "BRAKE WIRINGS";
                        }
                        else if (row["Safety Type"].Equals("AGRICULTURE"))
                        {
                            category = "SAFETY/AGRICULTURE";
                            item = "REFLECTORS";
                        }
                        else if (row["Safety Type"].Equals("MOTORCYCLE"))
                        {
                            category = "SAFETY/MOTORCYCLE";
                            item = "INSTRUMENTS";
                        }
                    }
                    if (col.ColumnName.Equals("CODE59"))
                    {
                        if (row["Safety Type"].Equals("BASIC"))
                        {
                            category = "SAFETY/BASIC";
                            item = "PARKING BRAKE";
                        }
                        else if (row["Safety Type"].Equals("BUS") || row["Safety Type"].Equals("TRUCK"))
                        {
                            category = "SAFETY/TRUCK";
                            item = "PARKING BRAKE";
                        }
                        else if (row["Safety Type"].Equals("TRAILER"))
                        {
                            category = "SAFETY/TRAILER";
                            item = "REAR LIGHTS";
                        }
                        else if (row["Safety Type"].Equals("AGRICULTURE"))
                        {
                            category = "SAFETY/AGRICULTURE";
                            item = "TAIL LIGHTS";
                        }
                        else if (row["Safety Type"].Equals("MOTORCYCLE"))
                        {
                            category = "SAFETY/MOTORCYCLE";
                            item = "HORN/ELECTRICAL SYSTEM";
                        }
                    }
                    if (col.ColumnName.Equals("CODE60"))
                    {
                        if (row["Safety Type"].Equals("BASIC"))
                        {
                            category = "SAFETY/BASIC";
                            item = "INSTRUMENTS";
                        }
                        else if (row["Safety Type"].Equals("BUS") || row["Safety Type"].Equals("TRUCK"))
                        {
                            category = "SAFETY/TRUCK";
                            item = "AIR BRAKE SYSTEM";
                        }
                        else if (row["Safety Type"].Equals("TRAILER"))
                        {
                            category = "SAFETY/TRAILER";
                            item = "STOP LIGHTS";
                        }
                        else if (row["Safety Type"].Equals("MOTORCYCLE"))
                        {
                            category = "SAFETY/MOTORCYCLE";
                            item = "REAR LIGHTS";
                        }
                    }
                    if (col.ColumnName.Equals("CODE61"))
                    {
                        if (row["Safety Type"].Equals("BASIC"))
                        {
                            category = "SAFETY/BASIC";
                            item = "HORN/ELECTRICAL SYSTEM";
                        }
                        else if (row["Safety Type"].Equals("BUS") || row["Safety Type"].Equals("TRUCK"))
                        {
                            category = "SAFETY/TRUCK";
                            item = "INSTRUMENTS";
                        }
                        else if (row["Safety Type"].Equals("TRAILER"))
                        {
                            category = "SAFETY/TRAILER";
                            item = "BUMPER";
                        }
                        else if (row["Safety Type"].Equals("MOTORCYCLE"))
                        {
                            category = "SAFETY/MOTORCYCLE";
                            item = "STOP LIGHTS";
                        }
                    }
                    if (col.ColumnName.Equals("CODE62"))
                    {
                        if (row["Safety Type"].Equals("BASIC"))
                        {
                            category = "SAFETY/BASIC";
                            item = "REAR LIGHTS";
                        }
                        else if (row["Safety Type"].Equals("BUS") || row["Safety Type"].Equals("TRUCK"))
                        {
                            category = "SAFETY/TRUCK";
                            item = "HORN/ELETRICAL SYSTEM";
                        }
                        else if (row["Safety Type"].Equals("TRAILER"))
                        {
                            category = "SAFETY/TRAILER";
                            item = "BODY/CHASSIS";
                        }
                        else if (row["Safety Type"].Equals("MOTORCYCLE"))
                        {
                            category = "SAFETY/MOTORCYCLE";
                            item = "FRONT LIGHTS";
                        }
                    }
                    if (col.ColumnName.Equals("CODE63"))
                    {
                        if (row["Safety Type"].Equals("BASIC"))
                        {
                            category = "SAFETY/BASIC";
                            item = "STOP LIGHTS";
                        }
                        else if (row["Safety Type"].Equals("BUS") || row["Safety Type"].Equals("TRUCK"))
                        {
                            category = "SAFETY/TRUCK";
                            item = "REAR LIGHTS";
                        }
                        else if (row["Safety Type"].Equals("MOTORCYCLE"))
                        {
                            category = "SAFETY/MOTORCYCLE";
                            item = "CHECKED DIRECTIONAL SIGNAL";
                        }
                    }
                    if (col.ColumnName.Equals("CODE64"))
                    {
                        if (row["Safety Type"].Equals("BASIC"))
                        {
                            category = "SAFETY/BASIC";
                            item = "FRONT LIGHTS";
                        }
                        else if (row["Safety Type"].Equals("BUS") || row["Safety Type"].Equals("TRUCK"))
                        {
                            category = "SAFETY/TRUCK";
                            item = "STOP LIGHTS";
                        }
                        else if (row["Safety Type"].Equals("MOTORCYCLE"))
                        {
                            category = "SAFETY/MOTORCYCLE";
                            item = "CHECKED OTHER LIGHTS";
                        }
                    }
                    if (col.ColumnName.Equals("CODE65"))
                    {
                        if (row["Safety Type"].Equals("BASIC"))
                        {
                            category = "SAFETY/BASIC";
                            item = "DIRECTIONAL SIGNAL";
                        }
                        else if (row["Safety Type"].Equals("BUS") || row["Safety Type"].Equals("TRUCK"))
                        {
                            category = "SAFETY/TRUCK";
                            item = "FRONT LIGHTS";
                        }
                        else if (row["Safety Type"].Equals("MOTORCYCLE"))
                        {
                            category = "SAFETY/MOTORCYCLE";
                            item = "CHECKED HEADLIGHT AIM";
                        }
                    }
                    if (col.ColumnName.Equals("CODE66"))
                    {
                        if (row["Safety Type"].Equals("BASIC"))
                        {
                            category = "SAFETY/BASIC";
                            item = "OTHER LIGHTS";
                        }
                        else if (row["Safety Type"].Equals("BUS") || row["Safety Type"].Equals("TRUCK"))
                        {
                            category = "SAFETY/TRUCK";
                            item = "DIRECTIONAL SIGNAL";
                        }
                        else if (row["Safety Type"].Equals("MOTORCYCLE"))
                        {
                            category = "SAFETY/MOTORCYCLE";
                            item = "CHECKED MIRRORS";
                        }
                    }
                    if (col.ColumnName.Equals("CODE67"))
                    {
                        if (row["Safety Type"].Equals("BASIC"))
                        {
                            category = "SAFETY/BASIC";
                            item = "HEADLIGHT AIM";
                        }
                        else if (row["Safety Type"].Equals("BUS") || row["Safety Type"].Equals("TRUCK"))
                        {
                            category = "SAFETY/TRUCK";
                            item = "OTHER LIGHTS";
                        }
                        else if (row["Safety Type"].Equals("MOTORCYCLE"))
                        {
                            category = "SAFETY/MOTORCYCLE";
                            item = "CHECKED GLASS";
                        }
                    }
                    if (col.ColumnName.Equals("CODE68"))
                    {
                        if (row["Safety Type"].Equals("BASIC"))
                        {
                            category = "SAFETY/BASIC";
                            item = "MIRRORS";
                        }
                        else if (row["Safety Type"].Equals("BUS") || row["Safety Type"].Equals("TRUCK"))
                        {
                            category = "SAFETY/TRUCK";
                            item = "HEADLIGHT AIM";
                        }
                        else if (row["Safety Type"].Equals("MOTORCYCLE"))
                        {
                            category = "SAFETY/MOTORCYCLE";
                            item = "CHECKED EXHAUST";
                        }
                    }
                    if (col.ColumnName.Equals("CODE69"))
                    {
                        if (row["Safety Type"].Equals("BASIC"))
                        {
                            category = "SAFETY/BASIC";
                            item = "DEFROSTER";
                        }
                        else if (row["Safety Type"].Equals("BUS") || row["Safety Type"].Equals("TRUCK"))
                        {
                            category = "SAFETY/TRUCK";
                            item = "MIRRORS";
                        }
                        else if (row["Safety Type"].Equals("MOTORCYCLE"))
                        {
                            category = "SAFETY/MOTORCYCLE";
                            item = "CHECKED FUEL SYSTEM";
                        }
                    }
                    if (col.ColumnName.Equals("CODE70"))
                    {
                        if (row["Safety Type"].Equals("BASIC"))
                        {
                            category = "SAFETY/BASIC";
                            item = "GLASS";
                        }
                        else if (row["Safety Type"].Equals("BUS") || row["Safety Type"].Equals("TRUCK"))
                        {
                            category = "SAFETY/TRUCK";
                            item = "DEFROSTER";
                        }
                        else if (row["Safety Type"].Equals("MOTORCYCLE"))
                        {
                            category = "SAFETY/MOTORCYCLE";
                            item = "CHECKED BODY/CHASSIS/OTHER";
                        }
                    }
                    if (col.ColumnName.Equals("CODE71"))
                    {
                        if (row["Safety Type"].Equals("BASIC"))
                        {
                            category = "SAFETY/BASIC";
                            item = "WIPERS";
                        }
                        else if (row["Safety Type"].Equals("BUS") || row["Safety Type"].Equals("TRUCK"))
                        {
                            category = "SAFETY/TRUCK";
                            item = "GLASS";
                        }
                    }
                    if (col.ColumnName.Equals("CODE72"))
                    {
                        if (row["Safety Type"].Equals("BASIC"))
                        {
                            category = "SAFETY/BASIC";
                            item = "EXHAUST";
                        }
                        else if (row["Safety Type"].Equals("BUS") || row["Safety Type"].Equals("TRUCK"))
                        {
                            category = "SAFETY/TRUCK";
                            item = "WIPERS";
                        }
                    }
                    if (col.ColumnName.Equals("CODE73"))
                    {
                        if (row["Safety Type"].Equals("BASIC"))
                        {
                            category = "SAFETY/BASIC";
                            item = "FUEL SYSTEM";
                        }
                        else if (row["Safety Type"].Equals("BUS") || row["Safety Type"].Equals("TRUCK"))
                        {
                            category = "SAFETY/TRUCK";
                            item = "REFLECTIVE WARNING DEVICE";
                        }
                    }
                    if (col.ColumnName.Equals("CODE74"))
                    {
                        if (row["Safety Type"].Equals("BASIC"))
                        {
                            category = "SAFETY/BASIC";
                            item = "BUMPERS";
                        }
                        else if (row["Safety Type"].Equals("BUS") || row["Safety Type"].Equals("TRUCK"))
                        {
                            category = "SAFETY/TRUCK";
                            item = "FIRE EXTINGUISHER";
                        }
                    }
                    if (col.ColumnName.Equals("CODE75"))
                    {
                        if (row["Safety Type"].Equals("BASIC"))
                        {
                            category = "SAFETY/BASIC";
                            item = "BODY/CHASSIS";
                        }
                        else if (row["Safety Type"].Equals("BUS") || row["Safety Type"].Equals("TRUCK"))
                        {
                            category = "SAFETY/TRUCK";
                            item = "EXHAUST";
                        }
                    }
                    if (col.ColumnName.Equals("CODE76"))
                    {
                        if (row["Safety Type"].Equals("BUS") || row["Safety Type"].Equals("TRUCK"))
                        {
                            category = "SAFETY/TRUCK";
                            item = "FUEL SYSTEM";
                        }
                    }
                    if (col.ColumnName.Equals("CODE77"))
                    {
                        if (row["Safety Type"].Equals("BUS") || row["Safety Type"].Equals("TRUCK"))
                        {
                            category = "SAFETY/TRUCK";
                            item = "BUMPERS";
                        }
                    }
                    if (col.ColumnName.Equals("CODE78"))
                    {
                        if (row["Safety Type"].Equals("BUS") || row["Safety Type"].Equals("TRUCK"))
                        {
                            category = "SAFETY/TRUCK";
                            item = "BODY/CHASSIS";
                        }
                    }
                    if (col.ColumnName.Equals("CODE79"))
                    {
                        if (row["Safety Type"].Equals("BUS") || row["Safety Type"].Equals("TRUCK"))
                        {
                            category = "SAFETY/TRUCK";
                            item = "BUS BODY";
                        }
                    }
                    if (col.ColumnName.Equals("CODE80"))
                    {
                        if (row["Safety Type"].Equals("BUS") || row["Safety Type"].Equals("TRUCK"))
                        {
                            category = "SAFETY/TRUCK";
                            item = "BUS INTERIOR";
                        }
                    }
                    #endregion
                    if (!String.IsNullOrEmpty(category))
                    {
                        html += "<tr>";
                        html += "<td colspan=\"2\" class=\"detail_red\">";
                        html += category;
                        html += "</td>";
                        html += "<td colspan=\"2\" class=\"detail_red\">";
                        html += item;
                        html += "</td>";
                        html += "</tr>";
                        System.Diagnostics.Debug.WriteLine("code with value 2 found");
                    }

                }
            }

            html += "</table></div>";

            return html;

        }

        private void ConvertReportColumns()
        {
            if (Master.UserReport != null)
            {
                foreach (ReportRow row in Master.UserReport.Rows)
                {
                    try
                    {
                        // Convert fields common to all SQL
                        if (!String.IsNullOrEmpty(row["Reschedule Date"].Value))
                        {
                            String strRescheduledDate = row["Reschedule Date"].Value.Trim();
                            if (strRescheduledDate == "00000000")
                            {
                                row["Reschedule Date"].Value = "None";
                            }
                            else
                            {
                                DateTime dateRescheduledDate = DateTime.ParseExact(strRescheduledDate, "yyyyMMdd", CultureInfo.InvariantCulture);
                                row["Reschedule Date"].Value = dateRescheduledDate.ToShortDateString().ToString().Trim();
                            }
                        }
                        string strPriorAuditDT, x;
                        strPriorAuditDT = row["Date"].Value;

                        if (NHPortal.Classes.NHPortalUtilities.TryConvertDateTimeFormat(strPriorAuditDT, "yyyyMMddHHmmss", "M/d/yyyy hh:mm:ss tt", out x))
                        {
                            string msg = String.Format("Converted {0} to {1}", strPriorAuditDT, x);
                            System.Diagnostics.Debug.WriteLine(msg);
                            row["Date"].Value = x;
                        }
                        if (!String.IsNullOrEmpty(row["Overall Result"].Value))
                        {
                            row["Overall Result"].Value = Classes.Reference.AuditResultTypes.GetDescription(row["Overall Result"].Value[0]);
                        }

                        if (!String.IsNullOrEmpty(row["Station Audit Type"].Value))
                        {
                            String strStationAuditType = row["Station Audit Type"].Value.Trim();
                            switch (strStationAuditType)
                            {
                                case "A":
                                    strStationAuditType = "AUTOMOBILE";
                                    break;
                                case "M":
                                    strStationAuditType = "MOTORCYCLE";
                                    break;
                                case "I":
                                    strStationAuditType = "MOTORCYCLE";
                                    break;
                                case "F":
                                    strStationAuditType = "FLEET";
                                    break;
                                case "U":
                                    strStationAuditType = "MUNICIPLE";
                                    break;
                                default:
                                    strStationAuditType = "N/A";
                                    break;
                            }
                            row["Station Audit Type"].Value = strStationAuditType.ToString().Trim();
                        }

                            if (row["Station ID/ Mechanic OLN"].Value.Length <= 8)
                            {
                                row["Station ID/ Mechanic OLN"].Value = row["Station ID/ Mechanic OLN"].ToString().Remove(0, 4);
                            }
                        if(row["Officer ID"].Value.Length >= 3)
                        {
                            row["Officer ID"].Value = row["Officer ID"].ToString().Remove(0, 6);
                        }

                        row["NOTES"].Value = row["NOTES"].ToString().Trim();

                        Master.UserReport.Columns["Station Audit Type"].Text = "Station Audit Type";
                    }
                    catch (FormatException ex)
                    {
                        LogException(ex);
                        LogMessage("Error in ConvertReportColumns(): " + ex, LogSeverity.Information);
                    }
                }
            }
        }
        //main table data
        private string BuildQuery()
        {
            StringBuilder sb = new StringBuilder();

            if (cboInquiryType.SelectedValue.ToString() == "S")
            {
                sb.AppendLine("SELECT Concat(\"DATE\", TIME) AS \"Date\"");
                sb.AppendLine(", STATIONID as \"Station ID/ Mechanic OLN\"");
                sb.AppendLine(", AUDITORID as \"Officer ID\"");
                sb.AppendLine(", 'STATION' AS \"Audit Type\"");
                sb.AppendLine(", REASON as \"Station Audit Type\"");
                sb.AppendLine(", RESULT as \"Overall Result\"");
                sb.AppendLine(", RESCHEDULEDATE as \"Reschedule Date\"");
                sb.AppendLine(", NOTES as \"Notes\"");
                sb.AppendLine("FROM STATIONAUDITS");
                sb.AppendLine("WHERE \"DATE\" >= '" + dateSelector.StartDateControl.GetDateText() + "' ");
                sb.AppendLine("AND \"DATE\" <= '" + dateSelector.EndDateControl.GetDateText() + "' ");
                if (m_inputId != "ALL")
                {
                    sb.AppendLine("AND STATIONID = '" + m_inputId + "'");
                }
                sb.AppendLine("ORDER BY \"DATE\" DESC");
            }
            if (cboInquiryType.SelectedValue.ToString() == "O")
            {
                sb.AppendLine("SELECT Concat(\"DATE\", TIME) AS \"Date\"");
                sb.AppendLine(", INSPECTORID  AS \"Station ID/ Mechanic OLN\"");
                sb.AppendLine(", AUDITORID as \"Officer ID\"");
                sb.AppendLine(", 'MECHANIC' AS \"Audit Type\"");
                sb.AppendLine(", REASON as \"Station Audit Type\"");
                sb.AppendLine(", RESULT as \"Overall Result\"");
                sb.AppendLine(", RESCHEDULEDATE as \"Reschedule Date\"");
                sb.AppendLine(", NOTES as \"Notes\"");
                sb.AppendLine("FROM INSPECTORAUDITS");
                sb.AppendLine("WHERE AUDITORID = '" + m_inputId + "'");
                sb.AppendLine("AND \"DATE\" >= '" + dateSelector.StartDateControl.GetDateText() + "' ");
                sb.AppendLine("AND \"DATE\" <= '" + dateSelector.EndDateControl.GetDateText() + "' ");
                sb.AppendLine("UNION ");
                sb.AppendLine("SELECT Concat(\"DATE\", TIME) AS \"Date\"");
                sb.AppendLine(", STATIONID  AS \"Station ID/ Mechanic OLN\"");
                sb.AppendLine(", AUDITORID as \"Officer ID\"");
                sb.AppendLine(", 'STATION' as \"Audit Type\"");
                sb.AppendLine(", REASON as \"Station Audit Type\"");
                sb.AppendLine(", RESULT as \"Overall Result\"");
                sb.AppendLine(", RESCHEDULEDATE as \"Reschedule Date\"");
                sb.AppendLine(", NOTES as \"Notes\"");
                sb.AppendLine("FROM STATIONAUDITS");
                sb.AppendLine("WHERE \"DATE\" >= '" + dateSelector.StartDateControl.GetDateText() + "' ");
                sb.AppendLine("AND \"DATE\" <= '" + dateSelector.EndDateControl.GetDateText() + "' ");
                if (m_inputId != "ALL")
                {
                    sb.AppendLine("AND UPPER(AUDITORID) = '" + m_inputId + "'");
                }

                sb.AppendLine("ORDER BY \"Date\" DESC");
            }
            if (cboInquiryType.SelectedValue.ToString() == "R")
            {
                sb.AppendLine("SELECT Concat(\"DATE\", TIME) AS \"Date\"");
                sb.AppendLine(", INSPECTORID  AS \"Station ID/ Mechanic OLN\"");
                sb.AppendLine(", AUDITORID as \"Officer ID\"");
                sb.AppendLine(", 'MECHANIC' AS \"Audit Type\"");
                sb.AppendLine(", REASON as \"Station Audit Type\"");
                sb.AppendLine(", RESULT as \"Overall Result\"");
                sb.AppendLine(", RESCHEDULEDATE as \"Reschedule Date\"");
                sb.AppendLine(", NOTES as \"Notes\"");
                sb.AppendLine("FROM INSPECTORAUDITS");
                //sb.AppendLine("WHERE INSPECTORID = '" + m_inputId + "'");
                sb.AppendLine("WHERE \"DATE\" >= '" + dateSelector.StartDateControl.GetDateText() + "' ");
                sb.AppendLine("AND \"DATE\" <= '" + dateSelector.EndDateControl.GetDateText() + "' ");
                if (m_inputId != "ALL")
                {
                    sb.AppendLine("      AND INSPECTORID = '" + m_inputId + "'");
                }
                sb.AppendLine("ORDER BY \"Date\" DESC");
            }
            return sb.ToString();
        }
        //additional details query
        private string BuildDetailsQuery()
        {
            int idx = GDCoreUtilities.NullSafe.ToInt(hidSelectedIndex.Value);
            var row = Master.UserReport.Rows[idx];

            string date, time, redate;
            NHPortalUtilities.TryConvertDateTimeFormat(row[1].Value, "M/d/yyyy h:m:ss tt", "yyyyMMdd", out date);
            NHPortalUtilities.TryConvertDateTimeFormat(row[1].Value, "M/d/yyyy h:m:ss tt", "HHmmss", out time);
            NHPortalUtilities.TryConvertDateTimeFormat(row[7].Value, "M/d/yyyy", "yyyyMMdd", out redate);
            if (row[7].Value == "None" || row[7].Value == "")
            {
                row[7].Value = "00000000";
            }
            else
            {
                if (String.IsNullOrEmpty(redate))
                {
                    redate = row[7].Value;
                }
                else
                {
                    row[7].Value = redate;
                }
                // row[7].Value = redate;
            }

            string stationid = row[2].Value;
            if(stationid.Length <10)
            {
              stationid = "0000" + row[2].Value;
            }
            string audtiorid = "AD0000" + row[3].Value;
            StringBuilder sb = new StringBuilder();
            if (cboInquiryType.SelectedValue.ToString() == "S")
            {
                sb.AppendLine("SELECT \"DATE\" AS \"Date\"");
                sb.AppendLine(", TIME AS \"Time\"");
                sb.AppendLine(", 'STATION' AS \"Audit Type\"");
                sb.AppendLine(", REASON as \"Station Audit Type\"");
                sb.AppendLine(", 'N/A' AS \"Safety Type\"");
                sb.AppendLine(", 'N/A' AS \"Emission Type\"");
                sb.AppendLine(", STATIONID as \"Station ID\"");
                sb.AppendLine(", 'N/A' AS \"Station Name\"");
                sb.AppendLine(", 'N/A' \"Mechanic OLN\"");
                sb.AppendLine(", 'N/A' AS \"Mechanic Name\"");
                sb.AppendLine(", AUDITSEQ AS \"Audit Seq.\"");
                sb.AppendLine(", RESULT as \"Overall Result\"");
                sb.AppendLine(", RESCHEDULEDATE as \"Reschedule Date\"");
                sb.AppendLine(", AUDITORID as \"Officer ID\"");
                sb.AppendLine(", NOTES as \"Notes\"");
                sb.AppendLine(",        CODE1, CODE2, CODE3, CODE4, CODE5 ");
                sb.AppendLine(",        CODE6, CODE7, CODE8, CODE9, CODE10 ");
                sb.AppendLine(",        CODE11, CODE12, CODE13, CODE14, CODE15 ");
                sb.AppendLine(",        CODE17, CODE18, CODE19, CODE20, CODE21 ");
                sb.AppendLine(",        CODE22, CODE23, CODE24, CODE25, CODE26 ");
                sb.AppendLine(",        CODE27, CODE28 ");
                sb.AppendLine("FROM STATIONAUDITS");
                sb.AppendLine("WHERE STATIONID = '" + stationid + "' ");
                sb.AppendLine("    AND \"DATE\" = '" + date + "' ");
                sb.AppendLine("    AND TIME = '" + time + "' ");
                sb.AppendLine("    AND AUDITORID = '" + audtiorid + "' ");
                sb.AppendLine("      ORDER BY \"DATE\", TIME, REASON, STATIONID, AUDITSEQ, RESULT, RESCHEDULEDATE, AUDITORID, NOTES ");
                sb.AppendLine(",     CODE1, CODE2, CODE3, CODE4, CODE5, CODE6, CODE7, CODE8, CODE9 ");
                sb.AppendLine(",     CODE10, CODE11, CODE12, CODE13, CODE14, CODE15, CODE17, CODE18 ");
                sb.AppendLine(",     CODE19, CODE20, CODE21, CODE22, CODE23, CODE24, CODE25, CODE26, CODE27, CODE28 ");
            }
            if (cboInquiryType.SelectedValue.ToString() == "O")
            {
                sb.AppendLine("SELECT \"DATE\" AS \"Date\"");
                sb.AppendLine(", TIME AS \"Time\"");
                sb.AppendLine(", 'MECHANIC' AS \"Audit Type\"");
                sb.AppendLine(", REASON as \"Station Audit Type\"");
                sb.AppendLine(", 'N/A' AS \"Safety Type\"");
                sb.AppendLine(", 'N/A' AS \"Emission Type\"");
                sb.AppendLine(", STATIONID as \"Station ID\"");
                sb.AppendLine(", 'N/A' AS \"Station Name\"");
                sb.AppendLine(", 'N/A' AS \"Mechanic OLN\"");
                sb.AppendLine(", 'N/A' AS \"Mechanic Name\"");
                sb.AppendLine(", AUDITSEQ AS \"Audit Seq.\"");
                sb.AppendLine(", RESULT as \"Overall Result\"");
                sb.AppendLine(", RESCHEDULEDATE as \"Reschedule Date\"");
                sb.AppendLine(", AUDITORID as \"Officer ID\"");
                sb.AppendLine(", NOTES as \"Notes\"");
                sb.AppendLine(",        CODE1, CODE2, CODE3, CODE4, CODE5 ");
                sb.AppendLine(",        CODE6, CODE7, CODE8, CODE9, CODE10 ");
                sb.AppendLine(",        CODE11, CODE12, CODE13, CODE14, CODE15 ");
                sb.AppendLine(",        CODE17, CODE18, CODE19, CODE20, CODE21 ");
                sb.AppendLine(",        CODE22, CODE23, CODE24, CODE25, CODE26 ");
                sb.AppendLine(",        CODE27, CODE28 ");
                sb.AppendLine("FROM STATIONAUDITS");
                sb.AppendLine("WHERE STATIONID = '" + stationid + "' ");
                sb.AppendLine("    AND \"DATE\" = '" + date + "' ");
                sb.AppendLine("    AND TIME = '" + time + "' ");
                sb.AppendLine("AND RESCHEDULEDATE = '" + row[7].Value + "'");
                sb.AppendLine("    AND AUDITORID = '" + audtiorid + "' ");
                sb.AppendLine("UNION");
                sb.AppendLine("SELECT \"DATE\" AS \"Date\"");
                sb.AppendLine(", TIME AS \"Time\"");
                sb.AppendLine(", 'MECHANIC' AS \"Audit Type\"");
                sb.AppendLine(", REASON as \"Station Audit Type\"");
                sb.AppendLine(", SAFETYTYPE AS \"Safety Type\"");
                sb.AppendLine(", EMISTYPE AS \"Emission Type\"");
                sb.AppendLine(", STATIONID as \"Station ID\"");
                sb.AppendLine(", 'N/A' AS \"Station Name\"");
                sb.AppendLine(", INSPECTORID AS \"Mechanic OLN\"");
                sb.AppendLine(", 'N/A' AS \"Mechanic Name\"");
                sb.AppendLine(", AUDITSEQ AS \"Audit Seq.\"");
                sb.AppendLine(", RESULT as \"Overall Result\"");
                sb.AppendLine(", RESCHEDULEDATE as \"Reschedule Date\"");
                sb.AppendLine(", AUDITORID as \"Officer ID\"");
                sb.AppendLine(", NOTES as \"Notes\"");
                sb.AppendLine(",        CODE1, CODE2, CODE3, CODE4, CODE5 ");
                sb.AppendLine(",        CODE6, CODE7, CODE8, CODE9, CODE10 ");
                sb.AppendLine(",        CODE11, CODE12, CODE13, CODE14, CODE15 ");
                sb.AppendLine(",        CODE17, CODE18, CODE19, CODE20, CODE21 ");
                sb.AppendLine(",        CODE22, CODE23, CODE24, CODE25, CODE26 ");
                sb.AppendLine(",        CODE27, CODE28 ");
                sb.AppendLine("FROM INSPECTORAUDITS");
                sb.AppendLine("WHERE (INSPECTORID = '" + stationid + "' OR STATIONID = '" + stationid + "')");
                sb.AppendLine("    AND \"DATE\" = '" + date + "' ");
                sb.AppendLine("    AND TIME = '" + time + "' ");
                sb.AppendLine("AND RESCHEDULEDATE = '" + row[7].Value + "'");
                sb.AppendLine("    AND AUDITORID = '" + audtiorid + "' ");
            }
            if (cboInquiryType.SelectedValue.ToString() == "R")
            {
                sb.AppendLine("SELECT \"DATE\" AS \"Date\"");
                sb.AppendLine(", TIME AS \"Time\"");
                sb.AppendLine(", 'MECHANIC' AS \"Audit Type\"");
                sb.AppendLine(", REASON as \"Station Audit Type\"");
                sb.AppendLine(", SAFETYTYPE AS \"Safety Type\"");
                sb.AppendLine(", EMISTYPE AS \"Emission Type\"");
                sb.AppendLine(", STATIONID as \"Station ID\"");
                sb.AppendLine(", 'N/A' AS \"Station Name\"");
                sb.AppendLine(", INSPECTORID as \"Mechanic OLN\"");
                sb.AppendLine(", 'N/A' AS \"Mechanic Name\"");
                sb.AppendLine(", AUDITSEQ AS \"Audit Seq.\"");
                sb.AppendLine(", RESULT as \"Overall Result\"");
                sb.AppendLine(", RESCHEDULEDATE as \"Reschedule Date\"");
                sb.AppendLine(", AUDITORID as \"Officer ID\"");
                sb.AppendLine(", NOTES as \"Notes\"");
                sb.AppendLine(",        CODE1, CODE2, CODE3, CODE4, CODE5, CODE6, CODE7, CODE51, CODE52, CODE53, CODE54 ");
                sb.AppendLine(",        CODE55, CODE56, CODE57, CODE58, CODE59, CODE60, CODE61, CODE62");
                sb.AppendLine(",        CODE63, CODE64, CODE65, CODE66, CODE67, CODE68, CODE69, CODE70");
                sb.AppendLine(",        CODE71, CODE72, CODE73, CODE74, CODE75, CODE76, CODE77, CODE78");
                sb.AppendLine(",        CODE22, CODE23, CODE24, CODE25, CODE26 ");
                sb.AppendLine(",        CODE79, CODE80 ");
                sb.AppendLine("FROM INSPECTORAUDITS");
                sb.AppendLine("WHERE INSPECTORID = '" + row[2].Value + "' ");
                sb.AppendLine("    AND \"DATE\" = '" + date + "' ");
                sb.AppendLine("    AND TIME = '" + time + "' ");
                sb.AppendLine("    AND AUDITORID = '" + audtiorid + "' ");
                sb.AppendLine("      ORDER BY \"DATE\", TIME, REASON, STATIONID, AUDITSEQ, RESULT, RESCHEDULEDATE, AUDITORID, NOTES ");
                sb.AppendLine(",     CODE1, CODE2, CODE3, CODE4, CODE5, CODE6, CODE7, CODE51, CODE52, CODE53, CODE54 ");
                sb.AppendLine(",        CODE55, CODE56, CODE57, CODE58, CODE59, CODE60, CODE61, CODE62");
                sb.AppendLine(",        CODE63, CODE64, CODE65, CODE66, CODE67, CODE68, CODE69, CODE70");
                sb.AppendLine(",        CODE71, CODE72, CODE73, CODE74, CODE75, CODE76, CODE77, CODE78");
                sb.AppendLine(",        CODE22, CODE23, CODE24, CODE25, CODE26 ");
                sb.AppendLine(",        CODE79, CODE80 ");
            }
            return sb.ToString();
        }

        //add SELECT button to display additional data
        private void AddDetailSelectRow()
        {
            int i = 0;
            foreach (ReportRow row in Master.UserReport.Rows)
            {
                row[0].Value = "Select";
                row[0].OnClick = String.Format("javascript: selectRow({0})" , i);
                row[0].Href = "#/" + row[i] + "_Select";
                row[0].AddClass("popup_class");
                i += 1;
            }
        }

        private void SetMetaData()
        {
            if (Master.UserReport != null)
            {
                Master.UserReport.MetaData.Add("Start Date", dateSelector.StartDateControl.Text);
                Master.UserReport.MetaData.Add("End Date", dateSelector.EndDateControl.Text);

                if (!String.IsNullOrEmpty(cboInquiryType.SelectedValue))
                {
                    Master.UserReport.MetaData.Add("Inquiry Type", cboInquiryType.SelectedItem.Text, 
                        cboInquiryType.SelectedValue);
                }
                Master.UserReport.MetaData.Add("ID/OLN", m_inputId);
            }
        }
    }
}

