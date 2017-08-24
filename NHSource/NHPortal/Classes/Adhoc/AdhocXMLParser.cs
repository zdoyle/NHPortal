using GDCoreUtilities;
using GDCoreUtilities.XML;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace NHPortal.Classes.Adhoc
{
    /// <summary>Parses an xml file defining the structure of adhoc fields to use for the adhoc builder.</summary>
    public class AdhocXMLParser
    {
        private const string ROOT_ELEMENT = "sections";
        private XmlReaderHelper m_helper;
        private List<AdhocSection> m_sections;

        /// <summary>Instantiates a new instance of the AdhocParser class.</summary>
        public AdhocXMLParser()
        {
            m_sections = new List<AdhocSection>();
        }

        /// <summary>Parses the adhoc xml file and creates objects representing the structure of the file.</summary>
        /// <param name="fullPath">Full path of the file to parse.</param>
        /// <returns>Array of sections parsed from the file.</returns>
        public AdhocSection[] Parse(string fullPath)
        {
            try
            {
                using (m_helper = new XmlReaderHelper(fullPath))
                {
                    bool hasRoot = m_helper.FindRoot(ROOT_ELEMENT);
                    if (hasRoot)
                    {
                        ProcessFile();
                    }
                }
            }
            catch (XmlReaderHelperException ex)
            {
                System.Diagnostics.Debug.WriteLine("XmlReaderHelperException: " + ex.ToString());
            }
            return m_sections.ToArray();
        }

        private void ProcessFile()
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
                case "SECTION":
                    CreateSection();
                    break;
                case "FIELD":
                    AdhocField f = CreateField();
                    AddFieldToSection(f);
                    break;
                case "AGGREGATE":
                    AdhocAggregate agg = CreateAggregate();
                    AddAggregateToField(agg);
                    break;
                case "OPTION":
                    AdhocListOption option = CreateListOption();
                    AddOptionToField(option);
                    break;
                case "OPTIONRANGE":
                    AdhocListOption[] options = CreateListOptionRange();
                    foreach (var o in options)
                    {
                        AddOptionToField(o);
                    }
                    break;
                case "OPTIONREF":
                    AddReferenceOptions();
                    break;
                case "RESULT_OPTIONS":
                    AddResultsOptions();
                    break;
                case "READY_OPTIONS":
                    AddReadyOptions();
                    break;
                default:
                    System.Diagnostics.Debug.WriteLine("Unknown element to process: " + elementName);
                    break;
            }
        }

        private AdhocSection CreateSection()
        {
            AdhocSection section = null;
            string name = m_helper.GetAttribute("name");
            string id = m_helper.GetAttribute("id");

            if (!String.IsNullOrEmpty(name) && !String.IsNullOrEmpty(id))
            {
                section = new AdhocSection(name, id);
                m_sections.Add(section);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Invalid section -- missing name or id field");
            }
            return section;
        }

        private AdhocField CreateField()
        {
            string name = m_helper.GetAttribute("name");
            string type = m_helper.GetAttribute("type");
            string dbField = m_helper.GetAttribute("dbfield");
            string dbType = m_helper.GetAttribute("dbtype");
            string headerName = m_helper.GetAttribute("headername");

            AdhocField field = null;
            if (!String.IsNullOrEmpty(name) && !String.IsNullOrEmpty(type))
            {
                switch (type.Trim().ToUpper())
                {
                    case "TEXT":
                        field = new AdhocTextField(CurrentSection, name);
                        break;
                    case "LIST":
                        field = new AdhocListField(CurrentSection, name);
                        break;
                    default:
                        System.Diagnostics.Debug.WriteLine("Could not create AdhocField.  Invalid type: " + type);
                        break;
                }

                if (field != null)
                {
                    field.DatabaseField = dbField;
                    field.DatabaseFieldType = GetDBFieldType(dbType);
                    field.ID = CurrentSection.Fields.Count + 1;
                    field.ColumnHeaderName = headerName;
                }
            }
            return field;
        }

        private void AddFieldToSection(AdhocField field)
        {
            if (field != null && CurrentSection != null)
            {
                CurrentSection.Fields.Add(field);
            }
        }

        private AdhocDatabaseFieldType GetDBFieldType(string type)
        {
            AdhocDatabaseFieldType dbFieldType = AdhocDatabaseFieldType.Varchar;
            switch (type.Trim().ToUpper())
            {
                case "VARCHAR":
                case "VARCHAR2":
                case "CHAR":
                case "":
                    dbFieldType = AdhocDatabaseFieldType.Varchar;
                    break;
                case "NUMBER":
                    dbFieldType = AdhocDatabaseFieldType.Number;
                    break;
                case "DATE":
                    dbFieldType = AdhocDatabaseFieldType.Date;
                    break;
                case "TIME":
                    dbFieldType = AdhocDatabaseFieldType.Time;
                    break;
                default:
                    System.Diagnostics.Debug.WriteLine("Invalid dbtype value of " + type);
                    break;
            }
            return dbFieldType;
        }

        private AdhocAggregate CreateAggregate()
        {
            string type = m_helper.GetAttribute("type");
            AdhocAggregateType? aggType = GetAggregateType(type);

            AdhocAggregate agg = null;
            if (aggType.HasValue)
            {
                agg = new AdhocAggregate(aggType.Value);
            }
            return agg;
        }

        private void AddAggregateToField(AdhocAggregate agg)
        {
            if (agg != null && CurrentField != null)
            {
                CurrentField.Aggregates.Add(agg);
            }
        }

        private AdhocListOption CreateListOption()
        {
            string txt = m_helper.GetAttribute("text");
            string val = m_helper.GetAttribute("value");

            AdhocListOption option = null;
            if (!String.IsNullOrEmpty(txt) && !String.IsNullOrEmpty(val))
            {
                option = new AdhocListOption(txt, val);
            }
            return option;
        }

        private AdhocListOption[] CreateListOptionRange()
        {
            int min = NullSafe.ToInt(m_helper.GetAttribute("min"));
            int max = NullSafe.ToInt(m_helper.GetAttribute("max"));
            string order = m_helper.GetAttribute("orderby");

            List<AdhocListOption> options = new List<AdhocListOption>();
            if (min < max)
            {
                if (StringUtilities.AreEqual(order, "DESC"))
                {
                    for (int i = max; i >= min; i--)
                    {
                        options.Add(new AdhocListOption(i.ToString(), i.ToString()));
                    }
                }
                else
                {
                    for (int i = min; i <= max; i++)
                    {
                        options.Add(new AdhocListOption(i.ToString(), i.ToString()));
                    }
                }
            }
            return options.ToArray();
        }

        private void AddResultsOptions()
        {
            List<AdhocListOption> options = new List<AdhocListOption>();
            // List<AdhocListOption> options = new List<AdhocListOption>();
            // options.Add(new AdhocListOption("Pass", "P"));
            // options.Add(new AdhocListOption("Reject", "R"));
            // options.Add(new AdhocListOption("Corrected", "C"));
            // options.Add(new AdhocListOption("N/A", "N"));
            options.Add(new AdhocListOption("Pass", "1"));
            options.Add(new AdhocListOption("Reject", "2"));
            options.Add(new AdhocListOption("Corrected", "3"));
            options.Add(new AdhocListOption("N/A", "4"));

            foreach (var o in options)
            {
                AddOptionToField(o);
            }
        }

        private void AddReadyOptions()
        {
            List<AdhocListOption> options = new List<AdhocListOption>();
            options.Add(new AdhocListOption("Ready", "1"));
            options.Add(new AdhocListOption("Not Ready", "2"));
            options.Add(new AdhocListOption("Not Supported", "4"));

            foreach (var o in options)
            {
                AddOptionToField(o);
            }
        }

        private void AddReferenceOptions()
        {
            string refValues = m_helper.GetAttribute("reference").Trim().ToUpper();
            switch (refValues)
            {
                case "COUNTIES":
                    foreach (var c in Counties.All)
                    {
                        AddOptionToField(new AdhocListOption(c.Name, c.Name));
                    }
                    break;
            }
        }

        private void AddOptionToField(AdhocListOption option)
        {
            if (option != null && CurrentField != null)
            {
                AdhocListField listField = CurrentField as AdhocListField;
                if (listField != null)
                {
                    listField.Options.Add(option);
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

        /// <summary>Gets a reference to the last section read by the parser.</summary>
        public AdhocSection CurrentSection
        {
            get
            {
                AdhocSection current = null;
                if (m_sections != null && m_sections.Count > 0)
                {
                    current = m_sections.Last();
                }
                return current;
            }
        }

        /// <summary>Gets a reference to the last field on the last section read by the parser.</summary>
        public AdhocField CurrentField
        {
            get
            {
                AdhocField currentField = null;
                if (CurrentSection != null && CurrentSection.Fields.Count > 0)
                {
                    currentField = CurrentSection.Fields.Last();
                }
                return currentField;
            }
        }
    }
}