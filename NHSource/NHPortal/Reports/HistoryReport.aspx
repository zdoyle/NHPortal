<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HistoryReport.aspx.cs" Inherits="NHPortal.Reports.HistoryReport"
    MasterPageFile="~/MasterPages/ReportMaster.master" Title="Gordon-Darby History Report" %>

<%@ MasterType VirtualPath="~/MasterPages/ReportMaster.master" %>
<%@ Register Src="~/UserControls/DatePeriodSelector.ascx" TagPrefix="GD" TagName="DatePeriodSelector" %>
<%@ Register Src="~/UserControls/PortalMenu.ascx" TagPrefix="GD" TagName="PortalMenu" %>
<%@ Register Assembly="NHPortal" Namespace="NHPortal.Classes.WebControls" TagPrefix="GD" %>

<asp:Content runat="server" ID="HistoryReport" ContentPlaceHolderID="ReportContent">
    <div class="report-criteria padded centered-text" id="reportContent">
        <GD:DatePeriodSelector runat="server" ID="dateSelector"></GD:DatePeriodSelector>
        <br />
        <br />
        <label class="report-criteria__label">Inquiry Type</label>
        <GD:InquiryTypeDropDown runat="server" ID="lstInquiryType" OnChange="inquiryChange, changeValue "></GD:InquiryTypeDropDown>
        <label class="report-criteria__label">ID/OLN</label>
        <asp:TextBox runat="server" ID="tbInquiry" MaxLength="10" />

        <asp:RequiredFieldValidator ID="tbInquiryValidator" runat="server"
            ErrorMessage="Please enter an ID number. "
            ControlToValidate="tbInquiry" Display="Dynamic"
            CssClass="small error" SetFocusOnError="True" />
    </div>

    <div id="div-overlay" class="hidden overlay">
        <div id="div-details-report-popup" class="smallOverlay_height">
            <div id="popup-content" class="expandedDetails">
                <div id="popup" class="overlaystyle">
                    <asp:Literal runat="server" ID="ltrlPopUpRpt"></asp:Literal>
                    <br />
                    <input id="btn" type="button" value="Close" onclick="javascript: hideOverlay();" />
                </div>
            </div>
        </div>
    </div>

    <asp:HiddenField runat="server" ID="hidDetailID" />
    <asp:HiddenField runat="server" ID="hidInspectorID" />
    <asp:HiddenField runat="server" ID="hidShowOverlay" />
    <asp:HiddenField runat="server" ID="hidActiveDiv" />
    <asp:HiddenField runat="server" ID="hidSelectedIndex"></asp:HiddenField>
    <asp:HiddenField runat="server" ID="hidStationSelectIndex" />
    <asp:HiddenField runat="server" ID="hidstatement"></asp:HiddenField>

    <script type="text/javascript">
        function RunReportSupport() {
            doPostBack('SupportHistory');
        }

        function RunReportBilling() {
            doPostBack('BillingHistory');
        }

        function RunReportBillingDetails(activityNum) {
            document.getElementById("<%=hidSelectedIndex.ClientID%>").value = activityNum;
            doPostBack('BillingHistoryDetails');
        }

        function RunReportCall() {
            doPostBack('CallHistory');
        }

        <%--   function RunReportBillingDetails(activityNum) {
                document.getElementById("<%=hidSelectedIndex.ClientID%>").value = activityNum;
                doPostBack('CallHistoryDetails');
            }--%>

        function RunNotes(activityNum) {
            document.getElementById("<%=hidSelectedIndex.ClientID%>").value = activityNum;
            doPostBack('NOTES');
        }

        function RunNotesCall(activityNum) {
            document.getElementById("<%=hidSelectedIndex.ClientID%>").value = activityNum;
            doPostBack('NOTESCALL');
        }

        function RunNotesBilling(activityNum, statementNum) {
            document.getElementById("<%=hidSelectedIndex.ClientID%>").value = activityNum;
            document.getElementById("<%=hidstatement.ClientID%>").value = statementNum;
            doPostBack('NOTESBILLING');
        }

        function RunConsumable(activityNum) {
            console.log(activityNum);
            document.getElementById("<%=hidSelectedIndex.ClientID%>").value = activityNum;
            console.log(document.getElementById("<%=hidSelectedIndex.ClientID%>").value);
            doPostBack('CONSUMABLE');
            //scrollToBottom();
        }

        function scrollToBottom() {
            document.body.scrollTop = document.body.scrollHeight;
        }

        function RunComponent(activityNum) {
            document.getElementById("<%=hidSelectedIndex.ClientID%>").value = activityNum;
            doPostBack('COMPONENT');
        }

        function RunMechanic(inspectorid, stationid) {
            document.getElementById("<%=hidSelectedIndex.ClientID%>").value = inspectorid;
            document.getElementById("<%=hidStationSelectIndex.ClientID%>").value = stationid;
            doPostBack('MECHANIC');
        }
        function RunReportSupportForMech(stationid) {
            document.getElementById("<%=hidSelectedIndex.ClientID%>").value = stationid;
                doPostBack('SUPPORTHISTORYDETAILS');
            }

            window.onsubmit = new function () {
                var showOverlay = document.getElementById("<%=hidShowOverlay.ClientID%>").value;
            if (showOverlay && showOverlay.toUpperCase() === 'TRUE') {
                document.getElementById("div-overlay").classList.remove('hidden');
                document.getElementById("<%=hidShowOverlay.ClientID%>").value = '';
            }
        }

        function hideOverlay() {
            document.getElementById("div-overlay").classList.add('hidden');
        }

        function closeNote(divID) {
            document.getElementById('divOverlay').style.display = "none";
            document.getElementById(divID).style.display = "none";
        }

        function changeValue() {
            var select = document.getElementById('<%=lstInquiryType.ClientID%>');
            if (select.options[select.selectedIndex].value == 'R') {
                document.getElementById('<%=btnCallHistory.ClientID%>').style.display = "none";
                document.getElementById('<%=btnBillingHistory.ClientID%>').style.display = "none";
                document.getElementById('<%=btnSupportHistory.ClientID%>').style = "display: block; margin: 0 auto;";
            }
            else {
                document.getElementById('<%=btnCallHistory.ClientID%>').style.display = "inline";
                document.getElementById('<%=btnCallHistory.ClientID%>').style.visibility = "true";
                document.getElementById('<%=btnBillingHistory.ClientID%>').style.display = "inline";
                document.getElementById('<%=btnSupportHistory.ClientID%>').style = "inline";
            }
        }

        function pageLoad() {
            changeValue();
        }
    </script>
</asp:Content>

<asp:Content runat="server" ID="btnContent" ContentPlaceHolderID="ExtraButtonsPlaceholder">
    <asp:Button runat="server" ID="btnSupportHistory" CssClass="rpt-button" Text="Support History" OnClientClick="javascript: RunReportSupport();" />
    <asp:Button runat="server" ID="btnCallHistory" CssClass="rpt-button" Text="Call History" OnClientClick="javascript: RunReportCall();" Style="display: inline-block;" />
    <asp:Button runat="server" ID="btnBillingHistory" CssClass="rpt-button" Text="Billing History" OnClientClick="javascript: RunReportBilling();" Style="display: inline-block;" />
</asp:Content>

<asp:Content runat="server" ID="ReportExtraContent" ContentPlaceHolderID="ReportExtraContent">

    <asp:Literal runat="server" ID="ltrlSupportDetailsRpt"></asp:Literal>

    <style type="text/css">
        .smallOverlay_height {
            display: inline-block;
            max-width: 60%;
            max-height: 60%;
            overflow-y: auto;
            position: fixed;
            margin: auto;
            top: 0;
            bottom: 0;
            left: 0;
            right: 0;
            text-align: center;
            font-size: 12px;
        }

        .expandedDetails .table_header th, .expandedDetails .table_header tr {
            background-color: #acacac;
            font-size: 16px;
        }

        .expandedDetails tr:nth-of-type(2n+1) {
            background-color: #F2F2F6;
            font-family: Arial;
            font-size: 14px;
        }

        .expandedDetails tr:nth-of-type(2n) {
            background-color: #FFFFFF;
            color: #000;
            font-size: 14px;
        }

        .links {
            color: blue;
        }
    </style>
</asp:Content>
