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
    /// <summary>Stores information about a single criteria for a user favorite.</summary>
    public class UserFavoriteCriteria
    {
        /// <summary>Instantiates a new instance of the UserFavoriteCriteria class.</summary>
        /// <param name="fav">Reference to the UserFavorite the criteria is for.</param>
        /// <param name="desc">Description to assign to the criteria.</param>
        /// <param name="display">Display text for the criteria.</param>
        /// <param name="value">Value to assign to the criteria.</param>
        public UserFavoriteCriteria(UserFavorite fav, string desc, string display, string value)
        {
            m_favorite = fav;
            Description = desc;
            Display = display;
            Value = value;
        }

        /// <summary>Instantiates a new instance of the UserFavoriteCriteria class.</summary>
        /// <param name="fav">Reference to the UserFavorite the criteria is for.</param>
        /// <param name="dr">DataRow containing information about the criteria record.</param>
        public UserFavoriteCriteria(UserFavorite fav, DataRow dr)
        {
            m_favorite = fav;
            SysNo = GDCoreUtilities.NullSafe.ToLong(dr["UFC_SYS_NO"]);
            Description = GDCoreUtilities.NullSafe.ToString(dr["UFC_DESC"]);
            Display = GDCoreUtilities.NullSafe.ToString(dr["UFC_DISPLAY"]);
            Value = GDCoreUtilities.NullSafe.ToString(dr["UFC_VALUE"]);
        }

        /// <summary>Saves the criteria to the database.</summary>
        /// <param name="userName">Name to use for the created by field.</param>
        /// <returns>True if the save was successful, false otherwise.</returns>
        public bool Save(string userName)
        {
            List<OracleParameter> oraParameters = new List<OracleParameter>();
            oraParameters.Add(new OracleParameter("sysNo", OracleDbType.Int32, 8, SysNo, ParameterDirection.InputOutput));
            oraParameters.Add(new OracleParameter("favSysNo", OracleDbType.Int32, 8, m_favorite.SysNo, ParameterDirection.Input));
            oraParameters.Add(new OracleParameter("critDesc", OracleDbType.Varchar2, 30, Description, ParameterDirection.Input));
            oraParameters.Add(new OracleParameter("critDisplay", OracleDbType.Varchar2, 4000, Display, ParameterDirection.Input));
            oraParameters.Add(new OracleParameter("critValue", OracleDbType.Varchar2, 4000, Value, ParameterDirection.Input));
            oraParameters.Add(new OracleParameter("createUsr", OracleDbType.Varchar2, 30, userName, ParameterDirection.Input));
            oraParameters.Add(new OracleParameter("rspMsg", OracleDbType.Varchar2, 2000, null, ParameterDirection.Output));
            oraParameters.Add(new OracleParameter("rspCode", OracleDbType.Int32, 4, null, ParameterDirection.Output));

            OracleResponse procRsp = PortalFramework.Database.ODAP.CallProcedure("CREATE_USER_FAVORITE_CRITERIA", oraParameters.ToArray(), PortalFramework.Database.DatabaseTarget.Adhoc);
            if (NHProcedureUtilities.WasSuccessful(procRsp))
            {
                SysNo = GDCoreUtilities.NullSafe.ToInt(procRsp.ReturnParameters["sysNo"]);
            }
            return procRsp.Successful;
        }

        /// <summary>Gets the criteria for a favorite record.</summary>
        /// <param name="fav">The favorite record.</param>
        /// <returns>Array of criteria for the favorite record.</returns>
        public static UserFavoriteCriteria[] GetForFavorite(UserFavorite fav)
        {
            string qry = "SELECT  ufc.UFC_SYS_NO, ufc.UFC_DESC, ufc.UFC_DISPLAY, ufc.UFC_VALUE" + Environment.NewLine
                       + "FROM    nh_user_favorite_criteria ufc" + Environment.NewLine
                       + "WHERE   ufc.ufc_fav_sys_no = " + fav.SysNo; // :favSysNo";
            
            // TODO use bind variable

            List<UserFavoriteCriteria> criteria = new List<UserFavoriteCriteria>();
            OracleResponse rsp = ODAP.GetDataTable(qry, DatabaseTarget.Adhoc);
            if (rsp.Successful)
            {
                foreach (DataRow dr in rsp.ResultsTable.Rows)
                {
                    criteria.Add(new UserFavoriteCriteria(fav, dr));
                }
            }
            return criteria.ToArray();
        }


        /// <summary>Gets the unique system number assigned to the criteria.</summary>
        public long SysNo { get; private set; }

        private readonly UserFavorite m_favorite;
        /// <summary>Gets a reference to the UserFavorite object the criteria is for.</summary>
        public UserFavorite FavoriteSysNo
        {
            get { return m_favorite; }
        }

        /// <summary>Gets the description of the criteria.</summary>
        public string Description { get; private set; }

        /// <summary>Gets the display text of the criteria.</summary>
        public string Display { get; private set; }

        /// <summary>Gets the value of the criteria.</summary>
        public string Value { get; private set; }
    }
}