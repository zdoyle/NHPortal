<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SafetyDeficiency.aspx.cs" Inherits="NHPortal.Reports.SafetyDeficiency"
    MasterPageFile="~/MasterPages/ReportMaster.master" Title="Gordon-Darby Safety Deficiency Report" %>

<%@ MasterType VirtualPath="~/MasterPages/ReportMaster.master" %>
<%@ Register Src="~/UserControls/DatePeriodSelector.ascx" TagPrefix="GD" TagName="DatePeriodSelector" %>
<%@ Register Assembly="NHPortal" Namespace="NHPortal.Classes.WebControls" TagPrefix="GD" %>

<asp:Content runat="server" ID="content" ContentPlaceHolderID="ReportContent">

    <div class="report-criteria padded centered-text">
        <div class="report-input-container">

            <label class="report-criteria__label">Test Sequence</label>
            <GD:TestSequenceListBox runat="server" ID="lstTestSeq" aria-label="Test Sequence" role="listbox"></GD:TestSequenceListBox>

            <label class="report-criteria__label">Model Year</label>
            <GD:ModelYearListBox runat="server" ID="lstModelYear" aria-label="Model Year" role="listbox"></GD:ModelYearListBox>

            <label class="report-criteria__label">Weight Class</label>
            <GD:WeightClassListBox runat="server" ID="lstWeightClass" aria-label="Weight Class" role="listbox"></GD:WeightClassListBox>

            <label class="report-criteria__label">Vehicle Type</label>
            <GD:VehicleTypeListBox runat="server" ID="lstVehicleType" aria-label="Vehicle Type" role="listbox"></GD:VehicleTypeListBox>
            <br />
            <label class="report-criteria__label">County</label>
            <GD:CountyListBox runat="server" ID="lstCounty" aria-label="County" role="listbox"></GD:CountyListBox><br />
        </div>
        <GD:DatePeriodSelector runat="server" ID="dateSelector"></GD:DatePeriodSelector>

    </div>

</asp:Content>
<asp:Content runat="server" ID="ReportExtraContent" ContentPlaceHolderID="ReportExtraContent">


    <style type="text/css">
        /* syling for pop up tables*/
    
     #expandedDetails .table_header th  {
                background-color: #acacac;
                font-size: 16px;
            }

      #expandedDetails tr:nth-of-type(2n+1) 
        {
            background-color: #F2F2F6;
            font-family: Arial;
                font-size: 14px;
             
        }
          #expandedDetails tr:nth-of-type(2n) {
        background-color: #FFFFFF;
        color: #000;
            font-size: 14px;
    }  
    </style>
</asp:Content>
