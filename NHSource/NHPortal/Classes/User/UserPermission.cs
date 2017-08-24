using GDCoreUtilities;
using PortalFramework.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace NHPortal.Classes.User
{
    /// <summary>Used for storing and accessing values stored in the user permissions table.</summary>
    public static class UserPermissions
    {
        private static UserPermission[] all;
        /// <summary>Gets an array of all user permissions retrieved from the database.</summary>
        public static UserPermission[] All
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

        /// <summary>Gets an array of all active user permissions.</summary>
        public static UserPermission[] AllActive
        {
            get
            {
                return All.Where(p => p.IsActive).ToArray();
            }
        }

        /// <summary>Gets the user permissions from the database to store in memory.</summary>
        public static void Initialize()
        {
            string qry = "SELECT    rup.RUP_PERMISSION_CODE" + Environment.NewLine
                       + "        , rup.RUP_PERMISSION_NAME" + Environment.NewLine
                       + "        , rup.RUP_RUPG_CD" + Environment.NewLine
                       + "        , rup.RUP_ALLOW_READONLY" + Environment.NewLine
                       + "        , rup.RUP_ALLOW_FULLACCESS" + Environment.NewLine
                       + "        , rup.RUP_IS_ACTIVE" + Environment.NewLine
                       + "        , rup.RUP_DISPLAY_ORDER" + Environment.NewLine
                       + "        , rup.RUP_DEFAULT_VALUE" + Environment.NewLine
                       + "FROM    R_USER_PERMISSION rup" + Environment.NewLine
                       + "ORDER BY rup.RUP_PERMISSION_CODE";

            List<UserPermission> usrPermissions = new List<UserPermission>();
            GDDatabaseClient.Oracle.OracleResponse response = ODAP.GetDataTable(qry, DatabaseTarget.Adhoc);
            if (response.Successful)
            {
                foreach (DataRow dr in response.ResultsTable.Rows)
                {
                    usrPermissions.Add(new UserPermission(dr));
                }
            }
            all = usrPermissions.ToArray();
        }

        /// <summary>Finds a user permission.</summary>
        /// <param name="code">Code of the permission to find.</param>
        /// <returns>User permission matching the code, or null if no match was found.</returns>
        public static UserPermission Find(int code)
        {
            UserPermission foundPermission = null;
            foreach (var usrPrm in All)
            {
                if (usrPrm.Code.Equals(code))
                {
                    foundPermission = usrPrm;
                    break;
                }
            }
            return foundPermission;
        }

        /// <summary>Queries for and returns an array of user permissions for a permission group.</summary>
        /// <param name="group">User permission group to retrieve the permissions of.</param>
        /// <returns>Array of permissions for the permission group.</returns>
        public static UserPermission[] GetForGroup(UserPermissionGroup group)
        {
            List<UserPermission> permissions = new List<UserPermission>();
            foreach (UserPermission p in All)
            {
                if (p.PermissionGroupCode.Equals(group.Code))
                {
                    permissions.Add(p);
                }
            }
            return permissions.OrderBy(p => p.DisplayOrder).ThenBy(p => p.Code).ToArray();
        }

        public static UserPermission MainReport { get { return Find(1); } }
        public static UserPermission ModelYearRejection { get { return Find(2); } }
        public static UserPermission StationRejection { get { return Find(3); } }
        public static UserPermission CountyStationRejection { get { return Find(4); } }
        public static UserPermission MechanicRejection { get { return Find(5); } }
        public static UserPermission OBDResults { get { return Find(6); } }
        public static UserPermission FirstTestReset { get { return Find(7); } }
        public static UserPermission SafetyDeficiency { get { return Find(8); } }
        public static UserPermission CallsByHour { get { return Find(9); } }
        public static UserPermission CallsByDuration { get { return Find(10); } }
        public static UserPermission ReasonsAndResolutions { get { return Find(11); } }
        public static UserPermission SuspectTestReport { get { return Find(12); } }
        public static UserPermission StationReport { get { return Find(13); } }
        public static UserPermission AuditHistoryInquiry { get { return Find(14); } }
        public static UserPermission RescheduledAudits { get { return Find(15); } }
        public static UserPermission StickerDenialReport { get { return Find(16); } }
        public static UserPermission InspectionInquiriesBySticker { get { return Find(17); } }
        public static UserPermission StickerRegistry { get { return Find(18); } }
        public static UserPermission QuestionableStickers { get { return Find(19); } }
        public static UserPermission RedeemSoldMismatch { get { return Find(20); } }
        public static UserPermission ComponentPerformance { get { return Find(21); } }
        public static UserPermission StationSupport { get { return Find(22); } }
        public static UserPermission ComponentInventory { get { return Find(23); } }
        public static UserPermission ConsumableInventory { get { return Find(24); } }
        public static UserPermission HistoryReport { get { return Find(25); } }
        
        public static UserPermission MobileTestingTabletByVIN { get { return Find(26); } }
        public static UserPermission InspectionInquiryByVIN { get { return Find(27); } }
        public static UserPermission InspectionInquiryByPlate { get { return Find(28); } }
        public static UserPermission InspectionInquiryByStation { get { return Find(29); } }
        public static UserPermission InspectionInquiryBySticker { get { return Find(30); } }
        public static UserPermission EmissionTestRejectionRates { get { return Find(31); } }
        public static UserPermission OBDIIReadinessMonitor { get { return Find(32); } }
        public static UserPermission OBDIIMILStatus { get { return Find(33); } }
        public static UserPermission OBDIICommunications { get { return Find(34); } }
        public static UserPermission OBDIIDTCErrorCodes { get { return Find(35); } }
        public static UserPermission OBDIIProtocolUsage { get { return Find(36); } }
        public static UserPermission eVINMismatch { get { return Find(37); } }
        public static UserPermission CommunicationProtocol { get { return Find(38); } }
        public static UserPermission Rejection { get { return Find(39); } }
        public static UserPermission ReadinessPatternMismatch { get { return Find(40); } }
        public static UserPermission TimeBeforeTests { get { return Find(41); } }
        public static UserPermission StationSafetyDefect { get { return Find(42); } }
        public static UserPermission NoVoltage { get { return Find(43); } }
        public static UserPermission StationInspectionOverview { get { return Find(44); } }
        public static UserPermission WeightedTriggerScore { get { return Find(45); } }
        public static UserPermission UserMaintenance { get { return Find(46); } }
        public static UserPermission Documents { get { return Find(47); } }
        public static UserPermission ReportBuilderTool { get { return Find(48); } }
        public static UserPermission SQLReportBuilder { get { return Find(49); } }
        public static UserPermission ConsumableSales { get { return Find(50); } }
        public static UserPermission RejectionRateByMake { get { return Find(51); } }
        public static UserPermission RejectionRateByModel { get { return Find(52); } }
        public static UserPermission RejectionRateByModelYear { get { return Find(53); } }
        public static UserPermission RejectionRateByCounty { get { return Find(54); } }
        public static UserPermission RejectionRateByInspector { get { return Find(55); } }
        public static UserPermission RejectionRateByStation { get { return Find(56); } }
        public static UserPermission MonthlyRejectionReport { get { return Find(57); } }
        public static UserPermission NoFinalOutcomeReport { get { return Find(58); } }

    }

    /// <summary>Represents a record from the user permissions reference table.</summary>
    public class UserPermission
    {
        /// <summary>Instantiates a new instance of the UserPermission class.</summary>
        /// <param name="dr">DataRow containing information about the UserPermission.</param>
        public UserPermission(DataRow dr)
        {
            NullSafeRowWrapper wrapper = new NullSafeRowWrapper(dr);
            m_code = wrapper.ToInt("RUP_PERMISSION_CODE");
            Name = wrapper.ToString("RUP_PERMISSION_NAME");
            PermissionGroupCode = wrapper.ToInt("RUP_RUPG_CD");
            AllowReadonly = wrapper.ToBoolean("RUP_ALLOW_READONLY");
            AllowFullAccess = wrapper.ToBoolean("RUP_ALLOW_FULLACCESS");
            IsActive = wrapper.ToBoolean("RUP_IS_ACTIVE");
            DisplayOrder = wrapper.ToInt("RUP_DISPLAY_ORDER");
            DefaultValue = AccessLevels.Find(wrapper.ToString("RUP_DEFAULT_VALUE"));
        }

        private readonly int m_code;
        /// <summary>Gets the identifying code of the permission.  Readonly field.</summary>
        public int Code
        {
            get { return m_code; }
        }

        /// <summary>Gets the description of the permission.</summary>
        public string Name { get; private set; }

        /// <summary>Gets the group code for which the permission belongs to.</summary>
        public int PermissionGroupCode { get; private set; }

        /// <summary>Gets whether or not the permission can be set as read only.</summary>
        public bool AllowReadonly { get; private set; }

        /// <summary>Gets whether or not the permission can be set as full access.</summary>
        public bool AllowFullAccess { get; private set; }

        /// <summary>Gets whether or not the permission is active.</summary>
        public bool IsActive { get; private set; }

        /// <summary>Gets the display order for the permission.</summary>
        public int DisplayOrder { get; private set; }

        /// <summary>Gets the default access level for the permission.</summary>
        public AccessLevel DefaultValue { get; private set; }
    }
}