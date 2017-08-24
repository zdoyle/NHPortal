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
    /// <summary>Used for storing and accessing values stored in the r_fuelcode table.</summary>
    public class FuelCodeTypes
    {
        public static void Initialize()
        {
            string qry = "SELECT ret.FUELCODE, ret.FUELCODE_DESC" + Environment.NewLine
                       + "FROM   r_fuelcode ret";

            List<FuelCodeType> emisTypes = new List<FuelCodeType>();
            OracleResponse resp = ODAP.GetDataTable(qry, DatabaseTarget.Adhoc);
            if (resp.Successful)
            {
                foreach (DataRow dr in resp.ResultsTable.Rows)
                {
                    emisTypes.Add(new FuelCodeType(dr));
                }
            }
            m_all = emisTypes.ToArray();
        }

        /// <summary>Returns an fuel code type by value.</summary>
        /// <param name="value">Value of the fuel code type to find.</param>
        /// <returns>Fuel code type matching the value, or null if no match found.</returns>
        public static FuelCodeType Find(char value)
        {
            FuelCodeType fuelCodeType = null;
            foreach (var eType in All)
            {
                if (eType.Value.Equals(value))
                {
                    fuelCodeType = eType;
                    break;
                }
            }
            return fuelCodeType;
        }

        /// <summary>Gets the description for the fuel code type from the value.</summary>
        /// <param name="value">The value to get the description of.</param>
        /// <returns>The description for the value, or the passed value if no match found.</returns>
        public static string GetDescription(char value)
        {
            string desc = value.ToString();
            FuelCodeType eType = Find(value);
            if (eType != null)
            {
                desc = eType.Description;
            }
            return desc;
        }

        private static FuelCodeType[] m_all;
        /// <summary>Gets all known fuel code types retrieved from the database.</summary>
        public static FuelCodeType[] All
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
    }

    /// <summary>Stores a fuel code type record from the R_FUELCODE table.</summary>
    public class FuelCodeType
    {
        /// <summary>Instantiates a new instance of the FuelCodeType class.</summary>
        /// <param name="dr">DataRow containing record information from the database.</param>
        public FuelCodeType(DataRow dr)
        {
            m_value = NullSafe.ToString(dr["FUELCODE"])[0];
            m_description = NullSafe.ToString(dr["FUELCODE_DESC"]);
        }

        /// <summary>Returns the description of the fuel code type.</summary>
        /// <returns>Description of the fuel code type.</returns>
        public override string ToString()
        {
            return Description;
        }

        private readonly char m_value;
        /// <summary>Gets the value of the fuel code type.</summary>
        public char Value
        {
            get { return m_value; }
        }

        private readonly string m_description;
        /// <summary>Gets the description of the fuel code type.</summary>
        public string Description
        {
            get { return m_description; }
        }
    }
}
