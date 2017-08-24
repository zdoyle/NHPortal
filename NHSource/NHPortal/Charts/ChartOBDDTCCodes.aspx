<%@ Page
    Language="C#" AutoEventWireup="true"
    CodeBehind="ChartOBDDTCCodes.aspx.cs"
    MasterPageFile="~/MasterPages/ChartMaster.Master"
    Inherits="NHPortal.Charts.ChartOBDDTCCodes" %>

<%@ MasterType VirtualPath="~/MasterPages/ChartMaster.Master" %>

<asp:Content runat="server" ID="AuditHistortReport" ContentPlaceHolderID="ExtraChartContent">
    <asp:Panel runat="server" ID="divAllCodes" CssClass="div-centered all-codes" ClientIDMode="Static">
        <asp:Button runat="server"  ID="btnAllCodes" ClientIDMode="Static" OnClientClick="javascript: buttonReportClick('ALL'); return false;" />
        &nbsp;
        <asp:Button runat="server"  ID="btnAllCodesByMakeModel" ClientIDMode="Static" OnClientClick="javascript: buttonReportClick('YMM'); return false;" />
    </asp:Panel>

</asp:Content>
