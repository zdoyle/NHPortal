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

namespace NHPortal
{
    public partial class InqMobileTestByVIN : NHPortal.Classes.PortalPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RedirectOnInvalidPermission(UserPermissions.MobileTestingTabletByVIN);
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
            Master.SetHeaderText("Mobile Testing Tablet Report Inquiries By VIN");
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

        private void RunReport()
        {
            string qry = BuildQuery();
            GDDatabaseClient.Oracle.OracleResponse response = ODAP.GetDataTable(qry, DatabaseTarget.OLTP);
            if (response.Successful)
            {
                LogMessage("Response Successful", LogSeverity.Information);
                Master.UserReport = new Report("Mobile Testing Tablet Report Inquiries By VIN", response.ResultsTable);
                SetMetaData();
                Master.SetDataLastUpdate(DateTime.Now);
                if (response.HasResults)
                {
                    LogMessage("Has Results", LogSeverity.Information);
                    CalculateReportValues();
                    SetColumnTypes();
                    Master.UserReport.Sortable = true;
                    Master.UserReport.FooterVisibility = FooterVisibility.Auto;
                }
            }
            else
            {
                LogMessage("Response Unsuccessful", LogSeverity.Information);
                Master.UserReport = null;
                Master.SetError(response.Exception);
            }

            LogOracleResponse(response);
            Master.RenderReportToPage();
            SessionHelper.SetCurrentReport(this.Session, Master.UserReport);
        }

        private void SetMetaData()
        {
            if (Master.UserReport != null)
            {
                Master.UserReport.MetaData.Add("VIN", tbVIN.Text);
            }
        }

        private string BuildQuery()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("   SELECT ");
            sb.AppendLine("          T.TESTDATE || T.TESTENDTIME AS \"Test Date\" ");
            sb.AppendLine(",         T.VIN AS \"VIN\" ");
            sb.AppendLine(",         ' ' AS \"EVIN\" ");
            sb.AppendLine(",         T.TESTASMAKE AS \"Make\" ");
            sb.AppendLine(",         T.TESTASMODEL AS \"Model\" ");
            sb.AppendLine(",         T.TESTASMODELYR AS \"Model Year\" ");
            sb.AppendLine(",         O.RPM AS \"RPM\" ");
            sb.AppendLine(",         O.VOLTAGE AS \"Voltage\" ");
            sb.AppendLine(",         O.PROTOCOLID AS \"Test Protocol\" ");
            sb.AppendLine(",         PV.PROTOCOLID AS \"Expected Protocol\" ");
            sb.AppendLine(",         CASE WHEN T.OBDMISFIRE IN ('1','2') THEN 'S' ");
            sb.AppendLine("               WHEN T.OBDMISFIRE = '4' THEN 'U' END||  ");
            sb.AppendLine("          CASE WHEN T.OBDFUELSYSTEM IN ('1','2') THEN 'S'  ");
            sb.AppendLine("               WHEN T.OBDFUELSYSTEM = '4' THEN 'U' END||  ");
            sb.AppendLine("          CASE WHEN T.OBDCOMPONENT IN ('1','2') THEN 'S'  ");
            sb.AppendLine("               WHEN T.OBDCOMPONENT = '4' THEN 'U' END||  ");
            sb.AppendLine("          CASE WHEN T.OBDCATALYST IN ('1','2') THEN 'S'  ");
            sb.AppendLine("               WHEN T.OBDCATALYST = '4' THEN 'U' END||  ");
            sb.AppendLine("          CASE WHEN T.OBDHEATEDCATALYST IN ('1','2') THEN 'S'  ");
            sb.AppendLine("               WHEN T.OBDHEATEDCATALYST = '4' THEN 'U' END||  ");
            sb.AppendLine("          CASE WHEN T.OBDEVAP IN ('1','2') THEN 'S'  ");
            sb.AppendLine("               WHEN T.OBDEVAP = '4' THEN 'U' END||  ");
            sb.AppendLine("          CASE WHEN T.OBDAIRSYSTEM IN ('1','2') THEN 'S'  ");
            sb.AppendLine("               WHEN T.OBDAIRSYSTEM = '4' THEN 'U' END||  ");
            sb.AppendLine("          CASE WHEN T.OBDAC IN ('1','2') THEN 'S'  ");
            sb.AppendLine("               WHEN T.OBDAC = '4' THEN 'U' END||  ");
            sb.AppendLine("          CASE WHEN T.OBDOXYGEN IN ('1','2') THEN 'S'  ");
            sb.AppendLine("               WHEN T.OBDOXYGEN = '4' THEN 'U' END||  ");
            sb.AppendLine("          CASE WHEN T.OBDHEATEDOXYGEN IN ('1','2') THEN 'S'  ");
            sb.AppendLine("               WHEN T.OBDHEATEDOXYGEN = '4' THEN 'U' END||  ");
            sb.AppendLine("          CASE WHEN T.OBDEGR IN ('1','2') THEN 'S'  ");
            sb.AppendLine("               WHEN T.OBDEGR = '4' THEN 'U' END AS \"Test Readiness Monitor\" ");
            sb.AppendLine(",         PR.READINESSPATTERN AS \"Expected Readiness Monitor\" ");
            sb.AppendLine(",         S.COUNTY AS \"County\" ");
            sb.AppendLine(",         T.STATIONID AS \"Station Id\" ");
            sb.AppendLine(",         T.INSPECTORID AS \"Inspector Id\" ");
            sb.AppendLine(",         T.UNITID AS \"Unit ID\" ");
            sb.AppendLine(",         T.EMISSTESTTYPE AS \"Emission Test Type\" ");
            sb.AppendLine(",         T.OBDPF AS \"OBD Test Results\" ");
            sb.AppendLine(",         T.STICKERTYPE AS \"Sticker Type\" ");
            sb.AppendLine(",         T.STICKERSERIES AS \"Sticker Series\" ");
            sb.AppendLine(",         T.STICKERNUMBER AS \"Sticker Number\" ");
            sb.AppendLine(",         T.NOTES AS \"Notes\" ");
            sb.AppendLine(",         T.OBDLOCATABLE AS \"DLC Locatable\" ");
            sb.AppendLine(",         T.OBDCOMMUNICABLE AS \"Communicable\" ");
            sb.AppendLine(",         T.OBDMILCMDON AS \"MIL Commanded On\" ");
            sb.AppendLine(",         T.OBDMILENGOFF AS \"Ken On Engine Off\" ");
            sb.AppendLine(",         T.OBDMILENGON AS \"Key On Engine Running\" ");
            sb.AppendLine(",         T.OBDMISFIRE AS \"Misfire Monitor\" ");
            sb.AppendLine(",         T.OBDFUELSYSTEM AS \"Fuel System Monitor\" ");
            sb.AppendLine(",         T.OBDCOMPONENT AS \"Component Monitor\" ");
            sb.AppendLine(",         T.OBDCATALYST AS \"Catalyst Monitor\" ");
            sb.AppendLine(",         T.OBDHEATEDCATALYST AS \"Heated Catalyst Monitor\" ");
            sb.AppendLine(",         T.OBDEVAP AS \"Evap Monitor\" ");
            sb.AppendLine(",         T.OBDAIRSYSTEM AS \"Air System Monitor\" ");
            sb.AppendLine(",         T.OBDAC AS \"A/C Monitor\" ");
            sb.AppendLine(",         T.OBDOXYGEN AS \"Oxygen\" ");
            sb.AppendLine(",         T.OBDHEATEDOXYGEN AS \"Heated Oxygen Sensor Monitor\" ");
            sb.AppendLine(",         T.OBDEGR AS \"EGR Monitor\" ");
            sb.AppendLine(",         T.OBDNUMDTCS AS \"Number DTCs\" ");
            sb.AppendLine(",         T.DTC1 AS \"DTC1\" ");
            sb.AppendLine(",         T.DTC2 AS \"DTC2\" ");
            sb.AppendLine(",         T.DTC3 AS \"DTC3\" ");
            sb.AppendLine(",         T.DTC4 AS \"DTC4\" ");
            sb.AppendLine(",         T.DTC5 AS \"DTC5\" ");
            sb.AppendLine(",         T.DTC6 AS \"DTC6\" ");
            sb.AppendLine(",         T.DTC7 AS \"DTC7\" ");
            sb.AppendLine(",         T.DTC8 AS \"DTC8\" ");
            sb.AppendLine(",         T.DTC9 AS \"DTC9\" ");
            sb.AppendLine(",         T.DTC10 AS \"DTC10\" ");
            sb.AppendLine("     FROM TESTRECORD T ");
            sb.AppendLine(" LEFT OUTER JOIN STATION S ON S.STATIONID = T.STATIONID ");
            sb.AppendLine(" LEFT OUTER JOIN OBD_TESTDATA@NH_ADHOC_PRIMARY O ON T.TESTDATE = O.TESTDATE AND T.TESTENDTIME = O.TESTTIME AND T.VIN = O.VIN ");
            sb.AppendLine(" LEFT OUTER JOIN PMV_OBDPROTOCOL_TRUTHTABLE@NH_ADHOC_PRIMARY PV ON PV.VIN10 = O.VIN10 ");
            sb.AppendLine(" LEFT OUTER JOIN PMV_OBDREADINESS_TRUTHTABLE@NH_ADHOC_PRIMARY PR ON PR.VIN10 = O.VIN10 ");
            sb.AppendLine("     WHERE T.VEHICLESEQUENCENUM IN (SELECT V.VEHICLESEQUENCENUMBER ");
            sb.AppendLine("                                     FROM VEHICLEMASTERTBL V ");
            sb.AppendLine("                                    WHERE V.VIN = '" + tbVIN.Text + "') ");
            sb.AppendLine("      AND T.OBDPF NOT IN ('4','9') ");
            sb.AppendLine(" ORDER BY T.TESTDATE DESC, T.TESTENDTIME DESC ");


            //sb.AppendLine("   SELECT ");
            //sb.AppendLine("          T.TESTDATE || T.TESTENDTIME AS \"Test Date\" ");
            //sb.AppendLine(",         T.VIN AS \"VIN\" ");
            //sb.AppendLine(",         ' ' AS \"EVIN\" ");
            //sb.AppendLine(",         T.TESTASMAKE AS \"Make\" ");
            //sb.AppendLine(",         T.TESTASMODEL AS \"Model\" ");
            //sb.AppendLine(",         T.TESTASMODELYR AS \"Model Year\" ");
            //sb.AppendLine(",         O.RPM AS \"RPM\" ");
            //sb.AppendLine(",         O.VOLTAGE AS \"Voltage\" ");
            //sb.AppendLine(",         O.PROTOCOLID AS \"Test Protocol\" ");
            //sb.AppendLine(",         PP.PROTOCOLID AS \"Expected Protocol\" ");
            //sb.AppendLine(",         CASE WHEN T.OBDMISFIRE IN ('1','2') THEN 'S' ");
            //sb.AppendLine("               WHEN T.OBDMISFIRE = '4' THEN 'U' END||  ");
            //sb.AppendLine("          CASE WHEN T.OBDFUELSYSTEM IN ('1','2') THEN 'S'  ");
            //sb.AppendLine("               WHEN T.OBDFUELSYSTEM = '4' THEN 'U' END||  ");
            //sb.AppendLine("          CASE WHEN T.OBDCOMPONENT IN ('1','2') THEN 'S'  ");
            //sb.AppendLine("               WHEN T.OBDCOMPONENT = '4' THEN 'U' END||  ");
            //sb.AppendLine("          CASE WHEN T.OBDCATALYST IN ('1','2') THEN 'S'  ");
            //sb.AppendLine("               WHEN T.OBDCATALYST = '4' THEN 'U' END||  ");
            //sb.AppendLine("          CASE WHEN T.OBDHEATEDCATALYST IN ('1','2') THEN 'S'  ");
            //sb.AppendLine("               WHEN T.OBDHEATEDCATALYST = '4' THEN 'U' END||  ");
            //sb.AppendLine("          CASE WHEN T.OBDEVAP IN ('1','2') THEN 'S'  ");
            //sb.AppendLine("               WHEN T.OBDEVAP = '4' THEN 'U' END||  ");
            //sb.AppendLine("          CASE WHEN T.OBDAIRSYSTEM IN ('1','2') THEN 'S'  ");
            //sb.AppendLine("               WHEN T.OBDAIRSYSTEM = '4' THEN 'U' END||  ");
            //sb.AppendLine("          CASE WHEN T.OBDAC IN ('1','2') THEN 'S'  ");
            //sb.AppendLine("               WHEN T.OBDAC = '4' THEN 'U' END||  ");
            //sb.AppendLine("          CASE WHEN T.OBDOXYGEN IN ('1','2') THEN 'S'  ");
            //sb.AppendLine("               WHEN T.OBDOXYGEN = '4' THEN 'U' END||  ");
            //sb.AppendLine("          CASE WHEN T.OBDHEATEDOXYGEN IN ('1','2') THEN 'S'  ");
            //sb.AppendLine("               WHEN T.OBDHEATEDOXYGEN = '4' THEN 'U' END||  ");
            //sb.AppendLine("          CASE WHEN T.OBDEGR IN ('1','2') THEN 'S'  ");
            //sb.AppendLine("               WHEN T.OBDEGR = '4' THEN 'U' END AS \"Test Readiness Monitor\" ");
            //sb.AppendLine(",         PR.READINESSPATTERN AS \"Expected Readiness Monitor\" ");
            //sb.AppendLine(",         T.COUNTY AS \"County\" ");
            //sb.AppendLine(",         T.STATIONID AS \"Station Id\" ");
            //sb.AppendLine(",         T.INSPECTORID AS \"Inspector Id\" ");
            //sb.AppendLine(",         T.UNITID AS \"Unit ID\" ");
            //sb.AppendLine(",         T.EMISSTESTTYPE AS \"Emission Test Type\" ");
            //sb.AppendLine(",         T.OBDPF AS \"OBD Test Results\" ");
            //sb.AppendLine(",         T.STICKERTYPE AS \"Sticker Type\" ");
            //sb.AppendLine(",         T.STICKERSERIES AS \"Sticker Series\" ");
            //sb.AppendLine(",         T.STICKERNUMBER AS \"Sticker Number\" ");
            //sb.AppendLine(",         T.NOTES AS \"Notes\" ");
            //sb.AppendLine(",         T.OBDLOCATABLE AS \"DLC Locatable\" ");
            //sb.AppendLine(",         T.OBDCOMMUNICABLE AS \"Communicable\" ");
            //sb.AppendLine(",         T.OBDMILCMDON AS \"MIL Commanded On\" ");
            //sb.AppendLine(",         T.OBDMILENGOFF AS \"Ken On Engine Off\" ");
            //sb.AppendLine(",         T.OBDMILENGON AS \"Key On Engine Running\" ");
            //sb.AppendLine(",         T.OBDMISFIRE AS \"Misfire Monitor\" ");
            //sb.AppendLine(",         T.OBDFUELSYSTEM AS \"Fuel System Monitor\" ");
            //sb.AppendLine(",         T.OBDCOMPONENT AS \"Component Monitor\" ");
            //sb.AppendLine(",         T.OBDCATALYST AS \"Catalyst Monitor\" ");
            //sb.AppendLine(",         T.OBDHEATEDCATALYST AS \"Heated Catalyst Monitor\" ");
            //sb.AppendLine(",         T.OBDEVAP AS \"Evap Monitor\" ");
            //sb.AppendLine(",         T.OBDAIRSYSTEM AS \"Air System Monitor\" ");
            //sb.AppendLine(",         T.OBDAC AS \"A/C Monitor\" ");
            //sb.AppendLine(",         T.OBDOXYGEN AS \"Oxygen\" ");
            //sb.AppendLine(",         T.OBDHEATEDOXYGEN AS \"Heated Oxygen Sensor Monitor\" ");
            //sb.AppendLine(",         T.OBDEGR AS \"EGR Monitor\" ");
            //sb.AppendLine(",         T.OBDNUMDTCS AS \"Number DTCs\" ");
            //sb.AppendLine(",         T.DTC1 AS \"DTC1\" ");
            //sb.AppendLine(",         T.DTC2 AS \"DTC2\" ");
            //sb.AppendLine(",         T.DTC3 AS \"DTC3\" ");
            //sb.AppendLine(",         T.DTC4 AS \"DTC4\" ");
            //sb.AppendLine(",         T.DTC5 AS \"DTC5\" ");
            //sb.AppendLine(",         T.DTC6 AS \"DTC6\" ");
            //sb.AppendLine(",         T.DTC7 AS \"DTC7\" ");
            //sb.AppendLine(",         T.DTC8 AS \"DTC8\" ");
            //sb.AppendLine(",         T.DTC9 AS \"DTC9\" ");
            //sb.AppendLine(",         T.DTC10 AS \"DTC10\" ");
            //sb.AppendLine("     FROM NEW_TESTRECORD T ");
            //sb.AppendLine("     LEFT OUTER JOIN OBD_TESTDATA O ON T.TESTDATE = O.TESTDATE AND T.TESTENDTIME = O.TESTTIME AND T.VIN = O.VIN ");
            //sb.AppendLine("     LEFT OUTER JOIN PMV_OBDPROTOCOL_TRUTHTABLE PP ON PP.VIN10 = O.VIN10 ");
            //sb.AppendLine("     LEFT OUTER JOIN PMV_OBDREADINESS_TRUTHTABLE PR ON PR.VIN10 = O.VIN10 ");
            //sb.AppendLine("    WHERE T.VIN = '" + tbVIN.Text + "' ");
            //sb.AppendLine("      AND T.OBDPF NOT IN ('4','9') ");
            //sb.AppendLine(" ORDER BY T.TESTDATE DESC, T.TESTENDTIME DESC ");

            LogMessage("SQL Query:" + sb.ToString(), LogSeverity.Information);
            return sb.ToString();

        }

        private void CalculateReportValues()
        {
            string strDateTimeFormatIn, strDateTimeFormatOut;
            LogMessage("Start CalculateReportValues...", LogSeverity.Information);
            foreach (ReportRow row in Master.UserReport.Rows)
            {
                strDateTimeFormatIn = row["Test Date"].Value;
                if (NHPortal.Classes.NHPortalUtilities.TryConvertDateTimeFormat(strDateTimeFormatIn, "yyyyMMddHHmmss", "M/d/yyyy hh:mm:ss tt", out strDateTimeFormatOut))
                {
                    row["Test Date"].Value = strDateTimeFormatOut;
                }
            }
        }

        private void SaveFavorite()
        {
            Master.SaveFavorite(NHPortal.UserControls.RedirectCodes.REPORT_INQ_MOBILE_TEST_BY_VIN, UserFavoriteTypes.Report);
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
        }
    }
}