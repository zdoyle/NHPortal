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
    public class OBDIIProtocolUsage : BaseChart
    {
        public OBDIIProtocolUsage()
        {
            RunInitialReport = false;
            DrillProcName = "NH_CHRT_GET_PROTOCOL_DATA";
            ChartProcName = "NH_CHRT_GET_PROTOCOL_CODES";
            this.BaseReport = BaseReportMaster.OBDIIProtocolUsage;
        }

        public override void LoadContainer()
        {
            if (ChartParams == null) return;

            Container = new ChartContainer();
            ChartParams.ChartType = ChartTypes.Pie;
            ChartParams.ChartDimention = ChartDimention.ThreeDimentional;
            NHChartWrapper protocolUsage = NHChartWrapper.Create("charts", "obdProtocolUsage");
            protocolUsage.Height = 400;
            protocolUsage.Width = 700;
            protocolUsage.EnableReportDrillDown = true;
            protocolUsage.OverrideDrillFunction = "function(e){ dtcMainChartClick(e.point.series.userOptions.id, e.point.series.name, e.point.name);}";
            protocolUsage.ChartTitle = String.Empty;
            BuildChart(protocolUsage, null);
            Container.AddChartWrapper(protocolUsage);

            GetContainerSeries();
        }

        public override void GetContainerSeries()
        {
            DataTable dt = BaseReportMaster.GetProcedureDataTable(ChartProcName, GetOracleParams(false));

            if (dt.Rows.Count > 3)
            {
                Container.ChartWrappers[0].Chart.SetSeries(new Series
                {
                    Id = "BOTTOM_DRILL_LEVEL",
                    Name = "OBD II Protocol Usage",
                    Data = new Data(LoadSeriesData(dt)),
                    ShowInLegend = false
                });
            }       
        }

        public override void LoadReport()
        {
            DataTable dt;
            Report report = new Report(ChartParams.SelectedPoint + " Protocol Usage Summary");
            ReportRow headerRow = new ReportRow(report);
            ReportCell currentCell;
            ReportRow currentRow;

            dt = BaseReportMaster.GetProcedureDataTable(DrillProcName, GetOracleParams(true));

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
                    currentCell.Value = dRow[i].ToString();
                    currentRow.Cells.Add(currentCell);
                }
                report.Rows.Add(currentRow);
            }

            this.Report = report;
        }

        private SeriesData[] LoadSeriesData(DataTable dt)
        {
            List<SeriesData> seriesDataList = new List<SeriesData>();
            int count = 0;

            foreach (DataRow dRow in dt.Rows)
            {
                seriesDataList.Add(new SeriesData { Name = dRow["PROTOCOLNAME"].ToString(), Y = NullSafe.ToDouble(dRow["QUANTITY"]), Drilldown = dRow["PROTOCOLNAME"].ToString(), Color = Colors[count++ % 5] });
            }

            return seriesDataList.ToArray();
        }

        public override string RenderContainer()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(" <div class='div-centered'> ");
            builder.AppendLine(" <table class='tbl-chrt-obd-readiness'> ");
            builder.AppendLine(" <tr><td class='tbl-chrt-note'>Click any pie wedge to view the supporting data table.</td></tr> ");
            builder.AppendLine(" <tr><td><div class='div-chrt-med-center'>" + Container.ChartWrappers[0].Chart.ChartContainerHtmlString().ToString() + "</div></td></tr> ");
            builder.AppendLine(" </table> ");
            builder.AppendLine(" </div> ");

            return builder.ToString();
        }

        public override OracleParameter[] GetOracleParams(bool isReport)
        {
            List<OracleParameter> paramList = new List<OracleParameter>();

            if (isReport)
            {
                paramList.Add(new OracleParameter("PROTOCOL", OracleDbType.Varchar2, 20, ChartParams.SelectedPoint, ParameterDirection.Input)); // Year/Month/Day
                paramList.Add(new OracleParameter("RECORD_DATA", OracleDbType.RefCursor, ParameterDirection.Output));
            }
            else
            {
                paramList.Add(new OracleParameter("RECORD_DATA", OracleDbType.RefCursor, ParameterDirection.Output));
                return paramList.ToArray();
            }

            return paramList.ToArray();
        }

        // Colors used for pie slices.
        private Color[] Colors = new Color[] { ColorTranslator.FromHtml("#CC00FF"), ColorTranslator.FromHtml("#009933"), ColorTranslator.FromHtml("#1E90FF"), ColorTranslator.FromHtml("#FF0000"), ColorTranslator.FromHtml("#FFA500") };
    }
}