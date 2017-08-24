<%@ Page Title="Gordon-Darby Rejection Rate By Make Report" Language="C#" MasterPageFile="~/MasterPages/ReportMaster.master" AutoEventWireup="true" CodeBehind="RejectionByMake.aspx.cs" Inherits="NHPortal.Reports.RejectionByMake" %>

<%@ MasterType VirtualPath="~/MasterPages/ReportMaster.master" %>

<asp:Content ID="RejectionByMakeContent" ContentPlaceHolderID="ReportContent" runat="server">

    <div id="divRejectionCriteria" class="centered rejection-criteria">
        <label class="criteria-label">Make:</label>
        <asp:TextBox runat="server" ID="txtMake" MaxLength="100" Width="300"></asp:TextBox>
        <div class="criteria-note">
            Use semi-colon (;) to seperate multiple terms.
            <br />
            Use * or % for wildcard searches.
        </div>
    </div>

</asp:Content>
