using GDCoreUtilities;
using GDCoreUtilities.Logging;
using NHPortal.Classes;
using NHPortal.Classes.User;
using Oracle.DataAccess.Client;
using PortalFramework.Database;
using PortalFramework.ReportModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace NHPortal.Reports
{
    public partial class NoFinalOutcomeReport : NHPortal.Classes.PortalPage
    {
        public BaseReport ReportData { get; set; }
       public string searchDate { get; set; }
        public string formattedSearchDate { get; set; }
        public string  dateFormat;

        protected void Page_Load(object sender, EventArgs e)
        {
            RedirectOnInvalidPermission(UserPermissions.NoFinalOutcomeReport);

            if (IsPostBack)
            {
                ProcessPostBack();
            }
            else
            {
                InitializePage();

                LoadFavorite();
                //RunReport();
            }       
        }
        private void InitializePage()
        {
            Master.SetHeaderText("No Final Outcome Report");
            dpStart.Text = DateTime.Now.ToString("M/d/yyyy");
                      
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

        private void ConvertDate()
        {
            string formatdate = dpStart.Text;
            if (NHPortal.Classes.NHPortalUtilities.TryConvertDateTimeFormat(formatdate, "M/d/yyyy", "yyyyMMdd", out formatdate))
            {
                dateFormat = formatdate;
            }
        }
        private void SaveFavorite()
        {
            Master.SaveFavorite(NHPortal.UserControls.RedirectCodes.REPORT_NO_FINAL_OUTCOME, UserFavoriteTypes.Report);
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
                        case "DATE":
                            this.dpStart.Text = c.Value;
                            break;
                        default:
                            break;
                    }
                }
            }
        }
       
        private void RunReport()
        {
            Master.UserReport = new Report("NFO Report");
            ConvertDate();
            SetReportFromFile();
            SetMetaData();
            Master.RenderReportToPage();
            SessionHelper.SetCurrentReport(this.Session, Master.UserReport);

        }
      

        private void SetReportFromFile()
        {
            string file = System.IO.Path.Combine(PortalFramework.PortalIniSettings.Settings.GetValue("DIR_NO_FINAL_OUTCOME"),
                "NFOREPORT_" + dateFormat + ".csv");

            NHPortalUtilities.LogSessionMessage("NFO Report File [" + file + "]", GDCoreUtilities.Logging.LogSeverity.Debug);

            try
            {
                GDCoreUtilities.IO.CSVReader csvReader = new GDCoreUtilities.IO.CSVReader(file);
                DataTable dt = csvReader.ToDataTable();

                if (dt != null)
                {
                    Master.UserReport = new PortalFramework.ReportModel.Report("No Final Outcome Report", dt);
                    Master.UserReport.Sortable = true;
                    //FormatCSVReportHeaders();
                    //Master.RenderReportToPage();
                }
            }
            catch (System.IO.IOException ex)
            {
                NHPortalUtilities.LogSessionException(ex, "There was a problem reading the Sticker Denial file from disk.");
                //FileLogger Logger = SessionHelper.GetSessionLogger(System.Web.HttpContext.Current.Session);
                //Logger.Log("There was a problem reading the Sticker Denial file from disk." + Environment.NewLine + ex.Message, LogSeverity.Error);
            }
        }
        private Oracle.DataAccess.Client.OracleParameter[] GetOracleParams()
        {
            List<OracleParameter> paramList = new List<OracleParameter>();


            paramList.Add(new OracleParameter("RECORD_DATA", OracleDbType.RefCursor, ParameterDirection.Output));

            return paramList.ToArray();
        }

        private void FormatCSVReportHeaders()
        {
            //foreach (ReportHeaderCell rhc in Master.UserReport.HeaderRow)
            foreach (ReportColumn rhc in Master.UserReport.Columns)
            {
                KeyValuePair<string, ReportColumnInfo> headerEntry = ReportData.ReportColumnData.FirstOrDefault(x => x.Key == rhc.Text);

                if (headerEntry.Value != null)
                {
                    rhc.Text = headerEntry.Value.DisplayName;
                }
            }
        }

        //private string BuildQuery()
        //{
        //    StringBuilder sb = new StringBuilder();
        //    sb.AppendLine("SELECT ");
        //    sb.AppendLine("VIN, TAG, TESTDATE, TESTENDTIME, REGEXPIRATION, ");
        //    sb.AppendLine("NFOOBD, NFOSAF, NFOVIS, ");
        //    sb.AppendLine("TESTASMAKE, TESTASMODEL, TESTASMODELYR, STATIONID, ");
        //    sb.AppendLine("INSPECTORID,OVERALLPF, REGDATE,");
        //    sb.AppendLine("SAFETYTESTTYPE,  OBDPF, SAFETYPF, VISUALPF,");
        //    sb.AppendLine("OBDMILCMDON, OBDMILENGON, VISAIRPUMP ");
        //    sb.AppendLine(" VISPCV, VISEVAP, VISFUEL, VISCAT,  ");
        //    sb.AppendLine(" SAF_BS_VEHINFO, ");
        //    sb.AppendLine("SAF_BS_WHEELS, SAF_BS_TIRES, SAF_BS_STEERING, SAF_BS_FOOTBRAKE, SAF_BS_PARKINGBRAKE, ");
        //    sb.AppendLine(" SAF_BS_INST, SAF_BS_HORN, SAF_BS_REARLIGHTS,");
        //    sb.AppendLine(" SAF_BS_STOPLIGHTS, SAF_BS_FRONTLIGHTS, SAF_BS_SIGNAL, ");
        //    sb.AppendLine("SAF_BS_OTHERLIGHTS, SAF_BS_HEADLIGHTAIM, SAF_BS_MIRRORS, ");
        //    sb.AppendLine("SAF_BS_DEFROSTER, SAF_BS_GLASS, SAF_BS_WIPERS, ");
        //    sb.AppendLine("SAF_BS_EXHAUST, SAF_BS_FUELSYS, SAF_BS_BUMPERS, ");
        //    sb.AppendLine(" SAF_BS_BODY, ");
        //    sb.AppendLine("SAF_TB_VEHINFO, SAF_TB_WHEELS, SAF_TB_TIRES, SAF_TB_STEERING, ");
        //    sb.AppendLine(" SAF_TB_FOOTBRAKE, SAF_TB_PARKINGBRAKE, SAF_TB_AIRBRAKE, ");
        //    sb.AppendLine(" SAF_TB_INST, SAF_TB_HORN, SAF_TB_REARLIGHTS, ");
        //    sb.AppendLine(" SAF_TB_STOPLIGHTS, SAF_TB_FRONTLIGHTS, SAF_TB_SIGNAL, ");
        //    sb.AppendLine(" SAF_TB_OTHERLIGHTS, SAF_TB_HEADLIGHTAIM, SAF_TB_MIRRORS, ");
        //    sb.AppendLine(" SAF_TB_DEFROSTER, SAF_TB_GLASS, SAF_TB_WIPERS, ");
        //    sb.AppendLine(" SAF_TB_REFLECTOR, SAF_TB_FIREEXT, SAF_TB_EXHAUST, ");
        //    sb.AppendLine(" SAF_TB_FUELSYS, SAF_TB_BUMPERS, SAF_TB_BODY, ");
        //    sb.AppendLine("SAF_TB_BUSBODY, SAF_TB_BUSINTERIOR ");
        //    sb.AppendLine("SAF_TR_VEHINFO, SAF_TR_TIRES, SAF_TR_MAINBRAKES, SAF_TR_PARKINGBRAKE, ");
        //    sb.AppendLine(" SAF_TRAIEER_EBREAKS, SAF_TR_BREAKWIRING, SAF_TR_REARLIGHTS, ");
        //    sb.AppendLine("SAF_TR_STOPLIGHTS, SAF_TR_BUMPER, SAF_TR_BODY,");

        //    sb.AppendLine("SAF_AG_VIN, SAF_AG_BRAKES, SAF_AG_STEERING, SAF_AG_STOPLIGHTS ");
        //    sb.AppendLine("SAF_AG_EXHAUST, SAF_AG_HEADLIGHTS, SAF_AG_REFLECTORS, SAF_AG_TAILLIGHTS,");

        //    sb.AppendLine(" SAF_MC_VEHINFO, SAF_MC_WHEELS, SAF_MC_TIRES, ");
        //    sb.AppendLine("SAF_MC_STEERING, SAF_MC_FOOTBRAKE, SAF_MC_INST, ");
        //    sb.AppendLine("SAF_MC_HORN, SAF_MC_REARLIGHTS, SAF_MC_STOPLIGHTS, ");
        //    sb.AppendLine("SAF_MC_FRONTLIGHTS, SAF_MC_SIGNAL, SAF_MC_OTHERLIGHTS, ");
        //    sb.AppendLine("SAF_MC_HEADLIGHTAIM, SAF_MC_MIRRORS, SAF_MC_GLASS, ");
        //    sb.AppendLine("SAF_MC_EXHAUST, SAF_MC_FUELSYS, SAF_MC_BODY");

        //    sb.AppendLine(" FROM NH_MV_NFO");
        //    sb.AppendLine(" order by ");
        //    sb.AppendLine("SAFETYTESTTYPE asc, TESTDATE desc, TESTENDTIME desc");
        //    return sb.ToString();
        //}


        //        private ReportTable InfoToReportTable(DataTable dt)
        //        {


        //            ReportTable tbl = new ReportTable();
        //            tbl.RenderLocation = RenderLocation.Above;
        //            tbl.Columns.Add("VIN", "VIN", ColumnDataType.String);
        //            tbl.Columns.Add("TAG", "TAG", ColumnDataType.String);
        //            tbl.Columns.Add("TESTDATE", "TESTDATE", ColumnDataType.String);
        //            tbl.Columns.Add("TESTENDTIME", "TESTTIME", ColumnDataType.String);
        //            tbl.Columns.Add("REGEXPIRATION", "REGEXPIRATION", ColumnDataType.String);
        //            tbl.Columns.Add("NFOOBD", "OBDNFO", ColumnDataType.String);
        //            tbl.Columns.Add("NFOSAF", "SAFETYNFO", ColumnDataType.String);
        //            tbl.Columns.Add("NFOVIS", "VISUALNFO", ColumnDataType.String);
        //            tbl.Columns.Add("TESTASMAKE", "MAKE", ColumnDataType.String);
        //            tbl.Columns.Add("TESTASMODEL", "MODEL", ColumnDataType.String);
        //            tbl.Columns.Add("TESTASMODELYR", "MODELYR", ColumnDataType.String);
        //            tbl.Columns.Add("STATIONID", "STATIONID", ColumnDataType.String);
        //            tbl.Columns.Add("INSPECTORID", "MECHANICID", ColumnDataType.String);
        //            tbl.Columns.Add("OVERALLPF", "OVERALL_PF", ColumnDataType.String);
        //            tbl.Columns.Add("REGDATE", "REGDATE", ColumnDataType.String);
        //            tbl.Columns.Add("SAFETYTESTTYPE", "SAFETYTESTTYPE", ColumnDataType.String);
        //            tbl.Columns.Add("OBDPF", "OBDPF", ColumnDataType.String);
        //            tbl.Columns.Add("SAFETYPF", "SAFETYPF", ColumnDataType.String);
        //            tbl.Columns.Add("VISUALPF", "VISUALPF", ColumnDataType.String);
        //            tbl.Columns.Add("NFOSAF", "SAFETYNFO", ColumnDataType.String);
        //            tbl.Columns.Add("NFOSAF", "SAFETYNFO", ColumnDataType.String);

        //            tbl.Columns.Add("NFOSAF", "SAFETYNFO", ColumnDataType.String);
        //            tbl.Columns.Add("NFOSAF", "SAFETYNFO", ColumnDataType.String);
        //            tbl.Columns.Add("NFOSAF", "SAFETYNFO", ColumnDataType.String);
        //            tbl.Columns.Add("NFOSAF", "SAFETYNFO", ColumnDataType.String);


        //            foreach (DataRow rows in dt.Rows)
        //            {
        //                ReportRow rptRow = new ReportRow(tbl);
        //                rptRow.Cells.Add(rows["Component"].ToString().Trim());
        //                rptRow.Cells.Add(rows["OLD_SERIAL"].ToString().Trim());
        //                rptRow.Cells.Add(rows["NEW_SERIAL"].ToString().Trim());
        //                rptRow.Cells.Add(rows["SHIPTRACKINGNUM"].ToString().Trim());
        //                rptRow.Cells.Add(rows["RETURNTRACKINGNUM"].ToString().Trim());
        //                tbl.Rows.Add(rptRow);
        //            }
        //            return tbl;
        //        }


        //private void SetColumnTypes()
        //{
        //    for (int i = 1; i < Master.UserReport.ColumnCount; i++)
        //    {
        //        Master.UserReport.Columns[i].ColumnDataType = ColumnDataType.Number;
        //    }
        //}

        private void SetMetaData()
        {
            if (Master.UserReport != null)
            {
                Master.UserReport.MetaData.Add("Date", dpStart.Text);

            }
        }
    }
    
}