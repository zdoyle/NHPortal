using GDDatabaseClient.Oracle;
using NHPortal.Classes;
using NHPortal.Classes.User;
using PortalFramework.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NHPortal
{
    public partial class SQLEditor : PortalPage
    {
        private const string FAV_CRITERIA = "Query String";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                ProcessPostBack();
            }
            else
            {
                InitializePage();
                LoadFavorite();
                if (String.IsNullOrEmpty(tbEditor.Text))
                {
                    // If textbox is empty, no favorite was loaded -- default the textbox's text
                    tbEditor.Text = "SELECT " + Environment.NewLine + "FROM NEW_TESTRECORD";
                }
            }
        }

        private void InitializePage()
        {
            Master.SetHeaderText("SQL Report Builder");
            Master.SetRunButtonText("Execute Query");
            Master.SetButtonVisibility(MasterPages.ReportButton.PDF, false);

            if (PortalFramework.PortalIniSettings.Values.AdhocLimitCap > 0)
            {
                lblNote.Text = String.Format("Note: Results limited to {0} records.", PortalFramework.PortalIniSettings.Values.AdhocLimitCap);
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
            string qry = tbEditor.Text;
            OracleResponse response = ODAP.GetDataTable(qry, DatabaseTarget.AdhocLimited, PortalFramework.PortalIniSettings.Values.AdhocLimitCap);
            if (response.Successful)
            {
                Master.UserReport = new PortalFramework.ReportModel.Report("Query Results", response.ResultsTable);
                Master.SetDataLastUpdate(DateTime.Now);
                Master.UserReport.Sortable = true;
            }
            else
            {
                Master.UserReport = null;

                if (response.ErrorCode == 903)
                {
                    Master.SetError("Invalid Query: Only select statements are allowed.");
                }
                else
                {
                    Master.SetError(response.Exception);
                }
            }

            LogOracleResponse(response);
            Master.RenderReportToPage();
            SessionHelper.SetCurrentReport(this.Session, Master.UserReport);
        }

        private void SaveFavorite()
        {
            UserFavorite fav = new UserFavorite();
            fav.Title = Master.UserReport.Title;
            fav.Description = Master.FavDescTextBox.Text;
            fav.NavCode = NHPortal.UserControls.RedirectCodes.SQL_BUILDER;
            fav.FavType = UserFavoriteTypes.QueryBuilder;

            fav.AddCriteria(FAV_CRITERIA, FAV_CRITERIA, tbEditor.Text);

            bool success = fav.Save();
            Master.SetFavoriteResultPrompt(success);
        }

        private void LoadFavorite()
        {
            UserFavorite fav = SessionHelper.GetSelectedFavorite(this.Session);
            if (fav != null)
            {
                tbEditor.Text = fav.GetCriteriaValue(FAV_CRITERIA);
            }
        }
    }
}