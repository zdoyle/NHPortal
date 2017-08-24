$(document).ready(function () {
    Highcharts.setOptions({
        lang: {
            thousandsSep: ',',
            noData: "No Data Available"
        },
        noData: {
            style: {
                fontSize: '18px',
                fontWeight: 'bold',
                color: '#666666'
            }
        },
        global: {
            useUTC: true,
            timezoneOffset: 7 * 60
        },
        chart: {
            style: {
                fontFamily: 'Arial'
            }
        }
    });
});
