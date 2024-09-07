
var chart;

RentalsChart();
function RentalsChart(startDate, endDate) {
    $.get({
        url: `/Dashboard/GetNumberOfRentalsPerDay?startDate=${startDate}&endDate=${endDate}`,
        success: function (res) {
            DrawChart(res);
        }
    });
}

function DrawChart(data) {
    var element = document.getElementById('RentalsChart');
    var height = parseInt(KTUtil.css(element, 'height'));
    var labelColor = KTUtil.getCssVariableValue('--kt-gray-500');
    var borderColor = KTUtil.getCssVariableValue('--kt-gray-200');
    var baseColor = KTUtil.getCssVariableValue('--kt-info');
    var lightColor = KTUtil.getCssVariableValue('--kt-info-light');


    if (!element) {
        return;
    }

    var options = {
        series: [{
            name: '# of Rentals',
            data: data.map(item => item.value)
        }],
        chart: {
            fontFamily: 'inherit',
            type: 'area',
            height: height,
            toolbar: {
                show: false
            }
        },
        plotOptions: {

        },
        legend: {
            show: false
        },
        dataLabels: {
            enabled: false
        },
        fill: {
            type: 'solid',
            opacity: 1
        },
        stroke: {
            curve: 'smooth',
            show: true,
            width: 3,
            colors: [baseColor]
        },
        xaxis: {
            categories: data.map(item => item.text),
            axisBorder: {
                show: false,
            },
            axisTicks: {
                show: false
            },
            labels: {
                style: {
                    colors: labelColor,
                    fontSize: '12px'
                }
            },
            crosshairs: {
                position: 'front',
                stroke: {
                    color: baseColor,
                    width: 1,
                    dashArray: 3
                }
            },
            tooltip: {
                enabled: true,
                formatter: undefined,
                offsetY: 0,
                style: {
                    fontSize: '12px'
                }
            }
        },
        yaxis: {
            min: 0,
            tickAmount: Math.max(...data.map(item => item.value)),
            labels: {
                style: {
                    colors: labelColor,
                    fontSize: '12px'
                }
            }
        },
        states: {
            normal: {
                filter: {
                    type: 'none',
                    value: 0
                }
            },
            hover: {
                filter: {
                    type: 'none',
                    value: 0
                }
            },
            active: {
                allowMultipleDataPointsSelection: false,
                filter: {
                    type: 'none',
                    value: 0
                }
            }
        },
        tooltip: {
            style: {
                fontSize: '12px'
            },

        },
        colors: [lightColor],
        grid: {
            borderColor: borderColor,
            strokeDashArray: 4,
            yaxis: {
                lines: {
                    show: true
                }
            }
        },
        markers: {
            strokeColor: baseColor,
            strokeWidth: 3
        }
    };

    chart = new ApexCharts(element, options);
    chart.render();
}

$(function () {
    // DatePicker
    var start = moment().subtract(29, "days");
    var end = moment();

    function cb(start, end) {
        $("#DatePicker").html(start.format("MMMM D, YYYY") + " - " + end.format("MMMM D, YYYY"));
    }

    $("#DatePicker").daterangepicker({
        startDate: start,
        endDate: end,
        ranges: {
            "Today": [moment(), moment()],
            "Yesterday": [moment().subtract(1, "days"), moment().subtract(1, "days")],
            "Last 7 Days": [moment().subtract(6, "days"), moment()],
            "Last 30 Days": [moment().subtract(29, "days"), moment()],
            "This Month": [moment().startOf("month"), moment().endOf("month")],
            "Last Month": [moment().subtract(1, "month").startOf("month"), moment().subtract(1, "month").endOf("month")]
        }
    }, cb);

    cb(start, end);


    // get value from datePicker and update chart
    $('#DatePicker').on("change", function () {
        var date = $(this).text();
        var startAndEndDate = date.split(' - ');

        chart.destroy();

        RentalsChart(startAndEndDate[0], startAndEndDate[1]);

    });
});


//SubscriberChart
SubscriberChart();
function SubscriberChart() {
    $.get({
        url: "/Dashboard/GetSubscribersPerCities",
        success: function (res) {
            console.log(res)
            DrawSubscriberChart(res);
        }
    })
}


function DrawSubscriberChart(res) {
    var ctx = document.getElementById('SubscriberChart');

    // Define colors
    var primaryColor = KTUtil.getCssVariableValue('--kt-primary');
    var dangerColor = KTUtil.getCssVariableValue('--kt-danger');
    var successColor = KTUtil.getCssVariableValue('--kt-success');
    var warningColor = KTUtil.getCssVariableValue('--kt-warning');
    var infoColor = KTUtil.getCssVariableValue('--kt-info');

    // Define fonts
    var fontFamily = KTUtil.getCssVariableValue('--bs-font-sans-serif');

    // Chart labels
    const labels = res.map(item=>item.text);

    // Chart data
    const data = {
        labels: labels,
        datasets: [{
            label: '# of Votes',
            data: res.map(item=>item.value),
            borderWidth: 1,
            backgroundColor: [
                primaryColor,
                dangerColor,
                successColor,
                warningColor,
                infoColor,
                "#5F91B6",
                "#D2F6FC",
                "#C8B0D2"
            ],
        }]
    };

    // Chart config
    const config = {
        type: 'pie',
        data: data,
        options: {
            plugins: {
                title: {
                    display: false,
                }
            },
            responsive: true,
        },
        defaults: {
            global: {
                defaultFont: fontFamily
            }
        }
    };

    // Init ChartJS -- for more info, please visit: https://www.chartjs.org/docs/latest/
    var myChart = new Chart(ctx, config);
}