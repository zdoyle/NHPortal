using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NHPortal.Classes.Adhoc.WebControls
{
    /// <summary>Control used for storing a list of values on the adhoc builder.</summary>
    public class AdhocListBox : PortalFramework.WebControls.ReportListBox, IAdhocWebControl
    {
        /// <summary>Instantiates a new instance of the AdhocListBox class.</summary>
        public AdhocListBox()
        {

        }

        /// <summary>Clears the items in the AdhocListBox.</summary>
        public override void Initialize()
        {
            Items.Clear();
        }

        /// <summary>Gets whether or not the AdhocListBox has selected values.</summary>
        public bool HasInput
        {
            get
            {
                return (!AllSelected && SelectedItems.Count > 0);
            }
        }

        /// <summary>Gets a comma delimited list of the text of each item selected.</summary>
        public string Input
        {
            get 
            {
                return GetDelimitedText(", ");
            }
        }

        /// <summary>Sets the selected property of the item matching the value to true.</summary>
        /// <param name="inputValue">Input value to set to true.</param>
        public void SetInput(string inputValue)
        {
            // TODO: any way to pass in the delimiter ?
            // TODO: currently called from AdhocFavoriteHelper
            string[] values = inputValue.Split(new char[] { ';' });
            SetSelectedValues(values);
        }
    }
}