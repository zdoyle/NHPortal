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

    /// <summary>Used for storing and accessing values stored in the R_Inq_ChargeableDesc table.</summary>
    public static class Inq_ChargeableDescs
    {
        /// <summary>Gets the inquiry inspection chargeable descriptions from the database to store in memory.</summary>
        public static void Initialize()
        {
            string qry = "SELECT ret.CODE_VALUE, ret.DESCRIPTION" + Environment.NewLine
                       + "FROM   R_INQ_CHARGEABLE_DESC ret";

            List<Inq_ChargeableDesc> Inq_ChargeableDescs = new List<Inq_ChargeableDesc>();
            OracleResponse resp = ODAP.GetDataTable(qry, DatabaseTarget.Adhoc);
            if (resp.Successful)
            {
                foreach (DataRow dr in resp.ResultsTable.Rows)
                {
                    Inq_ChargeableDescs.Add(new Inq_ChargeableDesc(dr));
                }
            }
            m_all = Inq_ChargeableDescs.ToArray();
        }

        /// <summary>Returns an inquiry inspection chargeable descriptions by value.</summary>
        /// <param name="value">Value of the inquiry inspection chargeable descriptions to find.</param>
        /// <returns>inquiry inspection chargeable descriptions matching the value, or null if no match found.</returns>
        public static Inq_ChargeableDesc Find(char value)
        {
            Inq_ChargeableDesc Inq_ChargeableDesc = null;
            foreach (var eType in All)
            {
                if (eType.Value.Equals(value))
                {
                    Inq_ChargeableDesc = eType;
                    break;
                }
            }
            return Inq_ChargeableDesc;
        }

        /// <summary>Gets the description for the inquiry inspection chargeable descriptions from the value.</summary>
        /// <param name="value">The value to get the description of.</param>
        /// <returns>The description for the value, or the passed value if no match found.</returns>
        public static string GetDescription(char value)
        {
            string desc = value.ToString();
            Inq_ChargeableDesc eType = Find(value);
            if (eType != null)
            {
                desc = eType.Description;
            }
            return desc;
        }



        private static Inq_ChargeableDesc[] m_all;
        /// <summary>Gets all known inquiry inspection chargeable descriptionss retrieved from the database.</summary>
        public static Inq_ChargeableDesc[] All
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

        /// <summary>Gets the inquiry inspection chargeable descriptions for the  record.</summary>
        public static Inq_ChargeableDesc No { get { return Find('N'); } }
        public static Inq_ChargeableDesc ThreeFourty { get { return Find('1'); } }
        public static Inq_ChargeableDesc Five { get { return Find('2'); } }
        public static Inq_ChargeableDesc FourSeventy { get { return Find('3'); } } 
        public static Inq_ChargeableDesc FourSixtyFive { get { return Find('4'); } }  
        public static Inq_ChargeableDesc FiveFifteen { get { return Find('5'); } }
        public static Inq_ChargeableDesc FiveTen { get { return Find('6'); } }
        public static Inq_ChargeableDesc ThreeThirtyEight { get { return Find('7'); } }

    }



    /// <summary>Stores a inquiry inspection chargeable descriptions record from the R_EMISTYPE table.</summary>
    public class Inq_ChargeableDesc
    {
        /// <summary>Instantiates a new instance of the EmissionType class.</summary>
        /// <param name="dr">DataRow containing record information from the database.</param>
        public Inq_ChargeableDesc(DataRow dr)
        {
            m_value = NullSafe.ToString(dr["CODE_VALUE"])[0];
            m_description = NullSafe.ToString(dr["DESCRIPTION"]);
        }

        /// <summary>Returns the description of the inquiry inspection chargeable descriptions.</summary>
        /// <returns>Description of the inquiry inspection chargeable descriptions.</returns>
        public override string ToString()
        {
            return Description;
        }



        private readonly char m_value;
        /// <summary>Gets the value of the inquiry inspection chargeable descriptions.</summary>
        public char Value
        {
            get { return m_value; }
        }

        private readonly string m_description;
        /// <summary>Gets the description of the inquiry inspection chargeable descriptions.</summary>
        public string Description
        {
            get { return m_description; }
        }
    }
}