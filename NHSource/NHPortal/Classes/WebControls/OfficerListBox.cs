using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using PortalFramework.ReportModel;
using NHPortal.Classes;
using NHPortal.Classes.User;
using System.Data;

namespace NHPortal.Classes.WebControls
{
    /// <summary>ListBox control for vehicle types.</summary>
    [System.Web.UI.ToolboxData("<{0}:OfficerListBox runat='server'></{0}:OfficerListBox>")]
    public class OfficerListBox : PortalFramework.WebControls.ReportListBox
    {
        /// <summary>Instantiates a new instance of the OfficerListBox class.</summary>
        public OfficerListBox()
        {
            ColumnName = "auditor_id";
            ValueType = PortalFramework.WebControls.ListBoxValueType.Alpha;
        }
        private string BuildQuery()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("SELECT distinct ");
            sb.AppendLine("SUBSTR(AUDITORID,7,10) AS AUD_ID");
            sb.AppendLine("FROM STATIONAUDITS");
            sb.AppendLine("WHERE (AUDITORID > 'AD00000000') ");
            sb.AppendLine("AND (AUDITORID < 'AD99999999')");
            sb.AppendLine("AND AUDITORID NOT IN ('AD0000JOSH','AD000BARRY') ");
            sb.AppendLine("ORDER BY SUBSTR(AUDITORID,7,10)");

            return sb.ToString();

        }
        private void RunReport()
        {
            string qry = BuildQuery();
            GDDatabaseClient.Oracle.OracleResponse response = PortalFramework.Database.ODAP.GetDataTable(qry, PortalFramework.Database.DatabaseTarget.Adhoc);
        }
        /// <summary>Initializes the items in the ListBox.</summary>
        public override void Initialize()
        {
            string qry = BuildQuery();
            GDDatabaseClient.Oracle.OracleResponse response = PortalFramework.Database.ODAP.GetDataTable(qry, PortalFramework.Database.DatabaseTarget.Adhoc);

            this.Items.Clear();
            AddItem("All", String.Empty);
            string officerId;
            //foreach (var row in response.ResultsTable.Rows)
            //{
                string[] results = new string[response.ResultsTable.Rows.Count];
                for (int i = 0; i < response.ResultsTable.Rows.Count; i++)
                {
                    officerId = response.ResultsTable.Rows[i]["AUD_ID"].ToString();
                    AddItem(officerId, officerId);
                }

            //}

            //["AUD_ID"].ToString()
            //AddItem(response.ResultsRow.ToString(), response.ResultsRow.ToString());


            if (Items.Count > 0)
            {
                SelectedIndex = 0;
            }
        }



    }
}

//AddItem("0008", "0008");
//AddItem("0011", "0011");
//AddItem("0013", "0013");
//AddItem("0017", "0017");
//AddItem("0021", "0021");
//AddItem("0028", "0028");
//AddItem("0029", "0029");
//AddItem("0035", "0035");
//AddItem("0038", "0038");
//AddItem("0046", "0046");
//AddItem("0055", "0055");
//AddItem("0059", "0059");
//AddItem("0063", "0063");
//AddItem("0066", "0066");
//AddItem("0067", "0067");
//AddItem("0069", "0069");
//AddItem("0070", "0070");
//AddItem("0071", "0071");
//AddItem("0072", "0072");
//AddItem("0073", "0073");
//AddItem("0074", "0074");
//AddItem("0079", "0079");
//AddItem("0080", "0080");
//AddItem("0081", "0081");
//AddItem("0082", "0082");
//AddItem("0083", "0083");
//AddItem("0084", "0084");
//AddItem("0085", "0085");
//AddItem("0086", "0086");
//AddItem("0087", "0087");
//AddItem("0092", "0092");
//AddItem("0094", "0094");
//AddItem("0095", "0095");
//AddItem("0097", "0097");
//AddItem("0098", "0098");
//AddItem("0099", "0099");
//AddItem("0101", "0101");
//AddItem("0814", "0814");
//AddItem("0885", "0885");
//AddItem("0896", "0896");
//AddItem("0945", "0945");
//AddItem("0995", "0995");
//AddItem("1026", "1026");
//AddItem("1032", "1032");
//AddItem("1035", "1035");
//AddItem("1038", "1038");
//AddItem("1049", "1049");
//AddItem("1050", "1050");
//AddItem("1054", "1054");
//AddItem("1056", "1056");
//AddItem("1060", "1060");
//AddItem("1061", "1061");
//AddItem("1062", "1062");
//AddItem("1065", "1065");
//AddItem("1070", "1070");
//AddItem("1075", "1075");
//AddItem("1077", "1077");
//AddItem("1078", "1078");
//AddItem("1079", "1079");
//AddItem("1080", "1080");
//AddItem("1082", "1082");
//AddItem("1083", "1083");
//AddItem("1084", "1084");
//AddItem("1085", "1085");
//AddItem("1086", "1086");
//AddItem("1088", "1088");
//AddItem("1089", "1089");
//AddItem("1091", "1091");
//AddItem("1093", "1093");
//AddItem("1094", "1094");
//AddItem("1095", "1095");
//AddItem("1096", "1096");
//AddItem("1097", "1097");
//AddItem("1098", "1098");
//AddItem("1099", "1099");
//AddItem("1171", "1171");
//AddItem("1173", "1173");
//AddItem("C478", "C478");
//AddItem("C479", "C479");
//AddItem("C480", "C480");
//AddItem("C484", "C484");
//AddItem("C485", "C485");
//AddItem("C486", "C486");
//AddItem("C488", "C488");
//AddItem("C490", "C490");
//AddItem("C551", "C551");
//AddItem("C552", "C552");
//AddItem("UDIT", "UDIT");

