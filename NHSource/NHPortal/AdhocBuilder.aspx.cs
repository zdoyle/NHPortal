using GDDatabaseClient.Oracle;
using NHPortal.Classes;
using NHPortal.Classes.Adhoc;
using NHPortal.Classes.Adhoc.WebControls;
using NHPortal.Classes.User;
using PortalFramework.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NHPortal
{
    public partial class AdhocBuilder : PortalPage
    {
        public const string REPORT_TITLE = "Adhoc Query Results";
        private const string HTML_SECTIONS_KEY = "ADHOC_BUILDER_HTML_SECTIONS";

        private System.Web.UI.HtmlControls.HtmlGenericControl[] m_htmlControls;
        private AdhocFieldRow[] m_rows;
        private AdhocQueryHelper m_qryBuilder;

        protected void Page_Init(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                m_htmlControls = this.Session[HTML_SECTIONS_KEY] as System.Web.UI.HtmlControls.HtmlGenericControl[];
            }

            RenderSections();
            m_rows = GetFieldRowControls();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                ProcessPostBack();
            }
            else
            {
                Master.SetHeaderText("Report Builder Tool");
                Master.SetButtonVisibility(NHPortal.MasterPages.ReportButton.PDF, false);
                Master.SetRunButtonText("Execute Query");

                LoadFavorite();
            }
        }

        private void ProcessPostBack()
        {
            switch (Master.MasterPage.HidActionValue)
            {
                case "RUN_REPORT":
                    RunReport();
                    break;
                case "SAVE_FAVORITE":
                    SaveFavorite();
                    break;
            }
        }

        private void RenderSections()
        {
            if (m_htmlControls == null)
            {
                AdhocRenderer renderer = new AdhocRenderer();
                renderer.Render(AdhocSections.Sections);
                m_htmlControls = renderer.HtmlSections;
                this.Session[HTML_SECTIONS_KEY] = m_htmlControls;
            }

            foreach (var html in m_htmlControls)
            {
                phAdhoc.Controls.Add(html);
            }
        }

        private AdhocFieldRow[] GetFieldRowControls()
        {
            List<AdhocFieldRow> rows = new List<AdhocFieldRow>();
            foreach (var c in GDWebUtilities.WebUtilities.GetAllControls(phAdhoc))
            {
                if (c is AdhocFieldRow)
                {
                    AdhocFieldRow row = c as AdhocFieldRow;
                    rows.Add(row);
                }
            }
            return rows.ToArray();
        }

        private void RunReport()
        {
            CreateQueryBuilder();
            if (m_qryBuilder.SelectFields.Count > 0 || m_qryBuilder.SelectAggregateFields.Count > 0)
            {
                string qry = BuildQuery();
                OracleResponse response = ODAP.GetDataTable(qry, DatabaseTarget.AdhocLimited, PortalFramework.PortalIniSettings.Values.AdhocLimitCap);
                if (response.Successful)
                {
                    Master.UserReport = new PortalFramework.ReportModel.Report(REPORT_TITLE, response.ResultsTable);
                    SetMetaData();
                    Master.SetDataLastUpdate(DateTime.Now);
                    Master.UserReport.Sortable = true;
                }
                else
                {
                    Master.UserReport = null;
                    Master.SetError(response.Exception);
                }

                LogOracleResponse(response);
                Master.RenderReportToPage();
                SessionHelper.SetCurrentReport(this.Session, Master.UserReport);
                this.Page.ClientScript.RegisterStartupScript(this.GetType(), "reportwasrendered", "reportWasRendered();", true);
            }
            else
            {
                Master.SetGenericError("No fields selected.  Please select at least one field.");
            }
        }

        private void CreateQueryBuilder()
        {
            m_qryBuilder = new AdhocQueryHelper();
            foreach (var row in m_rows)
            {
                if (row.Included || chkAllFields.Checked)
                {
                    //string select = String.Format("{0} AS \"{1}\"", row.Field.DatabaseField, row.Field.Name);
                    m_qryBuilder.SelectFields.Add(row.Field.DatabaseField + " AS \"" + row.Field.ColumnHeaderName + "\"");
                    m_qryBuilder.GroupByFields.Add(row.Field.DatabaseField);
                }

                if (row.HasInput)
                {
                    m_qryBuilder.WhereFields.Add(row.GetInputString());
                }

                foreach (var chk in row.AggregateCheckBoxes)
                {
                    if (chk.Checked)
                    {
                        string aggFunc = chk.Aggregate.GetOracleAggFunction(row.Field.DatabaseField, row.Field.ColumnHeaderName);
                        m_qryBuilder.SelectAggregateFields.Add(aggFunc);
                    }
                }
            }
        }

        private string BuildQuery()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(BuildSelect());
            sb.Append("FROM NEW_TESTRECORD");
            sb.Append(BuildWhere());
            sb.Append(BuildGroupBy());
            return sb.ToString();
        }

        private string BuildSelect()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT ");
            foreach (var select in m_qryBuilder.SelectStatements)
            {
                sb.Append(select);
                if (select != m_qryBuilder.SelectStatements.Last())
                {
                    sb.AppendLine(", ");
                }
            }
            sb.AppendLine();
            return sb.ToString();
        }

        private string BuildWhere()
        {
            StringBuilder sb = new StringBuilder();
            if (m_qryBuilder.WhereFields.Count > 0)
            {
                sb.AppendLine();
                sb.Append("WHERE ");
                foreach (var where in m_qryBuilder.WhereFields)
                {
                    sb.Append("( " + where.ToUpper() + " )");
                    if (where != m_qryBuilder.WhereFields.Last())
                    {
                        sb.AppendLine(" AND ");
                    }
                }
            }
            return sb.ToString();
        }

        private string BuildGroupBy()
        {
            StringBuilder sb = new StringBuilder();
            if (m_qryBuilder.SelectAggregateFields.Count > 0 && m_qryBuilder.SelectFields.Count > 0)
            {
                sb.AppendLine();
                sb.AppendLine("GROUP BY");
                foreach (var select in m_qryBuilder.GroupByFields)
                {
                    sb.Append(select);
                    if (select != m_qryBuilder.GroupByFields.Last())
                    {
                        sb.AppendLine(", ");
                    }
                }
            }
            return sb.ToString();
        }

        private void SetMetaData()
        {
            foreach (var row in m_rows)
            {
                if (row.HasInput)
                {
                    Master.UserReport.MetaData.Add(row.Field.Name, row.Input);
                }
            }
        }

        private void SaveFavorite()
        {
            AdhocFavoriteHelper favHelper = new AdhocFavoriteHelper(m_rows);
            favHelper.AllFieldsCheckBox = chkAllFields;
            bool success = favHelper.CreateFavorite(PortalUser.UserSysNo, Master.FavDescTextBox.Text);
            Master.SetFavoriteResultPrompt(success);
        }

        private void LoadFavorite()
        {
            UserFavorite fav = SessionHelper.GetSelectedFavorite(this.Session);
            AdhocFavoriteHelper favHelper = new AdhocFavoriteHelper(m_rows);
            favHelper.AllFieldsCheckBox = chkAllFields;
            favHelper.LoadFavorite(fav);
        }
    }
}