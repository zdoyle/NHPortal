using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NHPortal.Classes.WebControls
{
    /// <summary>ListBox control for model years.</summary>
    [System.Web.UI.ToolboxData("<{0}:ModelYearListBox runat='server'></{0}:ModelYearListBox>")]
    public class ModelYearListBox : PortalFramework.WebControls.ReportListBox
    {
        /// <summary>Instantiates a new instance of the ModelYearListBox class.</summary>
        public ModelYearListBox()
        {
            ColumnName = "testasmodelyr";
            ValueType = PortalFramework.WebControls.ListBoxValueType.Alpha;
        }

        /// <summary>Initializes the items in the ListBox.</summary>
        public override void Initialize()
        {
            this.Items.Clear();
            AddItem("All", String.Empty);

            int start = DateTime.Now.Year + 1;
            for (int i = start; i > 1900; i--)
            {
                AddItem(i.ToString(), i.ToString());
            }

            if (Items.Count > 0)
            {
                SelectedIndex = 0;
            }
        }
    }
}