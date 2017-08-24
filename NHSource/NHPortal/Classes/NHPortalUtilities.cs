using GDCoreUtilities.Logging;
using PortalFramework.Export;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace NHPortal.Classes
{
    /// <summary>Contains common utilities methods used in the New Hampshire portal.</summary>
    public static class NHPortalUtilities
    {
        private static ILogger m_applicationLogger;

        private static LogSeverity m_minimumLogLevel;

        /// <summary>Gets the root folder of the project.</summary>
        public static string WebRoot { get { return System.AppDomain.CurrentDomain.BaseDirectory; } }

        /// <summary>Gets the directory to use for exports.</summary>
        public static string ExportDirectory
        {
            get { return PortalFramework.PortalIniSettings.Values.Directories.Exports; }
        }



        /// <summary>Static constructor for the NHPortalUtilities class.</summary>
        static NHPortalUtilities()
        {
            Enum.TryParse(PortalFramework.PortalIniSettings.Settings.GetValue("LOG_MIN_LEVEL"), true, out m_minimumLogLevel);
            string path = System.IO.Path.Combine(WebRoot, PortalFramework.PortalIniSettings.Values.Directories.Logs);
            m_applicationLogger = new FileLogger(path, "PortalLog_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".log");
            LogApplicationMessage("Application Logger Initialized", LogSeverity.Information);
        }


        /// <summary>Converts a date time from a string format to another string format.</summary>
        /// <param name="value">The date time value to convert.</param>
        /// <param name="currentFormat">The current date time format.</param>
        /// <param name="newFormat">The format to convert the date time to.</param>
        /// <param name="converted">The output of the conversion.</param>
        /// <returns>True if the conversion was successful, false if not.</returns>
        public static bool TryConvertDateTimeFormat(string value, string currentFormat, string newFormat, out string converted)
        {
            LogSessionMessage(String.Format("Call to TryConvertDateTimeFormat with params [{0}] [{1}] [{2}]", 
                value, currentFormat, newFormat), LogSeverity.Debug);

            bool success = false;
            converted = String.Empty;

            DateTime? dt = GDCoreUtilities.NullSafe.ToNullableDate(value, currentFormat);
            if (dt.HasValue)
            {
                converted = dt.Value.ToString(newFormat);
                if (!String.IsNullOrEmpty(converted))
                {
                    success = true;
                    LogSessionMessage("Set converted to " + converted, LogSeverity.Information);
                }
            }
            LogSessionMessage(String.Format("Return value of TryConvertDateTimeFormat: [{0}]", success), LogSeverity.Debug);
            return success;
        }

        /// <summary>Selects the item in a DropDownList matching the provided value.</summary>
        /// <param name="cbo">Target DropDownList.</param>
        /// <param name="value">Value to select.</param>
        public static void SetComboBoxValue(DropDownList cbo, string value)
        {
            LogSessionMessage(String.Format("Call to SetComboBoxValue with params [{0}] and [{1}]", cbo.ClientID, value), 
                LogSeverity.Information);

            if (cbo != null)
            {
                foreach (ListItem itm in cbo.Items)
                {
                    if (GDCoreUtilities.StringUtilities.AreEqual(itm.Value, value))
                    {
                        cbo.SelectedIndex = cbo.Items.IndexOf(itm);
                        LogSessionMessage(String.Format("Selected item [{0}]", itm.Value), LogSeverity.Information);
                        break;
                    }
                }
            }
        }

        /// <summary>Writes a log to the user's session log.</summary>
        /// <param name="msg">Message to log.</param>
        /// <param name="severity">Severity of the log file to write.</param>
        public static void LogSessionMessage(string msg, LogSeverity severity)
        {
            ILogger logger = SessionHelper.GetSessionLogger(HttpContext.Current.Session);
            if (logger != null && severity >= m_minimumLogLevel)
            {
                //logger.Debug("Current thread ID: {0}", System.Threading.Thread.CurrentThread.ManagedThreadId);
                logger.Log(msg, severity);
            }

            // write the log entry to the application log as well
            LogApplicationMessage(msg, severity);
        }

        /// <summary>Logs an exception to the user's session log.</summary>
        /// <param name="ex">Exception to log.</param>
        public static void LogSessionException(Exception ex)
        {
            LogSessionException(ex, String.Empty);

            // write the exception to the application log as well
            LogApplicationException(ex);
        }

        /// <summary>Logs an exception to the user's session log.</summary>
        /// <param name="ex">Exception to log.</param>
        /// <param name="msg">Additional log message to prepend to the log.</param>
        public static void LogSessionException(Exception ex, string msg)
        {
            ILogger logger = SessionHelper.GetSessionLogger(HttpContext.Current.Session);
            if (logger != null)
            {
                if (!String.IsNullOrEmpty(msg))
                {
                    logger.Debug("Current thread ID: {0}", System.Threading.Thread.CurrentThread.ManagedThreadId);
                    logger.Log(msg, LogSeverity.Error);
                }
                logger.Error("Session Exception: {0}", ex);
                //logger.Log(ex);
            }

            // write the exception to the application log as well
            LogApplicationException(ex, msg);
        }

        /// <summary>Logs a message to the global application logger.</summary>
        /// <param name="msg">Message to log.</param>
        /// <param name="severity">Severity of the message.</param>
        public static void LogApplicationMessage(string msg, LogSeverity severity)
        {
            if (m_applicationLogger != null && severity >= m_minimumLogLevel)
            {
                m_applicationLogger.Log(msg, severity);
            }

            if (System.Diagnostics.Debugger.IsAttached)
            {
                System.Diagnostics.Debug.WriteLine("Call to LogApplicationMessage; params" + Environment.NewLine +
                    "\t{0}" + Environment.NewLine +
                    "\t{1}",
                    msg.Replace(Environment.NewLine, "\t"), severity);
            }
        }

        /// <summary>Logs an exception to the global application logger.</summary>
        /// <param name="ex">Exception to log.</param>
        public static void LogApplicationException(Exception ex)
        {
            if (m_applicationLogger != null)
            {
                m_applicationLogger.Error("Session Exception: {0}", ex);
                //m_applicationLogger.Log(ex);
            }

            if (System.Diagnostics.Debugger.IsAttached)
            {
                string err = (ex == null ? "NULL EXCEPTION OBJECT" : ex.ToString().Replace(Environment.NewLine, "\t"));
                string msg = String.Format("Call to LogApplicationException; params" + Environment.NewLine + "\t{0}", err);

                System.Diagnostics.Debug.WriteLine(msg);
            }
        }

        /// <summary>Logs an exception to the global application logger.</summary>
        /// <param name="ex">Exception to log.</param>
        /// <param name="msg">Additional log message to prepend to the log.</param>
        public static void LogApplicationException(Exception ex, string msg)
        {
            if (m_applicationLogger != null)
            {
                if (!String.IsNullOrEmpty(msg))
                {
                    m_applicationLogger.Log(msg, LogSeverity.Error);
                }
                m_applicationLogger.Error("Session Exception: {0}", ex);
                //m_applicationLogger.Log(ex);
            }

            if (System.Diagnostics.Debugger.IsAttached)
            {
                System.Diagnostics.Debug.WriteLine("Call to LogApplicationException with params '{0}' and '{1}'", ex, msg);
            }
        }

        /// <summary>Exports a report to a CSV file.</summary>
        /// <param name="report">Report to export.</param>
        /// <param name="response">Web response to use to send the file back to the client.</param>
        public static void ExportReportToCsv(PortalFramework.ReportModel.Report report, HttpResponse response)
        {
            if (report != null)
            {
                string filename = report.ExportTitle + "_" + Guid.NewGuid().ToString() + ".csv";
                string fullpath = System.IO.Path.Combine(WebRoot, ExportDirectory, filename);

                ReportExporter.ToCsv(report, fullpath);
                PortalFramework.Utilities.DownloadFile(filename, fullpath, response);
            }
        }

        /// <summary>Exports a report to an Excel file.</summary>
        /// <param name="report">Report to export.</param>
        /// <param name="response">Web response to use to send the file back to the client.</param>
        public static void ExportReportToXLSX(PortalFramework.ReportModel.Report report, HttpResponse response)
        {
            if (report != null)
            {
                string filename = report.ExportTitle + "_" + Guid.NewGuid().ToString() + ".xlsx";
                string fullpath = System.IO.Path.Combine(WebRoot, ExportDirectory, filename);

                ReportExporter.ToXLSX(report, fullpath);
                PortalFramework.Utilities.DownloadFile(filename, fullpath, response);
            }
        }

        /// <summary>Exports a report to a PDF file.</summary>
        /// <param name="report">Report to export.</param>
        /// <param name="response">Web response to use to send the file back to the client.</param>
        public static void ExportReportToPDF(PortalFramework.ReportModel.Report report, HttpResponse response)
        {
            if (report != null)
            {
                string filename = report.ExportTitle + "_" + Guid.NewGuid().ToString() + ".pdf";
                string fullpath = System.IO.Path.Combine(WebRoot, ExportDirectory, filename);

                ReportExporter.ToPDF(report, fullpath);
                PortalFramework.Utilities.DownloadFile(filename, fullpath, response);
            }
        }

        ///// <summary>Creates a directory in the project's directory structure if it does not already exist.</summary>
        ///// <param name="subDirectory">Sub directory to create.</param>
        //public static void EnsureDirectory(string subDirectory)
        //{
        //    if (!String.IsNullOrEmpty(subDirectory))
        //    {
        //        //string directory = Server.MapPath(iniDirectoryValue);
        //        string directory = System.IO.Path.Combine(WebRoot, subDirectory);
        //        if (!System.IO.Directory.Exists(directory))
        //        {
        //            NHPortalUtilities.LogApplicationMessage(String.Format("Directory {0} does not exist; creating.", directory), LogSeverity.Information);
        //            if (GDCoreUtilities.IO.DirectoryHelper.TryCreateDirectory(directory))
        //            {
        //                NHPortalUtilities.LogApplicationMessage(String.Format("Directory {0} successfully created!", directory), LogSeverity.Information);
        //            }
        //            else
        //            {
        //                NHPortalUtilities.LogApplicationMessage(String.Format("Directory {0} creation failed!", directory), LogSeverity.Information);
        //            }
        //        }
        //        else
        //        {
        //            NHPortalUtilities.LogApplicationMessage(String.Format("Directory {0} already exists.", directory), LogSeverity.Information);
        //        }
        //    }
        //}

        /// <summary>Gets the length of the content of the current request.</summary>
        /// <param name="sender">The ASP.NET application.</param>
        /// <returns>Length of the request.</returns>
        public static int GetRequestLength(object sender)
        {
            System.Web.Configuration.HttpRuntimeSection runTime = (System.Web.Configuration.HttpRuntimeSection)System.Web.Configuration.WebConfigurationManager.GetSection("system.web/httpRuntime");
            int maxRequestLength = (runTime.MaxRequestLength - 100) * 1024;
            HttpContext context = ((HttpApplication)sender).Context;

            LogSessionMessage("Request content length: " + context.Request.ContentLength, LogSeverity.Debug);
            return context.Request.ContentLength;
        }

        /// <summary>Builds the CSV file name for a CSV based report.</summary>
        /// <param name="path">Base path of the CSV file.</param>
        /// <param name="prefix">Prefix of the CSV file.</param>
        /// <param name="rptDate">Report date of the file to get.</param>
        /// <returns>The filename for the CSV generated from the provided options.</returns>
        public static string BuildCSVFileName(string path, string prefix, DateTime? rptDate)
        {
            return BuildCSVFileName(path, prefix, rptDate, String.Empty);
        }

        /// <summary>Builds the CSV file name for a CSV based report.</summary>
        /// <param name="path">Base path of the CSV file.</param>
        /// <param name="prefix">Prefix of the CSV file.</param>
        /// <param name="rptDate">Report date of the file to get.</param>
        /// <param name="county">The county of the report.</param>
        /// <returns>The filename for the CSV generated from the provided options.</returns>
        public static string BuildCSVFileName(string path, string prefix, DateTime? rptDate, string county)
        {
            string msg = String.Format("Call to BuildCSVFileName with params [{0}] [{1}] [{2}] [{3}]",
                path, prefix, (rptDate.HasValue ? rptDate.Value.ToString() : "NULL"), county);
            LogSessionMessage(msg, LogSeverity.Debug);

            string fullPath = String.Empty;

            if (rptDate.HasValue)
            {
                string year = rptDate.Value.Year.ToString();
                string month = rptDate.Value.Month.ToString().PadLeft(2, '0');
                string day = rptDate.Value.Day.ToString().PadLeft(2, '0');

                string relativePath = String.Empty;
                if (rptDate.Value == DateTime.Now)
                {
                    relativePath = "RepYr" + year;
                }
                else
                {
                    relativePath = "RepYr" + year + "Mo" + month + "Da" + day;
                }

                string filename = prefix + "_" + relativePath;
                if (!String.IsNullOrEmpty(county))
                {
                    filename += "_County" + county;
                }
                filename += ".csv";

                fullPath = System.IO.Path.Combine(path, relativePath, filename);
            }

            LogSessionMessage(String.Format("Returning [{0}] from BuildCSVFileName", fullPath), LogSeverity.Debug);
            return fullPath;
        }

        /// <summary>Checks whether the provided text contains tags.</summary>
        /// <param name="text">Text to check.</param>
        /// <returns>True if the text contains tags, false otherwise.</returns>
        public static bool ContainsTags(string text)
        {
            bool hasTags = false;
            if (GDCoreUtilities.StringUtilities.ContainsMatchingTags(text) ||
                GDCoreUtilities.StringUtilities.ContainsSingleTags(text))
            {
                hasTags = true;
            }
            return hasTags;
        }

        /// <summary>Pads a user's input text to a station ID.</summary>
        /// <param name="input">Input to pad.</param>
        /// <returns>Station ID.</returns>
        public static string ToStationID(string input)
        {
            LogSessionMessage(String.Format("ToStationID with param [{0}]", input), LogSeverity.Debug);
            string stationId = String.Empty;
            if (!String.IsNullOrWhiteSpace(input))
            {
                input = input.Trim().ToUpper();
                if (GDCoreUtilities.StringUtilities.AreEqual(input, "ALL"))
                {
                    stationId = "ALL";
                }
                else
                {
                    stationId = input.Trim().ToUpper().PadLeft(8, '0');
                }
            }
            LogSessionMessage(String.Format("ToStationID returning [{0}]", stationId), LogSeverity.Debug);
            return stationId;
        }

        /// <summary>Converts a user's input text to an inspector ID.</summary>
        /// <param name="input">Input to convert</param>
        /// <returns>Inspector ID.</returns>
        public static string ToOfficerID(string input)
        {
            LogSessionMessage(String.Format("ToInspectorID with param [{0}]", input), LogSeverity.Debug);

            string inspectorID = String.Empty;
            if (!String.IsNullOrWhiteSpace(input))
            {
                input = input.Trim().ToUpper();
                if (GDCoreUtilities.StringUtilities.AreEqual(input, "ALL"))
                {
                    inspectorID = "ALL";
                }
                else
                {
                    if (input.StartsWith("AD"))
                    {
                        if (input.Length == 10)
                        {
                            inspectorID = input; // assume user input the full inspector ID
                        }
                        else
                        {
                            // user entered "AD" but not a full length of 10 characters, so pad with zeroes
                            inspectorID = "AD" + GDCoreUtilities.StringUtilities.GetSubstring(input, 2).PadLeft(8, '0');
                        }
                    }
                    else
                    {
                        if (input.Length > 7)
                        {
                            input = GDCoreUtilities.StringUtilities.GetSubstring(input, 0, 8);
                        }
                        inspectorID = "AD" + input.PadLeft(8, '0');
                    }
                }
            }
            LogSessionMessage(String.Format("ToInspectorID returning [{0}]", inspectorID), LogSeverity.Debug);
            return inspectorID;
        }

        /// <summary>Calculates the upper or lower boundry of a sticker range based on a provided number inside that range.</summary>
        /// <param name="stickerNumString">The sticker number used to determine a sticker boundry.</param>
        /// <param name="stickerType">The type of the sticker that is being used.</param>
        /// <param name="isStartNumber">Determines if the returned boundry is the start or end number in the sticker range.</param>
        /// <returns>A string containing the upper or lower boundry of a sticker range.</returns>
        public static string GetStickerBoundry(string stickerNumString, string stickerType, bool isStartNumber)
        {
            string returnVal;
            long modVal, boundryDistance;
            long stickerNum = GDCoreUtilities.NullSafe.ToLong(stickerNumString);

            if (stickerType == "A")
            {
                modVal = 25;
            }
            else // Type M
            {
                modVal = 30;
            }

            if (isStartNumber)
            {
                boundryDistance = (stickerNum - 1) % modVal;

                if (boundryDistance == 0)
                {
                    returnVal = stickerNum.ToString();
                }
                else
                {
                    returnVal = (stickerNum - boundryDistance).ToString();
                }
            }
            else
            {
                boundryDistance = modVal - (stickerNum - 1) % modVal;
                returnVal = (stickerNum - 1 + boundryDistance).ToString();
            }

            return returnVal.PadLeft(10, '0');
        }
    }
}