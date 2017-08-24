<%@ Page Language="C#"
    AutoEventWireup="true"
    CodeBehind="TriggerCommProtocol.aspx.cs"
    Inherits="NHPortal.TriggerCommProtocol"
    MasterPageFile="~/MasterPages/TriggerMaster.Master" %>

<%@ MasterType VirtualPath="~/MasterPages/TriggerMaster.Master" %>

<asp:Content runat="server" ID="Content" ContentPlaceHolderID="TriggerContent">

    <div id="div-definition-overlay" class="hidden overlay" onclick="hideChartOverlay(this);">
        <div id="divDefinitionOuter" class="definition-body-outer">
            <div id="div-definition" class="definition">
                <!-- #include file="~/Classes/Reports/Triggers/Definitions/OBDIICommunicationProtocolTrigger.html" -->
            </div>
        </div>
    </div>

</asp:Content>

