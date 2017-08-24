<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserMaintenance.aspx.cs" Inherits="NHPortal.UserMaintenance"
    MasterPageFile="~/MasterPages/PortalMaster.master" Title="User Maintenance" %>

<%@ MasterType VirtualPath="~/MasterPages/PortalMaster.master" %>

<asp:Content runat="server" ID="content" ContentPlaceHolderID="PageContent">
    <link type="text/css" rel="stylesheet" href="Style/UserMaintenance.css" />

    <div>
        <asp:UpdatePanel runat="server" ID="updatePanel">
            <ContentTemplate>
                <asp:Panel runat="server" ID="divUsers">
                    <label class="um-label">User:</label>
                    <asp:DropDownList runat="server" ID="cboUsers" OnSelectedIndexChanged="cboUsers_SelectedIndexChanged" AutoPostBack="true" CssClass="um-list"></asp:DropDownList>
                <%--    <asp:RequiredFieldValidator runat="server"
                        ID="rvalUsersDropDownList"
                        ControlToValidate="cboUsers"
                        InitialValue="0"
                        Display="Dynamic"
                        Text="Please select a user."
                        CssClass="error"></asp:RequiredFieldValidator>--%>
                </asp:Panel>

                <div>
                    <label class="um-label">* Username:</label>
                    <asp:TextBox runat="server" ID="tbUsers" MaxLength="30"></asp:TextBox>
                    <asp:RequiredFieldValidator runat="server"
                        ID="rvalUsernameTextbox"
                        ControlToValidate="tbUsers"
                        Text="*"
                        ErrorMessage="User Name"
                        ValidationGroup="valGroupUsers"
                        CssClass="error"></asp:RequiredFieldValidator>
                </div>

                <div>
                    <label class="um-label">* First Name:</label>
                    <asp:TextBox runat="server" ID="tbFirstName" MaxLength="30"></asp:TextBox>
                    <asp:RequiredFieldValidator runat="server"
                        ID="rvalFirstName"
                        ControlToValidate="tbFirstName"
                        Text="*"
                        ErrorMessage="First Name"
                        ValidationGroup="valGroupUsers"
                        CssClass="error"></asp:RequiredFieldValidator>

                    <label class="um-label">* Last Name:</label>
                    <asp:TextBox runat="server" ID="tbLastName" MaxLength="30"></asp:TextBox>
                    <asp:RequiredFieldValidator runat="server"
                        ID="rvalLastName"
                        ControlToValidate="tbLastName"
                        Text="*"
                        ErrorMessage="Last Name"
                        ValidationGroup="valGroupUsers"
                        CssClass="error"></asp:RequiredFieldValidator>
                </div>

                <div>
                    <label class="um-label">Company:</label>
                    <asp:TextBox runat="server" ID="tbCompany" MaxLength="30"></asp:TextBox>
                </div>

                <div>
                    <span class="um-label">Address:</span>
                    <asp:TextBox runat="server" ID="tbAddress" MaxLength="50"></asp:TextBox>
                </div>

                <div>
                    <span class="um-label">Line 2:</span>
                    <asp:TextBox runat="server" ID="tbLineTwo" MaxLength="30"></asp:TextBox>
                </div>

                <div>
                    <span class="um-label">* City:</span>
                    <asp:TextBox runat="server" ID="tbCity" MaxLength="25"></asp:TextBox>
                    <asp:RequiredFieldValidator runat="server"
                        ID="rvalCity"
                        ControlToValidate="tbCity"
                        Text="*"
                        ErrorMessage="City"
                        ValidationGroup="valGroupUsers"
                        CssClass="error"></asp:RequiredFieldValidator>

                    <span class="um-label">State:</span>
                    <asp:DropDownList runat="server" ID="cboState" CssClass="um-statelist"></asp:DropDownList>

                    <span class="um-label">* Zip:</span>
                    <asp:TextBox runat="server" ID="tbZip" MaxLength="5"></asp:TextBox>
                    <asp:RequiredFieldValidator runat="server"
                        ID="rvalZip"
                        ControlToValidate="tbZip"
                        Text="*"
                        ErrorMessage="Zip"
                        ValidationGroup="valGroupUsers"
                        CssClass="error"></asp:RequiredFieldValidator>
                </div>

                <div>
                    <span class="um-label">County:</span>
                    <asp:DropDownList runat="server" ID="cboCounty" CssClass="um-list"></asp:DropDownList>
                </div>

                <div>
                    <span class="um-label">Phone:</span>
                    <asp:TextBox runat="server" ID="tbPhone"></asp:TextBox>
                </div>

                <div>
                    <span class="um-label">Email:</span>
                    <asp:TextBox runat="server" ID="tbEmail" MaxLength="30"></asp:TextBox>
                </div>

                <div>
                    <span class="um-label">Active:</span>
                    <asp:CheckBox runat="server" ID="chkActive" />
                </div>

                <span class="required-field">* Required Field</span>

                <div class="centered centered-text error">
                    <asp:ValidationSummary runat="server"
                        ID="valSummary"
                        HeaderText="The following fields are required:"
                        DisplayMode="List"
                        ValidationGroup="valGroupUsers" />
                </div>

                <div id="divButtons" class="centered-text padded">
                    <input runat="server" id="btnEditPermissions" type="button" value="Edit Permissions" onclick="javascript: showPermissions();" />
                    <asp:Button runat="server" ID="btnSaveUser" Text="Save"
                        OnClick="btnSaveUser_Click" ValidationGroup="valGroupUsers" CausesValidation="true" />
                    <asp:Button runat="server" ID="btnUnlock" Text="Unlock User"
                        Visible="false" OnClick="btnUnlock_Click" />
                </div>

                <div id="div-permissions-overlay" class="overlay hidden">
                    <div id="div-permissions">
                        <div id="div-permissions-container">
                            <asp:Label runat="server" ID="lblPermissions"></asp:Label>
                            <asp:PlaceHolder runat="server" ID="phPermissions"></asp:PlaceHolder>
                            <asp:Literal runat="server" ID="ltrlPermissions"></asp:Literal>
                        </div>

                        <div class="centered-text">
                            <input type="button" value="Close" onclick="javascript: hidePermissions();" />
                        </div>
                    </div>
                </div>

                <asp:HiddenField runat="server" ID="hidSaveResult" ClientIDMode="Static" />
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>

    <asp:HiddenField runat="server" ID="hidShowOverlay" ClientIDMode="Static" />

    <script type="text/javascript" src="Scripts/jquery.maskedinput.min.js"></script>
    <script type="text/javascript">
        var overlay;
        var showOverlay;

        function showPermissions() {
            if (overlay) {
                overlay.classList.remove('hidden');
                showOverlay.value = 'true';
            }
        }

        function hidePermissions() {
            if (overlay) {
                overlay.classList.add('hidden');
                showOverlay.value = 'false';
            }
        }

        function showSaveResult() {
            var result = document.getElementById('hidSaveResult');
            if (result && result.value !== '') {
                alert(result.value);
                result.value = '';
            }
        }

        function stateChange(cbo) {
            if (cbo && cbo.value === 'NH') {
                disableCounty(false);
            } else {
                disableCounty(true);
            }
        }

        function disableCounty(disable) {
            document.getElementById('<%=cboCounty.ClientID%>').disabled = disable;
        }

        function pageLoad() {
            overlay = document.getElementById('div-permissions-overlay');
            showOverlay = document.getElementById('hidShowOverlay');
            showMessage();
            showSaveResult();

            jQuery(function ($) {
                $("#<%= tbPhone.ClientID %>").mask("(999) 999-9999");
            });

            if (showOverlay && showOverlay.value == 'true') {
                showPermissions();
            }
            }
    </script>
</asp:Content>
