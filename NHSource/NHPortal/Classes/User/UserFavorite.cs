using GDCoreUtilities;
using GDDatabaseClient.Oracle;
using Oracle.DataAccess.Client;
using PortalFramework.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace NHPortal.Classes.User
{
    /// <summary>Stores information about a user's "My Favorite".</summary>
    public class UserFavorite
    {
        /// <summary>Instantiates a new instance of the UserFavorite class.</summary>
        public UserFavorite()
        {
            SysNo = 0;
            Title = String.Empty;
            Description = String.Empty;
            NavCode = String.Empty;
            FavType = UserFavoriteTypes.Report;
            Active = true;
            m_criteria = new List<UserFavoriteCriteria>();
        }

        /// <summary>Instantiates a new instance of the UserFavorite class.</summary>
        /// <param name="dr">DataRow containing information about the favorite record.</param>
        public UserFavorite(DataRow dr)
        {
            NullSafeRowWrapper row = new NullSafeRowWrapper(dr);
            SysNo = row.ToLong("FAV_SYS_NO");
            Title = row.ToString("FAV_TITLE");
            Description = row.ToString("FAV_DESCRIPTION");
            NavCode = row.ToString("FAV_NAV_CODE");
            FavType = UserFavoriteTypes.Find(row.ToInt("FAV_RFT_CODE"));
            Active = row.ToBoolean("FAV_ACTIVE");
            m_criteria = null;
        }

        /// <summary>Adds a criteria value to the favorite.</summary>
        /// <param name="desc">Description of the criteria.</param>
        /// <param name="display">Display text of the criteria.</param>
        /// <param name="value">Value of the criteria.</param>
        public void AddCriteria(string desc, string display, string value)
        {
            UserFavoriteCriteria c = new UserFavoriteCriteria(this, desc, display, value);
            Criteria.Add(c);
        }

        /// <summary>Gets a value for the criteria matching the description.</summary>
        /// <param name="desc">Description of the criteria to get the value of.</param>
        /// <returns>Value of the criteria matching the description, or an empty string.</returns>
        public string GetCriteriaValue(string desc)
        {
            string value = String.Empty;

            foreach (var c in Criteria)
            {
                if (GDCoreUtilities.StringUtilities.AreEqual(c.Description, desc))
                {
                    value = c.Value;
                    break;
                }
            }
            return value;
        }

        /// <summary>Saves the record and child records to the database. Uses the PortalUser stored in session for parameters.</summary>
        /// <returns>True if the save was successful, false otherwise.</returns>
        public bool Save()
        {
            string usrName = "UNKNOWN";
            long usrSysNo = -1;

            PortalUser usr = SessionHelper.GetPortalUser(HttpContext.Current.Session);
            if (usr != null)
            {
                usrName = usr.UserName;
                usrSysNo = usr.UserSysNo;
            }

            return Save(usrName, usrSysNo);
        }

        /// <summary>Saves the record and child records to the database. Uses the PortalUser stored in session for parameters.</summary>
        /// <param name="usrName">Name to store in the created by field.</param>
        /// <param name="usrSysNo">System number the favorite record is for.</param>
        /// <returns>True if the save was successful, false otherwise.</returns>
        public bool Save(string usrName, long usrSysNo)
        {
            List<OracleParameter> oraParameters = new List<OracleParameter>();
            oraParameters.Add(new OracleParameter("sysNo", OracleDbType.Int32, 8, SysNo, ParameterDirection.InputOutput));
            oraParameters.Add(new OracleParameter("usrSysNo", OracleDbType.Int32, 8, usrSysNo, ParameterDirection.Input));
            oraParameters.Add(new OracleParameter("favTitle", OracleDbType.Varchar2, 50, Title, ParameterDirection.Input));
            oraParameters.Add(new OracleParameter("favDesc", OracleDbType.Varchar2, 100, Description, ParameterDirection.Input));
            oraParameters.Add(new OracleParameter("navCode", OracleDbType.Varchar2, 30, NavCode, ParameterDirection.Input));
            oraParameters.Add(new OracleParameter("favType", OracleDbType.Int32, 2, FavType.Code, ParameterDirection.Input));
            oraParameters.Add(new OracleParameter("createUsr", OracleDbType.Varchar2, 30, usrName, ParameterDirection.Input));
            oraParameters.Add(new OracleParameter("rspMsg", OracleDbType.Varchar2, 2000, null, ParameterDirection.Output));
            oraParameters.Add(new OracleParameter("rspCode", OracleDbType.Int32, 4, null, ParameterDirection.Output));

            bool success = false;
            OracleResponse procRsp = PortalFramework.Database.ODAP.CallProcedure("CREATE_USER_FAVORITE", oraParameters.ToArray(), PortalFramework.Database.DatabaseTarget.Adhoc);
            if (NHProcedureUtilities.WasSuccessful(procRsp))
            {
                SysNo = GDCoreUtilities.NullSafe.ToInt(procRsp.ReturnParameters["sysNo"]);
                success = SaveCriteria(usrName);
            }
            return success;
        }

        private bool SaveCriteria(string usrName)
        {
            bool success = true;
            foreach (var c in Criteria)
            {
                success = c.Save(usrName);
                if (!success)
                {
                    break; // stop saving criteria if one fails
                }
            }
            return success;
        }

        /// <summary>Sets the favorite record as inactive in the database.</summary>
        /// <param name="usrName">Name of the user updating the favorite record.</param>
        /// <returns>True if the record was successfully inactived, false otherwise.</returns>
        public bool Delete(string usrName)
        {
            List<OracleParameter> oraParameters = new List<OracleParameter>();
            oraParameters.Add(new OracleParameter("sysNo", OracleDbType.Int32, 8, SysNo, ParameterDirection.Input));
            oraParameters.Add(new OracleParameter("updateUsr", OracleDbType.Varchar2, 30, usrName, ParameterDirection.Input));
            oraParameters.Add(new OracleParameter("rspMsg", OracleDbType.Varchar2, 2000, null, ParameterDirection.Output));
            oraParameters.Add(new OracleParameter("rspCode", OracleDbType.Int32, 4, null, ParameterDirection.Output));

            OracleResponse procRsp = PortalFramework.Database.ODAP.CallProcedure("DELETE_USER_FAVORITE", oraParameters.ToArray(), PortalFramework.Database.DatabaseTarget.Adhoc);
            return NHProcedureUtilities.WasSuccessful(procRsp);
        }

        /// <summary>Gets an array of favorite records for the user.</summary>
        /// <param name="usr">User to get the favorites for.</param>
        /// <returns>Array of favorites for the user. Empty list if the PortalUser object is null.</returns>
        public static UserFavorite[] GetForUser(PortalUser usr)
        {
            List<UserFavorite> favs = new List<UserFavorite>();
            if (usr != null)
            {
                string qry = "SELECT  fav.FAV_SYS_NO, fav.FAV_NHUSR_SYS_NO," + Environment.NewLine
                           + "        fav.FAV_TITLE, fav.FAV_DESCRIPTION, fav.FAV_NAV_CODE," + Environment.NewLine
                           + "        fav.FAV_RFT_CODE, fav.FAV_ACTIVE" + Environment.NewLine
                           + "FROM    nh_user_favorite fav" + Environment.NewLine
                           + "WHERE   fav.FAV_NHUSR_SYS_NO = " + usr.UserSysNo + Environment.NewLine // :usrSysNo";
                           + "ORDER BY fav.FAV_SYS_NO";

                // TODO use bind variable

                OracleResponse rsp = ODAP.GetDataTable(qry, DatabaseTarget.Adhoc);
                if (rsp.Successful)
                {
                    foreach (DataRow dr in rsp.ResultsTable.Rows)
                    {
                        favs.Add(new UserFavorite(dr));
                    }
                }
            }
            return favs.ToArray();
        }



        /// <summary>Gets the unique system number assigned to the favorite.</summary>
        public long SysNo { get; private set; }

        /// <summary>Gets or sets the favorite's title.</summary>
        public string Title { get; set; }

        /// <summary>Gets or sets the description of the favorite.</summary>
        public string Description { get; set; }

        /// <summary>Gets or sets the navigation code for the favorite.</summary>
        public string NavCode { get; set; }

        /// <summary>Gets or sets the favorite type.</summary>
        public UserFavoriteType FavType { get; set; }

        /// <summary>Gets whether or not the favorite is active.</summary>
        public bool Active { get; private set; }

        private List<UserFavoriteCriteria> m_criteria;
        /// <summary>Gets a collection of criteria defining the favorite record.</summary>
        public List<UserFavoriteCriteria> Criteria
        {
            get
            {
                if (m_criteria == null)
                {
                    m_criteria = UserFavoriteCriteria.GetForFavorite(this).ToList();
                }
                return m_criteria;
            }
        }

        /// <summary>Gets the criteria matching the provided description.</summary>
        /// <param name="description">Description of the criteria to find.</param>
        /// <returns>Criteria matching the description, or null if no match found.</returns>
        public UserFavoriteCriteria this[string description]
        {
            get
            {
                UserFavoriteCriteria match = null;
                foreach (var c in Criteria)
                {
                    if (StringUtilities.AreEqual(description, c.Description))
                    {
                        match = c;
                        break;
                    }
                }
                return match;
            }
        }
    }
}