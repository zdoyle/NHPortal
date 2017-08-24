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
    /// <summary>Used for storing and accessing values stored in the r_emistype table.</summary>
    public class StationAuditTypes
    {
        /// <summary>Gets the emission types from the database to store in memory.</summary>
        public static void Initialize()
        {
            string qry = "SELECT ret.STATION_AUDIT_CODE, ret.STATION_AUDIT_DESCRIPTION" + Environment.NewLine
                       + "FROM   r_station_audit_type ret";

            List<StationAuditType> stationAuditTypes = new List<StationAuditType>();
            OracleResponse resp = ODAP.GetDataTable(qry, DatabaseTarget.Adhoc);
            if (resp.Successful)
            {
                foreach (DataRow dr in resp.ResultsTable.Rows)
                {
                    stationAuditTypes.Add(new StationAuditType(dr));
                }
            }
            m_all = stationAuditTypes.ToArray();
        }

        /// <summary>Returns an emission type by value.</summary>
        /// <param name="value">Value of the emission type to find.</param>
        /// <returns>Emission type matching the value, or null if no match found.</returns>
        public static StationAuditType Find(char value)
        {
            StationAuditType stationAuditType = null;
            foreach (var eType in All)
            {
                if (eType.Value.Equals(value))
                {
                    stationAuditType = eType;
                    break;
                }
            }
            return stationAuditType;
        }

        /// <summary>Gets the description for the emission type from the value.</summary>
        /// <param name="value">The value to get the description of.</param>
        /// <returns>The description for the value, or the passed value if no match found.</returns>
        public static string GetDescription(char value)
        {
            string desc = value.ToString();
            StationAuditType eType = Find(value);
            if (eType != null)
            {
                desc = eType.Description;
            }
            return desc;
        }



        private static StationAuditType[] m_all;
        /// <summary>Gets all known emission types retrieved from the database.</summary>
        public static StationAuditType[] All
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
        public static StationAuditType Motocycle { get { return Find('M'); } }

        /// <summary>Gets the emission type for the 'OBD' emission type record.</summary>
        public static StationAuditType Fleet { get { return Find('F'); } }

        /// <summary>Gets the emission type for the 'None' emission type record.</summary>
        public static StationAuditType Municiple { get { return Find('U'); } }

        /// <summary>Gets the emission type for the 'None' emission type record.</summary>
        public static StationAuditType Automobile { get { return Find('A'); } }
    }



    /// <summary>Stores a emission type record from the R_EMISTYPE table.</summary>
    public class StationAuditType
    {
        /// <summary>Instantiates a new instance of the EmissionType class.</summary>
        /// <param name="dr">DataRow containing record information from the database.</param>
        public StationAuditType(DataRow dr)
        {
            m_value = NullSafe.ToString(dr["STATION_AUDIT_CODE"])[0];
            m_description = NullSafe.ToString(dr["STATION_AUDIT_DESC"]);
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