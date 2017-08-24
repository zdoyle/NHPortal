using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NHPortal.UserControls
{
    public partial class DatePeriodSelector : System.Web.UI.UserControl
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            if (IsPostBack)
            {

            }
            else
            {
                Initialize();
                cboRptPeriod.Attributes.Add("OnChange", "getValues(this);");
            }

            // TODO: how to prevent from doing this each page load
            cboRptPeriod.StartDateTargetClientID = dpStart.TextBoxClientID;
            cboRptPeriod.EndDateTargetClientID = dpEnd.TextBoxClientID;
            cboRptPeriod.RegisterJavaScript();
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        private void Initialize()
        {
            cboRptPeriod.Initialize();
            dpStart.Text = cboRptPeriod.GetStartDateText();
            dpEnd.Text = cboRptPeriod.GetEndDateText();
            
            

            startDateValidator.ControlToValidate = dpStart.GetRelativeTextBoxID(startDateValidator);
            startDateComparer.ControlToValidate = dpStart.GetRelativeTextBoxID(startDateComparer);
            startDateComparer.ControlToCompare = dpEnd.GetRelativeTextBoxID(startDateComparer);
            startDateRequiredField.ControlToValidate = dpStart.GetRelativeTextBoxID(startDateRequiredField);

            endDateValidator.ControlToValidate = dpEnd.GetRelativeTextBoxID(endDateValidator);
            endDateComparer.ControlToValidate = dpEnd.GetRelativeTextBoxID(endDateComparer);
            endDateComparer.ControlToCompare = dpStart.GetRelativeTextBoxID(endDateComparer);
            endDateRequiredField.ControlToValidate = dpEnd.GetRelativeTextBoxID(endDateRequiredField);

            dpStart.SetValidatorVisibility(ValidatorDisplay.None);
            dpEnd.SetValidatorVisibility(ValidatorDisplay.None);
        }

        /// <summary>Selects an item in the report period drop down list.</summary>
        /// <param name="index">Index to select.</param>
        public void SelectReportPeriod(int index)
        {
            if (index > -1 && index < cboRptPeriod.Items.Count)
            {
                cboRptPeriod.SelectedIndex = index;
                dpStart.Text = cboRptPeriod.GetStartDateText();
                dpEnd.Text = cboRptPeriod.GetEndDateText();
            }
        }



        /// <summary>Gets a reference to the DateTextBox control for the start date.</summary>
        public DateTextBox StartDateControl
        {
            get { return dpStart; }
        }

        /// <summary>Gets a reference to the DateTextBox control for the end date.</summary>
        public DateTextBox EndDateControl
        {
            get { return dpEnd; }
        }
    }
}