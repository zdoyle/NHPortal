using GD.Highcharts.Enums;
using GDCoreUtilities;
using NHPortal.Classes;
using NHPortal.Classes.Charts;
using NHPortal.Classes.Reports.Triggers;
using PortalFramework.ReportModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NHPortal.MasterPages
{
    public partial class TriggerMaster : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                ProcessPostBack();
            }
            else
            {
                if (ReportData == null)
                {
                    Response.Redirect("~/Welcome.aspx");
                }

                InitPage();
                ClearReport();
                InitCriteria();
                LoadFavorite();
                Master.SetHeaderText(ReportData.BaseReport.ReportTitle);
                Master.SetRunButtonText("Submit Query");
            }
            this.Page.Title = "Gordon-Darby Triggers";
            Master.MasterPage.AsyncPostBackTimeoutValue = 600;
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
                        case "CHART TYPE":
                            NHPortalUtilities.SetComboBoxValue(this.cboChartType, c.Value);
                            break;
                        case "CHART DISPLAY":
                            NHPortalUtilities.SetComboBoxValue(this.cboChartDimention, c.Value);
                            break;
                        case "DRILL DOWN ORDER":
                            NHPortalUtilities.SetComboBoxValue(this.cboDrillOrder, c.Value);
                            break;
                        case "THRESHOLD":
                            NHPortalUtilities.SetComboBoxValue(this.cboTheshhold, c.Value);
                            break;
                    }
                }
                SessionHelper.SetSelectedFavorite(this.Session, null);
            }
        }

        private void InitPage()
        {
            this.divToggleChart.Style["display"] = "none";
            btnDecileBack.Visible = false;
            btnDetailedReport.Visible = false;
            btnReadinessFormat.Visible = false;

            if (ReportData is WeightedScore || ReportData is StationInspector)
            {
                this.btnTriggerDefinition.Visible = false;
            }
        }

        private void InitCriteria()
        {
            if (ReportData is StationInspector)
            {
                this.btnDecileBack.Text = "Back To Overview";
                this.divChartCriteria.Visible = false;
            }
            else
            {
                InitChartTypeControl();
                InitChartDisplayControl();
                InitDrillDownControl();
            }

            InitDateRangeControls();

            if (ReportData is TimeBeforeTests || ReportData is WeightedScore)
            {
                this.divThreshold.Visible = true;
                InitThresholdControl();
            }
            else
            {
                this.divThreshold.Visible = false;
            }
        }

        private void InitThresholdControl()
        {
            this.cboTheshhold.Items.Clear();
            this.cboTheshhold.Items.Add(new ListItem("1 Minute", "1"));
            this.cboTheshhold.Items.Add(new ListItem("2 Minutes", "2"));
            this.cboTheshhold.Items.Add(new ListItem("3 Minutes", "3"));
            this.cboTheshhold.Items.Add(new ListItem("4 Minutes", "4"));
            this.cboTheshhold.Items.Add(new ListItem("5 Minutes", "5"));

            this.cboTheshhold.SelectedIndex = 2;
        }

        private void InitDrillDownControl()
        {
            this.cboDrillOrder.Items.Clear();
            this.cboDrillOrder.Items.Add(new ListItem("Station-Inspector", "SI"));
            this.cboDrillOrder.Items.Add(new ListItem("Inspector-Station", "IS"));
            this.cboDrillOrder.SelectedIndex = 0;
        }

        private void InitChartDisplayControl()
        {
            this.cboChartDimention.Items.Clear();
            this.cboChartDimention.Items.Add(new ListItem("2D", ChartDimention.TwoDimentional.ToString()));
            this.cboChartDimention.Items.Add(new ListItem("3D", ChartDimention.ThreeDimentional.ToString()));
            this.cboChartDimention.SelectedIndex = 0;
        }

        private void InitChartTypeControl()
        {
            this.cboChartType.Items.Clear();

            this.cboChartType.Items.Add(new ListItem("Column", "Column"));
            this.cboChartType.Items.Add(new ListItem("Bar", "Bar"));
            this.cboChartType.Items.Add(new ListItem("Pie", "Pie"));
            this.cboChartType.Items.Add(new ListItem("Line", "Line"));
            this.cboChartType.Items.Add(new ListItem("Area", "Area"));
            this.cboChartType.Items.Add(new ListItem("Spline", "Spline"));

            this.cboChartType.SelectedIndex = 0;
        }

        private void InitDateRangeControls()
        {
            this.dpStart.Text = new DateTime(DateTime.Now.Year, 1, 1).ToShortDateString();
            this.dpEnd.Text = DateTime.Now.ToShortDateString();
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

        private void ClearReport()
        {
            Session[NHChartMaster.CHART_REPORT_DATA] = null;
        }

        private void RunReport()
        {
            CreateChartParams();

            if (ReportData is TimeBeforeTests || ReportData is WeightedScore)
            {
                ReportData.ThresholdMins = this.cboTheshhold.SelectedValue;
            }

            ReportData.LoadContainer();

            // Init charts
            SetCharts();

            if (ReportData is StationInspector)
            {

            }
            else
            {
                // Make toggle chart button visible.
                this.divToggleChart.Style["display"] = "block";
            }

            if (ReportData.RunInitialReport)
            {
                RenderReportToPage(false);
                this.divReportContainer.Style["display"] = "block";
            }
            else
            {
                // Clear any previous report.
                Master.ClearReport();
                Master.SetButtonVisibility(ReportButton.Favorite, true);
                ReportData.Report = null;

                btnDecileBack.Visible = false;
                btnDetailedReport.Visible = false;
                btnReadinessFormat.Visible = false;
            }

            Session[NHChartMaster.CHART_REPORT_DATA] = ReportData;
        }

        internal void RenderReportToPage(bool isDetailedReport)
        {
            bool isBackFromDetailedReport = this.hidIsBackToReport.Value == "true";
            this.hidIsBackToReport.Value = "false";

            if (isDetailedReport)
            {
                ReportData.LoadDetailedReport();
            }
            else
            {
                Report prevReport = SessionHelper.GetAuxiliaryReport(Session);
                
                // Load a new report or load the aux report if is a 'back to' button action
                if(prevReport == null || !isBackFromDetailedReport)
                {
                    ReportData.LoadReport();
                    SessionHelper.SetAuxiliaryReport(Session, ReportData.Report);
                }
                else
                {
                    ReportData.Report = prevReport;
                }
            }

            if (ReportData.Report == null)
            {
                Master.UserReport = null;
            }
            else
            {
                SetMetaData();
                Master.UserReport = ReportData.Report;
                Master.RenderReportToPage();
                SessionHelper.SetCurrentReport(this.Session, ReportData.Report);
            }
        }

        private void CreateChartParams()
        {
            if (ReportData.ChartParams == null)
            {
                ReportData.ChartParams = new ChartParams();
            }

            ChartDimention chartDim;
            ChartTypes chartType;

            ReportData.ChartParams.SelectedPoint = null;
            ReportData.ChartParams.SelectedSeries = null;
            ReportData.ChartParams.SelectedSeriesName = null;

            ReportData.ChartParams.StartDateText = this.dpStart.GetDateText();
            ReportData.ChartParams.EndDateText = this.dpEnd.GetDateText();
            ReportData.ChartParams.StartDate = this.dpStart.Text;
            ReportData.ChartParams.EndDate = this.dpEnd.Text;

            Enum.TryParse(this.cboChartDimention.SelectedValue, out chartDim);
            ReportData.ChartParams.ChartDimention = chartDim;

            Enum.TryParse(this.cboChartType.SelectedValue, out chartType);
            ReportData.ChartParams.ChartType = chartType;

            if (ReportData is StationInspector)
            {

            }
            else
            {
                ReportData.ChartParams.TableType = this.cboDrillOrder.SelectedValue;

                if (this.cboDrillOrder.SelectedIndex == 0)
                {
                    ReportData.ChartParams.DrillDown.DrillLevels = new NHDrillLevel[] { new NHDrillLevel(NHDrillType.Station), new NHDrillLevel(NHDrillType.Inspector) };
                    ReportData.StationInspector = "Station";
                }
                else
                {
                    ReportData.ChartParams.DrillDown.DrillLevels = new NHDrillLevel[] { new NHDrillLevel(NHDrillType.Inspector), new NHDrillLevel(NHDrillType.Station) };
                    ReportData.StationInspector = "Inspector";
                }
            }
        }

        private void SetMetaData()
        {
            ReportData.Report.MetaData.Clear();
            ReportData.Report.MetaData.Add("Start Date", this.dpStart.Text);
            ReportData.Report.MetaData.Add("End Date", this.dpEnd.Text);

            if (ReportData is StationInspector)
            {
                ReportData.Report.MetaData.Add("Search Filter", ReportData.StationInspectorMetaData);
                ReportData.Report.MetaData.Add("ID Number", ReportData.IDNumberString);
            }
            else
            {
                ReportData.Report.MetaData.Add("Chart Type", this.cboChartType.SelectedItem.Text, this.cboChartType.SelectedValue);
                ReportData.Report.MetaData.Add("Chart Display", this.cboChartDimention.SelectedItem.Text, this.cboChartDimention.SelectedValue);
                ReportData.Report.MetaData.Add("Drill Down Order", this.cboDrillOrder.SelectedItem.Text, this.cboDrillOrder.SelectedValue);

                if (ReportData.IsDetailedReport)
                {
                    ReportData.Report.MetaData.Add(ReportData.StationInspector, ReportData.ChartParams.SelectedPoint);
                }
                else
                {
                    ReportData.Report.MetaData.Add("Decile", this.hidDecile.Value);
                }
            }

            //if (ReportData is TimeBeforeTests || ReportData is WeightedScore)
            if (ReportData.ThresholdMins != null)
            {
                //ReportData.Report.MetaData.Add("Threshold", cboTheshhold.SelectedItem.Text, this.cboTheshhold.SelectedValue);
                ReportData.Report.MetaData.Add("Threshold", ReportData.ThresholdMins + " Minute" + (ReportData.ThresholdMins == "1" ? String.Empty : "s"), ReportData.ThresholdMins);
            }
        }

        private void SaveFavorite()
        {
            if (ReportData == null) return;

            NHPortal.Classes.User.UserFavorite fav = new NHPortal.Classes.User.UserFavorite();
            fav.Title = ReportData.BaseReport.ReportTitle;
            fav.Description = Master.FavDescTextBox.Text;
            fav.NavCode = ReportData.BaseReport.PageCode;
            fav.FavType = NHPortal.Classes.User.UserFavoriteTypes.Trigger;

            foreach (ChartMetaData meta in ReportData.ChartMetaData)
            {
                fav.AddCriteria(meta.Key, meta.Text, meta.Value);
            }

            if (ReportData is WeightedScore)
            {
                foreach (KeyValuePair<string, int> kvp in this.ReportData.TriggerWeights)
                {
                    fav.AddCriteria(kvp.Key, kvp.Value.ToString(), kvp.Value.ToString());
                }
            }
            
            string msg = String.Empty;
            bool saved = fav.Save();
            if (saved)
            {
                msg = "Saved to My Favorites.";
            }
            else
            {
                msg = "Failed to save to My Favorites.";
            }

            Master.Master.SetMessagePrompt(msg);
            SetCharts();
        }

        internal void SetCharts()
        {
            // Add chart divs to report.
            this.ltCharts.Text = ReportData.RenderContainer();

            // Add chart scripts to report.
            if (ReportData.Container != null)
            {
                foreach (NHChartWrapper wrap in ReportData.Container.ChartWrappers)
                {
                    string chartScript = wrap.Chart.ChartScriptHtmlString().ToString();
                    ScriptManager.RegisterStartupScript(this, this.GetType(), wrap.ChartName, chartScript, false);
                }
            }
        }

        private bool ValidateCriteria()
        {
            string errMsg = String.Empty;
            bool returnVal = false;

            if (this.dpStart.GetDateText() == String.Empty)
            {
                errMsg += "Please enter a valid start date. <br />";
            }
            if (this.dpEnd.GetDateText() == String.Empty)
            {
                errMsg += " Please enter a valid end date. <br />";
            }

            //  Check if child pages have set an error message.
            if (!String.IsNullOrEmpty(this.hidErrMsg.Value))
            {
                errMsg += this.hidErrMsg.Value;
            }

            this.hidErrMsg.Value = String.Empty;
            this.lblErrorMsg.Text = errMsg;

            if (errMsg == String.Empty)
            {
                returnVal = true;
            }
            else
            {
                // Make toggle chart and save favorite buttons hidden.
                this.divToggleChart.Style["display"] = "none";
                Master.SetButtonVisibility(ReportButton.Favorite, false);
                this.ltCharts.Text = String.Empty;
            }

            return returnVal;
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
            LoadReport(false);
        }

        protected void LoadDetailedReport(object sender, EventArgs e)
        {
            ReportData = Session[NHChartMaster.CHART_REPORT_DATA] as BaseTrigger;
            ReportData.IncludeVehicleInfo = this.chkVehicle.Checked;
            ReportData.IncludeInspectionInfo = this.chkInspection.Checked;
            ReportData.IncludeVisualInfo = this.chkVisual.Checked;
            ReportData.IncludeOBDInfo = this.chkOBD.Checked;
            ReportData.IncludeSafetyInfo = this.chkSafety.Checked;
            LoadReport(true);
        }

        private void LoadReport(bool isDetailedReport)
        {
            btnDecileBack.Visible = isDetailedReport ? true : false;
            btnDetailedReport.Visible = isDetailedReport ? true : false;
            btnReadinessFormat.Visible = isDetailedReport ? true : false;

            ReportData.IsDetailedReport = isDetailedReport;
            ReportData.FilterCount = FilterCount;
            InitDrillValues();
            RenderReportToPage(isDetailedReport);
            ScriptManager.RegisterStartupScript(this.upDashBuilder, this.GetType(), "inittable", "initReportTable();", true);
        }

        public BaseTrigger ReportData { get; set; }
        public HiddenField ErrorMessage { get { return this.hidErrMsg; } }
        public HiddenField IsWeighted { get { return this.hidIsWeighted; } }
        public int FilterCount
        {
            get
            {
                return NullSafe.ToInt(this.txtFilterCriteria.Text);
            }
        }
    }
}