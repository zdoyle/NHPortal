using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NHPortal.Classes.WebControls
{
    /// <summary>ListBox control for test sequences.</summary>
    [System.Web.UI.ToolboxData("<{0}:TestSequenceListBox runat='server'></{0}:TestSequenceListBox>")]
    public class TestSequenceListBox : PortalFramework.WebControls.ReportListBox
    {
        /// <summary>Instantiates a new instance of the TestSequenceListBox class.</summary>
        public TestSequenceListBox()
        {
            ColumnName = "testsequence";
            ValueType = PortalFramework.WebControls.ListBoxValueType.Alpha;
        }

        /// <summary>Initializes the items in the ListBox.</summary>
        public override void Initialize()
        {
            this.Items.Clear();
            AddItem("All", String.Empty);
            AddItem("Initial Test", "01");
            AddItem("First Retest", "02");
            AddItem("Other Retest", "03");

            if (Items.Count > 0)
            {
                SelectedIndex = 0;
            }
        }
    }
}