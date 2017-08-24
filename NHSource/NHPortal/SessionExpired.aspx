<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SessionExpired.aspx.cs" Inherits="NHPortal.SessionExpired"
    MasterPageFile="~/MasterPages/PortalMaster.master" Title="Session Expired"  %>
<%@ MasterType VirtualPath="~/MasterPages/PortalMaster.master" %>

<asp:Content runat="server" ID="content" ContentPlaceHolderID="PageContent">
    <img src="Images/div0011.gif" class="centered block-image" />

    <div class="centered-text bold-font padded">
        Your Session Has Expired.
    </div>

    <img src="Images/div0011.gif" class="centered block-image" />

    <div class="centered-text padded">
        <a href="Login.aspx">Home</a>
    </div>
</asp:Content>