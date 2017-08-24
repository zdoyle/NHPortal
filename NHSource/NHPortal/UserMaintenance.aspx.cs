using GDCoreUtilities.Logging;
using GDWebUtilities;
using NHPortal.Classes;
using NHPortal.Classes.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace NHPortal
{
    public partial class UserMaintenance : PortalPage
    {
        private UserMaintenanceMode m_pageMode;

        /// <summary>Gets the selected user from the page's drop down list.</summary>
        public PortalUser SelectedUser
        {
            get
            {
                NHPortalUtilities.LogSessionMessage("Getting SelectedUser with sys no " + cboUsers.SelectedValue,
                    LogSeverity.Information);

                long usrSysNo = GDCoreUtilities.NullSafe.ToLong(cboUsers.SelectedValue);
                PortalUser usr = PortalUsers.Find(usrSysNo);
                return usr;
            }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            m_pageMode = SessionHelper.GetUserMaintenanceMode(this.Session);
            RenderPermissions();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {

            }
            else
            {
                InitializePage();
                InitializeByMode();
            }
        }

        private void InitializePage()
        {
            cboUsers.Items.Add(new ListItem("", "0"));
            foreach (var u in PortalUsers.AllByLastName)
            {
                cboUsers.Items.Add(new ListItem(u.Name, u.UserSysNo.ToString()));
            }

            if (cboUsers.Items.Count > 0)
            {
                cboUsers.SelectedIndex = 0;
            }

            foreach (var state in NHPortal.Classes.Reference.States.All)
            {
                cboState.Items.Add(new ListItem(state.Name, state.Name));
            }
            cboState.Attributes.Add("onchange", "javascript: stateChange(this);");

            cboCounty.Items.Add(new ListItem("", ""));
            foreach (var cnty in Counties.All)
            {
                cboCounty.Items.Add(new ListItem(cnty.Name, cnty.Name));
            }
        }

        private void InitializeByMode()
        {
            switch (m_pageMode)
            {
                case UserMaintenanceMode.Add:
                    divUsers.Visible = false;
                    cboState.SelectedValue = "NH";
                    chkActive.Checked = true;
                    Master.SetHeaderText("Add New User Record");
                    break;
                case UserMaintenanceMode.Modify:
                    tbUsers.Enabled = false;
                    rvalUsernameTextbox.Visible = false;
                    rvalUsernameTextbox.Enabled = false;
                    Master.SetHeaderText("Update User Record");
                    break;
                case UserMaintenanceMode.View:
                    tbUsers.Enabled = false;
                    Master.SetHeaderText("View User Record");
                    btnEditPermissions.Value = "View Permissions";
                    btnSaveUser.Visible = false;
                    DisableInputs();
                    break;
            }
        }

        private void HideValidators()
        {
            foreach (var c in WebUtilities.GetAllControls(this.Page))
            {
                if (c is RequiredFieldValidator)
                {
                    RequiredFieldValidator rval = c as RequiredFieldValidator;
                    rval.Display = ValidatorDisplay.None;
                    rval.Enabled = false;
                }
            }
        }

        private void DisableInputs()
        {
            foreach (Control c in WebUtilities.GetAllControls(this.Page))
            {
                if (c is TextBox)
                {
                    (c as TextBox).Enabled = false;
                }

                if (c is CheckBox)
                {
                    (c as CheckBox).Enabled = false;
                }

                if (c is DropDownList && c != cboUsers)
                {
                    (c as DropDownList).Enabled = false;
                }
            }
        }

        private void ClearForm()
        {
            foreach (Control c in WebUtilities.GetAllControls(this.Page))
            {
                if (c is TextBox)
                {
                    (c as TextBox).Text = String.Empty;
                }

                if (c is CheckBox)
                {
                    (c as CheckBox).Checked = false;
                }

                if (c is DropDownList && c != cboUsers)
                {
                    DropDownList cbo = c as DropDownList;
                    if (cbo.Items.Count > 0)
                    {
                        (c as DropDownList).SelectedIndex = 0;
                    }
                }
            }
        }

        protected void btnSaveUser_Click(object sender, EventArgs e)
        {
            PortalUser usr = null;
            switch (m_pageMode)
            {
                case UserMaintenanceMode.Add:
                    string user = this.tbUsers.Text.Trim().ToUpper();

                    if(PortalUsers.UserExists(user))
                    {
                        this.rvalUsernameTextbox.IsValid = false;
                        this.rvalUsernameTextbox.ErrorMessage = "User Name: User Already Exists";
                    }
                    else
                    {
                        usr = new PortalUser(tbUsers.Text.Trim().ToUpper());
                    } 
                    break;
                case UserMaintenanceMode.Modify:
                    usr = SelectedUser;
                    break;
            }

            if (usr != null)
            {
                UpdateUserRecordFromForm(usr);
                UpdatePermissionsFromForm(usr);
                bool success = usr.Save(PortalUser.UserName);
                SetSaveResultMessage(usr, success);

                if (success)
                {
                    PortalUsers.Initialize();
                }
            }
        }

        private void SetSaveResultMessage(PortalUser usr, bool success)
        {
            string msg = String.Empty;
            if (success)
            {
                if (m_pageMode == UserMaintenanceMode.Add)
                {
                    msg = String.Format("User record {0} created with password {1}",
                        usr.UserName, usr.Password);
                }
                else
                {
                    msg = "User record successfully updated.";
                }
            }
            else
            {
                msg = "An error occurred saving the user record.";
            }

            NHPortalUtilities.LogSessionMessage("SetSaveResultMessage: " + msg, LogSeverity.Information);
            hidSaveResult.Value = msg; // TODO how to use the master's hidMsg instead of this?
            //Master.SetMessagePrompt(msg);
        }

        protected void cboUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList cbo = sender as DropDownList;
            if (cbo != null)
            {
                if (SelectedUser == null)
                {
                    ClearForm();
                    string msg = String.Format("cboUsers_SelectedIndexChanged event triggered with a null user; selectedItem.Value: {0}",
                        cbo.SelectedItem.Value);
                    NHPortalUtilities.LogSessionMessage(msg, LogSeverity.Warning);
                    btnUnlock.Visible = false;
                }
                else
                {
                    PopulateUserInformation(SelectedUser);
                    PopulatePermissionsFromUser(SelectedUser);
                    ResetGroupPermissionDropDownList();

                    if (SelectedUser.IsLocked && m_pageMode == UserMaintenanceMode.Modify)
                    {
                        btnUnlock.Visible = true;
                    }
                    else
                    {
                        btnUnlock.Visible = false;
                    }
                }
            }
        }

        protected void cboGroupPermission_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender is PermissionGroupDownDownList)
            {
                PermissionGroupDownDownList groupCbo = sender as PermissionGroupDownDownList;
                foreach (var cbo in PermissionDropDownList.GetForPage(Page))
                {
                    if (cbo.Permission.PermissionGroupCode.Equals(groupCbo.Group.Code))
                    {
                        UpdatePermissionCboValue(cbo, groupCbo.SelectedValue);
                    }
                }
            }
        }

        private void ResetGroupPermissionDropDownList()
        {
            foreach (var cbo in PermissionGroupDownDownList.GetForPage(Page))
            {
                if (cbo.Items.Count > 0)
                {
                    cbo.SelectedIndex = 0;
                }
            }
        }

        private void UpdatePermissionCboValue(PermissionDropDownList cbo, string value)
        {
            // Set permission drop down list to value of the group drop down list            
            ListItem item = cbo.Items.FindByValue(value);
            if (item == null && cbo.Items.Count > 0)
            {
                // TODO could probably use some better permanent logic
                if (GDCoreUtilities.StringUtilities.AreEqual(value, AccessLevels.Full.Value))
                {
                    item = cbo.Items.FindByValue(AccessLevels.ReadOnly.Value);
                }

                if (item == null)
                {
                    // Otherwise use the first item in the list
                    value = cbo.Items[0].Value;
                }
                else
                {
                    value = item.Value;
                }
            }

            NHPortalUtilities.SetComboBoxValue(cbo, value);
        }

        private void UpdateUserRecordFromForm(PortalUser usr)
        {
            if (usr == null)
            {
                NHPortalUtilities.LogSessionMessage("Call to UpdateUserRecordFromForm with a null PortalUser object", LogSeverity.Warning);
            }
            else
            {
                usr.FirstName = tbFirstName.Text;
                usr.LastName = tbLastName.Text;
                usr.Company = tbCompany.Text;
                usr.StreetAddress = tbAddress.Text;
                usr.AddressLineTwo = tbLineTwo.Text;
                usr.City = tbCity.Text;
                usr.State = cboState.SelectedItem.Text;
                usr.ZipCode = tbZip.Text;
                usr.County = cboCounty.SelectedValue;
                usr.PhoneNumber = GDCoreUtilities.StringUtilities.ParseDigits(tbPhone.Text);
                usr.Email = tbEmail.Text;
                usr.IsActive = chkActive.Checked;
            }
        }

        private void UpdatePermissionsFromForm(PortalUser usr)
        {
            foreach (var cbo in PermissionDropDownList.GetForPage(Page))
            {
                AccessLevel level = AccessLevels.Find(cbo.SelectedValue);
                if (level != null)
                {
                    usr.SetPermission(cbo.Permission, level);
                }
            }
        }

        private void PopulateUserInformation(PortalUser usr)
        {
            if (usr == null)
            {
                NHPortalUtilities.LogSessionMessage("Call to PopulateUserInformation with a null PortalUser object", LogSeverity.Warning);
                ClearForm();
            }
            else
            {
                tbUsers.Text = usr.UserName;
                tbFirstName.Text = usr.FirstName;
                tbLastName.Text = usr.LastName;
                tbCompany.Text = usr.Company;
                tbAddress.Text = usr.StreetAddress;
                tbLineTwo.Text = usr.AddressLineTwo;
                tbCity.Text = usr.City;
                NHPortalUtilities.SetComboBoxValue(cboState, usr.State);
                tbZip.Text = usr.ZipCode;
                NHPortalUtilities.SetComboBoxValue(cboCounty, usr.County);
                tbPhone.Text = usr.PhoneNumber;
                tbEmail.Text = usr.Email;
                chkActive.Checked = usr.IsActive;

                if (cboState.SelectedValue != "NH")
                {
                    cboCounty.Enabled = false;
                }
            }
        }

        private void PopulatePermissionsFromUser(PortalUser usr)
        {
            string debug = "Setting value of cbo {0} from {1} to {2}";

            if (usr == null)
            {
                NHPortalUtilities.LogSessionMessage("Call to PopulatePermissionsFromUser with a null PortalUser object", LogSeverity.Warning);
            }
            else
            {
                foreach (PermissionDropDownList cbo in PermissionDropDownList.GetForPage(Page))
                {
                    PortalUserPermission permission = usr.Permissions[cbo.Permission.Code];
                    if (permission != null)
                    {
                        NHPortalUtilities.LogSessionMessage(String.Format(debug, cbo.ClientID, cbo.SelectedValue, permission.AccessLevel.Value),
                            LogSeverity.Debug);
                        NHPortalUtilities.SetComboBoxValue(cbo, permission.AccessLevel.Value);
                    }
                    else
                    {
                        NHPortalUtilities.LogSessionMessage(String.Format(debug, cbo.ClientID, cbo.SelectedValue, AccessLevels.None.Value),
                            LogSeverity.Debug);
                        NHPortalUtilities.SetComboBoxValue(cbo, AccessLevels.None.Value);
                    }
                }
            }
        }

        private void RenderPermissions()
        {
            foreach (UserPermissionGroup group in UserPermissionGroups.AllActive)
            {
                var div = new HtmlGenericControl("div");
                div.ID = String.Format("div-permissions-group-{0}", group.Code);
                div.Controls.Add(BuildGroupSection(group));
                div.Controls.Add(BuildPermissionsSection(group));
                phPermissions.Controls.Add(div);
            }
        }

        private HtmlGenericControl BuildGroupSection(UserPermissionGroup group)
        {
            // H2
            // -> Span
            //    -> Label
            //    -> Select

            var h2 = new HtmlGenericControl("h2");
            h2.Attributes["class"] = "permission-group";
            h2.ID = "h2-group-" + group.Code;

            var span = new HtmlGenericControl("span");
            var label = new HtmlGenericControl("label");
            label.InnerText = group.Description;
            label.Attributes["class"] = "group-label";
            label.ID = "lbl-group-" + group.Code;
            span.ID = "span-group-" + group.Code;

            span.Controls.Add(label);

            if (m_pageMode != UserMaintenanceMode.View)
            {
                PermissionGroupDownDownList cboGroup = new PermissionGroupDownDownList(group);
                cboGroup.ID = "cbo-group-" + group.Code;
                cboGroup.SelectedIndexChanged += cboGroupPermission_SelectedIndexChanged;
                cboGroup.AutoPostBack = true;
                span.Controls.Add(cboGroup);
            }

            h2.Controls.Add(span);
            return h2;
        }

        private HtmlGenericControl BuildPermissionsSection(UserPermissionGroup group)
        {
            var div = new HtmlGenericControl("div");
            div.Attributes["class"] = "inner-permissions";
            //div.ID = "div-group-" + group.Code;

            foreach (UserPermission permission in group.ActivePermissions)
            {
                div.Controls.Add(BuildPermission(permission));
            }
            return div;
        }

        private HtmlGenericControl BuildPermission(UserPermission permission)
        {
            var span = new HtmlGenericControl("span");
            span.Attributes["class"] = "permission";
            //span.ID = "span-prm-" + permission.Code;

            var label = new HtmlGenericControl("label");
            label.Attributes["class"] = "permission-label";
            label.InnerText = permission.Name;
            //label.ID = "lbl-prm-" + permission.Code;

            PermissionDropDownList cbo = new PermissionDropDownList(permission);
            cbo.ID = "cbo-permission-" + permission.Code;
            NHPortalUtilities.SetComboBoxValue(cbo, permission.DefaultValue.Value);
            if (m_pageMode == UserMaintenanceMode.View)
            {
                cbo.Enabled = false;
            }

            span.Controls.Add(label);
            span.Controls.Add(cbo);
            return span;
        }

        protected void btnUnlock_Click(object sender, EventArgs e)
        {
            if (SelectedUser != null)
            {
                if (SelectedUser.UpdateLockedStatus(false))
                {
                    // reset login attempt count here, in case it has not reset yet
                    SelectedUser.LoginAttemptCount = 0;

                    string msg = "User record unlocked";
                    hidSaveResult.Value = msg; // TODO how to use the master's hidMsg instead of this?
                    //Master.SetMessagePrompt(msg);
                    btnUnlock.Visible = false;
                }
            }
        }
    }

    /// <summary>Defines the modes available for the User Maintenance module.</summary>
    public enum UserMaintenanceMode
    {
        /// <summary>User Maintenance will be read-only.</summary>
        View,

        /// <summary>User Maintenance will be read-write to update existing users.</summary>
        Modify,

        /// <summary>User Maintenance will be read-write for adding a new user.</summary>
        Add,
    }
}