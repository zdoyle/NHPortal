<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ConsumableSalesReport.aspx.cs" Inherits="NHPortal.Reports.ConsumableSalesReport"
    MasterPageFile="~/MasterPages/ReportMaster.master" Title="Gordon-Darby Consumable Sales Report" %>

<%@ MasterType VirtualPath="~/MasterPages/ReportMaster.master" %>
<%@ Register Src="~/UserControls/DatePeriodSelector.ascx" TagPrefix="GD" TagName="DatePeriodSelector" %>
<%@ Register Assembly="NHPortal" Namespace="NHPortal.Classes.WebControls" TagPrefix="GD" %>

<asp:Content runat="server" ID="ConsumableSaleReport" ContentPlaceHolderID="ReportContent">
    <div class="report-criteria padded centered-text" id="reportContent">

      <GD:DatePeriodSelector runat="server" ID="dateSelector"></GD:DatePeriodSelector>
    </div>
</asp:Content>
