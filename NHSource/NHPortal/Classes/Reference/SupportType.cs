using GDCoreUtilities;
using GDDatabaseClient.Oracle;
using PortalFramework.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace NHPortal.Classes.Reference
{
    /// <summary>Used for storing and accessing values stored in the r_suppType table.</summary>
    public static class SupportTypes
    {
        /// <summary>Gets the emission types from the database to store in memory.</summary>
        public static void Initialize()
        {
            string qry = "SELECT ret.CODE_VALUE, ret.DESCRIPTION" + Environment.NewLine
                       + "FROM   r_support_type ret";

            List<SupportType> suppTypes = new List<SupportType>();
            OracleResponse resp = ODAP.GetDataTable(qry, DatabaseTarget.Adhoc);
            if (resp.Successful)
            {
                foreach (DataRow dr in resp.ResultsTable.Rows)
                {
                    suppTypes.Add(new SupportType(dr));
                }
            }
            m_all = suppTypes.ToArray();
        }

        /// <summary>Returns an emission type by value.</summary>
        /// <param name="value">Value of the emission type to find.</param>
        /// <returns>Emission type matching the value, or null if no match found.</returns>
        public static SupportType Find(char value)
        {
            SupportType suppType = null;
            foreach (var eType in All)
            {
                if (eType.Value.Equals(value))
                {
                    suppType = eType;
                    break;
                }
            }
            return suppType;
        }

        /// <summary>Gets the description for the emission type from the value.</summary>
        /// <param name="value">The value to get the description of.</param>
        /// <returns>The description for the value, or the passed value if no match found.</returns>
        public static string GetDescription(char value)
        {
            string desc = value.ToString();
            SupportType eType = Find(value);
            if (eType != null)
            {
                desc = eType.Description;
            }
            return desc;
        }



        private static SupportType[] m_all;
        /// <summary>Gets all known emission types retrieved from the database.</summary>
        public static SupportType[] All
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

        /// <summary>Gets the emission type for the 'Visual' emission type record.</summary>
        public static SupportType Sales { get { return Find('A'); } }

        /// <summary>Gets the emission type for the 'OBD' emission type record.</summary>
        public static SupportType Support { get { return Find('S'); } }

        /// <summary>Gets the emission type for the 'None' emission type record.</summary>
        public static SupportType Inventory { get { return Find('I'); } }        
    }



    /// <summary>Stores a emission type record from the R_suppType table.</summary>
    public class SupportType
    {
        /// <summary>Instantiates a new instance of the SupportType class.</summary>
        /// <param name="dr">DataRow containing record information from the database.</param>
        public SupportType(DataRow dr)
        {
            m_value = NullSafe.ToString(dr["CODE_VALUE"])[0];
            m_description = NullSafe.ToString(dr["DESCRIPTION"]);
        }

        /// <summary>Returns the description of the emission type.</summary>
        /// <returns>Description of the emission type.</returns>
        public override string ToString()
        {
            return Description;
        }



        private readonly char m_value;
        /// <summary>Gets the value of the emission type.</summary>
        public char Value
        {
            get { return m_value; }
        }

        private readonly string m_description;
        /// <summary>Gets the description of the emission type.</summary>
        public string Description
        {
            get { return m_description; }
        }
    }
}