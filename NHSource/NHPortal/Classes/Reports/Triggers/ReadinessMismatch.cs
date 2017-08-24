using GD.Highcharts.GDAnalytics;
using NHPortal.Classes.Charts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NHPortal.Classes.Reports.Triggers
{
    public class ReadinessMismatch : BaseTrigger
    {
        public ReadinessMismatch()
        {
            RunInitialReport = false;
            DetailedReportProcName = "NH_TRIG_READINESS_DATA";
            DrillProcName = "NH_TRIG_READINESS_CHART";
            ChartProcName = DrillProcName;
            this.BaseReport = BaseReportMaster.ReadinessMismatch;
        }
    }
}