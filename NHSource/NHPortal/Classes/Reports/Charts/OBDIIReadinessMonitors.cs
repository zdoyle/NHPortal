using GD.Highcharts.Enums;
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
    public class OBDIIReadinessMonitors : BaseChart
    {
        public OBDIIReadinessMonitors()
        {
            RunInitialReport = false;
            DrillProcName = "NH_CHRT_READINESS_DD";
            ChartProcName = "NH_CHRT_READINESS_CHARTS";
            XaxisTitle = "Monitor Status";
            this.BaseReport = BaseReportMaster.OBDIIReadinessMonitors;
        }

        private const int CHART_SMALL_WIDTH = 450;
        private const int CHART_SMALL_HEIGHT = 240;

        private const string NotReady = "Not-Ready";
        private const string Ready = "Ready";
        private const string Unsupported = "Unsupported";

        private static readonly string[] overallCategories = { NotReady, Ready };
        private static readonly string[] monitorCategories = { NotReady, Ready, Unsupported };

        public override void LoadContainer()
        {
            if (ChartParams == null) return;

            Container = new ChartContainer();

            // OBD Readiness Status.
            NHChartWrapper overallReadiness;
            // Continuous Monitors.
            NHChartWrapper misfireMonitor, fuelSystemMonitor, componentMonitor;
            // Non-Continuous Monitors
            NHChartWrapper catalystMonitor, heatedCatalystMonitor, evapMonitor, eGRMonitor, airSystemMonitor, aCMonitor, oxygenMonitor, heatedOxygenMonitor;

            overallReadiness = NHChartWrapper.Create("charts", "overallReadiness");
            overallReadiness.Height = 350;
            overallReadiness.Width = 410;
            overallReadiness.ChartTitle = "Overall Readiness";
            overallReadiness.EnableReportDrillDown = true;
            BuildChart(overallReadiness, overallCategories);
            Container.AddChartWrapper(overallReadiness);

            misfireMonitor = NHChartWrapper.Create("charts", "misfiremonitor");
            misfireMonitor.Height = CHART_SMALL_HEIGHT;
            misfireMonitor.Width = CHART_SMALL_WIDTH;
            misfireMonitor.ChartTitle = "Misfire Monitor";
            misfireMonitor.EnableReportDrillDown = true;
            BuildChart(misfireMonitor, monitorCategories);
            Container.AddChartWrapper(misfireMonitor);

            fuelSystemMonitor = NHChartWrapper.Create("charts", "fuelsysmonitor");
            fuelSystemMonitor.Height = CHART_SMALL_HEIGHT;
            fuelSystemMonitor.Width = CHART_SMALL_WIDTH;
            fuelSystemMonitor.ChartTitle = "Fuel System Monitor";
            fuelSystemMonitor.EnableReportDrillDown = true;
            BuildChart(fuelSystemMonitor, monitorCategories);
            Container.AddChartWrapper(fuelSystemMonitor);

            componentMonitor = NHChartWrapper.Create("charts", "componentmonitor");
            componentMonitor.Height = CHART_SMALL_HEIGHT;
            componentMonitor.Width = CHART_SMALL_WIDTH;
            componentMonitor.ChartTitle = "Component Monitor";
            componentMonitor.EnableReportDrillDown = true;
            BuildChart(componentMonitor, monitorCategories);
            Container.AddChartWrapper(componentMonitor);

            catalystMonitor = NHChartWrapper.Create("charts", "cataylistmonitor");
            catalystMonitor.Height = CHART_SMALL_HEIGHT;
            catalystMonitor.Width = CHART_SMALL_WIDTH;
            catalystMonitor.ChartTitle = "Catalyst Monitor";
            catalystMonitor.EnableReportDrillDown = true;
            BuildChart(catalystMonitor, monitorCategories);
            Container.AddChartWrapper(catalystMonitor);

            heatedCatalystMonitor = NHChartWrapper.Create("charts", "heatedcatalystmonitor");
            heatedCatalystMonitor.Height = CHART_SMALL_HEIGHT;
            heatedCatalystMonitor.Width = CHART_SMALL_WIDTH;
            heatedCatalystMonitor.ChartTitle = "Heated Catalyst Monitor";
            heatedCatalystMonitor.EnableReportDrillDown = true;
            BuildChart(heatedCatalystMonitor, monitorCategories);
            Container.AddChartWrapper(heatedCatalystMonitor);

            evapMonitor = NHChartWrapper.Create("charts", "evapmonitor");
            evapMonitor.Height = CHART_SMALL_HEIGHT;
            evapMonitor.Width = CHART_SMALL_WIDTH;
            evapMonitor.ChartTitle = "Evap Monitor";
            evapMonitor.EnableReportDrillDown = true;
            BuildChart(evapMonitor, monitorCategories);
            Container.AddChartWrapper(evapMonitor);

            eGRMonitor = NHChartWrapper.Create("charts", "egrmonitor");
            eGRMonitor.Height = CHART_SMALL_HEIGHT;
            eGRMonitor.Width = CHART_SMALL_WIDTH;
            eGRMonitor.ChartTitle = "EGR Monitor";
            eGRMonitor.EnableReportDrillDown = true;
            BuildChart(eGRMonitor, monitorCategories);
            Container.AddChartWrapper(eGRMonitor);

            airSystemMonitor = NHChartWrapper.Create("charts", "airsysmonitor");
            airSystemMonitor.Height = CHART_SMALL_HEIGHT;
            airSystemMonitor.Width = CHART_SMALL_WIDTH;
            airSystemMonitor.ChartTitle = "Air System Monitor";
            airSystemMonitor.EnableReportDrillDown = true;
            BuildChart(airSystemMonitor, monitorCategories);
            Container.AddChartWrapper(airSystemMonitor);

            aCMonitor = NHChartWrapper.Create("charts", "acmonitor");
            aCMonitor.Height = CHART_SMALL_HEIGHT;
            aCMonitor.Width = CHART_SMALL_WIDTH;
            aCMonitor.ChartTitle = "A/C Monitor";
            aCMonitor.EnableReportDrillDown = true;
            BuildChart(aCMonitor, monitorCategories);
            Container.AddChartWrapper(aCMonitor);

            oxygenMonitor = NHChartWrapper.Create("charts", "oxygensnsmonitor");
            oxygenMonitor.Height = CHART_SMALL_HEIGHT;
            oxygenMonitor.Width = CHART_SMALL_WIDTH;
            oxygenMonitor.ChartTitle = "Oxygen Sensor Monitor";
            oxygenMonitor.EnableReportDrillDown = true;
            BuildChart(oxygenMonitor, monitorCategories);
            Container.AddChartWrapper(oxygenMonitor);

            heatedOxygenMonitor = NHChartWrapper.Create("charts", "heatedoxysnsmonitor");
            heatedOxygenMonitor.Height = CHART_SMALL_HEIGHT;
            heatedOxygenMonitor.Width = CHART_SMALL_WIDTH;
            heatedOxygenMonitor.ChartTitle = "Heated Oxygen Monitor";
            heatedOxygenMonitor.EnableReportDrillDown = true;
            BuildChart(heatedOxygenMonitor, monitorCategories);
            Container.AddChartWrapper(heatedOxygenMonitor);

            GetContainerSeries();
        }

        public override void GetContainerSeries()
        {
            int overallCount;
            DataTable dt = BaseReportMaster.GetProcedureDataTable(ChartProcName, GetOracleParams(false));

            if (dt == null || dt.Rows.Count < 1 || (dt.Rows.Count == 1 && dt.Rows[0]["OVERALL_COUNT"].ToString().Trim() == String.Empty)) return;

            DataRow row = dt.Rows[0];

            overallCount = NullSafe.ToInt(row["OVERALL_COUNT"]);

            Container.ChartWrappers[0].Chart.SetSeries(new Series { Id = "OVERALL", Name = "Overall", Data = new Data(LoadSeriesData(row["OVERALL_READY_COUNT"], row["OVERALL_NOTREADY_COUNT"], null, overallCount)), ShowInLegend = false });
            Container.ChartWrappers[1].Chart.SetSeries(new Series { Id = "MISFIRE", Name = "Misfire", Data = new Data(LoadSeriesData(row["MISFIRE_READY_COUNT"], row["MISFIRE_NOTREADY_COUNT"], row["MISFIRE_UNSUPPORTED_COUNT"], overallCount)), ShowInLegend = false });
            Container.ChartWrappers[2].Chart.SetSeries(new Series { Id = "FUELSYSTEM", Name = "Fuel System", Data = new Data(LoadSeriesData(row["FUELSYSTEM_READY_COUNT"], row["FUELSYSTEM_NOTREADY_COUNT"], row["FUELSYSTEM_UNSUPPORTED_COUNT"], overallCount)), ShowInLegend = false });
            Container.ChartWrappers[3].Chart.SetSeries(new Series { Id = "COMPONENT", Name = "Component", Data = new Data(LoadSeriesData(row["COMPONENT_READY_COUNT"], row["COMPONENT_NOTREADY_COUNT"], row["COMPONENT_UNSUPPORTED_COUNT"], overallCount)), ShowInLegend = false });
            Container.ChartWrappers[4].Chart.SetSeries(new Series { Id = "CATALYST", Name = "Catalyst", Data = new Data(LoadSeriesData(row["CATALYST_READY_COUNT"], row["CATALYST_NOTREADY_COUNT"], row["CATALYST_UNSUPPORTED_COUNT"], overallCount)), ShowInLegend = false });
            Container.ChartWrappers[5].Chart.SetSeries(new Series { Id = "HCATALYST", Name = "Heated Catalyst", Data = new Data(LoadSeriesData(row["HCATALYST_READY_COUNT"], row["HCATALYST_NOTREADY_COUNT"], row["HCATALYST_UNSUPPORTED_COUNT"], overallCount)), ShowInLegend = false });
            Container.ChartWrappers[6].Chart.SetSeries(new Series { Id = "EVAP", Name = "Evap", Data = new Data(LoadSeriesData(row["EVAP_READY_COUNT"], row["EVAP_NOTREADY_COUNT"], row["EVAP_UNSUPPORTED_COUNT"], overallCount)), ShowInLegend = false });
            Container.ChartWrappers[7].Chart.SetSeries(new Series { Id = "EGR", Name = "EGR", Data = new Data(LoadSeriesData(row["EGR_READY_COUNT"], row["EGR_NOTREADY_COUNT"], row["EGR_UNSUPPORTED_COUNT"], overallCount)), ShowInLegend = false });
            Container.ChartWrappers[8].Chart.SetSeries(new Series { Id = "AIRSYSTEM", Name = "Air System", Data = new Data(LoadSeriesData(row["AIRSYSTEM_READY_COUNT"], row["AIRSYSTEM_NOTREADY_COUNT"], row["AIRSYSTEM_UNSUPPORTED_COUNT"], overallCount)), ShowInLegend = false });
            Container.ChartWrappers[9].Chart.SetSeries(new Series { Id = "AC", Name = "A/C", Data = new Data(LoadSeriesData(row["AC_READY_COUNT"], row["AC_NOTREADY_COUNT"], row["AC_UNSUPPORTED_COUNT"], overallCount)), ShowInLegend = false });
            Container.ChartWrappers[10].Chart.SetSeries(new Series { Id = "OXYGEN", Name = "Oxygen", Data = new Data(LoadSeriesData(row["OXYGEN_READY_COUNT"], row["OXYGEN_NOTREADY_COUNT"], row["OXYGEN_UNSUPPORTED_COUNT"], overallCount)), ShowInLegend = false });
            Container.ChartWrappers[11].Chart.SetSeries(new Series { Id = "HOXYGEN", Name = "Heated Oxygen", Data = new Data(LoadSeriesData(row["HOXYGEN_READY_COUNT"], row["HOXYGEN_NOTREADY_COUNT"], row["HOXYGEN_UNSUPPORTED_COUNT"], overallCount)), ShowInLegend = false });
        }

        private SeriesData[] LoadSeriesData(object ready, object notReady, object unsupported, int overallCount)
        {
            List<SeriesData> seriesDataList = new List<SeriesData>();

            double notReadyVal, readyVal, unsupportedVal;

            if (ChartParams.ChartType == ChartTypes.Pie)
            {
                notReadyVal = NullSafe.ToDouble(notReady);
                readyVal = NullSafe.ToDouble(ready);
                unsupportedVal = NullSafe.ToDouble(unsupported);
            }
            else
            {
                notReadyVal = BaseReportMaster.CalculatePercent(NullSafe.ToInt(notReady), overallCount);
                readyVal = BaseReportMaster.CalculatePercent(NullSafe.ToInt(ready), overallCount);

                if (unsupported == null)
                {
                    unsupportedVal = 0;
                }
                else
                {
                    unsupportedVal = BaseReportMaster.CalculatePercent(NullSafe.ToInt(unsupported), overallCount);
                }
            }

            seriesDataList.Add(new SeriesData { Name = "Not Ready", Y = notReadyVal, Color = ColorTranslator.FromHtml("#FF0000") });
            seriesDataList.Add(new SeriesData { Name = "Ready", Y = readyVal, Color = ColorTranslator.FromHtml("#009933") });

            if (unsupported != null)
            {
                seriesDataList.Add(new SeriesData { Name = "Unsupported", Y = unsupportedVal, Color = ColorTranslator.FromHtml("#1E90FF") });
            }

            return seriesDataList.ToArray();
        }

        public override string RenderContainer()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(" <div class='div-centered'> ");
            builder.AppendLine(" <table class='tbl-chrt-obd-readiness'> ");
            builder.AppendLine(" <tr><td class='tbl-chrt-note'>Click on a chart data point to view the supporting data table.</td></tr> ");
            builder.AppendLine(" <tr><td class='tbl-chrt-header'>OBD Readiness Status</td></tr> ");
            builder.AppendLine(" <tr><td><div class='div-chrt-center'>" + Container.ChartWrappers[0].Chart.ChartContainerHtmlString().ToString() + "</div></td></tr> ");
            builder.AppendLine(" <tr><td class='tbl-chrt-header'>Continuous Monitors</td></tr> ");
            builder.AppendLine(" </table> ");
            builder.AppendLine(" <table class='tbl-chrt-obd-readiness'> ");
            builder.AppendLine(" <tr><td>" + Container.ChartWrappers[1].Chart.ChartContainerHtmlString().ToString() + "</td><td class='chrt-td-spacer'></td><td>" + Container.ChartWrappers[2].Chart.ChartContainerHtmlString().ToString() + "</td></tr> ");

            builder.AppendLine(" </table> ");
            builder.AppendLine(" <table class='tbl-chrt-obd-readiness'> ");
            builder.AppendLine(" <tr><td><div class='div-chrt-center'>" + Container.ChartWrappers[3].Chart.ChartContainerHtmlString().ToString() + "</div></td></tr> ");
            builder.AppendLine(" <tr><td class='tbl-chrt-header'>Non-Continuous Monitors</td></tr> ");
            builder.AppendLine(" </table> ");
            builder.AppendLine(" <table class='tbl-chrt-obd-readiness'> ");
            builder.AppendLine(" <tr><td>" + Container.ChartWrappers[4].Chart.ChartContainerHtmlString().ToString() + "</td><td class='chrt-td-spacer'></td><td>" + Container.ChartWrappers[5].Chart.ChartContainerHtmlString().ToString() + "</td></tr> ");
            builder.AppendLine(" <tr><td>" + Container.ChartWrappers[6].Chart.ChartContainerHtmlString().ToString() + "</td><td></td><td>" + Container.ChartWrappers[7].Chart.ChartContainerHtmlString().ToString() + "</td></tr> ");
            builder.AppendLine(" <tr><td>" + Container.ChartWrappers[8].Chart.ChartContainerHtmlString().ToString() + "</td><td></td><td>" + Container.ChartWrappers[9].Chart.ChartContainerHtmlString().ToString() + "</td></tr> ");
            builder.AppendLine(" <tr><td>" + Container.ChartWrappers[10].Chart.ChartContainerHtmlString().ToString() + "</td><td></td><td>" + Container.ChartWrappers[11].Chart.ChartContainerHtmlString().ToString() + "</td></tr> ");
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
                string selectedSeries = ChartParams.SelectedSeries;
                string testDate = ChartParams.DrillDown.DrillLevels[0].DrillValue;
                string firstDrillLevelVal = ChartParams.DrillDown.DrillLevels[1].DrillValue;
                string secondDrillLevelVal = ChartParams.DrillDown.DrillLevels[2].DrillValue;

                paramList.Add(new OracleParameter("DateFrequency", OracleDbType.Varchar2, 8, dateFreq, ParameterDirection.Input)); // Year/Month/Day
                paramList.Add(new OracleParameter("StartDate", OracleDbType.Varchar2, 8, ChartParams.StartDateText, ParameterDirection.Input));
                paramList.Add(new OracleParameter("EndDate", OracleDbType.Varchar2, 8, ChartParams.EndDateText, ParameterDirection.Input));
                paramList.Add(new OracleParameter("DDSel", OracleDbType.Varchar2, 12, selectedPoint, ParameterDirection.Input)); // Ready, Not Ready, etc.
                paramList.Add(new OracleParameter("DDLvl0", OracleDbType.Varchar2, 25, firstDrillLevelVal, ParameterDirection.Input)); // 'make', 'model', 'model year' values.
                paramList.Add(new OracleParameter("DDLvl1", OracleDbType.Varchar2, 25, secondDrillLevelVal, ParameterDirection.Input)); // 'make', 'model', 'model year' values.
                paramList.Add(new OracleParameter("DataView", OracleDbType.Varchar2, 10, selectedSeries, ParameterDirection.Input)); // OVERALL, fuel system, misfire, etc
                paramList.Add(new OracleParameter("TableType", OracleDbType.Varchar2, 3, tableType, ParameterDirection.Input)); // KYD (maKe,Year,moDel) etc.
                paramList.Add(new OracleParameter("DDLevel", OracleDbType.Varchar2, 1, drillLevel, ParameterDirection.Input)); // 0,1,2?
                paramList.Add(new OracleParameter("TestDate", OracleDbType.Varchar2, 10, testDate, ParameterDirection.Input)); // 'YYYY', 'YYYY-MM', 'YYYY-MM-DD' values.
                paramList.Add(new OracleParameter("RECORD_DATA", OracleDbType.RefCursor, ParameterDirection.Output));
            }
            else
            {
                paramList.Add(new OracleParameter("StartDate", OracleDbType.Varchar2, 8, ChartParams.StartDateText, ParameterDirection.Input));
                paramList.Add(new OracleParameter("EndDate", OracleDbType.Varchar2, 8, ChartParams.EndDateText, ParameterDirection.Input));
                paramList.Add(new OracleParameter("RECORD_DATA", OracleDbType.RefCursor, ParameterDirection.Output));
            }

            return paramList.ToArray();
        }
    }
}