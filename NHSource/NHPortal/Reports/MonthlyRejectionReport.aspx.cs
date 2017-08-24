using NHPortal.Classes;
using NHPortal.Classes.User;
using Oracle.DataAccess.Client;
using PortalFramework.Database;
using PortalFramework.ReportModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NHPortal.Reports
{
    public partial class MonthlyRejectionReport : PortalPage
    {
        private BaseReport ReportData = BaseReportMaster.MonthlyRejection;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.IsPostBack)
            {
                ProcessPostBack();
            }
            else
            {
                InitPage();
                LoadFavorite();
                RunReport();
            }
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
            DataTable dt = BaseReportMaster.GetProcedureDataTable("NH_REJ_MONTHLY_REJECTION", GetOracleParameters(), DatabaseTarget.Adhoc);
            ReportRow currentRow;

            if (dt.Rows.Count > 0)
            {
                Report report = new Report(ReportData.ReportTitle);
                report.Sortable = true;
                Master.UserReport = report;
                SetMetaData();

                AddColumnsToReport(report);

                foreach (DataRow dRow in dt.Rows)
                {
                    currentRow = new ReportRow(report, dRow);
                    AddRowColorCoding(currentRow);
                    report.Rows.Add(currentRow);
                }
            }
            else
            {
                Master.UserReport = null;
            }

            Master.RenderReportToPage();
            SessionHelper.SetCurrentReport(this.Session, Master.UserReport);
        }

        private void AddRowColorCoding(ReportRow currentRow)
        {
            FormatRejectionCell(currentRow, "AIRPUMP");
            FormatRejectionCell(currentRow, "PCV");
            FormatRejectionCell(currentRow, "EVAPSYSTEM");
            FormatRejectionCell(currentRow, "FUELCAP");
            FormatRejectionCell(currentRow, "CAT");
            FormatRejectionCell(currentRow, "VISUALRESULT");
            FormatRejectionCell(currentRow, "SAFETYRESULT");
            FormatRejectionCell(currentRow, "VEHINFO");
            FormatRejectionCell(currentRow, "WHEELS");
            FormatRejectionCell(currentRow, "TIRES");
            FormatRejectionCell(currentRow, "STEERINGFRONTEND");
            FormatRejectionCell(currentRow, "FOOTBRK");
            FormatRejectionCell(currentRow, "PARKINGBRK");
            FormatRejectionCell(currentRow, "INSTRUMENTS");
            FormatRejectionCell(currentRow, "HORNELEC");
            FormatRejectionCell(currentRow, "REARLIGHT");
            FormatRejectionCell(currentRow, "STOPLIGHT");
            FormatRejectionCell(currentRow, "FRONTLIGHT");
            FormatRejectionCell(currentRow, "DIRSIGNAL");
            FormatRejectionCell(currentRow, "OTHERLIGHT");
            FormatRejectionCell(currentRow, "HEADLIGHTAIM");
            FormatRejectionCell(currentRow, "MIRROR");
            FormatRejectionCell(currentRow, "DEFROSTER");
            FormatRejectionCell(currentRow, "GLASS");
            FormatRejectionCell(currentRow, "WIPERS");
            FormatRejectionCell(currentRow, "EXHAUST");
            FormatRejectionCell(currentRow, "FUELSYSTEM");
            FormatRejectionCell(currentRow, "BUMPERS");
            FormatRejectionCell(currentRow, "BODYCHASSIS");
        }

        private void FormatRejectionCell(ReportRow currentRow, string columnName)
        {
            if (currentRow[columnName] != null && currentRow[columnName].Value == "2")
            {
                currentRow[columnName].AddClass("red-cell");
            }

            currentRow[columnName].Value = GetRejectionValue(currentRow[columnName].Value);
        }

        private string GetRejectionValue(string code)
        {
            string returnVal;

            switch (code)
            {
                case "1":
                    returnVal = "P";
                    break;
                case "2":
                    returnVal = "R";
                    break;
                case "3":
                    returnVal = "C";
                    break;
                case "4":
                    returnVal = "N/A";
                    break;
                default:
                    returnVal = String.Empty;
                    break;
            }

            return returnVal;
        }

        private OracleParameter[] GetOracleParameters()
        {
            List<OracleParameter> paramList = new List<OracleParameter>();

            paramList.Add(new OracleParameter("StartDate", OracleDbType.Varchar2, 8, this.dpStart.GetDateText(), ParameterDirection.Input));
            paramList.Add(new OracleParameter("EndDate", OracleDbType.Varchar2, 8, this.dpEnd.GetDateText(), ParameterDirection.Input));
            paramList.Add(new OracleParameter("RECORD_DATA", OracleDbType.RefCursor, ParameterDirection.Output));

            return paramList.ToArray();
        }

        private void AddColumnsToReport(Report report)
        {
            foreach (KeyValuePair<string, ReportColumnInfo> kvp in ReportData.ReportColumnData)
            {
                report.Columns.Add(kvp.Key, kvp.Value.DisplayName, kvp.Value.ColumnDataType);
            }
        }

        private void SetMetaData()
        {
            if (Master.UserReport != null)
            {
                Master.UserReport.MetaData.Add("Start Date", this.dpStart.Text);
                Master.UserReport.MetaData.Add("End Date", this.dpEnd.Text);
            }
        }

        private void InitPage()
        {
            Master.SetHeaderText(ReportData.ReportTitle);
            int endDay = DateTime.Now.Day;

            if (endDay > 9)
            {
                endDay = 9;
            }

            DateTime startDate = DateTime.Now.AddMonths(-1);
            startDate = new DateTime(startDate.Year, startDate.Month, 1);
            
            this.dpStart.Text = startDate.ToShortDateString();
            this.dpEnd.Text = startDate.AddMonths(1).AddDays(endDay).ToShortDateString();

        }

        private void SaveFavorite()
        {
            Master.SaveFavorite(ReportData.PageCode, UserFavoriteTypes.Report);
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
                            dpStart.Text = c.Value;
                            break;
                        case "END DATE":
                            dpEnd.Text = c.Value;
                            break;
                    }
                }
            }
        }
    }
}