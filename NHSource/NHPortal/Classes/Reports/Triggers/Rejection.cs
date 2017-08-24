using GD.Highcharts.GDAnalytics;
using NHPortal.Classes.Charts;

namespace NHPortal.Classes.Reports.Triggers
{
    public class Rejection : BaseTrigger
    {
        public Rejection()
        {
            RunInitialReport = false;
            DetailedReportProcName = "NH_TRIG_DETAILED_REPORT";
            DrillProcName = "NH_TRIG_REJECTION_CHART";
            ChartProcName = DrillProcName;
            this.BaseReport = BaseReportMaster.Rejection;
        }
    }
}