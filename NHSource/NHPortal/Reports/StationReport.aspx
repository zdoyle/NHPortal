<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StationReport.aspx.cs" Inherits="NHPortal.Reports.StationReport"
    MasterPageFile="~/MasterPages/ReportMaster.master" Title="Gordon-Darby Station Audit Report" %>

<%@ MasterType VirtualPath="~/MasterPages/ReportMaster.master" %>
<%@ Register Src="~/UserControls/DatePeriodSelector.ascx" TagPrefix="GD" TagName="DatePeriodSelector" %>
<%@ Register Assembly="NHPortal" Namespace="NHPortal.Classes.WebControls" TagPrefix="GD" %>

<asp:Content runat="server" ID="Content" ContentPlaceHolderID="ReportContent">
    <div class="report-criteria padded centered-text">
        <div class="report-input-container">
            <label class="report-criteria__label">Officer</label>
            <GD:OfficerListBox runat="server" ID="lstOfficerType"></GD:OfficerListBox>

            <label class="report-criteria__label">Station Type</label>
            <GD:StationTypeListBox runat="server" ID="lstStationType"></GD:StationTypeListBox>
        </div>
        <GD:DatePeriodSelector runat="server" ID="dateSelector"></GD:DatePeriodSelector>
    </div>
</asp:Content>
