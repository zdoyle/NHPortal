<%@ Page Title="Gordon-Darby Monthly Rejection Report" Language="C#" MasterPageFile="~/MasterPages/ReportMaster.master" AutoEventWireup="true" CodeBehind="MonthlyRejectionReport.aspx.cs" Inherits="NHPortal.Reports.MonthlyRejectionReport" %>

<%@ MasterType VirtualPath="~/MasterPages/ReportMaster.master" %>
<%@ Register Assembly="NHPortal" Namespace="NHPortal.Classes.WebControls" TagPrefix="GD" %>
<%@ Register Src="~/UserControls/DateTextBox.ascx" TagPrefix="GD" TagName="DatePicker" %>

<asp:Content ID="MonthlyRejectionContent" ContentPlaceHolderID="ReportContent" runat="server">
    <link href="../Style/Trigger.css" rel="stylesheet" />

    <div id="divRejectionCriteria" class="centered monthly-rejection-criteria">
        <table>
            <tr>
                <td>
                    <label class="report-criteria__label">Start Date</label>
                    <GD:DatePicker runat="server" ID="dpStart" />
                </td>
                <td>&nbsp;
                </td>
                <td>
                    <label class="report-criteria__label">End Date</label>
                    <GD:DatePicker runat="server" ID="dpEnd" />
                </td>
            </tr>
        </table>
    </div>

</asp:Content>
