using GDCoreUtilities;
using PortalFramework.ReportModel;
using System;
using System.Data;
using System.Text;

namespace NHPortal.Classes.Reports.InspectionReports
{
    public class MechanicRejectionData : IReportData
    {
        public MechanicRejectionData()
        {
            BaseReport = BaseReportMaster.MechanicRejection;
        }

        public BaseReport BaseReport { get; set; }
        public string MechanicID { get; set; }
        public int OverallPass { get; set; }
        public int OverallReject { get; set; }
        public int OverallCorrected { get; set; }
        public int OverallAdmin { get; set; }
        public int OverallAbort { get; set; }
        public int OBDAbort { get; set; }
        public int OBDPass { get; set; }
        public int OBDReject { get; set; }
        public int OBDNoComm { get; set; }
        public int OBDUntested { get; set; }
        public int VisualAbort { get; set; }
        public int VisualPass { get; set; }
        public int VisualReject { get; set; }
        public int VisualCorrected { get; set; }
        public int VisualUntested { get; set; }
        public int SafetyAbort { get; set; }
        public int SafetyPass { get; set; }
        public int SafetyReject { get; set; }
        public int SafetyCorrected { get; set; }
        public int SafetyUntested { get; set; }

        public int TotalOverallPass { get; set; }
        public int TotalOverallReject { get; set; }
        public int TotalOverallCorrected { get; set; }
        public int TotalOverallAdmin { get; set; }
        public int TotalOverallAbort { get; set; }
        public int TotalOBDAbort { get; set; }
        public int TotalOBDPass { get; set; }
        public int TotalOBDReject { get; set; }
        public int TotalOBDNoComm { get; set; }
        public int TotalOBDUntested { get; set; }
        public int TotalVisualAbort { get; set; }
        public int TotalVisualPass { get; set; }
        public int TotalVisualReject { get; set; }
        public int TotalVisualCorrected { get; set; }
        public int TotalVisualUntested { get; set; }
        public int TotalSafetyAbort { get; set; }
        public int TotalSafetyPass { get; set; }
        public int TotalSafetyReject { get; set; }
        public int TotalSafetyCorrected { get; set; }
        public int TotalSafetyUntested { get; set; }

        public int TestCount
        {
            get { return OverallPass + OverallReject + OverallCorrected; }
        }

        public int SafetyCount
        {
            get { return SafetyPass + SafetyCorrected + SafetyReject + SafetyUntested; }
        }

        public int OBDCount
        {
            get { return OBDPass + OBDReject + OBDUntested; }
        }

        public int VisualCount
        {
            get { return VisualPass + VisualCorrected + VisualReject + VisualUntested; }
        }

        public int TotalTestCount
        {
            get { return TotalOverallPass + TotalOverallReject + TotalOverallCorrected; }
        }

        public int TotalSafetyCount
        {
            get { return TotalSafetyPass + TotalSafetyCorrected + TotalSafetyReject + TotalSafetyUntested; }
        }

        public int TotalOBDCount
        {
            get { return TotalOBDPass + TotalOBDReject + TotalOBDUntested; }
        }

        public int TotalVisualCount
        {
            get { return TotalVisualPass + TotalVisualCorrected + TotalVisualReject + TotalVisualUntested; }
        }

        public void SetPropertyValues(DataRow dRow)
        {
            int i = 0;

            MechanicID = NullSafe.ToString(dRow[i++]);
            OverallPass = NullSafe.ToInt(dRow[i++]);
            OverallReject = NullSafe.ToInt(dRow[i++]);
            OverallCorrected = NullSafe.ToInt(dRow[i++]);
            OverallAdmin = NullSafe.ToInt(dRow[i++]);
            OverallAbort = NullSafe.ToInt(dRow[i++]);
            OBDAbort = NullSafe.ToInt(dRow[i++]);
            OBDPass = NullSafe.ToInt(dRow[i++]);
            OBDReject = NullSafe.ToInt(dRow[i++]);
            OBDNoComm = NullSafe.ToInt(dRow[i++]);
            OBDUntested = NullSafe.ToInt(dRow[i++]);
            VisualAbort = NullSafe.ToInt(dRow[i++]);
            VisualPass = NullSafe.ToInt(dRow[i++]);
            VisualReject = NullSafe.ToInt(dRow[i++]);
            VisualCorrected = NullSafe.ToInt(dRow[i++]);
            VisualUntested = NullSafe.ToInt(dRow[i++]);
            SafetyAbort = NullSafe.ToInt(dRow[i++]);
            SafetyPass = NullSafe.ToInt(dRow[i++]);
            SafetyReject = NullSafe.ToInt(dRow[i++]);
            SafetyCorrected = NullSafe.ToInt(dRow[i++]);
            SafetyUntested = NullSafe.ToInt(dRow[i]);

            AddToTotals();
        }

        public void AddToTotals()
        {
            TotalOverallPass += OverallPass;
            TotalOverallReject += OverallReject;
            TotalOverallCorrected += OverallCorrected;
            TotalOverallAdmin += OverallAdmin;
            TotalOverallAbort += OverallAbort;
            TotalOBDAbort += OBDAbort;
            TotalOBDPass += OBDPass;
            TotalOBDReject += OBDReject;
            TotalOBDNoComm += OBDNoComm;
            TotalOBDUntested += OBDUntested;
            TotalVisualAbort += VisualAbort;
            TotalVisualPass += VisualPass;
            TotalVisualReject += VisualReject;
            TotalVisualCorrected += VisualCorrected;
            TotalVisualUntested += VisualUntested;
            TotalSafetyAbort += SafetyAbort;
            TotalSafetyPass += SafetyPass;
            TotalSafetyReject += SafetyReject;
            TotalSafetyCorrected += SafetyCorrected;
            TotalSafetyUntested += SafetyUntested;
        }

        public void AddDataToReportRow(ReportRow currentRow)
        {
            // Station ID
            currentRow.Cells.Add(MechanicID);
            // Test Count
            currentRow.Cells.Add(TestCount);
            // Reject Count
            currentRow.Cells.Add(OverallReject);
            // % Reject
            currentRow.Cells.Add(BaseReportMaster.CalculatePercentString(OverallReject, TestCount));
            // Safety Count
            currentRow.Cells.Add(SafetyCount);
            // Safety Reject
            currentRow.Cells.Add(SafetyReject);
            // % Safety Reject
            currentRow.Cells.Add(BaseReportMaster.CalculatePercentString(SafetyReject, TestCount));
            // OBD Count
            currentRow.Cells.Add(OBDCount);
            // OBD Reject
            currentRow.Cells.Add(OBDReject);
            // % OBD Reject
            currentRow.Cells.Add(BaseReportMaster.CalculatePercentString(OBDReject, TestCount));
            // OBD NoComm
            currentRow.Cells.Add(OBDNoComm);
            // % OBD NoComm
            currentRow.Cells.Add(BaseReportMaster.CalculatePercentString(OBDNoComm, TestCount));
            // Visual Count
            currentRow.Cells.Add(VisualCount);
            // Visual Reject
            currentRow.Cells.Add(VisualReject);
            // % Visual Reject
            currentRow.Cells.Add(BaseReportMaster.CalculatePercentString(VisualReject, TestCount));
            // Abort Count
            currentRow.Cells.Add(OverallAbort);
            // % Abort Count
            currentRow.Cells.Add(BaseReportMaster.CalculatePercentString(OverallAbort, TestCount));
        }

        public void SetTotalRowValues(ReportRow totalRow)
        {
            // Station ID
            totalRow.Cells.Add("ALL");
            // Test Count
            totalRow.Cells.Add(TotalTestCount);
            // Reject Count
            totalRow.Cells.Add(TotalOverallReject);
            // % Reject
            totalRow.Cells.Add(BaseReportMaster.CalculatePercentString(TotalOverallReject, TotalTestCount));
            // Safety Count
            totalRow.Cells.Add(TotalSafetyCount);
            // Safety Reject
            totalRow.Cells.Add(TotalSafetyReject);
            // % Safety Reject
            totalRow.Cells.Add(BaseReportMaster.CalculatePercentString(TotalSafetyReject, TotalTestCount));
            // OBD Count
            totalRow.Cells.Add(TotalOBDCount);
            // OBD Reject
            totalRow.Cells.Add(TotalOBDReject);
            // % OBD Reject
            totalRow.Cells.Add(BaseReportMaster.CalculatePercentString(TotalOBDReject, TotalTestCount));
            // OBD NoComm
            totalRow.Cells.Add(TotalOBDNoComm);
            // % OBD NoComm
            totalRow.Cells.Add(BaseReportMaster.CalculatePercentString(TotalOBDNoComm, TotalTestCount));
            // Visual Count
            totalRow.Cells.Add(TotalVisualCount);
            // Visual Reject
            totalRow.Cells.Add(TotalVisualReject);
            // % Visual Reject
            totalRow.Cells.Add(BaseReportMaster.CalculatePercentString(TotalVisualReject, TotalTestCount));
            // Abort Count
            totalRow.Cells.Add(TotalOverallAbort);
            // % Abort Count
            totalRow.Cells.Add(BaseReportMaster.CalculatePercentString(TotalOverallAbort, TotalTestCount));
        }

        public string BuildSQL()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine(" SELECT DISTINCT INSPECTORID,");
            builder.Append(String.Format(BaseReportMaster.QUERY_REPORT, "INSPECTORID"));
            builder.Append("{0}");
            builder.AppendLine("ORDER BY DECODE((OVERALL_PASS + OVERALL_REJECT + OVERALL_CORRECTED),0,0,OVERALL_REJECT/(OVERALL_PASS + OVERALL_REJECT + OVERALL_CORRECTED)) DESC, INSPECTORID ");
            return builder.ToString();
        }
    }
}