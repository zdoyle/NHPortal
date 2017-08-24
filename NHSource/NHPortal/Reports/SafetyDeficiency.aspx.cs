using NHPortal.Classes;
using NHPortal.Classes.User;
using PortalFramework.Database;
using PortalFramework.ReportModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;
using System.Web.UI.HtmlControls;
using System.Web.Mvc;
using GDCoreUtilities.Logging;

namespace NHPortal.Reports
{
    public partial class SafetyDeficiency : PortalPage
    {
        private string[] m_selectedRows;
        //private DataRow d_currentRow;
        int[] columnTotals = new int[100];

        protected void Page_Load(object sender, EventArgs e)
        {
            RedirectOnInvalidPermission(UserPermissions.SafetyDeficiency);

            if (IsPostBack)
            {
                ProcessPostBack();
            }
            else
            {
                InitializePage();

                // Set default selection for VehicleType to Basic
                lstVehicleType.SetSelectedText("Basic");

                ParseQueryStrings();
                LoadFavorite();
                RunReport();
            }

        }

        private void InitializePage()
        {
            Master.SetHeaderText("Safety Deficiency Report");
            lstTestSeq.Initialize();
            lstModelYear.Initialize();
            lstWeightClass.Initialize();
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
                default: break;
            }
        }

        private void ParseQueryStrings()
        {
            // set to start of month for month to date report
            dateSelector.StartDateControl.Text = DateTime.Now.ToString("M/1/yyyy");
        }

        private void SetColumnTypes()
        {
            for (int i = 1; i < Master.UserReport.ColumnCount; i++)
            {
                Master.UserReport.Columns[i].ColumnDataType = ColumnDataType.Number;
            }
        }

        private void SaveFavorite()
        {
            Master.SaveFavorite(NHPortal.UserControls.RedirectCodes.REPORT_SAFETY_DEFICIENCY, UserFavoriteTypes.Report);
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
                        case "WEIGHT CLASS":
                            lstWeightClass.SetSelectedValues(c.Value.Split(new char[] { ',' }));
                            break;
                    }
                }
            }
        }

        private void RunReport()
        {
            string qry = BuildQuery();
            GDDatabaseClient.Oracle.OracleResponse response = ODAP.GetDataTable(qry, DatabaseTarget.Adhoc);
            LogMessage("Attempting to run Safety Deficiency query...", LogSeverity.Trace);
            if (response.Successful)
            {
                LogMessage("Safety Deficiency query is successful", LogSeverity.Trace);
                Master.UserReport = new Report("NH Safety Inspection Data");
                SetMetaData();
                Master.SetDataLastUpdate(DateTime.Now);
                if (response.HasResults)
                {
                    LogMessage("Safety Deficiency query has results", LogSeverity.Trace);
                    /////////Master.UserReport = new Report("Safety Deficiency", response.ResultsTable);
                    AddColumnsToReport();
                    ApplyDataTableToReport(response.ResultsTable);
                    SetColumnTypes();
                    Master.UserReport.HeaderNote = "Item Defect Statistics";
                    Master.UserReport.FooterNote = "* Aborts and Administrative Certificates not included." + Environment.NewLine;
                    AddDetailsTable(response.ResultsTable);
                    Master.UserReport.Sortable = true;
                    Master.UserReport.FooterVisibility = FooterVisibility.Auto;
                    FormatRowName();
                    LogMessage("Safety Deficiency data rendering to page", LogSeverity.Trace);
                }
            }
            else
            {
                Master.UserReport = null;
                Master.SetError(response.Exception);
                LogMessage("Safety Deficiency query error: " + response.Exception, LogSeverity.Warning);
            }
            LogMessage("Safety Deficiency query Finshed", LogSeverity.Trace);
            LogOracleResponse(response);
            Master.RenderReportToPage();
            LogMessage("Safety Deficiency page Loaded", LogSeverity.Trace);
            SessionHelper.SetCurrentReport(this.Session, Master.UserReport);
        }
        //private void RunDetailReport()
        //{
        //    string qry = BuildDetailQuery();
        //    GDDatabaseClient.Oracle.OracleResponse response = ODAP.GetDataTable(qry, DatabaseTarget.Adhoc);
        //    LogMessage("Attempting to run Safety Deficiency Details query...", LogSeverity.Trace);
        //    if (response.Successful)
        //    {
        //        LogMessage("Safety Deficiency Details query is successful", LogSeverity.Trace);

        //        if (response.HasResults)
        //        {
        //            AddDetailsTable(response.ResultsTable);
        //            LogMessage("Safety Deficiency  Details data rendering to page", LogSeverity.Trace);
        //        }
        //    }
        //    else
        //    {
        //        Master.UserReport = null;
        //        Master.SetError(response.Exception);
        //        LogMessage("Safety Deficiency  Detailsquery error: " + response.Exception, LogSeverity.Warning);
        //    }
        //    LogMessage("Safety Deficiency Details query Finshed", LogSeverity.Trace);
        //    LogOracleResponse(response);
        //    Master.RenderReportToPage();
        //    LogMessage("Safety Deficiency Details page Loaded", LogSeverity.Trace);
        //    SessionHelper.SetCurrentReport(this.Session, Master.UserReport);
        //}

        private void AddColumnsToReport()
        {
            if (Master.UserReport != null)
            {
                Master.UserReport.Columns.Add("item", "ITEM*");
                Master.UserReport.Columns.Add("pass", "PASS", ColumnDataType.Number);
                Master.UserReport.Columns.Add("pass_percent", "PASS %", ColumnDataType.Percentage);
                Master.UserReport.Columns.Add("reject", "REJECT", ColumnDataType.Number);
                Master.UserReport.Columns.Add("reject_percent", "REJECT %", ColumnDataType.Percentage);
                Master.UserReport.Columns.Add("corrected", "CORRECTED", ColumnDataType.Number);
                Master.UserReport.Columns.Add("corrected_percent", "CORRECTED %", ColumnDataType.Percentage);
                Master.UserReport.Columns.Add("na", "N/A", ColumnDataType.Number);
                Master.UserReport.Columns.Add("na_percent", "N/A %", ColumnDataType.Percentage);
                Master.UserReport.Columns.Add("deficiency", "DEFICIENCY", ColumnDataType.Number);
                Master.UserReport.Columns.Add("deficiency_percent", "DEFICIENCY %", ColumnDataType.Percentage);
            }
        }

        private void ApplyDataTableToReport(DataTable dt)
        {
            AddRowsToReport(dt);
            GetTotals(dt);
        }

        private void GetTotals(DataTable dt)
        {
            int ii = 0;
            foreach (DataRow row in dt.Rows)
            {
                columnTotals[ii] = CalculateTotalColumn(ii, dt);
                ii += 1;
            }
        }

        private void AddRowsToReport(DataTable dt)
        {
            int[] sum = new int[7];
            int i = 0;
            DataRow totalRow = dt.Rows[0];

            foreach (string row in m_selectedRows)
            {
              //  sum[0] = 0;
                sum[1] = 0;
                sum[2] = 0;
                sum[3] = 0;
                sum[4] = 0;
                sum[5] = 0;

                foreach (DataRow rows in dt.Rows)
                {
                   // sum[0] += Convert.ToInt32(rows[1]); //total tests
                    //grabs every 5th column of data to display in each row. 
                    sum[1] += Convert.ToInt32(rows[3 + (i * 5)]); //pass
                    sum[2] += Convert.ToInt32(rows[4 + (i * 5)]);// reject
                    sum[3] += Convert.ToInt32(rows[5 + (i * 5)]);//corrected
                    sum[4] += Convert.ToInt32(rows[6 + (i * 5)]); //N/A
                    sum[5] += Convert.ToInt32(rows[7 + (i * 5)]); //Deficiiency
                }
              //  totalRow[1] = sum[0].ToString();
                totalRow[3] = sum[1].ToString();
                totalRow[4] = sum[2].ToString();
                totalRow[5] = sum[3].ToString();
                totalRow[6] = sum[4].ToString();
                totalRow[7] = sum[5].ToString();

                //pass %
                double passPercent = (((double)Convert.ToInt64(totalRow[3])) / (double)(Convert.ToInt64(totalRow[1]))) * 100;
                string passpercent = passPercent.ToString("0.00") + "%";
                //reject% 
                double rejectPercent = (((double)Convert.ToInt64(totalRow[4])) / (double)(Convert.ToInt64(totalRow[1]))) * 100;
                string rejectspercent = rejectPercent.ToString("0.00") + "%";
                //corrected%
                double correctedPercent = (((double)Convert.ToInt64(totalRow[5])) / (double)(Convert.ToInt64(totalRow[1]))) * 100;
                string correctedpercent = correctedPercent.ToString("0.00") + "%";
                //n/a%
                double naPercent = (((double)Convert.ToInt64(totalRow[6])) / (double)(Convert.ToInt64(totalRow[1]))) * 100;
                string napercent = naPercent.ToString("0.00") + "%";
                //deficiency%
                double deficiencyPercent = (((double)Convert.ToInt64(totalRow[7])) / (double)(Convert.ToInt64(totalRow[1]))) * 100;
                string deficiencypercent = deficiencyPercent.ToString("0.00") + "%";

                ReportRow rptRow = new ReportRow(Master.UserReport);
                rptRow.Cells.Add(row);
                rptRow.Cells.Add(totalRow[3]);
                rptRow.Cells.Add(passpercent);
                rptRow.Cells.Add(totalRow[4]);
                rptRow.Cells.Add(rejectspercent);
                rptRow.Cells.Add(totalRow[5]);
                rptRow.Cells.Add(correctedpercent);
                rptRow.Cells.Add(totalRow[6]);
                rptRow.Cells.Add(napercent);
                rptRow.Cells.Add(totalRow[7]);
                rptRow.Cells.Add(deficiencypercent);
                Master.UserReport.Rows.Add(rptRow);
                i++;
            }
        }

        private void FormatRowName()
        {
            if (Master.UserReport != null)
            {
                foreach (ReportRow row in Master.UserReport.Rows)
                {
                    try
                    {
                        // Convert fields common to all SQL
                        if (!String.IsNullOrEmpty(row[0].Value))
                        {
                            String rowname = row[0].Value;
                            switch (rowname)
                            {
                                case "AIRBRAKE":
                                    rowname = "Air Brake";
                                    break;
                                case "BODY":
                                    rowname = "Body";
                                    break;
                                case "BREAKWIRING":
                                    rowname = "Break Wiring";
                                    break;
                                case "BUMPERS":
                                    rowname = "Bumpers";
                                    break;
                                case "BUSBODY":
                                    rowname = "Bus Body";
                                    break;
                                case "BUSINTERIOR":
                                    rowname = "Bus Interior";
                                    break;
                                case "DEFROSTER":
                                    rowname = "Defroster";
                                    break;
                                case "EBREAKS":
                                    rowname = "Emergency Brake";
                                    break;
                                case "EXHAUST":
                                    rowname = "Exhaust";
                                    break;
                                case "FIREEXT":
                                    rowname = "Fire Exting.";
                                    break;
                                case "FOOTBRAKE":
                                    rowname = "Foot Brake";
                                    break;
                                case "FRONTLIGHTS":
                                    rowname = "Front Lights";
                                    break;
                                case "FUELSYS":
                                    rowname = "Fuel System";
                                    break;
                                case "GLASS":
                                    rowname = "Glass";
                                    break;
                                case "HEADLIGHTAIM":
                                    rowname = "Headlight Aim";
                                    break;
                                case "HORN":
                                    rowname = "Horn";
                                    break;
                                case "INST":
                                    rowname = "Instruments";
                                    break;
                                case "MAINBRAKES":
                                    rowname = "Main Brakes";
                                    break;
                                case "MIRRORS":
                                    rowname = "Mirrors";
                                    break;
                                case "OTHERLIGHTS":
                                    rowname = "Other Lights";
                                    break;
                                case "PARKINGBRAKE":
                                    rowname = "Parking Brake";
                                    break;
                                case "REARLIGHTS":
                                    rowname = "Rear Lights";
                                    break;
                                case "REFLECTOR":
                                    rowname = "Reflector";
                                    break;
                                case "SIGNAL":
                                    rowname = "Signal";
                                    break;
                                case "STEERING":
                                    rowname = "Steering";
                                    break;
                                case "STOPLIGHTS":
                                    rowname = "Stop Lights";
                                    break;
                                case "TIRES":
                                    rowname = "Tires";
                                    break;
                                case "VEHINFO":
                                    rowname = "Vehicle Info";
                                    break;
                                case "WHEELS":
                                    rowname = "Wheels";
                                    break;
                                case "WIPERS":
                                    rowname = "Wipers";
                                    break;
                                case "BRAKES":
                                    rowname = "Brakes";
                                    break;
                                case "HEADLIGHTS":
                                    rowname = "Head Lights";
                                    break;
                                case "REFLECTORS":
                                    rowname = "Reflectors";
                                    break;
                                case "TAILLIGHTS":
                                    rowname = "Tail Lights";
                                    break;

                            }
                            row[0].Value = rowname.ToString();

                        }
                    }
                    catch (FormatException ex)
                    {
                        LogException(ex);
                        // LogMessage("Error in ConvertReportColumns(): " + ex, LogSeverity.Information);
                    }
                }
            }
        }

        //nai
        private int CalculateTotalColumn(int rowName, DataTable dt)
        {
            int columnTotal = 0;
            foreach (DataRow row in dt.Rows)
            {
                columnTotal += GDCoreUtilities.NullSafe.ToInt(row[rowName], 0);

            }
            return columnTotal;
        }

        private string BuildSelectAggregate(string columnName, bool isLast = false)
        {
            StringBuilder sp = new StringBuilder();

            sp.AppendLine(" SUM(" + columnName + "_PASS) AS TOTAL_" + columnName + "_PASS, ");
            sp.AppendLine(" SUM(" + columnName + "_REJECT) AS TOTAL_" + columnName + "_REJECT,  ");
            sp.AppendLine(" SUM(" + columnName + "_CORRECTED) AS TTL_" + columnName + "_CORRECTED,  ");
            sp.AppendLine(" SUM(" + columnName + "_NA) AS TOTAL_" + columnName + "_NA, ");
            sp.Append(" SUM(" + columnName + "_DEFICIENCY) AS TTL_" + columnName + "_DEFICIENCY ");

            return sp.ToString();
        }
        private string BuildSelectStatementAggregates()
        {
            StringBuilder sb = new StringBuilder();
            m_selectedRows = SafetyDeficiencyRows.GetRows(lstVehicleType.GetDelimitedValues(","));

            foreach (string row in m_selectedRows)
            {
                sb.Append(BuildSelectAggregate(row));
                if (row != m_selectedRows.Last())
                {
                    sb.Append(",");
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }
        private string BuildInnerQueryCaseStmt(string columnName, string prefix, bool isLast = false)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(" CASE WHEN SAF_" + prefix + columnName + " = '1' THEN 1 ELSE 0 END AS " + columnName + "_PASS, ");
            sb.AppendLine(" CASE WHEN SAF_" + prefix + columnName + " = '2' THEN 1 ELSE 0 END AS " + columnName + "_REJECT, ");
            sb.AppendLine(" CASE WHEN SAF_" + prefix + columnName + " = '3' THEN 1 ELSE 0 END AS " + columnName + "_CORRECTED, ");
            sb.AppendLine(" CASE WHEN SAF_" + prefix + columnName + " = '4' THEN 1 ELSE 0 END AS " + columnName + "_NA, ");
            sb.AppendLine(" CASE WHEN ((SAF_" + prefix + columnName + " = '2') OR (SAF_" + prefix + columnName + " = '3')) THEN 1 ELSE 0 END AS " + columnName + "_DEFICIENCY ");

            if (!isLast)
            {
                sb.AppendLine(", ");
            }
            return sb.ToString();
        }
        private string BuildAllSQLCaseStmt(string caseType, string columnName, string altName = "")
        {
            StringBuilder sb = new StringBuilder();
            string altColumnName;

            // Set optional alternative column name for some case stmts that use 2 different columns.
            if (altName == String.Empty)
            {
                altColumnName = columnName;
            }
            else
            {
                altColumnName = altName;
            }
            if (lstVehicleType.SelectedValue.Equals("B"))
            {
                sb.AppendLine(" CASE WHEN ((SAFETYTESTTYPE = 'B') AND (SAF_BS_" + columnName + " = '1')) THEN 1 ELSE 0 END AS " + columnName + "_PASS, ");
                sb.AppendLine(" CASE WHEN ((SAFETYTESTTYPE = 'B') AND (SAF_BS_" + columnName + " = '2')) THEN 1 ELSE 0 END AS " + columnName + "_REJECT, ");
                sb.AppendLine(" CASE WHEN ((SAFETYTESTTYPE = 'B') AND (SAF_BS_" + columnName + " = '3')) THEN 1 ELSE 0 END AS " + columnName + "_CORRECTED, ");
                sb.AppendLine(" CASE WHEN ((SAFETYTESTTYPE = 'B') AND (SAF_BS_" + columnName + " = '4')) THEN 1 ELSE 0 END AS " + columnName + "_NA, ");
                sb.AppendLine(" CASE WHEN ((SAFETYTESTTYPE = 'B') AND (SAF_BS_" + columnName + " IN ('2', '3')))  THEN 1 ELSE 0 END AS " + columnName + "_DEFICIENCY, ");
            }
            if (lstVehicleType.SelectedValue.Equals("T"))
            {
                sb.AppendLine(" CASE WHEN (SAFETYTESTTYPE = 'T') AND (SAF_TB_" + columnName + " = '1')) THEN 1 ELSE 0 END AS " + columnName + "_PASS, ");
                sb.AppendLine(" CASE WHEN ((SAFETYTESTTYPE = 'T') AND (SAF_TB_" + columnName + " = '2')) THEN 1 ELSE 0 END AS " + columnName + "_REJECT, ");
                sb.AppendLine(" CASE WHEN  ((SAFETYTESTTYPE = 'T') AND (SAF_TB_" + columnName + " = '3')) THEN 1 ELSE 0 END AS " + columnName + "_CORRECTED, ");
                sb.AppendLine(" CASE WHEN ((SAFETYTESTTYPE = 'T') AND (SAF_TB_" + columnName + " = '4')) THEN 1 ELSE 0 END AS " + columnName + "_NA, ");
                sb.AppendLine(" CASE WHEN ((SAFETYTESTTYPE = 'T') AND (SAF_TB_" + columnName + " IN ('2','3'))) THEN 1 ELSE 0 END AS " + columnName + "_DEFICIENCY, ");
            }
            if (lstVehicleType.SelectedValue.Equals("M"))
            {
                sb.AppendLine(" CASE WHEN ((SAFETYTESTTYPE = 'M') AND (SAF_MC_" + columnName + " = '1')) OR ((SAFETYTESTTYPE = 'A') AND (SAF_AG_" + altColumnName + " = '1')) THEN 1 ELSE 0 END AS " + columnName + "_PASS, ");
                sb.AppendLine(" CASE WHEN  ((SAFETYTESTTYPE = 'M') AND (SAF_MC_" + columnName + " = '2')) OR ((SAFETYTESTTYPE = 'A') AND (SAF_AG_" + altColumnName + " = '2')) THEN 1 ELSE 0 END AS " + columnName + "_REJECT, ");
                sb.AppendLine(" CASE WHEN ((SAFETYTESTTYPE = 'M') AND (SAF_MC_" + columnName + " = '3')) OR ((SAFETYTESTTYPE = 'A') AND (SAF_AG_" + altColumnName + " = '3')) THEN 1 ELSE 0 END AS " + columnName + "_CORRECTED, ");
                sb.AppendLine(" CASE WHEN ((SAFETYTESTTYPE = 'M') AND (SAF_MC_" + columnName + " = '4')) OR ((SAFETYTESTTYPE = 'A') AND (SAF_AG_" + altColumnName + " = '4')) THEN 1 ELSE 0 END AS " + columnName + "_NA, ");
                sb.AppendLine(" CASE WHEN  ((SAFETYTESTTYPE = 'M') AND (SAF_MC_" + columnName + " IN ('2','3'))) OR ((SAFETYTESTTYPE = 'A') AND (SAF_AG_" + altColumnName + " IN ('2','3'))) THEN 1 ELSE 0 END AS " + columnName + "_DEFICIENCY, ");
            }
            if (lstVehicleType.SelectedValue.Equals("R"))
            {
                sb.AppendLine(" CASE WHEN ((SAFETYTESTTYPE = 'R') AND (SAF_TR_" + columnName + " = '1'))  THEN 1 ELSE 0 END AS " + columnName + "_PASS, ");
                sb.AppendLine(" CASE WHEN((SAFETYTESTTYPE = 'R') AND (SAF_TR_" + columnName + " = '2'))  THEN 1 ELSE 0 END AS " + columnName + "_REJECT, ");
                sb.AppendLine(" CASE WHEN ((SAFETYTESTTYPE = 'R') AND (SAF_TR_" + columnName + " = '3'))  THEN 1 ELSE 0 END AS " + columnName + "_CORRECTED, ");
                sb.AppendLine(" CASE WHEN ((SAFETYTESTTYPE = 'R') AND (SAF_TR_" + columnName + " = '4')) THEN 1 ELSE 0 END AS " + columnName + "_NA, ");
                sb.AppendLine(" CASE WHEN ((SAFETYTESTTYPE = 'R') AND (SAF_TR_" + columnName + " IN ('2','3'))) THEN 1 ELSE 0 END AS " + columnName + "_DEFICIENCY, ");
            }
            if (lstVehicleType.SelectedValue.Equals("A"))
            {
                sb.AppendLine(" CASE WHEN ((SAFETYTESTTYPE = 'A') AND (SAF_AG_" + altColumnName + " = '1')) THEN 1 ELSE 0 END AS " + columnName + "_PASS, ");
                sb.AppendLine(" CASE WHEN  ((SAFETYTESTTYPE = 'A') AND (SAF_AG_" + altColumnName + " = '2')) THEN 1 ELSE 0 END AS " + columnName + "_REJECT, ");
                sb.AppendLine(" CASE WHEN ((SAFETYTESTTYPE = 'A') AND (SAF_AG_" + altColumnName + " = '3')) THEN 1 ELSE 0 END AS " + columnName + "_CORRECTED, ");
                sb.AppendLine(" CASE WHEN ((SAFETYTESTTYPE = 'A') AND (SAF_AG_" + altColumnName + " = '4')) THEN 1 ELSE 0 END AS " + columnName + "_NA, ");
                sb.AppendLine(" CASE WHEN ((SAFETYTESTTYPE = 'A') AND (SAF_AG_" + altColumnName + " IN ('2','3'))) THEN 1 ELSE 0 END AS " + columnName + "_DEFICIENCY, ");
            }
            if (lstVehicleType.SelectedValue.Equals(""))
            {
                switch (caseType)
                {
                    // B - T - R Case (SAFETYTESTTYPE = 'B' , SAFETYTESTTYPE = 'T' etc)
                    case "BTR":
                        sb.AppendLine(" CASE WHEN ((SAFETYTESTTYPE = 'B') AND (SAF_BS_" + columnName + " = '1')) OR ((SAFETYTESTTYPE = 'T') AND (SAF_TB_" + columnName + " = '1')) OR ((SAFETYTESTTYPE = 'R') AND (SAF_TR_" + altColumnName + " = '1')) THEN 1 ELSE 0 END AS " + columnName + "_PASS, ");
                        sb.AppendLine(" CASE WHEN ((SAFETYTESTTYPE = 'B') AND (SAF_BS_" + columnName + " = '2')) OR ((SAFETYTESTTYPE = 'T') AND (SAF_TB_" + columnName + " = '2')) OR ((SAFETYTESTTYPE = 'R') AND (SAF_TR_" + altColumnName + " = '2')) THEN 1 ELSE 0 END AS " + columnName + "_REJECT, ");
                        sb.AppendLine(" CASE WHEN ((SAFETYTESTTYPE = 'B') AND (SAF_BS_" + columnName + " = '3')) OR ((SAFETYTESTTYPE = 'T') AND (SAF_TB_" + columnName + " = '3')) OR ((SAFETYTESTTYPE = 'R') AND (SAF_TR_" + altColumnName + " = '3')) THEN 1 ELSE 0 END AS " + columnName + "_CORRECTED, ");
                        sb.AppendLine(" CASE WHEN ((SAFETYTESTTYPE = 'B') AND (SAF_BS_" + columnName + " = '4')) OR ((SAFETYTESTTYPE = 'T') AND (SAF_TB_" + columnName + " = '4')) OR ((SAFETYTESTTYPE = 'R') AND (SAF_TR_" + altColumnName + " = '4')) THEN 1 ELSE 0 END AS " + columnName + "_NA, ");
                        sb.AppendLine(" CASE WHEN ((SAFETYTESTTYPE = 'B') AND (SAF_BS_" + columnName + " IN ('2', '3'))) OR ((SAFETYTESTTYPE = 'T') AND (SAF_TB_" + columnName + " IN ('2','3'))) OR ((SAFETYTESTTYPE = 'R') AND (SAF_TR_" + altColumnName + " IN ('2','3'))) THEN 1 ELSE 0 END AS " + columnName + "_DEFICIENCY, ");
                        break;
                    // B - T Case
                    case "BT":
                        sb.AppendLine(" CASE WHEN ((SAFETYTESTTYPE = 'B') AND (SAF_BS_" + columnName + " = '1')) OR ((SAFETYTESTTYPE = 'T') AND (SAF_TB_" + columnName + " = '1')) THEN 1 ELSE 0 END AS " + columnName + "_PASS, ");
                        sb.AppendLine(" CASE WHEN ((SAFETYTESTTYPE = 'B') AND (SAF_BS_" + columnName + " = '2')) OR ((SAFETYTESTTYPE = 'T') AND (SAF_TB_" + columnName + " = '2')) THEN 1 ELSE 0 END AS " + columnName + "_REJECT, ");
                        sb.AppendLine(" CASE WHEN ((SAFETYTESTTYPE = 'B') AND (SAF_BS_" + columnName + " = '3')) OR ((SAFETYTESTTYPE = 'T') AND (SAF_TB_" + columnName + " = '3')) THEN 1 ELSE 0 END AS " + columnName + "_CORRECTED, ");
                        sb.AppendLine(" CASE WHEN ((SAFETYTESTTYPE = 'B') AND (SAF_BS_" + columnName + " = '4')) OR ((SAFETYTESTTYPE = 'T') AND (SAF_TB_" + columnName + " = '4')) THEN 1 ELSE 0 END AS " + columnName + "_NA, ");
                        sb.AppendLine(" CASE WHEN ((SAFETYTESTTYPE = 'B') AND (SAF_BS_" + columnName + " IN ('2','3'))) OR ((SAFETYTESTTYPE = 'T') AND (SAF_TB_" + columnName + " IN ('2','3'))) THEN 1 ELSE 0 END AS " + columnName + "_DEFICIENCY, ");
                        break;
                    // B - T - M - A Case
                    case "BTMA":
                        sb.AppendLine(" CASE WHEN ((SAFETYTESTTYPE = 'B') AND (SAF_BS_" + columnName + " = '1')) OR ((SAFETYTESTTYPE = 'T') AND (SAF_TB_" + columnName + " = '1')) OR ((SAFETYTESTTYPE = 'M') AND (SAF_MC_" + columnName + " = '1')) OR ((SAFETYTESTTYPE = 'A') AND (SAF_AG_" + altColumnName + " = '1')) THEN 1 ELSE 0 END AS " + columnName + "_PASS, ");
                        sb.AppendLine(" CASE WHEN ((SAFETYTESTTYPE = 'B') AND (SAF_BS_" + columnName + " = '2')) OR ((SAFETYTESTTYPE = 'T') AND (SAF_TB_" + columnName + " = '2')) OR ((SAFETYTESTTYPE = 'M') AND (SAF_MC_" + columnName + " = '2')) OR ((SAFETYTESTTYPE = 'A') AND (SAF_AG_" + altColumnName + " = '2')) THEN 1 ELSE 0 END AS " + columnName + "_REJECT, ");
                        sb.AppendLine(" CASE WHEN ((SAFETYTESTTYPE = 'B') AND (SAF_BS_" + columnName + " = '3')) OR ((SAFETYTESTTYPE = 'T') AND (SAF_TB_" + columnName + " = '3')) OR ((SAFETYTESTTYPE = 'M') AND (SAF_MC_" + columnName + " = '3')) OR ((SAFETYTESTTYPE = 'A') AND (SAF_AG_" + altColumnName + " = '3')) THEN 1 ELSE 0 END AS " + columnName + "_CORRECTED, ");
                        sb.AppendLine(" CASE WHEN ((SAFETYTESTTYPE = 'B') AND (SAF_BS_" + columnName + " = '4')) OR ((SAFETYTESTTYPE = 'T') AND (SAF_TB_" + columnName + " = '4')) OR ((SAFETYTESTTYPE = 'M') AND (SAF_MC_" + columnName + " = '4')) OR ((SAFETYTESTTYPE = 'A') AND (SAF_AG_" + altColumnName + " = '4')) THEN 1 ELSE 0 END AS " + columnName + "_NA, ");
                        sb.AppendLine(" CASE WHEN ((SAFETYTESTTYPE = 'B') AND (SAF_BS_" + columnName + " IN ('2','3'))) OR ((SAFETYTESTTYPE = 'T') AND (SAF_TB_" + columnName + " IN ('2','3'))) OR ((SAFETYTESTTYPE = 'M') AND (SAF_MC_" + columnName + " IN ('2','3'))) OR ((SAFETYTESTTYPE = 'A') AND (SAF_AG_" + altColumnName + " IN ('2','3'))) THEN 1 ELSE 0 END AS " + columnName + "_DEFICIENCY, ");
                        break;
                    // B - T - M Case
                    case "BTM":
                        sb.AppendLine(" CASE WHEN ((SAFETYTESTTYPE = 'B') AND (SAF_BS_" + columnName + " = '1')) OR ((SAFETYTESTTYPE = 'T') AND (SAF_TB_" + columnName + " = '1')) OR ((SAFETYTESTTYPE = 'M') AND (SAF_MC_" + columnName + " = '1')) THEN 1 ELSE 0 END AS " + columnName + "_PASS, ");
                        sb.AppendLine(" CASE WHEN ((SAFETYTESTTYPE = 'B') AND (SAF_BS_" + columnName + " = '2')) OR ((SAFETYTESTTYPE = 'T') AND (SAF_TB_" + columnName + " = '2')) OR ((SAFETYTESTTYPE = 'M') AND (SAF_MC_" + columnName + " = '2')) THEN 1 ELSE 0 END AS " + columnName + "_REJECT, ");
                        sb.AppendLine(" CASE WHEN ((SAFETYTESTTYPE = 'B') AND (SAF_BS_" + columnName + " = '3')) OR ((SAFETYTESTTYPE = 'T') AND (SAF_TB_" + columnName + " = '3')) OR ((SAFETYTESTTYPE = 'M') AND (SAF_MC_" + columnName + " = '3')) THEN 1 ELSE 0 END AS " + columnName + "_CORRECTED, ");
                        sb.AppendLine(" CASE WHEN ((SAFETYTESTTYPE = 'B') AND (SAF_BS_" + columnName + " = '4')) OR ((SAFETYTESTTYPE = 'T') AND (SAF_TB_" + columnName + " = '4')) OR ((SAFETYTESTTYPE = 'M') AND (SAF_MC_" + columnName + " = '4')) THEN 1 ELSE 0 END AS " + columnName + "_NA, ");
                        sb.AppendLine(" CASE WHEN ((SAFETYTESTTYPE = 'B') AND (SAF_BS_" + columnName + " IN ('2','3'))) OR ((SAFETYTESTTYPE = 'T') AND (SAF_TB_" + columnName + " IN ('2','3'))) OR ((SAFETYTESTTYPE = 'M') AND (SAF_MC_" + columnName + " IN ('2','3'))) THEN 1 ELSE 0 END AS " + columnName + "_DEFICIENCY, ");
                        break;
                    // B - T - R - M Case
                    case "BTRM":
                        sb.AppendLine(" CASE WHEN ((SAFETYTESTTYPE = 'B') AND (SAF_BS_" + columnName + " = '1')) OR ((SAFETYTESTTYPE = 'T') AND (SAF_TB_" + columnName + " = '1')) OR ((SAFETYTESTTYPE = 'R') AND (SAF_TR_" + columnName + " = '1')) OR ((SAFETYTESTTYPE = 'M') AND (SAF_MC_" + columnName + " = '1')) THEN 1 ELSE 0 END AS " + columnName + "_PASS, ");
                        sb.AppendLine(" CASE WHEN ((SAFETYTESTTYPE = 'B') AND (SAF_BS_" + columnName + " = '2')) OR ((SAFETYTESTTYPE = 'T') AND (SAF_TB_" + columnName + " = '2')) OR ((SAFETYTESTTYPE = 'R') AND (SAF_TR_" + columnName + " = '2')) OR ((SAFETYTESTTYPE = 'M') AND (SAF_MC_" + columnName + " = '2')) THEN 1 ELSE 0 END AS " + columnName + "_REJECT, ");
                        sb.AppendLine(" CASE WHEN ((SAFETYTESTTYPE = 'B') AND (SAF_BS_" + columnName + " = '3')) OR ((SAFETYTESTTYPE = 'T') AND (SAF_TB_" + columnName + " = '3')) OR ((SAFETYTESTTYPE = 'R') AND (SAF_TR_" + columnName + " = '3')) OR ((SAFETYTESTTYPE = 'M') AND (SAF_MC_" + columnName + " = '3')) THEN 1 ELSE 0 END AS " + columnName + "_CORRECTED, ");
                        sb.AppendLine(" CASE WHEN ((SAFETYTESTTYPE = 'B') AND (SAF_BS_" + columnName + " = '4')) OR ((SAFETYTESTTYPE = 'T') AND (SAF_TB_" + columnName + " = '4')) OR ((SAFETYTESTTYPE = 'R') AND (SAF_TR_" + columnName + " = '4')) OR ((SAFETYTESTTYPE = 'M') AND (SAF_MC_" + columnName + " = '4')) THEN 1 ELSE 0 END AS " + columnName + "_NA, ");
                        sb.AppendLine(" CASE WHEN ((SAFETYTESTTYPE = 'B') AND (SAF_BS_" + columnName + " IN ('2','3'))) OR ((SAFETYTESTTYPE = 'T') AND (SAF_TB_" + columnName + " IN ('2','3'))) OR ((SAFETYTESTTYPE = 'R') AND (SAF_TR_" + columnName + " IN ('2','3'))) OR ((SAFETYTESTTYPE = 'M') AND (SAF_MC_" + columnName + " IN ('2','3'))) THEN 1 ELSE 0 END AS " + columnName + "_DEFICIENCY, ");
                        break;
                    // B - T - R - M - A Case
                    case "BTRMA":
                        sb.AppendLine(" CASE WHEN ((SAFETYTESTTYPE = 'B') AND (SAF_BS_" + columnName + " = '1')) OR ((SAFETYTESTTYPE = 'T') AND (SAF_TB_" + columnName + " = '1')) OR ((SAFETYTESTTYPE = 'R') AND (SAF_TR_" + columnName + " = '1')) OR ((SAFETYTESTTYPE = 'M') AND (SAF_MC_" + columnName + " = '1')) OR ((SAFETYTESTTYPE = 'A') AND (SAF_AG_" + altColumnName + " = '1')) THEN 1 ELSE 0 END AS " + columnName + "_PASS, ");
                        sb.AppendLine(" CASE WHEN ((SAFETYTESTTYPE = 'B') AND (SAF_BS_" + columnName + " = '2')) OR ((SAFETYTESTTYPE = 'T') AND (SAF_TB_" + columnName + " = '2')) OR ((SAFETYTESTTYPE = 'R') AND (SAF_TR_" + columnName + " = '2')) OR ((SAFETYTESTTYPE = 'M') AND (SAF_MC_" + columnName + " = '2')) OR ((SAFETYTESTTYPE = 'A') AND (SAF_AG_" + altColumnName + " = '2')) THEN 1 ELSE 0 END AS " + columnName + "_REJECT, ");
                        sb.AppendLine(" CASE WHEN ((SAFETYTESTTYPE = 'B') AND (SAF_BS_" + columnName + " = '3')) OR ((SAFETYTESTTYPE = 'T') AND (SAF_TB_" + columnName + " = '3')) OR ((SAFETYTESTTYPE = 'R') AND (SAF_TR_" + columnName + " = '3')) OR ((SAFETYTESTTYPE = 'M') AND (SAF_MC_" + columnName + " = '3')) OR ((SAFETYTESTTYPE = 'A') AND (SAF_AG_" + altColumnName + " = '3')) THEN 1 ELSE 0 END AS " + columnName + "_CORRECTED, ");
                        sb.AppendLine(" CASE WHEN ((SAFETYTESTTYPE = 'B') AND (SAF_BS_" + columnName + " = '4')) OR ((SAFETYTESTTYPE = 'T') AND (SAF_TB_" + columnName + " = '4')) OR ((SAFETYTESTTYPE = 'R') AND (SAF_TR_" + columnName + " = '4')) OR ((SAFETYTESTTYPE = 'M') AND (SAF_MC_" + columnName + " = '4')) OR ((SAFETYTESTTYPE = 'A') AND (SAF_AG_" + altColumnName + " = '4')) THEN 1 ELSE 0 END AS " + columnName + "_NA, ");
                        sb.AppendLine(" CASE WHEN ((SAFETYTESTTYPE = 'B') AND (SAF_BS_" + columnName + " IN ('2','3'))) OR ((SAFETYTESTTYPE = 'T') AND (SAF_TB_" + columnName + " IN ('2','3'))) OR ((SAFETYTESTTYPE = 'R') AND (SAF_TR_" + columnName + " IN ('2','3'))) OR ((SAFETYTESTTYPE = 'M') AND (SAF_MC_" + columnName + " IN ('2','3')))OR ((SAFETYTESTTYPE = 'A') AND (SAF_AG_" + altColumnName + " IN ('2','3'))) THEN 1 ELSE 0 END AS " + columnName + "_DEFICIENCY, ");
                        break;
                    // T - A Case
                    case "TA":
                        sb.AppendLine(" CASE WHEN ((SAFETYTESTTYPE = 'T') AND (SAF_TB_" + columnName + " = '1') OR (SAFETYTESTTYPE = 'A') AND (SAF_AG_" + columnName + "S = '1')) THEN 1 ELSE 0 END AS " + columnName + "_PASS, ");
                        sb.AppendLine(" CASE WHEN ((SAFETYTESTTYPE = 'T') AND (SAF_TB_" + columnName + " = '2') OR (SAFETYTESTTYPE = 'A') AND (SAF_AG_" + columnName + "S = '2'))  THEN 1 ELSE 0 END AS " + columnName + "_REJECT, ");
                        sb.AppendLine(" CASE WHEN ((SAFETYTESTTYPE = 'T') AND (SAF_TB_" + columnName + " = '3') OR (SAFETYTESTTYPE = 'A') AND (SAF_AG_" + columnName + "S = '3'))  THEN 1 ELSE 0 END AS " + columnName + "_CORRECTED, ");
                        sb.AppendLine(" CASE WHEN ((SAFETYTESTTYPE = 'T') AND (SAF_TB_" + columnName + " = '4') OR (SAFETYTESTTYPE = 'A') AND (SAF_AG_" + columnName + "S = '4'))  THEN 1 ELSE 0 END AS " + columnName + "_NA, ");
                        sb.AppendLine(" CASE WHEN ((SAFETYTESTTYPE = 'T') AND (SAF_TB_" + columnName + " IN ('2','3'))) OR ((SAFETYTESTTYPE = 'A') AND (SAF_AG_" + columnName + "S IN ('2','3'))) THEN 1 ELSE 0 END AS " + columnName + "_DEFICIENCY, ");
                        break;
                }
            }
            return sb.ToString();
        }
        private string BuildInnerSQLCaseStatements()
        {
            StringBuilder sb = new StringBuilder();

            if (lstVehicleType.SelectedValue.Equals("B"))
            {
                // Basic
                sb.AppendLine(BuildInnerQueryCaseStmt("BODY", "BS_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("BUMPERS", "BS_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("DEFROSTER", "BS_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("EXHAUST", "BS_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("FOOTBRAKE", "BS_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("FRONTLIGHTS", "BS_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("FUELSYS", "BS_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("GLASS", "BS_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("HEADLIGHTAIM", "BS_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("HORN", "BS_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("INST", "BS_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("MIRRORS", "BS_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("OTHERLIGHTS", "BS_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("PARKINGBRAKE", "BS_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("REARLIGHTS", "BS_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("SIGNAL", "BS_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("STEERING", "BS_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("STOPLIGHTS", "BS_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("TIRES", "BS_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("VEHINFO", "BS_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("WHEELS", "BS_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("WIPERS", "BS_", true));
            }
            if (lstVehicleType.SelectedValue.Equals("T"))
            {
                // Truck or Bus
                sb.AppendLine(BuildInnerQueryCaseStmt("BODY", "TB_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("BUMPERS", "TB_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("DEFROSTER", "TB_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("EXHAUST", "TB_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("FOOTBRAKE", "TB_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("FRONTLIGHTS", "TB_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("FUELSYS", "TB_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("GLASS", "TB_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("HEADLIGHTAIM", "TB_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("HORN", "TB_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("INST", "TB_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("MIRRORS", "TB_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("OTHERLIGHTS", "TB_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("PARKINGBRAKE", "TB_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("REARLIGHTS", "TB_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("SIGNAL", "TB_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("STEERING", "TB_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("STOPLIGHTS", "TB_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("TIRES", "TB_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("VEHINFO", "TB_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("WHEELS", "TB_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("WIPERS", "TB_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("AIRBRAKE", "TB_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("REFLECTOR", "TB_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("FIREEXT", "TB_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("BUSBODY", "TB_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("BUSINTERIOR", "TB_", true));
            }
            if (lstVehicleType.SelectedValue.Equals("R"))
            {
                // Trailer
                sb.AppendLine(BuildInnerQueryCaseStmt("BODY", "TR_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("BUMPER", "TR_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("PARKINGBRAKE", "TR_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("REARLIGHTS", "TR_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("STOPLIGHTS", "TR_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("TIRES", "TR_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("VEHINFO", "TR_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("MAINBRAKES", "TR_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("EBREAKS", "TRAIEER_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("BREAKWIRING", "TR_", true));
            }
            if (lstVehicleType.SelectedValue.Equals("A"))
            {
                // Agriculture
                sb.AppendLine(BuildInnerQueryCaseStmt("EXHAUST", "AG_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("BRAKES", "AG_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("HEADLIGHTS", "AG_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("STEERING", "AG_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("STOPLIGHTS", "AG_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("TAILLIGHTS", "AG_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("REFLECTORS", "AG_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("VIN", "AG_", true));
            }
            if (lstVehicleType.SelectedValue.Equals("M"))
            {
                // Motorcycle
                sb.AppendLine(BuildInnerQueryCaseStmt("BODY", "MC_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("EXHAUST", "MC_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("FOOTBRAKE", "MC_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("FRONTLIGHTS", "MC_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("FUELSYS", "MC_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("GLASS", "MC_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("HEADLIGHTAIM", "MC_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("HORN", "MC_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("INST", "MC_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("MIRRORS", "MC_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("OTHERLIGHTS", "MC_"));

                sb.AppendLine(" 0 AS PARKINGBRAKE_PASS, ");
                sb.AppendLine(" 0 AS PARKINGBRAKE_REJECT, ");
                sb.AppendLine(" 0 AS PARKINGBRAKE_CORRECTED, ");
                sb.AppendLine(" 0 AS PARKINGBRAKE_NA, ");
                sb.AppendLine(" 0 AS PARKINGBRAKE_DEFICIENCY, ");

                sb.AppendLine(BuildInnerQueryCaseStmt("REARLIGHTS", "MC_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("SIGNAL", "MC_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("STEERING", "MC_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("STOPLIGHTS", "MC_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("TIRES", "MC_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("VEHINFO", "MC_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("WHEELS", "MC_"));

                sb.AppendLine(" 0 AS WIPERS_PASS, ");
                sb.AppendLine(" 0 AS WIPERS_REJECT, ");
                sb.AppendLine(" 0 AS WIPERS_CORRECTED, ");
                sb.AppendLine(" 0 AS WIPERS_NA, ");
                sb.AppendLine(" 0 AS WIPERS_DEFICIENCY ");
            }
            if (lstVehicleType.SelectedValue.Equals(""))
            {
                sb.AppendLine(BuildAllSQLCaseStmt("BTR", "BODY"));
                sb.AppendLine(BuildAllSQLCaseStmt("BTR", "BUMPERS", "BUMPER"));
                sb.AppendLine(BuildAllSQLCaseStmt("BT", "DEFROSTER"));
                sb.AppendLine(BuildAllSQLCaseStmt("BTMA", "EXHAUST"));
                sb.AppendLine(BuildAllSQLCaseStmt("BTMA", "FOOTBRAKE", "BRAKES"));
                sb.AppendLine(BuildAllSQLCaseStmt("BTMA", "FRONTLIGHTS", "HEADLIGHTS"));
                sb.AppendLine(BuildAllSQLCaseStmt("BTM", "FUELSYS"));
                sb.AppendLine(BuildAllSQLCaseStmt("BTM", "GLASS"));
                sb.AppendLine(BuildAllSQLCaseStmt("BTM", "HEADLIGHTAIM"));
                sb.AppendLine(BuildAllSQLCaseStmt("BTM", "HORN"));
                sb.AppendLine(BuildAllSQLCaseStmt("BTM", "INST"));
                sb.AppendLine(BuildAllSQLCaseStmt("BTM", "MIRRORS"));
                sb.AppendLine(BuildAllSQLCaseStmt("BTM", "OTHERLIGHTS"));
                sb.AppendLine(BuildAllSQLCaseStmt("BTR", "PARKINGBRAKE"));
                sb.AppendLine(BuildAllSQLCaseStmt("BTRMA", "REARLIGHTS", "TAILLIGHTS"));
                sb.AppendLine(BuildAllSQLCaseStmt("BTM", "SIGNAL"));
                sb.AppendLine(BuildAllSQLCaseStmt("BTMA", "STEERING"));
                sb.AppendLine(BuildAllSQLCaseStmt("BTRMA", "STOPLIGHTS"));
                sb.AppendLine(BuildAllSQLCaseStmt("BTRM", "TIRES"));
                sb.AppendLine(BuildAllSQLCaseStmt("BTRMA", "VEHINFO", "VIN"));
                sb.AppendLine(BuildAllSQLCaseStmt("BTM", "WHEELS"));
                sb.AppendLine(BuildAllSQLCaseStmt("BT", "WIPERS"));
                sb.AppendLine(BuildInnerQueryCaseStmt("AIRBRAKE", "TB_"));
                sb.AppendLine(BuildAllSQLCaseStmt("TA", "REFLECTOR", "REFLECTORS"));
                sb.AppendLine(BuildInnerQueryCaseStmt("FIREEXT", "TB_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("BUSBODY", "TB_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("BUSINTERIOR", "TB_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("MAINBRAKES", "TR_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("EBREAKS", "TRAIEER_"));
                sb.AppendLine(BuildInnerQueryCaseStmt("BREAKWIRING", "TR_", true));
            }

            return sb.ToString();
        }
        private string BuildQuery()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("SELECT");
            sb.AppendLine("TEST_YEAR,");
            sb.AppendLine("SUM(TESTS) AS TOTAL_TESTS, ");
            sb.AppendLine("SUM(DEFICIENCIES) AS TOTAL_DEFICIENCIES, ");
            //select vehicle type
           sb.AppendLine(BuildSelectStatementAggregates());
            sb.AppendLine(" FROM ");
            sb.AppendLine(" ( SELECT   SUBSTR(TESTDATE,0,4) TEST_YEAR ,");
            sb.AppendLine(" CASE WHEN VIN > '0' THEN 1 ELSE 0 END as TESTS, ");
            sb.AppendLine(" CASE WHEN SAFETYPF='2' OR SAFETYPF='3' THEN 1 ELSE 0 END AS DEFICIENCIES, ");
            //Build Inner Query Case Statements
            sb.AppendLine(BuildInnerSQLCaseStatements());
            sb.AppendLine(" FROM NEW_TESTRECORD N ");
            sb.AppendLine(" WHERE ");
            sb.AppendLine(" SAFETYPF NOT IN ('0','4','5','6') ");
            //where clause
            if (!String.IsNullOrEmpty(lstTestSeq.SelectedValue))
            {
                sb.AppendLine("AND TESTSEQUENCE = '" + lstTestSeq.SelectedValue + "'");
                //switch (lstTestSeq.SelectedValue)
                //{
                //    case "01":
                //        sb.AppendLine(" AND TESTSEQUENCE = 1 ");
                //        break;
                //    case "02":
                //        sb.AppendLine(" AND TESTSEQUENCE = 2 ");
                //        break;
                //    case "03":
                //        sb.AppendLine(" AND TESTSEQUENCE > 2 ");
                //        break;
                //    default:
                //        break;
                //}
            }
            if (!String.IsNullOrEmpty(lstModelYear.SelectedValue))
            {
                sb.AppendLine("AND TESTASMODELYR = '" + lstModelYear.SelectedValue + "'");
            }
            if (!String.IsNullOrEmpty(lstWeightClass.SelectedValue))
            {
                sb.AppendLine("AND TESTASGVW = '" + lstWeightClass.SelectedValue + "'");
            }
            if (!String.IsNullOrEmpty(lstVehicleType.SelectedValue))
            {
                sb.AppendLine("AND SAFETYTESTTYPE = '" + lstVehicleType.SelectedValue + "'");
            }
            if (!String.IsNullOrEmpty(lstCounty.SelectedValue))
            {
                sb.AppendLine("AND COUNTY = '" + lstCounty.SelectedValue + "'");
            }
            sb.AppendLine("AND testdate >= '" + dateSelector.StartDateControl.GetDateText("yyyyMMdd") + "' ");
            sb.AppendLine("AND testdate <= '" + dateSelector.EndDateControl.GetDateText("yyyyMMdd") + "' ");
            sb.AppendLine(" ) GROUP BY TEST_YEAR ORDER BY TEST_YEAR");
            return sb.ToString();
        }
        //private string BuildDetailQuery()
        //{
        //    StringBuilder sb = new StringBuilder();
        //    sb.AppendLine("SELECT");
        //    sb.AppendLine("TEST_YEAR,");
        //    sb.AppendLine("SUM(TESTS) AS TOTAL_TESTS, ");
        //    sb.AppendLine("SUM(DEFICIENCIES) AS TOTAL_DEFICIENCIES, ");         
        //    sb.AppendLine(" FROM ");
        //    sb.AppendLine(" ( SELECT   SUBSTR(TESTDATE,0,4) TEST_YEAR ,");
        //    sb.AppendLine(" CASE WHEN VIN > '0' THEN 1 ELSE 0 END as TESTS, ");
        //    sb.AppendLine(" CASE WHEN SAFETYPF='2' OR SAFETYPF='3' THEN 1 ELSE 0 END AS DEFICIENCIES, ");
        //    sb.AppendLine(" FROM NEW_TESTRECORD N ");
        //    sb.AppendLine(" WHERE ");
        //    sb.AppendLine(" SAFETYPF NOT IN ('0','4','5','6') ");
        //    //where clause
        //    if (!String.IsNullOrEmpty(lstTestSeq.SelectedValue))
        //    {
        //        sb.AppendLine("AND TESTSEQUENCE = '" + lstTestSeq.SelectedValue + "'");
        //        //switch (lstTestSeq.SelectedValue)
        //        //{
        //        //    case "01":
        //        //        sb.AppendLine(" AND TESTSEQUENCE = 1 ");
        //        //        break;
        //        //    case "02":
        //        //        sb.AppendLine(" AND TESTSEQUENCE = 2 ");
        //        //        break;
        //        //    case "03":
        //        //        sb.AppendLine(" AND TESTSEQUENCE > 2 ");
        //        //        break;
        //        //    default:
        //        //        break;
        //        //}
        //    }
        //    if (!String.IsNullOrEmpty(lstModelYear.SelectedValue))
        //    {
        //        sb.AppendLine("AND TESTASMODELYR = '" + lstModelYear.SelectedValue + "'");
        //    }
        //    if (!String.IsNullOrEmpty(lstWeightClass.SelectedValue))
        //    {
        //        sb.AppendLine("AND TESTASGVW = '" + lstWeightClass.SelectedValue + "'");
        //    }
        //    if (!String.IsNullOrEmpty(lstVehicleType.SelectedValue))
        //    {
        //        sb.AppendLine("AND SAFETYTESTTYPE = '" + lstVehicleType.SelectedValue + "'");
        //    }
        //    if (!String.IsNullOrEmpty(lstCounty.SelectedValue))
        //    {
        //        sb.AppendLine("AND COUNTY = '" + lstCounty.SelectedValue + "'");
        //    }
        //    sb.AppendLine("AND testdate >= '" + dateSelector.StartDateControl.GetDateText("yyyyMMdd") + "' ");
        //    sb.AppendLine("AND testdate <= '" + dateSelector.EndDateControl.GetDateText("yyyyMMdd") + "' ");
        //    sb.AppendLine(" ) GROUP BY TEST_YEAR ORDER BY TEST_YEAR");
        //    return sb.ToString();
        //}
        private void AddDetailsTable(DataTable dt)
        {
            int[] sums = SumData(dt);

            int[] sum = new int[2];
               DataRow totalRow = dt.Rows[0];
            foreach (DataRow rows in dt.Rows)
            {
                sum[0] += Convert.ToInt32(rows[1]); 
            }
            totalRow[1] = sum[0].ToString();

            double deficencyPercent = 0.0;
            if (sums.Length > 1)
            {
                deficencyPercent = Math.Round((sums[1] * 1.0) / Convert.ToInt32(totalRow[1]), 4,
                    MidpointRounding.AwayFromZero);
            }

            ReportTable tbl = new ReportTable();
            tbl.Title = "Safety Defect Statistics";
            tbl.ID = "Safety Defect Statistics";
            tbl.HeaderVisible = false;
            tbl.RenderLocation = RenderLocation.Above;
            tbl.HeaderNote = "Safety Defect Statistics";
            tbl.Columns.Add("DESC", "", ColumnDataType.String);
            tbl.Columns.Add("NUM", "", ColumnDataType.Number);
            tbl.Columns.Add("PERCENT", "", ColumnDataType.Percentage);

            ReportRow safetyRow = new ReportRow(tbl);
            safetyRow.Cells.Add("Total Safety Tests*");
            safetyRow.Cells.Add(totalRow[1]);
            //safetyRow.Cells.Add(sums[0]);
            safetyRow.Cells[1].ColumnSpan = 2;
            tbl.Rows.Add(safetyRow);

            ReportRow testYearRow = new ReportRow(tbl);
            testYearRow.Cells.Add("Test Year");
            testYearRow.Cells.Add(GetYearRange(dt));
            testYearRow.Cells[1].ColumnSpan = 2;
            tbl.Rows.Add(testYearRow);

            ReportRow totalDeficiencyRow = new ReportRow(tbl);
            totalDeficiencyRow.Cells.Add("Total Safety Deficiencies");
            totalDeficiencyRow.Cells.Add(sums[1]);
            totalDeficiencyRow.Cells.Add(deficencyPercent.ToString("0.00%"));
            tbl.Rows.Add(totalDeficiencyRow);

            Master.UserReport.Tables.Add(tbl);
        }

        private int[] SumData(DataTable dt)
        {
            int[] sums = new int[2];
            foreach (DataRow dr in dt.Rows)
            {

                //sums[0] += Convert.ToInt32(dr[1]); //total tests
                sums[1] += Convert.ToInt32(dr[2]); // total deficiencies
            }
            return sums;
        }

        private string GetYearRange(DataTable dt)
        {
            string yearRange = "";

            int rowCount = dt.Rows.Count;
            if (rowCount > 1)
            {
                yearRange = dt.Rows[0]["TEST_YEAR"].ToString() + " - " + dt.Rows[rowCount - 1]["TEST_YEAR"].ToString();
            }
            else if (rowCount == 1)
            {
                yearRange = dt.Rows[0]["TEST_YEAR"].ToString();
            }
            return yearRange;
        }

        //this is the table at the top of the page, used in HeaderContent
        public string ConvertTabletoHtml(DataTable table)
        {
            if (table.Rows.Count > 0)
            {
                DataRow totalRow = table.Rows[0];
                int[] sums = SumData(table);

                totalRow[1] = sums[0].ToString();
                string totaltests = (totalRow[1].ToString()); //gets total test for safety defects statistics
                totalRow[2] = sums[1].ToString();
                string totalDefic = (totalRow[2].ToString());     // gets total safety deficiencies for saftey defects statistics                
                decimal deficPercent = (Convert.ToInt64(totalRow[2].ToString()) * 100) / (Convert.ToInt64(totaltests));
                string percent = deficPercent.ToString("0.00") + "%"; // gets safety deficiency % for safety defects statistics
                if (!String.IsNullOrEmpty(totaltests))
                {
                    #region safety defects statistics
                    string html = "<div class=\"details\" style=\"visibility:visible; width:55%; margin:auto;\" runat=\"server\">";
                    html += "<table class=\"expandedDetails\">";
                    html += "<tr><td colspan =\"4\" style=\"text-align:center; width:55%;\" class=\"table_header\"><h2>SAFETY DEFECT STATISTICS</h2></td></tr>";
                    html += "<tr>";
                    html += "<td>Total Safety Tests*</td>";
                    html += "<td colspan=\"2\">";
                    html += totalRow[1];
                    html += "</td>";
                    html += "</tr>";
                    html += "<tr>";
                    html += "<td>Test Year</td>";
                    html += "<td colspan=\"2\">";
                    html += GetYearRange(table);
                    html += "</td>";
                    html += "</tr>";
                    html += "<tr>";
                    html += "<td>Total Safety Deficiencies</td>";
                    html += "<td>";
                    html += totalRow[2];
                    html += "</td>";
                    html += "<td>";
                    html += percent;
                    html += "</td>";
                    html += "</tr>";
                    html += "</table>";
                    #endregion safety defect statistics

                    html += "</div>";
                    return html;
                }
                else
                {
                    return "Error";
                }
            }
            else
            {
                return "";
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
                if (lstWeightClass.ValuesSelected)
                {
                    Master.UserReport.MetaData.Add("Weight Class", lstWeightClass.GetDelimitedText(", "),
                        lstWeightClass.GetDelimitedValues(","));
                }
            }
        }
    }

    // this is where the Item names are determined based on the vehicle type picked
    public class SafetyDeficiencyRows
    {
        private static string[] basicRows;
        private static string[] agricultureRows;
        private static string[] trailerRows;
        private static string[] truckbusRows;
        private static string[] motorcycleRows;
        private static string[] allRows;

        static SafetyDeficiencyRows()
        {
            InitializeBasicRows();
            InitializeAgricultureRows();
            InitializeTrailerRows();
            InitializeTruckBusRows();
            InitializeMotorcycleRows();
            InitializeAllRows();
        }

        private static void InitializeBasicRows()
        {
            List<string> rows = new List<string>();
            rows.Add("BODY");
            rows.Add("BUMPERS");
            rows.Add("DEFROSTER");
            rows.Add("EXHAUST");
            rows.Add("FOOTBRAKE");
            rows.Add("FRONTLIGHTS");
            rows.Add("FUELSYS");
            rows.Add("GLASS");
            rows.Add("HEADLIGHTAIM");
            rows.Add("HORN");
            rows.Add("INST");
            rows.Add("MIRRORS");
            rows.Add("OTHERLIGHTS");
            rows.Add("PARKINGBRAKE");
            rows.Add("REARLIGHTS");
            rows.Add("SIGNAL");
            rows.Add("STEERING");
            rows.Add("STOPLIGHTS");
            rows.Add("TIRES");
            rows.Add("VEHINFO");
            rows.Add("WHEELS");
            rows.Add("WIPERS");

            basicRows = rows.ToArray();
        }

        private static void InitializeAgricultureRows()
        {
            List<string> rows = new List<string>();
            rows.Add("BRAKES");
            rows.Add("HEADLIGHTS");
            rows.Add("REFLECTORS");
            rows.Add("STEERING");
            rows.Add("STOPLIGHTS");
            rows.Add("TAILLIGHTS");
            rows.Add("VIN");

            agricultureRows = rows.ToArray();
        }

        private static void InitializeTrailerRows()
        {
            List<string> rows = new List<string>();
            rows.Add("BODY");
            rows.Add("BREAKWIRING");
            rows.Add("BUMPER");
            rows.Add("EBREAKS");
            rows.Add("MAINBRAKES");
            rows.Add("PARKINGBRAKE");
            rows.Add("REARLIGHTS");
            rows.Add("STOPLIGHTS");
            rows.Add("TIRES");
            rows.Add("VEHINFO");

            trailerRows = rows.ToArray();
        }

        private static void InitializeMotorcycleRows()
        {
            List<string> rows = new List<string>();
            rows.Add("BODY");
            rows.Add("EXHAUST");
            rows.Add("FOOTBRAKE");
            rows.Add("FRONTLIGHTS");
            rows.Add("FUELSYS");
            rows.Add("GLASS");
            rows.Add("HEADLIGHTAIM");
            rows.Add("HORN");
            rows.Add("INST");
            rows.Add("MIRRORS");
            rows.Add("OTHERLIGHTS");

            rows.Add("REARLIGHTS");
            rows.Add("SIGNAL");
            rows.Add("STEERING");
            rows.Add("STOPLIGHTS");
            rows.Add("TIRES");
            rows.Add("VEHINFO");
            rows.Add("WHEELS");

            motorcycleRows = rows.ToArray();
        }

        private static void InitializeTruckBusRows()
        {
            List<string> rows = new List<string>();
            rows.Add("AIRBRAKE");
            rows.Add("BODY");
            rows.Add("BUMPERS");
            rows.Add("BUSBODY");
            rows.Add("BUSINTERIOR");
            rows.Add("DEFROSTER");
            rows.Add("EXHAUST");
            rows.Add("FIREEXT");
            rows.Add("FOOTBRAKE");
            rows.Add("FRONTLIGHTS");
            rows.Add("FUELSYS");
            rows.Add("GLASS");
            rows.Add("HEADLIGHTAIM");
            rows.Add("HORN");
            rows.Add("INST");
            rows.Add("MIRRORS");
            rows.Add("OTHERLIGHTS");
            rows.Add("PARKINGBRAKE");
            rows.Add("REARLIGHTS");
            rows.Add("REFLECTOR");
            rows.Add("SIGNAL");
            rows.Add("STEERING");
            rows.Add("STOPLIGHTS");
            rows.Add("TIRES");
            rows.Add("VEHINFO");
            rows.Add("WHEELS");
            rows.Add("WIPERS");


            truckbusRows = rows.ToArray();
        }

        private static void InitializeAllRows()
        {
            List<string> rows = new List<string>();
            rows.Add("AIRBRAKE");
            rows.Add("BODY");
            rows.Add("BREAKWIRING");
            rows.Add("BUMPERS");
            rows.Add("BUSBODY");
            rows.Add("BUSINTERIOR");
            rows.Add("DEFROSTER");
            rows.Add("EBREAKS");
            rows.Add("EXHAUST");
            rows.Add("FIREEXT");
            rows.Add("FOOTBRAKE");
            rows.Add("FRONTLIGHTS");
            rows.Add("FUELSYS");
            rows.Add("GLASS");
            rows.Add("HEADLIGHTAIM");
            rows.Add("HORN");
            rows.Add("INST");
            rows.Add("MAINBRAKES");
            rows.Add("MIRRORS");
            rows.Add("OTHERLIGHTS");
            rows.Add("PARKINGBRAKE");
            rows.Add("REARLIGHTS");
            rows.Add("REFLECTOR");
            rows.Add("SIGNAL");
            rows.Add("STEERING");
            rows.Add("STOPLIGHTS");
            rows.Add("TIRES");
            rows.Add("VEHINFO");
            rows.Add("WHEELS");
            rows.Add("WIPERS");

            allRows = rows.ToArray();
        }

        public static string[] GetRows(string types)
        {
            string[] rowType = types.Split(new char[] { ',' });

            List<string> rows = new List<string>();
            foreach (string rType in rowType)
            {
                string[] rowsForType = GetRowsByType(rType);
                rows = rows.Union(rowsForType).ToList();
            }
            return rows.OrderBy(x => x).ToArray();
        }

        private static string[] GetRowsByType(string type)
        {
            string[] rows = new string[0];
            switch (type.Trim().ToUpper())
            {
                case "B":
                    rows = basicRows;
                    break;
                case "A":
                    rows = agricultureRows;
                    break;
                case "R":
                    rows = trailerRows;
                    break;
                case "T":
                    rows = truckbusRows;
                    break;
                case "M":
                    rows = motorcycleRows;
                    break;
                case "":
                    rows = allRows;
                    break;
            }
            return rows;
        }
    }
}