<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MainReport.aspx.cs" Inherits="NHPortal.Reports.MainReport"
    MasterPageFile="~/MasterPages/ReportMaster.master" Title="Gordon-Darby Main Report" %>

<%@ MasterType VirtualPath="~/MasterPages/ReportMaster.master" %>
<%@ Register Src="~/UserControls/DatePeriodSelector.ascx" TagPrefix="GD" TagName="DatePeriodSelector" %>
<%@ Register Assembly="NHPortal" Namespace="NHPortal.Classes.WebControls" TagPrefix="GD" %>

<asp:Content runat="server" ID="Content" ContentPlaceHolderID="ReportContent">
    <div class="report-criteria padded centered-text">
        <div class="report-input-container">
            <label class="report-criteria__label">Test Sequence</label>
            <GD:TestSequenceListBox runat="server" ID="lstTestSeq"></GD:TestSequenceListBox>

            <label class="report-criteria__label">Model Year</label>
            <GD:ModelYearListBox runat="server" ID="lstModelYear"></GD:ModelYearListBox>

            <label class="report-criteria__label">Vehicle Type</label>
            <GD:VehicleTypeListBox runat="server" ID="lstVehicleType"></GD:VehicleTypeListBox>

            <label class="report-criteria__label">County</label>
            <GD:CountyListBox runat="server" ID="lstCounty"></GD:CountyListBox>
        </div>

        <GD:DatePeriodSelector runat="server" ID="dateSelector"></GD:DatePeriodSelector>
    </div>
</asp:Content>
