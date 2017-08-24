using GD.Highcharts.GDAnalytics;
using GDCoreUtilities;
using NHPortal.Classes.Charts;
using Oracle.DataAccess.Client;
using PortalFramework.ReportModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace NHPortal.Classes.Reports.Triggers
{
    public class WeightedScore : BaseTrigger
    {
        public WeightedScore()
        {
            RunInitialReport = false;
            DetailedReportProcName = "NH_TRIG_DETAILED_REPORT";
            DrillProcName = "NH_TRIG_WEIGHTED_DATA";
            SubLevelDrillProcName = "NH_TRIG_WEIGHTED_DD";
            ChartProcName = "NH_TRIG_WEIGHTED_CHART";
            this.BaseReport = BaseReportMaster.WeightedTriggerScore;
        }

        public override void SetMetaData()
        {
            base.SetMetaData();

            MetaList.Add(new ChartMetaData("Threshold", ThresholdMins));
        }

        public override void LoadReport()
        {
            DataTable dt;
            Report report = new Report(this.BaseReport.ReportTitle + " Data");
            ReportRow headerRow = new ReportRow(report);
            ReportCell currentCell;
            ReportRow currentRow;
            string onclick = String.Empty;

            dt = BaseReportMaster.GetProcedureDataTable(DrillProcName, GetOracleParams(true));

            ChartParams.DrillDown.DrillLevel = 0;
            report.Sortable = true;
            report.ID = "tblChart";

            report.Columns.Add("TRIGGER_IQ", "Trigger IQ");
            report.Columns.Add("STATION_INSPECTOR", StationInspector + " Id");
            report.Columns.Add("TEST_COUNT", "Test Count");
            report.Columns.Add("WTS_SCORE", "WTS Score");

            // Use String.Format to add trigger weights to the column header values.
            foreach (KeyValuePair<string, ReportColumnInfo> kvp in BaseReport.ReportColumnData)
            {
                report.Columns.Add(kvp.Key, FormatHeaderWeights(kvp), kvp.Value.ColumnDataType);
            }

            foreach (DataRow dRow in dt.Rows)
            {
                currentRow = new ReportRow(report);

                currentCell = new ReportCell();
                currentCell.Value = "Details";
                currentCell.Href = "#/";
                currentCell.OnClick = "javascript: triggerDetailClick('" + dRow[0].ToString() + "')";
                currentRow.Cells.Add(currentCell);

                currentCell = new ReportCell();
                currentCell.Value = dRow[StationInspector + "ID"].ToString();
                currentRow.Cells.Add(currentCell);

                currentCell = new ReportCell();
                currentCell.Value = dRow["SIDCNT"].ToString();
                currentRow.Cells.Add(currentCell);

                currentCell = new ReportCell();
                currentCell.Value = dRow["TCW"].ToString();
                currentRow.Cells.Add(currentCell);

                foreach (KeyValuePair<string, ReportColumnInfo> kvp in BaseReport.ReportColumnData)
                {
                    currentCell = new ReportCell(kvp.Key);
                    currentCell.Value = dRow[kvp.Key].ToString();
                    currentCell.Href = "#/";
                    onclick = "javascript: drillDown(this.parentNode.parentNode, '" + kvp.Key + "', 0,'" + dRow[StationInspector + "ID"].ToString() + "', '');";
                    currentCell.OnClick = onclick;
                    currentRow.Cells.Add(currentCell);
                }

                report.Rows.Add(currentRow);
            }

            this.Report = report;
        }

        private string FormatHeaderWeights(KeyValuePair<string, ReportColumnInfo> kvp)
        {
            string returnVal = String.Empty;

            returnVal = String.Format(kvp.Value.DisplayName, this.TriggerWeights[kvp.Key].ToString());

            return returnVal;
        }

        public override string LoadDrillLevel(string weightType, string drillValue, string prevDrillValue, int drillLevel)
        {
            string returnVal, tableTitle;
            string tableName = "tbl" + weightType + drillValue + prevDrillValue;
            Table table = GetReportTable(tableName.Replace("(", "").Replace(")", ""));
            TableHeaderRow headerRow;
            TableRow currentRow;
            TableCell currentCell;
            DataTable dt;
            ChartParams.DrillDown.DrillLevel = NullSafe.ToInt(drillLevel);
            ChartParams.SelectedPoint = weightType;
            ChartParams.DrillDown.DrillLevels[0].DrillValue = ChartParams.SelectedPoint;


            IDNumberString = drillValue;
            headerRow = GetDrillTableHeader(drillLevel);

            if (ChartParams.DrillDown.DrillLevels.Length > 2)
            {
                if (String.IsNullOrEmpty(prevDrillValue))
                {
                    ChartParams.DrillDown.DrillLevels[1].DrillValue = drillValue;
                    ChartParams.DrillDown.DrillLevels[2].DrillValue = prevDrillValue;
                }
                else
                {
                    ChartParams.DrillDown.DrillLevels[1].DrillValue = prevDrillValue;
                    ChartParams.DrillDown.DrillLevels[2].DrillValue = drillValue;
                }
            }

            table.Rows.Add(headerRow);

            dt = BaseReportMaster.GetProcedureDataTable(SubLevelDrillProcName, GetDrillOracleParams());

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dRow in dt.Rows)
                {
                    currentRow = new TableRow();

                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        currentCell = new TableCell();

                        if (drillLevel == 0 && i == 0)
                        {
                            currentCell.Text = "<a href='#/' onclick=\"drillDown(this.parentNode.parentNode,'"
                                + weightType + "',"
                                + (drillLevel + 1).ToString()
                                + ",'"
                                + dRow[0].ToString().Trim() + "','"
                                + drillValue + "',''); \">"
                                + dRow[0].ToString().Trim() + "</a>";
                        }
                        else
                        {
                            currentCell.Text = dRow[i].ToString();
                        }

                        currentRow.Cells.Add(currentCell);
                    }

                    table.Rows.Add(currentRow);
                }
            }

            returnVal = NHChartMaster.RenderHTMLControl(table);

            try
            {
                KeyValuePair<string,ReportColumnInfo> weightKey = BaseReport.ReportColumnData.FirstOrDefault(x => x.Key == weightType);

                tableTitle = String.Format(weightKey.Value.DisplayName, TriggerWeights[weightKey.Key]);
            }
            catch
            {
                tableTitle = " Trigger Weight";
            }

            returnVal = "<div class='trigger-drill-title'> Data Table for " + tableTitle + "<br /></div>" + returnVal;

            return returnVal;
        }

        public override TableHeaderRow GetDrillTableHeader(int drillLevel)
        {
            TableHeaderRow headerRow = new TableHeaderRow();
            TableHeaderCell headerCell;

            headerRow.TableSection = TableRowSection.TableHeader;

            headerCell = new TableHeaderCell();
            headerCell.Text = ChartParams.DrillDown.DrillLevels[drillLevel].DrillType.ToString() + " Id";

            headerRow.Cells.Add(headerCell);

            foreach (KeyValuePair<string, ReportColumnInfo> kvp in GetDetailedReportColumns())
            {
                headerCell = new TableHeaderCell();
                headerCell.Text = kvp.Value.DisplayName;
                headerRow.Cells.Add(headerCell);
            }

            return headerRow;
        }

        private OracleParameter[] GetDrillOracleParams()
        {
            List<OracleParameter> paramList = new List<OracleParameter>();

            string decile = ChartParams.SelectedPoint == null ? "" : ChartParams.SelectedPoint;
            string showSub = ChartParams.DrillDown.DrillLevel == 0 ? String.Empty : "YES";
            string showType = ChartParams.SelectedPoint;

            paramList.Add(new OracleParameter("StartDate", OracleDbType.Varchar2, 8, ChartParams.StartDateText, ParameterDirection.Input));
            paramList.Add(new OracleParameter("EndDate", OracleDbType.Varchar2, 8, ChartParams.EndDateText, ParameterDirection.Input));
            paramList.Add(new OracleParameter("ShowType", OracleDbType.Varchar2, 6, showType, ParameterDirection.Input));
            paramList.Add(new OracleParameter("threshMins", OracleDbType.Varchar2, 1, ThresholdMins, ParameterDirection.Input));
            paramList.Add(new OracleParameter("StationId", OracleDbType.Varchar2, 10, IDNumberString, ParameterDirection.Input));
            paramList.Add(new OracleParameter("ShowSub", OracleDbType.Varchar2, 3, showSub, ParameterDirection.Input));
            paramList.Add(new OracleParameter("InspectorId", OracleDbType.Varchar2, 3, "", ParameterDirection.Input));
            paramList.Add(new OracleParameter("TableType", OracleDbType.Varchar2, 3, ChartParams.TableType, ParameterDirection.Input));
            paramList.Add(new OracleParameter("RECORD_DATA", OracleDbType.RefCursor, ParameterDirection.Output));

            return paramList.ToArray();
        }

        public override OracleParameter[] GetOracleParams(bool isReport)
        {
            List<OracleParameter> paramList = new List<OracleParameter>();

            string decile = ChartParams.SelectedPoint == null ? "" : ChartParams.SelectedPoint;
            paramList.Add(new OracleParameter("StartDate", OracleDbType.Varchar2, 8, ChartParams.StartDateText, ParameterDirection.Input));
            paramList.Add(new OracleParameter("EndDate", OracleDbType.Varchar2, 8, ChartParams.EndDateText, ParameterDirection.Input));
            paramList.Add(new OracleParameter("OBDProtocolWt", OracleDbType.Varchar2, 3, TriggerWeights["OBDPDA"].ToString(), ParameterDirection.Input));
            paramList.Add(new OracleParameter("OBDRejectWt", OracleDbType.Varchar2, 3, TriggerWeights["OBDRDA"].ToString(), ParameterDirection.Input));
            paramList.Add(new OracleParameter("OBDReadinessWt", OracleDbType.Varchar2, 3, TriggerWeights["OBDIDA"].ToString(), ParameterDirection.Input));
            paramList.Add(new OracleParameter("SafetyRejectWt", OracleDbType.Varchar2, 3, TriggerWeights["SDDA"].ToString(), ParameterDirection.Input));
            paramList.Add(new OracleParameter("NoVoltWt", OracleDbType.Varchar2, 3, TriggerWeights["NOVDA"].ToString(), ParameterDirection.Input));
            paramList.Add(new OracleParameter("eVINWt", OracleDbType.Varchar2, 3, TriggerWeights["EVINDA"].ToString(), ParameterDirection.Input));
            paramList.Add(new OracleParameter("TBTWt", OracleDbType.Varchar2, 3, TriggerWeights["TBTDA"].ToString(), ParameterDirection.Input));
            paramList.Add(new OracleParameter("threshMins", OracleDbType.Varchar2, 1, ThresholdMins, ParameterDirection.Input));

            if(isReport)
            {
                paramList.Add(new OracleParameter("DASel", OracleDbType.Varchar2, 3, decile, ParameterDirection.Input));
                paramList.Add(new OracleParameter("TCFilter", OracleDbType.Varchar2, 3, FilterCount, ParameterDirection.Input));
            }

            paramList.Add(new OracleParameter("TableType", OracleDbType.Varchar2, 3, ChartParams.TableType, ParameterDirection.Input));
            paramList.Add(new OracleParameter("RECORD_DATA", OracleDbType.RefCursor, ParameterDirection.Output));

            return paramList.ToArray();
        }

    }
}