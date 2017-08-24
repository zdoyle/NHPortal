using GD.Highcharts.Enums;
using NHPortal.Classes;
using NHPortal.Classes.Charts;
using PortalFramework.ReportModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NHPortal.MasterPages
{
    public partial class ChartMaster : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                ProcessPostBack();
            }
            else
            {
                ClearReport();
                InitPage();
                LoadFavorite();

                if (ReportData.BaseReport.RunReportOnInitialLoad)
                {
                    RunReport();
                }

                Master.SetRunButtonText("Submit Query");
                Master.SetHeaderText(ReportData.BaseReport.ReportTitle);
            }

            this.Page.Title = "Gordon-Darby Charts";
        }

        private void ClearReport()
        {
            Session[NHChartMaster.CHART_REPORT_DATA] = null;
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

        private bool ValidateCriteria()
        {
            string errMsg = String.Empty;

            if (this.dpStart.GetDateText() == String.Empty)
            {
                errMsg += "Please enter a valid start date. <br />";
            }
            if (this.dpEnd.GetDateText() == String.Empty)
            {
                errMsg += " Please enter a valid end date. ";
            }

            Master.SetGenericError(errMsg);

            return errMsg == String.Empty;
        }

        private void RunDrillReport()
        {
            InitDrillValues();
            RenderReportToPage();
        }

        private void RunReport()
        {
            CreateChartParams();
            ReportData.LoadContainer();

            // Init charts
            SetCharts();
            this.divToggleChart.Style["display"] = "block";

            if (ReportData.RunInitialReport)
            {
                RenderReportToPage();
                this.divReportContainer.Style["display"] = "block";
            }
            else
            {
                // Clear any previous report.
                ReportData.Report = null;
                Master.ClearReport();

                if (!(ReportData is OBDIIDTCErrorCodes || ReportData is OBDIIProtocolUsage))
                {
                    Master.SetButtonVisibility(ReportButton.Favorite, true);
                }
            }

            Session[NHChartMaster.CHART_REPORT_DATA] = ReportData;
        }

        internal void SetCharts()
        {
            // Add chart divs to report.
            this.ltCharts.Text = ReportData.RenderContainer();

            // Add chart scripts to report.
            foreach (NHChartWrapper wrap in ReportData.Container.ChartWrappers)
            {
                string chartScript = wrap.Chart.ChartScriptHtmlString().ToString();
                ScriptManager.RegisterStartupScript(this, this.GetType(), wrap.ChartName, chartScript, false);
            }
        }

        /// <summary>Render's the user's current report to the page.</summary>
        internal void RenderReportToPage()
        {
            ReportData.LoadReport();
            SetMetaData();
            Master.UserReport = ReportData.Report;
            Master.RenderReportToPage();
            SessionHelper.SetCurrentReport(this.Session, ReportData.Report);
        }

        private void CreateChartParams()
        {
            ReportData.ChartParams = new ChartParams();
            ChartDimention chartDim;
            ChartTypes chartType;
            DataGrouping dataGrouping;

            ReportData.ChartParams.StartDateText = this.dpStart.GetDateText();
            ReportData.ChartParams.EndDateText = this.dpEnd.GetDateText();
            ReportData.ChartParams.StartDate = this.dpStart.Text;
            ReportData.ChartParams.EndDate = this.dpEnd.Text;

            Enum.TryParse(this.cboChartDimention.SelectedValue, out chartDim);
            ReportData.ChartParams.ChartDimention = chartDim;

            Enum.TryParse(this.cboChartType.SelectedValue, out chartType);
            ReportData.ChartParams.ChartType = chartType;

            Enum.TryParse(this.cboGroupBy.SelectedValue, out dataGrouping);
            ReportData.ChartParams.GroupByData = GroupByMaster.Find(dataGrouping);

            ReportData.ChartParams.TableType = this.cboDrillOrder.SelectedValue;

            if (this.cboDrillOrder.SelectedIndex == 0)
            {
                ReportData.ChartParams.DrillDown.DrillLevels = new NHDrillLevel[] { new NHDrillLevel(NHDrillType.ModelYear), new NHDrillLevel(NHDrillType.Make), new NHDrillLevel(NHDrillType.Model) };
            }
            else if (this.cboDrillOrder.SelectedIndex == 1)
            {
                ReportData.ChartParams.DrillDown.DrillLevels = new NHDrillLevel[] { new NHDrillLevel(NHDrillType.Make), new NHDrillLevel(NHDrillType.ModelYear), new NHDrillLevel(NHDrillType.Model) };
            }
            else
            {
                ReportData.ChartParams.DrillDown.DrillLevels = new NHDrillLevel[] { new NHDrillLevel(NHDrillType.Make), new NHDrillLevel(NHDrillType.Model), new NHDrillLevel(NHDrillType.ModelYear) };
            }
        }

        private void InitPage()
        {
            if (ReportData is EmissionTestRejectionData || ReportData is OBDIIReadinessMonitors || ReportData is OBDMILStatus || ReportData is OBDIICommunications)
            {
                InitGroupByDropDown();
                InitChartTypeDropDown();
                InitChartDimentionDrowDown();
                InitDrillOrderDropDown();
                InitDateRangeControls();
            }
            else
            {
                this.divMainCriteria.Style["display"] = "none";
            }

            if (ReportData is OBDIIDTCErrorCodes)
            {
                this.divDTC.Style["display"] = "block";
            }
            else
            {
                this.divDTC.Style["display"] = "none";
            }

            this.divReportContainer.ClientIDMode = System.Web.UI.ClientIDMode.Static;

            SetDisplayOptions("none");
        }

        private void InitDateRangeControls()
        {
            this.dpStart.Text = new DateTime(DateTime.Now.Year, 1, 1).ToShortDateString();
            this.dpEnd.Text = DateTime.Now.ToShortDateString();
        }

        private void InitDrillOrderDropDown()
        {
            this.cboDrillOrder.Items.Clear();
            this.cboDrillOrder.Items.Add(new ListItem("Year-Make-Model", "YKD"));
            this.cboDrillOrder.Items.Add(new ListItem("Make-Year-Model", "KYD"));
            this.cboDrillOrder.Items.Add(new ListItem("Make-Model-Year", "KDY"));
            this.cboDrillOrder.SelectedIndex = 0;
        }

        private void InitChartDimentionDrowDown()
        {
            this.cboChartDimention.Items.Clear();
            this.cboChartDimention.Items.Add(new ListItem("2D", ChartDimention.TwoDimentional.ToString()));
            this.cboChartDimention.Items.Add(new ListItem("3D", ChartDimention.ThreeDimentional.ToString()));
            this.cboChartDimention.SelectedIndex = 0;
        }

        private void InitChartTypeDropDown()
        {
            this.cboChartType.Items.Clear();

            if (ReportData.BaseReport == BaseReportMaster.EmissionTestRejectionRates)
            {
                this.cboChartType.Items.Add(new ListItem("Column", "Column"));
                this.cboChartType.Items.Add(new ListItem("Bar", "Bar"));
                this.cboChartType.Items.Add(new ListItem("Line", "Line"));
            }
            else
            {
                this.cboChartType.Items.Add(new ListItem("Pie", "Pie"));
                this.cboChartType.Items.Add(new ListItem("Column", "Column"));
                this.cboChartType.Items.Add(new ListItem("Bar", "Bar"));
                this.cboChartType.Items.Add(new ListItem("Line", "Line"));
                this.cboChartType.Items.Add(new ListItem("Area", "Area"));
                this.cboChartType.Items.Add(new ListItem("Spline", "Spline"));
            }

            this.cboChartType.SelectedIndex = 0;
        }

        private void InitGroupByDropDown()
        {
            this.cboGroupBy.Items.Clear();
            this.cboGroupBy.Items.Add(new ListItem("Year", DataGrouping.Yearly.ToString()));
            this.cboGroupBy.Items.Add(new ListItem("Month", DataGrouping.Monthly.ToString()));
            this.cboGroupBy.Items.Add(new ListItem("Day", DataGrouping.Daily.ToString()));
            this.cboGroupBy.SelectedIndex = 0;
        }

        public void SetDisplayOptions(string display)
        {
            this.divToggleChart.Style["display"] = display;
        }

        private void SaveFavorite()
        {
            if (ReportData == null) return;

            NHPortal.Classes.User.UserFavorite fav = new NHPortal.Classes.User.UserFavorite();
            fav.Title = ReportData.BaseReport.ReportTitle;
            fav.Description = Master.FavDescTextBox.Text;
            fav.NavCode = ReportData.BaseReport.PageCode;
            fav.FavType = NHPortal.Classes.User.UserFavoriteTypes.Graph;

            foreach (var meta in ReportData.ChartMetaData)
            {
                fav.AddCriteria(meta.Key, meta.Text, meta.Value);
            }

            string msg = String.Empty;
            bool saved = fav.Save();
            Master.SetFavoriteResultPrompt(saved);

            SetCharts();
        }

        private void LoadFavorite()
        {
            NHPortal.Classes.User.UserFavorite fav = SessionHelper.GetSelectedFavorite(this.Session);
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
                        case "GROUP BY":
                            NHPortalUtilities.SetComboBoxValue(cboGroupBy, c.Value);
                            break;
                        case "CHART TYPE":
                            NHPortalUtilities.SetComboBoxValue(cboChartType, c.Value);
                            break;
                        case "CHART DISPLAY":
                            NHPortalUtilities.SetComboBoxValue(cboChartDimention, c.Value);
                            break;
                        case "DRILL DOWN ORDER":
                            NHPortalUtilities.SetComboBoxValue(cboDrillOrder, c.Value);
                            break;
                    }
                }
                SessionHelper.SetSelectedFavorite(this.Session, null);
            }
        }

        private void SetMetaData()
        {
            if (ReportData is OBDIIDTCErrorCodes || ReportData is OBDIIProtocolUsage)
            {
                
            }
            else
            {
                ReportData.Report.MetaData.Add("Start Date", this.dpStart.Text);
                ReportData.Report.MetaData.Add("End Date", this.dpEnd.Text);
                ReportData.Report.MetaData.Add("Group By", this.cboGroupBy.SelectedItem.Text, this.cboGroupBy.SelectedValue);
                ReportData.Report.MetaData.Add("Chart Type", this.cboChartType.SelectedItem.Text, this.cboChartType.SelectedValue);
                ReportData.Report.MetaData.Add("Chart Display", this.cboChartDimention.SelectedItem.Text, this.cboChartDimention.SelectedValue);
                ReportData.Report.MetaData.Add("Drill Down Order", this.cboDrillOrder.SelectedItem.Text, this.cboDrillOrder.SelectedValue);
            }

            if (!ReportData.RunInitialReport)
            {
                if (ReportData is OBDIIReadinessMonitors)
                {
                    ReportData.Report.MetaData.Add("Drill Category", ReportData.ChartParams.SelectedSeriesName);
                }

                if (ReportData.ChartParams.SelectedSeriesName == "ALL_CODES")
                {
                    if (ReportData.ChartParams.SelectedSeries == "ALL")
                    {
                        ReportData.Report.MetaData.Add(ReportData.ChartParams.SelectedPoint + " Codes", "All");
                    }
                    else
                    {
                        ReportData.Report.MetaData.Add(ReportData.ChartParams.SelectedPoint + " Codes", "All By Year-Make-Model");
                    }
                }
                else
                {
                    ReportData.Report.MetaData.Add("Drill Option", ReportData.ChartParams.SelectedPoint);
                }
            }
        }

        private void InitDrillValues()
        {
            ReportData.ChartParams.SelectedSeries = this.hidSeriesClicked.Value;
            ReportData.ChartParams.SelectedPoint = this.hidPointClicked.Value;
            ReportData.ChartParams.SelectedSeriesName = this.hidSeriesName.Value;
            ReportData.ChartParams.DrillDown.ClearDrillLevels();

            this.hidSeriesClicked.Value = String.Empty;
            this.hidPointClicked.Value = String.Empty;
            this.hidSeriesName.Value = String.Empty;
        }

        protected void LoadDrillTable(object sender, EventArgs e)
        {
            RunDrillReport();
            if (ReportData is OBDIIDTCErrorCodes || ReportData is OBDIIProtocolUsage)
            {
                Master.SetButtonVisibility(ReportButton.Favorite, false);
            }
            ScriptManager.RegisterStartupScript(this.udpCharts, this.GetType(), "inittable", "initReportTable();", true);
        }

        public IChartReportData ReportData { get; set; }
    }
}