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

    /// <summary>Used for storing and accessing values stored in the R_INQ_OVERALLRESULT table.</summary>
    public static class Inq_OverallResults
    {
        /// <summary>Gets the inquiry inspection overall results from the database to store in memory.</summary>
        public static void Initialize()
        {
            string qry = "SELECT ret.CODE_VALUE, ret.DESCRIPTION" + Environment.NewLine
                       + "FROM   R_INQ_OVERALLRESULT ret";

            List<Inq_OverallResult> Inq_OverallResults = new List<Inq_OverallResult>();
            OracleResponse resp = ODAP.GetDataTable(qry, DatabaseTarget.Adhoc);
            if (resp.Successful)
            {
                foreach (DataRow dr in resp.ResultsTable.Rows)
                {
                    Inq_OverallResults.Add(new Inq_OverallResult(dr));
                }
            }
            m_all = Inq_OverallResults.ToArray();
        }

        /// <summary>Returns an inquiry inspection overall results by value.</summary>
        /// <param name="value">Value of the inquiry inspection overall results to find.</param>
        /// <returns>inquiry inspection overall results matching the value, or null if no match found.</returns>
        public static Inq_OverallResult Find(char value)
        {
            Inq_OverallResult Inq_OverallResult = null;
            foreach (var eType in All)
            {
                if (eType.Value.Equals(value))
                {
                    Inq_OverallResult = eType;
                    break;
                }
            }
            return Inq_OverallResult;
        }

        /// <summary>Gets the description for the inquiry inspection overall results from the value.</summary>
        /// <param name="value">The value to get the description of.</param>
        /// <returns>The description for the value, or the passed value if no match found.</returns>
        public static string GetDescription(char value)
        {
            string desc = value.ToString();
            Inq_OverallResult eType = Find(value);
            if (eType != null)
            {
                desc = eType.Description;
            }
            return desc;
        }



        private static Inq_OverallResult[] m_all;
        /// <summary>Gets all known inquiry inspection overall resultss retrieved from the database.</summary>
        public static Inq_OverallResult[] All
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

        /// <summary>Gets the inquiry inspection overall results for the  record.</summary>
        public static Inq_OverallResult Unknown { get { return Find('0'); } }
        public static Inq_OverallResult Pass { get { return Find('1'); } }
        public static Inq_OverallResult Reject { get { return Find('2'); } }
        public static Inq_OverallResult Corrected { get { return Find('3'); } } 
        public static Inq_OverallResult NA { get { return Find('4'); } }  
        public static Inq_OverallResult Admin { get { return Find('5'); } }
        public static Inq_OverallResult Abort { get { return Find('6'); } }
        public static Inq_OverallResult PrevPass { get { return Find('9'); } }  
    }



    /// <summary>Stores a inquiry inspection overall results record from the R_EMISTYPE table.</summary>
    public class Inq_OverallResult
    {
        /// <summary>Instantiates a new instance of the EmissionType class.</summary>
        /// <param name="dr">DataRow containing record information from the database.</param>
        public Inq_OverallResult(DataRow dr)
        {
            m_value = NullSafe.ToString(dr["CODE_VALUE"])[0];
            m_description = NullSafe.ToString(dr["DESCRIPTION"]);
        }

        /// <summary>Returns the description of the inquiry inspection overall results.</summary>
        /// <returns>Description of the inquiry inspection overall results.</returns>
        public override string ToString()
        {
            return Description;
        }



        private readonly char m_value;
        /// <summary>Gets the value of the inquiry inspection overall results.</summary>
        public char Value
        {
            get { return m_value; }
        }

        private readonly string m_description;
        /// <summary>Gets the description of the inquiry inspection overall results.</summary>
        public string Description
        {
            get { return m_description; }
        }
    }
}