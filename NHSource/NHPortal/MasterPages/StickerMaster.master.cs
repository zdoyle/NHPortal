using GDCoreUtilities;
using NHPortal.Classes;
using NHPortal.Classes.User;
using Oracle.DataAccess.Client;
using PortalFramework.Database;
using PortalFramework.ReportModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI;

namespace NHPortal.MasterPages
{
    public partial class StickerMaster : MasterPage
    {
        public BaseReport ReportData { get; set; }
        public string searchDate { get; set; }
        public string formattedSearchDate { get; set; }

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

                if (ReportData.RunReportOnInitialLoad && ValidateCriteria())
                {
                    RunReport();
                }
            }
        }

        private void InitializePage()
        {
            PopulateStickerTypeDropDown();
        }

        private void PopulateStickerTypeDropDown()
        {
            this.cboStickerType.Items.Clear();

            this.cboStickerType.Items.Add(new System.Web.UI.WebControls.ListItem("A", "A"));
            this.cboStickerType.Items.Add(new System.Web.UI.WebControls.ListItem("M", "M"));

            this.cboStickerType.SelectedIndex = 0;
        }

        private bool ValidateCriteria()
        {
            string errMsg = String.Empty;
            bool isValid = false;

            if (ReportData == BaseReportMaster.StickerDenial)
            {

            }
            else
            {
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
            if (ReportData == null || ReportData == null)
            {
                Response.Redirect("~/Welcome.aspx");
            }

            if(ReportData == BaseReportMaster.StickerDenial)
            {
                this.divStationID.Style["display"] = "none";
                this.divStickerMain.Style["display"] = "none"; 
            }

            Master.SetHeaderText(ReportData.ReportTitle);
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

            if(ReportData == BaseReportMaster.StickerDenial)
            {
                SetReportFromFile();
            }
            else
            {
                DataTable dt = BaseReportMaster.GetProcedureDataTable(ReportData.ProcedureName, GetOracleParams(), DatabaseTarget.OLTP);

                if(dt.Rows.Count > 0)
                {
                    GenerateReport();
                    AddDataToReport(dt);
                }
            }

            SetMetaData();
            Master.UserReport.FooterNote = ReportData.FooterNote;

            Master.RenderReportToPage();
            SessionHelper.SetCurrentReport(this.Session, Master.UserReport);
        }

        private void SetReportFromFile()
        {
            string file = System.IO.Path.Combine(PortalFramework.PortalIniSettings.Settings.GetValue("DIR_STICKER_DENIAL"),
                "STICKERDENIAL_" + this.searchDate + ".csv");

            NHPortalUtilities.LogSessionMessage("Sticker Denial File [" + file + "]", GDCoreUtilities.Logging.LogSeverity.Debug);

            try
            {
                GDCoreUtilities.IO.CSVReader csvReader = new GDCoreUtilities.IO.CSVReader(file);
                DataTable dt = csvReader.ToDataTable();

                if (dt != null)
                {
                    Master.UserReport = new PortalFramework.ReportModel.Report(ReportData.ReportTitle, dt);
                    Master.UserReport.Sortable = true;
                    FormatCSVReportHeaders();
                    Master.RenderReportToPage();
                }
            }
            catch (System.IO.IOException ex)
            {
                NHPortalUtilities.LogSessionException(ex, "There was a problem reading the Sticker Denial file from disk.");
                //FileLogger Logger = SessionHelper.GetSessionLogger(System.Web.HttpContext.Current.Session);
                //Logger.Log("There was a problem reading the Sticker Denial file from disk." + Environment.NewLine + ex.Message, LogSeverity.Error);
            }
        }

        private void FormatCSVReportHeaders()
        {
            //foreach (ReportHeaderCell rhc in Master.UserReport.HeaderRow)
            foreach (ReportColumn rhc in Master.UserReport.Columns)
            {
                KeyValuePair<string, ReportColumnInfo> headerEntry = ReportData.ReportColumnData.FirstOrDefault(x => x.Key == rhc.Text);   
   
                if(headerEntry.Value != null)
                {
                    rhc.Text = headerEntry.Value.DisplayName;  
                }      
            }
        }

        private Oracle.DataAccess.Client.OracleParameter[] GetOracleParams()
        {
            List<OracleParameter> paramList = new List<OracleParameter>();

            string startStickerNum = GetStickerBoundry(this.txtSticker.Text, this.cboStickerType.SelectedValue, true);
            string endStickerNum = GetStickerBoundry(this.txtSticker.Text, this.cboStickerType.SelectedValue, false);

            paramList.Add(new OracleParameter("iStation", OracleDbType.Varchar2, 8, this.txtStationID.Text, ParameterDirection.Input));
            paramList.Add(new OracleParameter("iStickerType", OracleDbType.Varchar2, 1, this.cboStickerType.SelectedValue, ParameterDirection.Input));
            paramList.Add(new OracleParameter("iStickerSeries", OracleDbType.Varchar2, 2, this.txtSeries.Text, ParameterDirection.Input));
            paramList.Add(new OracleParameter("iStickerNumStart", OracleDbType.Varchar2, 10, startStickerNum, ParameterDirection.Input));
            paramList.Add(new OracleParameter("iStickerNumEnd", OracleDbType.Varchar2, 10, endStickerNum, ParameterDirection.Input));
            paramList.Add(new OracleParameter("RECORD_DATA", OracleDbType.RefCursor, ParameterDirection.Output));

            return paramList.ToArray();
        }

        private string GetStickerBoundry(string stickerNumString, string stickerType, bool isStartNumber)
        {
            string returnVal;
            long modVal, boundryDistance;
            long stickerNum = NullSafe.ToLong(stickerNumString);

            if(stickerType == "A")
            {
                modVal = 25;
            }
            else // Type M
            {
                modVal = 30;
            }

            if (isStartNumber)
            {
                boundryDistance = (stickerNum - 1) % modVal;

                if (boundryDistance == 0)
                {
                    returnVal = stickerNum.ToString();
                }
                else
                {
                    returnVal = (stickerNum - boundryDistance).ToString();
                }
            }
            else
            {
                boundryDistance = modVal - (stickerNum - 1) % modVal;
                returnVal = (stickerNum - 1 + boundryDistance).ToString();
            }

            return  returnVal.PadLeft(10,'0');
        }

        private void SetMetaData()
        {
            if (Master.UserReport != null)
            {
                if (ReportData == BaseReportMaster.StickerDenial)
                {
                    Master.UserReport.MetaData.Add("Date", this.formattedSearchDate);
                }
                else
                {
                    Master.UserReport.MetaData.Add("Sticker Number", this.txtSticker.Text);
                    Master.UserReport.MetaData.Add("Station", this.txtStationID.Text);
                    Master.UserReport.MetaData.Add("Sticker Series", this.txtSeries.Text);
                    Master.UserReport.MetaData.Add("Sticker Type", this.cboStickerType.SelectedItem.Text, this.cboStickerType.SelectedValue);
                }
            }
        }

        private void GenerateReport()
        {
            ReportRow headerRow = new ReportRow(Master.UserReport);
            Master.UserReport.Sortable = true;

            // Create header row.
            foreach (KeyValuePair<string, ReportColumnInfo> kvp in ReportData.ReportColumnData)
            {
                Master.UserReport.Columns.Add(kvp.Value.DisplayName, kvp.Value.DisplayName, kvp.Value.ColumnDataType);
                //ReportHeaderCell headerCell = new ReportHeaderCell(kvp.Value.DisplayName);
                //headerCell.ColumnDataType = kvp.Value.ColumnDataType;
                //headerRow.Cells.Add(headerCell);
            }

            //Master.UserReport.HeaderRow = headerRow;
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
              returnVal = dateString.Substring(4,2) + "/" + dateString.Substring(6,2) + "/" + dateString.Substring(0,4);
            }

            return returnVal;
        }

        private void AddDataToReport(DataTable dt)
        {
            ReportRow currentRow;
            foreach (DataRow dRow in dt.Rows)
            {
                currentRow = new ReportRow(Master.UserReport);

                foreach (KeyValuePair<string,ReportColumnInfo> kvp in ReportData.ReportColumnData)
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
    }

    public static class StringUtils
    {
        public static IEnumerable<string> SplitByLength(this string s, int length)
        {
            for (int i = 0; i < s.Length; i += length)
            {
                if (i + length <= s.Length)
                {
                    yield return s.Substring(i, length);
                }
                else
                {
                    yield return s.Substring(i);
                }
            }
        }
    }
}