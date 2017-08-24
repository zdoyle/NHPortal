using GD.Highcharts.GDAnalytics;
using Oracle.DataAccess.Client;
using PortalFramework.ReportModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace NHPortal.Classes.Charts
{
    /// <summary>Defines basic functionality for a Chart page.</summary>
    public interface IChartReportData
    {
        /// <summary>Loads a chart container object and adds any required charts.</summary>
        void LoadContainer();
        /// <summary>Sets the series data for the charts in the container.</summary>
        void GetContainerSeries();
        /// <summary>Loads the main report object for a chart page.</summary>
        void LoadReport();
        /// <summary>Loads an auxiliarry report based on a previous selection.</summary>
        void LoadDetailedReport();
        /// <summary>Sets chart specific meta data for things like saving a favorite if no report is present.</summary>
        void SetMetaData();
        /// <summary>Returns html formatted string containing a drill down table.</summary>
        string LoadDrillLevel(string testDateVal, string drillValue, string prevDrillValue, int drillLevel);
        /// <summaryReturns a html formatted string containing the chart container and its surrounding table and div elements.</summary>
        string RenderContainer();
        /// <summary>Whether or not a report should be ran when the page first loads.</summary>
        bool RunInitialReport { get; set; }
        /// <summary>Collection of oracle parameters used for calling a stored procedure.</summary>
        OracleParameter[] GetOracleParams(bool isReport);
        /// <summary>Static data about a report such as column definitions and permissions.</summary>
        BaseReport BaseReport { get; set; }
        /// <summary>The report base object for the chart page.</summary>
        Report Report { get; set; }
        /// <summary>Container for charts that appear on a page.</summary>
        ChartContainer Container { get; set; }
        /// <summary>Parameters that define a chart.</summary>
        ChartParams ChartParams { get; set; }
        /// <summary>Collection of meta data about a chart.</summary>
        ChartMetaData[] ChartMetaData { get; }
    }
}
