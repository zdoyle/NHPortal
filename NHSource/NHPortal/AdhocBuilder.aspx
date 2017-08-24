<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdhocBuilder.aspx.cs" Inherits="NHPortal.AdhocBuilder"
    MasterPageFile="~/MasterPages/ReportMaster.master" Title="Report Builder Tool" %>

<%@ MasterType VirtualPath="~/MasterPages/ReportMaster.master" %>

<asp:Content runat="server" ID="content" ContentPlaceHolderID="ReportContent">
    <link type="text/css" rel="stylesheet" href="Style/Adhoc.css" />

    <div class="centered centered-text">
        <div class="adhoc-help-div">
            <a onclick="DoHelpPage()" href="#">Ad Hoc Help Page</a>
            &nbsp;&nbsp;&nbsp;
            <a onclick="DoDictionaryPage()" href="#">Data Dictionary</a>
            &nbsp;&nbsp;&nbsp;
            <a onclick="DoRecordLayoutPage()" href="#">Record Layout</a>
            &nbsp;&nbsp;&nbsp;
            <a onclick="DoSafetyPage()" href="#">Safety Inspection Regulations</a>
        </div>

        <asp:CheckBox runat="server" ID="chkAllFields" Text="Include All Fields" />
        &nbsp; 
        <a href="#/" onclick="resetAdhocFields();">Clear All</a>
        <div id="adhoc-builder-wrap" class="padded">
            <table id="adhoc-builder-table-header" class="adhoc-builder-table">
                <tr>
                    <td>Field Name</td>
                    <td>Include</td>
                    <td>Criteria</td>
                    <td>Count</td>
                    <td>Sum</td>
                    <td>Min</td>
                    <td>Max</td>
                    <td>Avg</td>
                </tr>
            </table>
            <div id="adhoc-builder-scrollable">
                <asp:PlaceHolder runat="server" ID="phAdhoc"></asp:PlaceHolder>
            </div>
        </div>
    </div>

    <script>
        // Variable to define if page is in initial load state.
        var initialLoad = true;
        // jQuery global variables for all criteria listboxes, textboxes and checkbox parent td cells.
        var $listbox = $('.adhoc-builder-listbox'),
            $textbox = $('.adhoc-builder-txt'),
            $chkCell = $('.checkbox-cell');

        // This function will run automatically when the document loads and is ready for modification.
        $(document).ready(function () {
            if (initialLoad) {
                initialLoad = false;
                addCriteriaEvents();
            }

            highlightCells();
        });

        // Adds events to the criteria for selecting and highlighting.
        function addCriteriaEvents() {
            // Add onchange event to highlight cells that have a checked checkbox.
            $('input', $chkCell).on('change', function () {
                $(this).closest('td').toggleClass("highlight", this.checked);
            });

            //  Select the contained checkbox if a table cell is clicked.
            $chkCell.click(function (event) {
                if (event.target.type !== 'checkbox') {
                    toggleCell(this);
                }
            });

            // Highlight the cell containing the textbox if it contains text after its value changes.
            $textbox.change(
              function () {
                  $(this).closest('td').toggleClass('highlight', $(this).val() != '');
              }
            );

            // Highlight the cell containing the textbox if it contains values after a keypress.
            $textbox.keyup(
              function () {
                  $(this).closest('td').toggleClass('highlight', $(this).val() != '');
              }
            );

            // Add onchange event to highlight the listboxes if they have something selected other than 'All'.
            $listbox.change(
              function () {
                  $(this).closest('td').toggleClass('highlight', $(this).val() != '');
              }
            );
        }

        // Toggles the checkbox contained in the cell parameter and highlights the cell.
        function toggleCell(cell) {
            var chk = $(cell).find("input[type=checkbox]")[0];
            chk.checked = !chk.checked;
            $(cell).toggleClass("highlight", this.checked);
        }

        // Resets all adhoc criteria and removes any highlighting.
        function resetAdhocFields() {
            // Clear all criteria textboxes.
            $textbox.val('');

            // Clear all criteria listboxes and select the 'All' option.
            $listbox.val('');
            $listbox[0].selectedIndex = 0;
            $listbox.scrollTop(0);

            // Uncheck all 'include' and 'aggregate' checkboxes.
            $('input', $chkCell).prop('checked', false);

            // Remove any highlighting for the table cells.
            $chkCell.removeClass('highlight')
            $listbox.closest('td').removeClass('highlight');
            $textbox.closest('td').removeClass('highlight');
        }

        // Handles any client side actions that need to be performed when a report is rendered to the page.
        function reportWasRendered() {
            // Scroll down to report div export buttons.
            $('html, body').animate({
                scrollTop: $("#div-report-buttons").offset().top
            }, 1000);
        }

        // Apply highlighting to any critieria cells that are active.
        function highlightCells() {
            $('input', $chkCell).each(function () {
                $(this).closest('td').toggleClass('highlight', this.checked);
            });

            $textbox.each(function () {
                $(this).closest('td').toggleClass('highlight', $(this).val() != '');
            });

            $listbox.each(function () {
                $(this).closest('td').toggleClass('highlight', $(this).val() != '');
            });
        }

        function DoHelpPage() {
            window.open('HelpPages/AdhocHelp.html');
        }

        function DoDictionaryPage() {
            window.open('HelpPages/DataDictionary.html');
        }

        function DoRecordLayoutPage() {
            window.open('HelpPages/RecordLayout.html');
        }

        function DoSafetyPage() {
            window.open('HelpPages/SafetyInspectionRegulations.html');
        }
    </script>

</asp:Content>
