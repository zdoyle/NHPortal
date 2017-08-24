<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DateTextBox.ascx.cs" Inherits="NHPortal.UserControls.DateTextBox" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<asp:TextBox runat="server" ID="tbDate" MaxLength="10" CssClass="txt-date-input"  arial-label="Date Textbox"></asp:TextBox>
<asp:ImageButton runat="server" ID="imgCalendar" CausesValidation="false" ImageUrl="~/Images/calendar.png" alt="Calender Date Picker"/>

<div>
    <asp:RequiredFieldValidator runat="server"
        ID="valDateRequired"
        ErrorMessage="Dates must be filled out."
        Display="Dynamic"
        ControlToValidate="tbDate"
        CssClass="small error"
        ValidationGroup="valGroupDate" />
</div>
<div>
    <asp:CompareValidator runat="server"
        ID="valDateFormat"
        ErrorMessage="Dates must be in the format: mm/dd/yyyy."
        Display="Dynamic"
        Operator="DataTypeCheck"
        Type="Date"
        ControlToValidate="tbDate"
        CssClass="small error" />
</div>

<ajaxToolkit:CalendarExtender runat="server" ID="CalenderExtender"
    TargetControlID="tbDate"
    PopupButtonID="imgCalendar">
</ajaxToolkit:CalendarExtender>
