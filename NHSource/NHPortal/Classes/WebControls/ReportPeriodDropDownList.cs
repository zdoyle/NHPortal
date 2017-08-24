using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;

namespace NHPortal.Classes.WebControls
{
    /// <summary>DropDownList control for report periods.</summary>
    [System.Web.UI.ToolboxData("<{0}:ReportPeriodDropDownList runat='server'></{0}:ReportPeriodDropDownList>")]
    public class ReportPeriodDropDownList : DropDownList
    {
        public ReportPeriodDropDownList()
        {

        }

        //// <summary>Initializes the items in the DropDownList.</summary>
        public void Initialize()
        {
            this.Items.Clear();

            // Add Current Year - All
            AddItem(DateTime.Now.Year.ToString() + " - All",
                    DateTime.Now.Year.ToString() + "0101," + DateTime.Now.ToString("yyyyMMdd"));

            // Add Current Month
            AddItem(DateTime.Now.ToString("MMM yyyy"),
                    DateTime.Now.ToString("yyyyMM") + "01," + DateTime.Now.ToString("yyyyMM") + DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month).ToString("D2"));

            // Add last week
            string day;
            for (int i = -1; i > -8; i--)
            {
                day = DateTime.Now.AddDays(i).ToString("yyyyMMdd");
                AddItem(DateTime.Now.AddDays(i).ToString("MMM dd, yyyy"),
                        day + "," + day);
            }

            AddSeparator();

            // Add rest of months for the year
            for (int i = -1; i > DateTime.Now.Month * -1; i--)
            {
                AddItem(DateTime.Now.AddMonths(i).ToString("MMM yyyy"),
                DateTime.Now.AddMonths(i).ToString("yyyyMM") + "01," +
                DateTime.Now.AddMonths(i).ToString("yyyyMM") +
                DateTime.DaysInMonth(DateTime.Now.AddMonths(i).Year,
                DateTime.Now.AddMonths(i).Month).ToString("D2"));
            }

            AddSeparator();
            AddFullYear(DateTime.Now.AddYears(-1).Year); // Add prior year
            AddSeparator();
            AddFullYear(DateTime.Now.AddYears(-2).Year); // Add prior-prior year
            AddSeparator();
            AddRemainingDays();
            AddSeparator();

            if (Items.Count > 0)
            {
                SelectedIndex = 0;
            }
        }

        private void AddItem(string txt, string val)
        {
            ListItem item = new ListItem(txt, val);
            this.Items.Add(item);
        }

        private void AddSeparator()
        {
            ListItem item = new ListItem("-------------------", "-");
            item.Attributes.Add("disabled", "true");
            this.Items.Add(item);
        }

        private void AddFullYear(int year)
        {
            // Add the Year - All entry
            AddItem(year + " - All",
                    year + "0101," + year + "1231");

            // Add an entry for each month
            for (int i = 12; i > 0; i--)
            {
                AddItem(DateTime.Parse(i.ToString("D2") + "/01/" + year).ToString("MMM yyyy"),
                                year + i.ToString("D2") + "01," +
                                year + i.ToString("D2") + DateTime.DaysInMonth(year, i));
            }
        }

        private void AddRemainingDays()
        {
            for (int i = -8; i > -72; i--)
            {
                AddItem(DateTime.Now.AddDays(i).ToString("MMM dd, yyyy"),
                    DateTime.Now.AddDays(i).ToString("yyyyMMdd") + "," +
                    DateTime.Now.AddDays(i).ToString("yyyyMMdd"));
            }
        }

        /// <summary>Registers the JavaScript function for parsing the start and end dates and setting the target TextBoxes.</summary>
        public void RegisterJavaScript()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("function getValues(cbo) {");
            sb.AppendLine("    var vals = parseValues(cbo);");
            sb.AppendLine("    if (vals.length > 1) {");
            sb.AppendFormat("        document.getElementById('{0}').value = toDisplay(vals[0]);" + Environment.NewLine, StartDateTargetClientID);
            sb.AppendFormat("        document.getElementById('{0}').value = toDisplay(vals[1]);" + Environment.NewLine, EndDateTargetClientID);
            sb.AppendLine("    }");
            sb.AppendLine("}");

            if (this.Page != null)
            {
                this.Page.ClientScript.RegisterStartupScript(this.GetType(), "rpt-cbo-" + this.ClientID, sb.ToString(), true);
            }
        }

        private string ParseSelectedValue(int startIndex, int length)
        {
            string parsed = GDCoreUtilities.StringUtilities.GetSubstring(SelectedValue, startIndex, length);
            return parsed;

            //string parsedTxt = String.Empty;
            //if (!String.IsNullOrEmpty(SelectedValue) && SelectedValue.Length > startIndex &&
            //    (startIndex + length) <= SelectedValue.Length)
            //{
            //    parsedTxt = SelectedValue.Substring(startIndex, length);
            //}
            //return parsedTxt;
        }

        /// <summary>Gets the value of the start date as a string in M/d/yyyy format.</summary>
        /// <returns>The start date in M/d/yyyy format, or an empty string if the start date is invalid.</returns>
        public string GetStartDateText()
        {
            return GetStartDateText("M/d/yyyy");
        }

        /// <summary>Gets the value of the start date as a string in the provided format.</summary>
        /// <param name="format">Format to convert the start date value to.</param>
        /// <returns>The start date in the provided format, or an empty string if the start date is invalid.</returns>
        public string GetStartDateText(string format)
        {
            string dateText = String.Empty;
            if (StartDate.HasValue)
            {
                dateText = StartDate.Value.ToString(format);
            }
            return dateText;
        }

        /// <summary>Gets the value of the end date as a string in M/d/yyyy format.</summary>
        /// <returns>The end date in M/d/yyyy format, or an empty string if the end date is invalid.</returns>
        public string GetEndDateText()
        {
            return GetEndDateText("M/d/yyyy");
        }

        /// <summary>Gets the value of the end date as a string in the provided format.</summary>
        /// <param name="format">Format to convert the end date value to.</param>
        /// <returns>The end date in the provided format, or an empty string if the end date is invalid.</returns>
        public string GetEndDateText(string format)
        {
            string dateText = String.Empty;
            if (EndDate.HasValue)
            {
                dateText = EndDate.Value.ToString(format);
            }
            return dateText;
        }




        /// <summary>Gets or sets the target client ID of the start date textbox.</summary>
        public string StartDateTargetClientID { get; set; }

        /// <summary>Gets or sets the target client ID of the end date textbox.</summary>
        public string EndDateTargetClientID { get; set; }

        /// <summary>Gets the parsed start date from the DropDownList's SelectedValue as a Nullable DateTime.</summary>
        public DateTime? StartDate
        {
            get { return GDCoreUtilities.NullSafe.ToNullableDate(StartDateText, "yyyyMMdd"); }
        }

        /// <summary>Gets the parsed end date from the DropDownList's SelectedValue as a Nullable DateTime.</summary>
        public DateTime? EndDate
        {
            get { return GDCoreUtilities.NullSafe.ToNullableDate(EndDateText, "yyyyMMdd"); }
        }

        /// <summary>Gets the parsed start date from the SelectedValue as a string.</summary>
        public string StartDateText
        {
            get
            {
                return ParseSelectedValue(0, 8);
            }
        }

        /// <summary>Gets the parsed end date from the SelectedValue as a string.</summary>
        public string EndDateText
        {
            get
            {
                return ParseSelectedValue(9, 8);
            }
        }
    }
}