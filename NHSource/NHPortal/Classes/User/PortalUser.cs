using GDCoreUtilities;
using GDCoreUtilities.Helpers;
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
    /// <summary>Used for storing and retrieving user records.</summary>
    public static class PortalUsers
    {
        /// <summary>Retrieves all active users from the database and stores the results in memory.</summary>
        public static void Initialize()
        {
            all = new List<PortalUser>();

            string qry = "SELECT   ur.*" + Environment.NewLine
                       + "FROM     NH_USER_ACCOUNT ur";

            OracleResponse response = ODAP.GetDataTable(qry, DatabaseTarget.Adhoc);
            if (response.Successful)
            {
                foreach (DataRow dr in response.ResultsTable.Rows)
                {
                    all.Add(new PortalUser(dr));
                }
            }
        }

        /// <summary>Verifies a portal user's existence in the system.</summary>
        /// <param name="usrName">User name of the user to verify their existence.</param>
        /// <returns>Returns true if the portal user is found, otherwise returns false.</returns>
        public static bool UserExists(string usrName)
        {
            PortalUser foundUser = Find(usrName);
            return foundUser != null;
        }

        /// <summary>Verifies a portal user's existence in the system.</summary>
        /// <param name="sysNo">System number of the user to verify their existence.</param>
        /// <returns>Returns true if the portal user is found, otherwise returns false.</returns>
        public static bool UserExists(long sysNo)
        {
            PortalUser foundUser = Find(sysNo);
            return foundUser != null;
        }

        /// <summary>Finds and returns the PortalUser with the user system number provided.</summary>
        /// <param name="sysNo">System number of the user to find.</param>
        /// <returns>User matching the provided system number, or null if no match found.</returns>
        public static PortalUser Find(long sysNo)
        {
            PortalUser foundUser = null;
            foreach (PortalUser user in All)
            {
                if (user.UserSysNo.Equals(sysNo))
                {
                    foundUser = user;
                    break;
                }
            }
            return foundUser;
        }

        /// <summary>Finds and returns the PortalUser with the user system number provided.</summary>
        /// <param name="usrName">User name of the user to find.</param>
        /// <returns>User matching the provided user name, or null if no match found.</returns>
        public static PortalUser Find(string usrName)
        {
            PortalUser foundUser = null;
            foreach (PortalUser user in All)
            {
                if (StringUtilities.AreEqual(usrName, user.UserName))
                {
                    foundUser = user;
                    break;
                }
            }
            return foundUser;
        }



        private static List<PortalUser> all;
        /// <summary>Gets an array of all users in the system.</summary>
        public static PortalUser[] All
        {
            get
            {
                if (all == null)
                {
                    Initialize();
                }
                return all.ToArray();
            }
        }

        /// <summary>Gets an array of all users in the system, ordered by last name.</summary>
        public static PortalUser[] AllByLastName
        {
            get
            {
                return All.OrderBy(u => u.LastName.Trim().ToUpper()).ToArray();
            }
        }
    }



    /// <summary>Represents a user in the New Hampshire Portal.</summary>
    public class PortalUser
    {
        private const int PASSWORD_EXPIRATION_LENGTH = 90;

        /// <summary>Instantiates a new instance of the PortalUser class</summary>
        /// <param name="userName">user name to assign to the user.</param>
        public PortalUser(string userName)
        {
            UserSysNo = 0;
            UserName = userName;
            Password = StringUtilities.GetRandomCharString(3)
                     + StringUtilities.GetRandomNumberString(3);
            Password += StringUtilities.GetRandomCharString(3).ToUpper();
            LastPasswordChange = null;
            FirstName = String.Empty;
            LastName = String.Empty;
            StreetAddress = String.Empty;
            AddressLineTwo = String.Empty;
            City = String.Empty;
            State = String.Empty;
            ZipCode = String.Empty;
            County = String.Empty;
            Company = String.Empty;
            PhoneNumber = String.Empty;
            Email = String.Empty;
            LastLogin = null;
            AcceptedEULA = false;
            AcceptedEULADate = null;
            IsLocked = false;
            IsActive = true;
            //Permissions = new PermissionSet();
        }

        /// <summary>Instantiates a new instance of the PortalUser class</summary>
        /// <param name="dr">DataRow containing information about the user record.</param>
        public PortalUser(DataRow dr)
        {
            if (dr != null)
            {
                NullSafeRowWrapper wrap = new NullSafeRowWrapper(dr);
                UserSysNo = wrap.ToInt("NHUSR_SYS_NO");
                UserName = wrap.ToString("NHUSR_USERNAME");
                Password = wrap.ToString("NHUSR_PASSWORD");
                LastPasswordChange = wrap.ToNullableDate("NHUSR_LAST_PASSWORD_CHANGE_DT");
                FirstName = wrap.ToString("NHUSR_FIRST_NAME");
                LastName = wrap.ToString("NHUSR_LAST_NAME");
                StreetAddress = wrap.ToString("NHUSR_STREET");
                AddressLineTwo = wrap.ToString("NHUSR_STREET_LINE_2");
                City = wrap.ToString("NHUSR_CITY");
                State = wrap.ToString("NHUSR_STATE");
                ZipCode = wrap.ToString("NHUSR_ZIPCODE");
                County = wrap.ToString("NHUSR_COUNTY");
                Company = wrap.ToString("NHUSR_COMPANY");
                PhoneNumber = wrap.ToString("NHUSR_PHONE");
                Email = wrap.ToString("NHUSR_EMAIL");
                LastLogin = wrap.ToNullableDate("NHUSR_LAST_LOGIN");
                AcceptedEULA = wrap.ToBoolean("NHUSR_EULA_ACCEPT");
                AcceptedEULADate = wrap.ToNullableDate("NHUSR_EULA_ACCEPT_DT");
                IsLocked = wrap.ToBoolean("NHUSR_IS_LOCKED");
                IsActive = wrap.ToBoolean("NHUSR_IS_ACTIVE");
            }

            //Permissions = PortalUserPermission.GetForUser(this);
        }

        /// <summary>Saves the user record to an Oracle database.</summary>
        /// <param name="savedBy">User name of the user saving the user record.</param>
        /// <returns>True if the user's information was successfully saved, false otherwise.</returns>
        public bool Save(string savedBy)
        {
            List<OracleParameter> oraParameters = new List<OracleParameter>();
            oraParameters.Add(new OracleParameter("usrSysNo", OracleDbType.Int32, 8, UserSysNo, ParameterDirection.InputOutput));
            oraParameters.Add(new OracleParameter("username", OracleDbType.Varchar2, 30, UserName, ParameterDirection.Input));
            oraParameters.Add(new OracleParameter("usrPasswd", OracleDbType.Varchar2, 30, Password, ParameterDirection.Input));
            oraParameters.Add(new OracleParameter("firstName", OracleDbType.Varchar2, 30, FirstName, ParameterDirection.Input));
            oraParameters.Add(new OracleParameter("lastName", OracleDbType.Varchar2, 30, LastName, ParameterDirection.Input));
            oraParameters.Add(new OracleParameter("addrStreet", OracleDbType.Varchar2, 50, StreetAddress, ParameterDirection.Input));
            oraParameters.Add(new OracleParameter("addrLineTwo", OracleDbType.Varchar2, 50, AddressLineTwo, ParameterDirection.Input));
            oraParameters.Add(new OracleParameter("addrCity", OracleDbType.Varchar2, 25, City, ParameterDirection.Input));
            oraParameters.Add(new OracleParameter("addrState", OracleDbType.Varchar2, 2, State, ParameterDirection.Input));
            oraParameters.Add(new OracleParameter("addrZip", OracleDbType.Varchar2, 5, ZipCode, ParameterDirection.Input));
            oraParameters.Add(new OracleParameter("addrCounty", OracleDbType.Varchar2, 20, County, ParameterDirection.Input));
            oraParameters.Add(new OracleParameter("company", OracleDbType.Varchar2, 30, Company, ParameterDirection.Input));
            oraParameters.Add(new OracleParameter("phone", OracleDbType.Varchar2, 10, PhoneNumber, ParameterDirection.Input));
            oraParameters.Add(new OracleParameter("email", OracleDbType.Varchar2, 50, Email, ParameterDirection.Input));
            oraParameters.Add(new OracleParameter("isActive", OracleDbType.Varchar2, 1, IsActive ? "Y" : "N", ParameterDirection.Input));
            oraParameters.Add(new OracleParameter("saveLogin", OracleDbType.Varchar2, 30, savedBy, ParameterDirection.Input));
            oraParameters.Add(new OracleParameter("rspMsg", OracleDbType.Varchar2, 2000, null, ParameterDirection.Output));
            oraParameters.Add(new OracleParameter("rspCode", OracleDbType.Int32, 4, null, ParameterDirection.Output));

            bool success = false;
            OracleResponse procRsp = PortalFramework.Database.ODAP.CallProcedure("SAVE_USER_RECORD", oraParameters.ToArray(), PortalFramework.Database.DatabaseTarget.Adhoc);
            if (NHProcedureUtilities.WasSuccessful(procRsp))
            {
                if (UserSysNo == 0)
                {
                    UserSysNo = GDCoreUtilities.NullSafe.ToInt(procRsp.ReturnParameters["usrSysNo"]);
                }
                success = SavePermissions(savedBy);
            }
            return success;
        }

        private bool SavePermissions(string savedBy)
        {
            bool success = false;
            foreach (PortalUserPermission permission in Permissions)
            {
                success = permission.Save(savedBy);
                if (!success)
                {
                    break;
                }
            }
            return success;
        }

        /// <summary>Updates a user's password and last password change date in an Oracle database.</summary>
        /// <param name="newPassword">The user's new password.</param>
        /// <param name="savedBy">User name of the user updating the password.</param>
        /// <returns>True if the user successfully updated their password, false otherwise.</returns>
        public bool UpdatePassword(string newPassword, string savedBy)
        {
            List<OracleParameter> oraParameters = new List<OracleParameter>();
            oraParameters.Add(new OracleParameter("usrSysNo", OracleDbType.Int32, 8, UserSysNo, ParameterDirection.InputOutput));
            oraParameters.Add(new OracleParameter("newPass", OracleDbType.Varchar2, 30, newPassword, ParameterDirection.Input));
            oraParameters.Add(new OracleParameter("updateLogin", OracleDbType.Varchar2, 30, savedBy, ParameterDirection.Input));
            oraParameters.Add(new OracleParameter("rspMsg", OracleDbType.Varchar2, 2000, null, ParameterDirection.Output));
            oraParameters.Add(new OracleParameter("rspCode", OracleDbType.Int32, 4, null, ParameterDirection.Output));

            bool success = false;
            OracleResponse procRsp = PortalFramework.Database.ODAP.CallProcedure("UPDATE_USER_PASSWORD", oraParameters.ToArray(), PortalFramework.Database.DatabaseTarget.Adhoc);
            if (NHProcedureUtilities.WasSuccessful(procRsp))
            {
                LastPasswordChange = DateTime.Now;
                Password = newPassword;
                success = true;
            }
            return success;
        }

        /// <summary>Adds or updates a user's permission.</summary>
        /// <param name="permission">Permission to add or update.</param>
        /// <param name="accessLevel">Access level to set the permission to.</param>
        public void SetPermission(UserPermission permission, AccessLevel accessLevel)
        {
            PortalUserPermission userPermission = Permissions[permission.Code];
            if (userPermission == null)
            {
                Permissions.Add(new PortalUserPermission(this, permission, accessLevel));
            }
            else
            {
                userPermission.AccessLevel = accessLevel;
            }
        }

        /// <summary>Returns whether or not a user has read access for the provided permission.</summary>
        /// <param name="permissionCode">Code of the permission to check.</param>
        /// <returns>True if the user has read access, false otherwise.</returns>
        public bool HasReadAccess(int permissionCode)
        {
            bool canRead = false;
            PortalUserPermission p = Permissions[permissionCode];
            if (p != null)
            {
                if (p.AccessLevel == AccessLevels.ReadOnly || p.AccessLevel == AccessLevels.Full)
                {
                    canRead = true;
                }
            }
            return canRead;
        }

        /// <summary>Returns whether or not a user has full access for the provided permission.</summary>
        /// <param name="permissionCode">Code of the permission to check.</param>
        /// <returns>True if the user has full access, false otherwise.</returns>
        public bool HasFullAccess(int permissionCode)
        {
            bool fullAccess = false;
            PortalUserPermission p = Permissions[permissionCode];
            if (p != null)
            {
                if (p.AccessLevel == AccessLevels.Full)
                {
                    fullAccess = true;
                }
            }
            return fullAccess;
        }

        /// <summary>Attempts to log a user into the system.</summary>
        /// <param name="usrName">User name of the user to log in.</param>
        /// <param name="passwd">Password of the user to log in.</param>
        /// <param name="user">Out parameter containing the user's information if login was successful.</param>
        /// <returns>True if the user was successfully logged in, false otherwise.</returns>
        public static bool TryLogin(string usrName, string passwd, out PortalUser user)
        {
            string q = "SELECT   ur.*" + Environment.NewLine
                     + "FROM     NH_USER_ACCOUNT ur" + Environment.NewLine
                     + "WHERE    UPPER( TRIM( ur.NHUSR_USERNAME ) ) = UPPER( TRIM( :usrNm ) )" + Environment.NewLine
                     + "AND      ur.NHUSR_PASSWORD = :passwd" + Environment.NewLine
                     + "AND      ur.NHUSR_IS_ACTIVE = 'Y'" + Environment.NewLine
                     + "AND      ur.NHUSR_IS_LOCKED = 'N'";

            OracleCommandInfo cmdInfo = ODAP.BuildOracleCommand(q, DatabaseTarget.Adhoc);
            cmdInfo.AddParameter(usrName, Oracle.DataAccess.Client.OracleDbType.Varchar2, ParameterDirection.Input);
            cmdInfo.AddParameter(passwd, Oracle.DataAccess.Client.OracleDbType.Varchar2, ParameterDirection.Input);

            user = null;
            OracleResponse response = ODAP.GetDataRow(cmdInfo);
            if (response.Successful)
            {
                user = new PortalUser(response.ResultsRow);
            }
            return (user != null && user.UserSysNo > 0);
        }

        /// <summary>Updates the locked status of the user record.</summary>
        /// <returns>True if the record was updated successfully, false otherwise.</returns>
        public bool UpdateLockedStatus(bool setLocked)
        {
            List<OracleParameter> oraParameters = new List<OracleParameter>();
            oraParameters.Add(new OracleParameter("usrName", OracleDbType.Varchar2, 30, UserName, ParameterDirection.Input));
            oraParameters.Add(new OracleParameter("setLocked", OracleDbType.Varchar2, 1, (setLocked ? "Y" : "N"), ParameterDirection.Input));
            oraParameters.Add(new OracleParameter("rspMsg", OracleDbType.Varchar2, 2000, null, ParameterDirection.Output));
            oraParameters.Add(new OracleParameter("rspCode", OracleDbType.Int32, 4, null, ParameterDirection.Output));

            bool success = false;
            OracleResponse procRsp = PortalFramework.Database.ODAP.CallProcedure("UPDATE_PORTAL_USER_LOCK_STATUS", oraParameters.ToArray(), PortalFramework.Database.DatabaseTarget.Adhoc);
            if (NHProcedureUtilities.WasSuccessful(procRsp))
            {
                IsLocked = setLocked;
                success = true;
            }
            return success;
        }

        /// <summary>Gets a user record by a user's system number.</summary>
        /// <param name="sysNo">System number of the user to retrieve.</param>
        /// <param name="userRecord">Out parameter containing information about the user.</param>
        /// <returns>True if the user record was found, false otherwise.</returns>
        public static bool TryGetBySysNo(int sysNo, out PortalUser userRecord)
        {
            string q = "SELECT   ur.*" + Environment.NewLine
                     + "FROM     NH_USER_ACCOUNT ur" + Environment.NewLine
                     + "WHERE    ur.NHUSR_SYS_NO = :sysNo";

            OracleCommandInfo cmdInfo = ODAP.BuildOracleCommand(q, DatabaseTarget.Adhoc);
            cmdInfo.AddParameter(sysNo, Oracle.DataAccess.Client.OracleDbType.Int64, ParameterDirection.Input);

            userRecord = null;
            OracleResponse response = ODAP.GetDataRow(cmdInfo);
            if (response.Successful)
            {
                userRecord = new PortalUser(response.ResultsRow);
            }
            return (userRecord != null && userRecord.UserSysNo > 0);
        }



        private PermissionSet m_permissions;
        /// <summary>Gets the permissions associated with the user.</summary>
        public PermissionSet Permissions
        { 
            get
            {
                if (m_permissions == null)
                {
                    m_permissions = PortalUserPermission.GetForUser(this);
                }
                return m_permissions;
            }
        }

        /// <summary>Gets the unique system number assigned to the user.</summary>
        public long UserSysNo { get; private set; }

        /// <summary>Gets the user's user name.</summary>
        public string UserName { get; private set; }

        /// <summary>Gets the user's password.</summary>
        public string Password { get; private set; }

        /// <summary>Gets the date of the user's last password change.</summary>
        public DateTime? LastPasswordChange { get; private set; }

        /// <summary>Gets the number of days until the user's password is expired.</summary>
        public int DaysUntilPasswordExpires
        {
            get
            {
                int days = 0;
                if (LastPasswordChange.HasValue)
                {
                    TimeSpan diff = DateTime.Now - LastPasswordChange.Value;
                    days = (int)diff.TotalDays;
                }
                else
                {
                    days = PASSWORD_EXPIRATION_LENGTH + 1;
                }
                return PASSWORD_EXPIRATION_LENGTH - days;
            }
        }

        /// <summary>Gets whether or not the user is required to update their password.</summary>
        public bool PasswordUpdateRequired
        {
            get { return (DaysUntilPasswordExpires <= 0); }
        }

        /// <summary>Gets or sets the user's first name.</summary>
        public string FirstName { get; set; }

        /// <summary>Gets or sets the user's last name.</summary>
        public string LastName { get; set; }

        /// <summary>Gets the user's name in title case in the format of "Last Name, First Name".</summary>
        public string Name
        {
            get
            {
                return StringUtilities.ToTitleCase(LastName.Trim()) + ", " + StringUtilities.ToTitleCase(FirstName.Trim());
            }
        }

        /// <summary>Gets or sets the user's street address.</summary>
        public string StreetAddress { get; set; }

        /// <summary>Gets or sets the user's line two address.</summary>
        public string AddressLineTwo { get; set; }

        /// <summary>Gets or sets the user's city.</summary>
        public string City { get; set; }

        /// <summary>Gets or sets the user's state.</summary>
        public string State { get; set; }

        /// <summary>Gets or sets the user's zip code.</summary>
        public string ZipCode { get; set; }

        /// <summary>Gets or sets the user's county.</summary>
        public string County { get; set; }

        /// <summary>Gets or sets the user's company.</summary>
        public string Company { get; set; }

        /// <summary>Gets or sets the user's phone number.</summary>
        public string PhoneNumber { get; set; }

        /// <summary>Get's or sets the user's email address.</summary>
        public string Email { get; set; }

        /// <summary>Gets the date and time of user's last login.</summary>
        public DateTime? LastLogin { get; private set; }

        /// <summary>Gets whether or not the user has accepted the EULA.</summary>
        public bool AcceptedEULA { get; private set; }

        /// <summary>Gets the date and time of when the user accepted the EULA.</summary>
        public DateTime? AcceptedEULADate { get; private set; }

        /// <summary>Gets whether or not the user's account is locked.</summary>
        public bool IsLocked { get; private set; }

        /// <summary>Gets or sets the number of login attempts for the user.</summary>
        public int LoginAttemptCount { get; set; }

        /// <summary>Gets or sets whether or not the user's account is active.</summary>
        public bool IsActive { get; set; }
    }
}