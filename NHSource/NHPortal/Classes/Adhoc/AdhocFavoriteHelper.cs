using GDCoreUtilities.XML;
using NHPortal.Classes.Adhoc.WebControls;
using NHPortal.Classes.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace NHPortal.Classes.Adhoc
{
    /// <summary>Helps with saving and loading favorites to the adhoc builder too page.</summary>
    public class AdhocFavoriteHelper
    {
        private AdhocFieldRow[] m_rows;
        private AdhocFieldRow m_currentRow;

        private XmlWriter m_writer;
        private XmlReaderHelper m_helper;
        private string m_path;

        /// <summary>Instantiates a new instance of the AdhocFavoriteHelper class.</summary>
        /// <param name="adhocRows">Adhoc rows to use for saving or loading favorites.</param>
        public AdhocFavoriteHelper(AdhocFieldRow[] adhocRows)
        {
            m_path = System.IO.Path.Combine(NHPortalUtilities.WebRoot, "Favorites");
            m_rows = adhocRows;
        }

        /// <summary>Creates a new favorite record and an XML file for the user's selected input.</summary>
        /// <param name="usrSysNo">System number of the user to save the favorite for.</param>
        /// <param name="favDescription">Description of the favorite.</param>
        /// <returns>True if the favorite was successfully created, false otherwise.</returns>
        public bool CreateFavorite(long usrSysNo, string favDescription)
        {
            bool success = false;
            try
            {
                if (m_rows != null)
                {
                    string filename = String.Format("{0}_{1}.xml", usrSysNo, Guid.NewGuid().ToString().Replace("-", "_"));
                    string file = System.IO.Path.Combine(m_path, filename);
                    CreateXmlFile(file);

                    if (System.IO.File.Exists(file))
                    {
                        success = SaveFavoriteRecord(favDescription, filename);
                    }
                }
            }
            catch (System.IO.IOException ex)
            {
                System.Diagnostics.Debug.WriteLine("IOException in AdhocFavoriteHelper.CreateFavorite: " + ex.ToString());
            }
            return success;
        }

        /// <summary>Loads a user favorite record into the adhoc builder.</summary>
        /// <param name="fav">User favorite record to load.</param>
        public void LoadFavorite(UserFavorite fav)
        {
            if (fav != null && m_rows != null)
            {
                try
                {
                    UserFavoriteCriteria favCriteria = fav["XML FILE"];
                    if (favCriteria != null)
                    {
                        string file = System.IO.Path.Combine(m_path, favCriteria.Value);
                        using (m_helper = new XmlReaderHelper(file))
                        {
                            bool hasRoot = m_helper.FindRoot("UserFavorite");
                            if (hasRoot)
                            {
                                AllFieldsCheckBox.Checked = GDCoreUtilities.NullSafe.ToBoolean(m_helper.GetAttribute("allfields"));
                                ReadXml();
                            }
                        }
                    }
                }
                catch (XmlReaderHelperException ex)
                {
                    System.Diagnostics.Debug.WriteLine("XmlReaderHelperException: " + ex.ToString());
                }
            }
        }

        private bool SaveFavoriteRecord(string favDescription, string filename)
        {
            UserFavorite fav = new UserFavorite();
            fav.Title = AdhocBuilder.REPORT_TITLE;
            fav.Description = favDescription;
            fav.NavCode = NHPortal.UserControls.RedirectCodes.ADHOC_BUILDER;
            fav.FavType = UserFavoriteTypes.QueryBuilder;
            fav.AddCriteria("Xml File", "Xml File", filename);

            return fav.Save();
        }

        private void CreateXmlFile(string file)
        {
            if (!String.IsNullOrWhiteSpace(file))
            {
                using (m_writer = XmlWriter.Create(file))
                {
                    InitializeXML();
                    foreach (var row in m_rows)
                    {
                        WriteXMLElement(row);
                    }
                    FinalizeXML();
                }
            }
        }

        private void WriteXMLElement(AdhocFieldRow row)
        {
            m_writer.WriteStartElement("fav");
            m_writer.WriteAttributeString("fieldid", row.Field.FieldID);

            if (row.Included)
            {
                m_writer.WriteAttributeString("include", "true");
            }

            if (row.HasInput)
            {
                m_writer.WriteStartElement("favinput");
                if (row.InputControl is AdhocTextBox)
                {
                    m_writer.WriteString(row.Input);
                }

                if (row.InputControl is AdhocListBox)
                {
                    AdhocListBox lst = row.InputControl as AdhocListBox;
                    m_writer.WriteString(lst.GetDelimitedValues(";"));

                    //foreach (System.Web.UI.WebControls.ListItem item in lst.SelectedItems)
                    //{
                    //    m_writer.WriteElementString("favoption", item.Value);
                    //}
                }
                m_writer.WriteEndElement();
            }

            foreach (var aggChk in row.AggregateCheckBoxes)
            {
                if (aggChk.Checked)
                {
                    m_writer.WriteStartElement("favagg");
                    m_writer.WriteAttributeString("type", aggChk.Aggregate.Type.ToString());
                    m_writer.WriteEndElement();
                }
            }
            m_writer.WriteEndElement();
        }

        private void InitializeXML()
        {
            m_writer.WriteStartDocument();
            m_writer.WriteStartElement("UserFavorite");

            bool allFields = (AllFieldsCheckBox == null ? false : AllFieldsCheckBox.Checked);
            m_writer.WriteAttributeString("allfields", allFields.ToString());
        }

        private void FinalizeXML()
        {
            m_writer.WriteEndElement();
            m_writer.WriteEndDocument();
        }

        private void ReadXml()
        {
            string element;
            while (!String.IsNullOrEmpty(element = m_helper.GetNext()))
            {
                ProcessElement(element);
            }
        }

        private void ProcessElement(string elementName)
        {
            switch (elementName.Trim().ToUpper())
            {
                case "FAV":
                    FindFieldRow();
                    SetIncluded();
                    break;
                case "FAVINPUT":
                case "FAVOPTION":
                    SetInput(m_helper.GetInnerText());
                    break;
                case "FAVAGG":
                    SetAggregate();
                    break;
                default:
                    System.Diagnostics.Debug.WriteLine("Unknown element to process: " + elementName);
                    break;
            }
        }

        private void FindFieldRow()
        {
            string fieldId = m_helper.GetAttribute("fieldid");
            m_currentRow = m_rows.Where(r =>
                GDCoreUtilities.StringUtilities.AreEqual(fieldId, r.Field.FieldID)).FirstOrDefault();
        }

        private void SetIncluded()
        {
            string include = m_helper.GetAttribute("include");
            if (m_currentRow != null)
            {
                m_currentRow.Included = GDCoreUtilities.NullSafe.ToBoolean(include);
            }
        }

        private void SetInput(string input)
        {
            if (m_currentRow != null)
            {
                m_currentRow.InputControl.SetInput(input);
            }
        }

        private void SetAggregate()
        {
            if (m_currentRow != null)
            {
                AdhocAggregateType? aggType = GetAggregateType(m_helper.GetAttribute("type"));
                if (aggType.HasValue)
                {
                    var aggChk = m_currentRow.GetAggregateCheckBox(aggType.Value);
                    if (aggChk != null)
                    {
                        aggChk.Checked = true;
                    }
                }
            }
        }

        private AdhocAggregateType GetAggregateType(string type)
        {
            AdhocAggregateType? aggType = null;
            switch (type.Trim().ToUpper())
            {
                case "COUNT":
                    aggType = AdhocAggregateType.Count;
                    break;
                case "MIN":
                    aggType = AdhocAggregateType.Min;
                    break;
                case "MAX":
                    aggType = AdhocAggregateType.Max;
                    break;
                case "SUM":
                    aggType = AdhocAggregateType.Sum;
                    break;
                case "AVERAGE":
                case "AVG":
                    aggType = AdhocAggregateType.Average;
                    break;

            }
            return aggType.Value;
        }



        /// <summary>Gets or sets the reference to the "include all fields" checkbox.</summary>
        public System.Web.UI.WebControls.CheckBox AllFieldsCheckBox { get; set; }
    }
}