using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;

namespace NHPortal.Classes.Reference
{
    public static class InqInspectionBy
    {
        public static string GenerateDetailsTable(DataRow row)
        {
            string html = null;
            string saveTestType, saveSafetyResult;

            // Save Test Type for Conditional Table Generation
            saveTestType = row["Test Type"].ToString();
            saveSafetyResult = row["Safety Result"].ToString();

            FormatDetailsVehicleInformation(row);
            html += "<div class='details' style=\"visibility:visible\">";
            html += "<table class='expandedDetails'>";            
            html += "<tr><td colspan =\"4\" style=\"text-align:center;\" class=\"table_header\"><h2>Vehicle Information</h2></td></tr>";

            html += Create4Cell2DataTableRow("VIN", row["VIN"].ToString(), "Registration Number", row["Registration Number"].ToString());
            html += Create4Cell2DataTableRow("Model Year", row["Model Year"].ToString(), "Fuel Code", row["Fuel Code"].ToString());
            html += Create4Cell2DataTableRow("Make", row["Make"].ToString(), "Body Style", row["Body Style"].ToString());
            html += Create4Cell2DataTableRow("Model", row["Model"].ToString(), "License", row["License"].ToString());
            html += Create4Cell2DataTableRow("Transmission", row["Transmission"].ToString(), " ", " ");
            html += Create4Cell2DataTableRow("Number Cylinders", row["Number Cylinders"].ToString(), "Engine Size(ltr)", row["Engine Size"].ToString());
            html += Create4Cell2DataTableRow("Registration Date", row["Registration Date"].ToString(), "Registration Expiration", row["Registration Expiration"].ToString());
            html += "</table>";
            html += "</div>";

            FormatDetailsTestedAsInformation(row);
            html += "<div class='details' style=\"visibility:visible\">";
            html += "<table class='expandedDetails'>";  
            html += "<tr><td colspan =\"4\" style=\"text-align:center;\" class=\"table_header\"><h2>Tested As Information</h2></td></tr>";

            html += Create4Cell2DataTableRow("Odometer", row["Tested As Odometer"].ToString(), "Vehicle Type", row["Tested As Vehicle Type"].ToString());
            html += Create4Cell2DataTableRow("Model Year", row["Tested As Model Year"].ToString(), "Fuel Code", row["Tested As Fuel Code"].ToString());
            html += Create4Cell2DataTableRow("Make", row["Tested As Make"].ToString(), "GVW", row["Tested As GVW"].ToString());
            html += Create4Cell2DataTableRow("Model", row["Tested As Model"].ToString(), "Body Style", row["Tested AS Body Style"].ToString());
            html += "</table>";
            html += "</div>";

            FormatDetailsInspectionInformation(row);
            html += "<div class='details' style=\"visibility:visible\">";
            html += "<table class='expandedDetails'>";  
            html += "<tr><td colspan =\"4\" style=\"text-align:center;\" class=\"table_header\"><h2>Inspection Information</h2></td></tr>";

            html += Create4Cell2DataTableRow("Test Date", row["Test Date"].ToString(), "Test Time", row["Test Time"].ToString());
            html += Create4Cell2DataTableRow("60-Day End Date", row["60 Day End Date"].ToString(), " ", " ");
            html += Create4Cell2DataTableRow("Test Type", row["Test Type"].ToString(), "Test Sequence", row["Test Sequence"].ToString());
            html += Create4Cell2DataTableRow("Off-Line Begin of Inspection", row["OffLine Begin of Inspection"].ToString(), "Off-Line End of Inspection", row["OffLine End of Inspection"].ToString());
            html += Create4Cell2DataTableRow("Visual Result", row["Visual Result"].ToString(), "OBD Result", row["OBD Result"].ToString());
            html += Create4Cell2DataTableRow("Safety Result", row["Safety Result"].ToString(), "Overall Result", row["Overall Result"].ToString());
            html += Create4Cell2DataTableRow("Station ID", row["Station ID"].ToString(), "Inspector ID", row["Inspector ID"].ToString());
            html += Create4Cell2DataTableRow("Unit ID", row["Unit ID"].ToString(), "Sticker Number", row["Sticker Number"].ToString());
            html += Create4Cell2DataTableRow("Chargeable", row["Chargeable"].ToString(), "OBD Waiver - Advisory", row["OBD Waiver / Advisory"].ToString());
            html += Create2CellDataTableRow("Notes", row["Notes"].ToString());
            html += "</table>";
            html += "</div>";

            if ((saveTestType == "V") || (saveTestType == "I"))
            {
                FormatDetailsVisualInspectionInformation(row);
                html += "<div class='details' style=\"visibility:visible\">";
                html += "<table class='expandedDetails'>";  
                html += "<tr><td colspan =\"4\" style=\"text-align:center;\" class=\"table_header\"><h2>Visual Inspection Information</h2></td></tr>";

                html += Create4Cell2DataTableRow("Visual Inspection Result", row["Visual Inspection Result"].ToString(), "Evap", row["Evap"].ToString());
                html += Create4Cell2DataTableRow("Air Pump", row["Air Pump"].ToString(), "Fuel System", row["Fuel System"].ToString());
                html += Create4Cell2DataTableRow("PCV", row["PCV"].ToString(), "Catalyst", row["Catalyst"].ToString());
                html += "</table>";
                html += "</div>";
            }

            if (saveTestType == "O")
            {
                FormatDetailsOBD2InspectionInformation(row);
                html += "<div class='details' style=\"visibility:visible\">";
                html += "<table class='expandedDetails'>";  
                html += "<tr><td colspan =\"4\" style=\"text-align:center;\" class=\"table_header\"><h2>OBD II Inspection Information</h2></td></tr>";

                html += Create4Cell2DataTableRow("OBD Inspection Result", row["OBD Inspection Result"].ToString(), "MIL Status", row["MIL Status"].ToString());
                html += CreateHeaderRowDataTableRow("Bulb Checks");
                html += Create4Cell2DataTableRow("Key On Engine Off", row["Key On Engine Off"].ToString(), "Key On Engine Running", row["Key On Engine Running"].ToString());
                html += "<tr>" + "<td colspan=\"2\" style=\"text-align:center;\" class=\"detail_title\">" + "OBD Readiness Monitors" + "</td>" + "<td colspan=\"2\" style=\"text-align:center;\" class=\"detail_title\">" + "OBD DTCs" + "</td>";
                html += Create4Cell2DataTableRow("Misfire", row["Misfire"].ToString(), "Number of DTCs", row["Number of DTCs"].ToString());
                html += Create4Cell2DataTableRow("Fuel System", row["OBD Fuel System"].ToString(), "DTC-1", row["DTC-1"].ToString());
                html += Create4Cell2DataTableRow("Component", row["Component"].ToString(), "DTC-2", row["DTC-2"].ToString());
                html += Create4Cell2DataTableRow("Catalyst", row["OBD Catalyst"].ToString(), "DTC-3", row["DTC-3"].ToString());
                html += Create4Cell2DataTableRow("Heated Catalyst", row["Heated Catalyst"].ToString(), "DTC-4", row["DTC-4"].ToString());
                html += Create4Cell2DataTableRow("Evap System", row["Evap System"].ToString(), "DTC-5", row["DTC-5"].ToString());
                html += Create4Cell2DataTableRow("Air System", row["Air System"].ToString(), "DTC-6", row["DTC-6"].ToString());
                html += Create4Cell2DataTableRow("AC System", row["AC System"].ToString(), "DTC-7", row["DTC-7"].ToString());
                html += Create4Cell2DataTableRow("Oxygen Sensor", row["Oxygen Sensor"].ToString(), "DTC-8", row["DTC-8"].ToString());
                html += Create4Cell2DataTableRow("Heated O2 Sensor", row["Heated O2 Sensor"].ToString(), "DTC-9", row["DTC-9"].ToString());
                html += Create4Cell2DataTableRow("EGR System", row["EGR System"].ToString(), "DTC-10", row["DTC-10"].ToString());
                html += Create4Cell2DataTableRow("Adequate Voltage", row["Adequate Voltage"].ToString(), " ", " ");
                html += "</table>";
                html += "</div>";
            }

            html += "<div class='details' style=\"visibility:visible\">";
            html += "<table class='expandedDetails'>";  
            html += "<tr><td colspan =\"4\" style=\"text-align:center;\" class=\"table_header\"><h2>Visual Inspection Information</h2></td></tr>";

            html += Create2CellDataTableRow("Safety Inspection Result", row["Safety Result"].ToString());
            html += "</table>";
            html += "</div>";

            return html;
        }

        public static string GenerateSafetyBreakdownTable(DataRow row)
        {
            string html = null;

            html += "<div class='details' style=\"visibility:visible\">";
            html += "<table class='expandedDetails'>";
            html += "<tr><td colspan =\"3\" style=\"text-align:center;\" class=\"table_header\"><h2>Breakdown By Rejected And / Or Corrected Items (Red - Rejected, Blue - Corrected)</h2></td></tr>";

            html += InqInspectionBy.FormatDetailsSafetyInformation(row);
            html += "</table>";
            html += "</div>";

            return html;
        }

        public static void FormatDetailsVehicleInformation(DataRow row)
        {
            string strDateTimeFormattedOut;

            // Vehicle Information Data Transformations
            if (!String.IsNullOrEmpty(row["VMT Registration Date"].ToString()))
            {
                NHPortal.Classes.NHPortalUtilities.TryConvertDateTimeFormat(row["VMT Registration Date"].ToString(), "yyyyMMdd", "M/d/yyyy", out strDateTimeFormattedOut);
                row["VMT Registration Date"] = strDateTimeFormattedOut.ToString();
            }

            if (!String.IsNullOrEmpty(row["VMT Registration Expiration"].ToString()))
            {
                NHPortal.Classes.NHPortalUtilities.TryConvertDateTimeFormat(row["VMT Registration Expiration"].ToString(), "yyyyMMdd", "M/d/yyyy", out strDateTimeFormattedOut);
                row["VMT Registration Expiration"] = strDateTimeFormattedOut.ToString();
            }

            row["Fuel Code"] = Classes.Reference.FuelCodeTypes.GetDescription(row["Fuel Code"].ToString()[0]);
            row["Engine Size"] = GDCoreUtilities.NullSafe.ToDecimal(row["Engine Size"]).ToString("F1", CultureInfo.InvariantCulture);
        }

        public static string DisplayDetailsVehicleInformation(DataRow row)
        {
            string html;

            html = "<div class='details' style=\"visibility:visible\">";
            html += "<table class='expandedDetails'>";
            html += "<tr><td colspan =\"4\" style=\"text-align:center;\" class=\"table_header\"><h2>Vehicle Information</h2></td></tr>";

            html += Create4Cell2DataTableRow("VIN", row["VIN"].ToString(), "Registration Number", row["Registration Number"].ToString());
            html += Create4Cell2DataTableRow("Model Year", row["Model Year"].ToString(), "Fuel Code", row["Fuel Code"].ToString());
            html += Create4Cell2DataTableRow("Make", row["Make"].ToString(), "Body Style", row["Body Style"].ToString());
            html += Create4Cell2DataTableRow("Model", row["Model"].ToString(), "License", row["License"].ToString());
            html += Create4Cell2DataTableRow("Transmission", row["Transmission"].ToString(), " ", " ");
            html += Create4Cell2DataTableRow("Number Cylinders", row["Number Cylinders"].ToString(), "Engine Size(ltr)", row["Engine Size"].ToString());
            html += Create4Cell2DataTableRow("Registration Date", row["Registration Date"].ToString(), "Registration Expiration", row["Registration Expiration"].ToString());

            // Finialize Table
            html += "</table></div>";

            return html;
        }

        public static void FormatDetailsTestedAsInformation(DataRow row)
        {
            row["Tested As Fuel Code"] = Classes.Reference.FuelCodeTypes.GetDescription(row["Tested As Fuel Code"].ToString()[0]);
            row["Tested As Vehicle Type"] = Classes.Reference.SafetyTypes.GetDescription(row["Tested As Vehicle Type"].ToString()[0]);

            if (!String.IsNullOrEmpty(row["Tested As GVW"].ToString()))
            {
                if (row["Tested As GVW"].ToString() == "H") row["Tested As GVW"] = "GREATER THAN 10000 LBS";
                if (row["Tested As GVW"].ToString() == "M") row["Tested As GVW"] = "BETWEEN 8501 AND 10000 LBS";
                if (row["Tested As GVW"].ToString() == "L") row["Tested As GVW"] = "LESS THAN 8501 LBS";
            }
        }

        public static string DisplayDetailsTestedAsInformation(DataRow row)
        {
            string html;

            html = "<div class='details' style=\"visibility:visible\">";
            html += "<table class='expandedDetails'>";
            html += "<tr><td colspan =\"4\" style=\"text-align:center;\" class=\"table_header\"><h2>Tested As Information</h2></td></tr>";

            html += Create4Cell2DataTableRow("Odometer", row["Tested As Odometer"].ToString(), "Vehicle Type", row["Tested As Vehicle Type"].ToString());
            html += Create4Cell2DataTableRow("Model Year", row["Tested As Model Year"].ToString(), "Fuel Code", row["Tested As Fuel Code"].ToString());
            html += Create4Cell2DataTableRow("Make", row["Tested As Make"].ToString(), "GVW", row["Tested As GVW"].ToString());
            html += Create4Cell2DataTableRow("Model", row["Tested As Model"].ToString(), "Body Style", row["Tested AS Body Style"].ToString());

            // Finialize Table
            html += "</table></div>";

            return html;
        }

        public static void FormatDetailsInspectionInformation(DataRow row)
        {
            string strDateTimeFormattedOut;
            if (!String.IsNullOrEmpty(row["Test Date"].ToString()))
            {
                NHPortal.Classes.NHPortalUtilities.TryConvertDateTimeFormat(row["Test Date"].ToString(), "yyyyMMdd", "M/d/yyyy", out strDateTimeFormattedOut);
                row["Test Date"] = strDateTimeFormattedOut.ToString();
            }

            if (!String.IsNullOrEmpty(row["Test Time"].ToString()))
            {
                NHPortal.Classes.NHPortalUtilities.TryConvertDateTimeFormat(row["Test Time"].ToString(), "HHmmss", "hh:mm:ss tt", out strDateTimeFormattedOut);
                row["Test Time"] = strDateTimeFormattedOut.ToString();
            }

            if (!String.IsNullOrEmpty(row["Init Test Date"].ToString()))
            {
                NHPortal.Classes.NHPortalUtilities.TryConvertDateTimeFormat(row["Init Test Date"].ToString(), "yyyyMMdd", "M/d/yyyy", out strDateTimeFormattedOut);
                row["Init Test Date"] = strDateTimeFormattedOut.ToString();
            }

            if (!String.IsNullOrEmpty(row["Init Test Time"].ToString()))
            {
                NHPortal.Classes.NHPortalUtilities.TryConvertDateTimeFormat(row["Init Test Time"].ToString(), "HHmmss", "hh:mm:ss tt", out strDateTimeFormattedOut);
                row["Init Test Time"] = strDateTimeFormattedOut.ToString();
            }

            // Add 60 days to TestEndDate
            DateTime dtSixtyDayEndDate;
            DateTime.TryParseExact(row["60 Day End Date"].ToString(), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtSixtyDayEndDate);
            row["60 Day End Date"] = dtSixtyDayEndDate.AddDays(60).ToShortDateString();

            row["OffLine Begin of Inspection"] = OfflineInspectionDescription(row["OffLine Begin of Inspection"].ToString()[0]);
            row["OffLine End of Inspection"] = OfflineInspectionDescription(row["OffLine End of Inspection"].ToString()[0]);

            row["Test Type"] = Classes.Reference.EmissionTypes.GetDescription(row["Test Type"].ToString()[0]);
            row["Chargeable"] = GetChargeableDescription(row["Chargeable"].ToString()[0]);
            row["Visual Result"] = GetOverallResultDescription(row["Visual Result"].ToString()[0]);
            row["OBD Result"] = GetOverallResultDescription(row["OBD Result"].ToString()[0]);
            row["Overall Result"] = GetOverallResultDescription(row["Overall Result"].ToString()[0]);
            row["Safety Result"] = GetOverallResultDescription(row["Safety Result"].ToString()[0]);
        }

        public static void FormatDetailsVisualInspectionInformation(DataRow row)
        {
            row["Visual Inspection Result"] = GetOverallResultDescription(row["Visual Inspection Result"].ToString()[0]);
            row["Evap"] = GetOverallResultDescription(row["Evap"].ToString()[0]);
            row["Air Pump"] = GetOverallResultDescription(row["Air Pump"].ToString()[0]);
            row["Fuel System"] = GetOverallResultDescription(row["Fuel System"].ToString()[0]);
            row["PCV"] = GetOverallResultDescription(row["PCV"].ToString()[0]);
            row["Catalyst"] = GetOverallResultDescription(row["Catalyst"].ToString()[0]);
        }

        public static void FormatDetailsOBD2InspectionInformation(DataRow row)
        {
            row["OBD Inspection Result"] = row["OBD Result"];
            row["MIL Status"] = GetMILStatusDescription(row["MIL Status"].ToString());
            row["Key On Engine Off"] = FormatPassReject(row["Key On Engine Off"].ToString());
            row["Key On Engine Running"] = FormatPassReject(row["Key On Engine Running"].ToString());
            row["Misfire"] = GetOBDDescription(row["Misfire"].ToString());
            row["Number of DTCs"] = GDCoreUtilities.NullSafe.ToInt(row["Number of DTCs"]).ToString();
            row["OBD Fuel System"] = GetOBDDescription(row["OBD Fuel System"].ToString());
            row["DTC-1"] = FormatDTCs(row["DTC-1"].ToString());
            row["Component"] = GetOBDDescription(row["Component"].ToString());
            row["DTC-2"] = FormatDTCs(row["DTC-2"].ToString());
            row["OBD Catalyst"] = GetOBDDescription(row["OBD Catalyst"].ToString());
            row["DTC-3"] = FormatDTCs(row["DTC-3"].ToString());
            row["Heated Catalyst"] = GetOBDDescription(row["Heated Catalyst"].ToString());
            row["DTC-4"] = FormatDTCs(row["DTC-4"].ToString());
            row["Evap System"] = GetOBDDescription(row["Evap System"].ToString());
            row["DTC-5"] = FormatDTCs(row["DTC-5"].ToString());
            row["Air System"] = GetOBDDescription(row["Air System"].ToString());
            row["DTC-6"] = FormatDTCs(row["DTC-6"].ToString());
            row["AC System"] = GetOBDDescription(row["AC System"].ToString());
            row["DTC-7"] = FormatDTCs(row["DTC-7"].ToString());
            row["Oxygen Sensor"] = GetOBDDescription(row["Oxygen Sensor"].ToString());
            row["DTC-8"] = FormatDTCs(row["DTC-8"].ToString());
            row["Heated O2 Sensor"] = GetOBDDescription(row["Heated O2 Sensor"].ToString());
            row["DTC-9"] = FormatDTCs(row["DTC-9"].ToString());
            row["EGR System"] = GetOBDDescription(row["EGR System"].ToString());
            row["DTC-10"] = FormatDTCs(row["DTC-10"].ToString());
            row["Adequate Voltage"] = row["Adequate Voltage"];   
        }

        public static string Create3CellTableRow(string Value1, string Value2, string Value3, string Color)
        {
            string html = null;
            string myColor = Color;

            if (Color == "2")
            {
                html += "<tr>";
                html += "<td class=\"detail_red\">" + Value1 + "</td>";
                html += "<td class=\"detail_red\">" + Value2 + "</td>";
                html += "<td class=\"detail_red\">" + Value3 + "</td>";
                html += "</tr>";
            }
            else
            {
                html += "<tr>";
                html += "<td class=\"detail_blue\">" + Value1 + "</td>";
                html += "<td class=\"detail_blue\">" + Value2 + "</td>";
                html += "<td class=\"detail_blue\">" + Value3 + "</td>";
                html += "</tr>";
            }

            return html;
        }

        public static bool CheckSafetyRejectOrCorrected(string strSafetyField, DataRow row)
        {
            bool rejectedOrCorrected = false;
            if ((row[strSafetyField].ToString() == "2") || (row[strSafetyField].ToString() == "3"))
            {
                rejectedOrCorrected = true;
            }
            return rejectedOrCorrected;
        }

        public static string GetOverallResultDescription(char chrOverallResultCode)
        {
            string strOverallResultDescription = chrOverallResultCode.ToString();

            if (chrOverallResultCode == '0') strOverallResultDescription = "UNKNOWN";
            if (chrOverallResultCode == '1') strOverallResultDescription = "PASS";
            if (chrOverallResultCode == '2') strOverallResultDescription = "REJECT";
            if (chrOverallResultCode == '3') strOverallResultDescription = "CORRECTED";
            if (chrOverallResultCode == '4') strOverallResultDescription = "N/A";
            if (chrOverallResultCode == '5') strOverallResultDescription = "ADMIN";
            if (chrOverallResultCode == '6') strOverallResultDescription = "ABORT";
            if (chrOverallResultCode == '9') strOverallResultDescription = "PREV PASS";

            return strOverallResultDescription;
        }

        public static string OfflineInspectionDescription(char yOrN)
        {
            string yesNoDescription = "NO";

            if (yOrN == '1') yesNoDescription = "YES";

            return yesNoDescription;
        }

        public static string GetOBDDescription(string strOBDMonitors)
        {
            string obdDescription = "UNKNOWN";
            if (strOBDMonitors == "0") obdDescription = "UNKNOWN";
            if (strOBDMonitors == "1") obdDescription = "READY";
            if (strOBDMonitors == "2") obdDescription = "NOT READY";
            if (strOBDMonitors == "4") obdDescription = "NOT SUPPORTED";

            return obdDescription.ToString();
        }

        public static string GetChargeableDescription(char chrChargeCode)
        {
            string strChargeCodeDescription = chrChargeCode.ToString();

            if (chrChargeCode == 'N') strChargeCodeDescription = "NO";
            if (chrChargeCode == '1') strChargeCodeDescription = "$3.40";
            if (chrChargeCode == '2') strChargeCodeDescription = "$5.00";
            if (chrChargeCode == '3') strChargeCodeDescription = "$4.70";
            if (chrChargeCode == '4') strChargeCodeDescription = "$4.65";
            if (chrChargeCode == '5') strChargeCodeDescription = "$5.15";
            if (chrChargeCode == '6') strChargeCodeDescription = "$5.10";
            if (chrChargeCode == '7') strChargeCodeDescription = "$3.38";

            return strChargeCodeDescription;
        }

        public static string GetMILStatusDescription(string MILCode)
        {
            string strMILStatusDescription = "UNKNOWN";
            if (MILCode[0] == '0')
            {
                if (MILCode[1] == '0') strMILStatusDescription = "PASS: MIL CMD OFF";
                if (MILCode[1] == '3') strMILStatusDescription = "REJECT: UNABLE TO COMMUNICATE";
                if (MILCode[1] == '4') strMILStatusDescription = "REJECT: UNREADY";
                if (MILCode[1] == '5') strMILStatusDescription = "REJECT: MIL CMD ON";
            }
            else
            {
                strMILStatusDescription = "REJECT - DLC ";
                if (MILCode[0] == '1') strMILStatusDescription += "MISSING";
                if (MILCode[0] == '2') strMILStatusDescription += "DAMAGED";
                if (MILCode[0] == '3') strMILStatusDescription += "MOFDIFIED";
                if (MILCode[0] == '4') strMILStatusDescription += "TAMPERED";
                if (MILCode[0] == '5') strMILStatusDescription += "OBSTRUCTED";
                if (MILCode[0] == '6') strMILStatusDescription += "DISCONNECTED";

            }

            return strMILStatusDescription;
        }

        public static string FormatPassReject(string str10)
        {
            string strPassFail = "UNKNOWN";
            if (str10 == "0") strPassFail = "N/A";
            if (str10 == "1") strPassFail = "PASS";
            if (str10 == "2") strPassFail = "REJECT";

            return strPassFail;
        }

        public static string FormatDTCs(string strDTCs)
        {
            string dtrDescription = strDTCs;

            if (strDTCs == "00000") dtrDescription = " ";

            return dtrDescription.ToString();
        }

        public static string Create4Cell2DataTableRow(string Header1, string HeaderValue1, string Header2, string HeaderValue2)
        {
            string html = null;

            html += "<tr>";
            html += " <td class=\"detail_title\">" + Header1 + "</td>";
            html += "<td>" + HeaderValue1 + "</td>";
            html += " <td class=\"detail_title\">" + Header2 + "</td>";
            html += "<td>" + HeaderValue2 + "</td>";
            html += "</tr>";

            return html;
        }

        public static string Create2CellDataTableRow(string Header1, string HeaderValue1)
        {
            string html = null;

            html += "<tr>";
            html += " <td class=\"detail_title\">" + Header1 + "</td>";
            html += "<td>" + HeaderValue1 + "</td>";
            html += "</tr>";

            return html;
        }

        public static string CreateHeaderRowDataTableRow(string Header)
        {
            string html = null;
            html += "<tr><td colspan =\"4\" style=\"text-align:center;\" class=\"table_header\"><h2>" + Header + "</h2></td></tr>";

            return html;
        }

        public static StringBuilder InqInspectionSafetyBreakdownAdhoc(StringBuilder sb)
        {
            sb.AppendLine("   SELECT ");

            //Safety Information
            sb.AppendLine("         T.SAF_BS_VEHINFO ");
            sb.AppendLine(",           T.SAF_BS_ITEM_1 ");
            sb.AppendLine(",           T.SAF_BS_ITEM_2 ");
            sb.AppendLine(",           T.SAF_BS_ITEM_3 ");
            sb.AppendLine(",        T.SAF_BS_WHEELS  ");
            sb.AppendLine(",           T.SAF_BS_ITEM_4  ");
            sb.AppendLine(",           T.SAF_BS_ITEM_5  ");
            sb.AppendLine(",        T.SAF_BS_TIRES   ");
            sb.AppendLine(",           T.SAF_BS_ITEM_6  ");
            sb.AppendLine(",           T.SAF_BS_ITEM_7  ");
            sb.AppendLine(",           T.SAF_BS_ITEM_8  ");
            sb.AppendLine(",           T.SAF_BS_ITEM_9  ");
            sb.AppendLine(",           T.SAF_BS_ITEM_10  ");
            sb.AppendLine(",           T.SAF_BS_ITEM_11  ");
            sb.AppendLine(",        T.SAF_BS_STEERING  ");
            sb.AppendLine(",           T.SAF_BS_ITEM_12  ");
            sb.AppendLine(",           T.SAF_BS_ITEM_13  ");
            sb.AppendLine(",           T.SAF_BS_ITEM_14  ");
            sb.AppendLine(",           T.SAF_BS_ITEM_15  ");
            sb.AppendLine(",           T.SAF_BS_ITEM_16  ");
            sb.AppendLine(",           T.SAF_BS_ITEM_17  ");
            sb.AppendLine(",        T.SAF_BS_FOOTBRAKE  ");
            sb.AppendLine(",           T.SAF_BS_ITEM_18  ");
            sb.AppendLine(",           T.SAF_BS_ITEM_19  ");
            sb.AppendLine(",           T.SAF_BS_ITEM_20  ");
            sb.AppendLine(",           T.SAF_BS_ITEM_21  ");
            sb.AppendLine(",           T.SAF_BS_ITEM_22  ");
            sb.AppendLine(",           T.SAF_BS_ITEM_23  ");
            sb.AppendLine(",           T.SAF_BS_ITEM_24  ");
            sb.AppendLine(",           T.SAF_BS_ITEM_25  ");
            sb.AppendLine(",           T.SAF_BS_ITEM_26  ");
            sb.AppendLine(",        T.SAF_BS_PARKINGBRAKE  ");
            sb.AppendLine(",           T.SAF_BS_ITEM_27  ");
            sb.AppendLine(",           T.SAF_BS_ITEM_28  ");
            sb.AppendLine(",        T.SAF_BS_INST  ");
            sb.AppendLine(",           T.SAF_BS_ITEM_29  ");
            sb.AppendLine(",           T.SAF_BS_ITEM_30  ");
            sb.AppendLine(",           T.SAF_BS_ITEM_31  ");
            sb.AppendLine(",           T.SAF_BS_ITEM_32  ");
            sb.AppendLine(",        T.SAF_BS_HORN  ");
            sb.AppendLine(",           T.SAF_BS_ITEM_33  ");
            sb.AppendLine(",           T.SAF_BS_ITEM_34  ");
            sb.AppendLine(",           T.SAF_BS_ITEM_35  ");
            sb.AppendLine(",        T.SAF_BS_REARLIGHTS  ");
            sb.AppendLine(",           T.SAF_BS_ITEM_36  ");
            sb.AppendLine(",           T.SAF_BS_ITEM_37  ");
            sb.AppendLine(",           T.SAF_BS_ITEM_38  ");
            sb.AppendLine(",           T.SAF_BS_ITEM_39  ");
            sb.AppendLine(",           T.SAF_BS_ITEM_40  ");
            sb.AppendLine(",        T.SAF_BS_STOPLIGHTS  ");
            sb.AppendLine(",           T.SAF_BS_ITEM_41  ");
            sb.AppendLine(",           T.SAF_BS_ITEM_42  ");
            sb.AppendLine(",        T.SAF_BS_FRONTLIGHTS  ");
            sb.AppendLine(",           T.SAF_BS_ITEM_43  ");
            sb.AppendLine(",           T.SAF_BS_ITEM_44  ");
            sb.AppendLine(",        T.SAF_BS_SIGNAL  ");
            sb.AppendLine(",           T.SAF_BS_ITEM_45  ");
            sb.AppendLine(",           T.SAF_BS_ITEM_46  ");
            sb.AppendLine(",           T.SAF_BS_ITEM_47  ");
            sb.AppendLine(",           T.SAF_BS_ITEM_48  ");
            sb.AppendLine(",           T.SAF_BS_ITEM_49  ");
            sb.AppendLine(",        T.SAF_BS_OTHERLIGHTS  ");
            sb.AppendLine(",           T.SAF_BS_ITEM_50  ");
            sb.AppendLine(",           T.SAF_BS_ITEM_51  ");
            sb.AppendLine(",        T.SAF_BS_HEADLIGHTAIM  ");
            sb.AppendLine(",           T.SAF_BS_ITEM_52  ");
            sb.AppendLine(",           T.SAF_BS_ITEM_53  ");
            sb.AppendLine(",        T.SAF_BS_MIRRORS  ");
            sb.AppendLine(",           T.SAF_BS_ITEM_54  ");
            sb.AppendLine(",           T.SAF_BS_ITEM_55  ");
            sb.AppendLine(",        T.SAF_BS_DEFROSTER  ");
            sb.AppendLine(",        T.SAF_BS_GLASS  ");
            sb.AppendLine(",           T.SAF_BS_ITEM_56  ");
            sb.AppendLine(",           T.SAF_BS_ITEM_57  ");
            sb.AppendLine(",           T.SAF_BS_ITEM_58  ");
            sb.AppendLine(",           T.SAF_BS_ITEM_59  ");
            sb.AppendLine(",           T.SAF_BS_ITEM_60  ");
            sb.AppendLine(",           T.SAF_BS_ITEM_61  ");
            sb.AppendLine(",           T.SAF_BS_ITEM_62  ");
            sb.AppendLine(",           T.SAF_BS_ITEM_63  ");
            sb.AppendLine(",        T.SAF_BS_WIPERS  ");
            sb.AppendLine(",           T.SAF_BS_ITEM_64  ");
            sb.AppendLine(",           T.SAF_BS_ITEM_65  ");
            sb.AppendLine(",           T.SAF_BS_ITEM_66  ");
            sb.AppendLine(",        T.SAF_BS_EXHAUST  ");
            sb.AppendLine(",           T.SAF_BS_ITEM_67  ");
            sb.AppendLine(",           T.SAF_BS_ITEM_68  ");
            sb.AppendLine(",        T.SAF_BS_FUELSYS  ");
            sb.AppendLine(",           T.SAF_BS_ITEM_69  ");
            sb.AppendLine(",           T.SAF_BS_ITEM_70  ");
            sb.AppendLine(",           T.SAF_BS_ITEM_71  ");
            sb.AppendLine(",           T.SAF_BS_ITEM_72  ");
            sb.AppendLine(",           T.SAF_BS_ITEM_73  ");
            sb.AppendLine(",        T.SAF_BS_BUMPERS  ");
            sb.AppendLine(",           T.SAF_BS_ITEM_74  ");
            sb.AppendLine(",           T.SAF_BS_ITEM_75  ");
            sb.AppendLine(",           T.SAF_BS_ITEM_76  ");
            sb.AppendLine(",        T.SAF_BS_BODY  ");
            sb.AppendLine(",           T.SAF_BS_ITEM_77  ");
            sb.AppendLine(",           T.SAF_BS_ITEM_78  ");
            sb.AppendLine(",           T.SAF_BS_ITEM_79  ");
            sb.AppendLine(",           T.SAF_BS_ITEM_80  ");
            sb.AppendLine(",           T.SAF_BS_ITEM_81  ");

            sb.AppendLine(",        T.SAF_BS_SUBITEM_1  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_2  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_3  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_4  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_5  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_6  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_7  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_8  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_9  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_10  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_11  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_12  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_13  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_14  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_15  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_16  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_17  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_18  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_19  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_20  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_21  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_22  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_23  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_24  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_25  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_26  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_27  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_28  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_29  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_30  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_31  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_32  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_33  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_34  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_35  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_36  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_37  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_38  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_39  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_40  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_41  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_42  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_43  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_44  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_45  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_46  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_47  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_48  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_49  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_50  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_51  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_52  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_53  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_54  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_55  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_56  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_57  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_58  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_59  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_60  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_61  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_62  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_63  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_64  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_65  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_66  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_67  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_68  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_69  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_70  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_71  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_72  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_73  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_74  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_75  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_76  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_77  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_78  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_79  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_80  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_81  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_82  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_83  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_84  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_85  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_86  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_87  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_88  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_89  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_90  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_91  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_92  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_93  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_94  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_95  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_96  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_97  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_98  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_99  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_100  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_101  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_102  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_103  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_104  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_105  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_106  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_107  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_108  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_109  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_110  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_111  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_112  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_113  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_114  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_115  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_116  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_117  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_118  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_119  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_120  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_121  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_122  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_123  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_124  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_125  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_126  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_127  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_128  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_129  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_130  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_131  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_132  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_133  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_134  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_135  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_136  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_137  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_138  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_139  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_140  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_141  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_142  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_143  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_144  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_145  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_146  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_147  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_148  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_149  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_150  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_151  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_152  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_153  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_154  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_155  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_156  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_157  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_158  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_159  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_160  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_161  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_162  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_163  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_164  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_165  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_166  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_167  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_168  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_169  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_170  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_171  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_172  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_173  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_174  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_175  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_176  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_177  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_178  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_179  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_180  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_181  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_182  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_183  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_184  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_185  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_186  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_187  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_188  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_189  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_190  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_191  ");
            sb.AppendLine(",        T.SAF_BS_SUBITEM_192  ");

            sb.AppendLine(",        T.SAF_TB_VEHINFO  ");
            sb.AppendLine(",        T.SAF_TB_WHEELS  ");
            sb.AppendLine(",        T.SAF_TB_TIRES  ");
            sb.AppendLine(",        T.SAF_TB_STEERING  ");
            sb.AppendLine(",        T.SAF_TB_FOOTBRAKE  ");
            sb.AppendLine(",        T.SAF_TB_PARKINGBRAKE  ");
            sb.AppendLine(",        T.SAF_TB_AIRBRAKE  ");
            sb.AppendLine(",        T.SAF_TB_INST  ");
            sb.AppendLine(",        T.SAF_TB_HORN  ");
            sb.AppendLine(",        T.SAF_TB_REARLIGHTS  ");
            sb.AppendLine(",        T.SAF_TB_STOPLIGHTS  ");
            sb.AppendLine(",        T.SAF_TB_FRONTLIGHTS  ");
            sb.AppendLine(",        T.SAF_TB_SIGNAL  ");
            sb.AppendLine(",        T.SAF_TB_OTHERLIGHTS  ");
            sb.AppendLine(",        T.SAF_TB_HEADLIGHTAIM  ");
            sb.AppendLine(",        T.SAF_TB_MIRRORS  ");
            sb.AppendLine(",        T.SAF_TB_DEFROSTER  ");
            sb.AppendLine(",        T.SAF_TB_GLASS  ");
            sb.AppendLine(",        T.SAF_TB_WIPERS  ");
            sb.AppendLine(",        T.SAF_TB_REFLECTOR  ");
            sb.AppendLine(",        T.SAF_TB_FIREEXT  ");
            sb.AppendLine(",        T.SAF_TB_EXHAUST  ");
            sb.AppendLine(",        T.SAF_TB_FUELSYS  ");
            sb.AppendLine(",        T.SAF_TB_BUMPERS  ");
            sb.AppendLine(",        T.SAF_TB_BODY  ");
            sb.AppendLine(",        T.SAF_TB_BUSBODY  ");
            sb.AppendLine(",        T.SAF_TB_BUSINTERIOR  ");

            sb.AppendLine(",        T.SAF_TB_ITEM_1  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_2  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_3  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_4  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_5  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_6  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_7  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_8  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_9  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_10  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_11  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_12  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_13  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_14  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_15  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_16  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_17  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_18  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_19  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_20  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_21  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_22  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_23  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_24  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_25  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_26  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_27  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_28  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_29  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_30  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_31  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_32  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_33  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_34  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_35  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_36  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_37  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_38  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_39  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_40  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_41  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_42  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_43  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_44  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_45  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_46  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_47  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_48  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_49  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_50  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_51  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_52  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_53  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_54  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_55  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_56  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_57  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_58  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_59  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_60  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_61  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_62  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_63  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_64  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_65  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_66  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_67  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_68  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_69  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_70  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_71  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_72  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_73  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_74  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_75  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_76  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_77  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_78  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_79  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_80  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_81  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_82  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_83  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_84  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_85  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_86  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_87  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_88  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_89  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_90  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_91  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_92  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_93  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_94  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_95  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_96  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_97  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_98  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_99  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_100  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_101  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_102  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_103  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_104  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_105  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_106  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_107  ");
            sb.AppendLine(",        T.SAF_TB_ITEM_108  ");

            sb.AppendLine(",        T.SAF_TB_SUBITEM_1  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_2  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_3  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_4  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_5  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_6  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_7  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_8  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_9  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_10  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_11  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_12  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_13  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_14  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_15  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_16  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_17  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_18  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_19  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_20  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_21  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_22  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_23  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_24  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_25  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_26  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_27  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_28  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_29  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_30  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_31  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_32  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_33  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_34  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_35  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_36  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_37  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_38  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_39  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_40  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_41  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_42  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_43  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_44  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_45  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_46  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_47  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_48  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_49  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_50  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_51  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_52  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_53  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_54  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_55  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_56  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_57  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_58  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_59  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_60  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_61  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_62  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_63  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_64  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_65  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_66  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_67  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_68  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_69  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_70  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_71  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_72  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_73  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_74  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_75  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_76  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_77  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_78  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_79  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_80  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_81  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_82  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_83  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_84  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_85  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_86  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_87  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_88  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_89  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_90  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_91  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_92  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_93  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_94  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_95  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_96  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_97  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_98  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_99  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_100  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_101  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_102  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_103  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_104  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_105  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_106  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_107  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_108  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_109  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_110  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_111  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_112  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_113  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_114  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_115  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_116  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_117  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_118  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_119  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_120  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_121  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_122  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_123  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_124  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_125  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_126  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_127  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_128  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_129  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_130  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_131  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_132  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_133  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_134  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_135  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_136  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_137  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_138  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_139  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_140  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_141  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_142  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_143  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_144  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_145  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_146  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_147  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_148  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_149  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_150  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_151  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_152  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_153  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_154  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_155  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_156  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_157  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_158  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_159  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_160  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_161  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_162  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_163  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_164  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_165  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_166  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_167  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_168  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_169  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_170  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_171  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_172  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_173  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_174  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_175  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_176  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_177  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_178  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_179  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_180  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_181  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_182  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_183  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_184  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_185  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_186  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_187  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_188  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_189  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_190  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_191  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_192  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_193  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_194  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_195  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_196  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_197  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_198  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_199  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_200  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_201  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_202  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_203  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_204  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_205  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_206  ");
            sb.AppendLine(",        T.SAF_TB_SUBITEM_207  ");

            sb.AppendLine(",        T.SAF_MC_VEHINFO  ");
            sb.AppendLine(",        T.SAF_MC_WHEELS  ");
            sb.AppendLine(",        T.SAF_MC_TIRES  ");
            sb.AppendLine(",        T.SAF_MC_STEERING  ");
            sb.AppendLine(",        T.SAF_MC_FOOTBRAKE  ");
            sb.AppendLine(",        T.SAF_MC_INST  ");
            sb.AppendLine(",        T.SAF_MC_HORN  ");
            sb.AppendLine(",        T.SAF_MC_REARLIGHTS  ");
            sb.AppendLine(",        T.SAF_MC_STOPLIGHTS  ");
            sb.AppendLine(",        T.SAF_MC_FRONTLIGHTS  ");
            sb.AppendLine(",        T.SAF_MC_SIGNAL  ");
            sb.AppendLine(",        T.SAF_MC_OTHERLIGHTS  ");
            sb.AppendLine(",        T.SAF_MC_HEADLIGHTAIM  ");
            sb.AppendLine(",        T.SAF_MC_MIRRORS  ");
            sb.AppendLine(",        T.SAF_MC_GLASS  ");
            sb.AppendLine(",        T.SAF_MC_EXHAUST  ");
            sb.AppendLine(",        T.SAF_MC_FUELSYS  ");
            sb.AppendLine(",        T.SAF_MC_BODY  ");

            sb.AppendLine(",        T.SAF_MC_ITEM_1  ");
            sb.AppendLine(",        T.SAF_MC_ITEM_2  ");
            sb.AppendLine(",        T.SAF_MC_ITEM_3  ");
            sb.AppendLine(",        T.SAF_MC_ITEM_4  ");
            sb.AppendLine(",        T.SAF_MC_ITEM_5  ");
            sb.AppendLine(",        T.SAF_MC_ITEM_6  ");
            sb.AppendLine(",        T.SAF_MC_ITEM_7  ");
            sb.AppendLine(",        T.SAF_MC_ITEM_8  ");
            sb.AppendLine(",        T.SAF_MC_ITEM_9  ");
            sb.AppendLine(",        T.SAF_MC_ITEM_10  ");
            sb.AppendLine(",        T.SAF_MC_ITEM_11  ");
            sb.AppendLine(",        T.SAF_MC_ITEM_12  ");
            sb.AppendLine(",        T.SAF_MC_ITEM_13  ");
            sb.AppendLine(",        T.SAF_MC_ITEM_14  ");
            sb.AppendLine(",        T.SAF_MC_ITEM_15  ");
            sb.AppendLine(",        T.SAF_MC_ITEM_16  ");
            sb.AppendLine(",        T.SAF_MC_ITEM_17  ");
            sb.AppendLine(",        T.SAF_MC_ITEM_18  ");
            sb.AppendLine(",        T.SAF_MC_ITEM_19  ");
            sb.AppendLine(",        T.SAF_MC_ITEM_20  ");
            sb.AppendLine(",        T.SAF_MC_ITEM_21  ");
            sb.AppendLine(",        T.SAF_MC_ITEM_22  ");
            sb.AppendLine(",        T.SAF_MC_ITEM_23  ");
            sb.AppendLine(",        T.SAF_MC_ITEM_24  ");
            sb.AppendLine(",        T.SAF_MC_ITEM_25  ");
            sb.AppendLine(",        T.SAF_MC_ITEM_26  ");
            sb.AppendLine(",        T.SAF_MC_ITEM_27  ");
            sb.AppendLine(",        T.SAF_MC_ITEM_28  ");
            sb.AppendLine(",        T.SAF_MC_ITEM_29  ");
            sb.AppendLine(",        T.SAF_MC_ITEM_30  ");
            sb.AppendLine(",        T.SAF_MC_ITEM_31  ");
            sb.AppendLine(",        T.SAF_MC_ITEM_32  ");
            sb.AppendLine(",        T.SAF_MC_ITEM_33  ");
            sb.AppendLine(",        T.SAF_MC_ITEM_34  ");
            sb.AppendLine(",        T.SAF_MC_ITEM_35  ");
            sb.AppendLine(",        T.SAF_MC_ITEM_36  ");
            sb.AppendLine(",        T.SAF_MC_ITEM_37  ");
            sb.AppendLine(",        T.SAF_MC_ITEM_38  ");
            sb.AppendLine(",        T.SAF_MC_ITEM_39  ");
            sb.AppendLine(",        T.SAF_MC_ITEM_40  ");
            sb.AppendLine(",        T.SAF_MC_ITEM_41  ");
            sb.AppendLine(",        T.SAF_MC_ITEM_42  ");
            sb.AppendLine(",        T.SAF_MC_ITEM_43  ");
            sb.AppendLine(",        T.SAF_MC_ITEM_44  ");
            sb.AppendLine(",        T.SAF_MC_ITEM_45  ");
            sb.AppendLine(",        T.SAF_MC_ITEM_46  ");
            sb.AppendLine(",        T.SAF_MC_ITEM_47  ");
            sb.AppendLine(",        T.SAF_MC_ITEM_48  ");
            sb.AppendLine(",        T.SAF_MC_ITEM_49  ");
            sb.AppendLine(",        T.SAF_MC_ITEM_50  ");
            sb.AppendLine(",        T.SAF_MC_ITEM_51  ");
            sb.AppendLine(",        T.SAF_MC_ITEM_52  ");
            sb.AppendLine(",        T.SAF_MC_ITEM_53  ");
            sb.AppendLine(",        T.SAF_MC_ITEM_54  ");
            sb.AppendLine(",        T.SAF_MC_ITEM_55  ");
            sb.AppendLine(",        T.SAF_MC_ITEM_56  ");
            sb.AppendLine(",        T.SAF_MC_ITEM_57  ");

            sb.AppendLine(",        T.SAF_MC_SUBITEM_1  ");
            sb.AppendLine(",        T.SAF_MC_SUBITEM_2  ");
            sb.AppendLine(",        T.SAF_MC_SUBITEM_3  ");
            sb.AppendLine(",        T.SAF_MC_SUBITEM_4  ");
            sb.AppendLine(",        T.SAF_MC_SUBITEM_5  ");
            sb.AppendLine(",        T.SAF_MC_SUBITEM_6  ");
            sb.AppendLine(",        T.SAF_MC_SUBITEM_7  ");
            sb.AppendLine(",        T.SAF_MC_SUBITEM_8  ");
            sb.AppendLine(",        T.SAF_MC_SUBITEM_9  ");
            sb.AppendLine(",        T.SAF_MC_SUBITEM_10  ");
            sb.AppendLine(",        T.SAF_MC_SUBITEM_11  ");
            sb.AppendLine(",        T.SAF_MC_SUBITEM_12  ");
            sb.AppendLine(",        T.SAF_MC_SUBITEM_13  ");
            sb.AppendLine(",        T.SAF_MC_SUBITEM_14  ");
            sb.AppendLine(",        T.SAF_MC_SUBITEM_15  ");
            sb.AppendLine(",        T.SAF_MC_SUBITEM_16  ");
            sb.AppendLine(",        T.SAF_MC_SUBITEM_17  ");
            sb.AppendLine(",        T.SAF_MC_SUBITEM_18  ");
            sb.AppendLine(",        T.SAF_MC_SUBITEM_19  ");
            sb.AppendLine(",        T.SAF_MC_SUBITEM_20  ");
            sb.AppendLine(",        T.SAF_MC_SUBITEM_21  ");
            sb.AppendLine(",        T.SAF_MC_SUBITEM_22  ");
            sb.AppendLine(",        T.SAF_MC_SUBITEM_23  ");
            sb.AppendLine(",        T.SAF_MC_SUBITEM_24  ");
            sb.AppendLine(",        T.SAF_MC_SUBITEM_25  ");
            sb.AppendLine(",        T.SAF_MC_SUBITEM_26  ");
            sb.AppendLine(",        T.SAF_MC_SUBITEM_27  ");
            sb.AppendLine(",        T.SAF_MC_SUBITEM_28  ");
            sb.AppendLine(",        T.SAF_MC_SUBITEM_29  ");
            sb.AppendLine(",        T.SAF_MC_SUBITEM_30  ");
            sb.AppendLine(",        T.SAF_MC_SUBITEM_31  ");
            sb.AppendLine(",        T.SAF_MC_SUBITEM_32  ");
            sb.AppendLine(",        T.SAF_MC_SUBITEM_33  ");
            sb.AppendLine(",        T.SAF_MC_SUBITEM_34  ");
            sb.AppendLine(",        T.SAF_MC_SUBITEM_35  ");
            sb.AppendLine(",        T.SAF_MC_SUBITEM_36  ");
            sb.AppendLine(",        T.SAF_MC_SUBITEM_37  ");
            sb.AppendLine(",        T.SAF_MC_SUBITEM_38  ");
            sb.AppendLine(",        T.SAF_MC_SUBITEM_39  ");
            sb.AppendLine(",        T.SAF_MC_SUBITEM_40  ");
            sb.AppendLine(",        T.SAF_MC_SUBITEM_41  ");
            sb.AppendLine(",        T.SAF_MC_SUBITEM_42  ");
            sb.AppendLine(",        T.SAF_MC_SUBITEM_43  ");
            sb.AppendLine(",        T.SAF_MC_SUBITEM_44  ");
            sb.AppendLine(",        T.SAF_MC_SUBITEM_45  ");
            sb.AppendLine(",        T.SAF_MC_SUBITEM_46  ");
            sb.AppendLine(",        T.SAF_MC_SUBITEM_47  ");
            sb.AppendLine(",        T.SAF_MC_SUBITEM_48  ");
            sb.AppendLine(",        T.SAF_MC_SUBITEM_49  ");
            sb.AppendLine(",        T.SAF_MC_SUBITEM_50  ");
            sb.AppendLine(",        T.SAF_MC_SUBITEM_51  ");
            sb.AppendLine(",        T.SAF_MC_SUBITEM_52  ");
            sb.AppendLine(",        T.SAF_MC_SUBITEM_53  ");

            sb.AppendLine(",        T.SAF_TR_VEHINFO  ");
            sb.AppendLine(",        T.SAF_TR_TIRES  ");
            sb.AppendLine(",        T.SAF_TR_MAINBRAKES  ");
            sb.AppendLine(",        T.SAF_TR_PARKINGBRAKE  ");
            sb.AppendLine(",        T.SAF_TRAIEER_EBREAKS  ");
            sb.AppendLine(",        T.SAF_TR_BREAKWIRING  ");
            sb.AppendLine(",        T.SAF_TR_REARLIGHTS  ");
            sb.AppendLine(",        T.SAF_TR_STOPLIGHTS  ");
            sb.AppendLine(",        T.SAF_TR_BUMPER  ");
            sb.AppendLine(",        T.SAF_TR_BODY  ");

            sb.AppendLine(",        T.SAF_TR_ITEM_1  ");
            sb.AppendLine(",        T.SAF_TR_ITEM_2  ");
            sb.AppendLine(",        T.SAF_TR_ITEM_3  ");
            sb.AppendLine(",        T.SAF_TR_ITEM_4  ");
            sb.AppendLine(",        T.SAF_TR_ITEM_5  ");
            sb.AppendLine(",        T.SAF_TR_ITEM_6  ");
            sb.AppendLine(",        T.SAF_TR_ITEM_7  ");
            sb.AppendLine(",        T.SAF_TR_ITEM_8  ");
            sb.AppendLine(",        T.SAF_TR_ITEM_9  ");
            sb.AppendLine(",        T.SAF_TR_ITEM_10  ");
            sb.AppendLine(",        T.SAF_TR_ITEM_11  ");
            sb.AppendLine(",        T.SAF_TR_ITEM_12  ");
            sb.AppendLine(",        T.SAF_TR_ITEM_13  ");
            sb.AppendLine(",        T.SAF_TR_ITEM_14  ");
            sb.AppendLine(",        T.SAF_TR_ITEM_15  ");
            sb.AppendLine(",        T.SAF_TR_ITEM_16  ");
            sb.AppendLine(",        T.SAF_TR_ITEM_17  ");
            sb.AppendLine(",        T.SAF_TR_ITEM_18  ");
            sb.AppendLine(",        T.SAF_TR_ITEM_19  ");
            sb.AppendLine(",        T.SAF_TR_ITEM_20  ");
            sb.AppendLine(",        T.SAF_TR_ITEM_21  ");
            sb.AppendLine(",        T.SAF_TR_ITEM_22  ");
            sb.AppendLine(",        T.SAF_TR_ITEM_23  ");
            sb.AppendLine(",        T.SAF_TR_ITEM_24  ");
            sb.AppendLine(",        T.SAF_TR_ITEM_25  ");
            sb.AppendLine(",        T.SAF_TR_ITEM_26  ");
            sb.AppendLine(",        T.SAF_TR_ITEM_27  ");
            sb.AppendLine(",        T.SAF_TR_ITEM_28  ");
            sb.AppendLine(",        T.SAF_TR_ITEM_29  ");
            sb.AppendLine(",        T.SAF_TR_ITEM_30  ");
            sb.AppendLine(",        T.SAF_TR_ITEM_31  ");
            sb.AppendLine(",        T.SAF_TR_ITEM_32  ");
            sb.AppendLine(",        T.SAF_TR_ITEM_33  ");
            sb.AppendLine(",        T.SAF_TR_ITEM_34  ");
            sb.AppendLine(",        T.SAF_TR_ITEM_35  ");

            sb.AppendLine(",        T.SAF_TR_SUBITEM_1  ");
            sb.AppendLine(",        T.SAF_TR_SUBITEM_2  ");
            sb.AppendLine(",        T.SAF_TR_SUBITEM_3  ");
            sb.AppendLine(",        T.SAF_TR_SUBITEM_4  ");
            sb.AppendLine(",        T.SAF_TR_SUBITEM_5  ");
            sb.AppendLine(",        T.SAF_TR_SUBITEM_6  ");
            sb.AppendLine(",        T.SAF_TR_SUBITEM_7  ");
            sb.AppendLine(",        T.SAF_TR_SUBITEM_8  ");
            sb.AppendLine(",        T.SAF_TR_SUBITEM_9  ");
            sb.AppendLine(",        T.SAF_TR_SUBITEM_10  ");
            sb.AppendLine(",        T.SAF_TR_SUBITEM_11  ");
            sb.AppendLine(",        T.SAF_TR_SUBITEM_12  ");
            sb.AppendLine(",        T.SAF_TR_SUBITEM_13  ");
            sb.AppendLine(",        T.SAF_TR_SUBITEM_14  ");
            sb.AppendLine(",        T.SAF_TR_SUBITEM_15  ");
            sb.AppendLine(",        T.SAF_TR_SUBITEM_16  ");
            sb.AppendLine(",        T.SAF_TR_SUBITEM_17  ");
            sb.AppendLine(",        T.SAF_TR_SUBITEM_18  ");
            sb.AppendLine(",        T.SAF_TR_SUBITEM_19  ");
            sb.AppendLine(",        T.SAF_TR_SUBITEM_20  ");
            sb.AppendLine(",        T.SAF_TR_SUBITEM_21  ");
            sb.AppendLine(",        T.SAF_TR_SUBITEM_22  ");
            sb.AppendLine(",        T.SAF_TR_SUBITEM_23  ");
            sb.AppendLine(",        T.SAF_TR_SUBITEM_24  ");
            sb.AppendLine(",        T.SAF_TR_SUBITEM_25  ");
            sb.AppendLine(",        T.SAF_TR_SUBITEM_26  ");
            sb.AppendLine(",        T.SAF_TR_SUBITEM_27  ");
            sb.AppendLine(",        T.SAF_TR_SUBITEM_28  ");
            sb.AppendLine(",        T.SAF_TR_SUBITEM_29  ");
            sb.AppendLine(",        T.SAF_TR_SUBITEM_30  ");
            sb.AppendLine(",        T.SAF_TR_SUBITEM_31  ");
            sb.AppendLine(",        T.SAF_TR_SUBITEM_32  ");
            sb.AppendLine(",        T.SAF_TR_SUBITEM_33  ");
            sb.AppendLine(",        T.SAF_TR_SUBITEM_34  ");
            sb.AppendLine(",        T.SAF_TR_SUBITEM_35  ");
            sb.AppendLine(",        T.SAF_TR_SUBITEM_36  ");
            sb.AppendLine(",        T.SAF_TR_SUBITEM_37  ");
            sb.AppendLine(",        T.SAF_TR_SUBITEM_38  ");
            sb.AppendLine(",        T.SAF_TR_SUBITEM_39  ");
            sb.AppendLine(",        T.SAF_TR_SUBITEM_40  ");
            sb.AppendLine(",        T.SAF_TR_SUBITEM_41  ");
            sb.AppendLine(",        T.SAF_TR_SUBITEM_42  ");
            sb.AppendLine(",        T.SAF_TR_SUBITEM_43  ");
            sb.AppendLine(",        T.SAF_TR_SUBITEM_44  ");
            sb.AppendLine(",        T.SAF_TR_SUBITEM_45  ");
            sb.AppendLine(",        T.SAF_TR_SUBITEM_46  ");
            sb.AppendLine(",        T.SAF_TR_SUBITEM_47  ");
            sb.AppendLine(",        T.SAF_TR_SUBITEM_48  ");
            sb.AppendLine(",        T.SAF_TR_SUBITEM_49  ");
            sb.AppendLine(",        T.SAF_TR_SUBITEM_50  ");
            sb.AppendLine(",        T.SAF_TR_SUBITEM_51  ");
            sb.AppendLine(",        T.SAF_TR_SUBITEM_52  ");
            sb.AppendLine(",        T.SAF_TR_SUBITEM_53  ");

            sb.AppendLine(",        T.SAF_AG_VIN  ");
            sb.AppendLine(",        T.SAF_AG_BRAKES  ");
            sb.AppendLine(",        T.SAF_AG_STEERING  ");
            sb.AppendLine(",        T.SAF_AG_STOPLIGHTS  ");
            sb.AppendLine(",        T.SAF_AG_EXHAUST  ");
            sb.AppendLine(",        T.SAF_AG_HEADLIGHTS  ");
            sb.AppendLine(",        T.SAF_AG_REFLECTORS  ");
            sb.AppendLine(",        T.SAF_AG_TAILLIGHTS  ");

            sb.AppendLine("     FROM NEW_TESTRECORD T ");


            return sb;
        }

        public static string FormatDetailsSafetyInformation(DataRow row)
        {
            string strSystem, strItem, strSubitem;
            string html = null;

            // Safety BS
            #region Safety BS
            strSystem = "VEHICLE INFO";
            if ((row["SAF_BS_ITEM_3"].ToString()[0] == '2') || (row["SAF_BS_ITEM_3"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, "INT. FUEL TAX AGREEMENT NOT DISPLAYED", "", row["SAF_BS_SUBITEM_2"].ToString());
            strItem = "VIN";
            if ((row["SAF_BS_SUBITEM_1"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_1"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "DIFFERS FROM REGISTRATION", row["SAF_BS_SUBITEM_1"].ToString());
            if ((row["SAF_BS_SUBITEM_2"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_2"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "VIN PLATE MISSING / TAMPERED", row["SAF_BS_SUBITEM_2"].ToString());
            strItem = "REGISTRATION PLATES";
            if ((row["SAF_BS_SUBITEM_3"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_3"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "UNSECURED / IMPROPER PLACEMENT", row["SAF_BS_SUBITEM_3"].ToString());
            if ((row["SAF_BS_SUBITEM_4"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_4"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "MISSING PLATE", row["SAF_BS_SUBITEM_4"].ToString());
            if ((row["SAF_BS_SUBITEM_5"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_5"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "OBSCURED PLATE", row["SAF_BS_SUBITEM_5"].ToString());
            if ((row["SAF_BS_SUBITEM_6"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_6"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "WRONG COLOR", row["SAF_BS_SUBITEM_6"].ToString());
            strItem = "INT. FUEL TAX AGREEMENT NOT DISPLAYED";
            if ((row["SAF_TB_ITEM_3"].ToString()[0] == '2') || (row["SAF_TB_ITEM_3"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_TB_ITEM_3"].ToString());

            strSystem = "WHEELS";
            strItem = "BOLTS / STUDS / LUGS / MISSING / DAMAGED";
            if ((row["SAF_BS_SUBITEM_7"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_7"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LEFT FRONT", row["SAF_BS_SUBITEM_7"].ToString());
            if ((row["SAF_BS_SUBITEM_8"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_8"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "RIGHT FRONT", row["SAF_BS_SUBITEM_8"].ToString());
            if ((row["SAF_BS_SUBITEM_9"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_9"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LEFT REAR", row["SAF_BS_SUBITEM_9"].ToString());
            if ((row["SAF_BS_SUBITEM_10"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_10"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "RIGHT REAR", row["SAF_BS_SUBITEM_10"].ToString());
            strItem = "BENT / CRACKED / REWELDED / DAMAGED";
            if ((row["SAF_BS_SUBITEM_11"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_11"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LEFT FRONT", row["SAF_BS_SUBITEM_11"].ToString());
            if ((row["SAF_BS_SUBITEM_12"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_12"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "RIGHT FRONT", row["SAF_BS_SUBITEM_12"].ToString());
            if ((row["SAF_BS_SUBITEM_13"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_13"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LEFT REAR", row["SAF_BS_SUBITEM_13"].ToString());
            if ((row["SAF_BS_SUBITEM_14"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_14"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "RIGHT REAR", row["SAF_BS_SUBITEM_14"].ToString());

            strSystem = "TIRES";
            strItem = "BREAKS / CUTS / REPAIRS";
            if ((row["SAF_BS_SUBITEM_15"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_15"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LEFT FRONT", row["SAF_BS_SUBITEM_15"].ToString());
            if ((row["SAF_BS_SUBITEM_16"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_16"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "RIGHT FRONT", row["SAF_BS_SUBITEM_16"].ToString());
            if ((row["SAF_BS_SUBITEM_17"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_17"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LEFT REAR", row["SAF_BS_SUBITEM_17"].ToString());
            if ((row["SAF_BS_SUBITEM_18"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_18"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "RIGHT REAR", row["SAF_BS_SUBITEM_18"].ToString());
            strItem = "TREAD DEPTH";
            if ((row["SAF_BS_SUBITEM_19"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_19"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LEFT FRONT", row["SAF_BS_SUBITEM_19"].ToString());
            if ((row["SAF_BS_SUBITEM_20"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_20"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "RIGHT FRONT", row["SAF_BS_SUBITEM_20"].ToString());
            if ((row["SAF_BS_SUBITEM_21"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_21"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LEFT REAR", row["SAF_BS_SUBITEM_21"].ToString());
            if ((row["SAF_BS_SUBITEM_22"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_22"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "RIGHT REAR", row["SAF_BS_SUBITEM_22"].ToString());
            strItem = "REGROOVED";
            if ((row["SAF_BS_SUBITEM_23"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_23"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LEFT FRONT", row["SAF_BS_SUBITEM_23"].ToString());
            if ((row["SAF_BS_SUBITEM_24"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_24"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "RIGHT FRONT", row["SAF_BS_SUBITEM_24"].ToString());
            if ((row["SAF_BS_SUBITEM_25"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_25"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LEFT REAR", row["SAF_BS_SUBITEM_25"].ToString());
            if ((row["SAF_BS_SUBITEM_26"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_26"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "RIGHT REAR", row["SAF_BS_SUBITEM_26"].ToString());
            strItem = "MIS-MATCHED";
            if ((row["SAF_BS_SUBITEM_27"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_27"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "FRONT AXLE", row["SAF_BS_SUBITEM_27"].ToString());
            if ((row["SAF_BS_SUBITEM_28"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_28"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "REAR AXLE", row["SAF_BS_SUBITEM_28"].ToString());
            strItem = "TIRE WIDTH";
            if ((row["SAF_BS_SUBITEM_29"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_29"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LEFT FRONT", row["SAF_BS_SUBITEM_29"].ToString());
            if ((row["SAF_BS_SUBITEM_30"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_30"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "RIGHT FRONT", row["SAF_BS_SUBITEM_30"].ToString());
            if ((row["SAF_BS_SUBITEM_31"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_31"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LEFT REAR", row["SAF_BS_SUBITEM_31"].ToString());
            if ((row["SAF_BS_SUBITEM_32"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_32"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "RIGHT REAR", row["SAF_BS_SUBITEM_32"].ToString());
            strItem = "PROHIBITED";
            if ((row["SAF_BS_SUBITEM_33"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_33"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LEFT FRONT", row["SAF_BS_SUBITEM_33"].ToString());
            if ((row["SAF_BS_SUBITEM_34"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_34"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "RIGHT FRONT", row["SAF_BS_SUBITEM_34"].ToString());
            if ((row["SAF_BS_SUBITEM_35"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_35"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LEFT REAR", row["SAF_BS_SUBITEM_35"].ToString());
            if ((row["SAF_BS_SUBITEM_36"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_36"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "RIGHT REAR", row["SAF_BS_SUBITEM_36"].ToString());
            strItem = "SPRING / STRUT / TORSION BAR";
            if ((row["SAF_BS_SUBITEM_37"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_37"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LEFT FRONT", row["SAF_BS_SUBITEM_37"].ToString());
            if ((row["SAF_BS_SUBITEM_38"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_38"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "RIGHT FRONT", row["SAF_BS_SUBITEM_38"].ToString());
            if ((row["SAF_BS_SUBITEM_39"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_39"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LEFT REAR", row["SAF_BS_SUBITEM_39"].ToString());
            if ((row["SAF_BS_SUBITEM_40"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_40"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "RIGHT REAR", row["SAF_BS_SUBITEM_40"].ToString());
            strItem = "EXCESS BOUNCE";
            if ((row["SAF_BS_SUBITEM_41"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_41"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "FRONT", row["SAF_BS_SUBITEM_41"].ToString());
            if ((row["SAF_BS_SUBITEM_42"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_42"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "REAR", row["SAF_BS_SUBITEM_42"].ToString());
            strItem = "ALIGNMENT";
            if ((row["SAF_BS_SUBITEM_43"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_43"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LOOSE / DAMAGED", row["SAF_BS_SUBITEM_43"].ToString());
            if ((row["SAF_BS_SUBITEM_44"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_44"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "EXCESS CAMBER / CASTER/ TOE-IN", row["SAF_BS_SUBITEM_44"].ToString());
            strItem = "DEFECTIVE STEERING";
            if ((row["SAF_BS_SUBITEM_45"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_45"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "STEERING WHEEL FREEPLAY", row["SAF_BS_SUBITEM_45"].ToString());
            if ((row["SAF_BS_SUBITEM_46"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_46"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "BINDING", row["SAF_BS_SUBITEM_46"].ToString());
            if ((row["SAF_BS_SUBITEM_47"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_47"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "COLUMN LOOSENESS", row["SAF_BS_SUBITEM_47"].ToString());
            if ((row["SAF_BS_SUBITEM_48"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_48"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LEFT", row["SAF_BS_SUBITEM_48"].ToString());
            if ((row["SAF_BS_SUBITEM_49"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_49"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "RIGHT", row["SAF_BS_SUBITEM_49"].ToString());
            strItem = "WHEEL BEARING";
            if ((row["SAF_BS_SUBITEM_50"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_50"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LEFT", row["SAF_BS_SUBITEM_50"].ToString());
            if ((row["SAF_BS_SUBITEM_51"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_51"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "RIGHT", row["SAF_BS_SUBITEM_51"].ToString());
            strItem = "BALL JOINT";
            if ((row["SAF_BS_SUBITEM_52"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_52"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "BROKEN / DAMAGED", row["SAF_BS_SUBITEM_52"].ToString());
            if ((row["SAF_BS_SUBITEM_53"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_53"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "EXCESS WEAR", row["SAF_BS_SUBITEM_52"].ToString());

            strSystem = "FOOT BRAKE";
            if ((row["SAF_BS_ITEM_18"].ToString()[0] == '2') || (row["SAF_BS_ITEM_18"].ToString()[0] == '2')) html += Create3CellTableRow(strSystem, "INADEQUATE STOPPING POWER", "", row["SAF_BS_ITEM_18"].ToString());
            strItem = "NO BRAKING ON WHEEL";
            if ((row["SAF_BS_SUBITEM_54"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_54"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LEFT FRONT", row["SAF_BS_SUBITEM_52"].ToString());
            if ((row["SAF_BS_SUBITEM_55"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_55"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "RIGHT FRONT", row["SAF_BS_SUBITEM_52"].ToString());
            if ((row["SAF_BS_SUBITEM_56"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_56"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LEFT REAR", row["SAF_BS_SUBITEM_52"].ToString());
            if ((row["SAF_BS_SUBITEM_57"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_57"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "RIGHT REAR", row["SAF_BS_SUBITEM_52"].ToString());
            if ((row["SAF_BS_ITEM_20"].ToString()[0] == '2') || (row["SAF_BS_ITEM_20"].ToString()[0] == '2')) html += Create3CellTableRow(strSystem, "EXCESS BRAKE PEDAL TRAVEL", "", row["SAF_BS_ITEM_20"].ToString());
            strItem = "BRAKE WARNING LIGHT";
            if ((row["SAF_BS_SUBITEM_58"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_58"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "DOES NOT LIGHT KOEO", row["SAF_BS_SUBITEM_52"].ToString());
            if ((row["SAF_BS_SUBITEM_59"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_59"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "ILLUMINATES KOER", row["SAF_BS_SUBITEM_59"].ToString());
            if ((row["SAF_BS_SUBITEM_60"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_60"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "ILLUMINATES WHEN PEDAL PRESSED", row["SAF_BS_SUBITEM_60"].ToString());
            strItem = "DEFECTIVE BRAKE LINING";
            if ((row["SAF_BS_SUBITEM_61"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_61"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LEFT FRONT", row["SAF_BS_SUBITEM_61"].ToString());
            if ((row["SAF_BS_SUBITEM_62"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_62"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "RIGHT FRONT", row["SAF_BS_SUBITEM_62"].ToString());
            if ((row["SAF_BS_SUBITEM_63"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_63"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LEFT REAR", row["SAF_BS_SUBITEM_63"].ToString());
            if ((row["SAF_BS_SUBITEM_64"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_64"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "RIGHT REAR", row["SAF_BS_SUBITEM_64"].ToString());
            strItem = "DEFECTIVE DISK OR DRUM";
            if ((row["SAF_BS_SUBITEM_65"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_65"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LEFT FRONT", row["SAF_BS_SUBITEM_65"].ToString());
            if ((row["SAF_BS_SUBITEM_66"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_66"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "RIGHT FRONT", row["SAF_BS_SUBITEM_66"].ToString());
            if ((row["SAF_BS_SUBITEM_67"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_67"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LEFT REAR", row["SAF_BS_SUBITEM_67"].ToString());
            if ((row["SAF_BS_SUBITEM_68"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_68"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "RIGHT REAR", row["SAF_BS_SUBITEM_68"].ToString());
            strItem = "HYDRAULICS";
            if ((row["SAF_BS_SUBITEM_69"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_69"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LOW FLUID", row["SAF_BS_SUBITEM_69"].ToString());
            if ((row["SAF_BS_SUBITEM_70"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_70"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "MASTER CYLINDER LEAK", row["SAF_BS_SUBITEM_70"].ToString());
            if ((row["SAF_BS_SUBITEM_71"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_71"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "WHEEL CYLINDER LEAK", row["SAF_BS_SUBITEM_71"].ToString());
            if ((row["SAF_BS_SUBITEM_72"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_72"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "DEFECTIVE HOSE / TUBING", row["SAF_BS_SUBITEM_72"].ToString());
            strItem = "BOOST SYSTEM";
            if ((row["SAF_BS_SUBITEM_73"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_73"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "VACUUM BOOST", row["SAF_BS_SUBITEM_73"].ToString());
            if ((row["SAF_BS_SUBITEM_74"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_74"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "HYDRAULIC BOOST", row["SAF_BS_SUBITEM_74"].ToString());
            strItem = "LINKAGE";
            if ((row["SAF_BS_SUBITEM_75"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_75"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "EXCESS FRICTION", row["SAF_BS_SUBITEM_75"].ToString());
            if ((row["SAF_BS_SUBITEM_76"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_76"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "MISSING / BROKEN / EXCESS WEAR", row["SAF_BS_SUBITEM_76"].ToString());
            if ((row["SAF_BS_SUBITEM_77"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_77"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "BRAKE LEVERS IMPROPER POSITION / ALIGNMENT", row["SAF_BS_SUBITEM_77"].ToString());

            strSystem = "PARKING BRAKE";
            if ((row["SAF_BS_ITEM_27"].ToString()[0] == '2') || (row["SAF_BS_ITEM_27"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, "INSUFFICIENT BRAKING", "", row["SAF_BS_ITEM_27"].ToString());
            if ((row["SAF_BS_ITEM_28"].ToString()[0] == '2') || (row["SAF_BS_ITEM_28"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, "EXCESS PEDAL / LEVER TRAVEL", "", row["SAF_BS_ITEM_28"].ToString());

            strSystem = "INSTRUMENTS";
            if ((row["SAF_BS_ITEM_29"].ToString()[0] == '2') || (row["SAF_BS_ITEM_29"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, "SPEEDOMETER", "", row["SAF_BS_ITEM_29"].ToString());
            if ((row["SAF_BS_ITEM_30"].ToString()[0] == '2') || (row["SAF_BS_ITEM_30"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, "ODOMETER", "", row["SAF_BS_ITEM_30"].ToString());
            if ((row["SAF_BS_ITEM_31"].ToString()[0] == '2') || (row["SAF_BS_ITEM_31"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, "OPERABLE REAR GEAR", "", row["SAF_BS_ITEM_31"].ToString());
            if ((row["SAF_BS_ITEM_32"].ToString()[0] == '2') || (row["SAF_BS_ITEM_32"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, "OPERABLE PARK POSITION (AUTO TRANS)", "", row["SAF_BS_ITEM_32"].ToString());

            strSystem = "HORN / ELECTRICAL SYSTEM";
            if ((row["SAF_BS_SUBITEM_78"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_78"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "NOT OPERATING", row["SAF_BS_SUBITEM_78"].ToString());
            if ((row["SAF_BS_SUBITEM_79"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_79"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "NOT AUDIBLE 200 FEET AWAY", row["SAF_BS_SUBITEM_79"].ToString());
            if ((row["SAF_BS_SUBITEM_80"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_80"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "NOT FASTENED SECURITY", row["SAF_BS_SUBITEM_80"].ToString());

            strItem = "NEUTRAL STARTER SAFETY SWITCH";
            if ((row["SAF_BS_ITEM_34"].ToString()[0] == '2') || (row["SAF_BS_ITEM_34"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_BS_ITEM_34"].ToString());

            strItem = "ELECTRICAL WIRING AND SYSTEM";
            if ((row["SAF_BS_ITEM_35"].ToString()[0] == '2') || (row["SAF_BS_ITEM_35"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_BS_ITEM_35"].ToString());

            strSystem = "REAR LIGHTS";
            strItem = "LEFT REAR";
            if ((row["SAF_BS_SUBITEM_81"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_81"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "INOPERATIVE", row["SAF_BS_SUBITEM_81"].ToString());
            if ((row["SAF_BS_SUBITEM_82"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_82"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "BROKEN LENS", row["SAF_BS_SUBITEM_82"].ToString());
            if ((row["SAF_BS_SUBITEM_83"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_83"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "BRIGHTNESS", row["SAF_BS_SUBITEM_83"].ToString());
            if ((row["SAF_BS_SUBITEM_84"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_84"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "WRONG COLOR", row["SAF_BS_SUBITEM_84"].ToString());
            strItem = "RIGHT REAR";
            if ((row["SAF_BS_SUBITEM_85"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_85"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "INOPERATIVE", row["SAF_BS_SUBITEM_85"].ToString());
            if ((row["SAF_BS_SUBITEM_86"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_86"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "BROKEN LENS", row["SAF_BS_SUBITEM_86"].ToString());
            if ((row["SAF_BS_SUBITEM_87"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_87"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "BRIGHTNESS", row["SAF_BS_SUBITEM_87"].ToString());
            if ((row["SAF_BS_SUBITEM_88"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_88"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "WRONG COLOR", row["SAF_BS_SUBITEM_88"].ToString());
            strItem = "LICENSE PLATE";
            if ((row["SAF_BS_SUBITEM_89"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_89"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "INOPERATIVE", row["SAF_BS_SUBITEM_89"].ToString());
            if ((row["SAF_BS_SUBITEM_90"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_90"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "BROKEN LENS", row["SAF_BS_SUBITEM_90"].ToString());
            if ((row["SAF_BS_SUBITEM_91"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_91"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "BRIGHTNESS", row["SAF_BS_SUBITEM_91"].ToString());
            if ((row["SAF_BS_SUBITEM_92"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_92"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "WRONG COLOR", row["SAF_BS_SUBITEM_92"].ToString());
            strItem = "CLEARANCE";
            if ((row["SAF_BS_SUBITEM_93"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_93"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "INOPERATIVE", row["SAF_BS_SUBITEM_93"].ToString());
            if ((row["SAF_BS_SUBITEM_94"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_94"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "BROKEN LENS", row["SAF_BS_SUBITEM_94"].ToString());
            if ((row["SAF_BS_SUBITEM_95"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_95"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "BRIGHTNESS", row["SAF_BS_SUBITEM_95"].ToString());
            if ((row["SAF_BS_SUBITEM_96"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_96"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "WRONG COLOR", row["SAF_BS_SUBITEM_96"].ToString());
            strItem = "BACKUP LIGHT";
            if ((row["SAF_BS_SUBITEM_97"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_97"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "INOPERATIVE", row["SAF_BS_SUBITEM_97"].ToString());
            if ((row["SAF_BS_SUBITEM_98"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_98"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "BROKEN LENS", row["SAF_BS_SUBITEM_98"].ToString());
            if ((row["SAF_BS_SUBITEM_99"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_99"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "BRIGHTNESS", row["SAF_BS_SUBITEM_99"].ToString());
            if ((row["SAF_BS_SUBITEM_100"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_100"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "WRONG COLOR", row["SAF_BS_SUBITEM_100"].ToString());

            strSystem = "STOP LIGHTS";
            strItem = "LEFT REAR";
            if ((row["SAF_BS_SUBITEM_101"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_101"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "INOPERATIVE", row["SAF_BS_SUBITEM_101"].ToString());
            if ((row["SAF_BS_SUBITEM_102"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_102"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "BROKEN LENS", row["SAF_BS_SUBITEM_102"].ToString());
            if ((row["SAF_BS_SUBITEM_103"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_103"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "BRIGHTNESS", row["SAF_BS_SUBITEM_103"].ToString());
            if ((row["SAF_BS_SUBITEM_104"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_104"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "WRONG COLOR", row["SAF_BS_SUBITEM_104"].ToString());
            strItem = "RIGHT REAR";
            if ((row["SAF_BS_SUBITEM_105"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_105"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "INOPERATIVE", row["SAF_BS_SUBITEM_105"].ToString());
            if ((row["SAF_BS_SUBITEM_106"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_106"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "BROKEN LENS", row["SAF_BS_SUBITEM_106"].ToString());
            if ((row["SAF_BS_SUBITEM_107"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_107"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "BRIGHTNESS", row["SAF_BS_SUBITEM_107"].ToString());
            if ((row["SAF_BS_SUBITEM_108"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_108"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "WRONG COLOR", row["SAF_BS_SUBITEM_108"].ToString());

            strSystem = "FRONT LIGHTS";
            strItem = "LEFT FRONT";
            if ((row["SAF_BS_SUBITEM_109"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_109"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "INOPERATIVE", row["SAF_BS_SUBITEM_109"].ToString());
            if ((row["SAF_BS_SUBITEM_110"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_110"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "BROKEN LENS", row["SAF_BS_SUBITEM_110"].ToString());
            if ((row["SAF_BS_SUBITEM_111"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_111"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "BRIGHTNESS", row["SAF_BS_SUBITEM_111"].ToString());
            if ((row["SAF_BS_SUBITEM_112"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_112"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "WRONG COLOR", row["SAF_BS_SUBITEM_112"].ToString());
            strItem = "RIGHT FRONT";
            if ((row["SAF_BS_SUBITEM_113"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_113"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "INOPERATIVE", row["SAF_BS_SUBITEM_113"].ToString());
            if ((row["SAF_BS_SUBITEM_114"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_114"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "BROKEN LENS", row["SAF_BS_SUBITEM_114"].ToString());
            if ((row["SAF_BS_SUBITEM_115"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_115"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "BRIGHTNESS", row["SAF_BS_SUBITEM_115"].ToString());
            if ((row["SAF_BS_SUBITEM_116"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_116"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "WRONG COLOR", row["SAF_BS_SUBITEM_116"].ToString());

            strSystem = "DIRECTIONAL SIGNAL";
            strItem = "IMPROPER FUNCTION";
            if ((row["SAF_BS_ITEM_45"].ToString()[0] == '2') || (row["SAF_BS_ITEM_45"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_BS_ITEM_45"].ToString());
            strItem = "IMPROPER POSITION";
            if ((row["SAF_BS_ITEM_46"].ToString()[0] == '2') || (row["SAF_BS_ITEM_46"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_BS_ITEM_46"].ToString());
            strItem = "INOP SELF-CANCELING MECHANISM";
            if ((row["SAF_BS_ITEM_47"].ToString()[0] == '2') || (row["SAF_BS_ITEM_47"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_BS_ITEM_47"].ToString());
            strItem = "OBSCURED";
            if ((row["SAF_BS_ITEM_48"].ToString()[0] == '2') || (row["SAF_BS_ITEM_48"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_BS_ITEM_48"].ToString());
            strItem = "BROKEN / MISSING LAMP / LENS";
            if ((row["SAF_BS_SUBITEM_117"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_117"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LEFT FRONT", row["SAF_BS_SUBITEM_117"].ToString());
            if ((row["SAF_BS_SUBITEM_118"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_118"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "RIGHT FRONT", row["SAF_BS_SUBITEM_118"].ToString());
            if ((row["SAF_BS_SUBITEM_119"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_119"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LEFT REAR", row["SAF_BS_SUBITEM_119"].ToString());
            if ((row["SAF_BS_SUBITEM_120"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_120"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "RIGHT REAR", row["SAF_BS_SUBITEM_120"].ToString());

            strSystem = "OTHER LIGHTS";
            strItem = "EMERGENCY";
            if ((row["SAF_BS_SUBITEM_121"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_121"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "UNAUTHORIZED", row["SAF_BS_SUBITEM_121"].ToString());
            if ((row["SAF_BS_SUBITEM_122"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_122"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "WRONG COLOR", row["SAF_BS_SUBITEM_122"].ToString());
            if ((row["SAF_BS_SUBITEM_123"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_123"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "IMPROPER LOCATION", row["SAF_BS_SUBITEM_123"].ToString());
            strItem = "AUX";
            if ((row["SAF_BS_SUBITEM_124"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_124"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "EXCESS NUMBER", row["SAF_BS_SUBITEM_124"].ToString());
            if ((row["SAF_BS_SUBITEM_125"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_125"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "IMPROPER HEIGHT", row["SAF_BS_SUBITEM_125"].ToString());
            if ((row["SAF_BS_SUBITEM_126"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_126"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "HIGH MOUNTED / NO COVERS", row["SAF_BS_SUBITEM_126"].ToString());

            strSystem = "HEADLIGHT AIM";
            strItem = "HEADLIGHTS";
            if ((row["SAF_BS_SUBITEM_127"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_127"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LEFT LOW BEAM", row["SAF_BS_SUBITEM_127"].ToString());
            if ((row["SAF_BS_SUBITEM_128"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_128"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LEFT LOW BEAM", row["SAF_BS_SUBITEM_128"].ToString());
            if ((row["SAF_BS_SUBITEM_129"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_129"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "RIGHT LOW BEAM", row["SAF_BS_SUBITEM_129"].ToString());
            if ((row["SAF_BS_SUBITEM_130"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_130"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "RIGHT LOW BEAM", row["SAF_BS_SUBITEM_130"].ToString());
            strItem = "AUX LIGHTS";
            if ((row["SAF_BS_ITEM_53"].ToString()[0] == '2') || (row["SAF_BS_ITEM_53"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_BS_ITEM_53"].ToString());
            strItem = "OUTSIDE REAR VIEW";
            if ((row["SAF_BS_ITEM_54"].ToString()[0] == '2') || (row["SAF_BS_ITEM_54"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_BS_ITEM_54"].ToString());
            strItem = "INSIDE REAR VIEW";
            if ((row["SAF_BS_ITEM_55"].ToString()[0] == '2') || (row["SAF_BS_ITEM_55"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_BS_ITEM_55"].ToString());
            strItem = "CHIPPED / BROKEN WINDSHIELD";
            if ((row["SAF_BS_ITEM_56"].ToString()[0] == '2') || (row["SAF_BS_ITEM_56"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_BS_ITEM_56"].ToString());

            strSystem = "MIRRORS";
            if ((row["SAF_BS_MIRRORS"].ToString()[0] == '2') || (row["SAF_BS_MIRRORS"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, "", "", row["SAF_BS_MIRRORS"].ToString());
            strSystem = "DEFROSTER";
            if ((row["SAF_BS_DEFROSTER"].ToString()[0] == '2') || (row["SAF_BS_DEFROSTER"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, "", "", row["SAF_BS_DEFROSTER"].ToString());

            strSystem = "GLASS";
            strItem = "UNAUTHORIZED MATERIALS ON WINDOW";
            if ((row["SAF_BS_SUBITEM_131"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_131"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "FRONT", row["SAF_BS_SUBITEM_131"].ToString());
            if ((row["SAF_BS_SUBITEM_132"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_132"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "REAR", row["SAF_BS_SUBITEM_132"].ToString());
            if ((row["SAF_BS_SUBITEM_133"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_133"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LEFT", row["SAF_BS_SUBITEM_133"].ToString());
            if ((row["SAF_BS_SUBITEM_134"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_134"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "RIGHT", row["SAF_BS_SUBITEM_134"].ToString());
            strItem = "SAFETY GLASS";
            if ((row["SAF_BS_SUBITEM_135"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_135"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "FRONT", row["SAF_BS_SUBITEM_135"].ToString());
            if ((row["SAF_BS_SUBITEM_136"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_136"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "REAR", row["SAF_BS_SUBITEM_136"].ToString());
            if ((row["SAF_BS_SUBITEM_137"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_137"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LEFT", row["SAF_BS_SUBITEM_137"].ToString());
            if ((row["SAF_BS_SUBITEM_138"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_138"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "RIGHT", row["SAF_BS_SUBITEM_138"].ToString());
            strItem = "TINT";
            if ((row["SAF_BS_SUBITEM_139"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_139"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "FRONT", row["SAF_BS_SUBITEM_139"].ToString());
            if ((row["SAF_BS_SUBITEM_140"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_140"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "REAR", row["SAF_BS_SUBITEM_140"].ToString());
            if ((row["SAF_BS_SUBITEM_141"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_141"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LEFT", row["SAF_BS_SUBITEM_141"].ToString());
            if ((row["SAF_BS_SUBITEM_142"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_142"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "RIGHT", row["SAF_BS_SUBITEM_142"].ToString());
            strItem = "REAR BLIND";
            if ((row["SAF_BS_ITEM_60"].ToString()[0] == '2') || (row["SAF_BS_ITEM_60"].ToString()[0] == '2')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_BS_ITEM_60"].ToString());
            strItem = "CURTAINS";
            if ((row["SAF_BS_SUBITEM_143"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_143"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "FRONT", row["SAF_BS_SUBITEM_143"].ToString());
            if ((row["SAF_BS_SUBITEM_144"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_144"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "REAR", row["SAF_BS_SUBITEM_144"].ToString());
            if ((row["SAF_BS_SUBITEM_145"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_145"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LEFT", row["SAF_BS_SUBITEM_145"].ToString());
            if ((row["SAF_BS_SUBITEM_146"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_146"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "RIGHT", row["SAF_BS_SUBITEM_146"].ToString());
            strItem = "DAMAGED / IMPROPER PLASTIC GLAZING";
            if ((row["SAF_BS_SUBITEM_147"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_147"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "FRONT", row["SAF_BS_SUBITEM_147"].ToString());
            if ((row["SAF_BS_SUBITEM_148"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_148"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "REAR", row["SAF_BS_SUBITEM_148"].ToString());
            if ((row["SAF_BS_SUBITEM_149"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_149"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LEFT", row["SAF_BS_SUBITEM_149"].ToString());
            if ((row["SAF_BS_SUBITEM_150"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_150"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "RIGHT", row["SAF_BS_SUBITEM_150"].ToString());
            strItem = "FUNCTION / OPERATION";
            if ((row["SAF_BS_SUBITEM_151"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_151"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LEFT", row["SAF_BS_SUBITEM_151"].ToString());
            if ((row["SAF_BS_SUBITEM_152"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_152"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "RIGHT", row["SAF_BS_SUBITEM_152"].ToString());

            strSystem = "WIPERS";
            strItem = "WORN BLADE(S)";
            if ((row["SAF_BS_ITEM_64"].ToString()[0] == '2') || (row["SAF_BS_ITEM_64"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_BS_ITEM_64"].ToString());
            strItem = "LESS THAN 2 FUNCTIONING SPEEDS";
            if ((row["SAF_BS_ITEM_65"].ToString()[0] == '2') || (row["SAF_BS_ITEM_65"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_BS_ITEM_65"].ToString());
            strItem = "DOES NOT CLEAR";
            if ((row["SAF_BS_ITEM_66"].ToString()[0] == '2') || (row["SAF_BS_ITEM_66"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_BS_ITEM_66"].ToString());


            strSystem = "EXHAUST";
            strItem = "SYSTEM DEFECT";
            if ((row["SAF_BS_SUBITEM_153"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_153"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "MISSING COMPONENT", row["SAF_BS_SUBITEM_153"].ToString());
            if ((row["SAF_BS_SUBITEM_154"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_154"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "DEFECTIVE SUPPORT", row["SAF_BS_SUBITEM_154"].ToString());
            if ((row["SAF_BS_SUBITEM_155"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_155"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LOOSE / LEAKING JOINTS / SEAMS", row["SAF_BS_SUBITEM_155"].ToString());
            if ((row["SAF_BS_SUBITEM_156"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_156"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "NOT FASTENED SECURELY", row["SAF_BS_SUBITEM_156"].ToString());
            if ((row["SAF_BS_SUBITEM_157"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_157"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "TAILPIPE END DAMAGED / PINCHED", row["SAF_BS_SUBITEM_157"].ToString());
            if ((row["SAF_BS_SUBITEM_158"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_158"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "PATCHED MUFFLER", row["SAF_BS_SUBITEM_158"].ToString());
            if ((row["SAF_BS_SUBITEM_159"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_159"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "EXCESS NOISE", row["SAF_BS_SUBITEM_159"].ToString());
            if ((row["SAF_BS_SUBITEM_160"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_160"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LEAKING FUMES", row["SAF_BS_SUBITEM_160"].ToString());
            if ((row["SAF_BS_SUBITEM_161"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_161"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "ENTERS PASSENGER COMPARTMENT / TRUNK", row["SAF_BS_SUBITEM_161"].ToString());
            strItem = "OUTSIDE PIPES";
            if ((row["SAF_BS_SUBITEM_162"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_162"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "EXCESS NOISE / FUMES / OTHER EMISSIONS", row["SAF_BS_SUBITEM_162"].ToString());
            if ((row["SAF_BS_SUBITEM_163"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_163"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "GASSES IN PASSENGER COMPARTMENT", row["SAF_BS_SUBITEM_163"].ToString());
            if ((row["SAF_BS_SUBITEM_164"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_164"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "PROTRUDING EXHAUST WITHOUT HEAT SHIDLDING", row["SAF_BS_SUBITEM_164"].ToString());

            strSystem = "FUEL SYSTEM";
            strItem = "VAPOR / FUEL LEAK";
            if ((row["SAF_BS_ITEM_69"].ToString()[0] == '2') || (row["SAF_BS_ITEM_69"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_BS_ITEM_69"].ToString());
            strItem = "COMPONENTS FASTENED SECURELY";
            if ((row["SAF_BS_ITEM_70"].ToString()[0] == '2') || (row["SAF_BS_ITEM_70"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_BS_ITEM_70"].ToString());
            strItem = "FUEL CAP";
            if ((row["SAF_BS_ITEM_71"].ToString()[0] == '2') || (row["SAF_BS_ITEM_71"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_BS_ITEM_71"].ToString());
            strItem = "UNSECURED TANK";
            if ((row["SAF_BS_ITEM_72"].ToString()[0] == '2') || (row["SAF_BS_ITEM_72"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_BS_ITEM_72"].ToString());
            strItem = "IMPROPER PROPANE FUEL SYSTEM  INSTALLATION";
            if ((row["SAF_BS_ITEM_73"].ToString()[0] == '2') || (row["SAF_BS_ITEM_73"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_BS_ITEM_73"].ToString());

            strSystem = "BUMPERS";
            strItem = "HEIGHT";
            if ((row["SAF_BS_SUBITEM_165"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_165"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "FRONT", row["SAF_BS_SUBITEM_165"].ToString());
            if ((row["SAF_BS_SUBITEM_166"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_166"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "REAR", row["SAF_BS_SUBITEM_166"].ToString());
            strItem = "MISSING / LOOSE / RUSTED";
            if ((row["SAF_BS_SUBITEM_167"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_167"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "FRONT", row["SAF_BS_SUBITEM_167"].ToString());
            if ((row["SAF_BS_SUBITEM_168"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_168"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "REAR", row["SAF_BS_SUBITEM_168"].ToString());
            strItem = "IMPROPER WOODEN";
            if ((row["SAF_BS_SUBITEM_169"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_169"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "FRONT", row["SAF_BS_SUBITEM_169"].ToString());
            if ((row["SAF_BS_SUBITEM_170"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_170"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "REAR", row["SAF_BS_SUBITEM_170"].ToString());
            if ((row["SAF_BS_SUBITEM_171"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_171"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "SIZE", row["SAF_BS_SUBITEM_171"].ToString());
            if ((row["SAF_BS_SUBITEM_172"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_172"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "MATERIAL", row["SAF_BS_SUBITEM_172"].ToString());
            if ((row["SAF_BS_SUBITEM_173"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_173"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "HEIGHT", row["SAF_BS_SUBITEM_173"].ToString());
            if ((row["SAF_BS_SUBITEM_174"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_174"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "OEM ENERGY ABSORBING", row["SAF_BS_SUBITEM_174"].ToString());

            strSystem = "BODY / CHASSIS";
            strItem = "DAMAGED";
            if ((row["SAF_BS_SUBITEM_175"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_175"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "SHARP / PROTRUDING AREA", row["SAF_BS_SUBITEM_175"].ToString());
            if ((row["SAF_BS_SUBITEM_176"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_176"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "MISSING / IMPROPER FENDER", row["SAF_BS_SUBITEM_176"].ToString());
            if ((row["SAF_BS_SUBITEM_177"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_177"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "MISSING / DEFECTIVE DOOR", row["SAF_BS_SUBITEM_177"].ToString());
            if ((row["SAF_BS_SUBITEM_178"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_178"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "MISSING / DAMAGED DOOR LATCH / LOCK / HINGE / HANDLE", row["SAF_BS_SUBITEM_178"].ToString());
            if ((row["SAF_BS_SUBITEM_179"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_179"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "MISSING DEFECTIVE HOOD", row["SAF_BS_SUBITEM_179"].ToString());
            if ((row["SAF_BS_SUBITEM_180"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_180"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "DEFECTIVE HOOD CATCH / RELEASE LATCH", row["SAF_BS_SUBITEM_180"].ToString());
            if ((row["SAF_BS_SUBITEM_181"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_181"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "DEFECTIVE FLOOR PAN", row["SAF_BS_SUBITEM_181"].ToString());
            if ((row["SAF_BS_SUBITEM_182"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_182"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "MISSING / IMPROPER PART", row["SAF_BS_SUBITEM_182"].ToString());
            strItem = "EXHAUST ENTERING PASSENGER AREA";
            if ((row["SAF_BS_SUBITEM_183"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_183"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "DOOR", row["SAF_BS_SUBITEM_183"].ToString());
            if ((row["SAF_BS_SUBITEM_184"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_184"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "WINDOW", row["SAF_BS_SUBITEM_184"].ToString());
            if ((row["SAF_BS_SUBITEM_185"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_185"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "DECK LID", row["SAF_BS_SUBITEM_185"].ToString());
            if ((row["SAF_BS_SUBITEM_186"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_186"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "OTHER", row["SAF_BS_SUBITEM_186"].ToString());
            strItem = "MISSING / IMPROPER BUMPERS";
            if ((row["SAF_BS_SUBITEM_187"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_187"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "FRONT", row["SAF_BS_SUBITEM_187"].ToString());
            if ((row["SAF_BS_SUBITEM_188"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_188"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "REAR", row["SAF_BS_SUBITEM_188"].ToString());
            strItem = "RUST";
            if ((row["SAF_BS_SUBITEM_189"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_189"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "STRUCTURAL RUST", row["SAF_BS_SUBITEM_189"].ToString());
            if ((row["SAF_BS_SUBITEM_190"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_190"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "IMPROPER REPAIR", row["SAF_BS_SUBITEM_190"].ToString());
            strItem = "HOOD SCOOPS";
            if ((row["SAF_BS_SUBITEM_191"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_191"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "HEIGHT / SIZE", row["SAF_BS_SUBITEM_191"].ToString());
            if ((row["SAF_BS_SUBITEM_192"].ToString()[0] == '2') || (row["SAF_BS_SUBITEM_192"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "OBSCURED VIEW", row["SAF_BS_SUBITEM_192"].ToString());

            #endregion

            // Safety TB
            #region Safety TB
            strSystem = "VEHICLE INFO";
            strItem = "VIN";
            if ((row["SAF_TB_SUBITEM_1"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_1"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "DIFFERS FROM REGISTRATION", row["SAF_TB_SUBITEM_1"].ToString());
            if ((row["SAF_TB_SUBITEM_2"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_2"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "VIN PLATE MISSING / TAMPERED", row["SAF_TB_SUBITEM_2"].ToString());
            strItem = "REGISTRATION PLATES";
            if ((row["SAF_TB_SUBITEM_3"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_3"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "UNSECURED / IMPROPER PLACEMENT", row["SAF_TB_SUBITEM_3"].ToString());
            if ((row["SAF_TB_SUBITEM_4"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_4"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "MISSING PLATE", row["SAF_TB_SUBITEM_4"].ToString());
            if ((row["SAF_TB_SUBITEM_5"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_5"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "OBSCURED PLATE", row["SAF_TB_SUBITEM_5"].ToString());
            if ((row["SAF_TB_SUBITEM_6"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_6"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "WRONG COLOR", row["SAF_TB_SUBITEM_6"].ToString());
            strItem = "INT. FUEL TAX AGREEMENT NOT DISPLAYED";
            if ((row["SAF_TB_ITEM_3"].ToString()[0] == '2') || (row["SAF_TB_ITEM_3"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_TB_ITEM_3"].ToString());

            strSystem = "WHEELS";
            strItem = "BOLTS / STUDS / LUGS MISSING / DAMAGED";
            if ((row["SAF_TB_SUBITEM_7"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_7"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LEFT FRONT", row["SAF_TB_SUBITEM_7"].ToString());
            if ((row["SAF_TB_SUBITEM_8"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_8"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "RIGHT FRONT", row["SAF_TB_SUBITEM_8"].ToString());
            if ((row["SAF_TB_SUBITEM_9"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_9"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LEFT REAR", row["SAF_TB_SUBITEM_9"].ToString());
            if ((row["SAF_TB_SUBITEM_10"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_10"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "RIGHT REAR", row["SAF_TB_SUBITEM_10"].ToString());
            strItem = "BENT / CRACKED / RE-WELDED / DAMAGED";
            if ((row["SAF_TB_SUBITEM_11"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_11"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LEFT FRONT", row["SAF_TB_SUBITEM_11"].ToString());
            if ((row["SAF_TB_SUBITEM_12"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_12"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "RIGHT FRONT", row["SAF_TB_SUBITEM_12"].ToString());
            if ((row["SAF_TB_SUBITEM_13"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_13"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LEFT REAR", row["SAF_TB_SUBITEM_13"].ToString());
            if ((row["SAF_TB_SUBITEM_14"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_14"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "RIGHT REAR", row["SAF_TB_SUBITEM_14"].ToString());

            strSystem = "TIRES";
            strItem = "BREAKS / CUTS / REPAIRS";
            if ((row["SAF_TB_SUBITEM_15"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_15"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LEFT FRONT", row["SAF_TB_SUBITEM_15"].ToString());
            if ((row["SAF_TB_SUBITEM_16"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_16"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "RIGHT FRONT", row["SAF_TB_SUBITEM_16"].ToString());
            if ((row["SAF_TB_SUBITEM_17"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_17"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LEFT REAR", row["SAF_TB_SUBITEM_17"].ToString());
            if ((row["SAF_TB_SUBITEM_18"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_18"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "RIGHT REAR", row["SAF_TB_SUBITEM_18"].ToString());
            strItem = "TREAD DEPTH";
            if ((row["SAF_TB_SUBITEM_19"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_19"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LEFT FRONT", row["SAF_TB_SUBITEM_19"].ToString());
            if ((row["SAF_TB_SUBITEM_20"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_20"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "RIGHT FRONT", row["SAF_TB_SUBITEM_20"].ToString());
            if ((row["SAF_TB_SUBITEM_21"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_21"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LEFT REAR", row["SAF_TB_SUBITEM_21"].ToString());
            if ((row["SAF_TB_SUBITEM_22"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_22"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "RIGHT REAR", row["SAF_TB_SUBITEM_22"].ToString());
            strItem = "REGROOVED";
            if ((row["SAF_TB_SUBITEM_23"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_23"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LEFT FRONT", row["SAF_TB_SUBITEM_23"].ToString());
            if ((row["SAF_TB_SUBITEM_24"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_24"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "RIGHT FRONT", row["SAF_TB_SUBITEM_24"].ToString());
            if ((row["SAF_TB_SUBITEM_25"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_25"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LEFT REAR", row["SAF_TB_SUBITEM_25"].ToString());
            if ((row["SAF_TB_SUBITEM_26"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_26"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "RIGHT REAR", row["SAF_TB_SUBITEM_26"].ToString());
            strItem = "MIS-MATCHED";
            if ((row["SAF_TB_SUBITEM_27"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_27"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "FRONT AXLE", row["SAF_TB_SUBITEM_27"].ToString());
            if ((row["SAF_TB_SUBITEM_28"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_28"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "REAR AXLE", row["SAF_TB_SUBITEM_28"].ToString());
            strItem = "TIRE WIDTH";
            if ((row["SAF_TB_SUBITEM_29"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_29"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LEFT FRONT", row["SAF_TB_SUBITEM_29"].ToString());
            if ((row["SAF_TB_SUBITEM_30"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_30"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "RIGHT FRONT", row["SAF_TB_SUBITEM_30"].ToString());
            if ((row["SAF_TB_SUBITEM_31"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_31"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LEFT REAR", row["SAF_TB_SUBITEM_31"].ToString());
            if ((row["SAF_TB_SUBITEM_32"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_32"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "RIGHT REAR", row["SAF_TB_SUBITEM_32"].ToString());
            strItem = "PROHIBITED";
            if ((row["SAF_TB_SUBITEM_33"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_33"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LEFT FRONT", row["SAF_TB_SUBITEM_33"].ToString());
            if ((row["SAF_TB_SUBITEM_34"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_34"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "RIGHT FRONT", row["SAF_TB_SUBITEM_34"].ToString());
            if ((row["SAF_TB_SUBITEM_35"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_35"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LEFT REAR", row["SAF_TB_SUBITEM_35"].ToString());
            if ((row["SAF_TB_SUBITEM_36"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_36"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "RIGHT REAR", row["SAF_TB_SUBITEM_36"].ToString());
            strItem = "MISSING / IMPROPER TIRE FLAPS";
            if ((row["SAF_TB_SUBITEM_37"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_37"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LEFT", row["SAF_TB_SUBITEM_37"].ToString());
            if ((row["SAF_TB_SUBITEM_38"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_38"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "RIGHT", row["SAF_TB_SUBITEM_38"].ToString());
            strItem = "IMPROPER SPACED DUALS";
            if ((row["SAF_TB_ITEM_13"].ToString()[0] == '2') || (row["SAF_TB_ITEM_13"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_TB_ITEM_13"].ToString());
            strItem = "IMPROPER PRESSURE";
            if ((row["SAF_TB_ITEM_14"].ToString()[0] == '2') || (row["SAF_TB_ITEM_14"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_TB_ITEM_14"].ToString());
            strItem = "DEFICIENT VALVE STEM";
            if ((row["SAF_TB_ITEM_15"].ToString()[0] == '2') || (row["SAF_TB_ITEM_15"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_TB_ITEM_15"].ToString());
            strItem = "DAMAGED LOCK RING";
            if ((row["SAF_TB_ITEM_16"].ToString()[0] == '2') || (row["SAF_TB_ITEM_16"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_TB_ITEM_16"].ToString());

            strSystem = "STEERING / SUSPENSION";
            strItem = "SPRING / STRUT / TORSION BAR";
            if ((row["SAF_TB_SUBITEM_39"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_39"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LEFT FRONT", row["SAF_TB_SUBITEM_39"].ToString());
            if ((row["SAF_TB_SUBITEM_40"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_40"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "RIGHT FRONT", row["SAF_TB_SUBITEM_40"].ToString());
            if ((row["SAF_TB_SUBITEM_41"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_41"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LEFT REAR", row["SAF_TB_SUBITEM_41"].ToString());
            if ((row["SAF_TB_SUBITEM_42"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_42"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "RIGHT REAR", row["SAF_TB_SUBITEM_42"].ToString());
            strItem = "EXCESS BOUNCE";
            if ((row["SAF_TB_SUBITEM_43"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_43"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "FRONT", row["SAF_TB_SUBITEM_43"].ToString());
            if ((row["SAF_TB_SUBITEM_44"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_44"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "REAR", row["SAF_TB_SUBITEM_44"].ToString());
            strItem = "ALIGNMENT";
            if ((row["SAF_TB_SUBITEM_45"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_45"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LOOSE / DAMAGED", row["SAF_TB_SUBITEM_45"].ToString());
            if ((row["SAF_TB_SUBITEM_46"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_46"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "EXCESS CAMBER / CASTER / TOE-IN", row["SAF_TB_SUBITEM_46"].ToString());
            strItem = "DEFECTIVE STEERING";
            if ((row["SAF_TB_SUBITEM_47"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_47"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "STEERING WHEEL FREEPLAY", row["SAF_TB_SUBITEM_47"].ToString());
            if ((row["SAF_TB_SUBITEM_48"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_48"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LINKAGE FREEPLAY", row["SAF_TB_SUBITEM_48"].ToString());
            if ((row["SAF_TB_SUBITEM_49"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_49"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "BINDING", row["SAF_TB_SUBITEM_49"].ToString());
            if ((row["SAF_TB_SUBITEM_50"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_50"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "COLUMN LOOSENESS", row["SAF_TB_SUBITEM_50"].ToString());
            if ((row["SAF_TB_SUBITEM_51"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_51"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "STEERING BOX LOOSENESS", row["SAF_TB_SUBITEM_51"].ToString());
            strItem = "WHEEL BEARING";
            if ((row["SAF_TB_SUBITEM_52"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_52"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LEFT", row["SAF_TB_SUBITEM_52"].ToString());
            if ((row["SAF_TB_SUBITEM_53"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_53"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "RIGHT", row["SAF_TB_SUBITEM_53"].ToString());
            strItem = "BALL JOINT";
            if ((row["SAF_TB_SUBITEM_54"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_54"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "BROKEN DAMAGED", row["SAF_TB_SUBITEM_54"].ToString());
            if ((row["SAF_TB_SUBITEM_55"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_55"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "EXCESS WEAR", row["SAF_TB_SUBITEM_55"].ToString());

            strSystem = "FOOT BRAKE";
            strItem = "INADEQUATE STOPPING POWER";
            if ((row["SAF_TB_ITEM_23"].ToString()[0] == '2') || (row["SAF_TB_ITEM_23"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_TB_ITEM_23"].ToString());
            strItem = "NO BRAKING ON A WHEEL";
            if ((row["SAF_TB_SUBITEM_56"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_56"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LEFT FRONT", row["SAF_TB_SUBITEM_56"].ToString());
            if ((row["SAF_TB_SUBITEM_57"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_57"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "RIGHT FRONT", row["SAF_TB_SUBITEM_57"].ToString());
            if ((row["SAF_TB_SUBITEM_58"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_58"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LEFT REAR", row["SAF_TB_SUBITEM_58"].ToString());
            if ((row["SAF_TB_SUBITEM_59"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_59"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "RIGHT REAR", row["SAF_TB_SUBITEM_59"].ToString());
            strItem = "EXCESS BRAKE PEDAL TRAVEL";
            if ((row["SAF_TB_ITEM_25"].ToString()[0] == '2') || (row["SAF_TB_ITEM_25"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_TB_ITEM_25"].ToString());
            strItem = "BRAKE WARN LIGHT";
            if ((row["SAF_TB_SUBITEM_60"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_60"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "DOES NOT LIGHT KOEO", row["SAF_TB_SUBITEM_60"].ToString());
            if ((row["SAF_TB_SUBITEM_61"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_61"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "ILLUMINATES KOER", row["SAF_TB_SUBITEM_61"].ToString());
            if ((row["SAF_TB_SUBITEM_62"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_62"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "ILLUMINATES WHEN PEDAL PRESSED", row["SAF_TB_SUBITEM_62"].ToString());
            strItem = "DEFECTIVE BRAKE LINING";
            if ((row["SAF_TB_SUBITEM_63"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_63"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LEFT FRONT", row["SAF_TB_SUBITEM_63"].ToString());
            if ((row["SAF_TB_SUBITEM_64"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_64"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "RIGHT FRONT", row["SAF_TB_SUBITEM_64"].ToString());
            if ((row["SAF_TB_SUBITEM_65"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_65"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LEFT REAR", row["SAF_TB_SUBITEM_65"].ToString());
            if ((row["SAF_TB_SUBITEM_66"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_66"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "RIGHT REAR", row["SAF_TB_SUBITEM_66"].ToString());
            strItem = "DEFECTIVE DISK OR DRUM";
            if ((row["SAF_TB_SUBITEM_67"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_67"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LEFT FRONT", row["SAF_TB_SUBITEM_67"].ToString());
            if ((row["SAF_TB_SUBITEM_68"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_68"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "RIGHT FRONT", row["SAF_TB_SUBITEM_68"].ToString());
            if ((row["SAF_TB_SUBITEM_69"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_69"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LEFT REAR", row["SAF_TB_SUBITEM_69"].ToString());
            if ((row["SAF_TB_SUBITEM_70"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_70"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "RIGHT REAR", row["SAF_TB_SUBITEM_70"].ToString());
            strItem = "HYDRAULICS";
            if ((row["SAF_TB_SUBITEM_71"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_71"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LOW FLUID", row["SAF_TB_SUBITEM_71"].ToString());
            if ((row["SAF_TB_SUBITEM_72"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_72"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "MASTER CYLINDER LEAK", row["SAF_TB_SUBITEM_72"].ToString());
            if ((row["SAF_TB_SUBITEM_73"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_73"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "WHEEL CYLINDER LEAK", row["SAF_TB_SUBITEM_73"].ToString());
            if ((row["SAF_TB_SUBITEM_74"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_74"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "DEFECTIVE HOSE / TUBING", row["SAF_TB_SUBITEM_74"].ToString());
            strItem = "BOOST SYSTEM";
            if ((row["SAF_TB_SUBITEM_75"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_75"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "VACUUM BOOST", row["SAF_TB_SUBITEM_75"].ToString());
            if ((row["SAF_TB_SUBITEM_76"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_76"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "HYDRAULIC BOOST", row["SAF_TB_SUBITEM_76"].ToString());
            strItem = "LINKAGE";
            if ((row["SAF_TB_SUBITEM_77"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_77"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "EXCESS FRICTION", row["SAF_TB_SUBITEM_77"].ToString());
            if ((row["SAF_TB_SUBITEM_78"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_78"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "MISSING / BROKEN / EXCESS WEAR", row["SAF_TB_SUBITEM_78"].ToString());
            if ((row["SAF_TB_SUBITEM_79"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_79"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "IMPROPER POSITION / ALIGNMENT", row["SAF_TB_SUBITEM_79"].ToString());

            strSystem = "PARKING BRAKE";
            strItem = "INSUFFICIENT BRAKING";
            if ((row["SAF_TB_ITEM_32"].ToString()[0] == '2') || (row["SAF_TB_ITEM_32"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_TB_ITEM_32"].ToString());
            strItem = "EXCESS PEDAL / LEVER TRAVEL";
            if ((row["SAF_TB_ITEM_33"].ToString()[0] == '2') || (row["SAF_TB_ITEM_33"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_TB_ITEM_33"].ToString());
            strItem = "DRIVER CONTROL";
            if ((row["SAF_TB_SUBITEM_80"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_80"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "MALFUNCTION", row["SAF_TB_SUBITEM_80"].ToString());
            if ((row["SAF_TB_SUBITEM_81"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_81"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LOCATION", row["SAF_TB_SUBITEM_81"].ToString());

            strSystem = "AIR BRAKE SYSTEM";
            strItem = "GOVERNOR";
            if ((row["SAF_TB_ITEM_35"].ToString()[0] == '2') || (row["SAF_TB_ITEM_35"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_TB_ITEM_35"].ToString());
            strItem = "PRESSURE WARNING DEVICE";
            if ((row["SAF_TB_ITEM_36"].ToString()[0] == '2') || (row["SAF_TB_ITEM_36"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_TB_ITEM_36"].ToString());
            strItem = "EXCESS LEAK";
            if ((row["SAF_TB_ITEM_37"].ToString()[0] == '2') || (row["SAF_TB_ITEM_37"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_TB_ITEM_37"].ToString());
            strItem = "PUSH ROD / SLACK ADJUSTER OUT OF TOLERANCE";
            if ((row["SAF_TB_ITEM_38"].ToString()[0] == '2') || (row["SAF_TB_ITEM_38"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_TB_ITEM_38"].ToString());
            strItem = "EXCESS BREAK SHOE TRAVEL";
            if ((row["SAF_TB_ITEM_39"].ToString()[0] == '2') || (row["SAF_TB_ITEM_39"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_TB_ITEM_39"].ToString());
            strItem = "AIR COMPRESSOR DEFICIENCY";
            if ((row["SAF_TB_ITEM_40"].ToString()[0] == '2') || (row["SAF_TB_ITEM_40"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_TB_ITEM_40"].ToString());
            strItem = "AIR TANK DEFICIENCY";
            if ((row["SAF_TB_ITEM_41"].ToString()[0] == '2') || (row["SAF_TB_ITEM_41"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_TB_ITEM_41"].ToString());
            strItem = "FLEXIBLE BRAKE HOSE DEFICIENCY";
            if ((row["SAF_TB_ITEM_42"].ToString()[0] == '2') || (row["SAF_TB_ITEM_42"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_TB_ITEM_42"].ToString());
            strItem = "RIGID BRAKE LINE DEFICIENCY";
            if ((row["SAF_TB_ITEM_43"].ToString()[0] == '2') || (row["SAF_TB_ITEM_43"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_TB_ITEM_43"].ToString());
            strItem = "CLOGGED AIR INTAKE CLEANER";
            if ((row["SAF_TB_ITEM_44"].ToString()[0] == '2') || (row["SAF_TB_ITEM_44"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_TB_ITEM_44"].ToString());

            strSystem = "INSTRUMENTS";
            strItem = "SPEEDOMETER";
            if ((row["SAF_TB_ITEM_45"].ToString()[0] == '2') || (row["SAF_TB_ITEM_45"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_TB_ITEM_45"].ToString());
            strItem = "ODOMETER";
            if ((row["SAF_TB_ITEM_46"].ToString()[0] == '2') || (row["SAF_TB_ITEM_46"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_TB_ITEM_46"].ToString());
            strItem = "OPERABLE REAR GEAR";
            if ((row["SAF_TB_ITEM_47"].ToString()[0] == '2') || (row["SAF_TB_ITEM_47"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_TB_ITEM_47"].ToString());
            strItem = "OPERABLE PARK POSITION (AUTO TRANS)";
            if ((row["SAF_TB_ITEM_48"].ToString()[0] == '2') || (row["SAF_TB_ITEM_48"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_TB_ITEM_48"].ToString());

            strSystem = "HORN / ELECTRICAL SYSTEM";
            strItem = "ELECTRICAL WIRING AND SYSTEM";
            if ((row["SAF_TB_SUBITEM_82"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_82"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "NOT OPERATING", row["SAF_TB_SUBITEM_82"].ToString());
            if ((row["SAF_TB_SUBITEM_83"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_83"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "NOT AUDIBLE 200 FEET AWAY", row["SAF_TB_SUBITEM_83"].ToString());
            if ((row["SAF_TB_SUBITEM_84"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_84"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "NOT FASTENED SECURELY", row["SAF_TB_SUBITEM_84"].ToString());

            strSystem = "REAR LIGHTS";
            strItem = "LEFT REAR";
            if ((row["SAF_TB_SUBITEM_85"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_85"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "INOPERATIVE", row["SAF_TB_SUBITEM_85"].ToString());
            if ((row["SAF_TB_SUBITEM_86"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_86"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "BROKEN LENS", row["SAF_TB_SUBITEM_86"].ToString());
            if ((row["SAF_TB_SUBITEM_87"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_87"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "BRIGHTNESS", row["SAF_TB_SUBITEM_87"].ToString());
            if ((row["SAF_TB_SUBITEM_88"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_88"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "WRONG COLOR", row["SAF_TB_SUBITEM_88"].ToString());
            strItem = "RIGHT REAR";
            if ((row["SAF_TB_SUBITEM_89"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_89"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "INOPERATIVE", row["SAF_TB_SUBITEM_89"].ToString());
            if ((row["SAF_TB_SUBITEM_90"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_90"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "BROKEN LENS", row["SAF_TB_SUBITEM_90"].ToString());
            if ((row["SAF_TB_SUBITEM_91"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_91"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "BRIGHTNESS", row["SAF_TB_SUBITEM_91"].ToString());
            if ((row["SAF_TB_SUBITEM_92"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_92"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "WRONG COLOR", row["SAF_TB_SUBITEM_92"].ToString());
            strItem = "LICENSE PLATE";
            if ((row["SAF_TB_SUBITEM_93"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_93"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "INOPERATIVE", row["SAF_TB_SUBITEM_93"].ToString());
            if ((row["SAF_TB_SUBITEM_94"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_94"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "BROKEN LENS", row["SAF_TB_SUBITEM_94"].ToString());
            if ((row["SAF_TB_SUBITEM_95"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_95"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "BRIGHTNESS", row["SAF_TB_SUBITEM_95"].ToString());
            if ((row["SAF_TB_SUBITEM_96"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_96"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "WRONG COLOR", row["SAF_TB_SUBITEM_96"].ToString());
            strItem = "CLEARANCE";
            if ((row["SAF_TB_SUBITEM_97"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_97"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "INOPERATIVE", row["SAF_TB_SUBITEM_97"].ToString());
            if ((row["SAF_TB_SUBITEM_98"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_98"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "BROKEN LENS", row["SAF_TB_SUBITEM_98"].ToString());
            if ((row["SAF_TB_SUBITEM_99"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_99"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "BRIGHTNESS", row["SAF_TB_SUBITEM_99"].ToString());
            if ((row["SAF_TB_SUBITEM_100"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_100"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "WRONG COLOR", row["SAF_TB_SUBITEM_100"].ToString());
            strItem = "BACKUP LIGHT";
            if ((row["SAF_TB_SUBITEM_101"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_101"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "INOPERATIVE", row["SAF_TB_SUBITEM_101"].ToString());
            if ((row["SAF_TB_SUBITEM_102"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_102"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "BROKEN LENS", row["SAF_TB_SUBITEM_102"].ToString());
            if ((row["SAF_TB_SUBITEM_103"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_103"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "BRIGHTNESS", row["SAF_TB_SUBITEM_103"].ToString());
            if ((row["SAF_TB_SUBITEM_104"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_104"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "WRONG COLOR", row["SAF_TB_SUBITEM_104"].ToString());

            strSystem = "STOP LIGHTS";
            strItem = "LEFT REAR";
            if ((row["SAF_TB_SUBITEM_105"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_105"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "INOPERATIVE", row["SAF_TB_SUBITEM_105"].ToString());
            if ((row["SAF_TB_SUBITEM_106"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_106"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "BROKEN LENS", row["SAF_TB_SUBITEM_106"].ToString());
            if ((row["SAF_TB_SUBITEM_107"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_107"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "BRIGHTNESS", row["SAF_TB_SUBITEM_107"].ToString());
            if ((row["SAF_TB_SUBITEM_108"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_108"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "WRONG COLOR", row["SAF_TB_SUBITEM_108"].ToString());
            strItem = "RIGHT REAR";
            if ((row["SAF_TB_SUBITEM_109"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_109"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "INOPERATIVE", row["SAF_TB_SUBITEM_109"].ToString());
            if ((row["SAF_TB_SUBITEM_110"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_110"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "BROKEN LENS", row["SAF_TB_SUBITEM_110"].ToString());
            if ((row["SAF_TB_SUBITEM_111"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_111"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "BRIGHTNESS", row["SAF_TB_SUBITEM_111"].ToString());
            if ((row["SAF_TB_SUBITEM_112"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_112"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "WRONG COLOR", row["SAF_TB_SUBITEM_112"].ToString());

            strSystem = "FRONT LIGHTS";
            strItem = "LEFT FRONT";
            if ((row["SAF_TB_SUBITEM_113"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_113"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "INOPERATIVE", row["SAF_TB_SUBITEM_113"].ToString());
            if ((row["SAF_TB_SUBITEM_114"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_114"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "BROKEN LENS", row["SAF_TB_SUBITEM_114"].ToString());
            if ((row["SAF_TB_SUBITEM_115"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_115"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "BRIGHTNESS", row["SAF_TB_SUBITEM_115"].ToString());
            if ((row["SAF_TB_SUBITEM_116"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_116"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "WRONG COLOR", row["SAF_TB_SUBITEM_116"].ToString());
            strItem = "RIGHT FRONT";
            if ((row["SAF_TB_SUBITEM_117"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_117"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "INOPERATIVE", row["SAF_TB_SUBITEM_117"].ToString());
            if ((row["SAF_TB_SUBITEM_118"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_118"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "BROKEN LENS", row["SAF_TB_SUBITEM_118"].ToString());
            if ((row["SAF_TB_SUBITEM_119"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_119"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "BRIGHTNESS", row["SAF_TB_SUBITEM_119"].ToString());
            if ((row["SAF_TB_SUBITEM_120"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_120"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "WRONG COLOR", row["SAF_TB_SUBITEM_120"].ToString());

            strSystem = "DIRECTIONAL SIGNAL";
            strItem = "IMPROPER FUNCTION";
            if ((row["SAF_TB_ITEM_61"].ToString()[0] == '2') || (row["SAF_TB_ITEM_61"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_TB_ITEM_61"].ToString());
            strItem = "IMPROPER POSITION";
            if ((row["SAF_TB_ITEM_62"].ToString()[0] == '2') || (row["SAF_TB_ITEM_62"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_TB_ITEM_62"].ToString());
            strItem = "INOP SELF-CANCELING MECHANISM";
            if ((row["SAF_TB_ITEM_63"].ToString()[0] == '2') || (row["SAF_TB_ITEM_63"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_TB_ITEM_63"].ToString());
            strItem = "OBSCURED";
            if ((row["SAF_TB_ITEM_64"].ToString()[0] == '2') || (row["SAF_TB_ITEM_64"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_TB_ITEM_64"].ToString());
            strItem = "BROKEN / MISSING LAMP / LENS";
            if ((row["SAF_TB_SUBITEM_121"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_121"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LEFT FRONT", row["SAF_TB_SUBITEM_121"].ToString());
            if ((row["SAF_TB_SUBITEM_122"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_122"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "RIGHT FRONT", row["SAF_TB_SUBITEM_122"].ToString());
            if ((row["SAF_TB_SUBITEM_123"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_123"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LEFT REAR", row["SAF_TB_SUBITEM_123"].ToString());
            if ((row["SAF_TB_SUBITEM_124"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_124"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "RIGHT REAR", row["SAF_TB_SUBITEM_124"].ToString());

            strSystem = "OTHER LIGHTS";
            strItem = "EMERGENCY";
            if ((row["SAF_TB_SUBITEM_125"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_125"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "UNAUTHORIZED", row["SAF_TB_SUBITEM_125"].ToString());
            if ((row["SAF_TB_SUBITEM_126"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_126"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "WRONG COLOR", row["SAF_TB_SUBITEM_126"].ToString());
            if ((row["SAF_TB_SUBITEM_127"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_127"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "IMPROPER LOCATION", row["SAF_TB_SUBITEM_127"].ToString());
            strItem = "AUX";
            if ((row["SAF_TB_SUBITEM_128"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_128"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "EXCESS NUMBER", row["SAF_TB_SUBITEM_128"].ToString());
            if ((row["SAF_TB_SUBITEM_129"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_129"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "IMPROPER HEIGHT", row["SAF_TB_SUBITEM_129"].ToString());
            if ((row["SAF_TB_SUBITEM_130"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_130"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "HIGH MOUNTED / NO COVERS", row["SAF_TB_SUBITEM_130"].ToString());

            strSystem = "HEADLIGHT";
            strItem = "HEADLIGHTS";
            if ((row["SAF_TB_SUBITEM_131"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_131"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LEFT LOW BEAM", row["SAF_TB_SUBITEM_131"].ToString());
            if ((row["SAF_TB_SUBITEM_132"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_132"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LEFT HIGH BEAM", row["SAF_TB_SUBITEM_132"].ToString());
            if ((row["SAF_TB_SUBITEM_133"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_133"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "RIGHT LOW BEAM", row["SAF_TB_SUBITEM_133"].ToString());
            if ((row["SAF_TB_SUBITEM_134"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_134"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "RIGHT LOW BEAM", row["SAF_TB_SUBITEM_134"].ToString());
            strItem = "AUX LIGHTS";
            if ((row["SAF_TB_ITEM_69"].ToString()[0] == '2') || (row["SAF_TB_ITEM_69"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_TB_ITEM_69"].ToString());

            strSystem = "MIRRORS";
            strItem = "OUTSIDE REAR VIEW";
            if ((row["SAF_TB_ITEM_70"].ToString()[0] == '2') || (row["SAF_TB_ITEM_70"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_TB_ITEM_70"].ToString());
            strItem = "INSIDE REAR VIEW";
            if ((row["SAF_TB_ITEM_71"].ToString()[0] == '2') || (row["SAF_TB_ITEM_71"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_TB_ITEM_71"].ToString());

            strSystem = "DEFROSTER";
            if ((row["SAF_TB_DEFROSTER"].ToString()[0] == '2') || (row["SAF_TB_DEFROSTER"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, "", "", row["SAF_TB_DEFROSTER"].ToString());

            strSystem = "GLASS";
            strItem = "CHIPPED / BROKEN WINDSHIELD";
            if ((row["SAF_TB_ITEM_72"].ToString()[0] == '2') || (row["SAF_TB_ITEM_72"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_TB_ITEM_72"].ToString());
            strItem = "UNAUTHORIZED MATERIALS ON WINDOW";
            if ((row["SAF_TB_SUBITEM_135"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_135"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "FRONT", row["SAF_TB_SUBITEM_135"].ToString());
            if ((row["SAF_TB_SUBITEM_136"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_136"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "REAR", row["SAF_TB_SUBITEM_136"].ToString());
            if ((row["SAF_TB_SUBITEM_137"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_137"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LEFT", row["SAF_TB_SUBITEM_137"].ToString());
            if ((row["SAF_TB_SUBITEM_138"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_138"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "RIGHT", row["SAF_TB_SUBITEM_138"].ToString());
            strItem = "SAFETY GLASS";
            if ((row["SAF_TB_SUBITEM_139"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_139"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "FRONT", row["SAF_TB_SUBITEM_139"].ToString());
            if ((row["SAF_TB_SUBITEM_140"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_140"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "REAR", row["SAF_TB_SUBITEM_140"].ToString());
            if ((row["SAF_TB_SUBITEM_141"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_141"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LEFT", row["SAF_TB_SUBITEM_141"].ToString());
            if ((row["SAF_TB_SUBITEM_142"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_142"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "RIGHT", row["SAF_TB_SUBITEM_142"].ToString());
            strItem = "TINT";
            if ((row["SAF_TB_SUBITEM_143"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_143"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "FRONT", row["SAF_TB_SUBITEM_143"].ToString());
            if ((row["SAF_TB_SUBITEM_144"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_144"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "REAR", row["SAF_TB_SUBITEM_144"].ToString());
            if ((row["SAF_TB_SUBITEM_145"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_145"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LEFT", row["SAF_TB_SUBITEM_145"].ToString());
            if ((row["SAF_TB_SUBITEM_146"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_146"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "RIGHT", row["SAF_TB_SUBITEM_146"].ToString());
            strItem = "REAR BLIND";
            if ((row["SAF_TB_ITEM_76"].ToString()[0] == '2') || (row["SAF_TB_ITEM_76"].ToString()[0] == '2')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_TB_ITEM_76"].ToString());
            strItem = "CURTAINS";
            if ((row["SAF_TB_SUBITEM_147"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_147"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "FRONT", row["SAF_TB_SUBITEM_147"].ToString());
            if ((row["SAF_TB_SUBITEM_148"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_148"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "REAR", row["SAF_TB_SUBITEM_148"].ToString());
            if ((row["SAF_TB_SUBITEM_149"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_149"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LEFT", row["SAF_TB_SUBITEM_149"].ToString());
            if ((row["SAF_TB_SUBITEM_150"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_150"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "RIGHT", row["SAF_TB_SUBITEM_150"].ToString());
            strItem = "DAMAGED / IMPROPER PLASTIC GLAZING";
            if ((row["SAF_TB_SUBITEM_151"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_151"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "FRONT", row["SAF_TB_SUBITEM_151"].ToString());
            if ((row["SAF_TB_SUBITEM_152"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_152"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "REAR", row["SAF_TB_SUBITEM_152"].ToString());
            if ((row["SAF_TB_SUBITEM_153"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_153"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LEFT", row["SAF_TB_SUBITEM_153"].ToString());
            if ((row["SAF_TB_SUBITEM_154"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_154"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "RIGHT", row["SAF_TB_SUBITEM_154"].ToString());
            strItem = "FUNCTION / OPERATION";
            if ((row["SAF_TB_SUBITEM_155"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_155"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LEFT", row["SAF_TB_SUBITEM_155"].ToString());
            if ((row["SAF_TB_SUBITEM_156"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_156"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "RIGHT", row["SAF_TB_SUBITEM_156"].ToString());

            strSystem = "WIPERS";
            strItem = "WORN BLADE";
            if ((row["SAF_TB_ITEM_80"].ToString()[0] == '2') || (row["SAF_TB_ITEM_80"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_TB_ITEM_80"].ToString());
            strItem = "2 OR MORE FUNCTIONING SPEEDS";
            if ((row["SAF_TB_ITEM_81"].ToString()[0] == '2') || (row["SAF_TB_ITEM_81"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_TB_ITEM_81"].ToString());
            strItem = "DOES NOT CLEAR";
            if ((row["SAF_TB_ITEM_82"].ToString()[0] == '2') || (row["SAF_TB_ITEM_82"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_TB_ITEM_82"].ToString());

            strSystem = "REFLECTIVE WARNING DEVICE";
            if ((row["SAF_TB_REFLECTOR"].ToString()[0] == '2') || (row["SAF_TB_REFLECTOR"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, "", "", row["SAF_TB_REFLECTOR"].ToString());

            strSystem = "FIRE EXTINGUISHER";
            if ((row["SAF_TB_FIREEXT"].ToString()[0] == '2') || (row["SAF_TB_FIREEXT"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, "", "", row["SAF_TB_FIREEXT"].ToString());

            strSystem = "EXHAUST";
            strItem = "SYSTEM DEFECT";
            if ((row["SAF_TB_SUBITEM_157"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_157"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "MISSING COMPONENT", row["SAF_TB_SUBITEM_157"].ToString());
            if ((row["SAF_TB_SUBITEM_158"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_158"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "DEFECTIVE SUPPORT", row["SAF_TB_SUBITEM_158"].ToString());
            if ((row["SAF_TB_SUBITEM_159"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_159"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LOOSE / LEAKING JOINT / SEAMS", row["SAF_TB_SUBITEM_159"].ToString());
            if ((row["SAF_TB_SUBITEM_160"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_160"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "NOT FASTENED SECURELY", row["SAF_TB_SUBITEM_160"].ToString());
            if ((row["SAF_TB_SUBITEM_161"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_161"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "TAILPIE END DAMAGED / PINCHED", row["SAF_TB_SUBITEM_161"].ToString());
            if ((row["SAF_TB_SUBITEM_162"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_162"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "PATCHED MUFFLER", row["SAF_TB_SUBITEM_162"].ToString());
            if ((row["SAF_TB_SUBITEM_163"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_163"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "EXCESS NOISE", row["SAF_TB_SUBITEM_163"].ToString());
            if ((row["SAF_TB_SUBITEM_164"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_164"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LEAKING FUMES", row["SAF_TB_SUBITEM_164"].ToString());
            if ((row["SAF_TB_SUBITEM_165"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_165"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "ENTERS PASSENGER COMPARTMENT / TRUNK", row["SAF_TB_SUBITEM_165"].ToString());
            strItem = "OUTSIDE PIPES";
            if ((row["SAF_TB_SUBITEM_166"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_166"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "EXCESS NOISE / FUMES/ OTHER EMISSIONS", row["SAF_TB_SUBITEM_166"].ToString());
            if ((row["SAF_TB_SUBITEM_167"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_167"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "GASSES IN PASSENGER COMPARTMENT", row["SAF_TB_SUBITEM_167"].ToString());
            if ((row["SAF_TB_SUBITEM_168"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_168"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "PROTRUDING EXHAUST WITHOUT HEAT SHIELDING", row["SAF_TB_SUBITEM_168"].ToString());

            strSystem = "FUEL SYSTEM";
            strItem = "VAPOR / FUEL LEAK";
            if ((row["SAF_TB_ITEM_85"].ToString()[0] == '2') || (row["SAF_TB_ITEM_85"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_TB_ITEM_85"].ToString());
            strItem = "COMPONENTS NOT FASTENED SECURELY";
            if ((row["SAF_TB_ITEM_86"].ToString()[0] == '2') || (row["SAF_TB_ITEM_86"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_TB_ITEM_86"].ToString());
            strItem = "FUEL CAP";
            if ((row["SAF_TB_ITEM_87"].ToString()[0] == '2') || (row["SAF_TB_ITEM_87"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_TB_ITEM_87"].ToString());
            strItem = "UNSECURED TANK";
            if ((row["SAF_TB_ITEM_88"].ToString()[0] == '2') || (row["SAF_TB_ITEM_88"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_TB_ITEM_88"].ToString());
            strItem = "IMPROPER LPG INSTALLATION";
            if ((row["SAF_TB_ITEM_89"].ToString()[0] == '2') || (row["SAF_TB_ITEM_89"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_TB_ITEM_89"].ToString());


            strSystem = "BUMPERS";
            strItem = "HEIGHT";
            if ((row["SAF_TB_SUBITEM_169"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_169"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "FRONT", row["SAF_TB_SUBITEM_169"].ToString());
            if ((row["SAF_TB_SUBITEM_170"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_170"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "REAR", row["SAF_TB_SUBITEM_170"].ToString());
            strItem = "MISSING / LOOSE / RUSTED";
            if ((row["SAF_TB_SUBITEM_171"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_171"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "FRONT", row["SAF_TB_SUBITEM_171"].ToString());
            if ((row["SAF_TB_SUBITEM_172"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_172"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "REAR", row["SAF_TB_SUBITEM_172"].ToString());
            strItem = "IMPROPER WOODEN";
            if ((row["SAF_TB_SUBITEM_173"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_173"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "FRONT", row["SAF_TB_SUBITEM_173"].ToString());
            if ((row["SAF_TB_SUBITEM_174"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_174"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "REAR", row["SAF_TB_SUBITEM_174"].ToString());
            if ((row["SAF_TB_SUBITEM_175"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_175"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "SIZE", row["SAF_TB_SUBITEM_175"].ToString());
            if ((row["SAF_TB_SUBITEM_176"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_176"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "MATERIAL", row["SAF_TB_SUBITEM_176"].ToString());
            if ((row["SAF_TB_SUBITEM_177"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_177"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "HEIGHT", row["SAF_TB_SUBITEM_177"].ToString());
            if ((row["SAF_TB_SUBITEM_178"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_178"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "OEM ENERGY ABSORBING", row["SAF_TB_SUBITEM_178"].ToString());

            strSystem = "BODY / CHASSIS";
            strItem = "DAMAGE";
            if ((row["SAF_TB_SUBITEM_179"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_179"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "SHARP / PROTRUDING AREA", row["SAF_TB_SUBITEM_179"].ToString());
            if ((row["SAF_TB_SUBITEM_180"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_180"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "MISSING / IMPROPER FENDER", row["SAF_TB_SUBITEM_180"].ToString());
            if ((row["SAF_TB_SUBITEM_181"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_181"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "MISSING / DEFECTIVE DOOR", row["SAF_TB_SUBITEM_181"].ToString());
            if ((row["SAF_TB_SUBITEM_182"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_182"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "MISSING / DAMAGED DOOR LATCH / LOCK / HINGE / HANDLE", row["SAF_TB_SUBITEM_182"].ToString());
            if ((row["SAF_TB_SUBITEM_183"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_183"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "MISSING / DEFECTIVE HOOD", row["SAF_TB_SUBITEM_183"].ToString());
            if ((row["SAF_TB_SUBITEM_184"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_184"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "DEFECTIVE HOOD CATCH / RELEASE LATCH", row["SAF_TB_SUBITEM_184"].ToString());
            if ((row["SAF_TB_SUBITEM_185"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_185"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "DEFECTIVE FLOOR PAN", row["SAF_TB_SUBITEM_185"].ToString());
            if ((row["SAF_TB_SUBITEM_186"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_186"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "MISSING / IMPROPER PART", row["SAF_TB_SUBITEM_186"].ToString());
            strItem = "OPENINGS ALLOW EXHAUST INTO PASS. AREA";
            if ((row["SAF_TB_SUBITEM_187"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_187"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "DOOR", row["SAF_TB_SUBITEM_187"].ToString());
            if ((row["SAF_TB_SUBITEM_188"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_188"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "WINDOW", row["SAF_TB_SUBITEM_188"].ToString());
            if ((row["SAF_TB_SUBITEM_189"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_189"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "DECK LID", row["SAF_TB_SUBITEM_189"].ToString());
            if ((row["SAF_TB_SUBITEM_190"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_190"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "OTHER", row["SAF_TB_SUBITEM_190"].ToString());
            strItem = "MISSING / IMPROPER BUMPERS";
            if ((row["SAF_TB_SUBITEM_191"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_191"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "FRONT", row["SAF_TB_SUBITEM_191"].ToString());
            if ((row["SAF_TB_SUBITEM_192"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_192"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "REAR", row["SAF_TB_SUBITEM_192"].ToString());
            strItem = "RUST";
            if ((row["SAF_TB_SUBITEM_193"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_193"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "STRUCTURAL RUST", row["SAF_TB_SUBITEM_193"].ToString());
            if ((row["SAF_TB_SUBITEM_194"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_194"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "IMPROPER REPAIR", row["SAF_TB_SUBITEM_194"].ToString());
            strItem = "HOOD SCOOPS";
            if ((row["SAF_TB_SUBITEM_195"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_195"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "HEIGHT / SIZE", row["SAF_TB_SUBITEM_195"].ToString());
            if ((row["SAF_TB_SUBITEM_196"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_196"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "OBSCURED VIEW", row["SAF_TB_SUBITEM_196"].ToString());

            strSystem = "BUS BODY";
            strItem = "RIVET / BOLTS";
            if ((row["SAF_TB_ITEM_98"].ToString()[0] == '2') || (row["SAF_TB_ITEM_98"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_TB_ITEM_98"].ToString());
            strItem = "STEPWELL";
            if ((row["SAF_TB_ITEM_99"].ToString()[0] == '2') || (row["SAF_TB_ITEM_99"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_TB_ITEM_99"].ToString());
            strItem = "ENTRANCE DOOR MALFUNCTIONS";
            if ((row["SAF_TB_ITEM_100"].ToString()[0] == '2') || (row["SAF_TB_ITEM_100"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_TB_ITEM_100"].ToString());
            strItem = "EMERGENCY DOORS / WINDOWS";
            if ((row["SAF_TB_SUBITEM_197"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_197"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "NOT ACCESSIBLE", row["SAF_TB_SUBITEM_197"].ToString());
            if ((row["SAF_TB_SUBITEM_198"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_198"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "MALFUNCTION", row["SAF_TB_SUBITEM_198"].ToString());
            if ((row["SAF_TB_SUBITEM_199"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_199"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "INADEQUATE DECALS", row["SAF_TB_SUBITEM_199"].ToString());
            strItem = "COMPARTMENT DOOR";
            if ((row["SAF_TB_SUBITEM_200"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_200"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "INSECURELY ATTACHED", row["SAF_TB_SUBITEM_200"].ToString());
            if ((row["SAF_TB_SUBITEM_201"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_201"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "IMPROPER FUNCTIONING", row["SAF_TB_SUBITEM_201"].ToString());

            strSystem = "BUS INTERIOR";
            strItem = "FLOOR";
            if ((row["SAF_TB_SUBITEM_202"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_202"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LOOSE COVERING", row["SAF_TB_SUBITEM_202"].ToString());
            if ((row["SAF_TB_SUBITEM_203"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_203"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "FLOOR BOARD HOLES", row["SAF_TB_SUBITEM_203"].ToString());
            strItem = "SEATS";
            if ((row["SAF_TB_SUBITEM_204"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_204"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "MISSING / LOOSE / ROTTING SEAT FASTENER", row["SAF_TB_SUBITEM_204"].ToString());
            if ((row["SAF_TB_SUBITEM_205"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_205"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "MISSING / INSECURE ANCHOR BOLT", row["SAF_TB_SUBITEM_205"].ToString());
            if ((row["SAF_TB_SUBITEM_206"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_206"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "INADEQUATE SEAT COVER", row["SAF_TB_SUBITEM_206"].ToString());
            if ((row["SAF_TB_SUBITEM_207"].ToString()[0] == '2') || (row["SAF_TB_SUBITEM_207"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "HAZARDOUS FRAME / TRIM", row["SAF_TB_SUBITEM_207"].ToString());
            strItem = "STANCHION / GUARDS";
            if ((row["SAF_TB_ITEM_105"].ToString()[0] == '2') || (row["SAF_TB_ITEM_105"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_TB_ITEM_105"].ToString());
            strItem = "PACKAGE SHELF";
            if ((row["SAF_TB_ITEM_106"].ToString()[0] == '2') || (row["SAF_TB_ITEM_106"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_TB_ITEM_106"].ToString());
            strItem = "INOPERABLE HEATER";
            if ((row["SAF_TB_ITEM_107"].ToString()[0] == '2') || (row["SAF_TB_ITEM_107"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_TB_ITEM_107"].ToString());
            strItem = "INADEQUATE VENTILATION";
            if ((row["SAF_TB_ITEM_108"].ToString()[0] == '2') || (row["SAF_TB_ITEM_108"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_TB_ITEM_108"].ToString());


            #endregion

            // Safety TR
            #region Safety TR

            strSystem = "VEHICLE INFO";
            strItem = "VIN";
            if ((row["SAF_TR_SUBITEM_1"].ToString()[0] == '2') || (row["SAF_TR_SUBITEM_1"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "DIFFERS FROM REGISTRATION", row["SAF_TR_SUBITEM_1"].ToString());
            if ((row["SAF_TR_SUBITEM_2"].ToString()[0] == '2') || (row["SAF_TR_SUBITEM_2"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "VIN PLATE MISSING / TAMPERED", row["SAF_TR_SUBITEM_2"].ToString());
            strItem = "REGISTRATION PLATES";
            if ((row["SAF_TR_SUBITEM_3"].ToString()[0] == '2') || (row["SAF_TR_SUBITEM_3"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "UNSECURED / IMPROPER PLACEMENT", row["SAF_TR_SUBITEM_3"].ToString());
            if ((row["SAF_TR_SUBITEM_4"].ToString()[0] == '2') || (row["SAF_TR_SUBITEM_4"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "MISSING PLATE", row["SAF_TR_SUBITEM_4"].ToString());
            if ((row["SAF_TR_SUBITEM_5"].ToString()[0] == '2') || (row["SAF_TR_SUBITEM_5"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "OBSCURED PLATE", row["SAF_TR_SUBITEM_5"].ToString());
            if ((row["SAF_TR_SUBITEM_6"].ToString()[0] == '2') || (row["SAF_TR_SUBITEM_6"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "WRONG COLOR", row["SAF_TR_SUBITEM_6"].ToString());

            strSystem = "TIRES";
            strItem = "BRAKES / CUTS / REPAIRS";
            if ((row["SAF_TR_SUBITEM_7"].ToString()[0] == '2') || (row["SAF_TR_SUBITEM_7"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LEFT", row["SAF_TR_SUBITEM_7"].ToString());
            if ((row["SAF_TR_SUBITEM_8"].ToString()[0] == '2') || (row["SAF_TR_SUBITEM_8"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "RIGHT", row["SAF_TR_SUBITEM_8"].ToString());
            strItem = "TREAD DEPTH";
            if ((row["SAF_TR_SUBITEM_9"].ToString()[0] == '2') || (row["SAF_TR_SUBITEM_9"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LEFT", row["SAF_TR_SUBITEM_9"].ToString());
            if ((row["SAF_TR_SUBITEM_10"].ToString()[0] == '2') || (row["SAF_TR_SUBITEM_10"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "RIGHT", row["SAF_TR_SUBITEM_10"].ToString());
            strItem = "REGROOVED";
            if ((row["SAF_TR_SUBITEM_11"].ToString()[0] == '2') || (row["SAF_TR_SUBITEM_11"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LEFT", row["SAF_TR_SUBITEM_11"].ToString());
            if ((row["SAF_TR_SUBITEM_12"].ToString()[0] == '2') || (row["SAF_TR_SUBITEM_12"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "RIGHT", row["SAF_TR_SUBITEM_12"].ToString());
            strItem = "MIS-MATCHED";
            if ((row["SAF_TR_SUBITEM_13"].ToString()[0] == '2') || (row["SAF_TR_SUBITEM_13"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LEFT", row["SAF_TR_SUBITEM_13"].ToString());
            if ((row["SAF_TR_SUBITEM_14"].ToString()[0] == '2') || (row["SAF_TR_SUBITEM_14"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "RIGHT", row["SAF_TR_SUBITEM_14"].ToString());
            strItem = "TIRE WIDTH";
            if ((row["SAF_TR_SUBITEM_15"].ToString()[0] == '2') || (row["SAF_TR_SUBITEM_15"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LEFT", row["SAF_TR_SUBITEM_15"].ToString());
            if ((row["SAF_TR_SUBITEM_16"].ToString()[0] == '2') || (row["SAF_TR_SUBITEM_16"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "RIGHT", row["SAF_TR_SUBITEM_16"].ToString());
            strItem = "MISSING / IMPROPER TIRE FLAPS";
            if ((row["SAF_TR_SUBITEM_17"].ToString()[0] == '2') || (row["SAF_TR_SUBITEM_17"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LEFT", row["SAF_TR_SUBITEM_17"].ToString());
            if ((row["SAF_TR_SUBITEM_18"].ToString()[0] == '2') || (row["SAF_TR_SUBITEM_18"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "RIGHT", row["SAF_TR_SUBITEM_18"].ToString());
            strItem = "PROHIBITED";
            if ((row["SAF_TR_SUBITEM_19"].ToString()[0] == '2') || (row["SAF_TR_SUBITEM_19"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LEFT", row["SAF_TR_SUBITEM_19"].ToString());
            if ((row["SAF_TR_SUBITEM_20"].ToString()[0] == '2') || (row["SAF_TR_SUBITEM_20"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "RIGHT", row["SAF_TR_SUBITEM_20"].ToString());

            strSystem = "MAIN BRAKES";
            strItem = "INADEQUATE STOPPING POWER";
            if ((row["SAF_TR_ITEM_10"].ToString()[0] == '2') || (row["SAF_TR_ITEM_10"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_TR_ITEM_10"].ToString());
            strItem = "AUTOMATIC HOLD ON GRADE";
            if ((row["SAF_TR_ITEM_11"].ToString()[0] == '2') || (row["SAF_TR_ITEM_11"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_TR_ITEM_11"].ToString());
            strItem = "SIMULTANEOUS BRAKING";
            if ((row["SAF_TR_ITEM_12"].ToString()[0] == '2') || (row["SAF_TR_ITEM_12"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_TR_ITEM_12"].ToString());
            strItem = "INSUFFICIENT BRAKING RATE";
            if ((row["SAF_TR_ITEM_13"].ToString()[0] == '2') || (row["SAF_TR_ITEM_13"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_TR_ITEM_13"].ToString());
            strItem = "EXCESS SIDESWAY";
            if ((row["SAF_TR_ITEM_14"].ToString()[0] == '2') || (row["SAF_TR_ITEM_14"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_TR_ITEM_14"].ToString());
            strItem = "DEFECTIVE BRAKE LINING";
            if ((row["SAF_TR_ITEM_15"].ToString()[0] == '2') || (row["SAF_TR_ITEM_15"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_TR_ITEM_15"].ToString());

            strItem = "SLACK ADJ / AIR CHAMBER STROKE OUT OF TOLERANCE";
            if ((row["SAF_TR_ITEM_16"].ToString()[0] == '2') || (row["SAF_TR_ITEM_16"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_TR_ITEM_16"].ToString());
            strItem = "DEFECTIVE LINE / HOSE / ELECTRICAL";
            if ((row["SAF_TR_ITEM_17"].ToString()[0] == '2') || (row["SAF_TR_ITEM_17"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_TR_ITEM_17"].ToString());
            strItem = "ANTI-LOCK DEVICE LIGHT";
            if ((row["SAF_TR_ITEM_18"].ToString()[0] == '2') || (row["SAF_TR_ITEM_18"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_TR_ITEM_18"].ToString());

            strSystem = "PARKING BRAKE (AIR)";
            strItem = "PROPER APPLY / RELEASE";
            if ((row["SAF_TR_ITEM_19"].ToString()[0] == '2') || (row["SAF_TR_ITEM_19"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_TR_ITEM_19"].ToString());
            strItem = "NOT AUTO APPLY ON DISCONNECT";
            if ((row["SAF_TR_ITEM_20"].ToString()[0] == '2') || (row["SAF_TR_ITEM_20"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_TR_ITEM_20"].ToString());
            strItem = "HOSE CONNECTION LEAKAGE";
            if ((row["SAF_TR_ITEM_21"].ToString()[0] == '2') || (row["SAF_TR_ITEM_21"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_TR_ITEM_21"].ToString());

            strSystem = "EMERGENCY BRAKES";
            strItem = "NOT AUTO APPLY ON DISCONNECT";
            if ((row["SAF_TR_ITEM_22"].ToString()[0] == '2') || (row["SAF_TR_ITEM_22"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_TR_ITEM_22"].ToString());
            strItem = "RELEASE ON CONNECT";
            if ((row["SAF_TR_ITEM_23"].ToString()[0] == '2') || (row["SAF_TR_ITEM_23"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_TR_ITEM_23"].ToString());

            strSystem = "MAIN BRAKES";
            strItem = "DEFECTIVE BRAKE LINING";
            if ((row["SAF_TR_SUBITEM_21"].ToString()[0] == '2') || (row["SAF_TR_SUBITEM_21"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LEFT", row["SAF_TR_SUBITEM_21"].ToString());
            if ((row["SAF_TR_SUBITEM_22"].ToString()[0] == '2') || (row["SAF_TR_SUBITEM_22"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "RIGHT", row["SAF_TR_SUBITEM_22"].ToString());
            strItem = "ANTI-LOCK DEVICE LIGHT";
            if ((row["SAF_TR_SUBITEM_23"].ToString()[0] == '2') || (row["SAF_TR_SUBITEM_23"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "DOES NOT LIGHT KOEO", row["SAF_TR_SUBITEM_23"].ToString());
            if ((row["SAF_TR_SUBITEM_24"].ToString()[0] == '2') || (row["SAF_TR_SUBITEM_24"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "ILLUMINATES KOER", row["SAF_TR_SUBITEM_24"].ToString());

            strSystem = "REAR LIGHTS";
            strItem = "LEFT REAR";
            if ((row["SAF_TR_SUBITEM_25"].ToString()[0] == '2') || (row["SAF_TR_SUBITEM_25"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "INOPERATIVE", row["SAF_TR_SUBITEM_25"].ToString());
            if ((row["SAF_TR_SUBITEM_26"].ToString()[0] == '2') || (row["SAF_TR_SUBITEM_26"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "BROKEN LENS", row["SAF_TR_SUBITEM_26"].ToString());
            if ((row["SAF_TR_SUBITEM_27"].ToString()[0] == '2') || (row["SAF_TR_SUBITEM_27"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "BRIGHTNESS", row["SAF_TR_SUBITEM_27"].ToString());
            if ((row["SAF_TR_SUBITEM_28"].ToString()[0] == '2') || (row["SAF_TR_SUBITEM_28"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "WRONG COLOR", row["SAF_TR_SUBITEM_28"].ToString());
            strItem = "RIGHT REAR";
            if ((row["SAF_TR_SUBITEM_29"].ToString()[0] == '2') || (row["SAF_TR_SUBITEM_29"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "INOPERATIVE", row["SAF_TR_SUBITEM_29"].ToString());
            if ((row["SAF_TR_SUBITEM_30"].ToString()[0] == '2') || (row["SAF_TR_SUBITEM_30"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "BROKEN LENS", row["SAF_TR_SUBITEM_30"].ToString());
            if ((row["SAF_TR_SUBITEM_31"].ToString()[0] == '2') || (row["SAF_TR_SUBITEM_31"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "BRIGHTNESS", row["SAF_TR_SUBITEM_31"].ToString());
            if ((row["SAF_TR_SUBITEM_32"].ToString()[0] == '2') || (row["SAF_TR_SUBITEM_32"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "WRONG COLOR", row["SAF_TR_SUBITEM_32"].ToString());
            strItem = "LICENSE PLATE";
            if ((row["SAF_TR_SUBITEM_33"].ToString()[0] == '2') || (row["SAF_TR_SUBITEM_33"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "INOPERATIVE", row["SAF_TR_SUBITEM_33"].ToString());
            if ((row["SAF_TR_SUBITEM_34"].ToString()[0] == '2') || (row["SAF_TR_SUBITEM_34"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "BROKEN LENS", row["SAF_TR_SUBITEM_34"].ToString());
            if ((row["SAF_TR_SUBITEM_35"].ToString()[0] == '2') || (row["SAF_TR_SUBITEM_35"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "BRIGHTNESS", row["SAF_TR_SUBITEM_35"].ToString());
            if ((row["SAF_TR_SUBITEM_36"].ToString()[0] == '2') || (row["SAF_TR_SUBITEM_36"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "WRONG COLOR", row["SAF_TR_SUBITEM_36"].ToString());
            strItem = "CLEARANCE";
            if ((row["SAF_TR_SUBITEM_37"].ToString()[0] == '2') || (row["SAF_TR_SUBITEM_37"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "INOPERATIVE", row["SAF_TR_SUBITEM_37"].ToString());
            if ((row["SAF_TR_SUBITEM_38"].ToString()[0] == '2') || (row["SAF_TR_SUBITEM_38"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "BROKEN LENS", row["SAF_TR_SUBITEM_38"].ToString());
            if ((row["SAF_TR_SUBITEM_39"].ToString()[0] == '2') || (row["SAF_TR_SUBITEM_39"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "BRIGHTNESS", row["SAF_TR_SUBITEM_39"].ToString());
            if ((row["SAF_TR_SUBITEM_40"].ToString()[0] == '2') || (row["SAF_TR_SUBITEM_40"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "WRONG COLOR", row["SAF_TR_SUBITEM_40"].ToString());

            strSystem = "STOP LIGHTS";
            strItem = "LEFT REAR";
            if ((row["SAF_TR_SUBITEM_41"].ToString()[0] == '2') || (row["SAF_TR_SUBITEM_41"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "INOPERATIVE", row["SAF_TR_SUBITEM_41"].ToString());
            if ((row["SAF_TR_SUBITEM_42"].ToString()[0] == '2') || (row["SAF_TR_SUBITEM_42"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "BROKEN LENS", row["SAF_TR_SUBITEM_42"].ToString());
            if ((row["SAF_TR_SUBITEM_43"].ToString()[0] == '2') || (row["SAF_TR_SUBITEM_43"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "BRIGHTNESS", row["SAF_TR_SUBITEM_43"].ToString());
            if ((row["SAF_TR_SUBITEM_44"].ToString()[0] == '2') || (row["SAF_TR_SUBITEM_44"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "WRONG COLOR", row["SAF_TR_SUBITEM_44"].ToString());
            strItem = "RIGHT REAR";
            if ((row["SAF_TR_SUBITEM_45"].ToString()[0] == '2') || (row["SAF_TR_SUBITEM_45"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "INOPERATIVE", row["SAF_TR_SUBITEM_45"].ToString());
            if ((row["SAF_TR_SUBITEM_46"].ToString()[0] == '2') || (row["SAF_TR_SUBITEM_46"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "BROKEN LENS", row["SAF_TR_SUBITEM_46"].ToString());
            if ((row["SAF_TR_SUBITEM_47"].ToString()[0] == '2') || (row["SAF_TR_SUBITEM_47"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "BRIGHTNESS", row["SAF_TR_SUBITEM_47"].ToString());
            if ((row["SAF_TR_SUBITEM_48"].ToString()[0] == '2') || (row["SAF_TR_SUBITEM_48"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "WRONG COLOR", row["SAF_TR_SUBITEM_48"].ToString());

            strSystem = "BUMPER";
            strItem = "HEIGHT";
            if ((row["SAF_TR_ITEM_30"].ToString()[0] == '2') || (row["SAF_TR_ITEM_30"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_TR_ITEM_30"].ToString());
            strItem = "WIDTH";
            if ((row["SAF_TR_ITEM_31"].ToString()[0] == '2') || (row["SAF_TR_ITEM_31"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_TR_ITEM_31"].ToString());

            strSystem = "BODY / CHASSIS";
            strItem = "LANDING GEAR";
            if ((row["SAF_TR_ITEM_32"].ToString()[0] == '2') || (row["SAF_TR_ITEM_32"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_TR_ITEM_32"].ToString());
            strItem = "BODY / FRAME";
            if ((row["SAF_TR_ITEM_33"].ToString()[0] == '2') || (row["SAF_TR_ITEM_33"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "", row["SAF_TR_ITEM_32"].ToString());
            strItem = "DIMENSIONS";
            if ((row["SAF_TR_SUBITEM_49"].ToString()[0] == '2') || (row["SAF_TR_SUBITEM_49"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "HEIGHT", row["SAF_TR_SUBITEM_49"].ToString());
            if ((row["SAF_TR_SUBITEM_50"].ToString()[0] == '2') || (row["SAF_TR_SUBITEM_50"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "WIDTH", row["SAF_TR_SUBITEM_50"].ToString());
            if ((row["SAF_TR_SUBITEM_51"].ToString()[0] == '2') || (row["SAF_TR_SUBITEM_51"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LENGTH", row["SAF_TR_SUBITEM_51"].ToString());
            strItem = "TRAILER TOW";
            if ((row["SAF_TR_SUBITEM_52"].ToString()[0] == '2') || (row["SAF_TR_SUBITEM_52"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "COUPLING", row["SAF_TR_SUBITEM_52"].ToString());
            if ((row["SAF_TR_SUBITEM_53"].ToString()[0] == '2') || (row["SAF_TR_SUBITEM_53"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "SAFETY CHAINS", row["SAF_TR_SUBITEM_53"].ToString());

            #endregion

            // Safety AG
            #region Safety AG
            strItem = "";
            strSubitem = "";
            strSystem = "VIN";
            if ((row["SAF_AG_VIN"].ToString()[0] == '2') || (row["SAF_AG_VIN"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, strSubitem, row["SAF_BS_SUBITEM_1"].ToString());
            strSystem = "BRAKES";
            if ((row["SAF_AG_BRAKES"].ToString()[0] == '2') || (row["SAF_AG_BRAKES"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, strSubitem, row["SAF_BS_SUBITEM_2"].ToString());
            strSystem = "STEERING WHEEL";
            if ((row["SAF_AG_STEERING"].ToString()[0] == '2') || (row["SAF_AG_STEERING"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, strSubitem, row["SAF_BS_SUBITEM_3"].ToString());
            strSystem = "STOP LIGHTS";
            if ((row["SAF_AG_STOPLIGHTS"].ToString()[0] == '2') || (row["SAF_AG_STOPLIGHTS"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, strSubitem, row["SAF_BS_SUBITEM_4"].ToString());
            strSystem = "EXHAUST SYSTEM";
            if ((row["SAF_AG_EXHAUST"].ToString()[0] == '2') || (row["SAF_AG_EXHAUST"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, strSubitem, row["SAF_BS_SUBITEM_5"].ToString());
            strSystem = "HEADLIGHTS";
            if ((row["SAF_AG_HEADLIGHTS"].ToString()[0] == '2') || (row["SAF_AG_HEADLIGHTS"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, strSubitem, row["SAF_BS_SUBITEM_6"].ToString());
            strSystem = "REFLECTORS";
            if ((row["SAF_AG_REFLECTORS"].ToString()[0] == '2') || (row["SAF_AG_REFLECTORS"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, strSubitem, row["SAF_BS_SUBITEM_7"].ToString());
            strSystem = "TAIL LIGHTS";
            if ((row["SAF_AG_TAILLIGHTS"].ToString()[0] == '2') || (row["SAF_AG_TAILLIGHTS"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, strSubitem, row["SAF_BS_SUBITEM_8"].ToString());

            #endregion

            // Safety MC
            #region Safety MC

            strSystem = "VEHICLE INFO";
            strItem = "VIN";
            if ((row["SAF_MC_SUBITEM_1"].ToString()[0] == '2') || (row["SAF_MC_SUBITEM_1"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "DIFFERS FROM REGISTRATION", row["SAF_MC_SUBITEM_1"].ToString());
            if ((row["SAF_MC_SUBITEM_2"].ToString()[0] == '2') || (row["SAF_MC_SUBITEM_2"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "VIN PLATE MISSING / TAMPERED", row["SAF_MC_SUBITEM_2"].ToString());
            strSystem = "WHEELS";
            if ((row["SAF_MC_ITEM_3"].ToString()[0] == '2') || (row["SAF_MC_ITEM_3"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, "FRONT", "", row["SAF_MC_ITEM_3"].ToString());
            if ((row["SAF_MC_ITEM_4"].ToString()[0] == '2') || (row["SAF_MC_ITEM_4"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, "REAR", "", row["SAF_MC_ITEM_4"].ToString());
            strItem = "REGISTRATION PLATES";
            if ((row["SAF_MC_SUBITEM_3"].ToString()[0] == '2') || (row["SAF_MC_SUBITEM_3"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "UNSECURED / IMPROPER PLACEMENT", row["SAF_MC_SUBITEM_3"].ToString());
            if ((row["SAF_MC_SUBITEM_4"].ToString()[0] == '2') || (row["SAF_MC_SUBITEM_4"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "MISSING PLATE", row["SAF_MC_SUBITEM_4"].ToString());
            if ((row["SAF_MC_SUBITEM_5"].ToString()[0] == '2') || (row["SAF_MC_SUBITEM_5"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "OBSCURED PLATE", row["SAF_MC_SUBITEM_5"].ToString());
            if ((row["SAF_MC_SUBITEM_6"].ToString()[0] == '2') || (row["SAF_MC_SUBITEM_6"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "WRONG COLOR", row["SAF_MC_SUBITEM_6"].ToString());

            strSystem = "TIRES";
            strItem = "PRESSURE";
            if ((row["SAF_MC_SUBITEM_7"].ToString()[0] == '2') || (row["SAF_MC_SUBITEM_7"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "FRONT", row["SAF_MC_SUBITEM_7"].ToString());
            if ((row["SAF_MC_SUBITEM_8"].ToString()[0] == '2') || (row["SAF_MC_SUBITEM_8"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "REAR", row["SAF_MC_SUBITEM_8"].ToString());
            strItem = "BREAKS / CUTS / REPAIRS";
            if ((row["SAF_MC_SUBITEM_9"].ToString()[0] == '2') || (row["SAF_MC_SUBITEM_9"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "FRONT", row["SAF_MC_SUBITEM_9"].ToString());
            if ((row["SAF_MC_SUBITEM_10"].ToString()[0] == '2') || (row["SAF_MC_SUBITEM_10"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "REAR", row["SAF_MC_SUBITEM_10"].ToString());
            strItem = "TREAD DEPTH";
            if ((row["SAF_MC_SUBITEM_11"].ToString()[0] == '2') || (row["SAF_MC_SUBITEM_11"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "FRONT", row["SAF_MC_SUBITEM_11"].ToString());
            if ((row["SAF_MC_SUBITEM_12"].ToString()[0] == '2') || (row["SAF_MC_SUBITEM_12"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "REAR", row["SAF_MC_SUBITEM_12"].ToString());
            strItem = "REGROOVED";
            if ((row["SAF_MC_SUBITEM_13"].ToString()[0] == '2') || (row["SAF_MC_SUBITEM_13"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "FRONT", row["SAF_MC_SUBITEM_13"].ToString());
            if ((row["SAF_MC_SUBITEM_14"].ToString()[0] == '2') || (row["SAF_MC_SUBITEM_14"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "REAR", row["SAF_MC_SUBITEM_14"].ToString());
            strItem = "TIRE WIDTH";
            if ((row["SAF_MC_SUBITEM_15"].ToString()[0] == '2') || (row["SAF_MC_SUBITEM_15"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "FRONT", row["SAF_MC_SUBITEM_15"].ToString());
            if ((row["SAF_MC_SUBITEM_16"].ToString()[0] == '2') || (row["SAF_MC_SUBITEM_16"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "REAR", row["SAF_MC_SUBITEM_16"].ToString());

            strSystem = "STEERING / SUSPENSION";
            if ((row["SAF_MC_ITEM_10"].ToString()[0] == '2') || (row["SAF_MC_ITEM_10"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, "SUSPENSION", "", row["SAF_MC_ITEM_10"].ToString());
            if ((row["SAF_MC_ITEM_11"].ToString()[0] == '2') || (row["SAF_MC_ITEM_11"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, "ALIGNMENT / TRACKING", "", row["SAF_MC_ITEM_11"].ToString());
            if ((row["SAF_MC_ITEM_12"].ToString()[0] == '2') || (row["SAF_MC_ITEM_12"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, "STEERING HEAD", "", row["SAF_MC_ITEM_12"].ToString());
            if ((row["SAF_MC_ITEM_13"].ToString()[0] == '2') || (row["SAF_MC_ITEM_13"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, "FRONT GEOMETRY", "", row["SAF_MC_ITEM_13"].ToString());
            if ((row["SAF_MC_ITEM_14"].ToString()[0] == '2') || (row["SAF_MC_ITEM_14"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, "HANDLE BARS", "", row["SAF_MC_ITEM_14"].ToString());
            if ((row["SAF_MC_ITEM_15"].ToString()[0] == '2') || (row["SAF_MC_ITEM_15"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, "HANDGRIPS", "", row["SAF_MC_ITEM_15"].ToString());
            strItem = "SHOCK ABSORBERS";
            if ((row["SAF_MC_SUBITEM_17"].ToString()[0] == '2') || (row["SAF_MC_SUBITEM_17"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "FRONT", row["SAF_MC_SUBITEM_17"].ToString());
            if ((row["SAF_MC_SUBITEM_18"].ToString()[0] == '2') || (row["SAF_MC_SUBITEM_18"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "REAR", row["SAF_MC_SUBITEM_18"].ToString());

            strSystem = "FOOT BRAKE";
            if ((row["SAF_MC_ITEM_17"].ToString()[0] == '2') || (row["SAF_MC_ITEM_17"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, "INADEQUATE STOPPING POWER", "", row["SAF_MC_ITEM_17"].ToString());
            if ((row["SAF_MC_ITEM_19"].ToString()[0] == '2') || (row["SAF_MC_ITEM_19"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, "EXCESS BRAKE PEDAL TRAVEL", "", row["SAF_MC_ITEM_19"].ToString());
            strItem = "NO BRAKING ON A WHEEL";
            if ((row["SAF_MC_SUBITEM_19"].ToString()[0] == '2') || (row["SAF_MC_SUBITEM_19"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "FRONT", row["SAF_MC_SUBITEM_19"].ToString());
            if ((row["SAF_MC_SUBITEM_20"].ToString()[0] == '2') || (row["SAF_MC_SUBITEM_20"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "REAR", row["SAF_MC_SUBITEM_20"].ToString());
            strItem = "DEFECTIVE BRAKE LINING";
            if ((row["SAF_MC_SUBITEM_21"].ToString()[0] == '2') || (row["SAF_MC_SUBITEM_21"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "FRONT", row["SAF_MC_SUBITEM_21"].ToString());
            if ((row["SAF_MC_SUBITEM_22"].ToString()[0] == '2') || (row["SAF_MC_SUBITEM_22"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "REAR", row["SAF_MC_SUBITEM_22"].ToString());
            strItem = "DEFECTIVE DISK OR DRUM";
            if ((row["SAF_MC_SUBITEM_23"].ToString()[0] == '2') || (row["SAF_MC_SUBITEM_23"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "FRONT", row["SAF_MC_SUBITEM_23"].ToString());
            if ((row["SAF_MC_SUBITEM_24"].ToString()[0] == '2') || (row["SAF_MC_SUBITEM_24"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "REAR", row["SAF_MC_SUBITEM_24"].ToString());
            strItem = "HYDRAULICS";
            if ((row["SAF_MC_SUBITEM_25"].ToString()[0] == '2') || (row["SAF_MC_SUBITEM_25"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LOW FLUID", row["SAF_MC_SUBITEM_25"].ToString());
            if ((row["SAF_MC_SUBITEM_26"].ToString()[0] == '2') || (row["SAF_MC_SUBITEM_26"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "MASTER CYLINDER LEAK", row["SAF_MC_SUBITEM_26"].ToString());
            if ((row["SAF_MC_SUBITEM_27"].ToString()[0] == '2') || (row["SAF_MC_SUBITEM_27"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "WHEEL CYLINDER LEAK", row["SAF_MC_SUBITEM_27"].ToString());
            if ((row["SAF_MC_SUBITEM_28"].ToString()[0] == '2') || (row["SAF_MC_SUBITEM_28"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "DEFECTIVE HOSE / TUBING", row["SAF_MC_SUBITEM_28"].ToString());
            strItem = "LINKAGE";
            if ((row["SAF_MC_SUBITEM_29"].ToString()[0] == '2') || (row["SAF_MC_SUBITEM_29"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "EXCESS FRICTION", row["SAF_MC_SUBITEM_29"].ToString());
            if ((row["SAF_MC_SUBITEM_30"].ToString()[0] == '2') || (row["SAF_MC_SUBITEM_30"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "MISSING / BROKEN / EXCESS WEAR", row["SAF_MC_SUBITEM_30"].ToString());
            if ((row["SAF_MC_SUBITEM_31"].ToString()[0] == '2') || (row["SAF_MC_SUBITEM_31"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "IMPROPER POSITION / ALIGNMENT", row["SAF_MC_SUBITEM_31"].ToString());

            strSystem = "INSTRUMENTS";
            if ((row["SAF_MC_ITEM_24"].ToString()[0] == '2') || (row["SAF_MC_ITEM_24"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, "SPEEDOMETER", "", row["SAF_MC_ITEM_24"].ToString());
            if ((row["SAF_MC_ITEM_25"].ToString()[0] == '2') || (row["SAF_MC_ITEM_25"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, "ODOMETER", "", row["SAF_MC_ITEM_25"].ToString());
            strSystem = "HORN / ELECTRICAL SYSTEM";
            if ((row["SAF_MC_ITEM_27"].ToString()[0] == '2') || (row["SAF_MC_ITEM_27"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, "ELECTRICAL WIRING AND SYSTEM", "", row["SAF_MC_ITEM_27"].ToString());
            strItem = "HORN";
            if ((row["SAF_MC_SUBITEM_32"].ToString()[0] == '2') || (row["SAF_MC_SUBITEM_32"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "NOT OPERATING", row["SAF_MC_SUBITEM_32"].ToString());
            if ((row["SAF_MC_SUBITEM_33"].ToString()[0] == '2') || (row["SAF_MC_SUBITEM_33"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "NOT AUDIBLE 200 FEET AWAY", row["SAF_MC_SUBITEM_33"].ToString());
            if ((row["SAF_MC_SUBITEM_34"].ToString()[0] == '2') || (row["SAF_MC_SUBITEM_34"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "NOT FASTENED SECURELY", row["SAF_MC_SUBITEM_34"].ToString());

            strSystem = "REAR LIGHTS";
            strItem = "INOPERATIVE";
            if ((row["SAF_MC_SUBITEM_35"].ToString()[0] == '2') || (row["SAF_MC_SUBITEM_35"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "BROKEN LENS", row["SAF_MC_SUBITEM_35"].ToString());
            if ((row["SAF_MC_SUBITEM_36"].ToString()[0] == '2') || (row["SAF_MC_SUBITEM_36"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "BRIGHTNESS", row["SAF_MC_SUBITEM_36"].ToString());
            if ((row["SAF_MC_SUBITEM_37"].ToString()[0] == '2') || (row["SAF_MC_SUBITEM_37"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "WRONG COLOR", row["SAF_MC_SUBITEM_37"].ToString());

            strSystem = "STOP LIGHTS";
            strItem = "INOPERATIVE";
            if ((row["SAF_MC_SUBITEM_38"].ToString()[0] == '2') || (row["SAF_MC_SUBITEM_38"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "BROKEN LENS", row["SAF_MC_SUBITEM_38"].ToString());
            if ((row["SAF_MC_SUBITEM_39"].ToString()[0] == '2') || (row["SAF_MC_SUBITEM_39"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "BRIGHTNESS", row["SAF_MC_SUBITEM_39"].ToString());
            if ((row["SAF_MC_SUBITEM_40"].ToString()[0] == '2') || (row["SAF_MC_SUBITEM_40"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "WRONG COLOR", row["SAF_MC_SUBITEM_40"].ToString());
            strSystem = "FRONT LIGHTS";
            strItem = "INOPERATIVE";
            if ((row["SAF_MC_SUBITEM_41"].ToString()[0] == '2') || (row["SAF_MC_SUBITEM_41"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "BROKEN LENS", row["SAF_MC_SUBITEM_41"].ToString());
            if ((row["SAF_MC_SUBITEM_42"].ToString()[0] == '2') || (row["SAF_MC_SUBITEM_42"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "BRIGHTNESS", row["SAF_MC_SUBITEM_42"].ToString());
            if ((row["SAF_MC_SUBITEM_43"].ToString()[0] == '2') || (row["SAF_MC_SUBITEM_43"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "WRONG COLOR", row["SAF_MC_SUBITEM_43"].ToString());

            strSystem = "DIRECTIONAL SIGNAL";
            if ((row["SAF_MC_ITEM_31"].ToString()[0] == '2') || (row["SAF_MC_ITEM_31"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, "IMPROPER FUNCTION", "", row["SAF_MC_ITEM_31"].ToString());
            if ((row["SAF_MC_ITEM_32"].ToString()[0] == '2') || (row["SAF_MC_ITEM_32"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, "IMPROPER POSITION", "", row["SAF_MC_ITEM_32"].ToString());
            if ((row["SAF_MC_ITEM_33"].ToString()[0] == '2') || (row["SAF_MC_ITEM_33"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, "OBSCURED", "", row["SAF_MC_ITEM_33"].ToString());
            strItem = "BROKEN LAMP / LENS";
            if ((row["SAF_MC_SUBITEM_44"].ToString()[0] == '2') || (row["SAF_MC_SUBITEM_44"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LEFT FRONT", row["SAF_MC_SUBITEM_44"].ToString());
            if ((row["SAF_MC_SUBITEM_45"].ToString()[0] == '2') || (row["SAF_MC_SUBITEM_45"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "RIGHT FRONT", row["SAF_MC_SUBITEM_45"].ToString());
            if ((row["SAF_MC_SUBITEM_46"].ToString()[0] == '2') || (row["SAF_MC_SUBITEM_46"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "LEFT REAR", row["SAF_MC_SUBITEM_46"].ToString());
            if ((row["SAF_MC_SUBITEM_47"].ToString()[0] == '2') || (row["SAF_MC_SUBITEM_47"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "RIGHT REAR", row["SAF_MC_SUBITEM_47"].ToString());

            strSystem = "OTHER LIGHTS";
            strItem = "EMERGENCY";
            if ((row["SAF_MC_SUBITEM_48"].ToString()[0] == '2') || (row["SAF_MC_SUBITEM_48"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "UNAUTHORIZED", row["SAF_MC_SUBITEM_48"].ToString());
            if ((row["SAF_MC_SUBITEM_49"].ToString()[0] == '2') || (row["SAF_MC_SUBITEM_49"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "WRONG COLOR", row["SAF_MC_SUBITEM_49"].ToString());
            if ((row["SAF_MC_SUBITEM_50"].ToString()[0] == '2') || (row["SAF_MC_SUBITEM_50"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "IMPROPER LOCATION", row["SAF_MC_SUBITEM_50"].ToString());

            strSystem = "HEADLIGHT AIM";
            if ((row["SAF_MC_HEADLIGHTAIM"].ToString()[0] == '2') || (row["SAF_MC_HEADLIGHTAIM"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, "", "", row["SAF_MC_HEADLIGHTAIM"].ToString());

            strSystem = "MIRRORS";
            if ((row["SAF_MC_MIRRORS"].ToString()[0] == '2') || (row["SAF_MC_MIRRORS"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, "", "", row["SAF_MC_MIRRORS"].ToString());

            strSystem = "GLASS";
            if ((row["SAF_MC_GLASS"].ToString()[0] == '2') || (row["SAF_MC_GLASS"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, "", "", row["SAF_MC_GLASS"].ToString());

            strSystem = "EXHAUST";
            if ((row["SAF_MC_ITEM_36"].ToString()[0] == '2') || (row["SAF_MC_ITEM_36"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, "MISSING MUFFLER", "", row["SAF_MC_ITEM_36"].ToString());
            if ((row["SAF_MC_ITEM_37"].ToString()[0] == '2') || (row["SAF_MC_ITEM_37"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, "HOLES/RUST / WORN SURFACE", "", row["SAF_MC_ITEM_37"].ToString());
            if ((row["SAF_MC_ITEM_38"].ToString()[0] == '2') || (row["SAF_MC_ITEM_38"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, "DEFECTIVE SUPPORT", "", row["SAF_MC_ITEM_38"].ToString());
            if ((row["SAF_MC_ITEM_39"].ToString()[0] == '2') || (row["SAF_MC_ITEM_39"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, "LOOSE / LEAKING JOINT", "", row["SAF_MC_ITEM_39"].ToString());
            if ((row["SAF_MC_ITEM_40"].ToString()[0] == '2') || (row["SAF_MC_ITEM_40"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, "NOT SECURED", "", row["SAF_MC_ITEM_40"].ToString());
            if ((row["SAF_MC_ITEM_41"].ToString()[0] == '2') || (row["SAF_MC_ITEM_41"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, "INCHED / DAMAGED TAILPIPE END", "", row["SAF_MC_ITEM_41"].ToString());
            if ((row["SAF_MC_ITEM_42"].ToString()[0] == '2') || (row["SAF_MC_ITEM_42"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, "MISSING HEAT SHIELDING", "", row["SAF_MC_ITEM_42"].ToString());
            if ((row["SAF_MC_ITEM_43"].ToString()[0] == '2') || (row["SAF_MC_ITEM_43"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, "EXCESS NOISE", "", row["SAF_MC_ITEM_43"].ToString());
            if ((row["SAF_MC_ITEM_44"].ToString()[0] == '2') || (row["SAF_MC_ITEM_44"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, "EXCESS FUMES", "", row["SAF_MC_ITEM_44"].ToString());
            if ((row["SAF_MC_ITEM_45"].ToString()[0] == '2') || (row["SAF_MC_ITEM_45"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, "SPARK ARRESTER", "", row["SAF_MC_ITEM_45"].ToString());

            strSystem = "FUEL SYSTEM";
            if ((row["SAF_MC_ITEM_46"].ToString()[0] == '2') || (row["SAF_MC_ITEM_46"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, "VAPOR / FUEL LEAK", "", row["SAF_MC_ITEM_46"].ToString());
            if ((row["SAF_MC_ITEM_47"].ToString()[0] == '2') || (row["SAF_MC_ITEM_47"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, "COMPONENT INSECURELY FASTENED", "", row["SAF_MC_ITEM_47"].ToString());
            if ((row["SAF_MC_ITEM_48"].ToString()[0] == '2') || (row["SAF_MC_ITEM_48"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, "FUEL CAP", "", row["SAF_MC_ITEM_48"].ToString());
            if ((row["SAF_MC_ITEM_49"].ToString()[0] == '2') || (row["SAF_MC_ITEM_49"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, "UNVENTED SYSTEM", "", row["SAF_MC_ITEM_49"].ToString());
            if ((row["SAF_MC_ITEM_50"].ToString()[0] == '2') || (row["SAF_MC_ITEM_50"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, "MIS-ROUTED FUEL LINE", "", row["SAF_MC_ITEM_50"].ToString());
            if ((row["SAF_MC_ITEM_51"].ToString()[0] == '2') || (row["SAF_MC_ITEM_51"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, "MISSING CUTOFF VALVE", "", row["SAF_MC_ITEM_51"].ToString());


            strSystem = "MIRRORS";
            if ((row["SAF_MC_MIRRORS"].ToString()[0] == '2') || (row["SAF_MC_MIRRORS"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, "", "", row["SAF_MC_MIRRORS"].ToString());

            strSystem = "BODY / CHASSIS / OTHER";
            if ((row["SAF_MC_ITEM_53"].ToString()[0] == '2') || (row["SAF_MC_ITEM_53"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, "FRAME", "", row["SAF_MC_ITEM_53"].ToString());
            if ((row["SAF_MC_ITEM_54"].ToString()[0] == '2') || (row["SAF_MC_ITEM_54"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, "INSUFFICIENT HANDHOLD", "", row["SAF_MC_ITEM_54"].ToString());
            if ((row["SAF_MC_ITEM_54"].ToString()[0] == '2') || (row["SAF_MC_ITEM_55"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, "INSUFFICIENT / INACCESSIBLE FOOTREST", "", row["SAF_MC_ITEM_55"].ToString());
            if ((row["SAF_MC_ITEM_56"].ToString()[0] == '2') || (row["SAF_MC_ITEM_56"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, "IMPROPER HIGHWAY BARS", "", row["SAF_MC_ITEM_56"].ToString());
            if ((row["SAF_MC_ITEM_57"].ToString()[0] == '2') || (row["SAF_MC_ITEM_57"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, "MISSING CHAIN GUARD", "", row["SAF_MC_ITEM_57"].ToString());
            strItem = "BODY DAMAGE";
            if ((row["SAF_MC_SUBITEM_51"].ToString()[0] == '2') || (row["SAF_MC_SUBITEM_51"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "SHARP / PROTRUDING AREA", row["SAF_MC_SUBITEM_51"].ToString());
            if ((row["SAF_MC_SUBITEM_52"].ToString()[0] == '2') || (row["SAF_MC_SUBITEM_52"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "MISSING FENDER", row["SAF_MC_SUBITEM_52"].ToString());
            if ((row["SAF_MC_SUBITEM_53"].ToString()[0] == '2') || (row["SAF_MC_SUBITEM_53"].ToString()[0] == '3')) html += Create3CellTableRow(strSystem, strItem, "CRACKED / BROKEN COMPONENT", row["SAF_MC_SUBITEM_53"].ToString());

            #endregion

            return html;
        }

        public static StringBuilder InqInspectionBySQLbyOLTP(StringBuilder sb)
        {
            sb.AppendLine("    SELECT ");
            sb.AppendLine("            T.VEHICLESEQUENCENUM AS \"Vehicle Sequence Number\" ");
            sb.AppendLine(",           T.TESTDATE AS \"Test Date\" ");
            sb.AppendLine(",           T.TESTDATE AS \"60 Day End Date\" ");
            sb.AppendLine(",           T.TESTENDTIME AS \"Test Time\" ");
            sb.AppendLine(",           T.VIN AS \"VIN\" ");
            ////sb.AppendLine(",           T.ALTVIN AS \"Alt VIN\" ");
            sb.AppendLine(",           T.TAG AS \"License\" ");
            sb.AppendLine(",           T.MAKE AS \"Make\" ");
            sb.AppendLine(",           T.MODEL AS \"Model\" ");
            sb.AppendLine(",           T.MODELYEAR AS \"Model Year\" ");
            //sb.AppendLine(",           T.VEHICLESTYLE AS \"Vehicle Style\" ");
            sb.AppendLine(",           T.FUELCODE AS \"Fuel Code\" ");
            sb.AppendLine(",           T.ENGINESIZE AS \"Engine Size\" ");
            sb.AppendLine(",           T.NUMCYLINDERS AS \"Number Cylinders\" ");
            sb.AppendLine(",           T.ODOMETER AS \"Tested As Odometer\" ");
            sb.AppendLine(",           T.TRANSTYPE AS \"Transmission\" ");
            sb.AppendLine(",           T.BODYSTYLE AS \"Body Style\" ");
            sb.AppendLine(",           T.BODYSTYLE AS \"Tested As Body Style\" ");
            sb.AppendLine(",           T.PLATETYPE AS \"Plate Type\" ");
            //sb.AppendLine(",           T.REGGVW AS \"Registration GVW\" ");
            sb.AppendLine(",           T.REGDATE AS \"Registration Date\" ");
            sb.AppendLine(",           T.REGEXPIRATION AS \"Registration Expiration\" ");
            sb.AppendLine(",           T.INITIALTESTDATE AS \"Init Test Date\" ");
            sb.AppendLine(",           T.INITIALTESTTIME AS \"Init Test Time\" ");
            sb.AppendLine(",           T.VISUALPF AS \"Visual Result\" ");
            sb.AppendLine(",           T.VISUALPF AS \"Visual Inspection Result\" ");
            sb.AppendLine(",           T.OBDPF AS \"OBD Result\" ");
            sb.AppendLine(",           T.SAFETYPF AS \"Safety Result\" ");
            sb.AppendLine(",           T.OVERALLPF AS \"Overall Result\" ");
            sb.AppendLine(",           T.STATIONID AS \"Station ID\" ");
            sb.AppendLine(",           T.INSPECTORID AS \"Inspector ID\" ");
            sb.AppendLine(",           T.UNITID AS \"Unit ID\" ");
            ////sb.AppendLine(",           T.STICKERTYPE AS \"Sticker Type\" ");
            ////sb.AppendLine(",           T.STICKERSERIES AS \"Sticker Series\" ");
            ////sb.AppendLine(",           T.STICKERNUMBER AS \"Sticker Number\" ");
            sb.AppendLine(",           T.STICKERTYPE || '-' || T.STICKERSERIES || '-' || T.STICKERNUMBER AS \"Sticker Number\" ");
            sb.AppendLine(",           T.TESTSEQUENCE AS \"Test Sequence\" ");
            //sb.AppendLine(",           T.TESTTYPE AS \"Test Type\" ");
            ////sb.AppendLine(",           T.LOOKUPINDEX AS \"Lookup Index\" ");
            sb.AppendLine(",           T.VEHICLETYPE AS \"Vehicle Type\" ");
            //sb.AppendLine(",           T.ACTUALGVW AS \"Actual GVW\" ");
            sb.AppendLine(",           T.TESTASMODELYR AS \"Tested As Model Year\" ");
            sb.AppendLine(",           T.TESTASMAKE AS \"Tested As Make\" ");
            sb.AppendLine(",           T.TESTASMODEL AS \"Tested As Model\" ");
            sb.AppendLine(",           T.TESTASFUELCODE AS \"Tested As Fuel Code\" ");
            sb.AppendLine(",           T.TESTASGVW AS \"Tested As GVW\" ");
            sb.AppendLine(",           T.SAFETYTESTTYPE AS \"Tested As Vehicle Type\" ");
            sb.AppendLine(",           T.EMISSTESTTYPE AS \"Test Type\" ");
            sb.AppendLine(",           CASE SUBSTR(T.REPAIRAMOUNTDURINGTEST,1,1) ");
            sb.AppendLine("               WHEN 'N' THEN 'NO' ");
            sb.AppendLine("               WHEN 'Y' THEN 'YES' ");
            sb.AppendLine("            ELSE ");
            sb.AppendLine("               'UNKNOWN' END AS \"Adequate Voltage\" ");
            //sb.AppendLine(",           T.VINDECODED AS \"Vehicle Sequence Number\" ");
            //sb.AppendLine(",           T.RETESTCODEUSED AS \"Vehicle Sequence Number\" ");
            //sb.AppendLine(",           T.OBDWAIVER AS \"Vehicle Sequence Number\" ");
            //sb.AppendLine(",           T.OBDADVISORY AS \"Vehicle Sequence Number\" ");
            sb.AppendLine(",           T.VISPCV AS \"PCV\" ");
            sb.AppendLine(",           T.VISAIRPUMP AS \"Air Pump\" ");
            sb.AppendLine(",           T.VISEVAP AS \"Evap\" ");
            sb.AppendLine(",           T.VISFUEL AS \"Fuel System\" ");
            sb.AppendLine(",           T.VISCAT AS \"Catalyst\" ");
            //sb.AppendLine(",           T.TESTBEGINTIME AS \"Vehicle Sequence Number\" ");
            //sb.AppendLine(",           T.EMISSIONSENDTIME AS \"Vehicle Sequence Number\" ");
            sb.AppendLine(",           T.OBDMISFIRE AS \"Misfire\" ");
            sb.AppendLine(",           T.OBDFUELSYSTEM AS \"OBD Fuel System\" ");
            sb.AppendLine(",           T.OBDCOMPONENT AS \"Component\" ");
            sb.AppendLine(",           T.OBDCATALYST AS \"OBD Catalyst\" ");
            sb.AppendLine(",           T.OBDHEATEDCATALYST AS \"Heated Catalyst\" ");
            sb.AppendLine(",           T.OBDEVAP AS \"Evap System\" ");
            sb.AppendLine(",           T.OBDAIRSYSTEM AS \"Air System\" ");
            sb.AppendLine(",           T.OBDAC AS \"AC System\" ");
            sb.AppendLine(",           T.OBDOXYGEN AS \"Oxygen Sensor\" ");
            sb.AppendLine(",           T.OBDHEATEDOXYGEN AS \"Heated O2 Sensor\" ");
            sb.AppendLine(",           T.OBDEGR AS \"EGR System\" ");
            sb.AppendLine(",           T.OBDMILCMDON AS \"MIL Status\" ");
            sb.AppendLine(",           ' ' AS \"OBD Inspection Result\" ");
            sb.AppendLine(",           T.DTC1  AS \"DTC-1\" ");
            sb.AppendLine(",           T.DTC2  AS \"DTC-2\" ");
            sb.AppendLine(",           T.DTC3  AS \"DTC-3\" ");
            sb.AppendLine(",           T.DTC4  AS \"DTC-4\" ");
            sb.AppendLine(",           T.DTC5  AS \"DTC-5\" ");
            sb.AppendLine(",           T.DTC6  AS \"DTC-6\" ");
            sb.AppendLine(",           T.DTC7  AS \"DTC-7\" ");
            sb.AppendLine(",           T.DTC8  AS \"DTC-8\" ");
            sb.AppendLine(",           T.DTC9  AS \"DTC-9\" ");
            sb.AppendLine(",           T.DTC10 AS \"DTC-10\" ");
            sb.AppendLine(",           CASE WHEN T.OBDWAIVER IN ('Y') THEN 'YES' ");
            sb.AppendLine("                 WHEN T.OBDWAIVER IN ('N') THEN 'NO' ");
            sb.AppendLine("            ELSE NULL END || '/' || ");
            sb.AppendLine("            CASE WHEN T.OBDADVISORY IN ('Y') THEN 'YES' ");
            sb.AppendLine("                 WHEN T.OBDADVISORY IN ('N') THEN 'NO' ");
            sb.AppendLine("            ELSE NULL END AS \"OBD Waiver / Advisory\" ");
            sb.AppendLine(",           T.OBDMILENGOFF AS \"Key On Engine Off\" ");
            sb.AppendLine(",           T.OBDMILENGON AS \"Key On Engine Running\" ");
            //sb.AppendLine(",           T.OBDLOCATABLE AS \"DTC-1\" ");
            //sb.AppendLine(",           T.OBDCOMMUNICABLE AS \"DTC-1\" ");
            sb.AppendLine(",           T.OBDNUMDTCS AS \"Number of DTCs\" ");
            //sb.AppendLine(",           T.OBDSTEP AS \"DTC-1\" ");
            //sb.AppendLine(",           T.OBDEINFO AS \"DTC-1\" ");
            //sb.AppendLine(",           T.OBDSWVERSION AS \"DTC-1\" ");
            sb.AppendLine(",           T.CHARGEABLE AS \"Chargeable\" ");
            sb.AppendLine(",           T.OFFLINEBEGINNINGOFTEST AS \"OffLine Begin of Inspection\" ");
            sb.AppendLine(",           T.OFFLINEENDOFTEST AS \"OffLine End of Inspection\" ");
            //sb.AppendLine(",           T.REPAIRAMOUNTBETWEENTESTS AS \"DTC-1\" ");
            //sb.AppendLine(",           T.INQUIRYRESPSEC AS \"DTC-1\" ");
            //sb.AppendLine(",           T.UPDATERESPSEC AS \"DTC-1\" ");
            //sb.AppendLine(",           T.INQUIRYDAILATTEMPS AS \"DTC-1\" ");
            //sb.AppendLine(",           T.UPDATEDDAILATTEMPS AS \"DTC-1\" ");
            //sb.AppendLine(",           T.INQUIRYPHONENUM AS \"DTC-1\" ");
            //sb.AppendLine(",           T.UPDATEPHONENUM AS \"DTC-1\" ");
            //sb.AppendLine(",           T.DTAPPSERVERRECEIVED AS \"DTC-1\" ");
            //sb.AppendLine(",           T.LASTEDITSOURCE AS \"DTC-1\" ");
            //sb.AppendLine(",           T.RECEIVEDATE AS \"DTC-1\" ");
            //sb.AppendLine(",           T.REVIEW AS \"DTC-1\" ");
            //sb.AppendLine(",           T.REVIEWED AS \"DTC-1\" ");
            //sb.AppendLine(",           T.REVIEWTYPE AS \"DTC-1\" ");
            sb.AppendLine(",           T.REGISTRATIONNUM AS \"Registration Number\" ");
            sb.AppendLine(",           T.NOTES AS \"Notes\" ");
            sb.AppendLine(",           V.REGISTRATIONNUM AS \"VMT Registration Number\" ");
            sb.AppendLine(",           V.MODELYEAR AS \"VMT Model Year\" ");
            sb.AppendLine(",           V.FUELCODE AS \"VMT Fuel Code\" ");
            sb.AppendLine(",           V.MAKE AS \"VMT Make\" ");
            sb.AppendLine(",           V.BODYSTYLE AS \"VMT Body Style\" ");
            sb.AppendLine(",           V.MODEL AS \"VMT Model\" ");
            sb.AppendLine(",           V.TAG AS \"VMT License\" ");
            sb.AppendLine(",           V.TRANSTYPE AS \"VMT Transmission\" ");
            sb.AppendLine(",           V.PLATETYPE AS \"VMT Plate Type\" ");
            sb.AppendLine(",           V.NUMCYLINDERS AS \"VMT Number Cylinders\" ");
            sb.AppendLine(",           V.ENGINESIZE AS \"VMT Engine Size\" ");
            sb.AppendLine(",           V.REGDATE AS \"VMT Registration Date\" ");
            sb.AppendLine(",           V.REGEXPIRATION AS \"VMT Registration Expiration\" ");
            sb.AppendLine(",           V.ODOMETER AS \"VMT Odometer\" ");
            sb.AppendLine(",           V.VEHICLETYPE AS \"VMT Vehicle Type\" ");
            sb.AppendLine(",           V.ACTUALGVW AS \"VMT GVW\" ");
            sb.AppendLine("       FROM TESTRECORD T ");
            sb.AppendLine(" LEFT OUTER JOIN VEHICLEMASTERTBL V ON V.VEHICLESEQUENCENUMBER = T.VEHICLESEQUENCENUM AND V.TESTDATE = T.TESTDATE AND V.TESTENDTIME = T.TESTENDTIME ");

            return sb;
        }
    }
}