<%@ Page Title="Gordon-Darby Rejection Rate By Inspector Report" Language="C#" MasterPageFile="~/MasterPages/ReportMaster.master" AutoEventWireup="true" CodeBehind="RejectionByInspector.aspx.cs" Inherits="NHPortal.Reports.RejectionByInspector" %>
<%@ MasterType VirtualPath="~/MasterPages/ReportMaster.master" %>

<asp:Content ID="RejectionByModelContent" ContentPlaceHolderID="ReportContent" runat="server">

    <div id="divRejectionCriteria" class="centered rejection-criteria">
        <label class="criteria-label">Inspector:</label>
        <asp:TextBox runat="server" ID="txtInspectorID" MaxLength="100" Width="300"></asp:TextBox>
        <div class="criteria-note">
            Use semi-colon (;) to seperate multiple terms.
            <br />
            Use * or % for wildcard searches.
        </div>
    </div>

</asp:Content>