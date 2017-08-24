<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="ApplicationError.aspx.cs"
    Inherits="NHPortal.ApplicationError"
    MasterPageFile="~/MasterPages/PortalMaster.master"
    Title="Portal Application Error" %>

<%@ MasterType VirtualPath="~/MasterPages/PortalMaster.master" %>

<asp:Content runat="server" ID="content" ContentPlaceHolderID="PageContent">
    <img src="Images/div0011.gif" class="centered block-image" />

    <div class="centered-text bold-font padded">
        <div>
            We're sorry, but an unexpected error has occurred.
            <br />
            Details of this error have been logged.
        </div>

        <div class="padded error">
            Error Message:
            <asp:Label runat="server" ID="lblErrorMessage"></asp:Label>
        </div>
    </div>

    <img src="Images/div0011.gif" class="centered block-image" />

    <div class="centered-text padded">
        <a href="Welcome.aspx">Home</a>
    </div>
</asp:Content>
