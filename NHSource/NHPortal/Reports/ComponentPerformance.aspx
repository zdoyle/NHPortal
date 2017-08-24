<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ComponentPerformance.aspx.cs" Inherits="NHPortal.Reports.ComponentPerformance"
    MasterPageFile="~/MasterPages/ReportMaster.master" Title="Gordon-Darby Component Performance Report" %>

<%@ MasterType VirtualPath="~/MasterPages/ReportMaster.master" %>
<%@ Register Src="~/UserControls/DatePeriodSelector.ascx" TagPrefix="GD" TagName="DatePeriodSelector" %>
<%@ Register Src="~/UserControls/DateTextBox.ascx" TagPrefix="GD" TagName="DatePicker" %>
<%@ Register Assembly="NHPortal" Namespace="NHPortal.Classes.WebControls" TagPrefix="GD" %>

<asp:Content runat="server" ID="content" ContentPlaceHolderID="ReportContent">
    <div class=" padded centered-text centered station-support-criteria">
        <span class="spn-criteria">
            <label class="report-criteria__label">Report Date</label>
            <GD:DatePicker runat="server" ID="dpReportDate" />
        </span>
        <span class="spn-criteria">
            <label class="report-criteria__label">County</label>
            <GD:CountyDropDown runat="server" ID="cboCounty"></GD:CountyDropDown>
        </span>
    </div>
</asp:Content>
