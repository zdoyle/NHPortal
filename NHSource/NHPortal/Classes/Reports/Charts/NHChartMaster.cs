using GD.Highcharts.Enums;
using GD.Highcharts.GDAnalytics;
using GD.Highcharts.Helpers;
using GD.Highcharts.Options;
using GDCoreUtilities;
using GDDatabaseClient.Oracle;
using PortalFramework.Database;
using System;
using System.Text;
using System.Web.UI.WebControls;

namespace NHPortal.Classes.Charts
{
    public static class NHChartMaster
    {
        public static string GetDateFrequency(DataGrouping dataGrouping)
        {
            string returnVal = String.Empty;
            switch (dataGrouping)
            {
                case DataGrouping.Yearly:
                    returnVal = "Year";
                    break;
                case DataGrouping.Monthly:
                    returnVal = "Month";
                    break;
                case DataGrouping.Daily:
                    returnVal = "Day";
                    break;
            }

            return returnVal;
        }

        public static string RenderHTMLControl(System.Web.UI.Control control)
        {
            string returnVal = string.Empty;

            using (System.IO.StringWriter sw = new System.IO.StringWriter())
            {
                control.RenderControl(new System.Web.UI.HtmlTextWriter(sw));

                returnVal = sw.ToString();
            }

            return returnVal;
        }

        internal static string GetDrillSeries(string pointName, string seriesName, string seriesId, string chartName)
        {
            string returnVal = String.Empty;

            if (seriesId == "MILStatus")
            {
                returnVal = OBDIIDTCErrorCodes.GetDrillSeries(pointName, seriesName, seriesId);
            }

            return returnVal;
        }

        public const string CHART_REPORT_DATA = "CHART_REPORT_DATA";
        public const string DrilldownFunction = "function (e) { "
                       + " var chart = this; "
                       + "  chart._drilldowns = chart._drilldowns || {}; "
                       + "  var series = chart._drilldowns[e.point.drilldown]; "
                       + "   var subtitle =  (e.point.series.options.id ? e.point.series.options.id + '>' : '' )  + e.point.name;"
                       + "  if (series) { "
                       + "      e.seriesOptions = series; "
                       + "      chart.addSeriesAsDrilldown(e.point, series); "
                       + "  } "
                       + "  if (!e.seriesOptions) { "
                       + "     var seriesid; if( e.point.series.options.id){seriesid =e.point.series.options.id.replace(\"&\",\"%26\");}else{seriesid=\"\";}"
                       + "      $.ajax({ url: \"ChartDataHandler.ashx?data=dd&pt=\" + e.point.name.replace(\"&\",\"%26\") + \"&ser=\" + chart.series[0].name + \"&sid=\" + seriesid + \"&cid=\" + chart.name, "
                       + "          cache: false, error: function(jqXHR, exception){ logErrorToServer(jqXHR, exception); }, type: \"GET\",dataType: \"json\", success: function (data) {"
                       + "          chart.addSeriesAsDrilldown(e.point, data); "
                       + "          chart.applyDrilldown(); "
                       + "        chart.setTitle(null, { text: subtitle }); "
                       + "      }}); "
                       + "  } "
                       + " }";
    }

    public enum NHDrillType
    {
        ModelYear,
        Make,
        Model,
        Station,
        Inspector,
    }
}