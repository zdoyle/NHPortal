using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NHPortal.Classes.WebControls
{
    /// <summary>ListBox control for counties.</summary>
    [System.Web.UI.ToolboxData("<{0}:WeightClassListBox runat='server'></{0}:WeightClassListBox>")]
    public class WeightClassListBox : PortalFramework.WebControls.ReportListBox
    {
        /// <summary>Instantiates a new instance of the CountyListBox class.</summary>
        public WeightClassListBox()
        {
            ColumnName = "weightclass";
            ValueType = PortalFramework.WebControls.ListBoxValueType.Alpha;
        }

        /// <summary>Initializes the items in the ListBox.</summary>
        public override void Initialize()
        {
            this.Items.Clear();
            AddItem("All", String.Empty);
            AddItem("Lights", "L");
            AddItem("Medium", "M");
            AddItem("Heavy", "H");

            if (Items.Count > 0)
            {
                SelectedIndex = 0;
            }
        }
    }
}