using NHPortal.Classes.Adhoc.WebControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace NHPortal.Classes.Adhoc
{
    /// <summary>Represents a field tag of type "list" in the adhoc XML file.</summary>
    public class AdhocListField : AdhocField
    {
        /// <summary>Instantiates a new instance of the AdhocListField class.</summary>
        /// <param name="section">The AdhocSection the field belongs to.</param>
        /// <param name="name">Name of the field.</param>
        public AdhocListField(AdhocSection section, string name)
            : base(section, name, AdhocFieldType.List)
        {
            Options = new List<AdhocListOption>();
        }

        /// <summary>Returns an AdhocListbox Web Control.</summary>
        /// <returns>AdhocListbox control.</returns>
        public override IAdhocWebControl ToIAdhocWebControl()
        {
            AdhocListBox lst = new AdhocListBox();
            lst.ID = "cbo_" + FieldID;
            lst.CssClass = "adhoc-builder-listbox";
            lst.SelectionMode = ListSelectionMode.Multiple;
            lst.ColumnName = DatabaseField;
            lst.ValueType = PortalFramework.WebControls.ListBoxValueType.Alpha; // TODO: add and read this from the XML

            lst.Items.Add(new ListItem("Any", ""));
            foreach (var option in Options)
            {
                lst.Items.Add(new ListItem(option.Name, option.Value));
            }
            lst.SelectedIndex = 0;

            return lst;
        }



        /// <summary>Gets a list of options available for the field.</summary>
        public List<AdhocListOption> Options { get; private set; }
    }
}