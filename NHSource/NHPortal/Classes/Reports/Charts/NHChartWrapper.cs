using GD.Highcharts;
using GD.Highcharts.Enums;
using GD.Highcharts.Helpers;
using GD.Highcharts.Options;
using System;

namespace NHPortal.Classes.Charts
{
    public class NHChartWrapper : ChartWrapper
    {
        private NHChartWrapper()
            : base()
        {

        }

        private NHChartWrapper(string chartPrefix)
            : base(chartPrefix)
        {

        }

        private NHChartWrapper(string chartPrefix, string chartName)
            : base(chartPrefix, chartName)
        {

        }

        private void Init()
        {

        }

        public static NHChartWrapper Create()
        {
            NHChartWrapper wrap = new NHChartWrapper();
            return wrap;
        }

        public static NHChartWrapper Create(string chartName)
        {
            NHChartWrapper wrap = new NHChartWrapper(chartName);
            return wrap;
        }

        public static NHChartWrapper Create(string chartPrefix, string chartName)
        {
            NHChartWrapper wrap = new NHChartWrapper(chartPrefix, chartName);
            return wrap;
        }

        public override bool Delete()
        {
            throw new NotImplementedException();
        }

        protected override void Deserialize(string data)
        {
            throw new NotImplementedException();
        }

        public override bool Save()
        {
            throw new NotImplementedException();
        }

        public override bool SaveLayout()
        {
            throw new NotImplementedException();
        }

        public override void SetChartMetaData()
        {
            throw new NotImplementedException();
        }

        public new void BuildChart(GD.Highcharts.Options.XAxis xAxis = null, GD.Highcharts.Options.YAxis yAxis = null)
        {
            GD.Highcharts.Options.Chart chart = new GD.Highcharts.Options.Chart();
            chart.Type = ChartType;
            chart.Name = ChartName;
            chart.Width = Width;
            chart.Height = Height;

            if (this.HighsoftType == GD.Highcharts.Enums.HighsoftType.Map)
            {
                chart.PanKey = "shift";
                chart.ClassName = "map";
                chart.Panning = null;
                chart.ZoomType = ZoomTypes.Xy;
            }
            else if (this.ChartType == ChartTypes.Pie)
            {

            }
            else
            {
                chart.Panning = true;
                chart.PanKey = "shift";
                chart.ClassName = "chart";
                chart.Panning = true;
                chart.ZoomType = ZoomTypes.X;
            }

            if (this.ChartDimention == ChartDimention.ThreeDimentional)
            {
                if (this.ChartType == ChartTypes.Pie)
                {
                    chart.Options3d = new GD.Highcharts.Options.ChartOptions3d { Enabled = true, Alpha = 45, Beta = 0, Depth = 70 };
                }
                else
                {
                    chart.Options3d = new GD.Highcharts.Options.ChartOptions3d { Enabled = true, Alpha = 10, Beta = 25, Depth = 70 };
                }
            }

            Chart = new Highcharts(ChartName);
            Chart.InitChart(chart);
            Chart.SetCredits(new GD.Highcharts.Options.Credits { Enabled = false });
            Chart.SetBackgroundColor("#EEEEEE");

            if (xAxis == null && ChartType != ChartTypes.Pie)
            {
                SetXAxis();
            }
            else
            {
                SetXAxis(xAxis);
            }

            if (ChartType == ChartTypes.Pie)
            {
                Chart.SetTooltip(new Tooltip
                {
                    Formatter = "function () { return this.series.name + '<br /><span style=\"color:'+ this.point.color +';font-size:large;\">\u25CF</span> ' + ' ' + this.point.name + ': <b>' + this.y.toLocaleString() + '</b>'; }"
                });
            }
            else
            {
                if (yAxis == null)
                {
                    if (DashValueTypes.Length == 1 || ChartType == ChartTypes.Bubble)
                    {
                        SetYAxis();
                    }
                    else
                    {
                        SetYAxisArray();
                    }
                }
                else
                {
                    Chart.SetYAxis(yAxis);
                }
            }

            SetPlotOptions();
        }

        protected new void SetPlotOptions()
        {
            PlotOptionsSeries series = new PlotOptionsSeries { States = new PlotOptionsSeriesStates { Select = new PlotOptionsSeriesStatesSelect { Color = "null", BorderWidth = 0, BorderColor = "null" } } };
            string click;
            Cursors cursor;

            if (EnableReportDrillDown)
            {
                cursor = Cursors.Pointer;
                if (String.IsNullOrEmpty(OverrideDrillFunction))
                {
                    click = "function(e){ chartReportClick(e.point.series.userOptions.id, e.point.series.name, e.point.name);}";
                }
                else
                {
                    click = OverrideDrillFunction;
                }
            }
            else
            {
                click = null;
                cursor = Cursors.Default;
            }


            if (ChartType == ChartTypes.Pie)
            {
                Chart.SetPlotOptions(new PlotOptions
                {
                    Pie = new PlotOptionsPie
                    {
                        AllowPointSelect = EnableReportDrillDown,
                        Cursor = cursor,
                        Depth = 35,
                        Size = new PercentageOrPixel(Height * .50),
                        ShowInLegend = ShowLegend,
                        Events = new PlotOptionsPieEvents
                        {
                            Click = click
                        },
                        DataLabels = new PlotOptionsPieDataLabels
                        {
                            Enabled = true,
                            Padding = 0,
                            BackgroundColor = new BackColorOrGradient(System.Drawing.ColorTranslator.FromHtml("#eee")),
                            Formatter = "function () { return '<span style=\" color:' + this.point.color + '\">' + this.point.name + ' ' + this.point.y.toLocaleString() + ' (' + this.point.percentage.toFixed(2)  + '%)</span>';}",
                            //Style = "color: (Highcharts.theme && Highcharts.theme.contrastTextColor) || 'black'"
                        }
                    }
                });
            }
            else if (ChartType == ChartTypes.Column)
            {
                Chart.SetPlotOptions(new PlotOptions
                {
                    Column = new PlotOptionsColumn
                    {
                        AllowPointSelect = EnableReportDrillDown,
                        Cursor = cursor,
                        Depth = 35,
                        DataLabels = new PlotOptionsColumnDataLabels { Enabled = this.EnableDataLabels },
                        Events = new PlotOptionsColumnEvents
                        {
                            Click = click
                        }
                    },
                    Series = series
                });
            }
            else if (ChartType == ChartTypes.Bar)
            {
                Chart.SetPlotOptions(new PlotOptions
                {
                    Bar = new PlotOptionsBar
                    {
                        AllowPointSelect = EnableReportDrillDown,
                        DataLabels = new PlotOptionsBarDataLabels { Enabled = this.EnableDataLabels },
                        Cursor = cursor,
                        Depth = 35,
                        Events = new PlotOptionsBarEvents
                        {
                            Click = click
                        }
                    },
                    Series = series
                });
            }
            else if (ChartType == ChartTypes.Line)
            {
                Chart.SetPlotOptions(new PlotOptions
                {
                    Line = new PlotOptionsLine
                    {
                        AllowPointSelect = EnableReportDrillDown,
                        DataLabels = new PlotOptionsLineDataLabels { Enabled = this.EnableDataLabels },
                        Cursor = cursor,
                        Events = new PlotOptionsLineEvents
                        {
                            Click = click
                        }
                    },
                    Series = series
                });
            }
            else if (ChartType == ChartTypes.Area)
            {
                Chart.SetPlotOptions(new PlotOptions
                {
                    Area = new PlotOptionsArea
                    {
                        AllowPointSelect = EnableReportDrillDown,
                        DataLabels = new PlotOptionsAreaDataLabels { Enabled = this.EnableDataLabels },
                        Cursor = cursor,
                        Events = new PlotOptionsAreaEvents
                        {
                            Click = click
                        }
                    },
                    Series = series
                });
            }
            else if (ChartType == ChartTypes.Spline)
            {
                Chart.SetPlotOptions(new PlotOptions
                {
                    Spline = new PlotOptionsSpline
                    {
                        AllowPointSelect = EnableReportDrillDown,
                        DataLabels = new PlotOptionsSplineDataLabels { Enabled = this.EnableDataLabels },
                        Cursor = cursor,
                        Events = new PlotOptionsSplineEvents
                        {
                            Click = click
                        }
                    },
                    Series = series
                });
            }
        }

        public AxisTypes AxisType { get; set; }
        public bool EnableDataLabels { get; set; }
        public bool EnableReportDrillDown { get; set; }
        public string OverrideDrillFunction { get; set; }
    }
}