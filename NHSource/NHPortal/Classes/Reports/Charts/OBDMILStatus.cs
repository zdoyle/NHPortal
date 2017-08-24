using GD.Highcharts.GDAnalytics;
using GD.Highcharts.Helpers;
using GD.Highcharts.Options;
using GDCoreUtilities;
using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;

namespace NHPortal.Classes.Charts
{
    public class OBDMILStatus : BaseChart
    {
        public OBDMILStatus()
        {
            RunInitialReport = false;
            DrillProcName = "NH_CHRT_MILSTATUS_DD";
            ChartProcName = "NH_CHRT_MILSTATUS_CHART";
            XaxisTitle = "Mil Status";
            this.BaseReport = BaseReportMaster.OBDIIMILStatus;
        }

        private const string NotReady = "Not-Ready";
        private const string On = "On";
        private const string Off = "Off";

        private static readonly string[] milCategories = { NotReady, Off, On };

        public override void LoadContainer()
        {
            if (ChartParams == null) return;

            Container = new ChartContainer();

            // MIL Status.
            NHChartWrapper milStatus;

            milStatus = NHChartWrapper.Create("charts", "milstatus");
            milStatus.Height = 400;
            milStatus.Width = 600;
            milStatus.ChartTitle = String.Empty;
            milStatus.EnableReportDrillDown = true;
            BuildChart(milStatus, milCategories);
            Container.AddChartWrapper(milStatus);

            GetContainerSeries();
        }

        public override void GetContainerSeries()
        {
            DataTable dt = BaseReportMaster.GetProcedureDataTable(ChartProcName, GetOracleParams(false));

            if (dt.Rows.Count < 3 || (dt.Rows.Count == 3 && dt.Rows[0]["QUANTITY"].ToString().Trim() == String.Empty)) return;

            Container.ChartWrappers[0].Chart.SetSeries(new Series { Id = "MILStatus", Name = "MIL Status", Data = new Data(LoadSeriesData(dt.Rows[0]["QUANTITY"], dt.Rows[1]["QUANTITY"], dt.Rows[2]["QUANTITY"])), ShowInLegend = false });
        }

        private SeriesData[] LoadSeriesData(object notReady, object off, object on)
        {
            List<SeriesData> seriesDataList = new List<SeriesData>();

            double notReadyVal, offVal, onVal;

            notReadyVal = NullSafe.ToDouble(notReady);
            offVal = NullSafe.ToDouble(off);
            onVal = NullSafe.ToDouble(on);

            seriesDataList.Add(new SeriesData { Name = "Not Ready", Y = notReadyVal, Color = ColorTranslator.FromHtml("#FF0000") });
            seriesDataList.Add(new SeriesData { Name = "Off", Y = offVal, Color = ColorTranslator.FromHtml("#009933") });
            seriesDataList.Add(new SeriesData { Name = "On", Y = onVal, Color = ColorTranslator.FromHtml("#1E90FF") });

            return seriesDataList.ToArray();
        }

        public override string RenderContainer()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(" <div class='div-centered'> ");
            builder.AppendLine(" <table class='tbl-chrt-obd-readiness'> ");
            builder.AppendLine(" <tr><td class='tbl-chrt-note'>Click on a chart data point to view the supporting data table.</td></tr> ");
            builder.AppendLine(" <tr><td class='tbl-chrt-header'>MIL Status</td></tr> ");
            builder.AppendLine(" <tr><td><div class='div-chrt-mil-center'>" + Container.ChartWrappers[0].Chart.ChartContainerHtmlString().ToString() + "</div></td></tr> ");
            builder.AppendLine(" </table> ");
            builder.AppendLine(" </div> ");

            return builder.ToString();
        }

        public override OracleParameter[] GetOracleParams(bool isReport)
        {
            List<OracleParameter> paramList = new List<OracleParameter>();

            if (isReport)
            {
                string tableType = ChartParams.TableType;
                string dateFreq = NHChartMaster.GetDateFrequency(ChartParams.GroupByData.DataGrouping);
                string drillLevel = ChartParams.DrillDown.DrillLevel.ToString();
                string selectedPoint = ChartParams.SelectedPoint;
                string testDate = ChartParams.DrillDown.DrillLevels[0].DrillValue;
                string firstDrillLevelVal = ChartParams.DrillDown.DrillLevels[1].DrillValue;
                string secondDrillLevelVal = ChartParams.DrillDown.DrillLevels[2].DrillValue;

                paramList.Add(new OracleParameter("DateFrequency", OracleDbType.Varchar2, 8, dateFreq, ParameterDirection.Input)); // Year/Month/Day
                paramList.Add(new OracleParameter("StartDate", OracleDbType.Varchar2, 8, ChartParams.StartDateText, ParameterDirection.Input));
                paramList.Add(new OracleParameter("EndDate", OracleDbType.Varchar2, 8, ChartParams.EndDateText, ParameterDirection.Input));
                paramList.Add(new OracleParameter("DDSel", OracleDbType.Varchar2, 12, selectedPoint, ParameterDirection.Input)); // Ready, Not Ready, etc.
                paramList.Add(new OracleParameter("DDLvl0", OracleDbType.Varchar2, 25, firstDrillLevelVal, ParameterDirection.Input)); // 'make', 'model', 'model year' values.
                paramList.Add(new OracleParameter("DDLvl1", OracleDbType.Varchar2, 25, secondDrillLevelVal, ParameterDirection.Input)); // 'make', 'model', 'model year' values.
                paramList.Add(new OracleParameter("TableType", OracleDbType.Varchar2, 3, tableType, ParameterDirection.Input)); // KYD (maKe,Year,moDel) etc.
                paramList.Add(new OracleParameter("DDLevel", OracleDbType.Varchar2, 1, drillLevel, ParameterDirection.Input)); // 0,1,2?
                paramList.Add(new OracleParameter("TestDate", OracleDbType.Varchar2, 10, testDate, ParameterDirection.Input)); // 'YYYY', 'YYYY-MM', 'YYYY-MM-DD' values.
                paramList.Add(new OracleParameter("RECORD_DATA", OracleDbType.RefCursor, ParameterDirection.Output));
            }
            else
            {
                paramList.Add(new OracleParameter("StartDate", OracleDbType.Varchar2, 8, ChartParams.StartDateText, ParameterDirection.Input));
                paramList.Add(new OracleParameter("EndDate", OracleDbType.Varchar2, 8, ChartParams.EndDateText, ParameterDirection.Input));
                paramList.Add(new OracleParameter("ChartType", OracleDbType.Varchar2, 15, this.ChartParams.ChartType.ToString().ToUpper(), ParameterDirection.Input));
                paramList.Add(new OracleParameter("RECORD_DATA", OracleDbType.RefCursor, ParameterDirection.Output));
            }

            return paramList.ToArray();
        }
    }
}