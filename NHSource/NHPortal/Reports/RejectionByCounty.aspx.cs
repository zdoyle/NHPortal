using NHPortal.Classes;
using NHPortal.Classes.User;
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
    public partial class RejectionByCounty : PortalPage
    {
        private BaseReport ReportData = BaseReportMaster.RejectionByCounty;
        PredefinedQueryType queryType = PredefinedQueryType.County;

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
            string filterText = GetFiltetText();
            string sql = PredefinedQuerySQL.GetPredefinedSQL(queryType, filterText);

            GDDatabaseClient.Oracle.OracleResponse response = ODAP.GetDataTable(sql, ReportData.DatabaseTarget);

            if (response.Successful)
            {
                Report report = new Report(ReportData.ReportTitle);
                report.Sortable = true;
                Master.UserReport = report;
                SetMetaData(filterText);

                if (response.HasResults)
                {
                    AddColumnsToReport(report);

                    foreach (DataRow dRow in response.ResultsTable.Rows)
                    {
                        report.Rows.Add(dRow);
                    }
                }
            }
            else
            {
                Master.UserReport = null;
                Master.SetError(response.Exception);
            }

            LogOracleResponse(response);
            Master.RenderReportToPage();
            SessionHelper.SetCurrentReport(this.Session, Master.UserReport);
        }

        private string GetFiltetText()
        {
            string returnVal = String.Empty;
            int itemCount = this.lstCounty.SelectedValues.Count();

            if (!this.lstCounty.AllSelected && itemCount > 0)
            {
                if (itemCount == 1)
                {
                    returnVal = this.lstCounty.SelectedValue;
                }
                else
                {
                    List<string> modelYears = new List<string>();

                    foreach (string year in this.lstCounty.SelectedValues)
                    {
                        modelYears.Add("'" + year + "'");
                    }

                    returnVal = String.Join(",", modelYears);
                }
            }

            return returnVal;
        }

        private void AddColumnsToReport(Report report)
        {
            foreach (KeyValuePair<string, ReportColumnInfo> kvp in ReportData.ReportColumnData)
            {
                report.Columns.Add(kvp.Value.DisplayName, kvp.Value.DisplayName, kvp.Value.ColumnDataType);
            }

            foreach (KeyValuePair<string, ReportColumnInfo> kvp in PredefinedQueryReportHeader.MainColumns)
            {
                report.Columns.Add(kvp.Value.DisplayName, kvp.Value.DisplayName, kvp.Value.ColumnDataType);
            }
        }

        private void SetMetaData(string filterText)
        {
            if (Master.UserReport != null)
            {
                string metaDisplayName;

                if (filterText != String.Empty)
                {
                    if (this.lstCounty.SelectedValues.Count() > 1)
                    {
                        metaDisplayName = "Counties";
                    }
                    else
                    {
                        metaDisplayName = "County";
                    }

                    Master.UserReport.MetaData.Add(metaDisplayName, lstCounty.GetDelimitedText(", "));
                }
            }
        }

        private void InitPage()
        {
            Master.SetHeaderText(ReportData.ReportTitle);
            lstCounty.Initialize();
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
                        case "COUNTY":
                        case "COUNTIES":
                            lstCounty.SetSelectedValues(c.Value.Split(new char[] { ',' }));
                            break;
                    }
                }
            }
        }
    }
}