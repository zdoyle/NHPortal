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

    /// <summary>Used for storing and accessing values stored in the r_safety table.</summary>
    public static class SafetyTypes
    {
        /// <summary>Gets the emission types from the database to store in memory.</summary>
        public static void Initialize()
        {
            string qry = "SELECT ret.CODE_VALUE, ret.DESCRIPTION" + Environment.NewLine
                       + "FROM   r_safetytype ret";

            List<SafetyType> safetyTypes = new List<SafetyType>();
            OracleResponse resp = ODAP.GetDataTable(qry, DatabaseTarget.Adhoc);
            if (resp.Successful)
            {
                foreach (DataRow dr in resp.ResultsTable.Rows)
                {
                    safetyTypes.Add(new SafetyType(dr));
                }
            }
            m_all = safetyTypes.ToArray();
        }

        /// <summary>Returns an emission type by value.</summary>
        /// <param name="value">Value of the emission type to find.</param>
        /// <returns>Emission type matching the value, or null if no match found.</returns>
        public static SafetyType Find(char value)
        {
            SafetyType safetyType = null;
            foreach (var eType in All)
            {
                if (eType.Value.Equals(value))
                {
                    safetyType = eType;
                    break;
                }
            }
            return safetyType;
        }

        /// <summary>Gets the description for the emission type from the value.</summary>
        /// <param name="value">The value to get the description of.</param>
        /// <returns>The description for the value, or the passed value if no match found.</returns>
        public static string GetDescription(char value)
        {
            string desc = value.ToString();
            SafetyType eType = Find(value);
            if (eType != null)
            {
                desc = eType.Description;
            }
            return desc;
        }



        private static SafetyType[] m_all;
        /// <summary>Gets all known emission types retrieved from the database.</summary>
        public static SafetyType[] All
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
        public static SafetyType Basic { get { return Find('V'); } }

        /// <summary>Gets the emission type for the 'OBD' emission type record.</summary>
        public static SafetyType Truck { get { return Find('T'); } }

        /// <summary>Gets the emission type for the 'OBD' emission type record.</summary>
        public static SafetyType Bus { get { return Find('T'); } }

        /// <summary>Gets the emission type for the 'None' emission type record.</summary>
        public static SafetyType Motorcycle { get { return Find('M'); } } 
       
        /// <summary>Gets the emission type for the 'None' emission type record.</summary>
        public static SafetyType Agriculture { get { return Find('A'); } }  

        /// <summary>Gets the emission type for the 'None' emission type record.</summary>
        public static SafetyType Trailer { get { return Find('R'); } }  
    }



    /// <summary>Stores a emission type record from the R_EMISTYPE table.</summary>
    public class SafetyType
    {
        /// <summary>Instantiates a new instance of the EmissionType class.</summary>
        /// <param name="dr">DataRow containing record information from the database.</param>
        public SafetyType(DataRow dr)
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