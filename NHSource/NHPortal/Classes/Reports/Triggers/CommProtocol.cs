using GD.Highcharts.GDAnalytics;
using NHPortal.Classes.Charts;

namespace NHPortal.Classes.Reports.Triggers
{
    public class CommProtocol : BaseTrigger
    {
        public CommProtocol()
        {
            RunInitialReport = false;
            DetailedReportProcName = "NH_TRIG_DETAILED_REPORT";
            DrillProcName = "NH_TRIG_COMM_PROTOCOL_CHART"; // NOTE: Dev and PRd data are different.
            ChartProcName = DrillProcName;
            this.BaseReport = BaseReportMaster.CommProtocol;
        }
    }
}