using GD.Highcharts.GDAnalytics;
using NHPortal.Classes;
using NHPortal.Classes.Charts;
using NHPortal.Classes.Reports.Charts;
using System;
using System.Web.UI;

namespace NHPortal
{
    public partial class Welcome : PortalPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            InitWelcomeCharts();
        }

        private void InitWelcomeCharts()
        {
            ChartContainer container;

            container = SessionHelper.GetWelcomeContainer(this.Session);

            if (container == null)
            {
                container = WelcomePage.LoadWelcomePageContainer();
                SessionHelper.SetWelcomeContainer(this.Session, container);
            }

            foreach (NHChartWrapper wrap in container.ChartWrappers)
            {
                string chartScript = wrap.Chart.ChartScriptHtmlString().ToString();
                ScriptManager.RegisterStartupScript(this, this.GetType(), wrap.ChartName, chartScript, false);
            }
        }

    }
}