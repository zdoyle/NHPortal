<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PortalMenu.ascx.cs" Inherits="NHPortal.UserControls.PortalMenu" %>

<link rel="stylesheet" type="text/css" href=<%=ResolveClientUrl("~/Style/NavigationMenu.css")%> />
<asp:Menu runat="server" ID="portalMenu" Orientation="Horizontal" CssClass="portal-menu">
    <StaticMenuItemStyle CssClass="menu-item menu-item--static" />
    <DynamicMenuItemStyle CssClass="menu-item menu-item--dynamic" />
</asp:Menu>


<script type="text/javascript">
    function emptyNav() { }

    function portalMenuNav(navCode) {
        var proceed = true;

        if (navCode.toUpperCase() == 'LOGOUT') {
            proceed = confirm('Are you sure you want to Log Out?');
            if (!proceed) {
                return;
            }
        }

        if (proceed) {
            var relativePageURL = '<% this.RenderJSRelativeTargetURL(); %>';
            targetURL = relativePageURL + '?code=' + navCode;
            document.location = targetURL;
        }
    }
</script>
