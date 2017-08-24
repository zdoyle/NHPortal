<%@ Page Language="C#"
    AutoEventWireup="true"
    CodeBehind="TriggerWeightedScore.aspx.cs"
    Inherits="NHPortal.TriggerWeightedScore"
    MasterPageFile="~/MasterPages/TriggerMaster.Master" %>

<%@ MasterType VirtualPath="~/MasterPages/TriggerMaster.Master" %>

<asp:Content runat="server" ID="Content" ContentPlaceHolderID="TriggerContent">

    <div id="trigWeightedCriteria" class="centered div-weighted-criteria">
        <table id="tblTrigWeights" class="tbl-weighted-trig">
            <thead>
                <tr>
                    <th>
                        <a href="#/" onclick="clearWeights();">Reset</a>
                    </th>
                    <th>
                        OBD Protocol
                    </th>
                    <th>
                        OBD Rejection
                    </th>
                    <th>
                        OBD Readiness Monitors
                    </th>
                    <th style="width:80px;">
                        eVIN
                    </th>
                    <th>
                        Time Before Test
                    </th>
                    <th>
                        Safety Defect
                    </th>
                    <th>
                        No Voltage
                    </th>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <td>
                        Weighting
                    </td>
                    <td>
                        <asp:TextBox Width="35" MaxLength="3" runat="server" ID="txtProtocol" CssClass="nochar" ClientIDMode="Static"></asp:TextBox>%
                    </td>
                    <td>
                        <asp:TextBox Width="35" MaxLength="3" runat="server" ID="txtRejection" CssClass="nochar" ClientIDMode="Static"></asp:TextBox>%
                    </td>
                    <td>
                        <asp:TextBox Width="35" MaxLength="3" runat="server" ID="txtReadiness" CssClass="nochar" ClientIDMode="Static"></asp:TextBox>%
                    </td>
                    <td>
                        <asp:TextBox Width="35" MaxLength="3" runat="server" ID="txtOBDVIN" CssClass="nochar" ClientIDMode="Static"></asp:TextBox>%
                    </td>
                    <td>
                        <asp:TextBox Width="35" MaxLength="3" runat="server" ID="txtTBT" CssClass="nochar" ClientIDMode="Static"></asp:TextBox>%
                    </td>
                    <td>
                        <asp:TextBox Width="35" MaxLength="3" runat="server" ID="txtSafety" CssClass="nochar" ClientIDMode="Static"></asp:TextBox>%
                    </td>
                    <td>
                        <asp:TextBox Width="35" MaxLength="3" runat="server" ID="txtNoVolt" CssClass="nochar" ClientIDMode="Static"></asp:TextBox>%
                    </td>
                </tr>
            </tbody>
        </table>
        <div id="weightingMsg" class="centered weighting-msg">The trigger weightings must sum to 100.00% </div>
    </div>
    
    <script>
        $(document).ready(function () {
            $(".nochar").forceNumeric();
            $(".nochar").focus(function () { $(this).select(); });
        });

        function clearWeights() {
            $('.nochar').val('');
        }

        function resetWeights() {
            console.log('resetting weights.');
            $('#txtProtocol').val('10');
            $('#txtRejection').val('20');
            $('#txtReadiness').val('20');
            $('#txtOBDVIN').val('20');
            $('#txtTBT').val('10');
            $('#txtSafety').val('10');
            $('#txtNoVolt').val('10');
        }
    </script>

</asp:Content>
