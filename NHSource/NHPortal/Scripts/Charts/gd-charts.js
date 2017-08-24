$body = $("body");

$(document).ready(function () {
    // Init report table when document is ready,
    initReportTable();
});

// Handles load screen modal for async requests.
$(document).on({
    ajaxStart: function () { $body.addClass("loading"); },
    ajaxStop: function () { $body.removeClass("loading"); }
});

function initReportTable() {
    console.log('initreporttable called');
    // Init table sorter.
    $("#tblChart").tablesorter();

    // Init on click function to remove any child drill level table rows before sorting.
    $("table.tablesorter tr th.header").click(function () {
        $('.drillrow0').remove();
    });
}

// Handles chart point click for loading a report.
function chartReportClick(seriesID, seriesName, pointName) {
    console.log('Series ID: ' + seriesID + ' Series Name: ' + seriesName + ' Point Name: ' + pointName);
    if (seriesID) {
        document.getElementById('hidSeriesClicked').value = seriesID;
    }
    if (seriesName) {
        document.getElementById('hidSeriesName').value = seriesName;
    }
    if (pointName) {
        document.getElementById('hidPointClicked').value = pointName;
    }
    
    var reportDiv = document.getElementById('div-report'),
        chartDiv = document.getElementById('divChartContainer'),
        btn = document.getElementById('aToggleCharts');

    hideCharts(chartDiv, btn);
    reportDiv.style.display = 'none';
    clearHidActionVal();
    document.getElementById("btnUpdate").click();
}

// Handles chart point click for the main dtc error code chart.
function dtcMainChartClick(seriesID, seriesName, pointName) {
    console.log('dtc main chart click: Series ID: ' + seriesID + 'Series Name: ' + seriesName + ' Point Name: ' + pointName);

    document.getElementById('hidSeriesClicked').value = seriesID;
    document.getElementById('hidSeriesName').value = seriesName;
    document.getElementById('hidPointClicked').value = pointName;

    if (seriesID == "BOTTOM_DRILL_LEVEL") {
        console.log('load the report.');
        chartReportClick(seriesID, seriesName, pointName);
    }
    else {
        // Show 'All P Codes' buttons
        document.getElementById('divAllCodes').style.display = 'block';
        document.getElementById('btnAllCodes').value = "All " + pointName + " Codes";
        document.getElementById('btnAllCodesByMakeModel').value = "All " + pointName + " Codes By Year-Make-Model";
    }
}

function buttonReportClick(reportType) {
    console.log('button report click initialted');
    chartReportClick(reportType, 'ALL_CODES');
}

// 'Show Hide Charts' button action.
function toggleChartDiv() {
    console.log('Toggle Chart Div Activated');
    var chartDiv = document.getElementById('divChartContainer'),
        btn = document.getElementById('aToggleCharts');

    if (chartDiv) {
        if (chartDiv.style.display == 'none') {
            showCharts(chartDiv, btn);
        }
        else {
            hideCharts(chartDiv, btn);
        }
    }
}

// displays the charts on a page.
function showCharts(chartDiv, btn) {
    chartDiv.style.display = 'block';
    btn.innerHTML = 'Hide Charts';
}

// Hides the charts on a a page.
function hideCharts(chartDiv, btn) {
    chartDiv.style.display = 'none';
    btn.innerHTML = 'Show Charts';
}

// Handles report drilldown by sending ajax call for a drill down table to be placed in a new row.
function drillDown(curRow, testDateVal, curDrillLvl, curDrillVal, prevDrillVal) {

    // Main report table.
    var tbl = document.getElementById("tblChart");
    // Params to be sent to code behind.
    var data = { testDateVal: testDateVal, drillVal: curDrillVal, prevDrillValue: prevDrillVal, drillLevel: curDrillLvl };
    console.log('curTestYear: ' + testDateVal + ' curDrillLvl: ' + curDrillLvl + ' curDrillVal: ' + curDrillVal + ' prevDrillVal: ' + prevDrillVal);
    if (tbl) {

        // Remove whitespace and parens to prevent jquery failing.
        testDateVal = testDateVal.replace(/\s+/g, '').replace('(', '').replace(')', '');
        curDrillVal = curDrillVal.replace(/\s+/g, '').replace('(', '').replace(')', '');
        prevDrillVal = prevDrillVal.replace(/\s+/g, '').replace('(', '').replace(')', '');

        // remove child drill row if it already exists
        if ($("." + testDateVal + curDrillVal + curDrillLvl).length) {
            $("." + testDateVal + curDrillVal + curDrillLvl).remove();
        }
        else {
            // Ajax to server and get a table containing drill data.
            $.ajax({
                type: "POST",
                url: "../Charts/GDCharts.aspx/HandleDrillDown",
                timeout: 300000,
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify(data),
                dataType: "json",
                cache: false,
                error: function (jqXHR, exception) { logError(jqXHR, exception); },
                success: function (resp) {

                    // get response table
                    var table = resp.d;
                    var tableName = "tbl" + testDateVal + curDrillVal + prevDrillVal;
                    console.log(tableName);

                    // Add new row to parent table with a single cell containing response table.
                    $(curRow).after("<tr class='drillrow" + curDrillLvl + " " + testDateVal + curDrillVal + curDrillLvl + "' ><td class='drillcell'></td><td colspan='13'>" + table + "</td></tr>");

                    // Use javascript to get table object
                    var newTable = document.getElementById(tableName);

                    // Init table sorter on new table.
                    $(newTable).tablesorter();

                    // Add click event to table headers to remove any child drill rows before sorting.
                    if (curDrillLvl == 0) {
                        $('#' + tableName + " tr th.header").click(function () {
                            console.log('click event occured. remove all rows of: ' + tableName + '.drillrow' + (curDrillLvl + 1));
                            $('#' + tableName + ' tbody tr.drillrow' + (curDrillLvl + 1)).remove();
                        });
                    }
                }
            });
        }
    }
}

// Sends ajax request to notify server of client side error.
function logErrorToServer(jqXHR, exception) {
    // TODO Add error handling here.
}

// Logs ajax error to console.
function logError(jqXHR, exception) {
    console.log('Oops... Something bad happened: ' + exception + ' XHR: ' + jqXHR.responseText);
    alert('Drilldown error occured. Please try later or make another selection.');
}

// Displays the DTC Error codes overlay
function dtcCodes() {
    document.getElementById('div-code-overlay').classList.remove('hidden');
    $('#div-dtc-codes').scrollTop(0);
}

// Displays the DTC Format overlay.
function dtcFormat() {
    document.getElementById('div-format-overlay').classList.remove('hidden');
}

// Hides any active overlay elements.
function closeWindowClick() {
    $('.overlay').addClass('hidden');
}

// Hides an overlay element when the user clicks outside the innver div of the overlay.
function hideChartOverlay(overlay) {
    if (event.target === overlay) {
        overlay.classList.add('hidden');
    }
}

// Clears the hidAction control
function clearHidActionVal() {
    document.getElementById('hidAction').value = '';
}

// Postback functions
function exportToCsv() {
    doPostBack('EXPORT_TO_CSV');
}

function exportToExcel() {
    doPostBack('EXPORT_TO_EXCEL');
}

function exportToPDF() {
    doPostBack('EXPORT_TO_PDF');
}

function saveFav() {
    doPostBack('SAVE_FAVORITE');
}

function runChartReport() {
    console.log('Run chart report initiated.');
    $body.addClass("loading");
    doPostBack("RUN_REPORT");
}

