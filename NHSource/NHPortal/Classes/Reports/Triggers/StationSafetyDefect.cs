using GD.Highcharts.GDAnalytics;
using NHPortal.Classes.Charts;

namespace NHPortal.Classes.Reports.Triggers
{
    public class StationSafetyDefect : BaseTrigger
    {
        public StationSafetyDefect()
        {
            RunInitialReport = false;
            DetailedReportProcName = "NH_TRIG_DETAILED_REPORT";
            DrillProcName = "NH_TRIG_SAFETY_DEFECT_CHART";
            ChartProcName = DrillProcName;
            this.BaseReport = BaseReportMaster.SafetyDefect;
        }
    }
}