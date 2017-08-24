<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SuspectTestReport.aspx.cs" Inherits="NHPortal.Reports.SuspectTestReport"
    MasterPageFile="~/MasterPages/ReportMaster.master" Title="Gordon-Darby Suspect Test Report" %>

<%@ MasterType VirtualPath="~/MasterPages/ReportMaster.master" %>
<%@ Register Src="~/UserControls/DatePeriodSelector.ascx" TagPrefix="GD" TagName="DatePeriodSelector" %>

<asp:Content runat="server" ID="mainRptContent" ContentPlaceHolderID="ReportContent">
    <div class="report-criteria padded centered-text">
        <label class="report-criteria__label">Station:</label>
        <asp:TextBox runat="server" ID="tbStation"  MaxLength="8" ClientIDMode="Static"/>
        <br />
        <br />
        <GD:DatePeriodSelector runat="server" ID="dateSelector"></GD:DatePeriodSelector>
    </div>
</asp:Content>
