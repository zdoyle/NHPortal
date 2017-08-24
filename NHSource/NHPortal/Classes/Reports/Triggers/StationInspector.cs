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
    public class StationInspector : BaseTrigger
    {
        public StationInspector()
        {
            RunInitialReport = true;
            DetailedReportProcName = "NH_TRIG_DETAILED_REPORT";
            DrillProcName = "NH_TRIG_STATION_INSP_DATA";
            ChartProcName = "NH_TRIG_STATION_INSP_CHART";
            SubLevelDrillProcName = "NH_TRIG_ORM_TRIGGER_REPORT";
            this.BaseReport = BaseReportMaster.StationIpectorTrigger;
        }

        public override void SetMetaData()
        {
            MetaList = new List<Classes.Charts.ChartMetaData>();
            MetaList.Add(new ChartMetaData("Start Date", ChartParams.StartDate));
            MetaList.Add(new ChartMetaData("End Date", ChartParams.EndDate));
            MetaList.Add(new ChartMetaData("Search Filter", StationInspector));
            MetaList.Add(new ChartMetaData("Id Number", IDNumberString));
        }

        public override void LoadReport()
        {
            DataTable dt;
            Report report = new Report("Trigger Data Overview");
            ReportRow headerRow = new ReportRow(report);
            ReportCell currentCell;
            ReportRow currentRow;
            string onclick = String.Empty;

            dt = BaseReportMaster.GetProcedureDataTable(ChartProcName, GetOracleParams(false));

            ChartParams.DrillDown.DrillLevel = 0;
            report.Sortable = true;
            report.ID = "tblChart";

            foreach (KeyValuePair<string, ReportColumnInfo> kvp in BaseReport.ReportColumnData)
            {
                report.Columns.Add(kvp.Key, kvp.Value.DisplayName, kvp.Value.ColumnDataType);
            }

            foreach (DataRow dRow in dt.Rows)
            {
                currentRow = new ReportRow(report);
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    currentCell = new ReportCell();
                    if (i == 0)
                    {
                        currentCell = new ReportCell();
                        currentCell.Value = "Details";
                        currentCell.Href = "#/";
                        currentCell.OnClick = "javascript: triggerDetailClick('" + dRow[0].ToString() + "')";
                        currentRow.Cells.Add(currentCell);

                        if (dRow[1] != null)
                        {
                            onclick = "javascript: drillDown(this.parentNode.parentNode, '" + dRow[0].ToString().Trim() + "', 0,'" + dRow[1].ToString().Replace("\"", "*").Trim() + "', '');";
                        }
                    }

                    currentCell = new ReportCell();
                    currentCell.Value = dRow[i].ToString();
                    currentRow.Cells.Add(currentCell);

                    if (i == 0)
                    {
                        currentCell.Href = "#/";
                        currentCell.OnClick = onclick;
                    }
                    else if (i == 1 && dRow[0].ToString().Trim() == "Readiness Mismatch Trigger")
                    {
                        currentCell.Href = "#/";
                        currentCell.OnClick = "javascript: loadSIReadinessData();";
                    }

                }
                report.Rows.Add(currentRow);
            }

            this.Report = report;
        }

        public override TableHeaderRow GetDrillTableHeader(int drillLevel)
        {
            TableHeaderRow headerRow = new TableHeaderRow();
            TableHeaderCell headerCell;
            Dictionary<string, ReportColumnInfo> reportColumnData;

            reportColumnData = GetDetailedReportColumns();
            headerRow.TableSection = TableRowSection.TableHeader;

            headerCell = new TableHeaderCell();
            headerCell.Text = ChartParams.DrillDown.DrillLevels[drillLevel].DrillType.ToString() + " Id";

            headerRow.Cells.Add(headerCell);

            foreach (KeyValuePair<string, ReportColumnInfo> kvp in reportColumnData)
            {
                headerCell = new TableHeaderCell();
                headerCell.Text = kvp.Value.DisplayName;
                headerRow.Cells.Add(headerCell);
            }

            return headerRow;
        }

        public override void LoadContainer()
        {

        }

        public override string RenderContainer()
        {
            return String.Empty;
        }

        public override OracleParameter[] GetOracleParams(bool isDrillReport)
        {
            List<OracleParameter> paramList = new List<OracleParameter>();

            paramList.Add(new OracleParameter("StartDate", OracleDbType.Varchar2, 8, ChartParams.StartDateText, ParameterDirection.Input));
            paramList.Add(new OracleParameter("EndDate", OracleDbType.Varchar2, 8, ChartParams.EndDateText, ParameterDirection.Input));
            paramList.Add(new OracleParameter("SearchItem", OracleDbType.Varchar2, 10, IDNumberString, ParameterDirection.Input));
            paramList.Add(new OracleParameter("SearchFilter", OracleDbType.Varchar2, 9, StationInspector, ParameterDirection.Input));

            if (isDrillReport)
            {
                paramList.Add(new OracleParameter("TriggerID", OracleDbType.Varchar2, 50, this.ChartParams.DrillDown.DrillLevels[0].DrillValue, ParameterDirection.Input));
                paramList.Add(new OracleParameter("DDLevel", OracleDbType.Varchar2, 1, ChartParams.DrillDown.DrillLevel - 1, ParameterDirection.Input));
            }

            paramList.Add(new OracleParameter("RECORD_DATA", OracleDbType.RefCursor, ParameterDirection.Output));

            return paramList.ToArray();
        }

        public override OracleParameter[] GetDetailedOracleParams()
        {
            List<OracleParameter> paramList = new List<OracleParameter>();

            string decile = ChartParams.SelectedPoint == null ? "" : ChartParams.SelectedPoint;
            string tableType = StationInspector == "STATION" ? "SI" : "IS";

            paramList.Add(new OracleParameter("StartDate", OracleDbType.Varchar2, 8, ChartParams.StartDateText, ParameterDirection.Input));
            paramList.Add(new OracleParameter("EndDate", OracleDbType.Varchar2, 8, ChartParams.EndDateText, ParameterDirection.Input));
            paramList.Add(new OracleParameter("SIDSel", OracleDbType.Varchar2, 25, IDNumberString, ParameterDirection.Input));
            paramList.Add(new OracleParameter("TableType", OracleDbType.Varchar2, 3, tableType, ParameterDirection.Input));
            paramList.Add(new OracleParameter("RECORD_DATA", OracleDbType.RefCursor, ParameterDirection.Output));

            return paramList.ToArray();
        }

        private OracleParameter[] GetSIReadinessOracleParams()
        {
            List<OracleParameter> paramList = new List<OracleParameter>();

            paramList.Add(new OracleParameter("StartDate", OracleDbType.Varchar2, 8, ChartParams.StartDateText, ParameterDirection.Input));
            paramList.Add(new OracleParameter("EndDate", OracleDbType.Varchar2, 8, ChartParams.EndDateText, ParameterDirection.Input));
            paramList.Add(new OracleParameter("SearchFilter", OracleDbType.Varchar2, 10, StationInspector, ParameterDirection.Input));
            paramList.Add(new OracleParameter("RECORD_DATA", OracleDbType.RefCursor, ParameterDirection.Output));

            return paramList.ToArray();
        }

        internal string LoadReadinessReport()
        {
            string returnVal = String.Empty;
            Table table = GetReportTable("tblSIReadiness");
            TableHeaderRow headerRow;
            TableHeaderCell headerCell;
            TableRow currentRow;
            TableCell currentCell;
            DataTable dt;
            int stationInspectorCount = 0;

            table.CssClass = "tablesorter report-container__table";

            headerRow = new TableHeaderRow();
            headerRow.TableSection = TableRowSection.TableHeader;

            headerCell = new TableHeaderCell();
            headerCell.Text = "Decile";
            headerRow.Cells.Add(headerCell);

            headerCell = new TableHeaderCell();
            headerCell.Text = "Average Percent Mismatch";
            headerRow.Cells.Add(headerCell);

            headerCell = new TableHeaderCell();
            headerCell.Text = StationInspectorMetaData + " Count";
            headerRow.Cells.Add(headerCell);

            table.Rows.Add(headerRow);

            dt = BaseReportMaster.GetProcedureDataTable(SubLevelDrillProcName, GetSIReadinessOracleParams());

            // Add data as a temp report that gets cleared when overlay is hidden.
            Report report = new PortalFramework.ReportModel.Report("Readiness Trigger Data", dt); 
            SessionHelper.SetAuxiliaryReport(HttpContext.Current.Session, report);

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dRow in dt.Rows)
                {
                    currentRow = new TableRow();

                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        currentCell = new TableCell();
                        currentCell.Text = dRow[i].ToString();
                        currentRow.Cells.Add(currentCell);
                        stationInspectorCount += NullSafe.ToInt(dRow[i]);
                    }

                    table.Rows.Add(currentRow);
                }

                returnVal = "<div class='totalCount'>Total " + StationInspectorMetaData + " Count: " + stationInspectorCount.ToString() + "</div>" + NHChartMaster.RenderHTMLControl(table);
            }
            else
            {
                returnVal = "<div class='no-data-found'>No Data Found.</div>";
            }

            return returnVal;
        }
    }
}