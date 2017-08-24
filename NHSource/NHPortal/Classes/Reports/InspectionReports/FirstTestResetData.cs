using GDCoreUtilities;
using PortalFramework.ReportModel;
using System;
using System.Text;

namespace NHPortal.Classes.Reports.InspectionReports
{
    public class FirstTestResetData : IReportData
    {
        public FirstTestResetData()
        {
            BaseReport = BaseReportMaster.FirstTestReset;
        }

        public BaseReport BaseReport { get; set; }
        public string VIN { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string User { get; set; }
        public string Reason { get; set; }

        public void SetPropertyValues(System.Data.DataRow dRow)
        {
            int i = 0;

            VIN = NullSafe.ToString(dRow[i++]);
            Date = NullSafe.ToString(dRow[i++]);
            Time = NullSafe.ToString(dRow[i++]);
            User = NullSafe.ToString(dRow[i++]);
            Reason = NullSafe.ToString(dRow[i++]);
        }

        public void AddDataToReportRow(ReportRow currentRow)
        {
            // VIN
            currentRow.Cells.Add(VIN);
            // Date
            if (Date.Length == 8)
            {
                currentRow.Cells.Add(Date.Substring(4, 2) + "/" + Date.Substring(6, 2) + "/" + Date.Substring(0, 4));
            }
            else
            {
                currentRow.Cells.Add(String.Empty);
            }
            // Time
            if (Time.Length == 6)
            {
                currentRow.Cells.Add(Time.Substring(0, 2) + ":" + Time.Substring(2, 2) + ":" + Time.Substring(4, 2));
            }
            else
            {
                currentRow.Cells.Add(String.Empty);
            }
            // User
            currentRow.Cells.Add(User);
            // Reason
            currentRow.Cells.Add(Reason);
        }

        public void SetTotalRowValues(ReportRow totalRow)
        {

        }

        public string BuildSQL()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine(" SELECT VIN, RESETDATE, RESETTIME, EMPID, ");
            builder.AppendLine(" CASE WHEN RESETYPE = 0 THEN 'GVW' WHEN RESETYPE = 1 THEN '60-DAY' WHEN RESETYPE = 2 THEN 'OTHER' END AS REASON ");
            builder.AppendLine(" FROM FIRSTTESTRESET ");
            builder.Append(" {0} ");
            builder.AppendLine(" ORDER BY RESETDATE ");
            return builder.ToString();
        }
    }
}