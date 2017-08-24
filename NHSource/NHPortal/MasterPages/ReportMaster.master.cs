using GDCoreUtilities.Logging;
using NHPortal.Classes;
using NHPortal.Classes.User;
using PortalFramework.ReportModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NHPortal.MasterPages
{
    public partial class ReportMaster : System.Web.UI.MasterPage
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            //divFavoriteOverlay.Attributes.Add("onclick", "hideOverlayClick(this);");

            // registering this event seems to allow viewstate to load
            // so we can clear the error label each PostBack prior to the Page_Load event
            Page.PreLoad += Page_PreLoad;

            if (IsPostBack)
            {
                UserReport = SessionHelper.GetCurrentReport(this.Session);
            }
            else
            {
                // Reinitialize report when navigating to a new report page
                SessionHelper.ClearCurrentReport(this.Session);
            }
        }

        protected void Page_PreLoad(object sender, EventArgs e)
        {
            lblError.Text = String.Empty;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {

            }
            else
            {

            }

            SessionHelper.SetSelectedFavorite(this.Session, null);
        }

        /// <summary>Render's the user's current report to the page.</summary>
        internal void RenderReportToPage()
        {
            if (UserReport != null)
            {
                HTMLReportRenderer renderer = new HTMLReportRenderer();
                ltrlReport.Text = renderer.RenderReport(UserReport);
                ltrlReport.Visible = true;
                SetExportButtonsVisibility(true);
                btnSaveFavorite.Visible = true;
            }
            else
            {
                ClearReport();
            }
        }

        /// <summary>Clears the report and sets the visibility of the export buttons to false.</summary>
        internal void ClearReport()
        {
            UserReport = null;
            ltrlReport.Text = String.Empty;
            ltrlReport.Visible = false;
            SetExportButtonsVisibility(false);
            btnSaveFavorite.Visible = false;
        }

        /// <summary>Sets the visibility of the export buttons.</summary>
        /// <param name="show">True to show the export buttons, false otherwise.</param>
        internal void SetExportButtonsVisibility(bool show)
        {
            btnExportCSV.Visible = show;
            btnExportXLSX.Visible = show;
            btnExportPDF.Visible = show;
        }

        /// <summary>Saves the current report to the user's My Favorites.</summary>
        /// <param name="navCode">Navigation code of the report to save.</param>
        /// <param name="favType">The type of favorite being saved.</param>
        internal void SaveFavorite(string navCode, UserFavoriteType favType)
        {
            if (UserReport != null)
            {
                UserFavorite fav = new UserFavorite();
                fav.Title = UserReport.Title;
                fav.Description = FavDescTextBox.Text;
                fav.NavCode = navCode;
                fav.FavType = favType;

                foreach (var meta in UserReport.MetaData)
                {
                    fav.AddCriteria(meta.Key, meta.Text, meta.Value);
                }

                string msg = String.Empty;
                bool saved = fav.Save();
                SetFavoriteResultPrompt(saved);
            }
        }

        /// <summary>Sets the prompt to show the user based on the outcome of saving a favorite record.</summary>
        /// <param name="saveSuccessful">True if the favorite saved successfully, false otherwise.</param>
        internal void SetFavoriteResultPrompt(bool saveSuccessful)
        {
            string msg = String.Empty;
            if (saveSuccessful)
            {
                msg = "Saved to My Favorites.";
            }
            else
            {
                msg = "Failed to save to My Favorites.";
            }

            Master.SetMessagePrompt(msg);
        }

        /// <summary>Sets the text of the data last updated label on the PortalMaster master page.</summary>
        /// <param name="dataUpdateDt">Date time to assign to the label.</param>
        internal void SetDataLastUpdate(DateTime dataUpdateDt)
        {
            Master.DataLastUpdateLabel.Text = "Data Last Updated: " + dataUpdateDt.ToString("M/d/yyyy h:mm:ss tt");
        }

        /// <summary>Sets a generic error message on the page.</summary>
        /// <param name="error">Error message to display.</param>
        internal void SetGenericError(string message)
        {
            lblError.Text = message;
            NHPortalUtilities.LogSessionMessage("SetGenericError: " + message, LogSeverity.Error);
        }

        /// <summary>Sets an error message for report generation on the page.</summary>
        /// <param name="error">Error message to display.</param>
        internal void SetError(string error)
        {
            string msg = "An error occurred when running the report."
                       + "<br />"
                       + error;

            SetGenericError(msg);
            //lblError.Text = msg;
        }

        /// <summary>Sets the error message on the page.</summary>
        /// <param name="ex">Exception to display.</param>
        internal void SetError(Exception ex)
        {
            string msg = "Unknown error.";
            if (ex != null)
            {
                msg = ex.Message;
            }
            SetError(msg);
        }

        /// <summary>Sets the text of the "Run Report" button.</summary>
        /// <param name="text">Text to display on the button.</param>
        internal void SetRunButtonText(string text)
        {
            btnRunReport.Text = text;
        }

        /// <summary>Sets the text of the Master page's header label.</summary>
        /// <param name="text">Text to set to the header label.</param>
        internal void SetHeaderText(string text)
        {
            Master.SetHeaderText(text);
        }

        /// <summary>Sets the visibility of a button on the Report Master page.</summary>
        /// <param name="button">Report button to set the visibility of.</param>
        /// <param name="visible">True to show the button, false to hide.</param>
        internal void SetButtonVisibility(ReportButton button, bool visible)
        {
            Button btn = ResolveButtonReference(button);
            if (btn != null)
            {
                btn.Visible = visible;
            }
        }

        private Button ResolveButtonReference(ReportButton button)
        {
            Button btn = null;
            switch (button)
            {
                case ReportButton.Run:
                    btn = btnRunReport;
                    break;
                case ReportButton.CSV:
                    btn = btnExportCSV;
                    break;
                case ReportButton.XLSX:
                    btn = btnExportXLSX;
                    break;
                case ReportButton.PDF:
                    btn = btnExportPDF;
                    break;
                case ReportButton.Favorite:
                    btn = btnSaveFavorite;
                    break;
            }
            return btn;
        }

        protected void btnExportCSV_Click(object sender, EventArgs e)
        {
            NHPortalUtilities.ExportReportToCsv(UserReport, Response);
        }

        protected void btnExportXLSX_Click(object sender, EventArgs e)
        {
            NHPortalUtilities.ExportReportToXLSX(UserReport, Response);
        }

        protected void btnExportPDF_Click(object sender, EventArgs e)
        {
            NHPortalUtilities.ExportReportToPDF(UserReport, Response);
        }



        /// <summary>Gets a reference to the ReportMaster's Master property.</summary>
        public PortalMaster MasterPage
        {
            get { return this.Master; }
        }

        /// <summary>Gets a reference to the Literal control for the report's output.</summary>
        internal Literal ReportLiteral
        {
            get { return this.ltrlReport; }
        }

        /// <summary>Gets a reference to the error Label.</summary>
        internal Label ErrorLabel
        {
            get { return this.lblError; }
        }

        ///// <summary>Gets a reference to the Run Report button.</summary>
        //public Button RunButton
        //{
        //    get { return this.btnRunReport; }
        //}

        ///// <summary>Gets a reference to the favorite title TextBox.</summary>
        //public TextBox FavTitleTextBox
        //{
        //    get { return this.tbFavTitle; }
        //}

        /// <summary>Gets a reference to the favorite description TextBox.</summary>
        internal TextBox FavDescTextBox
        {
            get { return this.tbFavDesc; }
        }

        /// <summary>Gets or sets the user's report generated by the page.</summary>
        public Report UserReport { get; set; }
    }

    /// <summary>Enum for referencing the buttons available on the ReportMaster page.</summary>
    public enum ReportButton
    {
        /// <summary>Represents the "Run Report" button.</summary>
        Run,
        /// <summary>Represents the "Export CSV" button.</summary>
        CSV,
        /// <summary>Represents the "Export XLSX" button.</summary>
        XLSX,
        /// <summary>Represents the "Export PDF" button.</summary>
        PDF,
        /// <summary>Represents the "Save Favorite" button.</summary>
        Favorite,
    }
}