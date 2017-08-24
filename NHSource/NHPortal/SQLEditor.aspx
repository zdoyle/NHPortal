<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SQLEditor.aspx.cs" Inherits="NHPortal.SQLEditor"
    MasterPageFile="~/MasterPages/ReportMaster.master" Title="SQL Report Builder" %>

<%@ MasterType VirtualPath="~/MasterPages/ReportMaster.master" %>

<asp:Content runat="server" ID="content" ContentPlaceHolderID="ReportContent">
    <link type="text/css" rel="stylesheet" href="Style/Adhoc.css" />
    <div class="sql-editor-links">
        <a onclick="DoRecordLayoutPage()" title="View VID Column Identifiers To Get The Table Field Names" href="#">View VID Column Identifiers/RecordLayout</a>
    </div>
    <div class="centered-text padded">
        <b>Enter SQL Statement</b>
        <br />
        <asp:TextBox runat="server" ID="tbEditor" CssClass="textbox-editor" TextMode="MultiLine" MaxLength="4000"></asp:TextBox>
        <asp:Label runat="server" ID="lblNote" CssClass="textbox-editor-notes"></asp:Label>
    </div>
</asp:Content>

<asp:Content runat="server" ID="extraContent" ContentPlaceHolderID="ReportExtraContent">
    
    <div class="padded">
        <img src="Images/div0011.gif" class="centered block-image" />
    </div>
    
    <div id="disclaimer" class="centered">
        <label id="disclaimer__label">
            Disclaimer:
        </label>

        Simply put, you are on your own. What is returned to you will be what the back-room engine 
        interprets your SQL syntax to be requesting.  But you have to be careful to ask the right
        way.  However, SQL syntax can become very complex - any errors and you may not
        get the desired result. We believe all SQL functionality (except those statements that would 
        alter or add data) is supported.  Please go to the
        <a onclick="DoHelpPage()" href="#">Ad Hoc Help Page</a>
        for more information on the free form editor.
    </div>

    <script>
        function DoHelpPage() {
            window.open('HelpPages/AdhocHelp.html');
        }

        function DoRecordLayoutPage() {
            window.open('HelpPages/RecordLayout.html');
        }
    </script>

</asp:Content>
