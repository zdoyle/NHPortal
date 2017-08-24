using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NHPortal.Classes.Adhoc
{
    /// <summary>Stores the adhoc sections read from the adhoc XML file.</summary>
    public static class AdhocSections
    {
        /// <summary>Parses the Adhoc.xml file and stores the information about each section.</summary>
        public static void Initialize()
        {
            string xmlFile = System.IO.Path.Combine(NHPortalUtilities.WebRoot, "Adhoc.xml");
            AdhocXMLParser parser = new AdhocXMLParser();
            m_sections = parser.Parse(xmlFile);
        }

        private static AdhocSection[] m_sections;
        /// <summary>Gets an array of sections read from the XML file.</summary>
        public static AdhocSection[] Sections
        {
            get
            {
                if (m_sections == null)
                {
                    Initialize();
                }
                return m_sections;
            }
        }
    }



    /// <summary>Represents a section tag in the adhoc XML file.</summary>
    public class AdhocSection
    {
        /// <summary>Instantiates a new instance of the AdhocSection class.</summary>
        /// <param name="name">Name of the section.</param>
        /// <param name="id">Id of the section.</param>
        public AdhocSection(string name, string id)
        {
            m_name = name;
            m_id = id;
            Fields = new List<AdhocField>();
        }

        private readonly string m_name;
        /// <summary>Gets the name of the section.</summary>
        public string Name
        {
            get { return m_name; }
        }

        private readonly string m_id;
        /// <summary>Gets the ID of the section.</summary>
        public string ID
        {
            get { return m_id; }
        }

        /// <summary>Gets a list of fields associated with the section.</summary>
        public List<AdhocField> Fields { get; private set; }
    }
}