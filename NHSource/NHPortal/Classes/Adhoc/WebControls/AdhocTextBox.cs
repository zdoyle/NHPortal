using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NHPortal.Classes.Adhoc.WebControls
{
    /// <summary>Control used for receiving text input on the adhoc builder.</summary>
    public class AdhocTextBox : System.Web.UI.WebControls.TextBox, IAdhocWebControl
    {
        /// <summary>Instantiates a new instance of the AdhocTextBox class.</summary>
        public AdhocTextBox()
        {

        }

        /// <summary>Gets whether or not the textbox has input.</summary>
        public bool HasInput
        {
            get
            {
                return !String.IsNullOrEmpty(Text.Trim());
            }
        }

        /// <summary>Gets the text entered into the textbox.</summary>
        public string Input
        {
            get
            {
                return Text;
            }
        }

        /// <summary>Sets the text of the control to the provided value.</summary>
        /// <param name="inputValue">Text to set.</param>
        public void SetInput(string inputValue)
        {
            Text = inputValue;
        }
    }
}