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

namespace NHPortal
{
    public partial class TriggerStationInspector : PortalPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                ProcessPostback();
            }
            else
            {
                InitReportType();
                PopulateSearchFilterControl();
                LoadFavorite();
            }

            RedirectOnInvalidPermission(Master.ReportData.BaseReport.UserPermission);
        }

        private void ProcessPostback()
        {
            SetReportData();
            if (Master.Master.Master.HidActionValue == "RUN_REPORT")
            {
                HandleCriteria();
            }
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
                        case "ID NUMBER":
                            this.txtIDNumber.Text = c.Value;
                            break;
                        case "SEARCH FILTER":
                            NHPortalUtilities.SetComboBoxValue(this.cboSearchFilter, c.Value);
                            break;
                    }
                }
            }
        }

        private void HandleCriteria()
        {
            if (String.IsNullOrEmpty(this.txtIDNumber.Text.Trim()))
            {
                Master.ErrorMessage.Value = Environment.NewLine + "Please enter an ID number.";
            }
            else
            {
                Master.ReportData.ChartParams = new ChartParams();
                Master.ReportData.StationInspector = this.cboSearchFilter.SelectedValue;
                Master.ReportData.StationInspectorMetaData = this.cboSearchFilter.SelectedItem.Text;

                if(Master.ReportData.StationInspector == "STATION")
                {
                    Master.ReportData.ChartParams.TableType = "SI";
                    Master.ReportData.ChartParams.DrillDown.DrillLevels = new NHDrillLevel[] { new NHDrillLevel(NHDrillType.Station), new NHDrillLevel(NHDrillType.Inspector) };
                }
                else
                {
                    Master.ReportData.ChartParams.TableType = "IS";
                    Master.ReportData.ChartParams.DrillDown.DrillLevels = new NHDrillLevel[] { new NHDrillLevel(NHDrillType.Inspector), new NHDrillLevel(NHDrillType.Station) };
                }

                Master.ReportData.IDNumberString = this.txtIDNumber.Text.Trim().ToUpper();
            }
        }

        private void PopulateSearchFilterControl()
        {
            this.cboSearchFilter.Items.Clear();
            this.cboSearchFilter.Items.Add(new ListItem("Station", "STATION"));
            this.cboSearchFilter.Items.Add(new ListItem("Inspector", "INSPECTOR"));
            this.cboSearchFilter.SelectedIndex = 0;
        }

        private void SetReportData()
        {
            Master.ReportData = HttpContext.Current.Session[NHChartMaster.CHART_REPORT_DATA] as BaseTrigger;

            if (Master.ReportData == null)
            {
                InitReportType();
            }
        }

        private void InitReportType()
        {
            Master.ReportData = new StationInspector();
        }

        protected void btnExportReadinessCSV_Click(object sender, EventArgs e)
        {
            Report report = SessionHelper.GetAuxiliaryReport(Session);
            NHPortalUtilities.ExportReportToCsv(report, Response);
        }

        protected void btnExportReadinessXLSX_Click(object sender, EventArgs e)
        {
            Report report = SessionHelper.GetAuxiliaryReport(Session);
            NHPortalUtilities.ExportReportToXLSX(report, Response);
        }

        protected void btnExportReadinessPDF_Click(object sender, EventArgs e)
        {
            Report report = SessionHelper.GetAuxiliaryReport(Session);
            NHPortalUtilities.ExportReportToPDF(report, Response);
        }

    }
}