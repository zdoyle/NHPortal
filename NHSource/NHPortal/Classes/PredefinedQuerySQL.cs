using NHPortal.Classes.Adhoc;
using PortalFramework.ReportModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NHPortal.Classes
{
    public static class PredefinedQuerySQL
    {
        public static string GetPredefinedSQL(PredefinedQueryType queryType, string whereClauseItem)
        {
            string returnVal;
            string queryTypeValue = queryType.ToString().ToUpper();
            string whereClauseValue = CreateWhereClause(queryTypeValue, whereClauseItem);

            returnVal = String.Format(RejectionBySQL, queryTypeValue, whereClauseValue);

            if (queryType == PredefinedQueryType.ModelYear)
            {
                returnVal += " DESC";
            }

            return returnVal;
        }

        private static string CreateWhereClause(string queryTypeValue, string input)
        {
            string[] searchParts;
            string whereClause;

            if (input == string.Empty)
            {
                // No Where clause needed.
                whereClause = String.Empty;
            }
            else if (queryTypeValue == "MODELYEAR" || queryTypeValue == "COUNTY")
            {
                if (input.Contains(','))
                {
                    whereClause = String.Format("WHERE {0} IN ( {1} )", queryTypeValue, input);
                }
                else
                {
                    whereClause = String.Format("WHERE {0} =  '{1}' ", queryTypeValue, input);
                }
            }
            else
            {
                // make it where you only have to check for % in the terms.
                input = input.Replace('*', '%');

                searchParts = input.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                whereClause = "WHERE ";

                // Pad station or inspector ids if they are not wildcard searches.
                if (queryTypeValue == "STATIONID")
                {
                    for (int i = 0; i < searchParts.Length; i++)
                    {
                        if (!searchParts[i].Contains('%'))
                        {
                            searchParts[i] = searchParts[i].PadLeft(8, '0');
                        }
                    }
                }
                else if (queryTypeValue == "INSPECTORID")
                {
                    for (int i = 0; i < searchParts.Length; i++)
                    {
                        if (!searchParts[i].Contains('%'))
                        {
                            searchParts[i] = searchParts[i].PadLeft(10, '0');
                        }
                    }
                }

                int partsLength = searchParts.Length;
                int count = 0;
                foreach (string part in searchParts)
                {
                    if (part.Contains('%'))
                    {
                        //Wild Card
                        whereClause += "TRIM(" + queryTypeValue + ") LIKE ( '" + part.Trim().ToUpper() + "' ) " + Environment.NewLine;
                    }
                    else
                    {
                        // Equality
                        whereClause += queryTypeValue + " = '" + part.Trim().ToUpper() + "' " + Environment.NewLine;
                    }

                    count++;

                    // Seperate multiple terms with OR since LIKE will not work with IN() statement.
                    if (count < partsLength)
                    {
                        whereClause += " OR " + Environment.NewLine;
                    }
                }
            }

            return whereClause;
        }

        public static readonly string RejectionBySQL = "SELECT {0} MAIN_COLUMN ," + Environment.NewLine
              + "SUM (OVERALL_COUNT) TOTAL," + Environment.NewLine
              + "SUM (OVERALL_PASS_COUNT) PASS," + Environment.NewLine
              + "TO_CHAR(ROUND (" + Environment.NewLine
              + "           DECODE (" + Environment.NewLine
              + "              SUM (OVERALL_COUNT)," + Environment.NewLine
              + "              0," + Environment.NewLine
              + "              0," + Environment.NewLine
              + "              ( (SUM (OVERALL_PASS_COUNT) / SUM (OVERALL_COUNT))" + Environment.NewLine
              + "               * 100)" + Environment.NewLine
              + "           )," + Environment.NewLine
              + "           3" + Environment.NewLine
              + "        ))" + Environment.NewLine
              + "|| '%'" + Environment.NewLine
              + "   PASS_PCT," + Environment.NewLine
              + "SUM (OVERALL_REJECT_COUNT) REJECT," + Environment.NewLine
              + "TO_CHAR(ROUND (" + Environment.NewLine
              + "           DECODE (" + Environment.NewLine
              + "              SUM (OVERALL_COUNT)," + Environment.NewLine
              + "              0," + Environment.NewLine
              + "              0," + Environment.NewLine
              + "              ( (SUM (OVERALL_REJECT_COUNT) / SUM (OVERALL_COUNT))" + Environment.NewLine
              + "               * 100)" + Environment.NewLine
              + "           )," + Environment.NewLine
              + "           3" + Environment.NewLine
              + "        ))" + Environment.NewLine
              + "|| '%'" + Environment.NewLine
              + "   REJ_PCT," + Environment.NewLine
              + "SUM (OVERALL_CORRECTED_COUNT) CORRECTED," + Environment.NewLine
              + "TO_CHAR(ROUND (" + Environment.NewLine
              + "           DECODE (" + Environment.NewLine
              + "              SUM (OVERALL_COUNT)," + Environment.NewLine
              + "              0," + Environment.NewLine
              + "              0," + Environment.NewLine
              + "              ( (SUM (OVERALL_CORRECTED_COUNT)" + Environment.NewLine
              + "                 / SUM (OVERALL_COUNT))" + Environment.NewLine
              + "               * 100)" + Environment.NewLine
              + "           )," + Environment.NewLine
              + "           3" + Environment.NewLine
              + "        ))" + Environment.NewLine
              + "|| '%'" + Environment.NewLine
              + "   CRTD_PCT," + Environment.NewLine
              + "SUM (OVERALL_ABORTED_COUNT) ABORTED," + Environment.NewLine
              + "TO_CHAR(ROUND (" + Environment.NewLine
              + "           DECODE (" + Environment.NewLine
              + "              SUM (OVERALL_COUNT)," + Environment.NewLine
              + "              0," + Environment.NewLine
              + "              0," + Environment.NewLine
              + "              ( (SUM (OVERALL_ABORTED_COUNT)" + Environment.NewLine
              + "                 / SUM (OVERALL_COUNT))" + Environment.NewLine
              + "               * 100)" + Environment.NewLine
              + "           )," + Environment.NewLine
              + "           3" + Environment.NewLine
              + "        ))" + Environment.NewLine
              + "|| '%'" + Environment.NewLine
              + "   ABORT_PCT" + Environment.NewLine
              + "FROM   PMV_PARAM_FAILURES" + Environment.NewLine
              + " {1}" + Environment.NewLine
              + "GROUP BY {0}" + Environment.NewLine
              + "ORDER BY {0}";
    }

    /// <summary>Defines the column information for different selections in a detailed weighted trigger report.</summary>
    public static class PredefinedQueryReportHeader
    {
        public static Dictionary<string, ReportColumnInfo> MainColumns { get; private set; }

        static PredefinedQueryReportHeader()
        {
            MainColumns = new Dictionary<string, ReportColumnInfo>();

            MainColumns.Add("TOTAL", new ReportColumnInfo("Total", ColumnDataType.Number));
            MainColumns.Add("PASS", new ReportColumnInfo("Pass", ColumnDataType.Number));
            MainColumns.Add("PASS_PCT", new ReportColumnInfo("Pass%", ColumnDataType.Percentage));
            MainColumns.Add("REJECT", new ReportColumnInfo("Reject", ColumnDataType.Number));
            MainColumns.Add("REJ_PCT", new ReportColumnInfo("Reject%", ColumnDataType.Percentage));
            MainColumns.Add("CORRECTED", new ReportColumnInfo("Corrected", ColumnDataType.Number));
            MainColumns.Add("CRTD_PCT", new ReportColumnInfo("Corrected%", ColumnDataType.Percentage));
            MainColumns.Add("ABORTED", new ReportColumnInfo("Aborted", ColumnDataType.Number));
            MainColumns.Add("ABORTED_PCT", new ReportColumnInfo("Aborted%", ColumnDataType.Percentage));
        }
    }

    public static class MonthlyRejectionHeaders
    {
        public static Dictionary<string, ReportColumnInfo> MainColumns { get; private set; }

        static MonthlyRejectionHeaders()
        {
            MainColumns = new Dictionary<string, ReportColumnInfo>();

            MainColumns.Add("VIN", new ReportColumnInfo("VIN"));
            MainColumns.Add("MAKE", new ReportColumnInfo("Make"));
            MainColumns.Add("MODEL", new ReportColumnInfo("Model"));
            MainColumns.Add("MODELYR", new ReportColumnInfo("Model Year"));
            MainColumns.Add("STATIONID", new ReportColumnInfo("Station ID"));
            MainColumns.Add("MECHANICID", new ReportColumnInfo("Mechanic ID"));
            MainColumns.Add("TESTDATETIME", new ReportColumnInfo("Test Date & Time"));
            MainColumns.Add("AIRPUMP", new ReportColumnInfo("Air Pump"));
            MainColumns.Add("PCV", new ReportColumnInfo("PCV"));
            MainColumns.Add("EVAPSYSTEM", new ReportColumnInfo("Evap System"));
            MainColumns.Add("FUELCAP", new ReportColumnInfo("Fuel Cap"));
            MainColumns.Add("CAT", new ReportColumnInfo("Cat"));
            MainColumns.Add("VISUALRESULT", new ReportColumnInfo("Visual Result"));
            MainColumns.Add("SAFETYRESULT", new ReportColumnInfo("Safety Result"));
            MainColumns.Add("VEHINFO", new ReportColumnInfo("Vehicle Info"));
            MainColumns.Add("WHEELS", new ReportColumnInfo("Wheels"));
            MainColumns.Add("TIRES", new ReportColumnInfo("Tires"));
            MainColumns.Add("STEERINGFRONTEND", new ReportColumnInfo("Steering Front End"));
            MainColumns.Add("FOOTBRK", new ReportColumnInfo("Foot Brake"));
            MainColumns.Add("PARKINGBRK", new ReportColumnInfo("Parking Brake"));
            MainColumns.Add("INSTRUMENTS", new ReportColumnInfo("Instruments"));
            MainColumns.Add("HORNELEC", new ReportColumnInfo("Electric Horn"));
            MainColumns.Add("REARLIGHT", new ReportColumnInfo("Rear Light"));
            MainColumns.Add("STOPLIGHT", new ReportColumnInfo("Stop Light"));
            MainColumns.Add("FRONTLIGHT", new ReportColumnInfo("Front Light"));
            MainColumns.Add("DIRSIGNAL", new ReportColumnInfo("Directional Signal"));
            MainColumns.Add("OTHERLIGHT", new ReportColumnInfo("Other Light"));
            MainColumns.Add("HEADLIGHTAIM", new ReportColumnInfo("Headlight Aim"));
            MainColumns.Add("MIRROR", new ReportColumnInfo("Mirror"));
            MainColumns.Add("DEFROSTER", new ReportColumnInfo("Defroster"));
            MainColumns.Add("GLASS", new ReportColumnInfo("Glass"));
            MainColumns.Add("WIPERS", new ReportColumnInfo("Wipers"));
            MainColumns.Add("EXHAUST", new ReportColumnInfo("Exhaust"));
            MainColumns.Add("FUELSYSTEM", new ReportColumnInfo("Fuel System"));
            MainColumns.Add("BUMPERS", new ReportColumnInfo("Bumpers"));
            MainColumns.Add("BODYCHASSIS", new ReportColumnInfo("Body Chassis"));
        }
    }

    public enum PredefinedQueryType
    {
        Make = 0,
        Model = 1,
        ModelYear = 2,
        County = 3,
        InspectorID = 4,
        StationId = 5,
    }
}