using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NHPortal.Classes.WebControls
{
    /// <summary>ListBox control for vehicle types.</summary>
    [System.Web.UI.ToolboxData("<{0}:VehicleTypeListBox runat='server'></{0}:VehicleTypeListBox>")]
    public class VehicleTypeListBox : PortalFramework.WebControls.ReportListBox
    {
        /// <summary>Instantiates a new instance of the VehicleTypeListBox class.</summary>
        public VehicleTypeListBox()
        {
            ColumnName = "safetytesttype";
            ValueType = PortalFramework.WebControls.ListBoxValueType.Alpha;
        }

        /// <summary>Initializes the items in the ListBox.</summary>
        public override void Initialize()
        {
            this.Items.Clear();
            AddItem("All", String.Empty);
            AddItem("Basic", "B");
            AddItem("Truck Or Bus", "T");
            AddItem("Trailer", "R");
            AddItem("Agriculture", "A");
            AddItem("Motorcycle", "M");

            if (Items.Count > 0)
            {
                SelectedIndex = 0;
            }
        }
    }
}