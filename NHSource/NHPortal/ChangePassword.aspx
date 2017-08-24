<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChangePassword.aspx.cs" Inherits="NHPortal.ChangePassword"
    MasterPageFile="~/MasterPages/PortalMaster.master" Title="Change User Password" %>

<%@ MasterType VirtualPath="~/MasterPages/PortalMaster.master" %>

<asp:Content runat="server" ID="content" ContentPlaceHolderID="PageContent">
    <link type="text/css" rel="stylesheet" href="Style/Password.css" />

    <asp:Panel runat="server" ID="pnlUpdatePassword" CssClass="centered centered-text" DefaultButton="btnUpdatePassword">
        <div class="password-input-div">
            <label class="password-label">Current Password:</label>
            <asp:TextBox runat="server" ID="tbCrntPassword" CssClass="password-input" TextMode="Password" MaxLength="30"></asp:TextBox>

            <br />
            <label class="password-label">New Password:</label>
            <asp:TextBox runat="server" ID="tbNewPassword" CssClass="password-input" TextMode="Password" MaxLength="30"></asp:TextBox>

            <br />
            <label class="password-label">Confirm New Password:</label>
            <asp:TextBox runat="server" ID="tbConfirmPassword" CssClass="password-input" TextMode="Password" MaxLength="30"></asp:TextBox>
        </div>

        <div class="error">
            <asp:Label runat="server" ID="lblErrors"></asp:Label>

            <div>
                <asp:CompareValidator runat="server" ID="valPasswordMatch"
                    Display="Dynamic" Operator="Equal" Type="String"
                    ControlToValidate="tbConfirmPassword" ControlToCompare="tbNewPassword"
                    ErrorMessage="New password fields do not match"></asp:CompareValidator>
            </div>
            <div>
                <asp:RequiredFieldValidator runat="server" ID="valNewPassword"
                    Display="Dynamic" ControlToValidate="tbNewPassword"
                    ErrorMessage="New Password field must be filled out."></asp:RequiredFieldValidator>
            </div>
            <div>
                <asp:RequiredFieldValidator runat="server" ID="valConfirmPassword"
                    Display="Dynamic"
                    ControlToValidate="tbConfirmPassword"
                    ErrorMessage="Confirm New Password field must be filled out."></asp:RequiredFieldValidator>
            </div>
        </div>

        <div class="password-btn-container">
            <asp:Button runat="server" ID="btnUpdatePassword" Text="Update" CssClass="btn-update-password"
                OnClick="btnUpdatePassword_Click" />
            &nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button runat="server" ID="btnCancel" Text="Cancel" CssClass="btn-cancel"
                OnClick="btnCancel_Click" CausesValidation="false" />
        </div>

        <hr />

        <div class="password-rules-container">
            <span>Password Strength Requirements:</span>
            <br />
            1) Password must be at least eight (8) characters.
            <br />
            2) Password must contain at least one (1) upper-case character.
            <br />
            3) Password must contain at least one (1) number.
        </div>
    </asp:Panel>
</asp:Content>
