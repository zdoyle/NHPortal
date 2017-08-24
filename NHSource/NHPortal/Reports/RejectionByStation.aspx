<%@ Page Title="Gordon-Darby Rejection Rate By Station Report" Language="C#" MasterPageFile="~/MasterPages/ReportMaster.master" AutoEventWireup="true" CodeBehind="RejectionByStation.aspx.cs" Inherits="NHPortal.Reports.RejectionByStation" %>
<%@ MasterType VirtualPath="~/MasterPages/ReportMaster.master" %>

<asp:Content ID="RejectionByModelContent" ContentPlaceHolderID="ReportContent" runat="server">

    <div id="divRejectionCriteria" class="centered rejection-criteria">
        <label class="criteria-label">Station:</label>
        <asp:TextBox runat="server" ID="txtStationID" MaxLength="100" Width="300"></asp:TextBox>
        <div class="criteria-note">
            Use semi-colon (;) to seperate multiple terms.
            <br />
            Use * or % for wildcard searches.
        </div>
    </div>   

</asp:Content>