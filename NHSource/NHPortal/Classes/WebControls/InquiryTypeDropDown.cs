using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace NHPortal.Classes.WebControls
{
    /// <summary>ListBox control for vehicle types.</summary>
    [System.Web.UI.ToolboxData("<{0}:InquiryTypeDropDown runat='server'></{0}:InquiryTypeDropDown>")]
    public class InquiryTypeDropDown : DropDownList
    {
        ///  <summary>Instantiates a new instance of the InquiryTypeDropDown class.</summary>
        public InquiryTypeDropDown()
        {
            //ColumnName = "inquirytype";
            //ValueType = PortalFramework.WebControls.ListBoxValueType.Alpha;
        }
        ///   <summary>Initializes the items in the ListBox.</summary>
        public void Initialize()
        {
            this.Items.Clear();
            //AddItem("All", String.Empty);
            AddItem("Officer ID", "O");
            AddItem("Station ID", "S");
            AddItem("Mechanic OLN", "R");

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
    }
}