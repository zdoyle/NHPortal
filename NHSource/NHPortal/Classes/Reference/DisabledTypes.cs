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
    /// <summary>Used for storing and accessing values stored in the R_DISABLED_STATUS table.</summary>
    public static class DisabledTypes
    {
        /// <summary>Gets the disabled types from the database to store in memory.</summary>
        public static void Initialize()
        {
            string qry = "SELECT ret.CODE_VALUE, ret.DESCRIPTION" + Environment.NewLine
                       + "FROM   R_DISABLED_STATUS ret";

            List<DisabledType> disabledTypes = new List<DisabledType>();
            OracleResponse resp = ODAP.GetDataTable(qry, DatabaseTarget.Adhoc);
            if (resp.Successful)
            {
                foreach (DataRow dr in resp.ResultsTable.Rows)
                {
                    disabledTypes.Add(new DisabledType(dr));
                }
            }
            m_all = disabledTypes.ToArray();
        }

        /// <summary>Returns an emission type by value.</summary>
        /// <param name="value">Value of the emission type to find.</param>
        /// <returns>Emission type matching the value, or null if no match found.</returns>
        public static DisabledType Find(char value)
        {
            DisabledType disabledTypes = null;
            foreach (var eType in All)
            {
                if (eType.Value.Equals(value))
                {
                    disabledTypes = eType;
                    break;
                }
            }
            return disabledTypes;
        }

        /// <summary>Gets the description for the emission type from the value.</summary>
        /// <param name="value">The value to get the description of.</param>
        /// <returns>The description for the value, or the passed value if no match found.</returns>
        public static string GetDescription(char value)
        {
            string desc = value.ToString();
            DisabledType eType = Find(value);
            if (eType != null)
            {
                desc = eType.Description;
            }
            return desc;
        }



        private static DisabledType[] m_all;
        /// <summary>Gets all known emission types retrieved from the database.</summary>
        public static DisabledType[] All
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


        public static DisabledType nulls { get { return Find('0'); } }

        public static DisabledType Payment { get { return Find('1'); } }

        public static DisabledType Enforcement { get { return Find('2'); } }
        public static DisabledType PaymentEnforcement { get { return Find('3'); } }
        public static DisabledType OOB { get { return Find('4'); } }
        public static DisabledType PaymentOOB { get { return Find('5'); } }
        public static DisabledType EnforcementOOB { get { return Find('6'); } }
        public static DisabledType PayEnforceOOB { get { return Find('7'); } }     
    }



    /// <summary>Stores a emission type record from the R_EMISTYPE table.</summary>
    public class DisabledType
    {
        /// <summary>Instantiates a new instance of the EmissionType class.</summary>
        /// <param name="dr">DataRow containing record information from the database.</param>
        public DisabledType(DataRow dr)
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