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
    /// <summary>Holds a collection of permissions for a user.</summary>
    public class PermissionSet : IEnumerable<PortalUserPermission>
    {
        /// <summary>Instantiates a new instance of the PermissionSet class.</summary>
        public PermissionSet()
        {
            m_permissions = new List<PortalUserPermission>();
        }

        /// <summary>Gets an enumerator for the permissions.</summary>
        /// <returns>User permission enumerator.</returns>
        public IEnumerator<PortalUserPermission> GetEnumerator()
        {
            return m_permissions.GetEnumerator();
        }

        /// <summary>Gets an enumerator for the permissions.</summary>
        /// <returns>User permission enumerator.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>Adds a permissions to the permission set.</summary>
        /// <param name="permission">Permission to add.</param>
        public void Add(PortalUserPermission permission)
        {
            m_permissions.Add(permission);
        }

        /// <summary>Returns whether the permission set contains the provided permission.</summary>
        /// <param name="permission">Permission to check.</param>
        /// <returns>True if the set contains the permission, false otherwise.</returns>
        public bool Contains(PortalUserPermission permission)
        {
            return m_permissions.Contains(permission);
        }

        /// <summary>Returns a portal user permission from the permission set.</summary>
        /// <param name="permission">Permission to get.</param>
        /// <returns>PortalUserPermission contained within the set.</returns>
        public PortalUserPermission Get(PortalUserPermission permission)
        {
            PortalUserPermission userPermission = null;
            if (permission != null)
            {
                userPermission = this[permission.Permission.Code];
            }
            return userPermission;
        }



        private List<PortalUserPermission> m_permissions;
        /// <summary>Gets an array of permissions contained in the permission set.</summary>
        public PortalUserPermission[] Permissions
        {
            get { return m_permissions.ToArray(); }
        }

        /// <summary>Gets the permission from the set matching the permission code.</summary>
        /// <param name="permissionCode">Code of the permission to get.</param>
        /// <returns>Permission matching the code, or null if no match found.</returns>
        public PortalUserPermission this[int permissionCode]
        {
            get
            {
                PortalUserPermission permission = null;
                foreach (PortalUserPermission p in m_permissions)
                {
                    if (p.Permission.Code.Equals(permissionCode))
                    {
                        permission = p;
                        break;
                    }
                }
                return permission;
            }
        }
    }

    /// <summary>Represents information about a permission for a user.</summary>
    public class PortalUserPermission
    {
        /// <summary>Instantiates a new instance of the PortalUserPermission class.</summary>
        /// <param name="usr">Reference to the user the permission is associated with.</param>
        /// <param name="dr">DataRow containing information about the permission.</param>
        public PortalUserPermission(PortalUser usr, DataRow dr)
        {
            m_user = usr;
            if (dr != null)
            {
                NullSafeRowWrapper wrapper = new NullSafeRowWrapper(dr);
                SystemNumber = wrapper.ToLong("NHUP_SYS_NO");
                // TODO: do we check m_user here?  Load here and not pass it in as a param?
                m_permission = UserPermissions.Find(wrapper.ToInt("NHUP_RUP_PERMISSION_CODE"));
                AccessLevel = AccessLevels.Find(wrapper.ToString("NHUP_ACCESS_LEVEL"));
                IsActive = wrapper.ToBoolean("NHUP_IS_ACTIVE");
            }
        }

        /// <summary>Instantiates a new instance of the PortaluserPermission class.</summary>
        /// <param name="usr">Reference to the user the permission is associated with.</param>
        /// <param name="permission">Permission to instantiate the user permission for.</param>
        /// <param name="accessLevel">Access level to assign to the user permission.</param>
        public PortalUserPermission(PortalUser usr, UserPermission permission, AccessLevel accessLevel)
        {
            m_user = usr;
            SystemNumber = 0;
            m_permission = permission;
            AccessLevel = accessLevel;
            IsActive = true;
        }

        public bool Save(string savedBy)
        {
            List<OracleParameter> oraParameters = new List<OracleParameter>();
            oraParameters.Add(new OracleParameter("pSysNo", OracleDbType.Int32, 8, SystemNumber, ParameterDirection.InputOutput));
            oraParameters.Add(new OracleParameter("usrSysNo", OracleDbType.Int32, 8, m_user.UserSysNo, ParameterDirection.Input));
            oraParameters.Add(new OracleParameter("pCode", OracleDbType.Int32, 4, Permission.Code, ParameterDirection.Input));
            oraParameters.Add(new OracleParameter("accessLvl", OracleDbType.Varchar2, 1, AccessLevel.Value, ParameterDirection.Input));
            oraParameters.Add(new OracleParameter("isActive", OracleDbType.Varchar2, 1, IsActive ? "Y" : "N", ParameterDirection.Input));
            oraParameters.Add(new OracleParameter("savedBy", OracleDbType.Varchar2, 30, savedBy, ParameterDirection.Input));
            oraParameters.Add(new OracleParameter("rspMsg", OracleDbType.Varchar2, 2000, null, ParameterDirection.Output));
            oraParameters.Add(new OracleParameter("rspCode", OracleDbType.Int32, 4, null, ParameterDirection.Output));

            bool success = false;
            OracleResponse procRsp = PortalFramework.Database.ODAP.CallProcedure("SAVE_USER_PERMISSION", oraParameters.ToArray(), PortalFramework.Database.DatabaseTarget.Adhoc);
            if (NHProcedureUtilities.WasSuccessful(procRsp))
            {
                if (SystemNumber == 0)
                {
                    SystemNumber = NullSafe.ToLong(procRsp.ReturnParameters["pSysNo"]);
                }
                success = true;
            }
            return success;
        }

        /// <summary>Gets an array of permissions associated with a user.</summary>
        /// <param name="sysNo">The unique system number of the user to get permissions for.</param>
        /// <returns>The user's permissions.</returns>
        public static PermissionSet GetForUser(PortalUser usr)
        {
            string qry = "SELECT   up.*" + Environment.NewLine
                       + "FROM     NH_USER_PERMISSION up" + Environment.NewLine
                       + "WHERE    up.NHUP_NHUSR_SYS_NO = " + usr.UserSysNo; //:usrSysNo";

            PermissionSet permissions = new PermissionSet();
            GDDatabaseClient.Oracle.OracleResponse response = ODAP.GetDataTable(qry, DatabaseTarget.Adhoc);
            if (response.Successful)
            {
                foreach (DataRow dr in response.ResultsTable.Rows)
                {
                    permissions.Add(new PortalUserPermission(usr, dr));
                }
            }
            return permissions;
        }

        /// <summary>Gets the unique system number assigned to the user permission.</summary>
        public long SystemNumber { get; private set; }

        private readonly PortalUser m_user;
        /// <summary>Gets the user associated with the permission.</summary>
        public PortalUser User
        {
            get { return m_user; }
        }

        private readonly UserPermission m_permission;
        /// <summary>Gets a reference to a UserPermission object containing information about the permission.</summary>
        public UserPermission Permission
        {
            get { return m_permission; }
        }

        /// <summary>Gets or sets the access level of the permission.</summary>
        public AccessLevel AccessLevel { get; set; }

        /// <summary>Gets whether or not the permission is active.</summary>
        public bool IsActive { get; private set; }
    }
}