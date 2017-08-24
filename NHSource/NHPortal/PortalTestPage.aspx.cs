using NHPortal.Classes;
using PortalFramework.ReportModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NHPortal
{
    public partial class PortalTestPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            selectorTestTwo.StartDateControl.SetValidatorVisibility(ValidatorDisplay.None);
            selectorTestTwo.EndDateControl.SetValidatorVisibility(ValidatorDisplay.None);
        }

        //protected void btnTestCSVReader_Click(object sender, EventArgs e)
        //{
        //    string file = System.IO.Path.Combine(NHPortalUtilities.WebRoot, "ComponentInventoryReport_RepYr2017.csv");

        //    string file2 = System.IO.Path.Combine(NHPortalUtilities.WebRoot, "STICKERDENIAL_20170418.csv");

        //    string file3 = System.IO.Path.Combine(NHPortalUtilities.WebRoot, "test.csv");

        //    try
        //    {
        //        CSVReader csvReader = new CSVReader(file2);
        //        csvReader.MultiLineHeader = false; // needed for file 2 and 3
        //        csvReader.Process();

        //        if (csvReader.Table != null)
        //        {
        //            Master.UserReport = new PortalFramework.ReportModel.Report("Test CSV Reader Report", csvReader.Table);
        //            Master.RenderReportToPage();
        //        }
        //    }
        //    catch (IOException ex)
        //    {
        //        // TODO: log
        //    }
        //}

        protected void btnTestEx_Click(object sender, EventArgs e)
        {
            try
            {
                throw new Exception("Testing a handled exception!");
            }
            catch (Exception ex)
            {
                NHPortalUtilities.LogApplicationException(ex);
            }
        }

        protected void btnTestEx2_Click(object sender, EventArgs e)
        {
            throw new Exception("Unhandled Exception!!!!11!11!1!!");
        }

        protected void btnTestMsg_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(tbMsg.Text))
            {
                Master.MasterPage.SetMessagePrompt(tbMsg.Text);
            }
        }

        protected void btnReportTest_Click(object sender, EventArgs e)
        {
            CreateTestReport();
            AddTestTableOne();
            AddTestTableTwo();
            Master.RenderReportToPage();
            SessionHelper.SetCurrentReport(this.Session, Master.UserReport);
        }

        private void CreateTestReport()
        {
            Master.UserReport = new Report("My Test Report");
            Master.UserReport.Columns.Add("COL_A", "First Column");
            Master.UserReport.Columns.Add("COL_B", "Second Column");
            Master.UserReport.Columns.Add("COL_C", "Third Column");
            Master.UserReport.Columns.Add("COL_D", "Fourth Column");

            ReportRow rptRow;
            for (int i = 0; i < 100; i++)
            {
                rptRow = new ReportRow(Master.UserReport);
                rptRow.Cells.Add(i * 1);
                rptRow.Cells.Add(i * 2);
                rptRow.Cells.Add(i * 3);
                rptRow.Cells.Add(i * 4);
                Master.UserReport.Rows.Add(rptRow);
            }
        }

        private void AddTestTableOne()
        {
            string qry = "SELECT    grp.RUPG_CD" + Environment.NewLine
                       + "        , grp.RUPG_DESC" + Environment.NewLine
                       + "        , grp.RUPG_ACTIVE" + Environment.NewLine
                       + "        , grp.RUPG_DISPLAY_ORDER" + Environment.NewLine
                       + "FROM    R_USER_PERMISSION_GROUP grp" + Environment.NewLine
                       + "ORDER BY grp.RUPG_DISPLAY_ORDER, grp.RUPG_CD";

            GDDatabaseClient.Oracle.OracleResponse response = PortalFramework.Database.ODAP.GetDataTable(qry,
                PortalFramework.Database.DatabaseTarget.Adhoc);

            ReportTable tbl = new ReportTable(response.ResultsTable);
            tbl.RenderLocation = RenderLocation.Above;
            Master.UserReport.Tables.Add(tbl);
        }

        private void AddTestTableTwo()
        {
            ReportTable tbl = new ReportTable();
            tbl.RenderLocation = RenderLocation.Below;
            tbl.Title = "Test Table Twooo";
            tbl.Columns.Add("COL_A", "Column A");
            tbl.Columns.Add("COL_B", "Column B");
            tbl.Columns.Add("COL_C", "Column C");
            tbl.Columns.Add("COL_D", "Column D");
            tbl.Columns.Add("COL_DISPLAY", "Display Only!");
            tbl.Columns.Add("COL_EXPORT", "Exportable!");

            tbl.Columns["COL_DISPLAY"].Exportable = false;
            tbl.Columns["COL_EXPORT"].Visible = false;

            ReportRow rptRow = new ReportRow(tbl);
            rptRow.Cells.Add("A");
            rptRow.Cells.Add("B");
            rptRow.Cells.Add("C");
            rptRow.Cells.Add("D");
            rptRow.Cells.Add("DISPLAY");
            rptRow.Cells.Add("I am the export value!");
            tbl.Rows.Add(rptRow);

            Master.UserReport.Tables.Add(tbl);
        }
    }
}