﻿<%@ Page Title="Gordon-Darby Sticker Registry Report" Language="C#" MasterPageFile="~/MasterPages/ReportMaster.master" AutoEventWireup="true" CodeBehind="StickerRegistry.aspx.cs" Inherits="NHPortal.Reports.StickerRegistry" %>
<%@ MasterType VirtualPath="~/MasterPages/ReportMaster.Master" %>

<asp:Content ID="ReportMasterContent" ContentPlaceHolderID="ReportContent" runat="server">

    <asp:Panel runat="server" ID="divStickerMain" CssClass="main-div">
        <div id="stikCriteria" class="report-criteria padded centered-text">
            <asp:Panel runat="server" ID="divStationID" CssClass="sticker-stationid" style="padding-bottom:10px;">
                <label class="report-criteria__label">Station ID:</label>
                <asp:TextBox runat="server" ID="txtStationID" Width="90" MaxLength="8"></asp:TextBox>
            </asp:Panel>

            <asp:Panel runat="server" ID="divMainCriteria">
                <label class="report-criteria__label">Type:</label>
                <asp:DropDownList runat="server" Width="60" ID="cboStickerType"></asp:DropDownList>

                <label class="report-criteria__label">Series:</label>
                <asp:TextBox runat="server" ID="txtSeries" Width="60" MaxLength="2"></asp:TextBox>

                <label class="report-criteria__label">Sticker:</label>
                <asp:TextBox runat="server" ID="txtSticker" Width="90" MaxLength="10"></asp:TextBox>
            </asp:Panel>
        </div>
    </asp:Panel>

</asp:Content>