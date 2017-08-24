using GDCoreUtilities;
using NHPortal.Classes;
using NHPortal.Classes.Charts;
using NHPortal.Classes.Reports.Triggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NHPortal
{
    public partial class TriggerWeightedScore : PortalPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                ProcessPostback();
            }
            else
            {
                InitPage();
                LoadFavorite();
            }

            RedirectOnInvalidPermission(Master.ReportData.BaseReport.UserPermission);
        }

        private void ProcessPostback()
        {
            SetReportData();

            if (Master.Master.Master.HidActionValue == "RUN_REPORT")
            {
                HandleWeightedValues();
            }
        }

        private void LoadFavorite()
        {
            NHPortal.Classes.User.UserFavorite fav = SessionHelper.GetSelectedFavorite(this.Session);
            if (fav != null)
            {
                foreach (var c in fav.Criteria)
                {
                    switch (c.Description.Trim().ToUpper())
                    {
                        case "OBDPDA":
                            this.txtProtocol.Text = c.Value;
                            break;
                        case "OBDRDA":
                            this.txtRejection.Text = c.Value;
                            break;
                        case "OBDIDA":
                            this.txtReadiness.Text = c.Value;
                            break;
                        case "EVINDA":
                            this.txtOBDVIN.Text = c.Value;
                            break;
                        case "TBTDA":
                            this.txtTBT.Text = c.Value;
                            break;
                        case "SDDA":
                            this.txtSafety.Text = c.Value;
                            break;
                        case "NOVDA":
                            this.txtNoVolt.Text = c.Value;
                            break;
                    }
                }
            }
        }

        private void HandleWeightedValues()
        {
            int total = 0;
            int protocol, rejection, readiness, evin, tbt, safety, novolt;

            protocol = NullSafe.ToInt(this.txtProtocol.Text);
            rejection = NullSafe.ToInt(this.txtRejection.Text);
            readiness = NullSafe.ToInt(this.txtReadiness.Text);
            evin = NullSafe.ToInt(this.txtOBDVIN.Text);
            tbt = NullSafe.ToInt(this.txtTBT.Text);
            safety = NullSafe.ToInt(this.txtSafety.Text);
            novolt = NullSafe.ToInt(this.txtNoVolt.Text);

            Master.ReportData.TriggerWeights = new Dictionary<string, int>();
            Master.ReportData.TriggerWeights.Add("OBDPDA", protocol);
            Master.ReportData.TriggerWeights.Add("OBDRDA", rejection);
            Master.ReportData.TriggerWeights.Add("OBDIDA", readiness);
            Master.ReportData.TriggerWeights.Add("SDDA", safety);
            Master.ReportData.TriggerWeights.Add("NOVDA", novolt);
            Master.ReportData.TriggerWeights.Add("EVINDA", evin);
            Master.ReportData.TriggerWeights.Add("TBTDA", tbt);
            
            total = (protocol + rejection + readiness + evin + tbt + safety + novolt);

            if(total != 100)
            {
                Master.ErrorMessage.Value = "The trigger weightings do not sum to 100.00% ";
            }
        }

        private void SetReportData()
        {
            Master.ReportData = HttpContext.Current.Session[NHChartMaster.CHART_REPORT_DATA] as BaseTrigger;

            if (Master.ReportData == null)
            {
                InitPage();
            }
        }

        private void InitPage()
        {
            Master.ReportData = new WeightedScore();
            Master.IsWeighted.Value = "true";
        }
    }
}