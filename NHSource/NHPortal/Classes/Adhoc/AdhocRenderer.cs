using NHPortal.Classes.Adhoc.WebControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace NHPortal.Classes.Adhoc
{
    /// <summary>Used for rendering out objects generated from an XML file to HTML.</summary>
    public class AdhocRenderer
    {
        /// <summary>Instantiates a new instance of the AdhocRenderer class.</summary>
        public AdhocRenderer()
        {
            m_htmlSections = new List<HtmlGenericControl>();
        }

        /// <summary>Renders an array of sections out to HTML.</summary>
        /// <param name="sections">Adhoc sections to render.</param>
        public void Render(AdhocSection[] sections)
        {
            RenderSections(sections);
        }

        private void RenderSections(AdhocSection[] sections)
        {
            foreach (var section in sections)
            {
                var htmlSection = RenderSection(section);
                if (section != sections.Last())
                {
                    //var ws = CreateWhitespaceRow();
                    //htmlSection.Controls.Add(ws);
                    htmlSection.Attributes["class"] += " adhoc-builder-table--spacer";
                }
                m_htmlSections.Add(htmlSection);
            }
        }

        private HtmlGenericControl RenderSection(AdhocSection section)
        {
            var table = new HtmlGenericControl("table");
            table.Attributes.Add("class", "adhoc-builder-table");

            table.Controls.Add(CreateTextRow(section.Name));
            foreach (var field in section.Fields)
            {
                table.Controls.Add(CreateFieldRow(field));
            }
            return table;
        }

        //private HtmlGenericControl CreateWhitespaceRow()
        //{
        //    var whitespaceRow = new HtmlGenericControl("tr");
        //    whitespaceRow.Attributes.Add("class", "adhoc-table-whitespace-row");
        //    whitespaceRow.Controls.Add(CreateSpannedTableCell());
        //    return whitespaceRow;
        //}

        private HtmlGenericControl CreateSectionLabel(AdhocSection section)
        {
            var label = new HtmlGenericControl("label");
            label.InnerText = section.Name;
            label.Attributes.Add("class", "adhoc-section-label");
            return label;
        }

        private AdhocFieldRow CreateFieldRow(AdhocField field)
        {
            var row = new AdhocFieldRow(field);

            //var row = new HtmlGenericControl("tr");
            row.Controls.Add(CreateTableCell(field.Name));
            //row.Controls.Add(CreateTableCellWithCheckbox(field.FieldID + "_include"));
            row.Controls.Add(CreateTableCellWithCheckbox(row.IncludeCheckBox));

            var inputCell = CreateTableCell(String.Empty);
            inputCell.Controls.Add(row.WebControl);
            row.Controls.Add(inputCell);

            row.Controls.Add(CreateAggregateCell(field, row.GetAggregateCheckBox(AdhocAggregateType.Count)));
            row.Controls.Add(CreateAggregateCell(field, row.GetAggregateCheckBox(AdhocAggregateType.Sum)));
            row.Controls.Add(CreateAggregateCell(field, row.GetAggregateCheckBox(AdhocAggregateType.Min)));
            row.Controls.Add(CreateAggregateCell(field, row.GetAggregateCheckBox(AdhocAggregateType.Max)));
            row.Controls.Add(CreateAggregateCell(field, row.GetAggregateCheckBox(AdhocAggregateType.Average)));

            //row.Controls.Add(CreateAggregateCell(field, AdhocAggregateType.Count));
            //row.Controls.Add(CreateAggregateCell(field, AdhocAggregateType.Sum));
            //row.Controls.Add(CreateAggregateCell(field, AdhocAggregateType.Min));
            //row.Controls.Add(CreateAggregateCell(field, AdhocAggregateType.Max));
            //row.Controls.Add(CreateAggregateCell(field, AdhocAggregateType.Average));
            return row;
        }

        private HtmlGenericControl CreateTextRow(string text)
        {
            var cell = CreateSpannedTableCell();
            cell.Text = text;
            //cell.InnerText = text;
            cell.Attributes.Add("class", "adhoc-section-row");

            var row = new HtmlGenericControl("tr");
            row.Controls.Add(cell);
            return row;
        }

        private TableCell CreateAggregateCell(AdhocField field, AggregateCheckBox aggChk)
        {
            TableCell cell;
            //if (field.HasAggregate(type))
            if (aggChk == null)
            {
                cell = CreateTableCell(String.Empty);
            }
            else
            {
                //string prefix = field.FieldID + "_" + type.ToString();
                cell = CreateTableCellWithCheckbox(aggChk);
            }
            return cell;
        }

        private TableCell CreateSpannedTableCell()
        {
            var cell = new TableCell();
            cell.ColumnSpan = 8;
            //cell.Attributes.Add("colspan", "8");
            return cell;
        }

        private TableCell CreateTableCell(string text)
        {
            var cell = new TableCell();
            cell.Text = text;
            //cell.InnerText = value;
            cell.Attributes.Add("class", "adhoc-builder-table__empty-cell");
            return cell;
        }

        private TableCell CreateTableCellWithCheckbox(CheckBox chk)
        {
            //CheckBox chk = new CheckBox();
            //chk.ID = "chk_" + prefix;

            var cell = new TableCell();
            cell.CssClass = "checkbox-cell";
            cell.Controls.Add(chk);
            return cell;
        }


        private List<HtmlGenericControl> m_htmlSections;
        /// <summary>Gets an array of HTML controls generated from the renderer.</summary>
        public HtmlGenericControl[] HtmlSections
        {
            get { return m_htmlSections.ToArray(); }
        }
    }
}