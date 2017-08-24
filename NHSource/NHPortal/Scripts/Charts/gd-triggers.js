
$(document).ready(function () {
    // set detail options overlay to select checkbox on row click and set background color on row hover.
    $('#tbl-detail tr').click(function (event) {
        if (event.target.type !== 'checkbox') {
            toggleRow(this);
        }
    });

    //$('.detail-checkbox').css('cursor', 'pointer');

    $('#tbl-detail tr').hover(
      function () {
          $(this).css("background", "#DCF9C9");
      },
      function () {
          $(this).css("background", "");
      }
    );
});

function runTriggerReport() {
    console.log('Run trigger report initiated.');
    $body.addClass("loading");
    doPostBack("RUN_REPORT");
}

function runDetailReport() {
    clearHidActionVal();
    document.getElementById('btnDetails').click();
    closeWindowClick();
}

function showDecileReport() {
    showHideFiltercount(false);
    $("#hidIsBackToReport").val("true");
    var decile = document.getElementById('hidDecile').value;
    chartReportClick('BOTTOM_DRILL_LEVEL', 'eVIN Mismatch Trigger', decile);
}

function showDefinition() {
    document.getElementById('div-definition-overlay').classList.remove('hidden');
    $('#div-definition').scrollTop(0);
}

function showReadinessFormat() {
    document.getElementById('div-readiness-overlay').classList.remove('hidden');
    $('#div-readiness-parent').scrollTop(0);
}

// function to handle chart point click for the trigger chart.
function triggerChartClick(seriesID, seriesName, pointName) {

    showHideFiltercount(false);

    console.log('dtc main chart click: Series ID: ' + seriesID + ' Series Name: ' + seriesName + ' Point Name: ' + pointName);
    document.getElementById('hidSeriesClicked').value = seriesID;
    document.getElementById('hidSeriesName').value = seriesName;
    document.getElementById('hidPointClicked').value = pointName;
    document.getElementById('hidDecile').value = pointName;
    if (seriesID == "BOTTOM_DRILL_LEVEL") {
        console.log('load the report.');
        chartReportClick(seriesID, seriesName, pointName);
    }
}

function triggerDetailClick(stInspId) {

    console.log('trigger detail click initiated.');

    showHideFiltercount(true);

    document.getElementById('div-detail-overlay').classList.remove('hidden');
    if (stInspId) {
        console.log('setting st/insp id');
        document.getElementById('hidPointClicked').value = stInspId;
    }
    else {
    }
}

function toggleRow(row) {
    console.log('togglerow called. row: ' + row);
    var chk = $(row).find("input[type=checkbox]")[0];
    chk.checked = !chk.checked;
}

function showHideFiltercount(isHidden) {
    var isWeighted = document.getElementById('hidIsWeighted'),
        filterSpan = document.getElementById('spnFilterCount');

    if (isWeighted && isWeighted.value == 'true') {
        if (isHidden) {

            filterSpan.classList.add('hidden');
        }
        else {
            filterSpan.classList.remove('hidden');
        }
    }
}

function filterReport() {
    chartReportClick();
}

// Ajax to server and get a table containing station inspector readiness data.
function loadSIReadinessData() {

    $.ajax({
        type: "POST",
        url: "../Charts/GDCharts.aspx/HandleSIReadinessData",
        timeout: 300000,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        cache: false,
        error: function (jqXHR, exception) { logError(jqXHR, exception); },
        success: function (resp) {
            // get response table
            var table = resp.d;
            // Add readiness table to div.
            $("#readinessTable").html(table);
            // Add sorting to table
            $("#tblSIReadiness").tablesorter();
            // Show overlay
            $("#div-readiness-data-overlay").removeClass("hidden");
        }
    });
}

// Clears out the auxiliary report from session and hides the overlay.
function hideAuxiliarryDataOverlay(overlay) {
    if (!overlay) {
        clearAuxTable();
        $("#div-readiness-data-overlay").addClass('hidden');
    }
    else if (event.target === overlay) {
        clearAuxTable();
        overlay.classList.add('hidden');
    }
}

// Ajax to server and clear the auxiliary report.
function clearAuxTable() {
    $.ajax({
        type: "GET",
        url: "../Charts/ChartDataHandler.ashx?data=resetauxreport",
        timeout: 30000,
        cache: false,
        error: function (jqXHR, exception) { logError(jqXHR, exception); },
        success: function (resp) {
            // get response
            console.log(resp);
        }
    });
}