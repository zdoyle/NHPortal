<%@ Page Title="Gordon-Darby Rejection Rate By Model Year Report" Language="C#" MasterPageFile="~/MasterPages/ReportMaster.master" AutoEventWireup="true" CodeBehind="RejectionByModelYear.aspx.cs" Inherits="NHPortal.Reports.RejectionByModelYear" %>
<%@ MasterType VirtualPath="~/MasterPages/ReportMaster.master" %>
<%@ Register Assembly="NHPortal" Namespace="NHPortal.Classes.WebControls" TagPrefix="GD" %>

<asp:Content ID="RejectionByModelContent" ContentPlaceHolderID="ReportContent" runat="server">

    <div id="divRejectionCriteria" class="centered rejection-criteria">
        <label class="report-criteria__label">Model Year</label>
        <GD:ModelYearListBox runat="server" ID="lstModelYear" Height="100"></GD:ModelYearListBox>

    </div>

</asp:Content>
