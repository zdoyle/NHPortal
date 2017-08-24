using GDCoreUtilities.Logging;
using NHPortal.Classes;
using NHPortal.Classes.User;
using PortalFramework.ReportModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NHPortal.Reports
{
    public partial class StationReport : NHPortal.Classes.PortalPage
    {
        private DataRow s_currentRow;
        private StationReportRowInfoCollection s_reportData;

        protected void Page_Load(object sender, EventArgs e)
        {
            RedirectOnInvalidPermission(UserPermissions.StationReport);

            if (IsPostBack)
            {
                ProcessPostBack();
            }
            else
            {
                InitializePage();
                ParseQueryStrings();
                LoadFavorite();
                RunReport();

            }
        }

        private void InitializePage()
        {
            Master.SetHeaderText("Station Audit Report");
            lstOfficerType.Initialize();
            lstStationType.Initialize();
        }

        private void ProcessPostBack()
        {
            string action = Master.MasterPage.HidActionValue;
            switch (action)
            {
                case "RUN_REPORT":
                    RunReport();
                    break;
                case "SAVE_FAVORITE":
                    SaveFavorite();
                    break;
            }
        }
        private void ParseQueryStrings()
        {
            // set to start of month for month to date report
            dateSelector.StartDateControl.Text = DateTime.Now.ToString("M/dd/yyyy");

        }
        private void SaveFavorite()
        {
            Master.SaveFavorite(NHPortal.UserControls.RedirectCodes.REPORT_STATION, UserFavoriteTypes.Report);
        }

        private void LoadFavorite()
        {
            UserFavorite fav = SessionHelper.GetSelectedFavorite(this.Session);
            if (fav != null)
            {
                foreach (var c in fav.Criteria)
                {
                    switch (c.Description.Trim().ToUpper())
                    {
                        case "START DATE":
                            dateSelector.StartDateControl.Text = c.Value;
                            break;
                        case "END DATE":
                            dateSelector.EndDateControl.Text = c.Value;
                            break;
                        case "OFFICER":
                            lstOfficerType.SetSelectedValues(c.Value.Split(new char[] { ',' }));
                            break;
                        case "STATION TYPE":
                            lstStationType.SetSelectedTexts(c.Value.Split(new char[] { ',' }));
                            break;
                    }
                }
            }
        }

        private void SetMetaData()
        {
            if (Master.UserReport != null)
            {
                Master.UserReport.MetaData.Add("Start Date", dateSelector.StartDateControl.Text);
                Master.UserReport.MetaData.Add("End Date", dateSelector.EndDateControl.Text);

                if (lstOfficerType.ValuesSelected)
                {
                    Master.UserReport.MetaData.Add("Officer", lstOfficerType.GetDelimitedText(", "), lstOfficerType.GetDelimitedValues(","));
                }
                if (lstStationType.ValuesSelected)
                {
                    Master.UserReport.MetaData.Add("Station Type", lstStationType.GetDelimitedText(", "), lstStationType.GetDelimitedValues(","));
                }
            }
        }
        private void SetColumnTypes()
        {
            for (int i = 1; i < Master.UserReport.ColumnCount; i++)
            {
                Master.UserReport.Columns[i].ColumnDataType = ColumnDataType.Number;
            }
        }
        private void RunReport()
        {
            string qry = BuildQuery();
            GDDatabaseClient.Oracle.OracleResponse response = PortalFramework.Database.ODAP.GetDataTable(qry, PortalFramework.Database.DatabaseTarget.Adhoc);
            LogMessage("Attempting to run query...", LogSeverity.Information);
            if (response.Successful)
            {
                LogMessage("Query is successful", LogSeverity.Information);
                Master.UserReport = new Report("Station Audit Report");
                SetMetaData();
                Master.SetDataLastUpdate(DateTime.Now);

                if (response.HasResults)
                {
                    LogMessage("Query has results", LogSeverity.Information);
                    AddColumnsToReport();
                    // SetColumnTypes();
                    ApplyDataTableToReport(response.ResultsTable);
                    Master.UserReport.FooterNote = "* Percentages apply to columns. Percentages of individual items may add up to more than 100% due to multiple violation causes identified in some audits." + Environment.NewLine;
                    //Master.UserReport.Sortable = true;
                    Master.UserReport.FooterVisibility = FooterVisibility.Auto;
                    LogMessage("Data rendering to page", LogSeverity.Information);
                }
            }
            else
            {
                Master.UserReport = null;
                Master.SetError(response.Exception);
                LogMessage("Query error: " + response.Exception, LogSeverity.Information);
            }
            LogMessage("Query Finshed", LogSeverity.Information);
            LogOracleResponse(response);
            Master.RenderReportToPage();
            LogMessage("Page Loaded", LogSeverity.Information);
            SessionHelper.SetCurrentReport(this.Session, Master.UserReport);
        }

        //Add columns to report AND formatted for exports
        private void AddColumnsToReport()
        {
            if (Master.UserReport != null)
            {
                Master.UserReport.Columns.Add("desc", "Description");
                Master.UserReport.Columns.Add("total_audit", "Total", ColumnDataType.Number);
                Master.UserReport.Columns.Add("total_audit_percent", "Total %*", ColumnDataType.Percentage);
                Master.UserReport.Columns.Add("init_audit", "Initial Audits", ColumnDataType.Number);
                Master.UserReport.Columns.Add("init_audit_percent", "Initial Audits %*", ColumnDataType.Percentage);
                Master.UserReport.Columns.Add("follow_up_audits", "Follow Up Audits", ColumnDataType.Number);
                Master.UserReport.Columns.Add("follow_up_audits_percent", "Follow Up Audits %*", ColumnDataType.Percentage);
            }
        }
        private int GetCellValue(string column)
        {
            return GDCoreUtilities.NullSafe.ToInt(s_currentRow[column].ToString());
        }
        private void ApplyDataTableToReport(DataTable dt)
        {
            s_reportData = new StationReportRowInfoCollection();
            CalculateTotals(dt);
            AddDataToReport();
        }

        //Totals of each row. 
        private void CalculateTotals(DataTable dt)
        {
            for (int i = 1; i <= dt.Rows.Count; i++)
            {
                s_currentRow = dt.Rows[i - 1];

                //total initial audits column

                //total Deficiencies section
                int totalCertDef = GetCellValue("Total_Station_Certification") + GetCellValue("Total_Mechanic_Certification");
                int totalGeneralReq = GetCellValue("Total_Hours_Displayed_General") + GetCellValue("Total_Station_Sign_General") + GetCellValue("Total_Fee_Sign_General") + GetCellValue("Total_Motorcycle_Sign_General") + GetCellValue("Total_Inspection_Area_General") + GetCellValue("Total_Inspection_Space_General") + GetCellValue("Total_Rejection_Form_General") + GetCellValue("Total_Mechanic_Duty_General");
                int totalStickerReq = GetCellValue("Total_Stick_Secured_Sticker") + GetCellValue("Total_Stick_Returned_Sticker") + GetCellValue("Total_Stick_Sub_Compl_Sticker") + GetCellValue("Total_Stick_Back_Compl_Sticker");
                int totalTools = GetCellValue("Total_Brake_Drum_Tool") + GetCellValue("Total_Brake_Pad_Tool") + GetCellValue("Total_Ball_Joint_Tool") + GetCellValue("Total_Lift_Jack_Tool") + GetCellValue("Total_Hand_Tool") + GetCellValue("Total_Headlight_Tool") + GetCellValue("Total_Tint_Tool") + GetCellValue("Total_DecibalMeter_Tool");
                //int totalDef = totalCertDef; 

                //sum up total for each deficiency type
                //totals for all under total deficiencies
                s_reportData.TotalAudits[1] += GetCellValue("Total_Audits");
                s_reportData.TotalDeficiencies[1] += totalCertDef + totalGeneralReq + totalStickerReq + totalTools + GetCellValue("Total_NHOST_Unit") + GetCellValue("Total_Manuals");
                s_reportData.TotalCertifications[1] += totalCertDef;
                s_reportData.TotalGeneralRequirements[1] += totalGeneralReq;
                s_reportData.TotalStickerRequirements[1] += totalStickerReq;
                s_reportData.TotalToolsEquipment[1] += totalTools;
                s_reportData.TotalNHOSTUnit[1] += GetCellValue("Total_NHOST_Unit");
                s_reportData.Manuals[1] += GetCellValue("Total_Manuals");

                //totals for all under Certification Deficiencies
                s_reportData.TotalCertificationDeficiencies[1] += totalCertDef;
                s_reportData.TotalCertificationStation[1] += GetCellValue("Total_Station_Certification");
                s_reportData.TotalCertificationMechanics[1] += GetCellValue("Total_Mechanic_Certification");

                //totals for all under General Deficiencies
                s_reportData.TotalGeneralDeficiencies[1] += totalGeneralReq;
                s_reportData.TotalGeneralHoursDisplayed[1] += GetCellValue("Total_Hours_Displayed_General");
                s_reportData.TotalGeneralStationSign[1] += GetCellValue("Total_Station_Sign_General");
                s_reportData.TotalGeneralFeeSign[1] += GetCellValue("Total_Fee_Sign_General");
                s_reportData.TotalGeneralMotorcycleSign[1] += GetCellValue("Total_Motorcycle_Sign_General");
                s_reportData.TotalGeneralInspectionArea[1] += GetCellValue("Total_Inspection_Area_General");
                s_reportData.TotalGeneralInspectionSpace[1] += GetCellValue("Total_Inspection_Space_General");
                s_reportData.TotalGeneralRejectionForm[1] += GetCellValue("Total_Rejection_Form_General");
                s_reportData.TotalGeneralMechanicsOnDuty[1] += GetCellValue("Total_Mechanic_Duty_General");

                //totals for all under sticker deficiencies
                s_reportData.TotalStickerDeficency[1] += totalStickerReq;
                s_reportData.TotalStickerStubCompletion[1] += GetCellValue("Total_Stick_Sub_Compl_Sticker");
                s_reportData.TotalStickerBackCompletion[1] += GetCellValue("Total_Stick_Back_Compl_Sticker");
                s_reportData.TotalStickerProperlySecured[1] += GetCellValue("Total_Stick_Secured_Sticker");
                s_reportData.TotalStickerProperlyReturned[1] += GetCellValue("Total_Stick_Returned_Sticker");

                //totals for all under tools/equipment deficiencies
                s_reportData.TotalToolsEquipmentDeficiencies[1] += totalTools;
                s_reportData.TotalToolsEquipmentBrakeDrumGauge[1] += GetCellValue("Total_Brake_Drum_Tool");
                s_reportData.TotalToolsEquipmentBrakePadGauge[1] += GetCellValue("Total_Brake_Pad_Tool");
                s_reportData.TotalToolsEquipmentBallJointGauge[1] += GetCellValue("Total_Ball_Joint_Tool");
                s_reportData.TotalToolsEquipmentLiftOrJack[1] += GetCellValue("Total_Lift_Jack_Tool");
                s_reportData.TotalToolsEquipmentHandTools[1] += GetCellValue("Total_Hand_Tool");
                s_reportData.TotalToolsEquipmentHeadlightTools[1] += GetCellValue("Total_Headlight_Tool");
                s_reportData.TotalToolsEquipmentTintMeter[1] += GetCellValue("Total_Tint_Tool");
                s_reportData.TotalToolsEquipmentDecibelMeter[1] += GetCellValue("Total_DecibalMeter_Tool");


                //FOLLOW UP audits
                //total Deficiencies section
                int FUTotalCertDef = GetCellValue("FUTotal_Station_Cert") + GetCellValue("FUTotal_Mechanic_Cert");
                int FUTotalGeneralReq = GetCellValue("FUTotal_Hours_Displayed_Gen") + GetCellValue("FUTotal_Station_Sign_Gen") + GetCellValue("FUTotal_Fee_Sign_Gen") + GetCellValue("FUTotal_Motorcycle_Sign_Gen") + GetCellValue("FUTotal_Inspection_Area_Gen") + GetCellValue("FUTotal_Inspection_Space_Gen") + GetCellValue("FUTotal_Rejection_Form_Gen") + GetCellValue("FUTotal_Mechanic_Duty_Gen");
                int FUTotalStickerReq = GetCellValue("FUTotal_Stick_Secured_Stick") + GetCellValue("FUTotal_Stick_Returned_Stick") + GetCellValue("FUTotal_Stick_Sub_Compl_Stick") + GetCellValue("FUTotal_Stick_Back_Compl_Stick");
                int FUTotalTools = GetCellValue("FUTotal_Brake_Drum_Tool") + GetCellValue("FUTotal_Brake_Pad_Tool") + GetCellValue("FUTotal_Ball_Joint_Tool") + GetCellValue("FUTotal_Lift_Jack_Tool") + GetCellValue("FUTotal_Hand_Tool") + GetCellValue("FUTotal_Headlight_Tool") + GetCellValue("FUTotal_Tint_Tool") + GetCellValue("FUTotal_Decibal_Tool");
                int FUTotal_Audits = FUTotalCertDef + FUTotalGeneralReq + FUTotalStickerReq + FUTotalTools + GetCellValue("FU_NHOST_Unit") + GetCellValue("FU_Manuals");

                //sum up FUTotal for each deficiency type
                //totals for all under total deficiencies
                s_reportData.TotalAudits[2] += GetCellValue("fuTotal_Audits");
                s_reportData.TotalDeficiencies[2] += FUTotalCertDef + FUTotalGeneralReq + FUTotalStickerReq + FUTotalTools + GetCellValue("FU_NHOST_Unit") + GetCellValue("FU_Manuals");
                s_reportData.TotalCertifications[2] += FUTotalCertDef;
                s_reportData.TotalGeneralRequirements[2] += FUTotalGeneralReq;
                s_reportData.TotalStickerRequirements[2] += FUTotalStickerReq;
                s_reportData.TotalToolsEquipment[2] += FUTotalTools;
                s_reportData.TotalNHOSTUnit[2] += GetCellValue("FU_NHOST_Unit");
                s_reportData.Manuals[2] += GetCellValue("FU_Manuals");

                //Certification Deficiencies
                s_reportData.TotalCertificationDeficiencies[2] += FUTotalCertDef;
                s_reportData.TotalCertificationStation[2] += GetCellValue("FUTotal_Station_Cert");
                s_reportData.TotalCertificationMechanics[2] += GetCellValue("FUTotal_Mechanic_Cert");

                //General Deficiencies
                s_reportData.TotalGeneralDeficiencies[2] += FUTotalGeneralReq;
                s_reportData.TotalGeneralHoursDisplayed[2] += GetCellValue("FUTotal_Hours_Displayed_Gen");
                s_reportData.TotalGeneralStationSign[2] += GetCellValue("FUTotal_Station_Sign_Gen");
                s_reportData.TotalGeneralFeeSign[2] += GetCellValue("FUTotal_Fee_Sign_Gen");
                s_reportData.TotalGeneralMotorcycleSign[2] += GetCellValue("FUTotal_Motorcycle_Sign_Gen");
                s_reportData.TotalGeneralInspectionArea[2] += GetCellValue("FUTotal_Inspection_Area_Gen");
                s_reportData.TotalGeneralInspectionSpace[2] += GetCellValue("FUTotal_Inspection_Space_Gen");
                s_reportData.TotalGeneralRejectionForm[2] += GetCellValue("FUTotal_Rejection_Form_Gen");
                s_reportData.TotalGeneralMechanicsOnDuty[2] += GetCellValue("FUTotal_Mechanic_Duty_Gen");

                //sticker deficiencies
                s_reportData.TotalStickerDeficency[2] += FUTotalStickerReq;
                s_reportData.TotalStickerStubCompletion[2] += GetCellValue("FUTotal_Stick_Sub_Compl_stick");
                s_reportData.TotalStickerBackCompletion[2] += GetCellValue("FUTotal_Stick_Back_Compl_stick");
                s_reportData.TotalStickerProperlySecured[2] += GetCellValue("FUTotal_Stick_Secured_stick");
                s_reportData.TotalStickerProperlyReturned[2] += GetCellValue("FUTotal_Stick_Returned_stick");

                //tools/equipment deficiencies
                s_reportData.TotalToolsEquipmentDeficiencies[2] += FUTotalTools;
                s_reportData.TotalToolsEquipmentBrakeDrumGauge[2] += GetCellValue("FUTotal_Brake_Drum_Tool");
                s_reportData.TotalToolsEquipmentBrakePadGauge[2] += GetCellValue("FUTotal_Brake_Pad_Tool");
                s_reportData.TotalToolsEquipmentBallJointGauge[2] += GetCellValue("FUTotal_Ball_Joint_Tool");
                s_reportData.TotalToolsEquipmentLiftOrJack[2] += GetCellValue("FUTotal_Lift_Jack_Tool");
                s_reportData.TotalToolsEquipmentHandTools[2] += GetCellValue("FUTotal_Hand_Tool");
                s_reportData.TotalToolsEquipmentHeadlightTools[2] += GetCellValue("FUTotal_Headlight_Tool");
                s_reportData.TotalToolsEquipmentTintMeter[2] += GetCellValue("FUTotal_Tint_Tool");
                s_reportData.TotalToolsEquipmentDecibelMeter[2] += GetCellValue("FUTotal_Decibal_Tool");

                //TOTALS
                //sum up total for each deficiency type
                s_reportData.TotalAudits[0] += GetCellValue("Total_Audits") + GetCellValue("fuTotal_Audits"); ;
                s_reportData.TotalDeficiencies[0] += totalCertDef + totalGeneralReq + totalStickerReq + totalTools + GetCellValue("Total_NHOST_Unit") + GetCellValue("Total_Manuals") + FUTotalCertDef + FUTotalGeneralReq + FUTotalStickerReq + FUTotalTools + GetCellValue("FU_NHOST_Unit") + GetCellValue("FU_Manuals");
                s_reportData.TotalCertifications[0] += totalCertDef + FUTotalCertDef;
                s_reportData.TotalGeneralRequirements[0] += totalGeneralReq + FUTotalGeneralReq;
                s_reportData.TotalStickerRequirements[0] += totalStickerReq + FUTotalStickerReq;
                s_reportData.TotalToolsEquipment[0] += totalTools + FUTotalTools;
                s_reportData.TotalNHOSTUnit[0] += GetCellValue("Total_NHOST_Unit") + GetCellValue("FU_NHOST_Unit");
                s_reportData.Manuals[0] += GetCellValue("Total_Manuals") + GetCellValue("FU_Manuals");

                //Certification Deficiencies
                s_reportData.TotalCertificationDeficiencies[0] += totalCertDef + FUTotalCertDef;
                s_reportData.TotalCertificationStation[0] += GetCellValue("Total_Station_Certification") + GetCellValue("FUTotal_Station_Cert");
                s_reportData.TotalCertificationMechanics[0] += GetCellValue("Total_Mechanic_Certification") + GetCellValue("FUTotal_Mechanic_Cert");

                //General Deficiencies
                s_reportData.TotalGeneralDeficiencies[0] += totalGeneralReq + FUTotalGeneralReq;
                s_reportData.TotalGeneralHoursDisplayed[0] += GetCellValue("Total_Hours_Displayed_General") + GetCellValue("FUTotal_Hours_Displayed_Gen");
                s_reportData.TotalGeneralStationSign[0] += GetCellValue("Total_Station_Sign_General") + GetCellValue("FUTotal_Station_Sign_Gen");
                s_reportData.TotalGeneralFeeSign[0] += GetCellValue("Total_Fee_Sign_General") + GetCellValue("FUTotal_Fee_Sign_Gen");
                s_reportData.TotalGeneralMotorcycleSign[0] += GetCellValue("Total_Motorcycle_Sign_General") + GetCellValue("FUTotal_Motorcycle_Sign_Gen");
                s_reportData.TotalGeneralInspectionArea[0] += GetCellValue("Total_Inspection_Area_General") + GetCellValue("FUTotal_Inspection_Area_Gen");
                s_reportData.TotalGeneralInspectionSpace[0] += GetCellValue("Total_Inspection_Space_General") + GetCellValue("FUTotal_Inspection_Space_Gen");
                s_reportData.TotalGeneralRejectionForm[0] += GetCellValue("Total_Rejection_Form_General") + GetCellValue("FUTotal_Rejection_Form_Gen");
                s_reportData.TotalGeneralMechanicsOnDuty[0] += GetCellValue("Total_Mechanic_Duty_General") + GetCellValue("FUTotal_Mechanic_Duty_Gen");

                //sticker deficiencies
                s_reportData.TotalStickerDeficency[0] += totalStickerReq + FUTotalStickerReq;
                s_reportData.TotalStickerStubCompletion[0] += GetCellValue("Total_Stick_Sub_Compl_Sticker") + GetCellValue("FUTotal_Stick_Sub_Compl_stick");
                s_reportData.TotalStickerBackCompletion[0] += GetCellValue("Total_Stick_Back_Compl_Sticker") + GetCellValue("FUTotal_Stick_Back_Compl_stick");
                s_reportData.TotalStickerProperlySecured[0] += GetCellValue("Total_Stick_Secured_Sticker") + GetCellValue("FUTotal_Stick_Secured_stick");
                s_reportData.TotalStickerProperlyReturned[0] += GetCellValue("Total_Stick_Returned_Sticker") + GetCellValue("FUTotal_Stick_Returned_stick");

                //tools/equipment deficiencies
                s_reportData.TotalToolsEquipmentDeficiencies[0] += totalTools + FUTotalTools;
                s_reportData.TotalToolsEquipmentBrakeDrumGauge[0] += GetCellValue("Total_Brake_Drum_Tool") + GetCellValue("FUTotal_Brake_Drum_Tool");
                s_reportData.TotalToolsEquipmentBrakePadGauge[0] += GetCellValue("Total_Brake_Pad_Tool") + GetCellValue("FUTotal_Brake_Pad_Tool");
                s_reportData.TotalToolsEquipmentBallJointGauge[0] += GetCellValue("Total_Ball_Joint_Tool") + GetCellValue("FUTotal_Ball_Joint_Tool");
                s_reportData.TotalToolsEquipmentLiftOrJack[0] += GetCellValue("Total_Lift_Jack_Tool") + GetCellValue("FUTotal_Lift_Jack_Tool");
                s_reportData.TotalToolsEquipmentHandTools[0] += GetCellValue("Total_Hand_Tool") + GetCellValue("FUTotal_Hand_Tool");
                s_reportData.TotalToolsEquipmentHeadlightTools[0] += GetCellValue("Total_Headlight_Tool") + GetCellValue("FUTotal_Headlight_Tool");
                s_reportData.TotalToolsEquipmentTintMeter[0] += GetCellValue("Total_Tint_Tool") + GetCellValue("FUTotal_Tint_Tool");
                s_reportData.TotalToolsEquipmentDecibelMeter[0] += GetCellValue("Total_DecibalMeter_Tool") + GetCellValue("FUTotal_Decibal_Tool");
            }
        }

        private void AddDataToReport()
        {
            int idx = 0;
            foreach (var data in s_reportData)
            {
                AddRowToReport(data, idx);
                idx++;
            }
        }
        // displays each row
        private void AddRowToReport(StationReportRowInfo data, int rowIndex)
        {
            ReportRow rptRow = new ReportRow(Master.UserReport);
            rptRow.Cells.Add(data.Description);

            if (data.ContainsData)
            {
                for (int i = 0; i < 3; i++)
                {
                    //display data
                    int denom = s_reportData.GetDenominator(rowIndex, i);
                    rptRow.Cells.Add(data[i]);
                    rptRow.Cells.Add(PercentString(data[i], denom));
                }
            }
            else
            {
                //there is no data
                rptRow.Cells[0].ColumnSpan = Master.UserReport.ColumnCount;
            }

            Master.UserReport.Rows.Add(rptRow);
        }
        private string PercentString(int num, int den)
        {
            string percent = "0.0%";
            if (den != 0)
            {
                double d = (double)num / (double)den;
                percent = d.ToString("0.0%");
            }
            return percent;
        }
        private string BuildQuery()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("SELECT ");
            sb.AppendLine("AUDITORID");
            //sb.AppendLine("DISTINCT SUBSTR(AUDITORID,7,10) AS AUDITORID");
            sb.AppendLine(", sum(case when AUDITSEQ = '01' then 1 else 0 end) as Total_Audits");
            sb.AppendLine(", sum(case when (CODE1 = '2' OR CODE2 = '2' or CODE3 = '2' OR CODE4 = '2') AND AUDITSEQ = '01' then 1 else 0 end) as Total_Station_Certification");
            sb.AppendLine(", sum(case when CODE5 = '2'  AND AUDITSEQ = '01' then 1 else 0 end) as Total_Mechanic_Certification");
            sb.AppendLine(", sum(case when CODE6 = '2'  AND AUDITSEQ = '01' then 1 else 0 end) as Total_Hours_Displayed_General");
            sb.AppendLine(", sum(case when CODE7 = '2' AND AUDITSEQ = '01' then 1 else 0 end) as Total_Station_Sign_General");
            sb.AppendLine(", sum(case when CODE8 = '2'  and AUDITSEQ = '01' then 1 else 0 end) as Total_Fee_Sign_General");
            sb.AppendLine(", sum(case when CODE9 = '2'  AND AUDITSEQ = '01' then 1 else 0 end) as Total_Motorcycle_Sign_General");
            sb.AppendLine(", sum(case when CODE10 = '2'  AND AUDITSEQ = '01' then 1 else 0 end) as Total_Inspection_Area_General");
            sb.AppendLine(", sum(case when CODE11 = '2'  AND AUDITSEQ = '01' then 1 else 0 end) as Total_Inspection_Space_General");
            sb.AppendLine(", sum(case when CODE12 = '2'  AND AUDITSEQ = '01' then 1 else 0 end) as Total_Rejection_Form_General");
            sb.AppendLine(", sum(case when CODE13 = '2'  AND AUDITSEQ = '01' then 1 else 0 end) as Total_Mechanic_Duty_General");
            sb.AppendLine(", sum(case when CODE14 = '2' AND AUDITSEQ = '01'  then 1 else 0 end) as Total_Stick_Secured_Sticker");
            sb.AppendLine(", sum(case when CODE15 = '2' AND AUDITSEQ = '01'  then 1 else 0 end) as Total_Stick_Returned_Sticker");
            sb.AppendLine(", sum(case when CODE27 = '2' AND AUDITSEQ = '01'  then 1 else 0 end) as Total_Stick_Sub_Compl_Sticker");
            sb.AppendLine(", sum(case when CODE28 = '2' AND AUDITSEQ = '01'  then 1 else 0 end) as Total_Stick_Back_Compl_Sticker");
            sb.AppendLine(", sum(case when CODE19 = '2'  AND AUDITSEQ = '01' then 1 else 0 end) as Total_Brake_Drum_Tool");
            sb.AppendLine(", sum(case when CODE20 = '2'  AND AUDITSEQ = '01' then 1 else 0 end) as Total_Brake_Pad_Tool");
            sb.AppendLine(", sum(case when CODE21 = '2'  AND AUDITSEQ = '01' then 1 else 0 end) as Total_Ball_Joint_Tool");
            sb.AppendLine(", sum(case when CODE22 = '2'  AND AUDITSEQ = '01' then 1 else 0 end) as Total_Lift_Jack_Tool");
            sb.AppendLine(", sum(case when CODE23 = '2'  AND AUDITSEQ = '01' then 1 else 0 end) as Total_Hand_Tool");
            sb.AppendLine(", sum(case when CODE24 = '2' AND AUDITSEQ = '01'  then 1 else 0 end) as Total_Headlight_Tool");
            sb.AppendLine(", sum(case when CODE25 = '2'  AND AUDITSEQ = '01' then 1 else 0 end) as Total_Tint_Tool");
            sb.AppendLine(", sum(case when CODE26 = '2'  AND AUDITSEQ = '01' then 1 else 0 end) as Total_DecibalMeter_Tool");
            sb.AppendLine(", sum(case  when CODE17 = '2' AND AUDITSEQ = '01'  then 1 else 0 end) as Total_NHOST_Unit");
            sb.AppendLine(", sum(case when CODE18 = '2'  AND AUDITSEQ = '01' then 1 else 0 end) as Total_Manuals");
            sb.AppendLine(", sum(case when AUDITSEQ != '01' then 1 else 0 end) as fuTotal_Audits");
            sb.AppendLine(", sum(case when (CODE1 = '2' OR CODE2 = '2' or CODE3 = '2' OR CODE4 = '2') and AUDITSEQ != '01'  then 1 else 0 end) as FUTotal_Station_Cert");
            sb.AppendLine(", sum(case when CODE5 = '2' and AUDITSEQ != '01' then 1 else 0 end) as FUTotal_Mechanic_Cert");
            sb.AppendLine(", sum(case when CODE6 = '2' and AUDITSEQ != '01' then 1 else 0 end) as FUTotal_Hours_Displayed_Gen");
            sb.AppendLine(", sum(case when CODE7 = '2' and AUDITSEQ != '01' then 1 else 0 end) as FUTotal_Station_Sign_Gen");
            sb.AppendLine(", sum(case when CODE8 = '2' and AUDITSEQ != '01' then 1 else 0 end) as FUTotal_Fee_Sign_Gen");
            sb.AppendLine(", sum(case when CODE9 = '2' and AUDITSEQ != '01' then 1 else 0 end) as FUTotal_Motorcycle_Sign_Gen");
            sb.AppendLine(", sum(case when CODE10 = '2' and AUDITSEQ != '01' then 1 else 0 end) as FUTotal_Inspection_Area_Gen");
            sb.AppendLine(", sum(case when CODE11 = '2' and AUDITSEQ != '01' then 1 else 0 end) as FUTotal_Inspection_Space_Gen");
            sb.AppendLine(", sum(case when CODE12 = '2' and AUDITSEQ != '01' then 1 else 0 end) as FUTotal_Rejection_Form_Gen");
            sb.AppendLine(", sum(case when CODE13 = '2' and AUDITSEQ != '01' then 1 else 0 end) as FUTotal_Mechanic_Duty_Gen");
            sb.AppendLine(", sum(case when CODE14 = '2' and AUDITSEQ != '01' then 1 else 0 end) as FUTotal_Stick_Secured_Stick");//check to see if this is right
            sb.AppendLine(", sum(case when CODE15 = '2' and AUDITSEQ != '01' then 1 else 0 end) as FUTotal_Stick_Returned_Stick");
            sb.AppendLine(", sum(case when CODE27 = '2' and AUDITSEQ != '01' then 1 else 0 end) as FUTotal_Stick_Sub_Compl_Stick");
            sb.AppendLine(", sum(case when CODE28 = '2' and AUDITSEQ != '01' then 1 else 0 end) as FUTotal_Stick_Back_Compl_Stick");
            sb.AppendLine(", sum(case when CODE19 = '2' and AUDITSEQ != '01' then 1 else 0 end) as FUTotal_Brake_Drum_Tool");
            sb.AppendLine(", sum(case when CODE20 = '2' and AUDITSEQ != '01' then 1 else 0 end) as FUTotal_Brake_Pad_Tool");
            sb.AppendLine(", sum(case when CODE21 = '2' and AUDITSEQ != '01' then 1 else 0 end) as FUTotal_Ball_Joint_Tool");
            sb.AppendLine(", sum(case when CODE22 = '2' and AUDITSEQ != '01' then 1 else 0 end) as FUTotal_Lift_Jack_Tool");
            sb.AppendLine(", sum(case when CODE23 = '2' and AUDITSEQ != '01' then 1 else 0 end) as FUTotal_Hand_Tool");
            sb.AppendLine(", sum(case when CODE24 = '2' and AUDITSEQ != '01' then 1 else 0 end) as FUTotal_Headlight_Tool");
            sb.AppendLine(", sum(case when CODE25 = '2' and AUDITSEQ != '01' then 1 else 0 end) as FUTotal_Tint_Tool");
            sb.AppendLine(", sum(case when CODE26 = '2' and AUDITSEQ != '01' then 1 else 0 end) as FUTotal_Decibal_Tool");
            sb.AppendLine(", sum(case when CODE17 = '2' and AUDITSEQ != '01' then 1 else 0 end) AS FU_NHOST_Unit");
            sb.AppendLine(", sum(case when CODE18 = '2' and AUDITSEQ != '01' then 1 else 0 end) AS FU_Manuals");
            sb.AppendLine("FROM STATIONAUDITS");
            sb.AppendLine("WHERE (AUDITORID > 'AD00000000')");
            sb.AppendLine("AND (AUDITORID < 'AD99999999')");
            //sb.AppendLine("AND AUDITORID NOT IN ('AD0000JOSH','AD000BARRY')");
            //sb.AppendLine("AND AUDITSEQ = '01'");
            sb.AppendLine("AND \"DATE\" >= '" + dateSelector.StartDateControl.GetDateText() + "' ");
            sb.AppendLine("AND \"DATE\" <= '" + dateSelector.EndDateControl.GetDateText() + "' ");
            if (!String.IsNullOrEmpty(lstStationType.SelectedValue))
            {
                if (lstStationType.SelectedValue.Equals("M"))
                {
                    sb.AppendLine("AND REASON = '" + lstStationType.SelectedValue + "'");
                    //sb.AppendLine("OR REASON = 'I'");
                }
                else
                {
                    sb.AppendLine("AND REASON = '" + lstStationType.SelectedValue + "'");
                }
            }
            if (!String.IsNullOrEmpty(lstOfficerType.SelectedValue))
            {
                sb.AppendLine("AND SUBSTR(AUDITORID,7,10) = '" + lstOfficerType.SelectedValue + "'");
            }

            sb.AppendLine("GROUP BY AUDITORID");
            return sb.ToString();
        }

        //Description for each row
        public class StationReportRowInfoCollection : IEnumerable<StationReportRowInfo>
        {
            private List<StationReportRowInfo> info;

            /// <summary>Instantiates a new instance of the StationReportRowInfoCollection class.</summary>
            public StationReportRowInfoCollection()
            {
                info = new List<StationReportRowInfo>();
                info.Add(new StationReportRowInfo("Total Audits"));
                info.Add(new StationReportRowInfo("  Total Deficiencies"));
                info.Add(new StationReportRowInfo("    Certification"));
                info.Add(new StationReportRowInfo("    General Requirements"));
                info.Add(new StationReportRowInfo("    Sticker Requirements"));
                info.Add(new StationReportRowInfo("    Tools/Equipment"));
                info.Add(new StationReportRowInfo("    NHOST Unit"));
                info.Add(new StationReportRowInfo("    Manuals"));
                info.Add(new StationReportRowInfo("Certification Deficiencies", false));
                info.Add(new StationReportRowInfo("  Total Deficiencies"));
                info.Add(new StationReportRowInfo("    Station"));
                info.Add(new StationReportRowInfo("    Mechanics"));
                info.Add(new StationReportRowInfo("General Deficiencies", false));
                info.Add(new StationReportRowInfo("  Total Deficiencies"));
                info.Add(new StationReportRowInfo("    Hours Displayed"));
                info.Add(new StationReportRowInfo("    Station Sign"));
                info.Add(new StationReportRowInfo("    Fee Sign"));
                info.Add(new StationReportRowInfo("    Motorcycle Sign"));
                info.Add(new StationReportRowInfo("    Inspection Area"));
                info.Add(new StationReportRowInfo("    Inspection Space"));
                info.Add(new StationReportRowInfo("    Rejection Form"));
                info.Add(new StationReportRowInfo("    Mechanics On Duty"));
                info.Add(new StationReportRowInfo("Sticker Deficiencies", false));
                info.Add(new StationReportRowInfo("  Total Deficiencies"));
                info.Add(new StationReportRowInfo("    Sticker Stub Completion"));
                info.Add(new StationReportRowInfo("    Sticker Back Completion"));
                info.Add(new StationReportRowInfo("    Properly Secured"));
                info.Add(new StationReportRowInfo("    Properly Returned"));
                info.Add(new StationReportRowInfo("Tools/Equipment Deficiencies", false));
                info.Add(new StationReportRowInfo("  Total Deficiencies"));
                info.Add(new StationReportRowInfo("    Brake Drum Gauge"));
                info.Add(new StationReportRowInfo("    Brake Pad Gauge"));
                info.Add(new StationReportRowInfo("    Ball Joint Gauge"));
                info.Add(new StationReportRowInfo("    Lift Or Jack"));
                info.Add(new StationReportRowInfo("    Hand Tools"));
                info.Add(new StationReportRowInfo("    Headlight Tools"));
                info.Add(new StationReportRowInfo("    Tint Meter"));
                info.Add(new StationReportRowInfo("    Decibel Meter"));

            }
            //gets the total of each section
            public int GetDenominator(int rowIdx, int seqNum)
            {
                int val = 0;
                if (rowIdx.Between(0, 7))
                {
                    val = TotalAudits[seqNum];
                }
                else if (rowIdx.Between(8, 11))
                {
                    val = TotalCertifications[seqNum];
                }
                else if (rowIdx.Between(12, 21))
                {
                    val = TotalGeneralRequirements[seqNum];
                }
                else if (rowIdx.Between(22, 27))
                {
                    val = TotalStickerRequirements[seqNum];
                }
                else if (rowIdx.Between(28, 37))
                {
                    val = TotalToolsEquipment[seqNum];
                }
                return val;
            }
            //Total Audits
            public StationReportRowInfo TotalAudits { get { return info[0]; } }
            public StationReportRowInfo TotalDeficiencies { get { return info[1]; } }
            public StationReportRowInfo TotalCertifications { get { return info[2]; } }
            public StationReportRowInfo TotalGeneralRequirements { get { return info[3]; } }
            public StationReportRowInfo TotalStickerRequirements { get { return info[4]; } }
            public StationReportRowInfo TotalToolsEquipment { get { return info[5]; } }
            public StationReportRowInfo TotalNHOSTUnit { get { return info[6]; } }
            public StationReportRowInfo Manuals { get { return info[7]; } }

            //Certification Deficiencies
            public StationReportRowInfo TotalCert { get { return info[8]; } }
            public StationReportRowInfo TotalCertificationDeficiencies { get { return info[9]; } }
            public StationReportRowInfo TotalCertificationStation { get { return info[10]; } }
            public StationReportRowInfo TotalCertificationMechanics { get { return info[11]; } }

            //General Deficiencies
            public StationReportRowInfo TotalGenDef { get { return info[12]; } }
            public StationReportRowInfo TotalGeneralDeficiencies { get { return info[13]; } }
            public StationReportRowInfo TotalGeneralHoursDisplayed { get { return info[14]; } }
            public StationReportRowInfo TotalGeneralStationSign { get { return info[15]; } }
            public StationReportRowInfo TotalGeneralFeeSign { get { return info[16]; } }
            public StationReportRowInfo TotalGeneralMotorcycleSign { get { return info[17]; } }
            public StationReportRowInfo TotalGeneralInspectionArea { get { return info[18]; } }
            public StationReportRowInfo TotalGeneralInspectionSpace { get { return info[19]; } }
            public StationReportRowInfo TotalGeneralRejectionForm { get { return info[20]; } }
            public StationReportRowInfo TotalGeneralMechanicsOnDuty { get { return info[21]; } }

            //Sticker Deficiencies
            public StationReportRowInfo TotalStickers { get { return info[22]; } }
            public StationReportRowInfo TotalStickerDeficency { get { return info[23]; } }
            public StationReportRowInfo TotalStickerStubCompletion { get { return info[24]; } }
            public StationReportRowInfo TotalStickerBackCompletion { get { return info[25]; } }
            public StationReportRowInfo TotalStickerProperlySecured { get { return info[26]; } }
            public StationReportRowInfo TotalStickerProperlyReturned { get { return info[27]; } }

            //Tools/Equipment Deficiencies
            public StationReportRowInfo TotalToolEquip { get { return info[28]; } }
            public StationReportRowInfo TotalToolsEquipmentDeficiencies { get { return info[29]; } }
            public StationReportRowInfo TotalToolsEquipmentBrakeDrumGauge { get { return info[30]; } }
            public StationReportRowInfo TotalToolsEquipmentBrakePadGauge { get { return info[31]; } }
            public StationReportRowInfo TotalToolsEquipmentBallJointGauge { get { return info[32]; } }
            public StationReportRowInfo TotalToolsEquipmentLiftOrJack { get { return info[33]; } }
            public StationReportRowInfo TotalToolsEquipmentHandTools { get { return info[34]; } }
            public StationReportRowInfo TotalToolsEquipmentHeadlightTools { get { return info[35]; } }
            public StationReportRowInfo TotalToolsEquipmentTintMeter { get { return info[36]; } }
            public StationReportRowInfo TotalToolsEquipmentDecibelMeter { get { return info[37]; } }

            /// <summary>Gets the enumerator of the list of StationReportRowInfo objects.</summary>
            /// <returns>List enumerator.</returns>
            public IEnumerator<StationReportRowInfo> GetEnumerator()
            {
                return info.GetEnumerator();
            }

            /// <summary>Gets the enumerator of the list of StationReportRowInfo objects.</summary>
            /// <returns>List enumerator.</returns>
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }

        /// <summary>Stores information about a row in the station report.</summary>
        public class StationReportRowInfo
        {
            /// <summary>Instantiates a new instance of the StationReportRowInfo class.</summary>
            /// <param name="desc">Description of the row.</param>
            /// <param name="hasData">Whether or not the row contains values.  Defaults to true.</param>
            public StationReportRowInfo(string desc, bool hasData = true)
            {
                Description = desc;
                ContainsData = hasData;

                TotalAudits = 0;
                CertificationDeficiencies = 0;
                GeneralDeficiencies = 0;
                StickerDeficiencies = 0;
                ToolsEquipmentDeficiencies = 0;
            }

            public int this[int idx]
            {
                set
                {
                    switch (idx)
                    {
                        case 0:
                            TotalAudits = value;
                            break;
                        case 1:
                            CertificationDeficiencies = value;
                            break;
                        case 2:
                            GeneralDeficiencies = value;
                            break;
                        case 3:
                            StickerDeficiencies = value;
                            break;
                        case 4:
                            ToolsEquipmentDeficiencies = value;
                            break;
                    }
                }
                get
                {
                    int val = 0;
                    switch (idx)
                    {
                        case 0:
                            val = TotalAudits;
                            break;
                        case 1:
                            val = CertificationDeficiencies;
                            break;
                        case 2:
                            val = GeneralDeficiencies;
                            break;
                        case 3:
                            val = StickerDeficiencies;
                            break;
                        case 4:
                            val = ToolsEquipmentDeficiencies;
                            break;
                    }
                    return val;
                }
            }

            public string Description { get; private set; }

            /// <summary>Gets whether or not the row contains any data.</summary>
            public bool ContainsData { get; private set; }

            public int TotalAudits { get; set; }
            public int CertificationDeficiencies { get; set; }
            public int GeneralDeficiencies { get; set; }
            public int StickerDeficiencies { get; set; }
            public int ToolsEquipmentDeficiencies { get; set; }
        }
    }
}