using GDCoreUtilities;
using PortalFramework.ReportModel;
using System.Data;
using System.Text;

namespace NHPortal.Classes.Reports.InspectionReports
{
    public class OBDResultsData : IReportData
    {
        public OBDResultsData()
        {
            BaseReport = BaseReportMaster.OBDResults;
        }

        public BaseReport BaseReport { get; set; }

        // Database Values
        public string StationID { get; set; }
        public int InitStep { get; set; }
        public int VoltageStep { get; set; }
        public int TestStep { get; set; }
        public int ExtraInfoStep { get; set; }
        public int MILOk { get; set; }
        public int NoCom { get; set; }
        public int Unready { get; set; }
        public int MILOn { get; set; }
        public int ReadyExempt { get; set; }
        public int ComExempt { get; set; }

        // Running Totals
        public int TotalInitStep { get; set; }
        public int TotalVoltageStep { get; set; }
        public int TotalTestStep { get; set; }
        public int TotalExtraInfoStep { get; set; }
        public int TotalMILOk { get; set; }
        public int TotalNoCom { get; set; }
        public int TotalUnready { get; set; }
        public int TotalMILOn { get; set; }
        public int TotalReadyExempt { get; set; }
        public int TotalComExempt { get; set; }

        public int Total
        {
            get { return InitStep + VoltageStep + TestStep + ExtraInfoStep; }
        }

        public int GrandTotal
        {
            get { return TotalInitStep + TotalVoltageStep + TotalTestStep + TotalExtraInfoStep; }
        }

        public void SetPropertyValues(DataRow dRow)
        {
            int i = 0;

            StationID = NullSafe.ToString(dRow[i++]);
            InitStep = NullSafe.ToInt(dRow[i++]);
            VoltageStep = NullSafe.ToInt(dRow[i++]);
            TestStep = NullSafe.ToInt(dRow[i++]);
            ExtraInfoStep = NullSafe.ToInt(dRow[i++]);
            MILOk = NullSafe.ToInt(dRow[i++]);
            NoCom = NullSafe.ToInt(dRow[i++]);
            Unready = NullSafe.ToInt(dRow[i++]);
            MILOn = NullSafe.ToInt(dRow[i++]);
            ReadyExempt = NullSafe.ToInt(dRow[i++]);

            AddToTotals();
        }

        public void AddToTotals()
        {
            TotalInitStep += InitStep;
            TotalVoltageStep += VoltageStep;
            TotalTestStep += TestStep;
            TotalExtraInfoStep += ExtraInfoStep;
            TotalMILOk += MILOk;
            TotalNoCom += NoCom;
            TotalUnready += Unready;
            TotalMILOn += MILOn;
            TotalComExempt += ComExempt;
        }

        public void AddDataToReportRow(ReportRow currentRow)
        {
            // Station ID
            currentRow.Cells.Add(StationID);
            // Total
            currentRow.Cells.Add(Total);
            // Init Step
            currentRow.Cells.Add(InitStep);
            // % Init
            currentRow.Cells.Add(BaseReportMaster.CalculatePercentString(InitStep, Total));
            // Voltage Step
            currentRow.Cells.Add(VoltageStep);
            // % Voltage
            currentRow.Cells.Add(BaseReportMaster.CalculatePercentString(VoltageStep, Total));
            // Test Step
            currentRow.Cells.Add(TestStep);
            // % Test
            currentRow.Cells.Add(BaseReportMaster.CalculatePercentString(TestStep, Total));
            // Extra Info Step
            currentRow.Cells.Add(ExtraInfoStep);
            // % Extra Info
            currentRow.Cells.Add(BaseReportMaster.CalculatePercentString(ExtraInfoStep, Total));
            // MIL OK
            currentRow.Cells.Add(MILOk);
            // % MIL OK
            currentRow.Cells.Add(BaseReportMaster.CalculatePercentString(MILOk, Total));
            // No Com
            currentRow.Cells.Add(NoCom);
            // % No Com
            currentRow.Cells.Add(BaseReportMaster.CalculatePercentString(NoCom, Total));
            // Unready
            currentRow.Cells.Add(Unready);
            // % Unready
            currentRow.Cells.Add(BaseReportMaster.CalculatePercentString(Unready, Total));
            // MIL On
            currentRow.Cells.Add(MILOn);
            // % MIL On
            currentRow.Cells.Add(BaseReportMaster.CalculatePercentString(MILOn, Total));
            // Ready Exempt
            currentRow.Cells.Add(ReadyExempt);
            // % Ready Exempt
            currentRow.Cells.Add(BaseReportMaster.CalculatePercentString(ReadyExempt, Total));
            // Com Exempt
            currentRow.Cells.Add(ComExempt);
            // % Com Exempt
            currentRow.Cells.Add(BaseReportMaster.CalculatePercentString(ComExempt, Total));
        }

        public void SetTotalRowValues(ReportRow totalRow)
        {
            // Station ID
            totalRow.Cells.Add("ALL");
            // Total
            totalRow.Cells.Add(GrandTotal);
            // Init Step
            totalRow.Cells.Add(TotalInitStep);
            // % Init
            totalRow.Cells.Add(BaseReportMaster.CalculatePercentString(TotalInitStep, GrandTotal));
            // Voltage Step
            totalRow.Cells.Add(TotalVoltageStep);
            // % Voltage
            totalRow.Cells.Add(BaseReportMaster.CalculatePercentString(TotalVoltageStep, GrandTotal));
            // Test Step
            totalRow.Cells.Add(TotalTestStep);
            // % Test
            totalRow.Cells.Add(BaseReportMaster.CalculatePercentString(TotalTestStep, GrandTotal));
            // Extra Info Step
            totalRow.Cells.Add(TotalExtraInfoStep);
            // % Extra Info
            totalRow.Cells.Add(BaseReportMaster.CalculatePercentString(TotalExtraInfoStep, GrandTotal));
            // MIL OK
            totalRow.Cells.Add(TotalMILOk);
            // % MIL OK
            totalRow.Cells.Add(BaseReportMaster.CalculatePercentString(TotalMILOk, GrandTotal));
            // No Com
            totalRow.Cells.Add(TotalNoCom);
            // % No Com
            totalRow.Cells.Add(BaseReportMaster.CalculatePercentString(TotalNoCom, GrandTotal));
            // Unready
            totalRow.Cells.Add(TotalUnready);
            // % Unready
            totalRow.Cells.Add(BaseReportMaster.CalculatePercentString(TotalUnready, GrandTotal));
            // MIL On
            totalRow.Cells.Add(TotalMILOn);
            // % MIL On
            totalRow.Cells.Add(BaseReportMaster.CalculatePercentString(TotalMILOn, GrandTotal));
            // Ready Exempt
            totalRow.Cells.Add(TotalReadyExempt);
            // % Ready Exempt
            totalRow.Cells.Add(BaseReportMaster.CalculatePercentString(TotalReadyExempt, GrandTotal));
            // Com Exempt
            totalRow.Cells.Add(TotalComExempt);
            // % Com Exempt
            totalRow.Cells.Add(BaseReportMaster.CalculatePercentString(TotalComExempt, GrandTotal));
        }

        public string BuildSQL()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append(BaseReportMaster.QUERY_OBD);
            // Foramt placeholder for Where clause.
            builder.Append("{0}");
            builder.AppendLine(" AND OVERALLPF<>'6' AND ((OBDPF='1') OR (OBDPF='2')) ORDER BY STATIONID ");
            return builder.ToString();
        }
    }
}