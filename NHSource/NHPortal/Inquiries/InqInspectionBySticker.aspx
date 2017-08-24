<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InqInspectionBySticker.aspx.cs" Inherits="NHPortal.Inquiries.InqInspectionBySticker"
    MasterPageFile="~/MasterPages/ReportMaster.master" Title="Test Inquiry Inspection by Sticker" %>

<%@ MasterType VirtualPath="~/MasterPages/ReportMaster.master" %>
<%@ Register Assembly="NHPortal" Namespace="NHPortal.Classes.WebControls" TagPrefix="GD" %>

<asp:Content runat="server" ID="inqInspectionBySticker" ContentPlaceHolderID="ReportContent" ClientIDMode="Static">

    <div class="report-criteria padded centered-text">
        <label class="report-criteria__label">Type:</label>
        <asp:DropDownList runat="server" ID="ddlStickerType" ClientIDMode="Static" />

        <label class="report-criteria__label">Series:</label>
        <asp:TextBox runat="server" ID="tbSeries" MaxLength="2" ClientIDMode="Static" />

        <label class="report-criteria__label">Sticker:</label>
        <asp:TextBox runat="server" ID="tbSticker" MaxLength="10" ClientIDMode="Static" />
    </div>

    <div id="div-overlay" class="hidden overlay">
        <div id="div-details-report">
            <div style="overflow-y: auto; height: 90%">
                <asp:Literal runat="server" ID="ltrlDetailsRpt"></asp:Literal>
            </div>
            <div class="centered centered-text padded">
                <input type="button" value="Close" onclick="javascript: hideOverlay();" />
            </div>
        </div>
    </div>

    <asp:HiddenField runat="server" ID="hidSelectedIndex"></asp:HiddenField>
    <asp:HiddenField runat="server" ID="hidShowOverlay" />

    <script type="text/javascript">
        function selectRow(idx) {
            document.getElementById("<%=hidSelectedIndex.ClientID%>").value = idx;
            doPostBack('SELECT_ROW');
        }

        function selectRowD2(idx) {
            document.getElementById("<%=hidSelectedIndex.ClientID%>").value = idx;
            doPostBack('SELECT_ROW_D2');
        }

        window.onload = new function () {
            var showOverlay = document.getElementById("<%=hidShowOverlay.ClientID%>").value;
            if (showOverlay && showOverlay.toUpperCase() === 'TRUE') {
                document.getElementById("div-overlay").classList.remove('hidden');
                document.getElementById("<%=hidShowOverlay.ClientID%>").value = '';
            }
        }

        function hideOverlay() {
            document.getElementById("div-overlay").classList.add('hidden');
        }
    </script>
</asp:Content>

<%--<asp:Content runat="server" ID="ReportExtraContent" ContentPlaceHolderID="ReportExtraContent">
    <asp:Literal runat="server" ID="ltrlSupportDetailsRpt"></asp:Literal>

    <div id="div-overlay1" class="hidden overlay">
        <div id="div-details-report1">
            <asp:Literal runat="server" ID="ltrlPopUpRpt"></asp:Literal>
            <br />
            <input id="btn" type="button" value="Close" onclick="javascript: hideOverlay();" />
        </div>
    </div>


</asp:Content>--%>

