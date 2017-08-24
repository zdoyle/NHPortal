<%@ Page Title="Gordon-Darby Rejection Rate By County Report" Language="C#" MasterPageFile="~/MasterPages/ReportMaster.master" AutoEventWireup="true" CodeBehind="RejectionByCounty.aspx.cs" Inherits="NHPortal.Reports.RejectionByCounty" %>
<%@ MasterType VirtualPath="~/MasterPages/ReportMaster.master" %>

<%@ Register Assembly="NHPortal" Namespace="NHPortal.Classes.WebControls" TagPrefix="GD" %>

<asp:Content ID="RejectionByModelContent" ContentPlaceHolderID="ReportContent" runat="server">

    <div id="divRejectionCriteria" class="centered rejection-criteria">
        <label class="report-criteria__label">County</label>
        <GD:CountyListBox runat="server" id="lstCounty" Height="100"></GD:CountyListBox>
    </div>

</asp:Content>
