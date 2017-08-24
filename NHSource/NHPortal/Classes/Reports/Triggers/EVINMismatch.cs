using GD.Highcharts.GDAnalytics;
using NHPortal.Classes.Charts;

namespace NHPortal.Classes.Reports.Triggers
{
    public class EVINMismatch : BaseTrigger
    {
        public EVINMismatch()
        {
            RunInitialReport = false;
            DetailedReportProcName = "NH_TRIG_EVIN_MISMATCH_DATA";
            DrillProcName = "NH_TRIG_EVIN_MISMATCH_CHART";
            ChartProcName = DrillProcName;
            this.BaseReport = BaseReportMaster.EVINMismatch;
        }
    }
}