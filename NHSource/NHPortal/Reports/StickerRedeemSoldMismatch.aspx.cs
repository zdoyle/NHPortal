using NHPortal.Classes;
using PortalFramework.ReportModel;
using System;
using System.Globalization;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PortalFramework.Database;

namespace NHPortal.Reports
{
    public partial class StickerRedeemSoldMismatch : PortalPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                ProcessPostBack();
            }
            else
            {
                // This report has no postback
                InitializePage();
                RunReport();
                Master.SetButtonVisibility(MasterPages.ReportButton.Run, false);
            }
        }

        private void InitializePage()
        {

        }

        private void ProcessPostBack()
        {
            string action = Master.MasterPage.HidActionValue;
            switch (action)
            {
                case "RUN_REPORT":
                    RunReport();
                    break;
            }
        }

        private void RunReport()
        {
            string qry = BuildQuery();
            GDDatabaseClient.Oracle.OracleResponse response = ODAP.GetDataTable(qry, DatabaseTarget.OLTP);
            if (response.Successful)
            {
                Master.UserReport = new Report("Redeem/Sold Mismatch", response.ResultsTable);
                ConvertDateSoldColumn();
                ConvertRedeemDateColumn();
                Master.UserReport.FooterNote = "* Percentages apply to columns." + Environment.NewLine
                                  + "** Total Tests includes Completed tests, Aborted tests and Administrative Certificates." + Environment.NewLine
                                  + "*** Aborts and Administrative Certificates are not included in Inspection Results." + Environment.NewLine
                                  + "Please reference the document entitled Notes for Interpreting Report Data for further information.";
            }

            LogOracleResponse(response);
            Master.RenderReportToPage();
            SessionHelper.SetCurrentReport(this.Session, Master.UserReport);
        }

        private string BuildQuery()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("   SELECT ");
            sb.AppendLine("           STICKERNUMBER AS \"Sticker Number\"");
            sb.AppendLine(",          STICKERSERIES AS \"Sticker Series\"");
            sb.AppendLine(",          STATIONID AS \"Station\"");
            sb.AppendLine(",          DATESOLD AS \"Date Sold\"");
            sb.AppendLine(",          REDEEMEDSTATION AS \"Redeemed Station\"");
            sb.AppendLine(",          DATEREDEEMED AS \"Redeemed Date\"");
            sb.AppendLine("     FROM STICKERINVENTORYTBL ");
            sb.AppendLine("    WHERE DATEREDEEMED >= TO_CHAR(ADD_MONTHS(SYSDATE,-1),'YYYYMMDD')");
            sb.AppendLine("      AND REDEEMEDSTATION <> STATIONID");
            sb.AppendLine(" ORDER BY DATEREDEEMED DESC");
            return sb.ToString();
        }

        private void ConvertDateSoldColumn()
        {
            if (Master.UserReport != null)
            {
                foreach (ReportRow row in Master.UserReport.Rows)
                {
                    try
                    {
                        if (!String.IsNullOrEmpty(row["Date Sold"].Value))
                        {
                            String strDateSold = row["Date Sold"].Value;
                            DateTime dateSoldDate = DateTime.ParseExact(strDateSold,"yyyyMMdd",CultureInfo.InvariantCulture);
                            row["Date Sold"].Value = dateSoldDate.ToShortDateString().ToString();
                        }
                    }
                    catch (FormatException ex)
                    {
                        LogException(ex);
                    }
                }
            }
        }

        private void ConvertRedeemDateColumn()
        {
            if (Master.UserReport != null)
            {
                foreach (ReportRow row in Master.UserReport.Rows)
                {
                    try
                    {
                        if (!String.IsNullOrEmpty(row["Redeemed Date"].Value))
                        {
                            String strRedeemedDate = row["Redeemed Date"].Value;
                            DateTime dateRedeemedDate = DateTime.ParseExact(strRedeemedDate, "yyyyMMdd", CultureInfo.InvariantCulture);
                            row["Redeemed Date"].Value = dateRedeemedDate.ToShortDateString().ToString();
                        }
                    }
                    catch (FormatException ex)
                    {
                        LogException(ex);
                    }
                }
            }
        }
    }
}