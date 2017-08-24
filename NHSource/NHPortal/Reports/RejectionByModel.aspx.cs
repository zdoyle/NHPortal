﻿using NHPortal.Classes;
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
    public partial class RejectionByModel : PortalPage
    {
        private BaseReport ReportData = BaseReportMaster.RejectionByModel;
        PredefinedQueryType queryType = PredefinedQueryType.Model;

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
            string filterText = this.txtModel.Text.Trim().ToUpper().Replace(",", String.Empty);
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
                if (filterText != String.Empty)
                {
                    Master.UserReport.MetaData.Add(queryType.ToString(), filterText);
                }
            }
        }

        private void InitPage()
        {
            Master.SetHeaderText(ReportData.ReportTitle);
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
                        case "MODEL":
                            this.txtModel.Text = c.Value;
                            break;
                    }
                }
            }
        }
    }
}