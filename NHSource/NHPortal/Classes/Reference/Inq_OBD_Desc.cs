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

    /// <summary>Used for storing and accessing values stored in the R_Inq_OBD_Desc table.</summary>
    public static class Inq_OBD_Descs
    {
        /// <summary>Gets the inquiry inspection OBD descriptions from the database to store in memory.</summary>
        public static void Initialize()
        {
            string qry = "SELECT ret.CODE_VALUE, ret.DESCRIPTION" + Environment.NewLine
                       + "FROM   R_INQ_OBD_DESC ret";

            List<Inq_OBD_Desc> Inq_OBD_Descs = new List<Inq_OBD_Desc>();
            OracleResponse resp = ODAP.GetDataTable(qry, DatabaseTarget.Adhoc);
            if (resp.Successful)
            {
                foreach (DataRow dr in resp.ResultsTable.Rows)
                {
                    Inq_OBD_Descs.Add(new Inq_OBD_Desc(dr));
                }
            }
            m_all = Inq_OBD_Descs.ToArray();
        }

        /// <summary>Returns an inquiry inspection OBD descriptions by value.</summary>
        /// <param name="value">Value of the inquiry inspection OBD descriptions to find.</param>
        /// <returns>inquiry inspection OBD descriptions matching the value, or null if no match found.</returns>
        public static Inq_OBD_Desc Find(char value)
        {
            Inq_OBD_Desc Inq_OBD_Desc = null;
            foreach (var eType in All)
            {
                if (eType.Value.Equals(value))
                {
                    Inq_OBD_Desc = eType;
                    break;
                }
            }
            return Inq_OBD_Desc;
        }

        /// <summary>Gets the description for the inquiry inspection OBD descriptions from the value.</summary>
        /// <param name="value">The value to get the description of.</param>
        /// <returns>The description for the value, or the passed value if no match found.</returns>
        public static string GetDescription(char value)
        {
            string desc = value.ToString();
            Inq_OBD_Desc eType = Find(value);
            if (eType != null)
            {
                desc = eType.Description;
            }
            return desc;
        }



        private static Inq_OBD_Desc[] m_all;
        /// <summary>Gets all known inquiry inspection OBD descriptionss retrieved from the database.</summary>
        public static Inq_OBD_Desc[] All
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

        /// <summary>Gets the inquiry inspection OBD descriptions for the  record.</summary>
        public static Inq_OBD_Desc Unknown { get { return Find('0'); } }
        public static Inq_OBD_Desc Ready { get { return Find('1'); } }
        public static Inq_OBD_Desc NotReady { get { return Find('2'); } }
        public static Inq_OBD_Desc NotSupported { get { return Find('4'); } }  
    }



    /// <summary>Stores a inquiry inspection OBD descriptions record from the R_EMISTYPE table.</summary>
    public class Inq_OBD_Desc
    {
        /// <summary>Instantiates a new instance of the EmissionType class.</summary>
        /// <param name="dr">DataRow containing record information from the database.</param>
        public Inq_OBD_Desc(DataRow dr)
        {
            m_value = NullSafe.ToString(dr["CODE_VALUE"])[0];
            m_description = NullSafe.ToString(dr["DESCRIPTION"]);
        }

        /// <summary>Returns the description of the inquiry inspection OBD descriptions.</summary>
        /// <returns>Description of the inquiry inspection OBD descriptions.</returns>
        public override string ToString()
        {
            return Description;
        }



        private readonly char m_value;
        /// <summary>Gets the value of the inquiry inspection OBD descriptions.</summary>
        public char Value
        {
            get { return m_value; }
        }

        private readonly string m_description;
        /// <summary>Gets the description of the inquiry inspection OBD descriptions.</summary>
        public string Description
        {
            get { return m_description; }
        }
    }
}