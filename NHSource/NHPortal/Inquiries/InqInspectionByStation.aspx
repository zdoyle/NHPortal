﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InqInspectionByStation.aspx.cs" Inherits="NHPortal.InqInspectionByStation"
    MasterPageFile="~/MasterPages/ReportMaster.master" Title="Test Inquiry Inspection by StationID" %>

<%@ MasterType VirtualPath="~/MasterPages/ReportMaster.master" %>
<%@ Register Src="~/UserControls/DateTextBox.ascx" TagPrefix="GD" TagName="DatePicker" %>

<asp:Content runat="server" ID="vehicleTestInqHistory" ContentPlaceHolderID="ReportContent" ClientIDMode="Static">

    <div class="report-criteria padded centered-text">
        <label class="report-criteria__label">Station:</label>
        <asp:TextBox runat="server" ID="tbStation" MaxLength="8"  ClientIDMode ="Static" />

        <br />
        <br />

        <label class="report-criteria__label">Date:</label>
        <GD:DatePicker runat="server" ID="dpInqDate" ClientIDMode="Static" />
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
