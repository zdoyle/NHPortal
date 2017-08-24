<%@ Page Title="Gordon-Darby Sticker Denial Report" Language="C#" MasterPageFile="~/MasterPages/ReportMaster.master" AutoEventWireup="true" CodeBehind="StickerDenial.aspx.cs" Inherits="NHPortal.Reports.StickerDenial" %>

<%@ MasterType VirtualPath="~/MasterPages/ReportMaster.Master" %>

<%@ Register Assembly="NHPortal" Namespace="NHPortal.Classes.WebControls" TagPrefix="GD" %>
<%@ Register Src="~/UserControls/DateTextBox.ascx" TagPrefix="GD" TagName="DatePicker" %>

<asp:Content runat="server" ID="Content" ContentPlaceHolderID="ReportContent">

    <asp:Panel runat="server" ID="divDenial" CssClass="main-div">
        <div id="stikMainOuter" class="report-criteria padded centered-text" style="width: 400px;">
            <asp:Panel runat="server" ID="divMainCriteria">
                <table id="denialCriteria">
                    <tr>
                        <td>
                            <label class="report-criteria__label">Date:</label>
                            <GD:DatePicker runat="server" ID="dpStart" />
                        </td>
                        <td style="padding-left: 5px;">
                            <input type="button" value="Denial Reason Key" onclick="javascript: showReasonKey();" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </div>
    </asp:Panel>

    <div id="div-reasonkey-overlay" class="hidden overlay" onclick="hideOverlayDiv(this);">
        <div id="reasonKeyOuter" class="definition-body-outer">
            <div id="div-reasonkey" class="definition">
                <!-- #include file="~/Classes/Reports/StickerReports/StickerDenialReasonKey.html" -->
            </div>
        </div>
    </div>

    <script type="text/javascript">
        function showReasonKey() {
            document.getElementById('div-reasonkey-overlay').classList.remove('hidden');
            // $('#div-reasonkey').scrollTop(0);
        }

        function downloadReasonKey() {
            console.log('attempting to download reason key.');
            window.location.href = '/DownloadHandler.ashx?data=dlfile&file=denialkey';
            return false;

        }
    </script>

</asp:Content>
