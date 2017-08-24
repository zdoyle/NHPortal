<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RescheduledAudits.aspx.cs" Inherits="NHPortal.Reports.RescheduledAudits"
    MasterPageFile="~/MasterPages/ReportMaster.master" Title="Gordon-Darby Rescheduled Audit Report" %>

<%@ MasterType VirtualPath="~/MasterPages/ReportMaster.master" %>
<%@ Register Src="~/UserControls/DatePeriodSelector.ascx" TagPrefix="GD" TagName="DatePeriodSelector" %>
<%@ Register Assembly="NHPortal" Namespace="NHPortal.Classes.WebControls" TagPrefix="GD" %>

<asp:Content runat="server" ID="rescheduledAuditsReports" ContentPlaceHolderID="ReportContent">

    <div class="report-criteria padded centered-text">
        <span class="rescheduled-criteria-top">
            <label class="report-criteria__label">Inquiry Type</label>
            <%--<GD:InquiryTypeDropDown runat="server" ID="lstInquiryType" OnSelectedIndexChanged="btn_SelectedIndexChanged" AutoPostBack="true"></GD:InquiryTypeDropDown>--%>
            <GD:InquiryTypeDropDown runat="server" ID="lstInquiryType" OnChange="inquiryChange"></GD:InquiryTypeDropDown>

            <label class="report-criteria__label">ID/OLN</label>
            <asp:TextBox runat="server" ID="tbInquiry" Width="100" MaxLength="10" />
            <asp:RequiredFieldValidator
                runat="server"
                ID="OLNRequiredField"
                ControlToValidate="tbInquiry"
                ErrorMessage="* ID/OLN must be entered."
                CssClass="small error" />
        </span>
        <br />
        <br />
        <GD:DatePeriodSelector runat="server" ID="dateSelector"></GD:DatePeriodSelector>

        <div id="div-overlay" class="hidden overlay">
            <div id="div-details-report-popup" class="largeOverlay_height">
                 <div id="popup-content" class="expandedDetails">
                    <div id="popup" class="overlaystyle">
                <asp:Literal runat="server" ID="ltrlDetailsRpt"></asp:Literal>
                <br />
                <input type="button" value="Close" onclick="javascript: hideOverlay();" />
            </div>
        </div>
        </div>
            </div>
        <asp:HiddenField runat="server" ID="hidSelectedIndex"></asp:HiddenField>
        <asp:HiddenField runat="server" ID="hidShowOverlay" />


        <script type="text/javascript">
            function selectRow(idx) {
                document.getElementById("<%=hidSelectedIndex.ClientID%>").value = idx;
                doPostBack('SELECT_ROW');
            }

            window.onload = new function () {
                var showOverlay = document.getElementById("<%=hidShowOverlay.ClientID%>").value;
                if (showOverlay && showOverlay.toUpperCase() === 'TRUE') {
                    document.getElementById("div-overlay").classList.remove('hidden');
                    document.getElementById("<%=hidShowOverlay.ClientID%>").value = '';
                }
            }

            function hideOverlay() {
                document.getElementById("div-overlay").classList.add('hidden');
            }
        </script>
    </div>

    <asp:Literal runat="server" ID="ltrlReport"></asp:Literal>

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
    </style>

</asp:Content>
<asp:Content runat="server" ID="ReportExtraContent" ContentPlaceHolderID="ReportExtraContent">
</asp:Content>




