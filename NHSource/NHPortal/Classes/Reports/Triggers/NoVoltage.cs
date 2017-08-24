using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NHPortal.Classes.Reports.Triggers
{
    public class NoVoltage : BaseTrigger
    {
        public NoVoltage()
        {
            RunInitialReport = false;
            DetailedReportProcName = "NH_TRIG_DETAILED_REPORT";
            DrillProcName = "NH_TRIG_NOVOLT_CHART";
            ChartProcName = DrillProcName;
            this.BaseReport = BaseReportMaster.NoVoltage;
        }
    }
}