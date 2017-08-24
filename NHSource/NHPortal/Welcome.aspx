<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Welcome.aspx.cs" Inherits="NHPortal.Welcome"
    MasterPageFile="~/MasterPages/PortalMaster.master" Title="Welcome" %>

<%@ MasterType VirtualPath="~/MasterPages/PortalMaster.master" %>

<asp:Content runat="server" ID="pageContent" ContentPlaceHolderID="PageContent">
    <link rel="stylesheet" type="text/css" href="Style/Welcome.css" />

    <div class="div-main">
        <div class="container__div-testcount">
            <div class="div-testcount" id="welcometestcount_container"></div>
            <div class="div-rejectcount" id="welcomerejectcount_container"></div>
        </div>
        <div class="container__mainreport-map">
            <div class="div-nh-map-title">
                Click County For
                <br />
                Month-To-Date Main Report
            </div>
            <div class="mainreport-map">
                <span class="image-helper"></span>
                <img id='nhmap' src="Images/nhmap.gif" usemap="#nhmap" />
                <map name="nhmap">
                    <area shape="poly" coords="7, 287, 15, 237, 37, 241, 52, 246, 54, 285, 29, 287" alt="Cheshire" href="Reports/MainReport.aspx?cnty=Cheshire" />
                    <area shape="poly" coords="56, 288, 54, 236, 86, 250, 98, 256, 105, 285, 106, 288" alt="Hillsborough" href="Reports/MainReport.aspx?cnty=Hillsborough" />
                    <area shape="poly" coords="116, 285, 105, 277, 106, 257, 119, 226, 130, 248, 131, 248, 161, 251, 145, 268, 125, 283" alt="Rockingham" href="Reports/MainReport.aspx?cnty=Rockingham" />
                    <area shape="poly" coords="17, 239, 23, 188, 43, 197, 55, 197, 48, 240, 52, 241, 45, 247, 39, 234" alt="Sullivan" href="Reports/MainReport.aspx?cnty=Sullivan" />
                    <area shape="poly" coords="57, 233, 52, 225, 55, 206, 66, 196, 78, 201, 113, 227, 103, 253, 77, 243" alt="Merrimack" href="Reports/MainReport.aspx?cnty=Merrimack" />
                    <area shape="poly" coords="121, 220, 126, 203, 124, 199, 138, 203, 150, 243, 144, 252, 129, 245" alt="Strafford" href="Reports/MainReport.aspx?cnty=Strafford" />
                    <area shape="poly" coords="79, 199, 88, 208, 117, 227, 121, 220, 120, 209, 115, 196, 109, 194, 101, 185, 102, 180, 86, 182" alt="Belknap" href="Reports/MainReport.aspx?cnty=Belknap" />
                    <area shape="poly" coords="57, 199, 31, 191, 54, 140, 56, 115, 76, 107, 103, 125, 107, 144, 103, 158, 86, 158, 90, 176, 76, 195, 76, 188, 74, 195, 69, 192" alt="Grafton" href="Reports/MainReport.aspx?cnty=Grafton" />
                    <area shape="poly" coords="107, 132, 111, 157, 92, 163, 100, 183, 116, 198, 139, 196, 137, 199, 137, 163, 137, 134, 135, 121, 125, 123, 119, 125" alt="Carroll" href="Reports/MainReport.aspx?cnty=Carroll" />
                    <area shape="poly" coords="135, 113, 131, 124, 119, 122, 116, 126, 110, 129, 106, 125, 77, 100, 88, 97, 85, 56, 101, 32, 101, 13, 109, 7, 119, 7, 126, 5, 128, 7, 128, 11" alt="Coos" href="Reports/MainReport.aspx?cnty=Coos" />
                    <area shape="default" />
                </map>
            </div>

        </div>
        <div class="container__nhost-links">
            <span class="nhost-links">NHOST LINKS
                <br />
                <a href="http://www.nh.gov/safety/dmv/" target="_blank">New Hampshire DMV</a>
                <br />
                <a href="http://www.nhostservices.com/" target="_blank">NHOST Services</a>
                <br />
                <a href="http://www.nhinspect.com/fees.html" target="_blank">NH Inspection Fees</a>
                <br />
                <a href="http://www.des.state.nh.us/ard_intro.htm" target="_blank">New Hampshire DES/Air</a>
                <br />
                <a href="http://www.epa.gov/otaq/im.htm" target="_blank">U.S. EPA I/M</a>
                <br />
                <a href="http://www.epa.gov/obd" target="_blank">U.S. EPA OBD</a>
                <br />
                <a href="http://www.gordon-darby.com" target="_blank">Gordon-Darby</a>
            </span>
        </div>
    </div>
 
    <script src="Scripts/jquery-3.1.0.js"></script>
    <script src="Scripts/Charts/highcharts.js"></script>
    <script src="Scripts/Charts/Modules/no-data-to-display.js"></script>
    <script src="Scripts/Charts/welcome.js"></script>
    <script src="Scripts/Charts/gd-global-options.js"></script>
</asp:Content>
