using GD.Highcharts.Enums;
using GD.Highcharts.GDAnalytics;
using GD.Highcharts.Options;
using GDCoreUtilities;
using NHPortal.Classes.Reports.Triggers;
using Oracle.DataAccess.Client;
using PortalFramework.ReportModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI.WebControls;

namespace NHPortal.Classes.Charts
{
    /// <summary>Defines basic functionality for a chart object.</summary>
    public class BaseChart : IChartReportData
    {
        public virtual void LoadReport()
        {
            DataTable dt;
            Report report = new Report(this.BaseReport.ReportTitle);
            ReportRow headerRow = new ReportRow(report);
            ReportCell currentCell;
            ReportRow currentRow;

            dt = BaseReportMaster.GetProcedureDataTable(DrillProcName, GetOracleParams(true));

            ChartParams.DrillDown.DrillLevel = 0;
            report.Sortable = true;
            report.ID = "tblChart";

            // Build Columns
            report.Columns.Add("TBL_TEST_DATE", "Test " + this.ChartParams.GroupByData.GroupLabel);

            string text;
            if (this.ChartParams.DrillDown.DrillLevels[0].DrillType == NHDrillType.ModelYear)
            {
                text = "Model Year";
            }
            else
            {
                    text = this.ChartParams.DrillDown.DrillLevels[0].DrillType.ToString();
            }

            report.Columns.Add("TBL_GRP_ZERO", text);

            foreach (KeyValuePair<string, ReportColumnInfo> kvp in BaseReport.ReportColumnData)
            {
                report.Columns.Add(kvp.Key, kvp.Value.DisplayName, kvp.Value.ColumnDataType);
            }

            if (this is OBDIIReadinessMonitors)
            {
                if (ChartParams.SelectedSeries.ToUpper().Trim() != "OVERALL")
                {
                    report.Columns.Add("TBL_UNSUPPORTED_COUNT", "Unsupported Status", ColumnDataType.Number);
                    report.Columns.Add("TBL_UNSUPPORTED_PCT", "Unsupported %", ColumnDataType.Percentage);
                }
            }

            // Build Report body.
            foreach (DataRow dRow in dt.Rows)
            {
                currentRow = new ReportRow(report);
                string onclick = String.Empty;

                // Js function containing row, test year, dril level, drill value, prev drill value.
                if (dRow[1] != null)
                { 
                    onclick = "javascript: drillDown(this.parentNode.parentNode, '" + dRow[0].ToString().Trim() + "', 0,'" + dRow[1].ToString().Replace("\"", "*").Trim() + "', '');";
                }

                try
                {
                    foreach (ReportColumn rc in report.Columns)
                    {
                        currentCell = new ReportCell();
                        currentCell.Value = dRow[rc.Name].ToString();
                        currentRow.Cells.Add(currentCell);

                        // Add drill funciton to second column.
                        if (rc.Name == "TBL_GRP_ZERO")
                        {
                            currentCell.Href = "#/";
                            currentCell.OnClick = onclick;
                        }
                    }
                }
                catch
                {
                    // TODO Add logging
                }

                report.Rows.Add(currentRow);
            }

            this.Report = report;
        }

        public virtual string LoadDrillLevel(string testDateVal, string drillValue, string prevDrillValue, int drillLevel)
        {
            string returnVal;
            string tableName = "tbl" + testDateVal + drillValue + prevDrillValue;
            Table table = GetReportTable(tableName.Replace("(","").Replace(")",""));
            TableHeaderRow headerRow;
            TableRow currentRow;
            TableCell currentCell;
            DataTable dt;

            ChartParams.DrillDown.DrillLevel = NullSafe.ToInt(drillLevel) + 1;
            ChartParams.DrillDown.DrillLevels[0].DrillValue = testDateVal;

            headerRow = GetDrillTableHeader(drillLevel);

            if (ChartParams.DrillDown.DrillLevels.Length > 2)
            {
                if (String.IsNullOrEmpty(prevDrillValue))
                {
                    ChartParams.DrillDown.DrillLevels[1].DrillValue = drillValue;
                    ChartParams.DrillDown.DrillLevels[2].DrillValue = prevDrillValue;
                }
                else
                {
                    ChartParams.DrillDown.DrillLevels[1].DrillValue = prevDrillValue;
                    ChartParams.DrillDown.DrillLevels[2].DrillValue = drillValue;
                }
            }

            table.Rows.Add(headerRow);

            dt = BaseReportMaster.GetProcedureDataTable(DrillProcName, GetOracleParams(true));

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dRow in dt.Rows)
                {
                    currentRow = new TableRow();

                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        currentCell = new TableCell();

                        if (i == 0 && drillLevel < 1 && (ChartParams.DrillDown.DrillLevels.Length > 2 || this is StationInspector))
                        {
                            currentCell.Text = "<a href='#/' onclick=\"drillDown(this.parentNode.parentNode,'"
                                + testDateVal + "',"
                                + (drillLevel + 1).ToString() + ",'"
                                + dRow[0].ToString().Trim() + "','"
                                + drillValue + "'); \">"
                                + dRow[0].ToString().Trim() + "</a>";

                            currentRow.Cells.Add(currentCell);
                        }
                        else
                        {
                            currentCell.Text = dRow[i].ToString();
                            currentRow.Cells.Add(currentCell);
                        }
                    }

                    table.Rows.Add(currentRow);
                }

                returnVal = NHChartMaster.RenderHTMLControl(table);
            }
            else
            {
                returnVal = "<div class='drill-no-data'>No Data Found.</div>";
            }

            

            return returnVal;
        }

        public void BuildChart(NHChartWrapper wrap, string[] categories = null)
        {
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
                        Type = (categories == null ? AxisTypes.Datetime : AxisTypes.Category),
                        Title = new XAxisTitle { Text = "<b>" + XaxisTitle + "</b>" },
                        Categories = categories
                    },
                    new YAxis
                    {
                        Title = new YAxisTitle { Text = "<b>% Test Count</b>", },
                        GridLineColor = System.Drawing.ColorTranslator.FromHtml("#AAAAAA"),
                        Labels = new YAxisLabels { Formatter = "function () {return '' + this.value.toLocaleString();}" }
                    }
                    );
                wrap.Chart.SetTooltip(new Tooltip
                {
                    Formatter = "function () { return '<span style=\"color:'+ this.point.color +';font-size:large;\">\u25CF</span> '+ this.point.name + ': <b>' + this.y.toLocaleString() + '%</b>'; }"
                });
            }

            wrap.Chart.SetTitle(new Title { Text = wrap.ChartTitle, X = -20 });
        }

        public static Table GetReportTable(string tableId)
        {
            Table table = new Table();
            table.ID = tableId.Replace(" ", "");
            table.CssClass = "tablesorter";
            table.ClientIDMode = System.Web.UI.ClientIDMode.Static;

            return table;
        }

        public virtual TableHeaderRow GetDrillTableHeader(int drillLevel)
        {
            TableHeaderRow headerRow = new TableHeaderRow();
            TableHeaderCell headerCell;

            headerRow.TableSection = TableRowSection.TableHeader;

            headerCell = new TableHeaderCell();

            if (ChartParams.DrillDown.DrillLevels[drillLevel + 1].DrillType == NHDrillType.ModelYear)
            {
                headerCell.Text = "Model Year";
            }
            else
            {
                headerCell.Text = ChartParams.DrillDown.DrillLevels[drillLevel + 1].DrillType.ToString();
            }

            headerRow.Cells.Add(headerCell);

            foreach (KeyValuePair<string, ReportColumnInfo> kvp in BaseReport.ReportColumnData)
            {
                headerCell = new TableHeaderCell();
                headerCell.Text = kvp.Value.DisplayName;
                headerRow.Cells.Add(headerCell);
            }

            if (this is OBDIIReadinessMonitors)
            {
                if (ChartParams.SelectedSeries.ToUpper().Trim() != "OVERALL")
                {
                    headerCell = new TableHeaderCell();
                    headerCell.Text = "Unsupported Status";
                    headerRow.Cells.Add(headerCell);

                    headerCell = new TableHeaderCell();
                    headerCell.Text = "Unsupported %";
                    headerRow.Cells.Add(headerCell);
                }
            }

            return headerRow;
        }

        public virtual string RenderContainer()
        {
            throw new NotImplementedException();
        }

        public virtual void LoadDetailedReport()
        {
            throw new NotImplementedException();
        }

        public virtual OracleParameter[] GetOracleParams(bool isReport)
        {
            throw new NotImplementedException();
        }

        public virtual void LoadContainer()
        {
            throw new NotImplementedException();
        }

        public virtual void GetContainerSeries()
        {
            throw new NotImplementedException();
        }

        public void ExportCSV(HttpResponse response)
        {
            if (Report != null)
            {
                NHPortalUtilities.ExportReportToCsv(Report, response);
            }
        }

        public void ExportExcel(HttpResponse response)
        {
            if (Report != null)
            {
                NHPortalUtilities.ExportReportToXLSX(Report, response);
            }
        }

        public void ExportPDF(HttpResponse response)
        {
            if (Report != null)
            {
                NHPortalUtilities.ExportReportToPDF(Report, response);
            }
        }

        public virtual void SetMetaData()
        {
            MetaList = new List<ChartMetaData>();

            MetaList.Add(new ChartMetaData("Start Date", ChartParams.StartDate));
            MetaList.Add(new ChartMetaData("End Date", ChartParams.EndDate));
            MetaList.Add(new ChartMetaData("Group By", ChartParams.GroupByData.DataGrouping.ToString()));
            MetaList.Add(new ChartMetaData("Chart Type", ChartParams.ChartType.ToString()));
            MetaList.Add(new ChartMetaData("Chart Display", ChartParams.ChartDimention.ToString()));
            MetaList.Add(new ChartMetaData("Drill Down Order", ChartParams.TableType));
        }

        public ChartContainer Container { get; set; }
        public ChartParams ChartParams { get; set; }
        public BaseReport BaseReport { get; set; }
        public Report Report { get; set; }
        public bool RunInitialReport { get; set; }

        // Additional Properties
        /// <summary>Whether or not to include vehicle info columns in a detailed report.</summary>
        public bool IncludeVehicleInfo { get; set; }
        /// <summary>Whether or not to include inspection info columns in a detailed report.</summary>
        public bool IncludeInspectionInfo { get; set; }
        /// <summary>Whether or not to include visual info columns in a detailed report.</summary>
        public bool IncludeVisualInfo { get; set; }
        /// <summary>Whether or not to include OBD info columns in a detailed report.</summary>
        public bool IncludeOBDInfo { get; set; }
        /// <summary>Whether or not to include Safety info columns in a detailed report.</summary>
        public bool IncludeSafetyInfo { get; set; }
        /// <summary>The name of the stored procedure for drilling into a report.</summary>
        public string DrillProcName { get; set; }
        /// <summary>The name of the stored procedure for sub level drilling into a report.</summary>
        public string SubLevelDrillProcName { get; set; }
        /// <summary>The name of the procedure for building the series of a chart.</summary>
        public static string ChartProcName { get; set; }
        /// <summary>The name of the stored procedure for the detailed summary of a chart report.</summary>
        public string DetailedReportProcName { get; set; }
        /// <summary>The title of the x axis of a chart.</summary>
        public string XaxisTitle { get; set; }
        protected List<ChartMetaData> MetaList;
        /// <summary>Collection of chart meta data.</summary>
        public ChartMetaData[] ChartMetaData
        {
            get
            {
                SetMetaData();
                return MetaList.ToArray();
            }
        }
    }
}