using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace NHPortal.Classes.Adhoc.WebControls
{
    /// <summary>Web control representing a single row in the adhoc builder.</summary>
    public class AdhocFieldRow : TableRow
    {
        /// <summary>Instantiates a new instance of the AdhocFieldRow class.</summary>
        /// <param name="field">Field the row is representing.</param>
        public AdhocFieldRow(AdhocField field)
        {
            m_field = field;
            Initialize();
        }

        private void Initialize()
        {
            IncludeCheckBox = new CheckBox();
            IncludeCheckBox.ID = m_field.FieldID + "_include";
            IncludeCheckBox.CssClass = "incl-checkbox";
            InputControl = m_field.ToIAdhocWebControl();

            AggregateCheckBoxes = new List<AggregateCheckBox>();
            foreach (var agg in m_field.Aggregates)
            {
                AggregateCheckBox aggChkBox = new AggregateCheckBox(agg);
                aggChkBox.ID = m_field.FieldID + "_" + agg.Type.ToString();
                aggChkBox.CssClass = "agg-checkbox";
                AggregateCheckBoxes.Add(aggChkBox);
            }
        }

        /// <summary>Gets an AggregateCheckBox control for the provided aggregate type.</summary>
        /// <param name="agg">Aggregate type to get the corresponding web control of.</param>
        /// <returns>AggregateCheckBox for the provided aggregate type, or null if no match found.</returns>
        public AggregateCheckBox GetAggregateCheckBox(AdhocAggregateType agg)
        {
            AggregateCheckBox chk = null;
            foreach (var aggChk in AggregateCheckBoxes)
            {
                if (aggChk.Aggregate.Type == agg)
                {
                    chk = aggChk;
                    break;
                }
            }
            return chk;
        }

        /// <summary>Returns a where clause for the input of the row.</summary>
        /// <returns>Where clause string.</returns>
        public string GetInputString()
        {
            string input = String.Empty;
            if (InputControl is AdhocTextBox)
            {
                AdhocTextBox tb = InputControl as AdhocTextBox;
                AdhocInterpreter interpreter = new AdhocInterpreter(Field);
                input = interpreter.Interpret(tb.Text);
            }

            if (InputControl is AdhocListBox)
            {
                AdhocListBox lst = InputControl as AdhocListBox;
                
                // Special case for Notes field. TODO: find a better location for this logic if necessary and if possible modify controls to handle Notes.
                if (lst.ColumnName == "NOTES")
                {
                    if (lst.SelectedValues.Count() == 1)
                    {
                        string equalityOperator = "=";
                        if (lst.SelectedValue == "WITH")
                        {
                            equalityOperator = "<>";
                        }
                        input = String.Format("NOTES {0} '{1}'", equalityOperator, "".PadLeft(100, ' '));
                    }
                    else
                    {
                         // Dummy where clause string in case 'With' and 'Without' are both selected and 'All' is not selected.
                        input = "1=1";
                    }
                }
                else
                {
                    input = lst.GetOracleText();
                }
            }
            return input;
        }



        private readonly AdhocField m_field;
        /// <summary>Gets a reference to the AdhocField for the row.  Read-only field.</summary>
        public AdhocField Field
        {
            get { return m_field; }
        }

        /// <summary>Gets the CheckBox web control for the include checkbox.</summary>
        public CheckBox IncludeCheckBox { get; private set; }

        /// <summary>Gets the adhoc input control used for the field's input.</summary>
        public IAdhocWebControl InputControl { get; private set; }

        /// <summary>Gets the web control used for the field's input.</summary>
        public System.Web.UI.Control WebControl
        {
            get { return InputControl as System.Web.UI.Control; }
        }

        /// <summary>Gets a list of AggregateCheckBox web controls for the field's aggregates.</summary>
        public List<AggregateCheckBox> AggregateCheckBoxes { get; private set; }

        /// <summary>Gets or sets whether or not the field is to be included in an adhoc query.</summary>
        public bool Included
        {
            get { return IncludeCheckBox.Checked; }
            set { IncludeCheckBox.Checked = value; }
        }

        /// <summary>Gets whether or not the row has criteria specified.</summary>
        public bool HasInput
        {
            get { return InputControl.HasInput; }
        }

        /// <summary>Gets the input from the row's adhoc control.</summary>
        public string Input
        {
            get { return InputControl.Input; }
        }
    }
}