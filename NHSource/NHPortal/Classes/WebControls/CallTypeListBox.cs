using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NHPortal.Classes.WebControls
{
    /// <summary>ListBox control for counties.</summary>
    [System.Web.UI.ToolboxData("<{0}:CallTypeListBox runat='server'></{0}:CallTypeListBox>")]
    public class CallTypeListBox : PortalFramework.WebControls.ReportListBox
    {
        /// <summary>Instantiates a new instance of the CountyListBox class.</summary>
        public CallTypeListBox()
        {
            ColumnName = "calltype";
            ValueType = PortalFramework.WebControls.ListBoxValueType.Alpha;
        }

        /// <summary>Initializes the items in the ListBox.</summary>
        public override void Initialize()
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
    }
}