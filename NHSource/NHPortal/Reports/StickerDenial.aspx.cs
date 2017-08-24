using NHPortal.Classes;
using NHPortal.Classes.User;
using PortalFramework.ReportModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace NHPortal.Reports
{
    public partial class StickerDenial : NHPortal.Classes.PortalPage
    {
        public BaseReport ReportData { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            ReportData = BaseReportMaster.StickerDenial;
            RedirectOnInvalidPermission(ReportData.UserPermission);

            if (IsPostBack)
            {
                ProcessPostBack();
            }
            else
            {
                Master.SetHeaderText(ReportData.ReportTitle);
                this.dpStart.Text = DateTime.Now.ToShortDateString();
                LoadFavorite();
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
            Master.UserReport = new Report(ReportData.ReportTitle);

            SetReportFromFile();
            SetMetaData();

            Master.UserReport.FooterNote = ReportData.FooterNote;
            Master.RenderReportToPage();

            SessionHelper.SetCurrentReport(this.Session, Master.UserReport);
        }

        private void SetMetaData()
        {
            if (Master.UserReport != null)
            {
                Master.UserReport.MetaData.Add("Date", this.dpStart.Text);
            }
        }

        private void FormatCSVReportHeaders()
        {
            foreach (ReportColumn rhc in Master.UserReport.Columns)
            {
                KeyValuePair<string, ReportColumnInfo> headerEntry = ReportData.ReportColumnData.FirstOrDefault(x => x.Key == rhc.Text);

                if (headerEntry.Value != null)
                {
                    rhc.Text = headerEntry.Value.DisplayName;
                }
            }
        }

        private void SetReportFromFile()
        {
            string file = System.IO.Path.Combine(PortalFramework.PortalIniSettings.Settings.GetValue("DIR_STICKER_DENIAL"), "STICKERDENIAL_" + this.dpStart.GetDateText() + ".csv");

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
                        case "DATE":
                            this.dpStart.Text = c.Value;
                            break;
                        default:
                            break;
                    }
                }

                RunReport();
            }
        }
    }
}