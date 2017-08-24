using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace NHPortal.Classes.User
{
    /// <summary>DropDownList web control representing a selection for a UserPermissionGroup object.</summary>
    public class PermissionGroupDownDownList : DropDownList
    {
        private readonly UserPermissionGroup m_group;
        /// <summary>Gets the UserPermissionGroup the control represents.</summary>
        public UserPermissionGroup Group
        {
            get { return m_group; }
        }

        /// <summary>Instantiates a new instance of the PermissionGroupDownDownList class.</summary>
        /// <param name="group">Group the control represents.</param>
        public PermissionGroupDownDownList(UserPermissionGroup group)
        {
            m_group = group;
            Initialize();
        }

        private void Initialize()
        {
            Items.Clear();
            Items.Add(new ListItem("", ""));
            foreach (var level in Group.AvailableAccessLevels)
            {
                Items.Add(new ListItem(level.Description, level.Value));
            }

            if (Items.Count > 0)
            {
                SelectedIndex = 0;
            }
        }

        /// <summary>Gets an array of PermissionGroupDownDownList controls contained on a page.</summary>
        /// <param name="page">Page to get PermissionGroupDownDownList controls of.</param>
        /// <returns>Array of PermissionGroupDownDownList controls found.</returns>
        public static PermissionGroupDownDownList[] GetForPage(System.Web.UI.Page page)
        {
            List<PermissionGroupDownDownList> cbos = new List<PermissionGroupDownDownList>();
            foreach (System.Web.UI.Control c in GDWebUtilities.WebUtilities.GetAllControls(page))
            {
                if (c is PermissionGroupDownDownList)
                {
                    cbos.Add(c as PermissionGroupDownDownList);
                }
            }
            return cbos.ToArray();
        }
    }
}