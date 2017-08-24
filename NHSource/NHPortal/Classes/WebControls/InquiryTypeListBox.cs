using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace NHPortal.Classes.WebControls
{
    /// <summary>ListBox control for vehicle types.</summary>
    [System.Web.UI.ToolboxData("<{0}:InquiryTypeListBox runat='server'></{0}:InquiryTypeListBox>")]
    public class InquiryTypeListBox : PortalFramework.WebControls.ReportListBox
    {
        ///  <summary>Instantiates a new instance of the InquiryTypeListBox class.</summary>
        public InquiryTypeListBox()
        {
            ColumnName = "inquirytype";
            ValueType = PortalFramework.WebControls.ListBoxValueType.Alpha;
        }
        ///   <summary>Initializes the items in the ListBox.</summary>
        public override void Initialize()
        {
            this.Items.Clear();
            //AddItem("All", String.Empty);
            AddItem("Office ID", "O");
            AddItem("Station ID", "S");
            AddItem("Mechanic OLN", "R");          

            if (Items.Count > 0)
            {
                SelectedIndex = 0;
            }
        }
    }
}