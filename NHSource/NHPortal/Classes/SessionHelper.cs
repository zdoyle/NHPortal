using GDCoreUtilities.Logging;
using NHPortal.Classes.User;
using PortalFramework.MenuModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace NHPortal.Classes
{
    /// <summary>Used to assist in retrieving and storing objects in session.</summary>
    public static class SessionHelper
    {
        /// <summary>Cleans up the user's active session.</summary>
        /// <param name="session">The user's session.</param>
        public static void CleanupUserSession(HttpSessionState session)
        {
            SetPortalUser(session, null);
            SetSessionLogger(session, null);
            SetPortalMenuItems(session, null);
        }

        /// <summary>Stores the collection of portal menu items in session.</summary>
        /// <param name="session">The user's session.</param>
        /// <param name="items">The collection of menu items to store.</param>
        public static void SetPortalMenuItems(HttpSessionState session, PortalMenuItemCollection items)
        {
            session[KeyNames.PORTAL_MENU_ITEMS] = items;
        }

        /// <summary>Gets the top level portal menu items from session.</summary>
        /// <param name="session">The user's session.</param>
        /// <returns>A collection of portal menu items representing the top menu items.</returns>
        public static PortalMenuItemCollection GetPortalMenuItems(HttpSessionState session)
        {
            PortalMenuItemCollection topMenuItems = null;
            if (session != null && session[KeyNames.PORTAL_MENU_ITEMS] != null)
            {
                topMenuItems = session[KeyNames.PORTAL_MENU_ITEMS] as PortalMenuItemCollection;
            }
            return topMenuItems;
        }      

        /// <summary>Stores a Report in session.</summary>
        /// <param name="session">The user's session.</param>
        /// <param name="report">The report to store.</param>
        public static void SetCurrentReport(HttpSessionState session, PortalFramework.ReportModel.Report report)
        {
            session[KeyNames.CURRENT_REPORT] = report;
        }

        /// <summary>Gets the current Report from session.</summary>
        /// <param name="session">The user's session.</param>
        /// <returns>The current report.</returns>
        public static PortalFramework.ReportModel.Report GetCurrentReport(HttpSessionState session)
        {
            return (session[KeyNames.CURRENT_REPORT] as PortalFramework.ReportModel.Report);
        }

        /// <summary>Clears the current report from session.</summary>
        /// <param name="session">The user's session.</param>
        public static void ClearCurrentReport(HttpSessionState session)
        {
            SetCurrentReport(session, null);
        }

        /// <summary>Stores an auxiliary Report in session.</summary>
        /// <param name="session">The user's session.</param>
        /// <param name="report">The auxiliary report to store.</param>
        public static void SetAuxiliaryReport(HttpSessionState session, PortalFramework.ReportModel.Report report)
        {
            session[KeyNames.AUXILIARY_REPORT] = report;
        }

        /// <summary>Gets the current auxiliary Report from session.</summary>
        /// <param name="session">The user's session.</param>
        /// <returns>The current auxiliary report.</returns>
        public static PortalFramework.ReportModel.Report GetAuxiliaryReport(HttpSessionState session)
        {
            return (session[KeyNames.AUXILIARY_REPORT] as PortalFramework.ReportModel.Report);
        }

        /// <summary>Clears the current auxiliary report from session.</summary>
        /// <param name="session">The user's session.</param>
        public static void ClearAuxiliaryReport(HttpSessionState session)
        {
            SetAuxiliaryReport(session, null);
        }

        /// <summary>Stores the welcome page chart container in session.</summary>
        /// <param name="session">The user's session.</param>
        /// <param name="report">The Welcome page chart container.</param>
        public static void SetWelcomeContainer(HttpSessionState session,  GD.Highcharts.GDAnalytics.ChartContainer container)
        {
            session[KeyNames.WELCOME_CONTAINER] = container;
        }

        /// <summary>Gets the Welcome page chart container from session.</summary>
        /// <param name="session">The user's session.</param>
        /// <returns>The Welcome page chart container.</returns>
        public static GD.Highcharts.GDAnalytics.ChartContainer GetWelcomeContainer(HttpSessionState session)
        {
            return (session[KeyNames.WELCOME_CONTAINER] as GD.Highcharts.GDAnalytics.ChartContainer);
        }

        /// <summary>Stores the current portal user in session.</summary>
        /// <param name="session">The user's session.</param>
        /// <param name="user">The user record to store.</param>
        public static void SetPortalUser(HttpSessionState session, PortalUser user)
        {
            session[KeyNames.PORTAL_USER] = user;
        }

        /// <summary>Gets the current portal user from session.</summary>
        /// <param name="session">The user's session.</param>
        /// <returns>The current portal user record.</returns>
        public static PortalUser GetPortalUser(HttpSessionState session)
        {
            PortalUser user = null;
            if (session[KeyNames.PORTAL_USER] != null)
            {
                user = session[KeyNames.PORTAL_USER] as PortalUser;
            }
            return user;
        }



        /// <summary>Stores the file logger in session.</summary>
        /// <param name="session">The user's session.</param>
        /// <param name="logger">ILogger to store.</param>
        public static void SetSessionLogger(HttpSessionState session, ILogger logger)
        {
            session[KeyNames.PORTAL_FILE_LOGGER] = logger;
        }

        /// <summary>Gets the file logger from session.</summary>
        /// <param name="session">The user's session.</param>
        /// <returns>ILogger object.</returns>
        public static ILogger GetSessionLogger(HttpSessionState session)
        {
            ILogger logger = null;
            if (session[KeyNames.PORTAL_FILE_LOGGER] != null)
            {
                logger = session[KeyNames.PORTAL_FILE_LOGGER] as ILogger;
            }
            return logger;
        }


        /// <summary>Stores the currently selected favorite record in session.</summary>
        /// <param name="session">The user's session.</param>
        /// <param name="fav">The favorite record to store.</param>
        public static void SetSelectedFavorite(HttpSessionState session, UserFavorite fav)
        {
            session[KeyNames.SELECTED_FAVORITE] = fav;
        }

        /// <summary>Gets the selected favorite record from session.</summary>
        /// <param name="session">The user's session.</param>
        /// <returns>The favorite record stored for the user.</returns>
        public static UserFavorite GetSelectedFavorite(HttpSessionState session)
        {
            return session[KeyNames.SELECTED_FAVORITE] as UserFavorite;
        }

        /// <summary>Stores the user's favorites in session.</summary>
        /// <param name="session">The user's session.</param>
        /// <param name="favs">Array of the user's favorites.</param>
        public static void SetUserFavorites(HttpSessionState session, UserFavorite[] favs)
        {
            session[KeyNames.USER_FAVORITES] = favs;
        }

        /// <summary>Gets the user's favorites from session.</summary>
        /// <param name="session">The user's session.</param>
        /// <returns>The array of user favorites.</returns>
        public static UserFavorite[] GetUserFavorites(HttpSessionState session)
        {
            return session[KeyNames.USER_FAVORITES] as UserFavorite[];
        }

        /// <summary>Stores the user maintenance mode in session.</summary>
        /// <param name="session">The user's session.</param>
        /// <param name="mode">User maintenance mode to store.</param>
        public static void SetUserMaintenanceMode(HttpSessionState session, UserMaintenanceMode mode)
        {
            session[KeyNames.USER_MAINTENANCE_MODE] = mode;
        }

        /// <summary>Gets the current user maintenance mode from session.</summary>
        /// <param name="session">The user's session.</param>
        /// <returns>User maintenance mode stored in session.</returns>
        public static UserMaintenanceMode GetUserMaintenanceMode(HttpSessionState session)
        {
            return (UserMaintenanceMode)session[KeyNames.USER_MAINTENANCE_MODE];
        }


        /// <summary>Sets the unhandled exception that occurred during the user's session.</summary>
        /// <param name="session">The user's session.</param>
        /// <param name="ex">Exception to store.</param>
        public static void SetSessionException(HttpSessionState session, Exception ex)
        {
            session[KeyNames.SESSION_EXCEPTION] = ex;
        }

        /// <summary>Gets the unhandled session exception from session..</summary>
        /// <param name="session">The user's session.</param>
        /// <returns>The exception stored in session.</returns>
        public static Exception GetSessionException(HttpSessionState session)
        {
            return session[KeyNames.SESSION_EXCEPTION] as Exception;
        }





        /// <summary>Used to store common keys for session variables.</summary>
        public static class KeyNames
        {
            /// <summary>Defines the key for the portal menu.</summary>
            public const string PORTAL_MENU_ITEMS = "portalMenuItems";

            /// <summary>Defines the key for the current report.</summary>
            public const string CURRENT_REPORT = "currentUserReport";

            /// <summary>Defines the key for the an auxiliary report.</summary>
            public const string AUXILIARY_REPORT = "AUX_REPORT";

            /// <summary>Defines the key for the welcome page chart container.</summary>
            public const string WELCOME_CONTAINER = "WELCOME_CONTAINER";

            /// <summary>Defines the key for the portal user.</summary>
            public const string PORTAL_USER = "crntPortalUser";

            /// <summary>Defines the key for the file logger.</summary>
            public const string PORTAL_FILE_LOGGER = "sessionFileLogger";

            /// <summary>Defines the key for the favorite record selected.</summary>
            public const string SELECTED_FAVORITE = "SELECTED_FAVORITE";

            /// <summary>Defines the key for the user's favorite records.</summary>
            public const string USER_FAVORITES = "USER_FAVORITES";

            /// <summary>Defines the key for the user maintenance mode.</summary>
            public const string USER_MAINTENANCE_MODE = "USER_MAINTENANCE_MODE";

            /// <summary>Defines the key for an unhandled session exception.</summary>
            public const string SESSION_EXCEPTION = "UNHANDLED_SESSION_EXCEPTION";
        }
    }
}