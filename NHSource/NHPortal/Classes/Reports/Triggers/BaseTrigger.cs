using GD.Highcharts.Enums;
using GD.Highcharts.GDAnalytics;
using GD.Highcharts.Helpers;
using GD.Highcharts.Options;
using GDCoreUtilities;
using GDCoreUtilities.Logging;
using NHPortal.Classes.Charts;
using Oracle.DataAccess.Client;
using PortalFramework.ReportModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web.UI.WebControls;

namespace NHPortal.Classes.Reports.Triggers
{
    public class BaseTrigger : BaseChart
    {
        public override void LoadDetailedReport()
        {
            //base.LoadDetailedReport();
            DataTable dt;
            Report report = new Report(this.BaseReport.ReportTitle + "  IQ");
            ReportRow headerRow = new ReportRow(report);
            ReportCell currentCell;
            ReportRow currentRow;

            report.Sortable = true;
            report.ID = "tblChart";
            report.FooterNote = BaseReport.FooterNote;

            AddDetailedHeaders(report, TriggerDetailedReportHeader.MainColumns);

            if (this.BaseReport == BaseReportMaster.StationIpectorTrigger)
            {
                if (ChartParams.SelectedPoint == "Time Before Test Trigger (3 Minute Threshold)")
                {
                    ThresholdMins = "3";
                }
                else
                {
                    ThresholdMins = null;
                }

            }

            if (IncludeVehicleInfo)
            {
                AddDetailedHeaders(report, TriggerDetailedReportHeader.VehicleInfoColumns);
            }

            if (IncludeInspectionInfo)
            {
                AddDetailedHeaders(report, TriggerDetailedReportHeader.InspectionInfoColumns);
            }

            if (IncludeVisualInfo)
            {
                AddDetailedHeaders(report, TriggerDetailedReportHeader.VisualInfoColumns);
            }

            if (IncludeOBDInfo)
            {
                AddDetailedHeaders(report, TriggerDetailedReportHeader.OBDInfoColumns);
            }

            if (IncludeSafetyInfo)
            {
                AddDetailedHeaders(report, TriggerDetailedReportHeader.SafetyInfoColumns);
            }

            dt = BaseReportMaster.GetProcedureDataTable(DetailedReportProcName, GetDetailedOracleParams());

            try
            {
                foreach (DataRow dRow in dt.Rows)
                {
                    currentRow = new ReportRow(report);

                    for (int i = 0; i < report.ColumnCount; i++)
                    {
                        currentCell = new ReportCell();
                        currentCell.Value = dRow[report.Columns[i].Name].ToString();
                        currentRow.Cells.Add(currentCell);
                    }

                    AddRowColorCoding(currentRow, NullSafe.ToString(dRow["CHECKVIN"]));

                    report.Rows.Add(currentRow);
                }
            }
            catch (Exception ex)
            {
                ILogger Logger = SessionHelper.GetSessionLogger(System.Web.HttpContext.Current.Session);
                Logger.Log("There was a problem loading the detailed report from the base trigger class." + Environment.NewLine + ex.Message, LogSeverity.Error);
            }

            this.Report = report;
        }

        /// <summary>Adds the base set of headersfor a detailed report.</summary>
        protected void AddDetailedHeaders(Report rpt, Dictionary<string, string> columns)
        {
            foreach (KeyValuePair<string, string> kvp in columns)
            {
                rpt.Columns.Add(kvp.Key, kvp.Value);
            }
        }

        /// <summary>Color codes the background of report cells if certain conditions are met.</summary>
        protected void AddRowColorCoding(ReportRow currentRow, string checkVin)
        {
            string eReadiness = currentRow["EREADINESS"].Value.Trim();
            string tReadiness = currentRow["TREADINESS"].Value.Trim();
            string eProtocol = currentRow["EPROTOCOL"].Value.Trim();
            string tProtocol = currentRow["TPROTOCOL"].Value.Trim();

            if (EvaluateVIN(currentRow["VIN"].Value.Trim(), currentRow["OBD_VIN"].Value.Trim(), checkVin))
            {
                currentRow["VIN"].AddClass("red-cell");
                currentRow["OBD_VIN"].AddClass("blue-cell");
            }

            if (EvaluateProtocol(tReadiness, eReadiness, tProtocol, eProtocol))
            {
                currentRow["TPROTOCOL"].AddClass("red-cell");
                currentRow["EPROTOCOL"].AddClass("blue-cell");
            }

            if (EvaluateReadiness(tReadiness, eReadiness, tProtocol, eProtocol))
            {
                currentRow["TREADINESS"].AddClass("red-cell");
                currentRow["EREADINESS"].AddClass("blue-cell");
            }

            if (ThresholdMins != null)
            {
                if (EvaluateThreshold(currentRow["ELAPSED_MIN"].Value.Trim(), ThresholdMins))
                {
                    currentRow["ELAPSED_MIN"].AddClass("red-cell");
                }
            }
        }

        private bool EvaluateVIN(string vin, string eVin, string checkVin)
        {
            bool addColorCoding = false;

            if (vin != eVin && vin != String.Empty && eVin != String.Empty && checkVin == "1")
            {
                addColorCoding = true;
            }

            return addColorCoding;
        }

        private bool EvaluateReadiness(string tReadiness, string eReadiness, string tProtocol, string eProtocol)
        {
            bool addColorCoding = false;

            if (tReadiness.Length < 1 || eReadiness.Length < 1 || tProtocol.Length != 2 || eProtocol.Length != 2) return addColorCoding;

            string firstTReadiness = tReadiness.Substring(0, 1);
            string firstEReadiness = eReadiness.Substring(0, 1);

            if
                (

                   (
                           (tReadiness != eReadiness)
                        && (tProtocol != String.Empty)
                        && (eProtocol != String.Empty)
                        && (String.Compare(tProtocol, "00") >= 0)
                        && (String.Compare(tProtocol, "FF") <= 0)
                        && (String.Compare(eProtocol, "00") >= 0)
                        && (String.Compare(eProtocol, "FF") <= 0)
                    )
                    &&
                    (
                        (firstTReadiness == "S")
                        || (firstTReadiness == "U")
                    )
                    &&
                    (
                        (firstEReadiness == "S") || (firstEReadiness == "U")
                    )
                    && !
                    (
                        (
                               (eProtocol == "03")
                            && (tProtocol == "08")
                        )
                            &&
                            (
                                (tReadiness == eReadiness)
                              || !
                                  (
                                      (
                                           (firstTReadiness == "S")
                                        || (firstTReadiness == "U")
                                      )
                                      &&
                                     (
                                          (firstEReadiness == "S")
                                       || (firstEReadiness == "U")
                                      )
                                  )
                            )
                            ||
                            (
                                   (eProtocol == "04")
                                && (tProtocol == "07")
                            )
                            &&
                            (
                                (tReadiness == eReadiness)
                                || !
                                (
                                    (
                                        (firstTReadiness == "S")
                                        || (firstTReadiness == "U")
                                    )
                                    &&
                                    (
                                        (firstEReadiness == "S")
                                        || (firstEReadiness == "U")
                                    )
                                )
                            )
                            ||
                            (
                                (eProtocol == "04")
                                && (tProtocol == "06")
                            )
                            &&
                            (
                                (tReadiness == eReadiness)
                                || !
                                (
                                    (
                                          (firstTReadiness == "S")
                                       || (firstTReadiness == "U")
                                    )
                                    &&
                                    (
                                           (firstEReadiness == "S")
                                        || (firstEReadiness == "U")
                                    )
                                )
                             )
                             ||
                             (
                                   (eProtocol == "05")
                                && (tProtocol == "09")
                              )
                            &&
                            (
                                (tReadiness == eReadiness)
                                || !
                                (
                                    (
                                          (firstTReadiness == "S")
                                       || (firstTReadiness == "U")
                                    )
                                    &&
                                    (
                                          (firstEReadiness == "S")
                                       || (firstEReadiness == "U")
                                    )
                                )
                            )
                            ||
                            (
                                  (eProtocol == "05")
                               && (tProtocol == "0B")
                            )
                            &&
                            (
                                (tReadiness == eReadiness)
                                || !
                                (
                                    (
                                          (firstTReadiness == "S")
                                       || (firstTReadiness == "U")
                                    )
                                    &&
                                    (
                                          (firstEReadiness == "S")
                                       || (firstEReadiness == "U")
                                    )
                                )
                            )
                            ||
                            (
                                  (eProtocol == "0A")
                               && (tProtocol == "0B")
                            )
                            &&
                            (
                                (tReadiness == eReadiness)
                                || !
                                (
                                    (
                                          (firstTReadiness == "S")
                                       || (firstTReadiness == "U")
                                    )
                                    &&
                                    (
                                          (firstEReadiness == "S")
                                       || (firstEReadiness == "U")
                                    )
                                )
                            )
                    )
                )
            {
                addColorCoding = true;
            }

            return addColorCoding;
        }

        private bool EvaluateProtocol(string tReadiness, string eReadiness, string tProtocol, string eProtocol)
        {
            bool addColorCoding = false;

            if (tReadiness.Length < 1 || eReadiness.Length < 1 || tProtocol.Length != 2 || eProtocol.Length != 2) return addColorCoding;

            string firstTReadiness = tReadiness.Substring(0, 1);
            string firstEReadiness = eReadiness.Substring(0, 1);

            if
                (
                    (
                           (tProtocol != eProtocol)
                        && (tProtocol != String.Empty)
                        && (eProtocol != String.Empty)
                        && (String.Compare(tProtocol, "00") >= 0)
                        && (String.Compare(tProtocol, "FF") <= 0)
                        && (String.Compare(eProtocol, "00") >= 0)
                        && (String.Compare(eProtocol, "FF") <= 0)
                    )
                    && !
                    (
                        (
                              (eProtocol == "03")
                           && (tProtocol == "08")
                        )
                        &&
                        (
                            (tReadiness == eReadiness)
                            || !
                            (
                                (
                                     (firstTReadiness == "S")
                                  || (firstTReadiness == "U")
                                )
                                &&
                                (
                                     (firstEReadiness == "S")
                                  || (firstEReadiness == "U")
                                )
                            )
                        )
                        ||
                        (
                              (eProtocol == "04")
                           && (tProtocol == "07")
                        )
                        &&
                        (
                            (tReadiness == eReadiness)
                            || !
                            (
                                (
                                      (firstTReadiness == "S")
                                   || (firstTReadiness == "U")
                                )
                                &&
                                (
                                      (firstEReadiness == "S")
                                   || (firstEReadiness == "U")
                                )
                            )
                        )
                        ||
                        (
                              (eProtocol == "04")
                           && (tProtocol == "06")
                        )
                        &&
                        (
                            (tReadiness == eReadiness)
                            || !
                            (
                                (
                                      (firstTReadiness == "S")
                                   || (firstTReadiness == "U")
                                ) &&
                                (
                                      (firstEReadiness == "S")
                                   || (firstEReadiness == "U")
                                )
                            )
                        )
                        ||
                        (
                              (eProtocol == "05")
                           && (tProtocol == "09")
                        )
                        &&
                        (
                            (tReadiness == eReadiness)
                            || !
                            (
                                (
                                      (firstTReadiness == "S")
                                   || (firstTReadiness == "U")
                                )
                                &&
                                (
                                      (firstEReadiness == "S")
                                   || (firstEReadiness == "U")
                                )
                            )
                        )
                        ||
                        (
                              (eProtocol == "05")
                           && (tProtocol == "0B")
                        )
                        &&
                        (
                            (tReadiness == eReadiness)
                            || !
                            (
                                (
                                      (firstTReadiness == "S")
                                   || (firstTReadiness == "U")
                                )
                                &&
                                (
                                      (firstEReadiness == "S")
                                   || (firstEReadiness == "U")
                                )
                            )
                        )
                        ||
                        (
                              (eProtocol == "0A")
                           && (tProtocol == "0B")
                        )
                        &&
                        (
                            (tReadiness == eReadiness)
                            || !
                            (
                                (
                                      (firstTReadiness == "S")
                                   || (firstTReadiness == "U")
                                )
                                &&
                                (
                                      (firstEReadiness == "S")
                                   || (firstEReadiness == "U")
                                )
                            )
                        )
                    )
                )
            {
                addColorCoding = true;
            }
            return addColorCoding;
        }

        private bool EvaluateThreshold(string elapsedMins, string thresholdMins)
        {
            bool addColorCoding = false;

            if (elapsedMins != null && elapsedMins.Trim().Length == 5)
            {
                int threshold = NullSafe.ToInt(ThresholdMins);
                int mins = NullSafe.ToInt(elapsedMins.Substring(0, 2));

                if (mins < threshold)
                {
                    addColorCoding = true;
                }
            }

            return addColorCoding;
        }

        public override TableHeaderRow GetDrillTableHeader(int drillLevel)
        {
            TableHeaderRow headerRow = new TableHeaderRow();
            TableHeaderCell headerCell;

            headerRow.TableSection = TableRowSection.TableHeader;

            headerCell = new TableHeaderCell();
            headerCell.Text = ChartParams.DrillDown.DrillLevels[1].DrillType.ToString() + " Id";

            headerRow.Cells.Add(headerCell);

            foreach (KeyValuePair<string, ReportColumnInfo> kvp in BaseReport.ReportColumnData)
            {
                headerCell = new TableHeaderCell();
                headerCell.Text = kvp.Value.DisplayName;
                headerRow.Cells.Add(headerCell);
            }

            return headerRow;
        }

        public override OracleParameter[] GetOracleParams(bool isReport)
        {
            List<OracleParameter> paramList = new List<OracleParameter>();

            string decile = String.IsNullOrEmpty(ChartParams.SelectedPoint) ? "" : ChartParams.SelectedPoint;

            paramList.Add(new OracleParameter("StartDate", OracleDbType.Varchar2, 8, ChartParams.StartDateText, ParameterDirection.Input));
            paramList.Add(new OracleParameter("EndDate", OracleDbType.Varchar2, 8, ChartParams.EndDateText, ParameterDirection.Input));
            paramList.Add(new OracleParameter("SIDSel", OracleDbType.Varchar2, 25, ChartParams.DrillDown.DrillLevels[0].DrillValue, ParameterDirection.Input));
            paramList.Add(new OracleParameter("TableType", OracleDbType.Varchar2, 3, ChartParams.TableType, ParameterDirection.Input));
            paramList.Add(new OracleParameter("DASel", OracleDbType.Varchar2, 12, decile, ParameterDirection.Input));
            paramList.Add(new OracleParameter("RECORD_DATA", OracleDbType.RefCursor, ParameterDirection.Output));

            return paramList.ToArray();
        }

        public virtual OracleParameter[] GetDetailedOracleParams()
        {
            List<OracleParameter> paramList = new List<OracleParameter>();

            string decile = ChartParams.SelectedPoint == null ? "" : ChartParams.SelectedPoint;
            paramList.Add(new OracleParameter("StartDate", OracleDbType.Varchar2, 8, ChartParams.StartDateText, ParameterDirection.Input));
            paramList.Add(new OracleParameter("EndDate", OracleDbType.Varchar2, 8, ChartParams.EndDateText, ParameterDirection.Input));
            paramList.Add(new OracleParameter("SIDSel", OracleDbType.Varchar2, 25, ChartParams.SelectedPoint, ParameterDirection.Input));
            paramList.Add(new OracleParameter("TableType", OracleDbType.Varchar2, 3, ChartParams.TableType, ParameterDirection.Input));
            paramList.Add(new OracleParameter("RECORD_DATA", OracleDbType.RefCursor, ParameterDirection.Output));

            return paramList.ToArray();
        }

        public override string RenderContainer()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(" <div class='div-centered'> ");
            builder.AppendLine(" <table class='tbl-chrt-obd-readiness'> ");
            builder.AppendLine(" <tr><td class='tbl-chrt-note'>Click on the chart to view the supporting data table.</td></tr> ");
            builder.AppendLine(" <tr><td><div class='div-chrt-med-center'>" + Container.ChartWrappers[0].Chart.ChartContainerHtmlString().ToString() + "</div></td></tr> ");
            builder.AppendLine(" </table> ");
            builder.AppendLine(" </div> ");

            return builder.ToString();
        }

        public override void GetContainerSeries()
        {
            DataTable dt = BaseReportMaster.GetProcedureDataTable(ChartProcName, GetOracleParams(false));

            if (dt.Rows.Count < 1) return;

            Container.ChartWrappers[0].Chart.SetSeries(new Series { Id = "BOTTOM_DRILL_LEVEL", Name = "Trigger Data", Data = GetSeriesData(dt), ShowInLegend = false });
        }

        protected Data GetSeriesData(DataTable dt)
        {
            Data data;
            try
            {
                if (ChartParams.ChartType == ChartTypes.Pie)
                {
                    data = new Data(LoadPieSeriesData(dt));
                }
                else
                {
                    data = new Data(LoadSeriesData(dt));
                }
            }
            catch (Exception ex)
            {
                ILogger Logger = SessionHelper.GetSessionLogger(System.Web.HttpContext.Current.Session);
                Logger.Error("There was a problem loading the detailed report from the base trigger class." + Environment.NewLine + ex.Message);
                Logger.Info("Test Info Log with params {0} and {1}", false, "some other param");
                Logger.Log("There was a problem loading the detailed report from the base trigger class." + Environment.NewLine + ex.Message, LogSeverity.Error);
                data = new Data(new object[] { });
            }

            return data;
        }

        // Loads the data for a pie chart series.
        private SeriesData[] LoadPieSeriesData(DataTable dt)
        {
            List<SeriesData> seriesDataList = new List<SeriesData>();

            foreach (DataRow dRow in dt.Rows)
            {
                seriesDataList.Add(new SeriesData { Name = "Decile: " + dRow["DA"].ToString(), X = NullSafe.ToDouble(dRow["DA"]), Y = NullSafe.ToDouble(dRow["CNT"]) });
            }

            return seriesDataList.ToArray();
        }

        /// <summary>Loads the data for a non pie chart series</summary>
        protected object[] LoadSeriesData(DataTable dt)
        {
            List<object[]> dblArrayData = new List<object[]>();

            foreach (DataRow dRow in dt.Rows)
            {
                dblArrayData.Add(new object[] { dRow["DA"].ToString(), NullSafe.ToDouble(dRow["CNT"]) });
            }

            return dblArrayData.ToArray();
        }

        public override void LoadReport()
        {
            DataTable dt;
            Report report = new Report(this.BaseReport.ReportTitle + " Data");
            ReportRow headerRow = new ReportRow(report);
            ReportCell currentCell;
            ReportRow currentRow;
            string onclick = String.Empty;

            dt = BaseReportMaster.GetProcedureDataTable(ChartProcName, GetOracleParams(false));

            ChartParams.DrillDown.DrillLevel = 0;
            report.Sortable = true;
            report.ID = "tblChart";

            report.Columns.Add("TRIGGER_IQ", "Trigger IQTM");
            report.Columns.Add("STATION_INSPECTOR", StationInspector);

            foreach (KeyValuePair<string, ReportColumnInfo> kvp in BaseReport.ReportColumnData)
            {
                report.Columns.Add(kvp.Key, kvp.Value.DisplayName, kvp.Value.ColumnDataType);
            }

            foreach (DataRow dRow in dt.Rows)
            {
                currentRow = new ReportRow(report);
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    currentCell = new ReportCell();
                    if (i == 0)
                    {
                        currentCell = new ReportCell();
                        currentCell.Value = "Details";
                        currentCell.Href = "#/";
                        currentCell.OnClick = "javascript: triggerDetailClick('" + dRow[0].ToString() + "')";
                        currentRow.Cells.Add(currentCell);

                        if (dRow[1] != null)
                        {
                            onclick = "javascript: drillDown(this.parentNode.parentNode, '" + dRow[0].ToString().Trim() + "', 0,'" + dRow[1].ToString().Replace("\"", "*").Trim() + "', '');";
                        }
                    }

                    currentCell = new ReportCell();
                    currentCell.Value = dRow[i].ToString();
                    currentRow.Cells.Add(currentCell);

                    if (i == 0)
                    {
                        currentCell.Href = "#/";
                        currentCell.OnClick = onclick;
                    }
                }
                report.Rows.Add(currentRow);
            }

            this.Report = report;
        }

        /// <summary>Builds a chart object based on its surrounding wrapper.</summary>
        public void BuildChart(NHChartWrapper wrap)
        {
            string yAxisLabel = ChartParams.DrillDown.DrillLevels[0].DrillType == NHDrillType.Station ? "Station" : "Inspector";

            wrap.ChartType = ChartParams.ChartType;
            wrap.ChartDimention = ChartParams.ChartDimention;

            if (wrap.ChartType == ChartTypes.Pie)
            {
                wrap.BuildChart();
            }
            else
            {
                wrap.BuildChart(
                    new XAxis
                    {
                        Title = new XAxisTitle { Text = "<b>Decile</b>" },
                    },
                    new YAxis
                    {
                        Title = new YAxisTitle { Text = "<b>" + yAxisLabel + " Count</b>", },
                        GridLineColor = System.Drawing.ColorTranslator.FromHtml("#AAAAAA"),
                        Labels = new YAxisLabels { Formatter = "function () {return '' + this.value.toLocaleString();}" }
                    }
                    );
                wrap.Chart.SetTitle(new Title { Text = wrap.ChartTitle, X = -20 });
                wrap.Chart.SetTooltip(new Tooltip
                {
                    Formatter = "function () { return '<span style=\"color:'+ this.point.color +';font-size:large;\">\u25CF</span> Decile: '+ this.x+ ': <b>' + this.y.toLocaleString() + '</b>'; }"
                });
            }

            wrap.Chart.SetTitle(new Title { Text = wrap.ChartTitle, X = -20 });
        }

        public override void LoadContainer()
        {
            if (ChartParams == null) return;

            Container = new ChartContainer();

            NHChartWrapper trigWrap = NHChartWrapper.Create("triggers", "evinTriggerData");
            trigWrap.Height = 400;
            trigWrap.Width = 700;
            trigWrap.ChartType = ChartParams.ChartType;
            trigWrap.EnableReportDrillDown = true;
            trigWrap.EnableDataLabels = true;
            trigWrap.OverrideDrillFunction = "function(e){ triggerChartClick(e.point.series.userOptions.id, e.point.series.name, e.point.x);}";
            trigWrap.ChartTitle = "<b>" + BaseReport.ReportTitle + "</b>";
            BuildChart(trigWrap);
            Container.AddChartWrapper(trigWrap);

            GetContainerSeries();
        }

        public override void SetMetaData()
        {
            MetaList = new List<ChartMetaData>();

            MetaList.Add(new ChartMetaData("Start Date", ChartParams.StartDate));
            MetaList.Add(new ChartMetaData("End Date", ChartParams.EndDate));
            MetaList.Add(new ChartMetaData("Chart Type", ChartParams.ChartType.ToString()));
            MetaList.Add(new ChartMetaData("Chart Display", ChartParams.ChartDimention.ToString()));
            MetaList.Add(new ChartMetaData("Drill Down Order", ChartParams.TableType));
        }

        /// <summary>Returns a dictionary based on drill level.</summary>
        protected Dictionary<string, ReportColumnInfo> GetDetailedReportColumns()
        {
            Dictionary<string, ReportColumnInfo> returnVal;

            switch (ChartParams.DrillDown.DrillLevels[0].DrillValue)
            {
                case "Communication Mismatch Trigger":
                case "OBDPDA":
                    returnVal = BaseReportMaster.CommProtocol.ReportColumnData;
                    break;
                case "No Voltage Trigger":
                case "NOVDA":
                    returnVal = BaseReportMaster.NoVoltage.ReportColumnData;
                    break;
                case "Readiness Mismatch Trigger":
                case "OBDIDA":
                    returnVal = BaseReportMaster.ReadinessMismatch.ReportColumnData;
                    break;
                case "Safety Defect Trigger":
                case "SDDA":
                    returnVal = BaseReportMaster.SafetyDefect.ReportColumnData;
                    break;
                case "Test Rejections Trigger":
                case "OBDRDA":
                    returnVal = BaseReportMaster.Rejection.ReportColumnData;
                    break;
                case "Time Before Test Trigger (3 Minute Threshold)":
                case "TBTDA":
                    returnVal = new Dictionary<string, ReportColumnInfo>() { { "TEST_COUNT", new ReportColumnInfo("Test Count", ColumnDataType.Number) }, { "THRESHOLD_CNT", new ReportColumnInfo("Threshold Count", ColumnDataType.Number) }, { "THRESHOLD_PCT", new ReportColumnInfo("Threshold %") } };
                    break;
                case "eVIN Mismatch Trigger":
                case "EVINDA":
                    returnVal = BaseReportMaster.EVINMismatch.ReportColumnData;
                    break;
                default:
                    returnVal = new Dictionary<string, ReportColumnInfo>();
                    break;
            }

            return returnVal;
        }

        private const string LowerProtocolBound = "00";
        private const string UpperProtocolBound = "FF";

        /// <summary>Whether or not a report is a detailed version.</summary>
        public bool IsDetailedReport { get; set; }
        /// <summary>The label for a station or inspector selection</summary>
        public string StationInspector { get; set; }
        /// <summary>MEta data for a station or inspector selection.</summary>
        public string StationInspectorMetaData { get; set; }
        /// <summary>Selected ID number for a station or inspector.</summary>
        public string IDNumberString { get; set; }
        /// <summary>Threshold value for time between tests.</summary>
        public string ThresholdMins { get; set; }
        /// <summary>The minimum number of values to filter by.</summary>
        public int FilterCount { get; set; }
        /// <summary>The weightings for each trigger category.</summary>
        public Dictionary<string, int> TriggerWeights { get; set; }
    }

    /// <summary>Defines the column information for different selections in a detailed weighted trigger report.</summary>
    public static class TriggerDetailedReportHeader
    {
        public static Dictionary<string, string> MainColumns { get; private set; }
        public static Dictionary<string, string> VehicleInfoColumns { get; private set; }
        public static Dictionary<string, string> InspectionInfoColumns { get; private set; }
        public static Dictionary<string, string> VisualInfoColumns { get; private set; }
        public static Dictionary<string, string> OBDInfoColumns { get; private set; }
        public static Dictionary<string, string> SafetyInfoColumns { get; private set; }

        static TriggerDetailedReportHeader()
        {
            MainColumns = new Dictionary<string, string>();
            VehicleInfoColumns = new Dictionary<string, string>();
            InspectionInfoColumns = new Dictionary<string, string>();
            VisualInfoColumns = new Dictionary<string, string>();
            OBDInfoColumns = new Dictionary<string, string>();
            SafetyInfoColumns = new Dictionary<string, string>();

            MainColumns.Add("TESTDATE", "Test Date & Time");
            MainColumns.Add("ELAPSED_MIN", "Time Before Tests");
            MainColumns.Add("VIN", "VIN");
            MainColumns.Add("MAKE", "Make");
            MainColumns.Add("MODEL", "Model");
            MainColumns.Add("MODELYEAR", "Model Year");
            MainColumns.Add("OBD_VIN", "eVIN");
            MainColumns.Add("RPM", "RPM");
            MainColumns.Add("VOLTAGE", "Voltage");
            MainColumns.Add("TPROTOCOL", "Test Protocol");
            MainColumns.Add("EPROTOCOL", "Expected Protocol");
            MainColumns.Add("TREADINESS", "Test Readiness Monitors");
            MainColumns.Add("EREADINESS", "Expected Readiness Monitors");

            VehicleInfoColumns.Add("FUELCODE", "Fuel Code");
            VehicleInfoColumns.Add("GVW", "GVW");
            VehicleInfoColumns.Add("VINDECODED", "VIN Decoded");
            VehicleInfoColumns.Add("TAG", "License Plate Number");
            VehicleInfoColumns.Add("NUMCYLINDERS", "Number Cylinders");
            VehicleInfoColumns.Add("ODOMETER", "Odometer");
            VehicleInfoColumns.Add("TRANSTYPE", "Transmission Type");
            VehicleInfoColumns.Add("BODYSTYLE", "Body Style");

            InspectionInfoColumns.Add("PLATETYPE", "Plate Type");
            InspectionInfoColumns.Add("REGISTRATIONNUM", "Registration Number");
            InspectionInfoColumns.Add("REGDATE", "Registration Date");
            InspectionInfoColumns.Add("REGEXPDATE", "Registration Expiration Date");
            InspectionInfoColumns.Add("COUNTY", "County");
            InspectionInfoColumns.Add("STATIONID", "Station Id");
            InspectionInfoColumns.Add("INSPECTORID", "Inspector Id");
            InspectionInfoColumns.Add("UNITID", "UNIT Id");
            InspectionInfoColumns.Add("SAFETYTESTTYPE", "Safety Test Type");
            InspectionInfoColumns.Add("EMISSTESTTYPE", "Emission Test Type");
            InspectionInfoColumns.Add("OVERALLPF", "Overall Test Result");
            InspectionInfoColumns.Add("STICKERTYPE", "Sticker Type");
            InspectionInfoColumns.Add("STICKERSERIES", "Sticker Series");
            InspectionInfoColumns.Add("STICKERNUMBER", "Sticker Number");
            InspectionInfoColumns.Add("NOTES", "Notes");

            VisualInfoColumns.Add("VISAIRPUMP", "Air Pump");
            VisualInfoColumns.Add("VISPCV", "PCV");
            VisualInfoColumns.Add("VISEVAP", "Evap System");
            VisualInfoColumns.Add("VISFUEL", "Fuel Cap");
            VisualInfoColumns.Add("VISCAT", "Catalyst");
            VisualInfoColumns.Add("VISUALPF", "Visual Test Result");

            OBDInfoColumns.Add("OBDWAIVER", "Waiver Issued");
            OBDInfoColumns.Add("OBDADVISORY", "OBD Test Was Advisory");
            OBDInfoColumns.Add("OBDLOCATABLE", "DLC Locatable");
            OBDInfoColumns.Add("OBDCOMMUNICABLE", "Communicable");
            OBDInfoColumns.Add("MILCMDON", "MIL Commanded On");
            OBDInfoColumns.Add("MILENGOFF", "Key On Engine Off");
            OBDInfoColumns.Add("MILENGON", "Key On Engine Running");
            OBDInfoColumns.Add("OBDMISFIRE", "Misfire Monitor");
            OBDInfoColumns.Add("OBDFUELSYSTEM", "Fuel System Monitor");
            OBDInfoColumns.Add("OBDCOMPONENT", "Component Monitor");
            OBDInfoColumns.Add("OBDCATALYST", "Catalyst Monitor");
            OBDInfoColumns.Add("OBDHEATEDCATALYST", "Heated Catalyst Monitor");
            OBDInfoColumns.Add("OBDEVAP", "Evap Monitor");
            OBDInfoColumns.Add("OBDAIRSYSTEM", "Air System Monitor");
            OBDInfoColumns.Add("OBDAC", "A/C Monitor");
            OBDInfoColumns.Add("OBDOXYGEN", "Oxygen Sensor Monitor");
            OBDInfoColumns.Add("OBDHEATEDOXYGEN", "Heated Oxygen Sensor Monitor");
            OBDInfoColumns.Add("OBDEGR", "EGR Monitor");
            OBDInfoColumns.Add("NUMDTCS", "Number DTC's");
            OBDInfoColumns.Add("DTC1", "DTC1");
            OBDInfoColumns.Add("DTC2", "DTC2");
            OBDInfoColumns.Add("DTC3", "DTC3");
            OBDInfoColumns.Add("DTC4", "DTC4");
            OBDInfoColumns.Add("DTC5", "DTC5");
            OBDInfoColumns.Add("DTC6", "DTC6");
            OBDInfoColumns.Add("DTC7", "DTC7");
            OBDInfoColumns.Add("DTC8", "DTC8");
            OBDInfoColumns.Add("DTC9", "DTC9");
            OBDInfoColumns.Add("DTC10", "DTC10");
            OBDInfoColumns.Add("OBDPF", "OBD Test Result");

            SafetyInfoColumns.Add("SAFETYPF", "Safety Test Result");
            SafetyInfoColumns.Add("SAF_BS_VEHINFO", "Basic Vehicle Info");
            SafetyInfoColumns.Add("BWHEELS", "Basic Wheels");
            SafetyInfoColumns.Add("BTIRES", "Basic Tires");
            SafetyInfoColumns.Add("BSTEERING", "Basic Steering / Front  End");
            SafetyInfoColumns.Add("BFOOTBRAKE", "Basic Foot Brake");
            SafetyInfoColumns.Add("BPARKINGBRAKE", "Basic Parking Brake");
            SafetyInfoColumns.Add("BINSTRUMENTS", "Basic Instruments");
            SafetyInfoColumns.Add("BHORNELECTRIC", "Basic Horn / Electrical System");
            SafetyInfoColumns.Add("BREARLIGHTS", "Basic Rear Lights");
            SafetyInfoColumns.Add("BSTOPLIGHTS", "Basic Stop Lights");
            SafetyInfoColumns.Add("BFRONTLIGHTS", "Basic Front Lights");
            SafetyInfoColumns.Add("BDIRSIGNAL", "Basic Directional Signal");
            SafetyInfoColumns.Add("BOTHERLIGHTS", "Basic Other Lights");
            SafetyInfoColumns.Add("BHEADLIGHTAIM", "Basic Headlight Aim");
            SafetyInfoColumns.Add("BMIRRORS", "Basic Mirrors");
            SafetyInfoColumns.Add("BDEFROSTER", "Basic Defroster");
            SafetyInfoColumns.Add("BGLASS", "Basic Glass");
            SafetyInfoColumns.Add("BWIPERS", "Basic Wipers");
            SafetyInfoColumns.Add("BEXHAUST", "Basic Exhaust");
            SafetyInfoColumns.Add("BFUELSYSTEM", "Basic Fuel System");
            SafetyInfoColumns.Add("BBUMPERS", "Basic Bumpers");
            SafetyInfoColumns.Add("BBODYCHASSIS", "Basic Body / Chassis");
            SafetyInfoColumns.Add("TBVEHINFO", "Truck/Bus Vehicle Info");
            SafetyInfoColumns.Add("TBWHEELS", "Truck/Bus Wheels");
            SafetyInfoColumns.Add("TBTIRES", "Truck/Bus Tires");
            SafetyInfoColumns.Add("TBSTEERING", "Truck/Bus Steering / Front End");
            SafetyInfoColumns.Add("TBFOOTBRAKE", "Truck/Bus Foot Brake");
            SafetyInfoColumns.Add("TBPARKINGBRAKE", "Truck/Bus Parking Brake");
            SafetyInfoColumns.Add("TBAIRBRAKE", "Truck/Bus Air Brake System");
            SafetyInfoColumns.Add("TBINSTRUMENTS", "Truck/Bus Instruments");
            SafetyInfoColumns.Add("TBHORNELECTRIC", "Truck/Bus  Horn / Electrical System");
            SafetyInfoColumns.Add("TBREARLIGHTS", "Truck/Bus Rear Lights");
            SafetyInfoColumns.Add("TBSTOPLIGHTS", "Truck/Bus Stop Lights");
            SafetyInfoColumns.Add("TBFRONTLIGHTS", "Truck/Bus Front Lights");
            SafetyInfoColumns.Add("TBDIRSIGNAL", "Truck/Bus Directional Signal");
            SafetyInfoColumns.Add("TBOTHERLIGHTS", "Truck/Bus Other Lights");
            SafetyInfoColumns.Add("TBHEADLIGHTAIM", "Truck/Bus Headlight Aim");
            SafetyInfoColumns.Add("TBMIRRORS", "Truck/Bus Mirrors");
            SafetyInfoColumns.Add("TBDEFROSTER", "Truck/Bus Defroster");
            SafetyInfoColumns.Add("TBGLASS", "Truck/Bus Glass");
            SafetyInfoColumns.Add("TBWIPERS", "Truck/Bus Wipers");
            SafetyInfoColumns.Add("TBREFLECTOR", "Truck/Bus Reflective Warning Device");
            SafetyInfoColumns.Add("TBFIREEXT", "Truck/Bus Fire Extinguisher");
            SafetyInfoColumns.Add("TBEXHAUST", "Truck/Bus Exhaust");
            SafetyInfoColumns.Add("SAF_TB_FUELSYS", "Truck/Bus Fuel System");
            SafetyInfoColumns.Add("SAF_TB_BUMPERS", "Truck/Bus Bumpers");
            SafetyInfoColumns.Add("SAF_TB_BODY", "Truck/Bus Chassis");
            SafetyInfoColumns.Add("SAF_TB_BUSBODY", "Truck/Bus Bus Body");
            SafetyInfoColumns.Add("SAF_TB_BUSINTERIOR", "Truck/Bus Bus Interior");
            SafetyInfoColumns.Add("SAF_TR_VEHINFO", "Trailer Vehicle Info");
            SafetyInfoColumns.Add("SAF_TR_TIRES", "Trailer Tires");
            SafetyInfoColumns.Add("SAF_TR_MAINBRAKES", "Trailer Main Brakes");
            SafetyInfoColumns.Add("SAF_TR_PARKINGBRAKE", "Trailer Parking Brake (Air)");
            SafetyInfoColumns.Add("SAF_TRAIEER_EBREAKS", "Trailer Emergency Brakes");
            SafetyInfoColumns.Add("SAF_TR_BREAKWIRING", "Trailer Brake Wiring");
            SafetyInfoColumns.Add("SAF_TR_REARLIGHTS", "Trailer Rear Lights");
            SafetyInfoColumns.Add("SAF_TR_STOPLIGHTS", "Trailer Stop Lights");
            SafetyInfoColumns.Add("SAF_TR_BUMPER", "Trailer Bumper");
            SafetyInfoColumns.Add("SAF_TR_BODY", "Trailer Body / Chassis");
            SafetyInfoColumns.Add("SAF_AG_VIN", "Agricultural VIN");
            SafetyInfoColumns.Add("SAF_AG_BRAKES", "Agricultural Brakes");
            SafetyInfoColumns.Add("SAF_AG_STEERING", "Agricultural Steering Wheel");
            SafetyInfoColumns.Add("SAF_AG_STOPLIGHTS", "Agricultural Stop Lights");
            SafetyInfoColumns.Add("SAF_AG_EXHAUST", "Agricultural Exhaust System");
            SafetyInfoColumns.Add("SAF_AG_HEADLIGHTS", "Agricultural Headlights");
            SafetyInfoColumns.Add("SAF_AG_REFLECTORS", "Agricultural Reflectors");
            SafetyInfoColumns.Add("SAF_AG_TAILLIGHTS", "Agricultural Tail Lights");
            SafetyInfoColumns.Add("SAF_MC_VEHINFO", "Motorcycle Vehicle Information");
            SafetyInfoColumns.Add("SAF_MC_WHEELS", "Motorcycle Wheels");
            SafetyInfoColumns.Add("SAF_MC_TIRES", "Motorcycle Tires");
            SafetyInfoColumns.Add("SAF_MC_STEERING", "Motorcycle Steering / Front End");
            SafetyInfoColumns.Add("SAF_MC_FOOTBRAKE", "Motorcycle Foot Brake");
            SafetyInfoColumns.Add("SAF_MC_INST", "Motorcycle Instruments");
            SafetyInfoColumns.Add("SAF_MC_HORN", "Motorcycle Horn / Electrical System");
            SafetyInfoColumns.Add("SAF_MC_REARLIGHTS", "Motorcycle Rear Lights");
            SafetyInfoColumns.Add("SAF_MC_STOPLIGHTS", "Motorcycle Stop Lights");
            SafetyInfoColumns.Add("SAF_MC_FRONTLIGHTS", "Motorcycle Front Lights");
            SafetyInfoColumns.Add("SAF_MC_SIGNAL", "Motorcycle Directional Signal");
            SafetyInfoColumns.Add("SAF_MC_OTHERLIGHTS", "Motorcycle Other Lights");
            SafetyInfoColumns.Add("SAF_MC_HEADLIGHTAIM", "Motorcycle Headlight Aim");
            SafetyInfoColumns.Add("SAF_MC_MIRRORS", "Motorcycle Mirrors");
            SafetyInfoColumns.Add("SAF_MC_GLASS", "Motorcycle Glass");
            SafetyInfoColumns.Add("SAF_MC_EXHAUST", "Motorcycle Exhaust");
            SafetyInfoColumns.Add("SAF_MC_FUELSYS", "Motorcycle Fuel System");
            SafetyInfoColumns.Add("SAF_MC_BODY", "Motorcycle Body / Chassis / Other");
        }
    }
}