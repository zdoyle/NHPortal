using GDCoreUtilities;
using GDDatabaseClient.Oracle;
using PortalFramework.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace NHPortal.Classes.User
{
    /// <summary>Used for storing and accessing values stored in the r_favorite_type table.</summary>
    public static class UserFavoriteTypes
    {
        /// <summary>Gets the user favorite types from the database to store in memory.</summary>
        public static void Initialize()
        {
            List<UserFavoriteType> favs = new List<UserFavoriteType>();
            string qry = "SELECT rft.rft_code, rft.rft_desc" + Environment.NewLine
                       + "FROM   r_favorite_type rft" + Environment.NewLine
                       + "ORDER BY rft.rft_code";

            OracleResponse oResponse = ODAP.GetDataTable(qry, DatabaseTarget.Adhoc);
            if (oResponse.Successful)
            {
                foreach (DataRow dr in oResponse.ResultsTable.Rows)
                {
                    favs.Add(new UserFavoriteType(dr));
                }
            }
            all = favs.ToArray();
        }

        /// <summary>Finds a favorite type by code.</summary>
        /// <param name="code">Code of the favorite type to find.</param>
        /// <returns>User favorite type matching the provide code, or null if none found.</returns>
        public static UserFavoriteType Find(int code)
        {
            UserFavoriteType usrFavorite = null;
            foreach (var fav in All)
            {
                if (fav.Code.Equals(code))
                {
                    usrFavorite = fav;
                    break;
                }
            }
            return usrFavorite;
        }



        private static UserFavoriteType[] all;
        /// <summary>Gets an array of UserFavorite types.</summary>
        public static UserFavoriteType[] All
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

        /// <summary>Gets a reference to the Report favorite type.</summary>
        public static UserFavoriteType Report { get { return Find(1); } }

        /// <summary>Gets a reference to the Trigger favorite type.</summary>
        public static UserFavoriteType Trigger { get { return Find(2); } }

        /// <summary>Gets a reference to the Graph favorite type.</summary>
        public static UserFavoriteType Graph { get { return Find(3); } }

        /// <summary>Gets a reference to the Query Builder favorite type.</summary>
        public static UserFavoriteType QueryBuilder { get { return Find(4); } }
    }



    /// <summary>Stores information about a R_FAVOTITE_TYPE record.</summary>
    public class UserFavoriteType
    {
        /// <summary>Instantiates a new instance of the UserFavoriteType class.</summary>
        /// <param name="dr">DataRow containing information about the record.</param>
        public UserFavoriteType(DataRow dr)
        {
            m_code = NullSafe.ToInt(dr["RFT_CODE"]);
            m_desc = NullSafe.ToString(dr["RFT_DESC"]);
        }

        /// <summary>Returns the description of the user favorite type.</summary>
        /// <returns>Description of the user favorite type.</returns>
        public override string ToString()
        {
            return Description;
        }



        private readonly int m_code;
        /// <summary>Gets the code for the favorite type. Read-only field.</summary>
        public int Code
        {
            get { return m_code; }
        }

        private readonly string m_desc;
        /// <summary>Gets the description of the favorite type. Read-only field.</summary>
        public string Description
        {
            get { return m_desc; }
        }
    }
}