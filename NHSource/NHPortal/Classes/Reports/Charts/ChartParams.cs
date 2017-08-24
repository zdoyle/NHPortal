using GD.Highcharts.Enums;
using System;
using System.Collections.Generic;

namespace NHPortal.Classes.Charts
{
    /// <summary>Defines parametrs for building a chart.</summary>
    public class ChartParams
    {
        public ChartParams()
        {
            DrillDown = new DrillDown();
        }

        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string StartDateText { get; set; }
        public string EndDateText { get; set; }
        public ChartTypes ChartType { get; set; }
        public ChartDimention ChartDimention { get; set; }
        public GroupByData GroupByData { get; set; }
        public DrillDown DrillDown { get; set; }
        public string SelectedSeries { get; set; }
        public string SelectedSeriesName { get; set; }
        public string SelectedPoint { get; set; }
        public string TableType { get; set; }
    }

    /// <summary>Defines meta data for a chart.</summary>
    public class ChartMetaData
    {
        public ChartMetaData(string key, string text, string value)
        {
            Key = key;
            Text = text;
            Value = value;
        }

        public ChartMetaData(string key, string text)
        {
            Key = key;
            Text = text;
            Value = text;
        }
        
        public string Key { get; set; }
        public string Text { get; set; }
        public string Value { get; set; }
    }

    /// <summary>Defines information about drilling down into a report.</summary>
    public class DrillDown
    {
        public DrillDown()
        {
            DrillLevel = 0;
            DrillLevels = new NHDrillLevel[3];
        }
        public int DrillLevel { get; set; }
        public NHDrillLevel[] DrillLevels { get; set; }

        internal void ClearDrillLevels()
        {
            DrillLevel = 0;
            foreach (NHDrillLevel dl in DrillLevels)
            {
                if (dl != null)
                {
                    dl.DrillValue = String.Empty;
                    dl.Selected = false;
                } 
            }  
        }
    }

    /// <summary>Containes information about a specific drill level.</summary>
    public class NHDrillLevel
    {
        public NHDrillLevel()
        {
            DrillValue = String.Empty;
        }
        public NHDrillLevel(NHDrillType drillType)
        {
            this.DrillType = drillType;
            DrillValue = String.Empty;
        }
        public NHDrillType DrillType{ get; set; }
        public string DrillValue { get; set; }
        public bool Selected { get; set; }
    }

    /// <summary>Stores info about different groupby types.</summary>
    public static class GroupByMaster
    {
        static GroupByMaster()
        {
            Yearly = new GroupByData(DataGrouping.Yearly, "YYYY", "%Y", "0101", "YYYY", "Year", 29030400000);
            Monthly = new GroupByData(DataGrouping.Monthly, "YYYYMM", "%b %Y", "01", "YYYY-MM", "Month", 2419200000);
            Daily = new GroupByData(DataGrouping.Daily, "YYYYMMDD", "%b %e, %Y", String.Empty, "YYYY-MM-DD", "Day", 86400000);
            all = new List<GroupByData>();
            all.Add(Yearly);
            all.Add(Monthly);
            all.Add(Daily);
        }

        /// <summary>Finds specific group by information.</summary>
        public static GroupByData Find(DataGrouping dataGrouping)
        {
            GroupByData groupByData = null;

            foreach (GroupByData gbd in all)
            {
                if (gbd.DataGrouping == dataGrouping)
                {
                    groupByData = gbd;
                    break;
                }
            }

            return groupByData;
        }

        private static List<GroupByData> all;
        public static GroupByData[] All
        {
            get
            {
                return all.ToArray();
            }
        }

        public static GroupByData Yearly { get; set; }
        public static GroupByData Monthly { get; set; }
        public static GroupByData Daily { get; set; }
    }

    /// <summary>Defines data relating to building and formatting sql or building charts.</summary>
    public class GroupByData
    {
        public GroupByData(DataGrouping dataGroup, string sqlGroupBy, string dateFormt, string dateMsk, string sqlMask, string grpLbl, double interval)
        {
            DataGrouping = dataGroup;
            SqlDataGroup = sqlGroupBy;
            DateFormat = dateFormt;
            DateMask = dateMsk;
            SQLDateMask = sqlMask;
            GroupLabel = grpLbl;
            TickInterval = interval;
        }

        public DataGrouping DataGrouping { get; set; }
        public string SqlDataGroup { get; set; }
        public string DateFormat { get; set; }
        public string DateMask { get; set; }
        public string SQLDateMask { get; set; }
        public string GroupLabel { get; set; }
        public double TickInterval { get; set; }
    }
}