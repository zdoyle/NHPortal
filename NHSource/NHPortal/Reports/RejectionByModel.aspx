<%@ Page Title="Gordon-Darby Rejection Rate By Model Report" Language="C#" MasterPageFile="~/MasterPages/ReportMaster.master" AutoEventWireup="true" CodeBehind="RejectionByModel.aspx.cs" Inherits="NHPortal.Reports.RejectionByModel" %>
<%@ MasterType VirtualPath="~/MasterPages/ReportMaster.master" %>

<asp:Content ID="RejectionByModelContent" ContentPlaceHolderID="ReportContent" runat="server">

    <div id="divRejectionCriteria" class="centered rejection-criteria">
        <label class="criteria-label">Model:</label>
        <asp:TextBox runat="server" ID="txtModel" MaxLength="100" Width="300"></asp:TextBox>
        <div class="criteria-note">
            Use semi-colon (;) to seperate multiple terms.
            <br />
            Use * or % for wildcard searches.
        </div>
    </div>

</asp:Content>
