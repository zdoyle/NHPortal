using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace NHPortal.Classes.User
{
    /// <summary>DropDownList control representing a selecting for a UserPermission.</summary>
    public class PermissionDropDownList : DropDownList
    {
        private readonly UserPermission m_permission;
        /// <summary>Gets the UserPermissions the control represents.</summary>
        public UserPermission Permission
        {
            get { return m_permission; }
        }

        /// <summary>Instantiates a new instance of the PermissionDropDownList control.</summary>
        /// <param name="permission">UserPermission the control is for.</param>
        public PermissionDropDownList(UserPermission permission)
        {
            m_permission = permission;
            Initialize();
        }

        private void Initialize()
        {
            Items.Clear();
            AddListItem(AccessLevels.None);
            if (m_permission.AllowReadonly)
            {
                AddListItem(AccessLevels.ReadOnly);
            }
            if (m_permission.AllowFullAccess)
            {
                AddListItem(AccessLevels.Full);
            }

            if (Items.Count > 0)
            {
                SelectedIndex = 0;
            }
        }

        private void AddListItem(AccessLevel accessLevel)
        {
            if (accessLevel != null)
            {
                Items.Add(new ListItem(accessLevel.Description, accessLevel.Value));
            }
        }

        /// <summary>Gets an array of PermissionDropDownList controls contained on a page.</summary>
        /// <param name="page">Page to get PermissionDropDownList controls of.</param>
        /// <returns>Array of PermissionDropDownList controls found.</returns>
        public static PermissionDropDownList[] GetForPage(System.Web.UI.Page page)
        {
            List<PermissionDropDownList> cbos = new List<PermissionDropDownList>();
            foreach (System.Web.UI.Control c in GDWebUtilities.WebUtilities.GetAllControls(page))
            {
                if (c is PermissionDropDownList)
                {
                    cbos.Add(c as PermissionDropDownList);
                }
            }
            return cbos.ToArray();
        }
    }
}