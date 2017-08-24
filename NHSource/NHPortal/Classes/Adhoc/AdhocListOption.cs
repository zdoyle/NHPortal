using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NHPortal.Classes.Adhoc
{
    /// <summary>Represents a field tag in the adhoc XML file.</summary>
    public class AdhocListOption
    {
        public AdhocListOption(string name, string value)
        {
            m_name = name;
            m_value = value;
        }

        private readonly string m_name;
        public string Name
        {
            get { return m_name; }
        }

        private readonly string m_value;
        public string Value
        {
            get { return m_value; }
        }
    }
}