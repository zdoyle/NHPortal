﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace NHPortal.Classes.WebControls
{
    /// <summary>DropDownList control for Sticker Types.</summary>
    [System.Web.UI.ToolboxData("<{0}:StickerTypeDropDownList runat='server'></{0}:StickerTypeDropDownList>")]
    public class StickerTypeDropDownList : System.Web.UI.WebControls.DropDownList
    {
        //// <summary>Initializes the items in the DropDownList.</summary>
        public void Initialize()
        {
            this.Items.Clear();
            this.Items.Add(new System.Web.UI.WebControls.ListItem("A", "A"));
            this.Items.Add(new System.Web.UI.WebControls.ListItem("M", "M"));

            if (Items.Count > 0)
            {
                SelectedIndex = 0;
            }
        }
    }
}