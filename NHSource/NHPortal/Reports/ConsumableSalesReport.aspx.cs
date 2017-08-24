using GDCoreUtilities.Logging;
using NHPortal.Classes;
using NHPortal.Classes.User;
using PortalFramework.ReportModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NHPortal.Reports
{
    public partial class ConsumableSalesReport : NHPortal.Classes.PortalPage
    {
        public const string ReportTitle = "Consumable Sales Report";


        protected void Page_Load(object sender, EventArgs e)
        {
            RedirectOnInvalidPermission(UserPermissions.ConsumableSales);

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
            Master.SetHeaderText(ReportTitle);
            this.dateSelector.StartDateControl.Text = DateTime.Now.AddMonths(-1).ToString("M/d/yyyy");
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
            Master.SaveFavorite(NHPortal.UserControls.RedirectCodes.REPORT_CONSUMABLE_SALES, UserFavoriteTypes.Report);
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

            }
        }

        private void RunReport()
        {
            ReportFromSQL();
            //ReportFromCSV();
            SetMetaData();
            
            
            Master.RenderReportToPage();
            SessionHelper.SetCurrentReport(this.Session, Master.UserReport);
        }

        private void ReportFromSQL()
        {
            string qry = BuildQuery();
            GDDatabaseClient.Oracle.OracleResponse response = PortalFramework.Database.ODAP.GetDataTable(qry, PortalFramework.Database.DatabaseTarget.OLTP);
            LogMessage("Attempting to run query...", LogSeverity.Information);
           
            if (response.Successful)
            {
                LogMessage("Query is successful", LogSeverity.Information);
                Master.UserReport = new Report(ReportTitle, response.ResultsTable);
                Master.SetDataLastUpdate(DateTime.Now);

                if (response.HasResults)
                {
                    LogMessage("Query has results", LogSeverity.Information);
                    ConvertReportColumns();
                   // SetColumnTypes();
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
            LogMessage("Page Loaded", LogSeverity.Information);
        }

        private string BuildQuery()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Select u.StationID as \"Station ID\",");
            sb.AppendLine("SA.UnitID as \"Unit ID\", ");
            sb.AppendLine("typ.TypeID  as \"Type ID\", ");
            sb.AppendLine("typ.Description  as \"Description\", ");
            sb.AppendLine("SA.ACTIVITYNUMBER || '-' || SA.ACTIVITYATTEMPT as \"Order Number\",");
            sb.AppendLine("AD.QUANTITY as \"Quantity Required\",");
            sb.AppendLine("AD.QUANTITY as \"Quantity Shipped\",");
            //sb.AppendLine("'null' as \"Quantity Shipped\",");
            //--sum(case when ad.FROMID LIKE '%GD%SALE%' then 1 else 0 end) as "Quantity Shipped", ");
            sb.AppendLine("SA.shiptrackingnum  as \"Tracking Number\",");
            sb.AppendLine("SA.shipcost as \"Shiping Cost\"");
            sb.AppendLine("FROM  MANUFACTURER man");
            sb.AppendLine("left join ACTIVITYDETAIL ad on ad.typeid = man.typeid");
            sb.AppendLine("left JOIN SUPPORTACTIVITY SA ON SA.ACTIVITYNUMBER = AD.ACTIVITYNUMBER AND SA.ACTIVITYATTEMPT = AD.ACTIVITYATTEMPT ");
            sb.AppendLine("left join TYPEID typ on man.TYPEID = typ.TYPEID");
            sb.AppendLine("left join unit u on u.unitid = sa.unitid");
            sb.AppendLine("WHERE ad.\"DATE\"  >= '" + dateSelector.StartDateControl.GetDateText() + "' ");
            sb.AppendLine("AND ad.\"DATE\"  <= '" + dateSelector.EndDateControl.GetDateText() + "' ");
            sb.AppendLine("group by stationid, sa.UnitID, typ.TypeID, typ.Description,  SA.ACTIVITYNUMBER, Sa.ACTIVITYATTEMPT,");
            sb.AppendLine("ad.QUANTITY,SA.shiptrackingnum,SA.shipcost");
            return sb.ToString();
        }
        //set column types so export can read them as a number 
        private void SetColumnTypes()
        {
            for (int i = 5; i < Master.UserReport.ColumnCount; i++)
            {
                Master.UserReport.Columns[i].ColumnDataType = ColumnDataType.Number;
            }
        }


        private void ConvertReportColumns()
        {
            if (Master.UserReport != null)
            {
                foreach (ReportRow row in Master.UserReport.Rows)
                {
                    try
                    {

                        double shippingcost = (((double)Convert.ToInt64(row["Shiping Cost"].ToString())) / 100.00);

                        string shipcost = "$" + String.Format(CultureInfo.InvariantCulture, "{0:0.00}", shippingcost);//
                        //12,314.23shippingcost.ToString().Trim();

                        row["Shiping Cost"].Value = shipcost;
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