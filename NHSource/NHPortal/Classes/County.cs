using GDCoreUtilities;
using PortalFramework.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace NHPortal.Classes
{
    /// <summary>Used for storing and accessing values stored in the county table.</summary>
    public static class Counties
    {
        private static County[] all;
        /// <summary>Gets an array of all counties retrieved from the database.</summary>
        public static County[] All
        {
            get
            {
                if (all == null)
                {
                    Initialize();
                }
                return all;
            }
        }

        /// <summary>Gets the counties from the database to store in memory.</summary>
        public static void Initialize()
        {
            string qry = "SELECT rc.cnty_name" + Environment.NewLine
                       + "FROM   r_county rc";

            List<County> counties = new List<County>();
            GDDatabaseClient.Oracle.OracleResponse response = ODAP.GetDataTable(qry, DatabaseTarget.Adhoc);
            if (response.Successful)
            {
                foreach (DataRow dr in response.ResultsTable.Rows)
                {
                    counties.Add(new County(dr));
                }
            }
            all = counties.ToArray();
        }

        /// <summary>Finds a county.</summary>
        /// <param name="name">Name of the county to find.</param>
        /// <returns>County matching the provided name, or null if no match found.</returns>
        public static County Find(string name)
        {
            County foundCounty = null;
            foreach (County c in All)
            {
                if (c.Name.Trim().Equals(name.Trim(), StringComparison.InvariantCultureIgnoreCase))
                {
                    foundCounty = c;
                    break;
                }
            }
            return foundCounty;
        }

        public static County Belknap { get { return Find("Belknap"); } }
        public static County Carroll { get { return Find("Carroll"); } }
        public static County Cheshire { get { return Find("Cheshire"); } }
        public static County Coos { get { return Find("Coos"); } }
        public static County Grafton { get { return Find("Grafton"); } }
        public static County Hillsborough { get { return Find("Hillsborough"); } }
        public static County Merrimack { get { return Find("Merrimack"); } }
        public static County Rockingham { get { return Find("Rockingham"); } }
        public static County Strafford { get { return Find("Strafford"); } }
        public static County Sullivan { get { return Find("Sullivan"); } }
    }

    /// <summary>Represents a county stored in the R_COUNTY reference table.</summary>
    public class County
    {
        /// <summary>Instantiates a new instance of the County class.</summary>
        /// <param name="dr">DataRow containing county information.</param>
        public County(DataRow dr)
        {
            Name = NullSafe.ToString(dr["CNTY_NAME"]);
        }

        /// <summary>Gets the county name.</summary>
        public string Name { get; private set; }
    }
}