using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NHPortal.Classes.WebControls
{
    /// <summary>ListBox control for vehicle types.</summary>
    [System.Web.UI.ToolboxData("<{0}:StationTypeListBox runat='server'></{0}:StationTypeListBox>")]
    public class StationTypeListBox : PortalFramework.WebControls.ReportListBox
    {
        /// <summary>Instantiates a new instance of the VehicleTypeListBox class.</summary>
        public StationTypeListBox()
        {
            ColumnName = "stationtype";
            ValueType = PortalFramework.WebControls.ListBoxValueType.Alpha;
        }

        /// <summary>Initializes the items in the ListBox.</summary>
        public override void Initialize()
        {
            this.Items.Clear();
            AddItem("All", String.Empty);
            AddItem("Automobile", "A");
            AddItem("Motorcycle", "M");
            AddItem("Fleet", "F");
            AddItem("Municiple", "U");

            if (Items.Count > 0)
            {
                SelectedIndex = 0;
            }
        }
    }
}