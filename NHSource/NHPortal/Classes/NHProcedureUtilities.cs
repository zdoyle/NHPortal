using GDCoreUtilities;
using GDDatabaseClient.Oracle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NHPortal.Classes
{
    /// <summary>Contains methods used to assist with checking the response of a stored procedure call.</summary>
    public static class NHProcedureUtilities
    {
        /// <summary>Returns whether a stored procedure call was successful based on the response object and response code.</summary>
        /// <param name="response">Oracle response from the stored procedure call.</param>
        /// <returns>True if the response was successful, false otherwise.</returns>
        public static bool WasSuccessful(GDDatabaseClient.Oracle.OracleResponse response)
        {
            bool success = (response.Successful && GetResponseCode(response) == 0);
            NHPortalUtilities.LogSessionMessage("Returning [" + success + "] from NHProcedureUtilities.WasSuccessful",
                GDCoreUtilities.Logging.LogSeverity.Information);
            return success;
        }

        /// <summary>Gets the response code returned from the stored procedure.</summary>
        /// <param name="response">Oracle response from the stored procedure call.</param>
        /// <returns>The response code, or -1 if no return parameter match was found.</returns>
        /// <remarks>This method assumes the stored procedure contains an out parameter matching "rspCode".</remarks>
        public static int GetResponseCode(OracleResponse response)
        {
            int code = -1;
            if (response.ReturnParameters.ContainsKey("rspCode"))
            {
                code = NullSafe.ToInt(response.ReturnParameters["rspCode"]);
            }
            NHPortalUtilities.LogSessionMessage("Returning [" + code + "] from NHProcedureUtilities.GetResponseCode", 
                GDCoreUtilities.Logging.LogSeverity.Information);
            return code;
        }

        /// <summary>Gets the response message returned from the stored procedure.</summary>
        /// <param name="response">Oracle response from the stored procedure call.</param>
        /// <returns>The response code, or an empty string if no return parameter match was found.</returns>
        /// <remarks>This method assumes the stored procedure contains an out parameter matching "rspMsg".</remarks>
        public static string GetResponseMessage(OracleResponse response)
        {
            string msg = String.Empty;
            if (response.ReturnParameters.ContainsKey("rspMsg"))
            {
                msg = NullSafe.ToString(response.ReturnParameters["rspMsg"]);
            }
            NHPortalUtilities.LogSessionMessage("Returning [" + msg + "] from NHProcedureUtilities.GetResponseMessage",
                GDCoreUtilities.Logging.LogSeverity.Information);
            return msg;
        }
    }
}