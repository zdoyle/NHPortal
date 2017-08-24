using GD.Highcharts.Enums;
using GD.Highcharts.GDAnalytics;
using GD.Highcharts.Helpers;
using GD.Highcharts.Options;
using GDCoreUtilities;
using GDDatabaseClient.Oracle;
using NHPortal.Classes.Charts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace NHPortal.Classes.Reports.Charts
{
    public static class WelcomePage
    {
        public static ChartContainer LoadWelcomePageContainer()
        {
            NHChartWrapper testCount, rejectCount;
            ChartContainer container = new ChartContainer();

            // Add test Count By Month chart.
            testCount = NHChartWrapper.Create("welcome", "testcount");
            testCount.ChartType = ChartTypes.Line;
            testCount.Height = 275;
            testCount.Width = 450;
            testCount.ShowLegend = false;
            testCount.DashValueTypes = new DashValueType[] { DashValueType.Quantity };
            testCount.ChartTitle = "<b>Test Count By Month</b>";
            testCount.DataGroup = DataGroupMaster.Monthly;
            testCount.BuildChart(
                new XAxis { Type = AxisTypes.Datetime, Title = new XAxisTitle { Text = "<b>Test Month</b>" }, Labels = new XAxisLabels { Formatter = "function() { return Highcharts.dateFormat('%b %Y', this.value); }", }, TickInterval = 2419200000 },
                new YAxis
                {
                    Title = new YAxisTitle { Text = "<b>Test Count</b>", },
                    GridLineColor = System.Drawing.ColorTranslator.FromHtml("#AAAAAA"),
                    Labels = new YAxisLabels { Formatter = "function () {return '' + this.value.toLocaleString();}" }
                }
                );
            testCount.Chart.SetBackgroundColor("#EEEEEE");
            testCount.Chart.SetTitle(new Title { Text = "<b>Test Count By Month</b>", X = -20 });
            testCount.Chart.SetLegend(new Legend { Enabled = false });
            testCount.Chart.SetTooltip(new Tooltip
            {
                Formatter = "function () { return '<span style=\"color:'+ this.point.series.color +';font-size:large;\">\u25CF</span> ' + Highcharts.dateFormat('%b %Y', new Date(this.x)) + ' ' + this.series.name + ': <b>' + this.y.toLocaleString() + '</b>'; }"
            });
            container.AddChartWrapper(testCount);

            // Add % Reject Test By Month Chart.
            rejectCount = NHChartWrapper.Create("welcome", "rejectcount");
            rejectCount.SetChartType(GD.Highcharts.Enums.ChartTypes.Line);
            rejectCount.HighsoftType = GD.Highcharts.Enums.HighsoftType.Chart;
            rejectCount.YearType = GD.Highcharts.Enums.YearType.Calendar;
            rejectCount.TimePeriod = GD.Highcharts.Enums.TimePeriod.Specify;
            rejectCount.Height = 275;
            rejectCount.Width = 450;
            rejectCount.ShowLegend = false;
            rejectCount.DashValueTypes = new DashValueType[] { DashValueType.Quantity };
            rejectCount.ChartTitle = "<b>% Reject Count By Month</b>";
            rejectCount.DataGroup = DataGroupMaster.Monthly;
            rejectCount.BuildChart(
                new XAxis { Type = AxisTypes.Datetime, Title = new XAxisTitle { Text = "<b>Test Month</b>" }, Labels = new XAxisLabels { Formatter = "function() { return Highcharts.dateFormat('%b %Y', this.value); }", }, TickInterval = 2419200000 },
                new YAxis
                {
                    Title = new YAxisTitle { Text = "<b>% Reject Test Count</b>", },
                    GridLineColor = System.Drawing.ColorTranslator.FromHtml("#AAAAAA"),
                    Labels = new YAxisLabels { Formatter = "function () {return '' + this.value.toLocaleString();}" }
                }
                );
            rejectCount.Chart.SetBackgroundColor("#EEEEEE");
            rejectCount.Chart.SetTitle(new Title { Text = "<b>% Reject Test By Month</b>", X = -20 });
            rejectCount.Chart.SetPlotOptions(new PlotOptions
            {
                Series = new PlotOptionsSeries
                {
                    Events = new PlotOptionsSeriesEvents
                    {
                        LegendItemClick = "function(event) { var chrt = $('#welcometestcount_container').highcharts(), series = chrt.get(this.options.id);"
                            + "if (series) { "
                            + " if (this.visible) {"
                            + "   series.hide();"
                            + " } else {"
                            + "   series.show();"
                            + "  }"
                            + " }"
                            + "}"
                    }
                }
            });
            rejectCount.Chart.SetLegend(new Legend
            {
                BackgroundColor = new GD.Highcharts.Helpers.BackColorOrGradient(System.Drawing.ColorTranslator.FromHtml("#FFFFFF")),
                BorderColor = System.Drawing.ColorTranslator.FromHtml("#AAAAAA"),
                BorderWidth = 1
            });
            rejectCount.Chart.SetTooltip(new Tooltip
            {
                Formatter = "function () { return '<span style=\"color:'+ this.point.series.color +';font-size:large;\">\u25CF</span> ' + Highcharts.dateFormat('%b %Y', new Date(this.x)) + ' ' + this.series.name + ': <b>' + this.y + '%</b>'; }"
            });
            container.AddChartWrapper(rejectCount);

            SetWelcomePageSeries(container);

            return container;
        }

        private static void SetWelcomePageSeries(GD.Highcharts.GDAnalytics.ChartContainer container)
        {
            string sql = BuildWelcomeContainerSQL();

            List<SeriesData> testTotalSeriesData = new List<SeriesData>();
            List<SeriesData> testSafetySeriesData = new List<SeriesData>();
            List<SeriesData> testOBDSeriesData = new List<SeriesData>();
            List<SeriesData> testVisSeriesData = new List<SeriesData>();
            List<SeriesData> rejectTotalSeriesData = new List<SeriesData>();
            List<SeriesData> rejectSafetySeriesData = new List<SeriesData>();
            List<SeriesData> rejectOBDSeriesData = new List<SeriesData>();
            List<SeriesData> rejectVisSeriesData = new List<SeriesData>();

            Series testTotal = new Series() { Id = "seriestotal", Name = "Total" };
            Series testSafety = new Series() { Id = "seriessafety", Name = "Safety" };
            Series testOBd = new Series() { Id = "seriesobd", Name = "OBD" };
            Series testVis = new Series() { Id = "seriesvisual", Name = "Visual" };
            Series rejectTotal = new Series() { Id = "seriestotal", Name = "Total" };
            Series rejectSafety = new Series() { Id = "seriessafety", Name = "Safety" };
            Series rejectOBD = new Series() { Id = "seriesobd", Name = "OBD" };
            Series rejectVis = new Series() { Id = "seriesvisual", Name = "Visual" };

            OracleResponse response = BaseReportMaster.GetOracleResponse(sql);
            System.Globalization.CultureInfo provider = System.Globalization.CultureInfo.InvariantCulture;
            TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Mountain Standard Time");

            if (response.Successful && container.WrapperCount == 2)
            {
                for (int i = 0; i < response.ResultsTable.Rows.Count; i++)
                {
                    DateTime testDate;
                    double dateAsMilliseconds;

                    DateTime.TryParseExact(response.ResultsTable.Rows[i]["DATE_GROUPING"].ToString() + "01", "yyyyMMdd", provider, System.Globalization.DateTimeStyles.None, out testDate);
                    dateAsMilliseconds = Tools.GetTotalMilliseconds(testDate, timeZoneInfo);

                    testTotalSeriesData.Add( new SeriesData { Y = NullSafe.ToDouble(response.ResultsTable.Rows[i]["TOTAL_COUNT"]), X = dateAsMilliseconds });
                    testSafetySeriesData.Add( new SeriesData { Y = NullSafe.ToDouble(response.ResultsTable.Rows[i]["SAFETY_COUNT"]), X = dateAsMilliseconds });
                    testOBDSeriesData.Add( new SeriesData { Y = NullSafe.ToDouble(response.ResultsTable.Rows[i]["OBD_COUNT"]), X = dateAsMilliseconds });
                    testVisSeriesData.Add( new SeriesData { Y = NullSafe.ToDouble(response.ResultsTable.Rows[i]["VIS_COUNT"]), X = dateAsMilliseconds });
                    rejectTotalSeriesData.Add( new SeriesData { Y = NullSafe.ToDouble(response.ResultsTable.Rows[i]["TOTAL_PCT"]), X = dateAsMilliseconds });
                    rejectSafetySeriesData.Add( new SeriesData { Y = NullSafe.ToDouble(response.ResultsTable.Rows[i]["SAF_PCT"]), X = dateAsMilliseconds });
                    rejectOBDSeriesData.Add( new SeriesData { Y = NullSafe.ToDouble(response.ResultsTable.Rows[i]["OBD_PCT"]), X = dateAsMilliseconds });
                    rejectVisSeriesData.Add( new SeriesData { Y = NullSafe.ToDouble(response.ResultsTable.Rows[i]["VIS_PCT"]), X = dateAsMilliseconds });
                }

                testTotal.Data = new Data(testTotalSeriesData.ToArray());
                testSafety.Data = new Data(testSafetySeriesData.ToArray());
                testOBd.Data = new Data(testOBDSeriesData.ToArray());
                testVis.Data = new Data(testVisSeriesData.ToArray());
                rejectTotal.Data = new Data(rejectTotalSeriesData.ToArray());
                rejectSafety.Data = new Data(rejectSafetySeriesData.ToArray());
                rejectOBD.Data = new Data(rejectOBDSeriesData.ToArray());
                rejectVis.Data = new Data(rejectVisSeriesData.ToArray());

                container.ChartWrappers[0].Chart.SetSeries(new Series[] { testTotal, testSafety, testOBd, testVis });
                container.ChartWrappers[1].Chart.SetSeries(new Series[] { rejectTotal, rejectSafety, rejectOBD, rejectVis });
            }
        }

        private static string BuildWelcomeContainerSQL()
        {
            StringBuilder builder = new StringBuilder();

            // TODO Make current year. year is changed for testing.
            string startDate = DateTime.Now.AddMonths(-11).ToString("yyyyMM") + "01";
            string endDate = DateTime.Now.ToString("yyyyMMdd");

            builder.AppendLine(" SELECT ");
            builder.AppendLine(" TO_CHAR(TO_DATE(TEST_DATE, 'YYYYMMDD'),'YYYYMM')  DATE_GROUPING, ");
            builder.AppendLine(" SUM(OVERALL_COUNT) TOTAL_COUNT, ");
            builder.AppendLine(" SUM(SAF_COUNT) SAFETY_COUNT, ");
            builder.AppendLine(" SUM(OBD_COUNT) OBD_COUNT, ");
            builder.AppendLine(" SUM(VIS_COUNT) VIS_COUNT, ");
            builder.AppendLine(" NVL(ROUND(SUM(OVERALL_REJECT_COUNT) / NULLIF(SUM(OVERALL_COUNT),0),4),0) * 100 TOTAL_PCT, ");
            builder.AppendLine(" NVL(ROUND(SUM(SAF_REJECT_COUNT) / NULLIF(SUM(SAF_COUNT),0),4),0) * 100 SAF_PCT, ");
            builder.AppendLine(" NVL(ROUND(SUM(OBD_REJECT_COUNT) / NULLIF(SUM(OBD_COUNT),0),4),0) * 100 OBD_PCT, ");
            builder.AppendLine(" NVL(ROUND(SUM(VIS_REJECT_COUNT) / NULLIF(SUM(VIS_COUNT),0),4),0) * 100 VIS_PCT ");
            builder.AppendLine(" FROM PMV_COUNTS ");
            builder.AppendLine(" WHERE TEST_DATE BETWEEN '" + startDate + "' AND '" + endDate + "' ");
            builder.AppendLine(" GROUP BY TO_CHAR(TO_DATE(TEST_DATE, 'YYYYMMDD'),'YYYYMM')  ");
            builder.AppendLine(" ORDER BY DATE_GROUPING ");

            return builder.ToString();
        }
    }
}