<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DatePeriodSelector.ascx.cs" Inherits="NHPortal.UserControls.DatePeriodSelector" %>

<%@ Register Src="~/UserControls/DateTextBox.ascx" TagPrefix="GD" TagName="DatePicker" %>
<%@ Register Assembly="NHPortal" Namespace="NHPortal.Classes.WebControls" TagPrefix="GD" %>

<div class="date-selector-container">
    <span class="input-wrapper">
        <label class="report-criteria__label">Report Period</label>
        <GD:ReportPeriodDropDownList runat="server" ID="cboRptPeriod" aria-label="Report Period" role="listbox"></GD:ReportPeriodDropDownList>
    </span>

    <span class="input-wrapper">
        <span class="input-wrapper__controls">
            <label for="dpStart" class="report-criteria__label">Start Date</label>
            <GD:DatePicker runat="server" ID="dpStart" role="textbox" aria-label="start date textbox"/>
        </span>

        <span class="input-wrapper__errors">
            <asp:RequiredFieldValidator runat="server" ID="startDateRequiredField"
                ErrorMessage="* Dates must be filled out."
                Display="Dynamic"
                CssClass="small error" />
            <asp:CompareValidator ID="startDateValidator" runat="server"
                ErrorMessage="* Dates must be in the format: mm/dd/yyyy."
                Operator="DataTypeCheck" Type="Date" Display="Dynamic"
                CssClass="small error" />
            <asp:CompareValidator ID="startDateComparer" runat="server"
                ErrorMessage="* Start date must be less than or equal to end date."
                Operator="LessThanEqual" Type="Date" Display="Dynamic"
                CssClass="small error" />
        </span>
    </span>

    <span class="input-wrapper">
        <span class="input-wrapper__controls">
            <label class="report-criteria__label">End Date</label>
            <GD:DatePicker runat="server" ID="dpEnd" aria-label="end date textbox"/>
        </span>

        <span class="input-wrapper__errors">
            <asp:RequiredFieldValidator runat="server" ID="endDateRequiredField"
                ErrorMessage="* Dates must be filled out."
                Display="Dynamic"
                CssClass="small error" />
            <asp:CompareValidator ID="endDateValidator" runat="server"
                ErrorMessage="* Dates must be in the format: mm/dd/yyyy."
                Operator="DataTypeCheck" Type="Date" Display="Dynamic"
                CssClass="small error" />
            <asp:CompareValidator ID="endDateComparer" runat="server"
                ErrorMessage="* End date must be greater than or equal to start date."
                Operator="GreaterThanEqual" Type="Date" Display="Dynamic"
                CssClass="small error" 
                SetFocusOnError="True" />
        </span>
    </span>
</div>
<script type="text/javascript">
    function parseValues(cbo) {
        var value = cbo.options[cbo.selectedIndex].value;
        var comma = value.indexOf(',');

        var start = value.substr(0, comma);
        var end = value.substr(comma + 1);

        return [start, end];
    }

    function toDisplay(dateValue) {
        var displayValue = '';
        if (dateValue.length > 7) {
            displayValue = dateValue.substr(4, 2) + '/' +
                           dateValue.substr(6, 2) + '/' +
                           dateValue.substr(0, 4);
        }
        return displayValue;
    }
</script>
