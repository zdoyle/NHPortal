using NHPortal.Classes.Adhoc.WebControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NHPortal.Classes.Adhoc
{
    /// <summary>Represents a field tag of type "text" in the adhoc XML file.</summary>
    public class AdhocTextField : AdhocField
    {
        /// <summary>Instantiates a new instance of the AdhocTextField class.</summary>
        /// <param name="section">The AdhocSection the field belongs to.</param>
        /// <param name="name">Name of the field.</param>
        public AdhocTextField(AdhocSection section, string name)
            : base(section, name, AdhocFieldType.Text)
        {

        }

        /// <summary>Returns an AdhocTextBox Web Control.</summary>
        /// <returns>AdhocTextBox web control.</returns>
        public override IAdhocWebControl ToIAdhocWebControl()
        {
            AdhocTextBox tb = new AdhocTextBox();
            tb.ID = "tb_" + FieldID;
            tb.TextMode = TextBoxMode.MultiLine;
            tb.Attributes.Add("class", "adhoc-builder-txt");
            return tb;
        }
    }
}