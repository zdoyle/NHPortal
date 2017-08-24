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
    /// <summary>Used for storing and accessing values stored in the r_supportMethType table.</summary>
    public static class SupportMethods
    {
        /// <summary>Gets the emission types from the database to store in memory.</summary>
        public static void Initialize()
        {
            string qry = "SELECT ret.CODE_VALUE, ret.DESCRIPTION" + Environment.NewLine
                       + "FROM   r_support_method ret";

            List<SupportMethod> supportMethTypes = new List<SupportMethod>();
            OracleResponse resp = ODAP.GetDataTable(qry, DatabaseTarget.Adhoc);
            if (resp.Successful)
            {
                foreach (DataRow dr in resp.ResultsTable.Rows)
                {
                    supportMethTypes.Add(new SupportMethod(dr));
                }
            }
            m_all = supportMethTypes.ToArray();
        }

        /// <summary>Returns an emission type by value.</summary>
        /// <param name="value">Value of the emission type to find.</param>
        /// <returns>Emission type matching the value, or null if no match found.</returns>
        public static SupportMethod Find(char value)
        {
            SupportMethod supportMethType = null;
            foreach (var eType in All)
            {
                if (eType.Value.Equals(value))
                {
                    supportMethType = eType;
                    break;
                }
            }
            return supportMethType;
        }


        public static string GetDescription(char value)
        {
            string desc = value.ToString();
            SupportMethod eType = Find(value);
            if (eType != null)
            {
                desc = eType.Description;
            }
            return desc;
        }



        private static SupportMethod[] m_all;
        /// <summary>Gets all known emission types retrieved from the database.</summary>
        public static SupportMethod[] All
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

        public static SupportMethod NoShip { get { return Find('A'); } }
        public static SupportMethod Onsite { get { return Find('O'); } }
        public static SupportMethod Ship { get { return Find('S'); } }
        public static SupportMethod NoPend { get { return Find('B'); } }
        public static SupportMethod Depot { get { return Find('D'); } }
        public static SupportMethod None { get { return Find('N'); } }
        public static SupportMethod Pending { get { return Find('P'); } }
        public static SupportMethod Callback { get { return Find('C'); } }
       
    }


    public class SupportMethod
    {
        /// <summary>Instantiates a new instance of the SupportMethod class.</summary>
        /// <param name="dr">DataRow containing record information from the database.</param>
        public SupportMethod(DataRow dr)
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