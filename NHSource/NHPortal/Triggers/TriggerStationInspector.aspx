<%@ Page Language="C#"
    AutoEventWireup="true"
    CodeBehind="TriggerStationInspector.aspx.cs"
    Inherits="NHPortal.TriggerStationInspector"
    MasterPageFile="~/MasterPages/TriggerMaster.Master" %>

<%@ MasterType VirtualPath="~/MasterPages/TriggerMaster.Master" %>

<asp:Content runat="server" ID="Content" ContentPlaceHolderID="TriggerContent">
    <div id="stationInspCriteria" class="centered div-station-insp-criteria">
        <table id="tblStationInspCriteria">
            <tr>
                <td class="trig-sito-criteria-left">
                    <label class="criteria-label">Search Filter</label>
                    <asp:DropDownList runat="server" ID="cboSearchFilter"></asp:DropDownList>
                </td>
                <td></td>
                <td class="trig-sito-criteria-right">
                    <label class="criteria-label">ID Number</label>
                    <asp:TextBox runat="server" ID="txtIDNumber" Width="90" MaxLength="10"></asp:TextBox>
                </td>
            </tr>
        </table>
    </div>


    <div id="div-readiness-data-overlay" class="hidden overlay" onclick="hideAuxiliarryDataOverlay(this);">
        <div id="div-si-readiness" class="si-readiness-outer dtc-body">
            <div class="si-readiness-title">
                Readiness Trigger Data
            </div>

            <div id="readinessTable" class=" centered si-readiness">
            </div>
           
            <div id="div-report-buttons" class="centered centered-text">
                <asp:Button runat="server" ID="btnExportReadinessCSV" Text="Export To Csv"
                    CssClass="rpt-button"
                    OnClick="btnExportReadinessCSV_Click"
                    UseSubmitBehavior="true" Visible="true" />

                <asp:Button runat="server" ID="btnExportReadinessXLSX" Text="Export To Excel (XLSX)"
                    CssClass="rpt-button"
                    OnClick="btnExportReadinessXLSX_Click"
                    UseSubmitBehavior="true" Visible="true" />

                <asp:Button runat="server" ID="btnExportReadinessPDF" Text="Export To PDF"
                    CssClass="rpt-button"
                    OnClick="btnExportReadinessPDF_Click"
                    UseSubmitBehavior="true" Visible="true" />
            </div>
            <br />
             <div class="centered div-close">
                <a href="#" class="cancel-button" onclick="hideAuxiliarryDataOverlay();">Close Window</a>
            </div>
        </div>
    </div>

    <script>
        $(document).ready(function () {
            $(".nochar").forceNumeric();
        });
    </script>

</asp:Content>
