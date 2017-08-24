<%@ Page Language="C#"
    AutoEventWireup="true"
    CodeBehind="TriggerTimeBeforeTests.aspx.cs"
    Inherits="NHPortal.TriggerTimeBeforeTests"
    MasterPageFile="~/MasterPages/TriggerMaster.Master" %>

<%@ MasterType VirtualPath="~/MasterPages/TriggerMaster.Master" %>

<asp:Content runat="server" ID="Content" ContentPlaceHolderID="TriggerContent">

    <div id="div-definition-overlay" class="hidden overlay" onclick="hideChartOverlay(this);">
        <div id="divDefinitionOuter" class="definition-body-outer">
            <div id="div-definition" class="definition">
                <!-- #include file="~/Classes/Reports/Triggers/Definitions/TBTTrigger.html" -->
            </div>
        </div>
    </div>

</asp:Content>
