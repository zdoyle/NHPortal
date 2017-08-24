using NHPortal.Classes;
using NHPortal.Classes.User;
using PortalFramework.Database;
using PortalFramework.ReportModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;
using System.Web.UI.HtmlControls;
using System.Web.Mvc;
using GDCoreUtilities.Logging;

namespace NHPortal
{
    public partial class InqSpectionByPlate : NHPortal.Classes.PortalPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RedirectOnInvalidPermission(UserPermissions.InspectionInquiryByPlate);
            tbPlateNumber.Text = tbPlateNumber.Text.ToUpper().Trim();
            if (IsPostBack)
            {
                LogMessage("IsPostBack - Successful", LogSeverity.Information);
                ProcessPostBack();
            }
            else
            {
                LogMessage("IsPostBack - Else", LogSeverity.Information);
                InitializePage();
                LoadFavorite();
            }
        }

        private void InitializePage()
        {
            Master.SetHeaderText("Inspection History by Plate");

            ddlPlateType.Items.Add("All");
            ddlPlateType.Items.Add("All Moto");
            ddlPlateType.Items.Add("Active Duty Military");
            ddlPlateType.Items.Add("Agriculture");
            ddlPlateType.Items.Add("Antique");
            ddlPlateType.Items.Add("Antique MC");
            ddlPlateType.Items.Add("Apportioned");
            ddlPlateType.Items.Add("Commercial");
            ddlPlateType.Items.Add("Construction Eqpt");
            ddlPlateType.Items.Add("Disabled Veteran");
            ddlPlateType.Items.Add("Farm Vehicle");
            ddlPlateType.Items.Add("Former POW");
            ddlPlateType.Items.Add("Goverment");
            ddlPlateType.Items.Add("Handicap");
            ddlPlateType.Items.Add("Handicap MC");
            ddlPlateType.Items.Add("Hearse");
            ddlPlateType.Items.Add("Motorcycle");
            ddlPlateType.Items.Add("Moose");
            ddlPlateType.Items.Add("Moped");
            ddlPlateType.Items.Add("NH National Guard");
            ddlPlateType.Items.Add("Passenger");
            ddlPlateType.Items.Add("Pearl Habor");
            ddlPlateType.Items.Add("Purple Heart");
            ddlPlateType.Items.Add("Purple Heart MC");
            ddlPlateType.Items.Add("School Bus");
            ddlPlateType.Items.Add("Special Commercial");
            ddlPlateType.Items.Add("State Park");
            ddlPlateType.Items.Add("State Park - Moose");
            ddlPlateType.Items.Add("Street Rod");
            ddlPlateType.Items.Add("Temp");
            ddlPlateType.Items.Add("Tractor");
            ddlPlateType.Items.Add("Trailer");
            ddlPlateType.Items.Add("Veteran");
            ddlPlateType.Items.Add("Veteran MC");

            // Set Passenger as default dropdown value
            ddlPlateType.Items.FindByText("Passenger").Selected = true;
        }

        private void ProcessPostBack()
        {
            string action = Master.MasterPage.HidActionValue;
            switch (action)
            {
                case "RUN_REPORT":
                    RunReport();
                    break;

                case "SELECT_ROW":
                    RunDetailsReport();
                    break;

                case "SAVE_FAVORITE":
                    SaveFavorite();
                    break;
            }
        }

        private void SetMetaData()
        {
            if (Master.UserReport != null)
            {
                Master.UserReport.MetaData.Add("Plate Number", tbPlateNumber.Text);
                Master.UserReport.MetaData.Add("Plate Type", ddlPlateType.Text);
            }
        }

        private void RunReport()
        {
            string qry = BuildQuery();
            GDDatabaseClient.Oracle.OracleResponse response = ODAP.GetDataTable(qry, DatabaseTarget.OLTP);
            if (response.Successful)
            {
                LogMessage("RunReport - Response Successful", LogSeverity.Information);
                Master.UserReport = new Report("Inspection History by Plate", response.ResultsTable);
                SetMetaData();
                Master.SetDataLastUpdate(DateTime.Now);
                if (response.HasResults)
                {
                    LogMessage("RunReport - HasResults", LogSeverity.Information);
                    Master.UserReport.Columns.Insert("Details", "Select", 0);
                    AddDetailSelectColumn();
                    FormatPlateInspectionHistory();
                    SetColumnTypes();
                    Master.UserReport.Sortable = true;
                    Master.UserReport.FooterVisibility = FooterVisibility.Auto;
                }
            }
            else
            {
                LogMessage("RunReport - Unsuccessful", LogSeverity.Warning);
                Master.UserReport = null;
                Master.SetError(response.Exception);
            }

            LogOracleResponse(response);
            Master.RenderReportToPage();
            SessionHelper.SetCurrentReport(this.Session, Master.UserReport);
        }

        private void RunDetailsReport()
        {
            string qry = BuildDetailsQuery();
            GDDatabaseClient.Oracle.OracleResponse response = PortalFramework.Database.ODAP.GetDataRow(qry, PortalFramework.Database.DatabaseTarget.OLTP);
            if (response.Successful)
            {
                LogMessage("RunDetailsReport - Response Successful", LogSeverity.Information);
                if (response.HasResults)
                {
                    LogMessage("RunDetailsReport - HasResults", LogSeverity.Information);
                    DataRow row = response.ResultsRow;
                    DataRow rawRow = response.ResultsRow;
                    ltrlDetailsRpt.Text = Classes.Reference.InqInspectionBy.GenerateDetailsTable(row);
                    if ((rawRow["Safety Result"].ToString()[0] == 'C') || (rawRow["Safety Result"].ToString()[0] == 'R'))
                    {
                        string qrySafety = BuildSafetyBreakdownQuery();
                        GDDatabaseClient.Oracle.OracleResponse responseSafety = PortalFramework.Database.ODAP.GetDataRow(qrySafety, PortalFramework.Database.DatabaseTarget.Adhoc);
                        if ((responseSafety.Successful) && (responseSafety.HasResults))
                        {
                            DataRow rowSafety = responseSafety.ResultsRow;
                            ltrlDetailsRpt.Text += Classes.Reference.InqInspectionBy.GenerateSafetyBreakdownTable(rowSafety);
                        }
                    }
                    hidShowOverlay.Value = "TRUE";
                }
            }
        }

        private string BuildQuery()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("   SELECT ");
            sb.AppendLine("          T.TESTDATE || T.TESTENDTIME AS \"Test Date\" ");
            sb.AppendLine(",         T.VIN AS \"VIN\" ");
            sb.AppendLine(",         T.MAKE AS \"Make\" ");
            sb.AppendLine(",         T.MODELYEAR AS \"Model Year\" ");
            sb.AppendLine(",         T.EMISSTESTTYPE AS \"Test Type\" ");
            sb.AppendLine(",         T.REGISTRATIONNUM AS \"Registration Number\" ");
            sb.AppendLine(",         T.REGDATE AS \"Registration Date\" ");
            sb.AppendLine(",         T.OVERALLPF AS \"Overall Result\" ");
            sb.AppendLine("     FROM TESTRECORD T ");
            sb.AppendLine("      WHERE TRIM(SUBSTR(T.REGISTRATIONNUM,0,15)) = TRIM('" + tbPlateNumber.Text + "') ");


            if (ddlPlateType.Text != "All")
            {
                sb.AppendLine("      AND (TRIM(SUBSTR(REGISTRATIONNUM,LENGTH(REGISTRATIONNUM)-4,LENGTH(REGISTRATIONNUM))) IN ");

                if (ddlPlateType.Text == "All Moto") sb.AppendLine(" ('AMOT','HMOTO','IHMOT','IMOTO','MOTO','MOPE','IPMOT','PHMOT','60AMT','60HMT','60IHM','60IMT','60MOT','60PHM','IVMOT','VMOTO')) ");
                if (ddlPlateType.Text == "Active Duty Military") sb.AppendLine(" ('ADAF','IADAF')) ");
                if (ddlPlateType.Text == "Agriculture") sb.AppendLine(" ('AGRI','AMBU')) ");
                if (ddlPlateType.Text == "Antique") sb.AppendLine(" ('ANTI','IANTI')) ");
                if (ddlPlateType.Text == "Antique MC") sb.AppendLine(" ('AMOT')) ");
                if (ddlPlateType.Text == "Apportioned") sb.AppendLine(" ('APRO','IAPRO')) ");
                if (ddlPlateType.Text == "Commercial") sb.AppendLine(" ('COMM','ICOMM')) ");
                if (ddlPlateType.Text == "Construction Eqpt") sb.AppendLine(" ('CONEQ')) ");
                if (ddlPlateType.Text == "Disabled Veteran") sb.AppendLine(" ('DVETE','IDVET')) ");
                if (ddlPlateType.Text == "Farm Vehicle") sb.AppendLine(" ('FARM')) ");
                if (ddlPlateType.Text == "Former POW") sb.AppendLine(" ('FPOW')) ");
                if (ddlPlateType.Text == "Goverment") sb.AppendLine(" ('MPDMT','MUNPD','PADMN','PAGRI','PBANK','PCIVL','PDRED','PEDUC','PERM','PESE','PFISH','PHGWY','PHWEL','PJUDL','PLABR','PLIBR','PLIQR','PMISC','PMOTR','PNATG','PPLOT','PPRIS','PREVN','PSFTY','PSWEP','PTRAL','PUNIV','PYDEV')) ");
                if (ddlPlateType.Text == "Handicap") sb.AppendLine(" ('HCAP','IHCAP')) ");
                if (ddlPlateType.Text == "Handicap MC") sb.AppendLine(" ('HMOTO','IHMOT')) ");
                if (ddlPlateType.Text == "Hearse") sb.AppendLine(" ('HRSE')) ");
                if (ddlPlateType.Text == "Motorcycle") sb.AppendLine(" ('CPASS','ICPAS')) ");
                if (ddlPlateType.Text == "Moose") sb.AppendLine(" ('CPASS','ICPAS')) ");
                if (ddlPlateType.Text == "Moped") sb.AppendLine(" ('MOPE')) ");
                if (ddlPlateType.Text == "NH National Guard") sb.AppendLine(" ('INGNH','NGNH')) ");
                if (ddlPlateType.Text == "Passenger") sb.AppendLine(" ('IPASS','PASS')) ");
                if (ddlPlateType.Text == "Pearl Habor") sb.AppendLine(" ('IPHBR','PHBR')) ");
                if (ddlPlateType.Text == "Purple Heart") sb.AppendLine(" ('INPUR','NPURP')) ");
                if (ddlPlateType.Text == "Purple Heart MC") sb.AppendLine(" ('IPMOT','PHMOT')) ");
                if (ddlPlateType.Text == "School Bus") sb.AppendLine(" ('SBUS')) ");
                if (ddlPlateType.Text == "Special Commercial") sb.AppendLine(" ('SCOMM')) ");
                if (ddlPlateType.Text == "State Park") sb.AppendLine(" ('ISPPS','SPPAS')) ");
                if (ddlPlateType.Text == "State Park - Moose") sb.AppendLine(" ('ISPCP','SPCPS')) ");
                if (ddlPlateType.Text == "Street Rod") sb.AppendLine(" ('ISROD','SROD')) ");
                if (ddlPlateType.Text == "Temp") sb.AppendLine(" ('20DAY','20TM1','20TM2','60ADF','60AGR','60AMB','60AMT','60APT','60BUS','60COM','60FPW','60HCP','60HMT','60HPS','60HRS','60IAD','60ICM','60ICP','60IFP','60IHM','60IHP','60IMT','60IPM','60IPR','60IPS','60IRV','60ISC','60ISP','60ITR','60MOT','60NGH','60PAS','60PHM','60PRM','60RVT','60SCM','60SPC','60SPP','60TRL')) ");
                if (ddlPlateType.Text == "Tractor") sb.AppendLine(" ('IPASS','PASS')) ");
                if (ddlPlateType.Text == "Trailer") sb.AppendLine(" ('ATRAI','TRAI','ITRAI')) ");
                if (ddlPlateType.Text == "Veteran") sb.AppendLine(" ('IVVET','VVETE')) ");
                if (ddlPlateType.Text == "Veteran MC") sb.AppendLine(" ('IVMOT','VMOTO')) ");
            }
            sb.AppendLine(" ORDER BY T.TESTDATE DESC, T.TESTENDTIME DESC ");

            LogMessage("BuildQuery SQL:" + sb.ToString(), LogSeverity.Information);
            return sb.ToString();
        }

        private string BuildDetailsQuery()
        {
            int idx = GDCoreUtilities.NullSafe.ToInt(hidSelectedIndex.Value);
            var row = Master.UserReport.Rows[idx];

            string detailsTag = tbPlateNumber.Text.PadLeft(7);
            string detailsEmisstesttype = row["Test Type"].Value;

            string strDetailsTestDate;
            string strDetailsTestEndTime;

            NHPortal.Classes.NHPortalUtilities.TryConvertDateTimeFormat(row[1].Value, "M/d/yyyy hh:mm:ss tt", "yyyyMMdd", out strDetailsTestDate);
            NHPortal.Classes.NHPortalUtilities.TryConvertDateTimeFormat(row[1].Value, "M/d/yyyy hh:mm:ss tt", "HHmmss", out strDetailsTestEndTime);

            StringBuilder builder = new StringBuilder();

            builder = Classes.Reference.InqInspectionBy.InqInspectionBySQLbyOLTP(builder);
            builder.AppendLine("      WHERE TRIM(SUBSTR(T.REGISTRATIONNUM,0,15)) = TRIM('" + tbPlateNumber.Text + "') ");
            builder.AppendLine("      AND T.TESTDATE = '" + strDetailsTestDate + "' ");
            builder.AppendLine("      AND T.TESTENDTIME = '" + strDetailsTestEndTime + "' ");
            builder.AppendLine(" ORDER BY T.TESTDATE DESC, T.TESTENDTIME DESC");

            LogMessage("BuildDetailsQuery - SQL:" + builder.ToString(), LogSeverity.Information);
            return builder.ToString();
        }

        private string BuildSafetyBreakdownQuery()
        {
            int idx = GDCoreUtilities.NullSafe.ToInt(hidSelectedIndex.Value);
            var row = Master.UserReport.Rows[idx];

            string strDetailsTestDate;
            string strDetailsTestEndTime;

            NHPortal.Classes.NHPortalUtilities.TryConvertDateTimeFormat(row[1].Value, "M/d/yyyy hh:mm:ss tt", "yyyyMMdd", out strDetailsTestDate);
            NHPortal.Classes.NHPortalUtilities.TryConvertDateTimeFormat(row[1].Value, "M/d/yyyy hh:mm:ss tt", "HHmmss", out strDetailsTestEndTime);

            string strVIN = row[2].Value;

            StringBuilder builder = new StringBuilder();
            builder = Classes.Reference.InqInspectionBy.InqInspectionSafetyBreakdownAdhoc(builder);
            builder.AppendLine("    WHERE TRIM(T.VIN) = '" + strVIN + "' ");
            builder.AppendLine("      AND T.TESTDATE = '" + strDetailsTestDate + "' ");
            builder.AppendLine("      AND T.TESTENDTIME = '" + strDetailsTestEndTime + "' ");
            builder.AppendLine(" ORDER BY T.TESTDATE DESC, T.TESTENDTIME DESC");

            LogMessage("BuildSafetyBreakdownQuery SQL:" + builder.ToString(), LogSeverity.Information);
            return builder.ToString();
        }

        private void FormatPlateInspectionHistory()
        {
            LogMessage("FormatPlateInspectionHistory Start", LogSeverity.Information);
            string strDateTimeFormattedOut;
            foreach (ReportRow row in Master.UserReport.Rows)
            {
                if (!String.IsNullOrEmpty(row["Test Date"].Value))
                {
                    NHPortal.Classes.NHPortalUtilities.TryConvertDateTimeFormat(row["Test Date"].Value, "yyyyMMddHHmmss", "M/d/yyyy hh:mm:ss tt", out strDateTimeFormattedOut);
                    row["Test Date"].Value = strDateTimeFormattedOut.ToString();
                }

                if (!String.IsNullOrEmpty(row["Registration Date"].Value))
                {
                    NHPortal.Classes.NHPortalUtilities.TryConvertDateTimeFormat(row["Registration Date"].Value, "yyyyMMdd", "M/d/yyyy", out strDateTimeFormattedOut);
                    row["Registration Date"].Value = strDateTimeFormattedOut.ToString();
                }

                row["Test Type"].Value = Classes.Reference.EmissionTypes.GetDescription(row["Test Type"].Value[0]);
                row["Overall Result"].Value = Classes.Reference.InqInspectionBy.GetOverallResultDescription(row["Overall Result"].Value[0]);
            }
            LogMessage("FormatPlateInspectionHistory End", LogSeverity.Information);
        }

        private void AddDetailSelectColumn()
        {
            int i = 0;
            foreach (ReportRow row in Master.UserReport.Rows)
            {
                row[0].Value = "Select";
                row[0].OnClick = String.Format("javascript: selectRow({0})", i);
                row[0].Href = "#/";
                i += 1;
            }
        }

        private void SaveFavorite()
        {
            Master.SaveFavorite(NHPortal.UserControls.RedirectCodes.REPORT_INQ_INSPECTION_BY_PLATE, UserFavoriteTypes.Report);
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
                        case "PLATE NUMBER":
                            tbPlateNumber.Text = c.Value;
                            break;

                        case "PLATE TYPE":
                            ddlPlateType.Text = c.Value;
                            break;
                    }
                }
            }
        }

        private void SetColumnTypes()
        {
            for (int i = 1; i < Master.UserReport.ColumnCount; i++)
            {
                Master.UserReport.Columns[i].ColumnDataType = ColumnDataType.String;
            }
            Master.UserReport.Columns["Details"].Exportable = false;
        }
    }
}