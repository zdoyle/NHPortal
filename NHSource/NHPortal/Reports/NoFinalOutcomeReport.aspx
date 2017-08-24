<%@ Page Title="Gordon-Darby No Final Outcome Report" Language="C#" MasterPageFile="~/MasterPages/ReportMaster.master" AutoEventWireup="true" CodeBehind="NoFinalOutcomeReport.aspx.cs" Inherits="NHPortal.Reports.NoFinalOutcomeReport" %>
<%@ MasterType VirtualPath="~/MasterPages/ReportMaster.master" %>
<%@ Register Assembly="NHPortal" Namespace="NHPortal.Classes.WebControls" TagPrefix="GD" %>
<%@ Register Src="~/UserControls/DateTextBox.ascx" TagPrefix="GD" TagName="DatePicker" %>

<asp:Content ID="NoFinal" ContentPlaceHolderID="ReportContent" runat="server">
     <asp:Panel runat="server" ID="divNoFinal" CssClass="main-div">
        <div id="NoFinalOuter" class="report-criteria padded centered-text" style="width: 400px;">
        
                            <label class="report-criteria__label">Date:</label>
                            <GD:DatePicker runat="server" ID="dpStart" />
          
        </div>
    </asp:Panel>

</asp:Content>

