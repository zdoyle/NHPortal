<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InvalidPermissions.aspx.cs" Inherits="NHPortal.InvalidPermissions"
    MasterPageFile="~/MasterPages/PortalMaster.master" Title="Insufficient Permissions" %>

<%@ MasterType VirtualPath="~/MasterPages/PortalMaster.master" %>

<asp:Content runat="server" ID="content" ContentPlaceHolderID="PageContent">
    <img src="Images/div0011.gif" class="centered block-image" />

    <div class="centered-text bold-font padded">
        You Do Not Have The Appropriate Permissions To View The Requested Page
    </div>

    <img src="Images/div0011.gif" class="centered block-image" />

    <div class="centered-text padded">
        <a href="Welcome.aspx">Home</a>
    </div>
</asp:Content>
