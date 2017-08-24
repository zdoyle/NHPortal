<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MyFavorites.aspx.cs" Inherits="NHPortal.MyFavorites"
    MasterPageFile="~/MasterPages/PortalMaster.master" Title="My Favorites" %>

<%@ MasterType VirtualPath="~/MasterPages/PortalMaster.master" %>

<asp:Content runat="server" ID="content" ContentPlaceHolderID="PageContent">
    <div id="div-fav-container">
        <div id="div-my-reports" class="fav-section">
            <span class="fav-section__title">My Reports</span>
            <asp:Literal runat="server" ID="ltrlReports"></asp:Literal>
        </div>

        <div id="div-my-triggers" class="fav-section">
            <span class="fav-section__title">My Triggers</span>
            <asp:Literal runat="server" ID="ltrlTriggers"></asp:Literal>
        </div>

        <div id="div-my-graphs" class="fav-section">
            <span class="fav-section__title">My Graphs</span>
            <asp:Literal runat="server" ID="ltrlGraphs"></asp:Literal>
        </div>

        <div id="div-my-queries" class="fav-section">
            <span class="fav-section__title">My Queries</span>
            <asp:Literal runat="server" ID="ltrlQueries"></asp:Literal>
        </div>
    </div>

    <div class="centered centered-text">
        <input type="button" value="Remove Selected" onclick="javascript: removeFavs();" />
    </div>

    <asp:HiddenField runat="server" ID="hidFavSysNo" />

    <script type="text/javascript">
        function selectFav(favSysNo) {
            document.getElementById('<%=hidFavSysNo.ClientID%>').value = favSysNo;
            doPostBack('SELECT_FAV');
        }

        function removeFavs() {
            var proceed = confirm('Remove selected favorites?');
            if (proceed) {
                doPostBack('REMOVE_SELECTED_FAVS');
            }
        }
    </script>
</asp:Content>
