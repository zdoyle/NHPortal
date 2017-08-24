<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AuditHistoryInquiry.aspx.cs" Inherits="NHPortal.Reports.AuditHistoryInquiry"
    MasterPageFile="~/MasterPages/ReportMaster.master" Title="Audit History Inquiry Report" %>

<%@ MasterType VirtualPath="~/MasterPages/ReportMaster.master" %>
<%@ Register Src="~/UserControls/DatePeriodSelector.ascx" TagPrefix="GD" TagName="DatePeriodSelector" %>
<%@ Register Assembly="NHPortal" Namespace="NHPortal.Classes.WebControls" TagPrefix="GD" %>

<asp:Content runat="server" ID="AuditHistortReport" ContentPlaceHolderID="ReportContent">
    <div class="report-criteria padded centered-text" id="reportContent">

        <span class="rescheduled-criteria-top">
            <label class="report-criteria__label" >Inquiry Type</label>
            <GD:InquiryTypeDropDown runat="server" ID="cboInquiryType" OnChange="inquiryChange" aria-label="Inquiry Type" role="listbox"></GD:InquiryTypeDropDown>

            <label for="tbInquiry" class="report-criteria__label" aria-label="oln">ID/OLN</label>
           <asp:TextBox runat="server" ID="tbInquiry" Width="100" MaxLength="10" aria-label="ID/OLN Textbox" contenteditable="true" aria-required="true"/>
          <span class="errtext" id="err_2">
            <asp:RequiredFieldValidator
                runat="server"
                ID="OLNRequiredField"
                ControlToValidate="tbInquiry"
                ErrorMessage="* ID/OLN must be entered."
                CssClass="small error" />
              </span>
        </span>
        <br />
        <br />
        <GD:DatePeriodSelector runat="server" ID="dateSelector"></GD:DatePeriodSelector>

        <div id="div-overlay" class="hidden overlay" aria-label="Audit Details 1">
            <div id="div-details-report-popup" class="largeOverlay_height" aria-label="Audit Details 2" role="document">
                <div id="popup-content" class="expandedDetails" aria-label="Audit Details 3" ">
                    <div id="popup" class="overlaystyle" aria-label="Audit Details 4" >
                        <asp:Literal runat="server" ID="ltrlDetailsRpt" ></asp:Literal>
                        <br />
                        <input type="button" value="Close" tabindex="1"  onclick="javascript: hideOverlay();" />
                    </div>
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
    </div>

    <asp:Literal runat="server" ID="ltrlReport"></asp:Literal>


    <style type="text/css">
        .smallOverlay_height {
            display: inline-block;
            max-width: 60%;
            max-height: 60%;
            overflow-y: auto;
            position: fixed;
            margin: auto;
            top: 0;
            bottom: 0;
            left: 0;
            right: 0;
            text-align: center;
            font-size: 12px;
        }
        #div-details-report-popup{
            z-index:1000;
        }
        .popup_class{
            z-index:8;
        }
    </style>
</asp:Content>
