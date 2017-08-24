using GDWebUtilities;
using NHPortal.Classes;
using NHPortal.Classes.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NHPortal
{
    public partial class MyFavorites : PortalPage
    {
        private const string CHECKBOX_PREFIX = "chk-";
        private UserFavorite[] m_favorites;
        private HtmlBuilder m_builder;

        protected void Page_Load(object sender, EventArgs e)
        {
            m_favorites = SessionHelper.GetUserFavorites(this.Session);
            if (IsPostBack)
            {
                ProcessPostBack();
            }
            else
            {
                Master.SetHeaderText("My Favorites");
                m_favorites = UserFavorite.GetForUser(PortalUser);
                SessionHelper.SetUserFavorites(this.Session, m_favorites);
                RenderUserFavorites();
            }
        }

        private void ProcessPostBack()
        {
            string action = Master.HidActionValue;
            switch (action)
            {
                case "SELECT_FAV":
                    SelectFavorite();
                    break;
                case "REMOVE_SELECTED_FAVS":
                    RemoveSelectedFavorites();
                    break;
            }
        }

        private void RenderUserFavorites()
        {
            if (m_favorites != null)
            {
                RenderFavoriteSection(UserFavoriteTypes.Report, ltrlReports);
                RenderFavoriteSection(UserFavoriteTypes.Trigger, ltrlTriggers);
                RenderFavoriteSection(UserFavoriteTypes.Graph, ltrlGraphs);
                RenderFavoriteSection(UserFavoriteTypes.QueryBuilder, ltrlQueries);
            }
        }

        private void RenderFavoriteSection(UserFavoriteType favType, Literal targetOutput)
        {
            m_builder = new HtmlBuilder();
            IEnumerable<UserFavorite> favs = m_favorites.Where(f => f.FavType.Equals(favType) && f.Active);
            if (favs.Count() > 0)
            {
                InitializeFavTable();
                foreach (var fav in favs)
                {
                    RenderFavRow(fav);
                }
                m_builder.AddClosingTag(HtmlTags.Table);
            }
            else
            {
                m_builder.AddOpeningTag(HtmlTags.Div);
                m_builder.AddAttribute(HtmlAttributes.Class, "centered centered-text");
                m_builder.AddElement(HtmlTags.Label, RenderOptions.Complete, "No Favorites Found");
                //m_builder.AddAttribute(HtmlAttributes.Class, "fav-none-found");
                m_builder.AddClosingTag(HtmlTags.Div);
            }
            targetOutput.Text = m_builder.RenderBuilder();
        }

        private void InitializeFavTable()
        {
            m_builder.AddOpeningTag(HtmlTags.Table);
            m_builder.AddAttribute(HtmlAttributes.Class, "fav-section__table");
            m_builder.AddElement(HtmlTags.TableHeader, RenderOptions.Complete, "Title");
            m_builder.AddAttribute(HtmlAttributes.Class, "fav-section__table-col1");
            m_builder.AddElement(HtmlTags.TableHeader, RenderOptions.Complete, "Description");
            m_builder.AddAttribute(HtmlAttributes.Class, "fav-section__table-col2");
            m_builder.AddElement(HtmlTags.TableHeader, RenderOptions.Complete, "Delete");
            m_builder.AddAttribute(HtmlAttributes.Class, "fav-section__table-col3");
        }

        private void RenderFavRow(UserFavorite fav)
        {
            m_builder.AddOpeningTag(HtmlTags.TableRow);
            m_builder.AddOpeningTag(HtmlTags.TableData);
            m_builder.AddElement(HtmlTags.Anchor, RenderOptions.Complete, fav.Title);
            //builder.AddAttribute("favSysNo", fav.SysNo.ToString());
            m_builder.AddAttribute(HtmlAttributes.Href, String.Format("javascript: selectFav({0});", fav.SysNo));
            m_builder.AddClosingTag(HtmlTags.TableData);

            //builder.AddElement(HtmlTags.TableData, RenderOptions.Complete, fav.Title);
            m_builder.AddElement(HtmlTags.TableData, RenderOptions.Complete, fav.Description);

            m_builder.AddOpeningTag(HtmlTags.TableData);
            m_builder.AddAttribute(HtmlAttributes.Class, "centered-text");
            m_builder.AddCheckbox("", "", CHECKBOX_PREFIX + fav.SysNo);
            m_builder.AddClosingTag(HtmlTags.TableData);

            m_builder.AddClosingTag(HtmlTags.TableRow);
        }
    
        private void SelectFavorite()
        {
            long favSysNo = GDCoreUtilities.NullSafe.ToLong(hidFavSysNo.Value);
            foreach (var fav in m_favorites)
            {
                if (fav.SysNo.Equals(favSysNo))
                {
                    SessionHelper.SetSelectedFavorite(this.Session, fav);
                    string target = "~/PortalNavHandler.ashx?code=" + fav.NavCode;
                    Response.Redirect(target);
                    break;
                }
            }            
        }

        private void RemoveSelectedFavorites()
        {
            foreach (string key in Request.Form.AllKeys)
            {
                if (key.StartsWith("chk"))
                {
                    long sysNo = GDCoreUtilities.NullSafe.ToLong(key.Replace(CHECKBOX_PREFIX, ""));
                    RemoveFavorite(sysNo);
                }
            }

            // reset the favorites in session and render, now that we have removed the selected favorites
            SessionHelper.SetUserFavorites(this.Session, m_favorites);
            RenderUserFavorites();
        }

        private void RemoveFavorite(long sysNo)
        {
            UserFavorite fav = null;
            foreach (var f in m_favorites)
            {
                if (f.SysNo.Equals(sysNo))
                {
                    fav = f;
                    break;
                }
            }

            if (fav != null)
            {
                fav.Delete(PortalUser.UserName);
                var tmpFavs = m_favorites.ToList();
                tmpFavs.Remove(fav);
                m_favorites = tmpFavs.ToArray();
            }
        }
    }
}