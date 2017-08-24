using GDCoreUtilities;
using PortalFramework.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace NHPortal.Classes.User
{
    /// <summary>Used for storing and accessing values stored in the permission groups table.</summary>
    public static class UserPermissionGroups
    {
        private static UserPermissionGroup[] all;
        /// <summary>Gets an array of all permission groups retrieved from the database.</summary>
        public static UserPermissionGroup[] All
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

        /// <summary>Gets an array of all active permission groups.</summary>
        public static UserPermissionGroup[] AllActive
        {
            get
            {
                return All.Where(grp => grp.IsActive).ToArray();
            }
        }

        /// <summary>Gets the permission groups from the database to store in memory.</summary>
        public static void Initialize()
        {
            string qry = "SELECT    grp.RUPG_CD" + Environment.NewLine
                       + "        , grp.RUPG_DESC" + Environment.NewLine
                       + "        , grp.RUPG_ACTIVE" + Environment.NewLine
                       + "        , grp.RUPG_DISPLAY_ORDER" + Environment.NewLine
                       + "FROM    R_USER_PERMISSION_GROUP grp" + Environment.NewLine
                       + "ORDER BY grp.RUPG_DISPLAY_ORDER, grp.RUPG_CD";

            List<UserPermissionGroup> groups = new List<UserPermissionGroup>();
            GDDatabaseClient.Oracle.OracleResponse response = ODAP.GetDataTable(qry, DatabaseTarget.Adhoc);
            if (response.Successful)
            {
                foreach (DataRow dr in response.ResultsTable.Rows)
                {
                    groups.Add(new UserPermissionGroup(dr));
                }
            }
            all = groups.ToArray();
        }
    }



    /// <summary>Represents a record from the user permission group reference table.</summary>
    public class UserPermissionGroup
    {
        /// <summary>Instantiates a new instance of the UserPermissionGroup class.</summary>
        /// <param name="dr">DataRow containing information about the permission group.</param>
        public UserPermissionGroup(DataRow dr)
        {
            if (dr != null)
            {
                m_code = NullSafe.ToInt(dr["RUPG_CD"]);
                Description = NullSafe.ToString(dr["RUPG_DESC"]);
                IsActive = NullSafe.ToBoolean(dr["RUPG_ACTIVE"]);
                DisplayOrder = NullSafe.ToInt(dr["RUPG_DISPLAY_ORDER"]);
                Permissions = UserPermissions.GetForGroup(this);
            }
        }

        private readonly int m_code;
        /// <summary>Gets the code for the permission group.  Read-only field.</summary>
        public int Code
        {
            get { return m_code; }
        }

        /// <summary>Gets the description of the group.</summary>
        public string Description { get; private set; }

        /// <summary>Gets whether or not the group is active.</summary>
        public bool IsActive { get; private set; }

        /// <summary>Gets the display order for the group.</summary>
        public int DisplayOrder { get; private set; }

        /// <summary>Gets an array of user permissions associated with the group.</summary>
        public UserPermission[] Permissions { get; private set; }

        /// <summary>Gets an array of active user permissions associated with the group.</summary>
        public UserPermission[] ActivePermissions
        {
            get
            {
                return Permissions.Where(p => p.IsActive).ToArray();
            }
        }

        private AccessLevel[] m_availableAccessLevels;
        /// <summary>Gets an array of access levels available for the active permissions contained within the group.</summary>
        public AccessLevel[] AvailableAccessLevels
        {
            get
            {
                if (m_availableAccessLevels == null)
                {
                    List<AccessLevel> levels = new List<AccessLevel>();
                    levels.Add(AccessLevels.None);
                    foreach (var p in ActivePermissions)
                    {
                        if (p.AllowReadonly)
                        {
                            levels.Add(AccessLevels.ReadOnly);
                            break;
                        }
                    }

                    foreach (var p in ActivePermissions)
                    {
                        if (p.AllowFullAccess)
                        {
                            levels.Add(AccessLevels.Full);
                            break;
                        }
                    }
                    m_availableAccessLevels = levels.ToArray();
                }
                return m_availableAccessLevels;
            }
        }
    }
}