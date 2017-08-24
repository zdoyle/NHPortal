using GDCoreUtilities;
using NHPortal.Classes;
using NHPortal.Classes.User;
using Oracle.DataAccess.Client;
using PortalFramework.Database;
using PortalFramework.ReportModel;
using System;
using System.Collections.Generic;
using System.Data;

namespace NHPortal.Reports
{
    public partial class StickerRegistry : PortalPage
    {
        public BaseReport ReportData { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            ReportData = BaseReportMaster.StickerRegistry;
            RedirectOnInvalidPermission(ReportData.UserPermission);

            if (IsPostBack)
            {
                ProcessPostBack();
            }
            else
            {
                Master.SetHeaderText(ReportData.ReportTitle);
                PopulateStickerTypeDropDown();
                LoadFavorite();
            }
        }

        private void PopulateStickerTypeDropDown()
        {
            this.cboStickerType.Items.Clear();

            this.cboStickerType.Items.Add(new System.Web.UI.WebControls.ListItem("A", "A"));
            this.cboStickerType.Items.Add(new System.Web.UI.WebControls.ListItem("M", "M"));

            this.cboStickerType.SelectedIndex = 0;
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
            bool isValid = false;

            if (this.txtStationID.Text.Trim() == String.Empty)
            {
                errMsg += "Station ID Must Not Be Empty. <br />";
            }

            if (this.txtSeries.Text.Trim() == String.Empty)
            {
                errMsg += "Series Field Must Not Be Empty. <br />";
            }

            if (this.txtSticker.Text.Trim() == String.Empty)
            {
                errMsg += "Sticker Field Must Not Be Empty.";
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
                        case "STICKER NUMBER":
                            this.txtSticker.Text = c.Value;
                            break;
                        case "STICKER SERIES":
                            this.txtSeries.Text = c.Value;
                            break;
                        case "STICKER TYPE":
                            this.cboStickerType.SelectedValue = c.Value;
                            break;
                        case "STATION":
                            this.txtStationID.Text = c.Value;
                            break;
                        default:
                            break;
                    }
                }

                RunReport();
            }
        }

        private void RunReport()
        {
            Master.UserReport = new Report(ReportData.ReportTitle);

            DataTable dt = BaseReportMaster.GetProcedureDataTable(ReportData.ProcedureName, GetOracleParams(), DatabaseTarget.OLTP);

            if (dt.Rows.Count > 0)
            {
                GenerateReport();
                AddDataToReport(dt);
            }

            SetMetaData();
            Master.UserReport.FooterNote = ReportData.FooterNote;

            Master.RenderReportToPage();
            SessionHelper.SetCurrentReport(this.Session, Master.UserReport);
        }

        private void SetMetaData()
        {
            if (Master.UserReport != null)
            {
                Master.UserReport.MetaData.Add("Sticker Number", this.txtSticker.Text);
                Master.UserReport.MetaData.Add("Station", this.txtStationID.Text);
                Master.UserReport.MetaData.Add("Sticker Series", this.txtSeries.Text);
                Master.UserReport.MetaData.Add("Sticker Type", this.cboStickerType.SelectedItem.Text, this.cboStickerType.SelectedValue);
            }
        }

        private string FormatDate(string dateString)
        {
            string returnVal;

            if (dateString.Contains("*") || dateString.Contains("+"))
            {
                returnVal = dateString;
            }
            else
            {
                returnVal = dateString.Substring(4, 2) + "/" + dateString.Substring(6, 2) + "/" + dateString.Substring(0, 4);
            }

            return returnVal;
        }

        private void GenerateReport()
        {
            ReportRow headerRow = new ReportRow(Master.UserReport);
            Master.UserReport.Sortable = true;

            // Create header row.
            foreach (KeyValuePair<string, ReportColumnInfo> kvp in ReportData.ReportColumnData)
            {
                Master.UserReport.Columns.Add(kvp.Value.DisplayName, kvp.Value.DisplayName, kvp.Value.ColumnDataType);
            }
        }

        private void AddDataToReport(DataTable dt)
        {
            ReportRow currentRow;
            foreach (DataRow dRow in dt.Rows)
            {
                currentRow = new ReportRow(Master.UserReport);

                foreach (KeyValuePair<string, ReportColumnInfo> kvp in ReportData.ReportColumnData)
                {
                    ReportCell rptCell = new ReportCell(dRow[kvp.Key]);
                    if (kvp.Key == "TESTDATE")
                    {
                        currentRow.Cells.Add(FormatDate(dRow[kvp.Key].ToString()));
                    }
                    else if (kvp.Key == "TESTENDTIME")
                    {
                        string formattedTime;
                        NHPortal.Classes.NHPortalUtilities.TryConvertDateTimeFormat(dRow[kvp.Key].ToString(), "HHmmss", "hh:mm:ss tt", out formattedTime);
                        currentRow.Cells.Add(formattedTime);
                    }
                    else
                    {
                        currentRow.Cells.Add(rptCell);
                    }
                }

                Master.UserReport.Rows.Add(currentRow);
            }
        }

        private OracleParameter[] GetOracleParams()
        {
            List<OracleParameter> paramList = new List<OracleParameter>();

            string startStickerNum = NHPortalUtilities.GetStickerBoundry(this.txtSticker.Text, this.cboStickerType.SelectedValue, true);
            string endStickerNum = NHPortalUtilities.GetStickerBoundry(this.txtSticker.Text, this.cboStickerType.SelectedValue, false);
            string stickerSeries = this.txtSeries.Text.Trim().ToUpper();

            if (!StringUtilities.ContainsAlpha(stickerSeries))
            {
                stickerSeries = stickerSeries.PadLeft(2, '0');
            }

            paramList.Add(new OracleParameter("iStation", OracleDbType.Varchar2, 8, this.txtStationID.Text, ParameterDirection.Input));
            paramList.Add(new OracleParameter("iStickerType", OracleDbType.Varchar2, 1, this.cboStickerType.SelectedValue, ParameterDirection.Input));
            paramList.Add(new OracleParameter("iStickerSeries", OracleDbType.Varchar2, 2, stickerSeries, ParameterDirection.Input));
            paramList.Add(new OracleParameter("iStickerNumStart", OracleDbType.Varchar2, 10, startStickerNum, ParameterDirection.Input));
            paramList.Add(new OracleParameter("iStickerNumEnd", OracleDbType.Varchar2, 10, endStickerNum, ParameterDirection.Input));
            paramList.Add(new OracleParameter("RECORD_DATA", OracleDbType.RefCursor, ParameterDirection.Output));

            return paramList.ToArray();
        }
    }
}