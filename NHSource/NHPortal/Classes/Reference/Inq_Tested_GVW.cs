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

    /// <summary>Used for storing and accessing values stored in the R_Inq_Tested_GVW table.</summary>
    public static class Inq_Tested_GVWs
    {
        /// <summary>Gets the inquiry inspection Tested GVW from the database to store in memory.</summary>
        public static void Initialize()
        {
            string qry = "SELECT ret.CODE_VALUE, ret.DESCRIPTION" + Environment.NewLine
                       + "FROM   R_INQ_TESTED_GVW ret";

            List<Inq_Tested_GVW> Inq_Tested_GVWs = new List<Inq_Tested_GVW>();
            OracleResponse resp = ODAP.GetDataTable(qry, DatabaseTarget.Adhoc);
            if (resp.Successful)
            {
                foreach (DataRow dr in resp.ResultsTable.Rows)
                {
                    Inq_Tested_GVWs.Add(new Inq_Tested_GVW(dr));
                }
            }
            m_all = Inq_Tested_GVWs.ToArray();
        }

        /// <summary>Returns an inquiry inspection Tested GVW by value.</summary>
        /// <param name="value">Value of the inquiry inspection Tested GVW to find.</param>
        /// <returns>inquiry inspection Tested GVW matching the value, or null if no match found.</returns>
        public static Inq_Tested_GVW Find(char value)
        {
            Inq_Tested_GVW Inq_Tested_GVW = null;
            foreach (var eType in All)
            {
                if (eType.Value.Equals(value))
                {
                    Inq_Tested_GVW = eType;
                    break;
                }
            }
            return Inq_Tested_GVW;
        }

        /// <summary>Gets the description for the inquiry inspection Tested GVW from the value.</summary>
        /// <param name="value">The value to get the description of.</param>
        /// <returns>The description for the value, or the passed value if no match found.</returns>
        public static string GetDescription(char value)
        {
            string desc = value.ToString();
            Inq_Tested_GVW eType = Find(value);
            if (eType != null)
            {
                desc = eType.Description;
            }
            return desc;
        }



        private static Inq_Tested_GVW[] m_all;
        /// <summary>Gets all known inquiry inspection Tested GVWs retrieved from the database.</summary>
        public static Inq_Tested_GVW[] All
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

        /// <summary>Gets the inquiry inspection Tested GVW for the  record.</summary>
        public static Inq_Tested_GVW Greater { get { return Find('H'); } }
        public static Inq_Tested_GVW Between { get { return Find('M'); } }
        public static Inq_Tested_GVW Less { get { return Find('L'); } }
    }



    /// <summary>Stores a inquiry inspection Tested GVW record from the R_EMISTYPE table.</summary>
    public class Inq_Tested_GVW
    {
        /// <summary>Instantiates a new instance of the EmissionType class.</summary>
        /// <param name="dr">DataRow containing record information from the database.</param>
        public Inq_Tested_GVW(DataRow dr)
        {
            m_value = NullSafe.ToString(dr["CODE_VALUE"])[0];
            m_description = NullSafe.ToString(dr["DESCRIPTION"]);
        }

        /// <summary>Returns the description of the inquiry inspection Tested GVW.</summary>
        /// <returns>Description of the inquiry inspection Tested GVW.</returns>
        public override string ToString()
        {
            return Description;
        }



        private readonly char m_value;
        /// <summary>Gets the value of the inquiry inspection Tested GVW.</summary>
        public char Value
        {
            get { return m_value; }
        }

        private readonly string m_description;
        /// <summary>Gets the description of the inquiry inspection Tested GVW.</summary>
        public string Description
        {
            get { return m_description; }
        }
    }
}