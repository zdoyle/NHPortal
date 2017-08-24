<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="NHPortal.Login"
    MasterPageFile="~/MasterPages/PortalMaster.master" Title="NH Portal Login" %>

<%@ MasterType VirtualPath="~/MasterPages/PortalMaster.master" %>

<asp:Content runat="server" ID="loginContent" ContentPlaceHolderID="PageContent">
    <link rel="stylesheet" type="text/css" href="Style/Login.css" />
    <div class="div-login centered-text">
        <div id="divWelcome" class="bold-font">
            <img src="Images/div0011.gif" class="centered block-image" />

            Welcome To
            <span id="spanMVP">My Vehicle Portal (MVP)™</span>
            For
            <br />
            NEW HAMPSHIRE
        </div>

        <br />

        <div>
            Please Enter Your Login Information To Continue
        </div>

        <br />

        <asp:Panel runat="server" ID="pnlLogin" CssClass="padded" DefaultButton="btnLogin">
            <label class="textbox-label">
                Username:
            </label>
            <asp:TextBox runat="server" ID="tbUsername" MaxLength="30" onfocus="this.select();" CssClass="textbox-user-entry"></asp:TextBox>

            <br />

            <label class="textbox-label">
                Password:
            </label>
            <asp:TextBox runat="server" ID="tbPassword" MaxLength="30" onfocus="this.select();" CssClass="textbox-user-entry" TextMode="Password"></asp:TextBox>
        
            <br />
        </asp:Panel>

        <div class="padded">
            <asp:Button runat="server" ID="btnLogin" Text="Login" CssClass="btn-login" OnClick="btnLogin_Click" />
        </div>
    </div>

    <div class="centered centered-text padded error">
        <asp:Label runat="server" ID="lblError"></asp:Label>

        <div>
        <asp:RequiredFieldValidator runat="server" ID="valUserName"
            Display="Dynamic" ControlToValidate="tbUsername"
            ErrorMessage="Please enter a user name" ></asp:RequiredFieldValidator>
        </div>

        <div>
            <asp:RequiredFieldValidator runat="server" ID="valPassword" 
                Display="Dynamic" ControlToValidate="tbPassword"
                ErrorMessage="Please enter a password"></asp:RequiredFieldValidator>
        </div>
    </div>

    <img src="Images/div0011.gif" class="centered block-image" />
</asp:Content>
