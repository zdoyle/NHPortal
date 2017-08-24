<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ConsumableInventoryReport.aspx.cs" Inherits="NHPortal.Reports.ConsumableInventoryReport"
    MasterPageFile="~/MasterPages/ReportMaster.master" Title="Gordon-Darby Consumable Inventory Report" %>

<%@ MasterType VirtualPath="~/MasterPages/ReportMaster.master" %>
<%@ Register Src="~/UserControls/DateTextBox.ascx" TagPrefix="GD" TagName="DatePicker" %>
<%@ Register Assembly="NHPortal" Namespace="NHPortal.Classes.WebControls" TagPrefix="GD" %>

<asp:Content runat="server" ID="ConsumableInventoryReport" ContentPlaceHolderID="ReportContent">
    <div class="report-criteria padded centered-text" id="reportContent">
        <label class="report-criteria__label">Report Date</label>
        <GD:DatePicker runat="server" ID="dpReportDate"/>
    </div>
</asp:Content>
