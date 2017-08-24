﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CallsByDuration.aspx.cs" Inherits="NHPortal.Reports.CallsByDuration"
    MasterPageFile="~/MasterPages/ReportMaster.master" Title="Gordon-Darby Calls By Duration Report" %>

<%@ MasterType VirtualPath="~/MasterPages/ReportMaster.master" %>
<%@ Register Src="~/UserControls/DatePeriodSelector.ascx" TagPrefix="GD" TagName="DatePeriodSelector" %>
<%@ Register Assembly="NHPortal" Namespace="NHPortal.Classes.WebControls" TagPrefix="GD" %>

<asp:Content runat="server" ID="content" ContentPlaceHolderID="ReportContent">
    <div class="report-criteria padded centered-text">
        <div class="padded">
            <label class="report-criteria__label">Call Type</label>
            <GD:CallTypeDropDown runat="server" ID="cboCallType"></GD:CallTypeDropDown>
        </div>

        <GD:DatePeriodSelector runat="server" ID="dateSelector"></GD:DatePeriodSelector>
    </div>
</asp:Content>
