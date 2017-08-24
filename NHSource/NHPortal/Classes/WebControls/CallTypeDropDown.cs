using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace NHPortal.Classes.WebControls
{
    /// <summary>ListBox control for counties.</summary>
    [System.Web.UI.ToolboxData("<{0}:CallTypeDropDown runat='server'></{0}:CallTypeDropDown>")]
    public class CallTypeDropDown : DropDownList
    {
        /// <summary>Instantiates a new instance of the CountyListBox class.</summary>
        public CallTypeDropDown()
        {
          
        }

        /// <summary>Initializes the items in the ListBox.</summary>
        public void Initialize()
        {
            this.Items.Clear();
            AddItem("All", String.Empty);
            AddItem("Public", "P");
            AddItem("Station", "S");

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