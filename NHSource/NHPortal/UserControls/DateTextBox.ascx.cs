using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NHPortal.UserControls
{
    /// <summary>User control used to link a TextBox control with an associated calendar control.</summary>
    public partial class DateTextBox : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>Gets the date value of the control in yyyyMMdd format.</summary>
        /// <returns>Date in the yyyyMMdd format, or an empty string if the Date is not valid.</returns>
        public string GetDateText()
        {
            return GetDateText("yyyyMMdd");
        }

        /// <summary>Gets the date value of the control in the provided format.</summary>
        /// <param name="format">Format to get the date in.</param>
        /// <returns>Date in the provided format, or an empty string if the Date is not valid.</returns>
        public string GetDateText(string format)
        {
            string dateValue = String.Empty;
            if (Date.HasValue)
            {
                dateValue = Date.Value.ToString(format);
            }
            return dateValue;
        }

        /// <summary>Gets a unique ID of the TextBox relative to the provided control.</summary>
        /// <param name="relativeControl">Relative control to derive the unique TextBox ID from.</param>
        /// <returns>Unique relative ID of the TextBox.</returns>
        public string GetRelativeTextBoxID(Control relativeControl)
        {
            return tbDate.GetUniqueIDRelativeTo(relativeControl);
        }

        /// <summary>Sets the Display property of the validators to None.</summary>
        /// <param name="display">Display property to assign to the validators.</param>
        public void SetValidatorVisibility(ValidatorDisplay display)
        {
            // TODO: this method is for hiding the validations on the date selector control
            // since the ValidationSummary control requires a postback
            // these validators are duplicated on the date selector control for now, so this hides the ones on the date text box control
            valDateFormat.Display = display;
            valDateRequired.Display = display;
        }




        /// <summary>Gets or sets the Text property of the TextBox control.</summary>
        public string Text
        {
            get { return tbDate.Text; }
            set { tbDate.Text = value; }
        }

        /// <summary>Gets the Text property of the TextBox control as a DateTime object.</summary>
        public DateTime? Date
        {
            get
            {
                DateTime? dt = null;
                string[] txtDate = Text.Split('/');

                if (txtDate.Length == 3) // Must have day, month, and year parts; otherwise not valid
                {
                    string dateToParse = txtDate[0].PadLeft(2, '0') + txtDate[1].PadLeft(2, '0') + txtDate[2].PadLeft(4, '0');
                    try
                    {
                        dt = DateTime.ParseExact(dateToParse, "MMddyyyy", System.Globalization.CultureInfo.InvariantCulture);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("Exception in DateTextBox.Date: " + Environment.NewLine + ex.ToString());
                    }
                }
                return dt;
            }
            //set { tbDate.Text = value.HasValue ? value.Value.ToString("MM/dd/yyyy") : String.Empty; }
        }

        /// <summary>Gets the Client ID of the TextBox control.</summary>
        public string TextBoxClientID
        {
            get { return tbDate.ClientID; }
        }
    }
}