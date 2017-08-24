using NHPortal.Classes.Charts;
using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace NHPortal.Classes.Reports.Triggers
{
    public class TimeBeforeTests : BaseTrigger
    {
        public TimeBeforeTests()
        {
            RunInitialReport = false;
            DetailedReportProcName = "NH_TRIG_TBT_DATA";
            DrillProcName = "NH_TRIG_TBT_CHART";
            ChartProcName = DrillProcName;
            this.BaseReport = BaseReportMaster.TimeBeforeTest;
        }

        public override void SetMetaData()
        {
            base.SetMetaData();

            MetaList.Add(new ChartMetaData("Threshold", ThresholdMins));
        }

        public override OracleParameter[] GetOracleParams(bool isReport)
        {
            List<OracleParameter> paramList = new List<OracleParameter>();

            string decile = ChartParams.SelectedPoint == null ? "" : ChartParams.SelectedPoint;
            paramList.Add(new OracleParameter("StartDate", OracleDbType.Varchar2, 8, ChartParams.StartDateText, ParameterDirection.Input));
            paramList.Add(new OracleParameter("EndDate", OracleDbType.Varchar2, 8, ChartParams.EndDateText, ParameterDirection.Input));
            paramList.Add(new OracleParameter("SIDSel", OracleDbType.Varchar2, 25, ChartParams.DrillDown.DrillLevels[0].DrillValue, ParameterDirection.Input));
            paramList.Add(new OracleParameter("TableType", OracleDbType.Varchar2, 3, ChartParams.TableType, ParameterDirection.Input));
            paramList.Add(new OracleParameter("DASel", OracleDbType.Varchar2, 12, decile, ParameterDirection.Input));
            paramList.Add(new OracleParameter("threshMins", OracleDbType.Varchar2, 12, ThresholdMins, ParameterDirection.Input));
            paramList.Add(new OracleParameter("RECORD_DATA", OracleDbType.RefCursor, ParameterDirection.Output));

            return paramList.ToArray();
        }

        public override TableHeaderRow GetDrillTableHeader(int drillLevel)
        {
            TableHeaderRow headerRow = new TableHeaderRow();
            TableHeaderCell headerCell;

            headerRow.TableSection = TableRowSection.TableHeader;

            headerCell = new TableHeaderCell();

            headerCell.Text = ChartParams.DrillDown.DrillLevels[drillLevel + 1].DrillType.ToString();

            headerRow.Cells.Add(headerCell);

             headerCell = new TableHeaderCell();
             headerCell.Text = "Test Count";
             headerRow.Cells.Add(headerCell);

             headerCell = new TableHeaderCell();
             headerCell.Text = "Threshold Count";
             headerRow.Cells.Add(headerCell);

             headerCell = new TableHeaderCell();
             headerCell.Text = "Threshold %";
             headerRow.Cells.Add(headerCell);

            return headerRow;
        }
    }
}