<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InqMobileTestByVIN.aspx.cs" Inherits="NHPortal.InqMobileTestByVIN"
    MasterPageFile="~/MasterPages/ReportMaster.master" Title="Vehicle Test Inquiry History" %>

<%@ MasterType VirtualPath="~/MasterPages/ReportMaster.master" %>
<%@ Register Src="~/UserControls/DateTextBox.ascx" TagPrefix="GD" TagName="DatePicker" %>

<asp:Content runat="server" ID="vehicleTestInqHistory" ContentPlaceHolderID="ReportContent">

    <div class="report-criteria padded centered-text">
        <label class="report-criteria__label">VIN:</label>
        <asp:TextBox runat="server" ID="tbVIN" MaxLength="17" ClientIDMode="Static" />
        <br />
        <asp:Button runat="server" CssClass="rpt-button" ID="btnReadinessFormat" Text="Readiness Format" OnClientClick="javascript: showReadinessFormat(); return false;" />
    </div>

    <div id="div-readiness-overlay" class="hidden overlay">
        <div id="divReadinessOuter" class="definition-body-outer">
            <div id="div-readiness-parent" class="definition">
                <!-- #include file="~/Classes/Reports/Triggers/Definitions/OBDReadinessMonitors.html" -->
            </div>
        </div>
    </div>

    <asp:HiddenField runat="server" ID="hidShowOverlay" />

    <script>
        function showReadinessFormat() {
            document.getElementById('div-readiness-overlay').classList.remove('hidden');
            $('#div-readiness-parent').scrollTop(0);
        }
    </script>

</asp:Content>
