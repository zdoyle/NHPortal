using GD.Highcharts;
using GD.Highcharts.Enums;
using GD.Highcharts.GDAnalytics;
using GD.Highcharts.Helpers;
using GD.Highcharts.Options;
using GDCoreUtilities;
using Oracle.DataAccess.Client;
using PortalFramework.ReportModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;

namespace NHPortal.Classes.Charts
{
    public class OBDIIDTCErrorCodes : BaseChart
    {
        public OBDIIDTCErrorCodes()
        {
            RunInitialReport = false;
            DrillProcName = "NH_CHRT_GET_DTC_DATA";
            ChartProcName = "NH_CHRT_GET_DTC_CODES";
            this.BaseReport = BaseReportMaster.OBDIIDTCErrorCodes;
        }

        public override void LoadContainer()
        {
            if (ChartParams == null) return;

            Container = new ChartContainer();
            ChartParams.ChartType = ChartTypes.Pie;
            ChartParams.ChartDimention = ChartDimention.ThreeDimentional;
            // OBD II DTC Error Codes.

            NHChartWrapper dtcErrorCodes = NHChartWrapper.Create("charts", "dtcErrorCodes");
            dtcErrorCodes.Height = 650;
            dtcErrorCodes.Width = 850;
            dtcErrorCodes.EnableReportDrillDown = true;
            dtcErrorCodes.OverrideDrillFunction = "function(e){ dtcMainChartClick(e.point.series.userOptions.id, e.point.series.name, e.point.name);}";
            dtcErrorCodes.ChartTitle = String.Empty;
            BuildChart(dtcErrorCodes, null);
            dtcErrorCodes.Chart.SetChartEvent(new ChartEvents()
            {
                Drilldown = NHChartMaster.DrilldownFunction,
                Drillup = ChartConsts.DrillupFunction
            });

            Container.AddChartWrapper(dtcErrorCodes);

            GetContainerSeries();
        }

        public override void LoadReport()
        {
            DataTable dt;
            Report report;
            ReportRow headerRow;
            ReportRow currentRow;
            ReportCell currentCell;

            if (ChartParams.SelectedSeriesName == "ALL_CODES" && ChartParams.SelectedPoint.Length > 1)
            {
                ChartParams.SelectedPoint = ChartParams.SelectedPoint.Substring(0, 1);
            }

            ChartParams.DrillDown.DrillLevel = 0;

            report = new Report("DTC " + ChartParams.SelectedPoint.PadRight(5,'X') + " Occurence Summary");
            report.Sortable = true;
            report.ID = "tblChart";
            headerRow = new ReportRow(report);
            
            dt = BaseReportMaster.GetProcedureDataTable(DrillProcName, GetOracleParams(true));

            if (ChartParams.SelectedSeriesName == "ALL_CODES" && ChartParams.SelectedSeries == "ALL")
            {
                report.Columns.Add("DTC", "DTC", ColumnDataType.String);
                report.Columns.Add("QUANTITY", "Quantity", ColumnDataType.Number);
            }
            else
            {
                foreach (KeyValuePair<string, ReportColumnInfo> kvp in BaseReport.ReportColumnData)
                {
                    report.Columns.Add(kvp.Key, kvp.Value.DisplayName, kvp.Value.ColumnDataType);
                }
            }

            foreach (DataRow dRow in dt.Rows)
            {
                currentRow = new ReportRow(report);
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    currentCell = new ReportCell();
                    currentCell.Value = dRow[i].ToString();
                    currentRow.Cells.Add(currentCell);
                }

                report.Rows.Add(currentRow);
            }

            this.Report = report;
        }

        public override void GetContainerSeries()
        {
            DataTable dt = BaseReportMaster.GetProcedureDataTable(ChartProcName, GetOracleParams(false, String.Empty));

            if (dt.Rows.Count < 4) return;

            Container.ChartWrappers[0].Chart.SetSeries(new Series { Id = "MILStatus", Name = "DTC Categories", Data = new Data(LoadSeriesData(dt))});
        }

        private SeriesData[] LoadSeriesData(DataTable dt)
        {
            List<SeriesData> seriesDataList = new List<SeriesData>();
            int count = 0;
            //b red, c green, u yellow, p blue
            foreach (DataRow dRow in dt.Rows)
            {
                seriesDataList.Add(new SeriesData { Name = dRow["DTC"].ToString(), Y = NullSafe.ToDouble(dRow["QUANTITY"]), Drilldown = dRow["DTC"].ToString(), Color = Colors[count++%4] });
            }

            return seriesDataList.ToArray();
        }

        public static OracleParameter[] GetOracleParams(bool isReport, string dtcCode)
        {
            List<OracleParameter> paramList = new List<OracleParameter>();

            paramList.Add(new OracleParameter("DTCCode", OracleDbType.Varchar2, 1, dtcCode, ParameterDirection.Input));
            paramList.Add(new OracleParameter("RECORD_DATA", OracleDbType.RefCursor, ParameterDirection.Output));
            return paramList.ToArray();
        }

        public override OracleParameter[] GetOracleParams(bool isReport)
        {
            List<OracleParameter> paramList = new List<OracleParameter>();

            paramList.Add(new OracleParameter("DTCCode", OracleDbType.Varchar2, 3, ChartParams.SelectedPoint, ParameterDirection.Input));
            paramList.Add(new OracleParameter("ReportType", OracleDbType.Varchar2, 3, ChartParams.SelectedSeries, ParameterDirection.Input)); 
            paramList.Add(new OracleParameter("RECORD_DATA", OracleDbType.RefCursor, ParameterDirection.Output));
            return paramList.ToArray();
        }

        public override string RenderContainer()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(" <div class='div-centered'> ");
            builder.AppendLine(" <table class='tbl-chrt-obd-readiness'> ");
            builder.AppendLine(" <tr><td class='tbl-chrt-note'>Click any pie wedge to view more information. </td></tr> ");
            builder.AppendLine(" <tr><td><div class='div-chrt-dtc-center'>" + Container.ChartWrappers[0].Chart.ChartContainerHtmlString().ToString() + "</div></td></tr> ");
            builder.AppendLine(" </table> ");
            builder.AppendLine(" </div> ");

            return builder.ToString();
        }

        public static string GetDrillSeries(string pointName, string drillName, string seriesId)
        {
            string returnVal;
            DrillSeries series = new DrillSeries() { };
            SeriesData seriesData = new SeriesData();
            List<SeriesData> seriesDataList = new List<SeriesData>();

            DataTable dt = BaseReportMaster.GetProcedureDataTable(ChartProcName, GetOracleParams(false, pointName));

            foreach (DataRow dRow in dt.Rows)
            {
                seriesData = new SeriesData() { Y = NullSafe.ToDouble(dRow["QUANTITY"]), Name = dRow["DTC"].ToString() };
                seriesDataList.Add(seriesData);
            }

            series.Data = new Data(seriesDataList.ToArray());
            series.Name = drillName;
            series.Id = "BOTTOM_DRILL_LEVEL";

            returnVal = JsonSerializer.Serialize(series, true);

            return returnVal;
        }

        private Color[] Colors = new Color[] { ColorTranslator.FromHtml("#FF0000"), ColorTranslator.FromHtml("#009933"), ColorTranslator.FromHtml("#1E90FF"), ColorTranslator.FromHtml("#FFA500") };
    }
}