<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PortalTestPage.aspx.cs" Inherits="NHPortal.PortalTestPage" MasterPageFile="~/MasterPages/ReportMaster.Master" %>

<%@ MasterType VirtualPath="~/MasterPages/ReportMaster.Master" %>

<%@ Register Src="~/UserControls/DateTextBox.ascx" TagPrefix="GD" TagName="DateTextBox" %>
<%@ Register Src="~/UserControls/DatePeriodSelector.ascx" TagPrefix="GD" TagName="DatePeriodSelector" %>

<asp:Content runat="server" ID="content" ContentPlaceHolderID="ReportContent">
    <asp:Button runat="server" ID="btnReportTest" Text="Test Rendering Report + ReportTable" OnClick="btnReportTest_Click" />

    <asp:Button runat="server" ID="btnTestEx" Text="Test Handled Exception" OnClick="btnTestEx_Click" />

    <asp:Button runat="server" ID="btnTestEx2" Text="Test Unhandled Exception" OnClick="btnTestEx2_Click" />

    <div>
        <asp:Button runat="server" ID="btnTestMsg" Text="Test Hidden Message" OnClick="btnTestMsg_Click" />
        <asp:TextBox runat="server" ID="tbMsg" Text="" MaxLength="50"></asp:TextBox>
    </div>

    <div>
        <GD:DateTextBox runat="server" ID="dtbTest" />
    </div>

    <div>
        <GD:DatePeriodSelector runat="server" ID="selectorTest" />
    </div>

    <div>
        <GD:DatePeriodSelector runat="server" ID="selectorTestTwo" />
    </div>

</asp:Content>
