using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NHPortal.Classes.WebControls
{
    /// <summary>ListBox control for counties.</summary>
    [System.Web.UI.ToolboxData("<{0}:CountyListBox runat='server'></{0}:CountyListBox>")]
    public class CountyListBox : PortalFramework.WebControls.ReportListBox
    {
        /// <summary>Instantiates a new instance of the CountyListBox class.</summary>
        public CountyListBox()
        {
            ColumnName = "county";
            ValueType = PortalFramework.WebControls.ListBoxValueType.Alpha;
        }

        /// <summary>Initializes the items in the ListBox.</summary>
        public override void Initialize()
        {
            this.Items.Clear();
            AddItem("All", String.Empty);
            
            foreach (County cnty in Counties.All)
            {
                AddItem(cnty.Name, cnty.Name.ToUpper());
            }

            if (Items.Count > 0)
            {
                SelectedIndex = 0;
            }
        }
    }
}