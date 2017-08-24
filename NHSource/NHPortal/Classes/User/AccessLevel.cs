using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NHPortal.Classes.User
{
    /// <summary>Stores all access levels in the system.</summary>
    public static class AccessLevels
    {
        /// <summary>Initializes the access levels in the system.</summary>
        public static void Initialize()
        {
            List<AccessLevel> accessLevels = new List<AccessLevel>();
            accessLevels.Add(new AccessLevel("No Access", "N"));
            accessLevels.Add(new AccessLevel("Read-Only", "R"));
            accessLevels.Add(new AccessLevel("Full Access", "F"));

            m_all = accessLevels.ToArray();
        }

        /// <summary>Returns an access level matching the provided value.</summary>
        /// <param name="value">Value of the access level to find.</param>
        /// <returns>Access level matching the value, or null if no match found.</returns>
        public static AccessLevel Find(string value)
        {
            AccessLevel accessLevel = null;
            foreach (var a in All)
            {
                if (GDCoreUtilities.StringUtilities.AreEqual(a.Value, value))
                {
                    accessLevel = a;
                    break;
                }
            }
            return accessLevel;
        }


        private static AccessLevel[] m_all;
        /// <summary>Static array of all access levels.</summary>
        public static AccessLevel[] All
        {
            get
            {
                if (m_all == null)
                {
                    Initialize();
                }
                return m_all;
            }
        }

        /// <summary>Gets a reference to the access level representing "No Access".</summary>
        public static AccessLevel None { get { return Find("N"); } }

        /// <summary>Gets a reference to the access level representing "Read Only Access".</summary>
        public static AccessLevel ReadOnly { get { return Find("R"); } }

        /// <summary>Gets a reference to the access level representing "Full Access".</summary>
        public static AccessLevel Full { get { return Find("F"); } }
    }



    /// <summary>Represents an access level for a user.</summary>
    public class AccessLevel
    {
        /// <summary>Instantiates a new instance of the AccessLevel class.</summary>
        /// <param name="description">Description of the access level.</param>
        /// <param name="value">Value of the access level.</param>
        public AccessLevel(string description, string value)
        {
            m_description = description;
            m_value = value;
        }

        private readonly string m_description;
        /// <summary>Gets the description of the access level.  Read-only field.</summary>
        public string Description
        {
            get { return m_description; }
        }

        private readonly string m_value;
        /// <summary>Gets the value of the access level.  Read-only field.</summary>
        public string Value
        {
            get { return m_value; }
        }
    }
}