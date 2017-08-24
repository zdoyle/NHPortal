using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NHPortal.Classes.Adhoc
{
    /// <summary>Represents a field tag in the adhoc XML file.</summary>
    public abstract class AdhocField
    {
        /// <summary>Instantiates a new instance of the AdhocField class.</summary>
        /// <param name="section">The AdhocSection the field belongs to.</param>
        /// <param name="name">Name of the field.</param>
        /// <param name="fieldType">Type of the field.</param>
        protected AdhocField(AdhocSection section, string name, AdhocFieldType fieldType)
        {
            m_section = section;
            m_name = name;
            m_type = fieldType;
            Aggregates = new List<AdhocAggregate>();
        }

        /// <summary>Returns whether or not the field has the provided aggregate.</summary>
        /// <param name="aggType">TYpe of aggregate to check for.</param>
        /// <returns>True if the field has the aggregate, false otherwise.</returns>
        public bool HasAggregate(AdhocAggregateType aggType)
        {
            bool hasType = false;
            foreach (var agg in Aggregates)
            {
                if (agg.Type == aggType)
                {
                    hasType = true;
                    break;
                }
            }
            return hasType;
        }

        /// <summary>Returns an IAdhocWebControl representing the field's input.</summary>
        /// <returns>IAdhocWebControl for the field's input.</returns>
        public abstract NHPortal.Classes.Adhoc.WebControls.IAdhocWebControl ToIAdhocWebControl();


        private readonly AdhocSection m_section;
        /// <summary>Gets the AdhocSection the field belongs to. Read-only field.</summary>
        public AdhocSection Section
        {
            get { return m_section; }
        }

        private readonly string m_name;
        /// <summary>Gets the name of the field. Read-only field.</summary>
        public string Name
        {
            get { return m_name; }
        }

        private readonly AdhocFieldType m_type;
        /// <summary>Gets the type of the field. Read-only field.</summary>
        public AdhocFieldType Type
        {
            get { return m_type; }
        }

        /// <summary>Gets or sets the database column the field is for.</summary>
        public string DatabaseField { get; set; }

        /// <summary>Gets or sets the report column header display name that the field is for.</summary>
        public string ColumnHeaderName { get; set; }

        /// <summary>Gets or sets the type of the database field.</summary>
        public AdhocDatabaseFieldType DatabaseFieldType { get; set; }

        /// <summary>Gets or sets the ID for the field.</summary>
        public int ID { get; set; }

        /// <summary>Gets the unique section/field ID for the field.</summary>
        public string FieldID
        {
            get
            {
                string fieldId = String.Empty;
                if (Section != null)
                {
                    //fieldId = Section.ID + "_" + DatabaseField;
                    fieldId = Section.ID + "_" + ID;
                }
                else
                {
                    //fieldId = DatabaseField;
                    fieldId = ID.ToString();
                }
                return fieldId;
            }
        }

        /// <summary>Gets a list of aggregates associated with the field.</summary>
        public List<AdhocAggregate> Aggregates { get; private set; }
    }

    /// <summary>Specifies the field types available for an adhoc entry.</summary>
    public enum AdhocFieldType
    {
        /// <summary>Indicates the field has an unknown or missing type.</summary>
        Unknown,
        /// <summary>Indicates the field accepts text input.</summary>
        Text,
        /// <summary>Indicates the field displays a list of values.</summary>
        List,
    }

    public enum AdhocDatabaseFieldType
    {
        Varchar,
        Number,
        Date,
        Time,
    }
}