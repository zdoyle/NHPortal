using GD.Highcharts.Enums;
using GD.Highcharts.GDAnalytics;
using GD.Highcharts.Helpers;
using GD.Highcharts.Options;
using GDCoreUtilities;
using GDDatabaseClient.Oracle;
using Oracle.DataAccess.Client;
using PortalFramework.ReportModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;

namespace NHPortal.Classes.Charts
{
    public class EmissionTestRejectionData : BaseChart
    {
        public EmissionTestRejectionData()
        {
            RunInitialReport = true;
            DrillProcName = "NH_CHRT_EMIS_TEST_REJ_DD";
            ChartProcName = "NH_CHRT_EMIS_TEST_REJ_CHARTS";
            this.BaseReport = BaseReportMaster.EmissionTestRejectionRates;
        }

        public EmissionTestRejectionData(ChartParams chartParams)
        {
            this.ChartParams = chartParams;
            LoadContainer();
            RunInitialReport = true;
        }

        public override void LoadContainer()
        {
            if (ChartParams != null)
            {
                NHChartWrapper testCount, rejectCount, pctRejectCount;
                ChartContainer container = new ChartContainer();

                // Add test Count By Year chart.
                testCount = NHChartWrapper.Create("charts", "testcount");
                testCount.ChartTitle = "<b>Test Count By " + ChartParams.GroupByData.GroupLabel + "</b>";
                BuildChart(testCount, "Test Count");
                container.AddChartWrapper(testCount);

                // Add  Reject Test By Month Chart.
                rejectCount = NHChartWrapper.Create("charts", "rejectcount");
                rejectCount.ChartTitle = "<b>Reject Test Count By " + ChartParams.GroupByData.GroupLabel + "</b>";
                BuildChart(rejectCount, "Reject Test Count");
                container.AddChartWrapper(rejectCount);

                // Add % Reject Test By Year Chart.
                pctRejectCount = NHChartWrapper.Create("charts", "pctrejectcount");
                pctRejectCount.ChartTitle = "<b>% Reject Test Count By " + ChartParams.GroupByData.GroupLabel + "</b>";
                BuildChart(pctRejectCount, "% Reject Test Count", true);
                container.AddChartWrapper(pctRejectCount);

                Container = container;

                GetContainerSeries();
            }
        }

        protected void BuildChart(NHChartWrapper wrap, string yAxisLabel, bool isPercent = false)
        {
            wrap.ChartType = ChartParams.ChartType;
            wrap.ChartDimention = ChartParams.ChartDimention;
            wrap.Height = 400;
            wrap.Width = 750;

            wrap.BuildChart(
                    new XAxis
                    {
                        Type = AxisTypes.Datetime,
                        Title = new XAxisTitle { Text = "<b>Test " + ChartParams.GroupByData.GroupLabel + "</b>" },
                        Labels = new XAxisLabels { Formatter = "function() { return Highcharts.dateFormat('" + ChartParams.GroupByData.DateFormat + "', this.value); }", },
                        TickInterval = ChartParams.GroupByData.TickInterval
                    },
                    new YAxis
                    {
                        Title = new YAxisTitle { Text = "<b>" + yAxisLabel + "</b>", },
                        GridLineColor = System.Drawing.ColorTranslator.FromHtml("#AAAAAA"),
                        Labels = new YAxisLabels { Formatter = "function () {return '' + this.value.toLocaleString();}" }
                    }
                    );
            wrap.Chart.SetBackgroundColor("#EEEEEE");
            wrap.Chart.SetTitle(new Title { Text = wrap.ChartTitle, X = -20 });
            wrap.Chart.SetTooltip(new Tooltip
            {
                Formatter = "function () { return '<span style=\"color:'+ this.point.series.color +';font-size:large;\">\u25CF</span> ' + Highcharts.dateFormat('"
                + ChartParams.GroupByData.DateFormat
                + "', new Date(this.x)) + ' ' + this.series.name + ': <b>' + this.y.toLocaleString() + '" + (isPercent ? "%" : "") + "</b>'; }"
            });
        }

        public override void GetContainerSeries()
        {
            if (Container.WrapperCount != 3) return;

            DataTable dt = BaseReportMaster.GetProcedureDataTable(ChartProcName, GetOracleParams(false));

            List<SeriesData> testTotalSeriesData = new List<SeriesData>();
            List<SeriesData> testSafetySeriesData = new List<SeriesData>();
            List<SeriesData> testOBDSeriesData = new List<SeriesData>();
            List<SeriesData> testVisSeriesData = new List<SeriesData>();
            List<SeriesData> rejectTotalSeriesData = new List<SeriesData>();
            List<SeriesData> rejectSafetySeriesData = new List<SeriesData>();
            List<SeriesData> rejectOBDSeriesData = new List<SeriesData>();
            List<SeriesData> rejectVisSeriesData = new List<SeriesData>();
            List<SeriesData> pctRejectTotalSeriesData = new List<SeriesData>();
            List<SeriesData> pctRejectSafetySeriesData = new List<SeriesData>();
            List<SeriesData> pctRejectOBDSeriesData = new List<SeriesData>();
            List<SeriesData> pctRejectVisSeriesData = new List<SeriesData>();

            Series testTotal = new Series() { Id = "seriestotal", Name = "Total Test" };
            Series testSafety = new Series() { Id = "seriessafety", Name = "Safety Test" };
            Series testOBd = new Series() { Id = "seriesobd", Name = "OBD Test" };
            Series testVis = new Series() { Id = "seriesvisual", Name = "Visual Test" };
            Series rejectTotal = new Series() { Id = "seriestotal", Name = "Total Reject Test" };
            Series rejectSafety = new Series() { Id = "seriessafety", Name = "Safety Reject Test" };
            Series rejectOBD = new Series() { Id = "seriesobd", Name = "OBD Reject Test" };
            Series rejectVis = new Series() { Id = "seriesobd", Name = "Visual Reject Test" };
            Series pctRejectTotal = new Series() { Id = "seriestotal", Name = "Total % Reject Test" };
            Series pctRejectSafety = new Series() { Id = "seriessafety", Name = "Safety % Reject Test" };
            Series pctRejectOBD = new Series() { Id = "seriesobd", Name = "OBD % Reject Test" };
            Series pctRejectVis = new Series() { Id = "seriesvisual", Name = "Visual % Reject Test" };

            System.Globalization.CultureInfo provider = System.Globalization.CultureInfo.InvariantCulture;
            TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Mountain Standard Time");

            if (dt != null && dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DateTime testDate;
                    double dateAsMilliseconds;

                    DateTime.TryParseExact(dt.Rows[i]["TEST_DATE"].ToString() + ChartParams.GroupByData.DateMask, "yyyyMMdd", provider, System.Globalization.DateTimeStyles.None, out testDate);
                    dateAsMilliseconds = Tools.GetTotalMilliseconds(testDate, timeZoneInfo);

                    testTotalSeriesData.Add(new SeriesData { Y = NullSafe.ToDouble(dt.Rows[i]["OVERALL_COUNT"]), X = dateAsMilliseconds });
                    testSafetySeriesData.Add(new SeriesData { Y = NullSafe.ToDouble(dt.Rows[i]["SAF_COUNT"]), X = dateAsMilliseconds });
                    testOBDSeriesData.Add(new SeriesData { Y = NullSafe.ToDouble(dt.Rows[i]["OBD_COUNT"]), X = dateAsMilliseconds });
                    testVisSeriesData.Add(new SeriesData { Y = NullSafe.ToDouble(dt.Rows[i]["VIS_COUNT"]), X = dateAsMilliseconds });
                    rejectTotalSeriesData.Add(new SeriesData { Y = NullSafe.ToDouble(dt.Rows[i]["OVERALL_REJECT_COUNT"]), X = dateAsMilliseconds });
                    rejectSafetySeriesData.Add(new SeriesData { Y = NullSafe.ToDouble(dt.Rows[i]["SAF_REJECT_COUNT"]), X = dateAsMilliseconds });
                    rejectOBDSeriesData.Add(new SeriesData { Y = NullSafe.ToDouble(dt.Rows[i]["OBD_REJECT_COUNT"]), X = dateAsMilliseconds });
                    rejectVisSeriesData.Add(new SeriesData { Y = NullSafe.ToDouble(dt.Rows[i]["VIS_REJECT_COUNT"]), X = dateAsMilliseconds });
                    pctRejectTotalSeriesData.Add(new SeriesData { Y = NullSafe.ToDouble(dt.Rows[i]["OVERALL_REJECT_PCT"]), X = dateAsMilliseconds });
                    pctRejectSafetySeriesData.Add(new SeriesData { Y = NullSafe.ToDouble(dt.Rows[i]["SAF_REJECT_PCT"]), X = dateAsMilliseconds });
                    pctRejectOBDSeriesData.Add(new SeriesData { Y = NullSafe.ToDouble(dt.Rows[i]["OBD_REJECT_PCT"]), X = dateAsMilliseconds });
                    pctRejectVisSeriesData.Add(new SeriesData { Y = NullSafe.ToDouble(dt.Rows[i]["VIS_REJECT_PCT"]), X = dateAsMilliseconds });
                }

                testTotal.Data = new Data(testTotalSeriesData.ToArray());
                testSafety.Data = new Data(testSafetySeriesData.ToArray());
                testOBd.Data = new Data(testOBDSeriesData.ToArray());
                testVis.Data = new Data(testVisSeriesData.ToArray());
                rejectTotal.Data = new Data(rejectTotalSeriesData.ToArray());
                rejectSafety.Data = new Data(rejectSafetySeriesData.ToArray());
                rejectOBD.Data = new Data(rejectOBDSeriesData.ToArray());
                rejectVis.Data = new Data(rejectVisSeriesData.ToArray());
                pctRejectTotal.Data = new Data(pctRejectTotalSeriesData.ToArray());
                pctRejectSafety.Data = new Data(pctRejectSafetySeriesData.ToArray());
                pctRejectOBD.Data = new Data(pctRejectOBDSeriesData.ToArray());
                pctRejectVis.Data = new Data(pctRejectVisSeriesData.ToArray());

                Container.ChartWrappers[0].Chart.SetSeries(new Series[] { testTotal, testSafety, testOBd, testVis });
                Container.ChartWrappers[1].Chart.SetSeries(new Series[] { rejectTotal, rejectSafety, rejectOBD, rejectVis });
                Container.ChartWrappers[2].Chart.SetSeries(new Series[] { pctRejectTotal, pctRejectSafety, pctRejectOBD, pctRejectVis });
            }
        }

        public override OracleParameter[] GetOracleParams(bool isReport)
        {
            List<OracleParameter> paramList = new List<OracleParameter>();

            string dateFreq = NHChartMaster.GetDateFrequency(ChartParams.GroupByData.DataGrouping);

            if (isReport)
            {
                string tableType = ChartParams.TableType;
                string drillLevel = ChartParams.DrillDown.DrillLevel.ToString();
                string selectedPoint = ChartParams.SelectedPoint;
                string testDate = ChartParams.DrillDown.DrillLevels[0].DrillValue;
                string firstDrillLevelVal = ChartParams.DrillDown.DrillLevels[1].DrillValue;
                string secondDrillLevelVal = ChartParams.DrillDown.DrillLevels[2].DrillValue;

                paramList.Add(new OracleParameter("DateFrequency", OracleDbType.Varchar2, 8, dateFreq, ParameterDirection.Input));
                paramList.Add(new OracleParameter("StartDate", OracleDbType.Varchar2, 8, ChartParams.StartDateText, ParameterDirection.Input));
                paramList.Add(new OracleParameter("EndDate", OracleDbType.Varchar2, 8, ChartParams.EndDateText, ParameterDirection.Input));
                paramList.Add(new OracleParameter("DDLvl0", OracleDbType.Varchar2, 25, firstDrillLevelVal, ParameterDirection.Input));
                paramList.Add(new OracleParameter("DDLvl1", OracleDbType.Varchar2, 25, secondDrillLevelVal, ParameterDirection.Input));
                paramList.Add(new OracleParameter("TableType", OracleDbType.Varchar2, 3, tableType, ParameterDirection.Input));
                paramList.Add(new OracleParameter("DDLevel", OracleDbType.Varchar2, 1, drillLevel, ParameterDirection.Input));
                paramList.Add(new OracleParameter("TestDate", OracleDbType.Varchar2, 10, testDate, ParameterDirection.Input));
                paramList.Add(new OracleParameter("RECORD_DATA", OracleDbType.RefCursor, ParameterDirection.Output));
            }
            else
            {
                paramList.Add(new OracleParameter("DateFrequency", OracleDbType.Varchar2, 8, dateFreq, ParameterDirection.Input)); // Year/Month/Day
                paramList.Add(new OracleParameter("StartDate", OracleDbType.Varchar2, 8, ChartParams.StartDateText, ParameterDirection.Input));
                paramList.Add(new OracleParameter("EndDate", OracleDbType.Varchar2, 8, ChartParams.EndDateText, ParameterDirection.Input));
                paramList.Add(new OracleParameter("RECORD_DATA", OracleDbType.RefCursor, ParameterDirection.Output));
            }

            return paramList.ToArray();
        }

        public override string RenderContainer()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(" <div class='div-emis-center'> ");
            foreach (NHChartWrapper wrap in Container.ChartWrappers)
            {
                builder.AppendLine(wrap.Chart.ChartContainerHtmlString().ToString());
            }

            builder.AppendLine(" </div> ");
            return builder.ToString();
        }
    }
}