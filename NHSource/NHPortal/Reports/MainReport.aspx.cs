using NHPortal.Classes;
using NHPortal.Classes.User;
using Oracle.DataAccess.Client;
using PortalFramework.ReportModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NHPortal.Reports
{
    public partial class MainReport : PortalPage
    {
        private DataRow m_currentRow;
        private MainReportRowInfoCollection m_reportData;

        protected void Page_Load(object sender, EventArgs e)
        {
            RedirectOnInvalidPermission(UserPermissions.MainReport);

            if (IsPostBack)
            {
                ProcessPostBack();
            }
            else
            {
                InitializePage();
                ParseQueryStrings();
                LoadFavorite();
                RunReport();
            }
        }

        private void InitializePage()
        {
            Master.SetHeaderText("Main Report");
            lstTestSeq.Initialize();
            lstModelYear.Initialize();
            lstVehicleType.Initialize();
            lstCounty.Initialize();
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

        private void ParseQueryStrings()
        {
            if (Request.QueryString["cnty"] != null)
            {
                string cnty = Request.QueryString["cnty"];
                lstCounty.SetSelectedValue(cnty);

                // set to start of month for month to date report
                dateSelector.StartDateControl.Text = DateTime.Now.ToString("M/1/yyyy");
            }
        }

        private void SaveFavorite()
        {
            Master.SaveFavorite(NHPortal.UserControls.RedirectCodes.REPORT_MAIN, UserFavoriteTypes.Report);
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
                        case "START DATE":
                            dateSelector.StartDateControl.Text = c.Value;
                            break;
                        case "END DATE":
                            dateSelector.EndDateControl.Text = c.Value;
                            break;
                        case "COUNTY":
                            lstCounty.SetSelectedValues(c.Value.Split(new char[] { ',' }));
                            break;
                        case "MODEL YEAR":
                            lstModelYear.SetSelectedValues(c.Value.Split(new char[] { ',' }));
                            break;
                        case "TEST SEQUENCE":
                            lstTestSeq.SetSelectedValues(c.Value.Split(new char[] { ',' }));
                            break;
                        case "VEHICLE TYPE":
                            lstVehicleType.SetSelectedValues(c.Value.Split(new char[] { ',' }));
                            break;
                    }
                }
            }
        }

        private void RunReport()
        {
            string qry = BuildQuery();
            GDDatabaseClient.Oracle.OracleResponse response = PortalFramework.Database.ODAP.GetDataTable(qry, PortalFramework.Database.DatabaseTarget.Adhoc);
            if (response.Successful)
            {
                Master.UserReport = new Report("Main Report");
                SetMetaData();
                Master.SetDataLastUpdate(DateTime.Now);

                if (response.HasResults)
                {
                    AddColumnsToReport();
                    ApplyDataTableToReport(response.ResultsTable);
                    Master.UserReport.FooterNote = "* Percentages apply to columns." + Environment.NewLine
                                  + "** Total Tests includes Completed tests, Aborted tests and Administrative Certificates." + Environment.NewLine
                                  + "*** Aborts and Administrative Certificates are not included in Inspection Results." + Environment.NewLine
                                  + "Please reference the document entitled Notes for Interpreting Report Data for further information.";
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

        private void AddColumnsToReport()
        {
            if (Master.UserReport != null)
            {
                Master.UserReport.Columns.Add("desc", "Description");
                Master.UserReport.Columns.Add("total_tests", "Total", ColumnDataType.Number);
                Master.UserReport.Columns.Add("total_tests_percent", "%*", ColumnDataType.Percentage);
                Master.UserReport.Columns.Add("init_tests", "Initial Tests", ColumnDataType.Number);
                Master.UserReport.Columns.Add("init_tests_percent", "IT %*", ColumnDataType.Percentage);
                Master.UserReport.Columns.Add("init_retests", "IT %*", ColumnDataType.Number);
                Master.UserReport.Columns.Add("init_retests_percent", "IR %*", ColumnDataType.Percentage);
                Master.UserReport.Columns.Add("other_retests", "Other Retests", ColumnDataType.Number);
                Master.UserReport.Columns.Add("other_retests_percent", "OR %*", ColumnDataType.Percentage);
            }
        }

        private void SetMetaData()
        {
            if (Master.UserReport != null)
            {
                Master.UserReport.MetaData.Add("Start Date", dateSelector.StartDateControl.Text);
                Master.UserReport.MetaData.Add("End Date", dateSelector.EndDateControl.Text);

                if (lstTestSeq.ValuesSelected)
                {
                    Master.UserReport.MetaData.Add("Test Sequence", lstTestSeq.GetDelimitedText(", "), lstTestSeq.GetDelimitedValues(","));
                }

                if (lstModelYear.ValuesSelected)
                {
                    Master.UserReport.MetaData.Add("Model Year", lstModelYear.GetDelimitedText(", "), lstModelYear.GetDelimitedValues(","));
                }

                if (lstCounty.ValuesSelected)
                {
                    Master.UserReport.MetaData.Add("County", lstCounty.GetDelimitedText(", "), lstCounty.GetDelimitedValues(","));
                }

                if (lstVehicleType.ValuesSelected)
                {
                    Master.UserReport.MetaData.Add("Vehicle Type", lstVehicleType.GetDelimitedText(", "), lstVehicleType.GetDelimitedValues(","));
                }
            }
        }

        private int GetCellValue(string column)
        {
            return GDCoreUtilities.NullSafe.ToInt(m_currentRow[column].ToString());
        }

        private void ApplyDataTableToReport(DataTable dt)
        {
            m_reportData = new MainReportRowInfoCollection();
            CalculateTotals(dt);
            AddDataToReport();
        }

        private void CalculateTotals(DataTable dt)
        {
            for (int i = 1; i <= dt.Rows.Count; i++)
            {
                m_currentRow = dt.Rows[i - 1];

                //sum up total for each test sequence
                m_reportData.TotalTests[0] += GetCellValue("OVERALL_PASS") + GetCellValue("OVERALL_REJECT") + GetCellValue("OVERALL_CORRECTED") + GetCellValue("OVERALL_ABORT") + GetCellValue("OVERALL_ADMIN");
                m_reportData.TotalTests[i] += GetCellValue("OVERALL_PASS") + GetCellValue("OVERALL_REJECT") + GetCellValue("OVERALL_CORRECTED") + GetCellValue("OVERALL_ABORT") + GetCellValue("OVERALL_ADMIN");
                m_reportData.TotalDeficiencies[0] += GetCellValue("OVERALL_REJECT") + GetCellValue("OVERALL_CORRECTED");
                m_reportData.TotalDeficiencies[i] += GetCellValue("OVERALL_REJECT") + GetCellValue("OVERALL_CORRECTED");
                m_reportData.TotalSafetyDeficiencies[0] += GetCellValue("SAFETY_REJECT") + GetCellValue("SAFETY_CORRECTED"); ;
                m_reportData.TotalSafetyDeficiencies[i] += GetCellValue("SAFETY_REJECT") + GetCellValue("SAFETY_CORRECTED"); ;
                m_reportData.TotalOBDDeficiencies[0] += GetCellValue("OBD_REJECT");
                m_reportData.TotalOBDDeficiencies[i] += GetCellValue("OBD_REJECT");
                m_reportData.TotalVisualDeficiencies[0] += GetCellValue("VISUAL_REJECT") + GetCellValue("VISUAL_CORRECTED");
                m_reportData.TotalVisualDeficiencies[i] += GetCellValue("VISUAL_REJECT") + GetCellValue("VISUAL_CORRECTED");
                m_reportData.TotalRejects[0] += GetCellValue("OVERALL_REJECT");
                m_reportData.TotalRejects[i] += GetCellValue("OVERALL_REJECT");
                m_reportData.TotalSafetyRejects[0] += GetCellValue("SAFETY_REJECT");
                m_reportData.TotalSafetyRejects[i] += GetCellValue("SAFETY_REJECT");
                m_reportData.TotalOBDReject[0] += GetCellValue("OBD_REJECT");
                m_reportData.TotalOBDReject[i] += GetCellValue("OBD_REJECT");
                m_reportData.TotalVisualReject[0] += GetCellValue("VISUAL_REJECT");
                m_reportData.TotalVisualReject[i] += GetCellValue("VISUAL_REJECT");
                m_reportData.TotalAborts[0] += GetCellValue("OVERALL_ABORT");
                m_reportData.TotalAborts[i] += GetCellValue("OVERALL_ABORT");
                m_reportData.TotalAdminCerts[0] += GetCellValue("OVERALL_ADMIN");
                m_reportData.TotalAdminCerts[i] += GetCellValue("OVERALL_ADMIN");

                m_reportData.OBDPass[0] += GetCellValue("OBD_PASS");
                m_reportData.OBDPass[i] += GetCellValue("OBD_PASS");
                m_reportData.OBDRejected[0] += GetCellValue("OBD_REJECT");
                m_reportData.OBDRejected[i] += GetCellValue("OBD_REJECT");
                m_reportData.OBDUntested[0] += GetCellValue("OBD_UNTESTED");
                m_reportData.OBDUntested[i] += GetCellValue("OBD_UNTESTED");
                //OBD_ABORT_COUNT[0] += GetCellValue("OBD_ABORT");
                //OBD_ABORT_COUNT[i] += GetCellValue("OBD_ABORT");               

                m_reportData.VisualPass[0] += GetCellValue("VISUAL_PASS");
                m_reportData.VisualPass[i] += GetCellValue("VISUAL_PASS");
                m_reportData.VisualCorrected[0] += GetCellValue("VISUAL_CORRECTED");
                m_reportData.VisualCorrected[i] += GetCellValue("VISUAL_CORRECTED");
                m_reportData.VisualRejected[0] += GetCellValue("VISUAL_REJECT");
                m_reportData.VisualRejected[i] += GetCellValue("VISUAL_REJECT");
                m_reportData.VisualUntested[0] += GetCellValue("VISUAL_UNTESTED");
                m_reportData.VisualUntested[i] += GetCellValue("VISUAL_UNTESTED");
                m_reportData.VisualPassRate[0] += GetCellValue("VISUAL_PASS") + GetCellValue("VISUAL_CORRECTED");
                m_reportData.VisualPassRate[i] += GetCellValue("VISUAL_PASS") + GetCellValue("VISUAL_CORRECTED");
                m_reportData.VisualDefRate[0] += GetCellValue("VISUAL_REJECT") + GetCellValue("VISUAL_CORRECTED");
                m_reportData.VisualDefRate[i] += GetCellValue("VISUAL_REJECT") + GetCellValue("VISUAL_CORRECTED");
                //VISUAL_ABORT_COUNT[0] += GetCellValue("VISUAL_ABORT");
                //VISUAL_ABORT_COUNT[i] += GetCellValue("VISUAL_ABORT");

                m_reportData.SafetyPass[0] += GetCellValue("SAFETY_PASS");
                m_reportData.SafetyPass[i] += GetCellValue("SAFETY_PASS");
                m_reportData.SafetyCorrected[0] += GetCellValue("SAFETY_CORRECTED");
                m_reportData.SafetyCorrected[i] += GetCellValue("SAFETY_CORRECTED");
                m_reportData.SafetyRejected[0] += GetCellValue("SAFETY_REJECT");
                m_reportData.SafetyRejected[i] += GetCellValue("SAFETY_REJECT");
                m_reportData.SafetyUntested[0] += GetCellValue("SAFETY_UNTESTED");
                m_reportData.SafetyUntested[i] += GetCellValue("SAFETY_UNTESTED");
                m_reportData.SafetyPassrate[0] += GetCellValue("SAFETY_PASS") + GetCellValue("SAFETY_CORRECTED");
                m_reportData.SafetyPassrate[i] += GetCellValue("SAFETY_PASS") + GetCellValue("SAFETY_CORRECTED");
                m_reportData.SafetyDefRate[0] += GetCellValue("SAFETY_REJECT") + GetCellValue("SAFETY_CORRECTED");
                m_reportData.SafetyDefRate[i] += GetCellValue("SAFETY_REJECT") + GetCellValue("SAFETY_CORRECTED");
                //SAFETY_ABORT_COUNT[0] += GetCellValue("SAFETY_ABORT");
                //SAFETY_ABORT_COUNT[i] += GetCellValue("SAFETY_ABORT");
            }

            //calculate the totals for each vertical section.
            for (int i = 0; i < 4; i++)
            {
                m_reportData.OBDTotal[i] = m_reportData.OBDPass[i] + m_reportData.OBDRejected[i] + m_reportData.OBDUntested[i];
                m_reportData.VisualTotal[i] = m_reportData.VisualPass[i] + m_reportData.VisualCorrected[i]
                    + m_reportData.VisualRejected[i] + m_reportData.VisualUntested[i];
                m_reportData.SafetyTotal[i] = m_reportData.SafetyPass[i] + m_reportData.SafetyCorrected[i] + m_reportData.SafetyRejected[i]
                    + m_reportData.SafetyUntested[i];
            }
        }

        private void AddDataToReport()
        {
            int idx = 0;
            foreach (var data in m_reportData)
            {
                AddRowToReport(data, idx);
                idx++;
            }
        }

        private void AddRowToReport(MainReportRowInfo data, int rowIndex)
        {
            ReportRow rptRow = new ReportRow(Master.UserReport);
            rptRow.Cells.Add(data.Description);

            if (data.ContainsData)
            {
                for (int i = 0; i < 4; i++)
                {
                    int denom = m_reportData.GetDenominator(rowIndex, i);
                    rptRow.Cells.Add(data[i]);
                    rptRow.Cells.Add(PercentString(data[i], denom));
                }
            }
            else
            {
                rptRow.Cells[0].ColumnSpan = Master.UserReport.ColumnCount;
            }

            Master.UserReport.Rows.Add(rptRow);
        }

        private string PercentString(int num, int den)
        {
            string percent = "0.0%";
            if (den != 0)
            {
                double d = (double)num / (double)den;
                percent = d.ToString("0.0%");
            }
            return percent;
        }

        private string BuildQuery()
        {
            string partitionBy = "case when testsequence = '01' then 1"
                               + " when testsequence = '02' then 2"
                               + " else 3 end";

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("SELECT distinct ");
            sb.AppendLine("    case when testsequence = '01' then 1 ");
            sb.AppendLine("         when testsequence = '02' then 2");
            sb.AppendLine("         else 3");
            sb.AppendLine("    end AS TEST_SEQ");
            sb.AppendLine("    , sum( case when OVERALLPF = '1' then 1 else 0 end ) over ( partition by " + partitionBy + " ) OVERALL_PASS");
            sb.AppendLine("    , sum( case when OVERALLPF = '2' then 1 else 0 end) over ( partition by " + partitionBy + " ) OVERALL_REJECT");
            sb.AppendLine("    , sum( case when OVERALLPF = '3' then 1 else 0 end) over ( partition by " + partitionBy + " ) OVERALL_CORRECTED");
            sb.AppendLine("    , sum( case when OVERALLPF = '5' then 1 else 0 end) over ( partition by " + partitionBy + " ) OVERALL_ADMIN");
            sb.AppendLine("    , sum( case when OVERALLPF not in ('1','2','3','5') then 1 else 0 end) over ( partition by " + partitionBy + " ) OVERALL_ABORT");
            sb.AppendLine("    , sum( case when EMISSTESTTYPE = 'O' and OBDPF = '0' then 1 else 0 end) over ( partition by " + partitionBy + " ) OBD_ABORT");
            sb.AppendLine("    , sum( case when EMISSTESTTYPE = 'O' and OBDPF = '1' then 1 else 0 end) over ( partition by " + partitionBy + " ) OBD_PASS");
            sb.AppendLine("    , sum( case when EMISSTESTTYPE = 'O' and OBDPF = '2' then 1 else 0 end) over ( partition by " + partitionBy + " ) OBD_REJECT");
            sb.AppendLine("    , sum( case when EMISSTESTTYPE = 'O' and OBDPF = '2' and substr(OBDMILCMDON,2,1) = '3' then 1 else 0 end) over ( partition by " + partitionBy + " ) OBD_NO_COM");
            sb.AppendLine("    , sum( case when EMISSTESTTYPE = 'O' and OBDPF = '9' then 1 else 0 end) over ( partition by " + partitionBy + " ) OBD_UNTESTED");
            sb.AppendLine("    , sum( case when EMISSTESTTYPE = 'V' and VISUALPF = '0' then 1 else 0 end) over ( partition by " + partitionBy + " ) VISUAL_ABORT");
            sb.AppendLine("    , sum( case when EMISSTESTTYPE = 'V' and VISUALPF = '1' then 1 else 0 end) over ( partition by " + partitionBy + " ) VISUAL_PASS");
            sb.AppendLine("    , sum( case when EMISSTESTTYPE = 'V' and VISUALPF = '2' then 1 else 0 end) over ( partition by " + partitionBy + " ) VISUAL_REJECT");
            sb.AppendLine("    , sum( case when EMISSTESTTYPE = 'V' and VISUALPF = '3' then 1 else 0 end) over ( partition by " + partitionBy + " ) VISUAL_CORRECTED");
            sb.AppendLine("    , sum( case when EMISSTESTTYPE = 'V' and VISUALPF = '9' then 1 else 0 end) over ( partition by " + partitionBy + " ) VISUAL_UNTESTED");
            sb.AppendLine("    , sum( case when SAFETYPF = '0' then 1 else 0 end) over ( partition by " + partitionBy + " ) SAFETY_ABORT");
            sb.AppendLine("    , sum( case when SAFETYPF = '1' then 1 else 0 end) over ( partition by " + partitionBy + " ) SAFETY_PASS");
            sb.AppendLine("    , sum( case when SAFETYPF = '2' then 1 else 0 end) over ( partition by " + partitionBy + " ) SAFETY_REJECT");
            sb.AppendLine("    , sum( case when SAFETYPF = '3' then 1 else 0 end) over ( partition by " + partitionBy + " ) SAFETY_CORRECTED");
            sb.AppendLine("    , sum( case when SAFETYPF = '9' then 1 else 0 end) over ( partition by " + partitionBy + " ) SAFETY_UNTESTED");
            sb.AppendLine("FROM new_testrecord");
            sb.AppendLine(BuildWhere());
            sb.AppendLine("ORDER BY test_seq");

            return sb.ToString();
        }

        private string BuildWhere()
        {
            // TODO: use oracle parameters, clean up method ?
            List<string> clauses = new List<string>();

            clauses.Add(lstTestSeq.GetOracleText());
            clauses.Add(lstModelYear.GetOracleText());
            clauses.Add(lstVehicleType.GetOracleText());
            clauses.Add(lstCounty.GetOracleText());
            clauses.Add("testdate >= '" + dateSelector.StartDateControl.GetDateText("yyyyMMdd") + "'");
            clauses.Add("testdate <= '" + dateSelector.EndDateControl.GetDateText("yyyyMMdd") + "'");

            string[] wheres = clauses.Where(x => !String.IsNullOrWhiteSpace(x)).ToArray();

            StringBuilder sb = new StringBuilder();
            foreach (string w in wheres)
            {
                if (w == wheres.First())
                {
                    sb.AppendLine("WHERE " + w);
                }
                else
                {
                    sb.AppendLine("AND   " + w);
                }
            }
            return sb.ToString();
        }
    }

    /// <summary>Collection of information for the main report.</summary>
    public class MainReportRowInfoCollection : IEnumerable<MainReportRowInfo>
    {
        private List<MainReportRowInfo> info;

        /// <summary>Instantiates a new instance of the MainReportRowInfoCollection class.</summary>
        public MainReportRowInfoCollection()
        {
            info = new List<MainReportRowInfo>();
            info.Add(new MainReportRowInfo("TOTAL TESTS**"));
            info.Add(new MainReportRowInfo("  TOTAL DEFICIENCIES (REJECTED and CORRECTED)"));
            info.Add(new MainReportRowInfo("    SAFETY"));
            info.Add(new MainReportRowInfo("    OBD"));
            info.Add(new MainReportRowInfo("    VISUAL"));
            info.Add(new MainReportRowInfo("  TOTAL REJECTIONS"));
            info.Add(new MainReportRowInfo("    SAFETY"));
            info.Add(new MainReportRowInfo("    OBD**"));
            info.Add(new MainReportRowInfo("    VISUAL"));
            info.Add(new MainReportRowInfo("  ABORTS"));
            info.Add(new MainReportRowInfo("  ADMIN CERTS"));
            info.Add(new MainReportRowInfo("SAFETY INSPECTION RESULTS***", false));
            info.Add(new MainReportRowInfo("  TOTAL"));
            info.Add(new MainReportRowInfo("  PASS RATE (PASS and CORRECTED)"));
            info.Add(new MainReportRowInfo("  DEFICIENCY RATE (PASS and CORRECTED)"));
            info.Add(new MainReportRowInfo("  PASS"));
            info.Add(new MainReportRowInfo("  CORRECTED"));
            info.Add(new MainReportRowInfo("  REJECTED"));
            info.Add(new MainReportRowInfo("  UNTESTED"));
            info.Add(new MainReportRowInfo("OBD INSPECTION RESULTS***", false));
            info.Add(new MainReportRowInfo("  TOTAL"));
            info.Add(new MainReportRowInfo("  PASS"));
            info.Add(new MainReportRowInfo("  REJECTED"));
            info.Add(new MainReportRowInfo("  UNTESTED"));
            info.Add(new MainReportRowInfo("VISUAL INSPECTION RESULTS***", false));
            info.Add(new MainReportRowInfo("  TOTAL"));
            info.Add(new MainReportRowInfo("  PASS RATE(PASS and CORRECTED)"));
            info.Add(new MainReportRowInfo("  DEFICIENCY RATE (PASS and CORRECTED)"));
            info.Add(new MainReportRowInfo("  PASS"));
            info.Add(new MainReportRowInfo("  CORRECTED"));
            info.Add(new MainReportRowInfo("  REJECTED"));
            info.Add(new MainReportRowInfo("  UNTESTED"));
        }

        /// <summary>Gets the denominator to use for the provided row index and test sequence number.</summary>
        /// <param name="rowIdx">Current row index.</param>
        /// <param name="seqNum">Test sequence number.</param>
        /// <returns>Value matching the </returns>
        public int GetDenominator(int rowIdx, int seqNum)
        {
            int val = 0;
            if (rowIdx.Between(0, 10))
            {
                val = TotalTests[seqNum];
            }
            else if (rowIdx.Between(11, 18))
            {
                val = SafetyTotal[seqNum];
            }
            else if (rowIdx.Between(19, 23))
            {
                val = OBDTotal[seqNum];
            }
            else if (rowIdx.Between(24, 31))
            {
                val = VisualTotal[seqNum];
            }
            return val;
        }

        public MainReportRowInfo TotalTests { get { return info[0]; } }
        public MainReportRowInfo TotalDeficiencies { get { return info[1]; } }
        public MainReportRowInfo TotalSafetyDeficiencies { get { return info[2]; } }
        public MainReportRowInfo TotalOBDDeficiencies { get { return info[3]; } }
        public MainReportRowInfo TotalVisualDeficiencies { get { return info[4]; } }
        public MainReportRowInfo TotalRejects { get { return info[5]; } }
        public MainReportRowInfo TotalSafetyRejects { get { return info[6]; } }
        public MainReportRowInfo TotalOBDReject { get { return info[7]; } }
        public MainReportRowInfo TotalVisualReject { get { return info[8]; } }
        public MainReportRowInfo TotalAborts { get { return info[9]; } }
        public MainReportRowInfo TotalAdminCerts { get { return info[10]; } }

        public MainReportRowInfo SafetyResults { get { return info[11]; } }
        public MainReportRowInfo SafetyTotal { get { return info[12]; } }
        public MainReportRowInfo SafetyPassrate { get { return info[13]; } }
        public MainReportRowInfo SafetyDefRate { get { return info[14]; } }
        public MainReportRowInfo SafetyPass { get { return info[15]; } }
        public MainReportRowInfo SafetyCorrected { get { return info[16]; } }
        public MainReportRowInfo SafetyRejected { get { return info[17]; } }
        public MainReportRowInfo SafetyUntested { get { return info[18]; } }

        public MainReportRowInfo OBDResults { get { return info[19]; } }
        public MainReportRowInfo OBDTotal { get { return info[20]; } }
        public MainReportRowInfo OBDPass { get { return info[21]; } }
        public MainReportRowInfo OBDRejected { get { return info[22]; } }
        public MainReportRowInfo OBDUntested { get { return info[23]; } }

        public MainReportRowInfo VisualResults { get { return info[24]; } }
        public MainReportRowInfo VisualTotal { get { return info[25]; } }
        public MainReportRowInfo VisualPassRate { get { return info[26]; } }
        public MainReportRowInfo VisualDefRate { get { return info[27]; } }
        public MainReportRowInfo VisualPass { get { return info[28]; } }
        public MainReportRowInfo VisualCorrected { get { return info[29]; } }
        public MainReportRowInfo VisualRejected { get { return info[30]; } }
        public MainReportRowInfo VisualUntested { get { return info[31]; } }

        /// <summary>Gets the enumerator of the list of MainReportRowInfo objects.</summary>
        /// <returns>List enumerator.</returns>
        public IEnumerator<MainReportRowInfo> GetEnumerator()
        {
            return info.GetEnumerator();
        }

        /// <summary>Gets the enumerator of the list of MainReportRowInfo objects.</summary>
        /// <returns>List enumerator.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }

    /// <summary>Stores information about a row in the main report.</summary>
    public class MainReportRowInfo
    {
        /// <summary>Instantiates a new instance of the MainReportRowInfo class.</summary>
        /// <param name="desc">Description of the row.</param>
        /// <param name="hasData">Whether or not the row contains values.  Defaults to true.</param>
        public MainReportRowInfo(string desc, bool hasData = true)
        {
            Description = desc;
            ContainsData = hasData;

            TotalTest = 0;
            InitialTest = 0;
            InitialRetest = 0;
            OtherRetest = 0;
        }

        /// <summary>
        /// Gets or sets the value found at the provided index.<para />
        /// 0 represents the total values. <para />
        /// Indexes 1 - 3 represent test sequence numbers 1 - 3.
        /// </summary>
        /// <param name="idx">Index of the test value to set.</param>
        /// <returns>Value returned from the provided index.</returns>
        public int this[int idx]
        {
            set
            {
                switch (idx)
                {
                    case 0:
                        TotalTest = value;
                        break;
                    case 1:
                        InitialTest = value;
                        break;
                    case 2:
                        InitialRetest = value;
                        break;
                    case 3:
                        OtherRetest = value;
                        break;
                }
            }
            get
            {
                int val = 0;
                switch (idx)
                {
                    case 0:
                        val = TotalTest;
                        break;
                    case 1:
                        val = InitialTest;
                        break;
                    case 2:
                        val = InitialRetest;
                        break;
                    case 3:
                        val = OtherRetest;
                        break;
                }
                return val;
            }
        }

        /// <summary>Gets the description to display in the description column of the main report.</summary>
        public string Description { get; private set; }

        /// <summary>Gets whether or not the row contains any data.</summary>
        public bool ContainsData { get; private set; }

        /// <summary>Gets or sets the total test values.</summary>
        public int TotalTest { get; set; }

        /// <summary>Gets or sets the initial test values.</summary>
        public int InitialTest { get; set; }

        /// <summary>Gets or sets the initial retest values.</summary>
        public int InitialRetest { get; set; }

        /// <summary>Gets or sets the other retest values.</summary>
        public int OtherRetest { get; set; }
    }

    // TODO: move this to a utilities library
    public static class IntExtensions
    {
        public static bool Between(this int i, int min, int max)
        {
            return (i >= min && i <= max);
        }
    }
}