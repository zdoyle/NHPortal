using NHPortal.Classes;
using NHPortal.Classes.Reports.InspectionReports;
using NHPortal.Classes.User;
using PortalFramework.Database;
using PortalFramework.ReportModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace NHPortal.MasterPages
{
    public partial class InspectionReportMaster : MasterPage
    {
        public IReportData ReportData { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            InitReportType();

            if (IsPostBack)
            {
                ProcessPostBack();
            }
            else
            {
                InitializePage();
                LoadFavorite();

                if (ReportData.BaseReport.RunReportOnInitialLoad && ValidateCriteria())
                {
                    RunReport();
                }
            }
        }

        private void InitializePage()
        {
            lstTestSeq.Initialize();
            lstModelYear.Initialize();
            lstVehicleType.Initialize();
            lstCounty.Initialize();

            if (ReportData is FirstTestResetData)
            {
                this.dateSelector.SelectReportPeriod(1);
            }
        }

        private bool ValidateCriteria()
        {
            string errMsg = String.Empty;
            bool isValid = false;

            if (this.dateSelector.StartDateControl.GetDateText() == String.Empty)
            {
                errMsg += "Please enter a valid start date. <br />";
            }
            if (this.dateSelector.EndDateControl.GetDateText() == String.Empty)
            {
                errMsg += " Please enter a valid end date. ";
            }

            if (errMsg == String.Empty)
            {
                isValid = true;
            }
            else
            {
                Master.SetError(errMsg);
            }

            return isValid;
        }

        private void InitReportType()
        {
            if (ReportData == null || ReportData.BaseReport == null)
            {
                Response.Redirect("~/Welcome.aspx");
            }

            if (ReportData.BaseReport == BaseReportMaster.FirstTestReset)
            {
                this.divMainCriteria.Style["display"] = "none";
            }

            Master.SetHeaderText(ReportData.BaseReport.ReportTitle);
        }

        private void ProcessPostBack()
        {
            string action = Master.MasterPage.HidActionValue;
            switch (action)
            {
                case "RUN_REPORT":
                    if (ValidateCriteria())
                    {
                        RunReport();
                    }
                    break;
                case "SAVE_FAVORITE":
                    SaveFavorite();
                    break;
            }
        }

        private void SaveFavorite()
        {
            Master.SaveFavorite(ReportData.BaseReport.PageCode, UserFavoriteTypes.Report);
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
                        case "COUNTY":
                            lstCounty.SetSelectedTexts(c.Value.Split(new char[] { ',' }));
                            break;
                        case "MODEL YEAR":
                            lstModelYear.SetSelectedTexts(c.Value.Split(new char[] { ',' }));
                            break;
                        case "TEST SEQUENCE":
                            lstTestSeq.SetSelectedTexts(c.Value.Split(new char[] { ',' }));
                            break;
                        case "VEHICLE TYPE":
                            lstVehicleType.SetSelectedTexts(c.Value.Split(new char[] { ',' }));
                            break;
                    }
                }
            }
        }

        private void RunReport()
        {
            string sql;
            sql = String.Format(ReportData.BuildSQL(), BuildWhere(ReportData.BaseReport.SQLTable));

            Master.UserReport = new Report(ReportData.BaseReport.ReportTitle);

            GDDatabaseClient.Oracle.OracleResponse response = ODAP.GetDataTable(sql, ReportData.BaseReport.DatabaseTarget);
            if (response.Successful)
            {
                if (response.HasResults)
                {
                    GenerateInspectionReport(response.ResultsTable);
                }
                SetMetaData();
                Master.UserReport.FooterNote = ReportData.BaseReport.FooterNote;
            }

            Master.RenderReportToPage();
            SessionHelper.SetCurrentReport(this.Session, Master.UserReport);
        }

        private string BuildWhere(string table = "TESTRECORD")
        {
            List<string> clauses = new List<string>();

            if (table == "TESTRECORD")
            {
                clauses.Add(lstTestSeq.GetOracleText());
                lstModelYear.ValueType = PortalFramework.WebControls.ListBoxValueType.Alpha;
                clauses.Add(lstModelYear.GetOracleText());
                clauses.Add(lstVehicleType.GetOracleText());
                clauses.Add(lstCounty.GetOracleText());
                clauses.Add("testdate >= '" + this.dateSelector.StartDateControl.GetDateText() + "'");
                clauses.Add("testdate <= '" + this.dateSelector.EndDateControl.GetDateText() + "'");
            }
            else
            {
                clauses.Add("RESETDATE >= '" + this.dateSelector.StartDateControl.GetDateText() + "'");
                clauses.Add("RESETDATE <= '" + this.dateSelector.EndDateControl.GetDateText() + "'");
            }

            string[] whereClauses = clauses.Where(x => !String.IsNullOrWhiteSpace(x)).ToArray();

            // Build where clauses.
            StringBuilder sb = new StringBuilder();

            foreach (string whereClause in whereClauses)
            {
                if (whereClause == whereClauses.First())
                {
                    sb.AppendLine("WHERE " + whereClause);
                }
                else
                {
                    sb.AppendLine("AND   " + whereClause);
                }
            }
            return sb.ToString();
        }

        private void SetMetaData()
        {
            if (Master.UserReport != null)
            {
                Master.UserReport.MetaData.Add("Start Date", this.dateSelector.StartDateControl.Text);
                Master.UserReport.MetaData.Add("End Date", this.dateSelector.EndDateControl.Text);

                if (lstTestSeq.ValuesSelected)
                {
                    Master.UserReport.MetaData.Add("Test Sequence", lstTestSeq.GetDelimitedText(", "));
                }

                if (lstModelYear.ValuesSelected)
                {
                    Master.UserReport.MetaData.Add("Model Year", lstModelYear.GetDelimitedText(", "));
                }

                if (lstCounty.ValuesSelected)
                {
                    Master.UserReport.MetaData.Add("County", lstCounty.GetDelimitedText(", "));
                }

                if (lstVehicleType.ValuesSelected)
                {
                    Master.UserReport.MetaData.Add("Vehicle Type", lstVehicleType.GetDelimitedText(", "));
                }
            }
        }

        private void GenerateInspectionReport(DataTable dt)
        {
            ReportRow headerRow = new ReportRow(Master.UserReport);
            Master.UserReport.Sortable = true;

            // Create header row.
            foreach (KeyValuePair<string, ReportColumnInfo> kvp in ReportData.BaseReport.ReportColumnData)
            {
                Master.UserReport.Columns.Add(kvp.Value.DisplayName, kvp.Value.DisplayName, kvp.Value.ColumnDataType);
            }

            // Add Total Row and fill in values after all total values are set.
            if (ReportData is FirstTestResetData)
            {
                // Add rows to report.
                AddDataToReport(dt);
            }
            else
            {
                ReportRow totalRow = new ReportRow(Master.UserReport);
                totalRow.AddClass("static");
                totalRow.AddClass("total-row");
                Master.UserReport.Rows.Add(totalRow);

                // Add rows to report.
                AddDataToReport(dt);

                // Set total row values.
                ReportData.SetTotalRowValues(totalRow);
            }
        }

        private void AddDataToReport(DataTable dt)
        {
            ReportRow currentRow;
            foreach (DataRow dRow in dt.Rows)
            {
                currentRow = new ReportRow(Master.UserReport);
                ReportData.SetPropertyValues(dRow);
                ReportData.AddDataToReportRow(currentRow);
                Master.UserReport.Rows.Add(currentRow);
            }
        }
    }
}