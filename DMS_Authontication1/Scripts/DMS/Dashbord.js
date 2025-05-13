
function ChartLiveScreen() {
    $(document).ready(function () {
        $('.counter').each(function () {
            let $this = $(this);
            let target = parseFloat($this.attr('data-count').replace(/,/g, ''));
            let current = 0;
            let duration = 3000;  
            let steps = 60;       
            let increment = target / steps;
    
            function formatNumber(num) {
                return num.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 });
            }
    
            let counter = setInterval(function () {
                current += increment;
                if (current >= target) {
                    current = target;
                    clearInterval(counter);
                }
                $this.text(formatNumber(current));
            }, duration / steps);
        });
    });
    $(document).ready(function () {
        
        function animateCount($el, target) {
            
            $el.stop(true, true);
        
            $({ countNum: 0 }).animate({ countNum: target }, {
                duration: 1200, //  
                easing: 'swing',
                step: function () {
                    $el.text(Math.floor(this.countNum));
                },
                complete: function () {
                    $el.text(this.countNum);
                }
            });
        }
        
        $.ajax({
            type: "GET",
            url: '/Dashboard/getLiveCard',
            success: function (data) { 
                let approval = parseInt(data.Approv) || 0;
                let online = parseInt(data.Onlin) || 0;
        
                animateCount($('#NumberApproval'), approval);
                animateCount($('#NumberOnline'), online);
            }
        });
    });

    const pie4 = document.getElementById('pieChart4');

    const graphSix = document.getElementById('graphChartSix');
    const graphFive = document.getElementById('graphChartFive');
    const graph11 = document.getElementById('graphChart11');


    //new Chart(pie4, {
    //    type: 'pie',
    //    data: {
    //        labels: [
    //            'daily',
    //            'Chronic'
    //        ],
    //        datasets: [{
    //            label: 'عدد الخدمات (ادوية)',
    //            data: [126, 305],
    //            backgroundColor: [
    //                '#8d6aaf',
    //                '#4f256a'
    //            ],
    //            borderWidth: 1
    //        }]
    //    },
    //    options: {
    //        plugins: {
    //            legend: {
    //                display: true, position: 'right',
    //                labels: {
    //                    font: {
    //                        size: 14,
    //                        family: "Open Sans"
    //                    }
    //                }
    //            },
    //            tooltip: {
    //                callbacks: {
    //                    label: function (context) {
    //                        const value = context.raw;
    //                        const total = context.dataset.data.reduce((a, b) => a + b, 0);
    //                        const percent = ((value / total) * 100).toFixed(1);
    //                        return `${context.label}: ${value} (${percent}%)`;
    //                    }
    //                }
    //            }
    //        }
    //    }
    //});
    $.ajax({
        type: "GET",
        url: '/Dashboard/GetMedServiceCounts',
        success: function (response) {
            const labels = response.map(x => x.label);
            const data = response.map(x => x.count);

            new Chart(pie4, {
                type: 'pie',
                data: {
                    labels: labels,
                    datasets: [{
                        label: 'عدد الخدمات (أدوية)',
                        data: data,
                        backgroundColor: [
                            '#2362c1',
                            '#3fa1fc'
                        ],
                        borderWidth: 1
                    }]
                },
                options: {
                    plugins: {
                        legend: {
                            display: true,
                            position: 'right',
                            labels: {
                                font: {
                                    size: 14,
                                    family: "Open Sans"
                                }
                            }
                        },
                        tooltip: {
                            callbacks: {
                                label: function (context) {
                                    const value = context.raw;
                                    const total = context.dataset.data.reduce((a, b) => a + b, 0);
                                    const percent = ((value / total) * 100).toFixed(1);
                                    return `${context.label}: ${value} (${percent}%)`;
                                }
                            }
                        }
                    }
                }
            });
        }
    });
    $.ajax({
        type: "GET",
        url: '/Dashboard/GetAllConsumMed',
        success: function (data) {
            const labels = data.map(item => item.company);
            const grossValues = data.map(item => item.gross);
            const netValues = data.map(item => item.net);
            const percentages = data.map(item => item.percent);

            // const ctx = document.getElementById('myChart').getContext('2d');
            new Chart(graphSix, {
                type: 'bar',
                data: {
                    labels: labels,
                    datasets: [
                        {
                            label: 'إجمالي الاستهلاك',
                            data: grossValues,
                            backgroundColor: '#2362c1',
                            yAxisID: 'amount'
                        },
                        {
                            label: 'صافي الاستهلاك',
                            data: netValues,
                            backgroundColor: '#3fa1fc',
                            yAxisID: 'amount'
                        }
                    ]
                },
                options: {
                    responsive: true,
                    scales: {
                        amount: {
                            type: 'linear',
                            position: 'left',
                            beginAtZero: true,
                            title: {
                                display: true,
                                text: 'القيمة'
                            },
                            ticks: {
                                callback: function (value) {
                                    return value.toLocaleString('en-US');
                                }
                            }
                        }
                    },
                    plugins: {
                        tooltip: {
                            callbacks: {
                                label: function (context) {
                                    const value = context.raw;
                                    return context.dataset.label + ': ' + value.toLocaleString('en-US') + ' جنيه';
                                },
                                afterBody: function (context) {
                                    const index = context[0].dataIndex;
                                    const percent = percentages[index];
                                    return 'النسبة من الاستهلاك: ' + percent + '%';
                                }
                            }
                        },
                        legend: {
                            position: 'top'
                        }
                    }
                }
            });
        }
    });

    $.ajax({
        type: "GET",
        url: '/Dashboard/GetAllConsumOther',
        success: function (data) {
            const labels = data.map(item => item.company);
            const grossValues = data.map(item => item.gross);
            const netValues = data.map(item => item.net);
            const percentages = data.map(item => item.percent);

            // const ctx = document.getElementById('myChart').getContext('2d');
            new Chart(graphFive, {
                type: 'bar',
                data: {
                    labels: labels,
                    datasets: [
                        {
                            label: 'إجمالي الاستهلاك',
                            data: grossValues,
                            backgroundColor: '#2362c1',
                            yAxisID: 'amount'
                        },
                        {
                            label: 'صافي الاستهلاك',
                            data: netValues,
                            backgroundColor: '#3fa1fc',
                            yAxisID: 'amount'
                        }
                    ]
                },
                options: {
                    responsive: true,
                    scales: {
                        amount: {
                            type: 'linear',
                            position: 'left',
                            beginAtZero: true,
                            title: {
                                display: true,
                                text: 'القيمة'
                            },
                            ticks: {
                                callback: function (value) {
                                    return value.toLocaleString('en-US');
                                }
                            }
                        }
                    },
                    plugins: {
                        tooltip: {
                            callbacks: {
                                label: function (context) {
                                    const value = context.raw;
                                    return context.dataset.label + ': ' + value.toLocaleString('en-US') + ' جنيه';
                                },
                                afterBody: function (context) {
                                    const index = context[0].dataIndex;
                                    const percent = percentages[index];
                                    return 'النسبة من الاستهلاك: ' + percent + '%';
                                }
                            }
                        },
                        legend: {
                            position: 'top'
                        }
                    }
                }
            });
        }
    });
    //$.ajax({
    //    type: "GET",
    //    url: '/Dashboard/GetAllConsumOther',
    //    success: function (data) {
    //        const labels = data.map(item => item.company);
    //        const grossValues = data.map(item => item.gross);
    //        const netValues = data.map(item => item.net);
    //        const percentages = data.map(item => item.percent);

    //        new Chart(graphFive, {
    //            type: 'bar',
    //            data: {
    //                labels: labels,
    //                datasets: [
    //                    {
    //                        label: 'إجمالي الاستهلاك',
    //                        data: grossValues,
    //                        backgroundColor: '#2362c1',
    //                        yAxisID: 'amount'
    //                    },
    //                    {
    //                        label: 'صافي الاستهلاك',
    //                        data: netValues,
    //                        backgroundColor: '#3fa1fc',
    //                        yAxisID: 'amount'
    //                    }
    //                ]
    //            },
    //            options: {
    //                responsive: true,
    //                scales: {
    //                    amount: {
    //                        type: 'linear',
    //                        position: 'left',
    //                        beginAtZero: true,
    //                        title: {
    //                            display: true,
    //                            text: 'القيمة'
    //                        },
    //                        ticks: {
    //                            callback: function (value) {
    //                                return value.toLocaleString('en-US');
    //                            }
    //                        }
    //                    }
    //                },
    //                plugins: {
    //                    tooltip: {
    //                        callbacks: {
    //                            label: function (context) {
    //                                const value = context.raw;
    //                                return context.dataset.label + ': ' + value.toLocaleString('en-US') + ' جنيه';
    //                            },
    //                            afterBody: function (context) {
    //                                const index = context[0].dataIndex;
    //                                const percent = percentages[index];
    //                                return 'النسبة من الاستهلاك: ' + percent + '%';
    //                            }
    //                        }
    //                    },
    //                    legend: {
    //                        position: 'top'
    //                    }
    //                }
    //            }
    //        });
    //    }
    //});
    $.ajax({
        type: "GET",
        url: '/Dashboard/GetAllOtherCounts',
        success: function (data) {
            const labels = data.map(item => item.labels);
            const count = data.map(item => item.count);

            new Chart(graph11, {
                type: 'bar',
                data: {
                    labels: labels, 
                    datasets: [{
                        label: 'عدد الخدمات',
                        data: count,
                        backgroundColor: '#3fa1fc',
                        borderColor: '#3fa1fc',
                        borderWidth: 2
                    }]
                },
                options: {
                    indexAxis: 'x',
                    responsive: true,
                    maintainAspectRatio: false,
                    elements: {
                        bar: {
                            borderWidth: 2
                        }
                    },
                    scales: {
                        x: {
                            beginAtZero: true,
                            title: {
                                display: true,
                                text: 'النوع'
                            },
                            ticks: {                               
                                callback: function (value) {
                                    return this.getLabelForValue(value);
                                }
                            }
                        },
                        y: {
                            beginAtZero: true,
                            title: {
                                display: true,
                                text: 'عدد الخدمات'
                            },
                            ticks: {
                                font: {
                                    size: 14,
                                    family: 'Open Sans'
                                }
                            }
                        }
                    },
                    plugins: {
                        tooltip: {
                            callbacks: {
                                label: function (context) {
                                    const value = context.raw;
                                    return 'عدد الخدمات: ' + value.toLocaleString('en-US');
                                },
                                afterBody: function (context) {
                                    const values = context[0].chart.data.datasets[0].data;
                                    const total = values.reduce((sum, val) => sum + val, 0);
                                    const value = context[0].raw;
                                    const percent = ((value / total) * 100).toFixed(2);
                                    return 'النسبة: ' + percent + '%';
                                }
                            }
                        },
                        legend: {
                            position: 'top'
                        }
                    }
                }
            });
        }
    });








}


function ChartConsumptionsScreen() {
    const graph = document.getElementById('graphChart');
    const graphSeven = document.getElementById('graphChartSeven');
    const graphEight = document.getElementById('graphChartEight');

    $.ajax({
        type: "GET",
        url: '/Dashboard/GetCompData',
        success: function (data) {
            const labels = data.map(item => item.company);
            const grossValues = data.map(item => item.gross);
            const netValues = data.map(item => item.net);
            const percentages = data.map(item => item.percent);

            new Chart(graph, {
                type: 'bar',
                data: {
                    labels: labels,
                    datasets: [
                        {
                            label: 'إجمالي الاستهلاك',
                            data: grossValues,
                            backgroundColor: '#2362c1',
                            yAxisID: 'amount'
                        },
                        {
                            label: 'صافي الاستهلاك',
                            data: netValues,
                            backgroundColor: '#3fa1fc',
                            yAxisID: 'amount'
                        }
                    ]
                },
                options: {
                    responsive: true,
                    scales: {
                        amount: {
                            type: 'linear',
                            position: 'left',
                            beginAtZero: true,
                            title: {
                                display: true,
                                text: 'القيمة'
                            },
                            ticks: {
                                callback: function (value) {
                                    return value.toLocaleString('en-US');
                                }
                            }
                        }
                    },
                    plugins: {
                        tooltip: {
                            callbacks: {
                                label: function (context) {
                                    const value = context.raw;
                                    return context.dataset.label + ': ' + value.toLocaleString('en-US') + ' جنيه';
                                },
                                afterBody: function (context) {
                                    const index = context[0].dataIndex;
                                    const percent = percentages[index];
                                    return 'النسبة من الاستهلاك: ' + percent + '%';
                                }
                            }
                        },
                        legend: {
                            position: 'top'
                        }
                    }
                }
            });
        }
    });

    $.ajax({
        type: "GET",
        url: '/Dashboard/GetServiceData',
        success: function (data) {
            const labels = data.map(item => item.company);
            const grossValues = data.map(item => item.gross);
            const netValues = data.map(item => item.net);
            const percentages = data.map(item => item.percent);

            new Chart(graphSeven, {
                type: 'bar',
                data: {
                    labels: labels,
                    datasets: [
                        {
                            label: 'إجمالي الاستهلاك',
                            data: grossValues,
                            backgroundColor: '#2362c1',
                            yAxisID: 'amount'
                        },
                        {
                            label: 'صافي الاستهلاك',
                            data: netValues,
                            backgroundColor: '#3fa1fc',
                            yAxisID: 'amount'
                        }
                    ]
                },
                options: {
                    responsive: true,
                    scales: {
                        amount: {
                            type: 'linear',
                            position: 'left',
                            beginAtZero: true,
                            title: {
                                display: true,
                                text: 'القيمة'
                            },
                            ticks: {
                                callback: function (value) {
                                    return value.toLocaleString('en-US');
                                }
                            }
                        }
                    },
                    plugins: {
                        tooltip: {
                            callbacks: {
                                label: function (context) {
                                    const value = context.raw;
                                    return context.dataset.label + ': ' + value.toLocaleString('en-US') + ' جنيه';
                                },
                                afterBody: function (context) {
                                    const index = context[0].dataIndex;
                                    const percent = percentages[index];
                                    return 'النسبة من الاستهلاك: ' + percent + '%';
                                }
                            }
                        },
                        legend: {
                            position: 'top'
                        }
                    }
                }
            });
        }
    });

    $.ajax({
        type: "GET",
        url: '/Dashboard/GetServiceDetailsData',
        success: function (data) {
            const labels = data.map(item => item.company);
            const grossValues = data.map(item => item.gross);
            const netValues = data.map(item => item.net);
            const percentages = data.map(item => item.percent);

            new Chart(graphEight, {
                type: 'bar',
                data: {
                    labels: labels,
                    datasets: [
                        {
                            label: 'إجمالي الاستهلاك',
                            data: grossValues,
                            backgroundColor: '#2362c1',
                            yAxisID: 'amount'
                        },
                        {
                            label: 'صافي الاستهلاك',
                            data: netValues,
                            backgroundColor: '#3fa1fc',
                            yAxisID: 'amount'
                        }
                    ]
                },
                options: {
                    responsive: true,
                    scales: {
                        amount: {
                            type: 'linear',
                            position: 'left',
                            beginAtZero: true,
                            title: {
                                display: true,
                                text: 'القيمة'
                            },
                            ticks: {
                                callback: function (value) {
                                    return value.toLocaleString('en-US');
                                }
                            }
                        }
                    },
                    plugins: {
                        tooltip: {
                            callbacks: {
                                label: function (context) {
                                    const value = context.raw;
                                    return context.dataset.label + ': ' + value.toLocaleString('en-US') + ' جنيه';
                                },
                                afterBody: function (context) {
                                    const index = context[0].dataIndex;
                                    const percent = percentages[index];
                                    return 'النسبة من الاستهلاك: ' + percent + '%';
                                }
                            }
                        },
                        legend: {
                            position: 'top'
                        }
                    }
                }
            });
        }
    });
}

function ChartProvidersScreen() {
    const graph12 = document.getElementById('graphChart12');
    const graphTwo = document.getElementById('graphChartTwo');
    const graphThree = document.getElementById('graphChartThree');
    const graph18 = document.getElementById('graphChart18');
    const graph25 = document.getElementById('graphChart25');
    const graph26 = document.getElementById('graphChart26');

    $.ajax({
        type: "GET",
        url: '/Dashboard/GetTypeProviderData',
        success: function (data) {
            const labels = data.map(item => item.company);
            const grossValues = data.map(item => item.gross);
            const netValues = data.map(item => item.net);
            const percentages = data.map(item => item.percent);

            new Chart(graphTwo, {
                type: 'bar',
                data: {
                    labels: labels,
                    datasets: [
                        {
                            label: 'إجمالي الاستهلاك',
                            data: grossValues,
                            backgroundColor: '#2362c1',
                            yAxisID: 'amount'
                        },
                        {
                            label: 'صافي الاستهلاك',
                            data: netValues,
                            backgroundColor: '#3fa1fc',
                            yAxisID: 'amount'
                        }
                    ]
                },
                options: {
                    responsive: true,
                    scales: {
                        amount: {
                            type: 'linear',
                            position: 'left',
                            beginAtZero: true,
                            title: {
                                display: true,
                                text: 'القيمة'
                            },
                            ticks: {
                                callback: function (value) {
                                    return value.toLocaleString('en-US');
                                }
                            }
                        }
                    },
                    plugins: {
                        tooltip: {
                            callbacks: {
                                label: function (context) {
                                    const value = context.raw;
                                    return context.dataset.label + ': ' + value.toLocaleString('en-US') + ' جنيه';
                                },
                                afterBody: function (context) {
                                    const index = context[0].dataIndex;
                                    const percent = percentages[index];
                                    return 'النسبة من الاستهلاك: ' + percent + '%';
                                }
                            }
                        },
                        legend: {
                            position: 'top'
                        }
                    }
                }
            });
        }
    });
    $.ajax({
        type: "GET",
        url: '/Dashboard/GetTypeProvider',
        success: function (data) {
            const labels = data.map(item => item.typeprovider);
            const netValues = data.map(item => item.net);
            const percentages = data.map(item => item.percent);

            new Chart(graphThree, {
                type: 'bar',
                data: {
                    labels: labels,
                    datasets: [
                        {
                            label: 'عدد الخدمات',
                            data: netValues,
                            backgroundColor: '#3fa1fc',
                            borderColor: '#3fa1fc',
                            borderWidth: 2
                        }
                    ]
                },
                options: {
                    indexAxis: 'y',
                    responsive: true,
                    maintainAspectRatio: false,
                    elements: {
                        bar: {
                            borderWidth: 2
                        }
                    },
                    scales: {
                        x: {
                            beginAtZero: true,
                            title: {
                                display: true,
                                text: 'العدد'
                            },
                            ticks: {
                                callback: function (value) {
                                    return value.toLocaleString('en-US');
                                }
                            }
                        },
                        y: {
                            ticks: {
                                font: {
                                    size: 14,
                                    family: 'Open Sans'
                                }
                            }
                        }
                    },
                    plugins: {
                        tooltip: {
                            callbacks: {
                                label: function (context) {
                                    const value = context.raw;
                                    return 'عدد الخدمات: ' + value.toLocaleString('en-US');
                                },
                                afterBody: function (context) {
                                    const index = context[0].dataIndex;
                                    const percent = percentages[index];
                                    return 'النسبة: ' + percent + '%';
                                }
                            }
                        },
                        legend: {
                            position: 'top'
                        }
                    }
                }
            });
        }
    });
    new Chart(graph18, {
        type: 'bar',
        data: {
            labels: [
                'صيدلية محمد منير خفاجى',
                'مجموعه صيدليات اى زد لادارة وتطوير المشروعات الدوائية',
                'صيدلية /سامى فريد - المطار - مصر الجديده - القاهرة',
                'مجموعه صيدليات سيف - القاهرة',
                'صيدليات مصر',
                'صيدلية هناء',
                'صيدلية /ابو داود - الاقصر',
                'صيدلية احمد السيد محمود',
                'مجموعه صيدليات خليل - الاسكندريه',
                'صيدليه روشان عمر - مدينه نصر - القاهرة',
                'صيدلية / ريمون كمال - اسوان'
            ],
            datasets: [{
                label: 'صافي الخدمات',
                data: [
                    7363130.68,
                    6218001.07,
                    5934788.21,
                    3853259.21,
                    3635896.46,
                    1790842.61,
                    1733829.09,
                    1525380.60,
                    909809.44,
                    874366.82,
                    763292.14
                ],
                backgroundColor: '#3fa1fc',
                borderWidth: 1
            }]
        },
        options: {
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: {
                        callback: function (value) {
                            return value.toLocaleString('en-US');
                        }
                    }
                }
            },
            plugins: {
                legend: {
                    labels: {
                        font: {
                            size: 14,
                            family: "Open Sans"
                        }
                    }
                }
            }
        }
    });

    new Chart(graph25, {
        type: 'bar',
        data: {
            labels: [
                'مصر للاشعة',
                'الفا سكان',
                'معامل ألفالاب  -16191',
                'معامل المختبر 19014',
                'معامل البرج - 19911',
                'كايرو سكان   - اشعة',
                'البرج سكان - شركة معامل البرج',
                'تكنوسكان - 19989',
                'جراند للتحاليل الطبية',
                'الطاهرة سكان 19173'
            ],
            datasets: [{
                label: 'صافي الخدمات',
                data: [
                    2013706.52,
                    1886609.00,
                    1883208.00,
                    1862339.48,
                    1055032.26,
                    709816.00,
                    297081.70,
                    268949.00,
                    169268.80,
                    77862.50
                ],
                backgroundColor: '#3fa1fc',
                borderWidth: 1
            }]
        },
        options: {
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: {
                        callback: function (value) {
                            return value.toLocaleString('en-US');
                        }
                    }
                }
            },
            plugins: {
                legend: {
                    labels: {
                        font: {
                            size: 14,
                            family: "Open Sans"
                        }
                    }
                }
            }
        }
    });

    new Chart(graph12, {
        type: 'bar',
        data: {
            labels: [
                'مستشفى الجوى',
                'مستشفى القاهرة التخصصى- داخلى',
                'صيدلية محمد منير خفاجى',
                'مجموعه صيدليات اى زد لادارة وتطوير المشروعات الدوائية',
                'صيدلية /سامى فريد - المطار - مصر الجديده - القاهرة',
                'مستشفى النزهة الدولى',
                'مستشفى تاون',
                'مجموعه صيدليات سيف - القاهرة',
                'صيدليات مصر',
                'مستشفى النيل بدراوى'
            ],
            datasets: [{
                label: 'صافي الخدمات',
                data: [
                    11935995.83,
                    10595368.17,
                    7363130.678,
                    6218001.074,
                    5934788.208,
                    4983527.48,
                    4398819.722,
                    3853259.214,
                    3635896.456,
                    3071241.26
                ],
                backgroundColor: '#3fa1fc',
                borderWidth: 1
            }]
        }
        ,
        options: {
            scales: {
                y: {
                    beginAtZero: true
                }
            }
            ,
            plugins: {
                legend: {
                    labels: {
                        font: {
                            size: 14,
                            family: "Open Sans"
                        }
                    }
                }
            }
        }
    });
    new Chart(graph26, {
        type: 'bar',
        data: {
            labels: [
                'مستشفى القاهرة التخصصى - خارجى',
                'مستشفى الجوى',
                'مستشفى النزهة الدولى',
                'مستشفى تاون',
                'مستشفى النيل بدراوى',
                'مستشفى الشروق - المهندسين',
                'مستشفى كيورا النصر',
                'مستشفى شفا التخصصى',
                'مجمع الجلاء الطبى للقوات المسلحة - مصر الجديدة',
                'مستشفى العروبة للخدمات الطبية'
            ],
            datasets: [{
                label: 'صافي الخدمات',
                data: [
                    12287220.99,
                    11935995.83,
                    4983527.48,
                    4398819.72,
                    3071241.26,
                    2292852.75,
                    1666394.14,
                    1617912.83,
                    1542776.45,
                    1441538.31
                ],
                backgroundColor: '#3fa1fc',
                borderWidth: 1
            }]
        },
        options: {
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: {
                        callback: function (value) {
                            return value.toLocaleString('en-US');
                        }
                    }
                }
            },
            plugins: {
                legend: {
                    labels: {
                        font: {
                            size: 14,
                            family: "Open Sans"
                        }
                    }
                }
            }
        }
    });

}

function ChartEmployeesScreen() {
    const doughnut = document.getElementById('doughnutChart');
    const doughnutTwo = document.getElementById('doughnutChartTwo');
    const graphnew = document.getElementById('graphChartNew');
    const graphnew2 = document.getElementById('graphChartNew2');
    const graph33 = document.getElementById('graphChart33');
    const graph34 = document.getElementById('graphChart34');

    new Chart(doughnut, {
        type: 'doughnut',
        data: {
            labels: [
                'ذكر',
                'انثى'
            ],
            datasets: [{
                label: 'My First Dataset',
                data: [10157, 2093],
                backgroundColor: [
                    '#2362c1',
                    '#3fa1fc'
                ],
                borderWidth: 1
            }]
        },
        options: {
            plugins: {
                datalabels: {
                    formatter: (value, context) => {
                        const data = context.chart.data.datasets[0].data;
                        const total = data.reduce((sum, val) => sum + val, 0);
                        const percentage = ((value / total) * 100).toFixed(1) + '%';
                        return `${value} (${percentage})`;
                    },
                    color: '#fff',
                    font: {
                        weight: 'bold'
                    }
                },
                tooltip: {
                    callbacks: {
                        label: function (context) {
                            const data = context.dataset.data;
                            const total = data.reduce((sum, val) => sum + val, 0);
                            const value = context.parsed;
                            const percent = ((value / total) * 100).toFixed(1);
                            return `${context.label}: ${value} (${percent}%)`;
                        }
                    }
                }
            }
        }
    });
    new Chart(doughnutTwo, {
        type: 'doughnut',
        data: {
            labels: [
                'موظفين',
                'معاشات'
            ],
            datasets: [{
                label: 'عدد المنفعين',
                data: [3660, 8590],
                backgroundColor: [
                    '#2362c1',
                    '#3fa1fc'
                ],
                borderWidth: 1
            }]
        },
        options: {
            plugins: {
                datalabels: {
                    formatter: (value, context) => {
                        const data = context.chart.data.datasets[0].data;
                        const total = data.reduce((sum, val) => sum + val, 0);
                        const percentage = ((value / total) * 100).toFixed(1) + '%';
                        return `${value} (${percentage})`;
                    },
                    color: '#fff',
                    font: {
                        weight: 'bold'
                    }
                },
                tooltip: {
                    callbacks: {
                        label: function (context) {
                            const data = context.dataset.data;
                            const total = data.reduce((sum, val) => sum + val, 0);
                            const value = context.parsed;
                            const percent = ((value / total) * 100).toFixed(1);
                            return `${context.label}: ${value} (${percent}%)`;
                        }
                    }
                }
            }
        }
    });
    $.ajax({
        type: "GET",
        url: '/Dashboard/GetConsumType',
        success: function (data) {
            const labels = data.map(item => item.company);
            const grossValues = data.map(item => item.gross);
            const netValues = data.map(item => item.net);
            const percentages = data.map(item => item.percent);

            // const ctx = document.getElementById('myChart').getContext('2d');
            new Chart(graphnew, {
                type: 'bar',
                data: {
                    labels: labels,
                    datasets: [
                        {
                            label: 'إجمالي الاستهلاك',
                            data: grossValues,
                            backgroundColor: '#2362c1',
                            yAxisID: 'amount'
                        },
                        {
                            label: 'صافي الاستهلاك',
                            data: netValues,
                            backgroundColor: '#3fa1fc',
                            yAxisID: 'amount'
                        }
                    ]
                },
                options: {
                    responsive: true,
                    scales: {
                        amount: {
                            type: 'linear',
                            position: 'left',
                            beginAtZero: true,
                            title: {
                                display: true,
                                text: 'القيمة'
                            },
                            ticks: {
                                callback: function (value) {
                                    return value.toLocaleString('en-US');
                                }
                            }
                        }
                    },
                    plugins: {
                        tooltip: {
                            callbacks: {
                                label: function (context) {
                                    const value = context.raw;
                                    return context.dataset.label + ': ' + value.toLocaleString('en-US') + ' جنيه';
                                },
                                afterBody: function (context) {
                                    const index = context[0].dataIndex;
                                    const percent = percentages[index];
                                    return 'النسبة من الاستهلاك: ' + percent + '%';
                                }
                            }
                        },
                        legend: {
                            position: 'top'
                        }
                    }
                }
            });
        }
    });
    //$.ajax({
    //    type: "GET",
    //    url: '/Dashboard/GetConsumType',
    //    success: function (data) {
    //        const labels = data.map(item => item.company);
    //        const grossValues = data.map(item => item.gross);
    //        const netValues = data.map(item => item.net);
    //        const percentages = data.map(item => item.percent);

    //        const options = {
    //            chart: {
    //                type: 'bar',
    //                height: 400
    //            },
    //            series: [
    //                {
    //                    name: 'إجمالي الاستهلاك',
    //                    data: grossValues
    //                },
    //                {
    //                    name: 'صافي الاستهلاك',
    //                    data: netValues
    //                }
    //            ],
    //            xaxis: {
    //                categories: labels
    //            },
    //            tooltip: {
    //                y: {
    //                    formatter: function (value, { series, seriesIndex, dataPointIndex, w }) {
    //                        return value.toLocaleString('en-US') + ' جنيه';
    //                    },
    //                    title: {
    //                        formatter: function (seriesName) {
    //                            return seriesName;
    //                        }
    //                    }
    //                },
    //                custom: function ({ series, seriesIndex, dataPointIndex, w }) {
    //                    return `
    //          <div class="apex-tooltip">
    //            <strong>${w.globals.seriesNames[seriesIndex]}:</strong> 
    //            ${series[seriesIndex][dataPointIndex].toLocaleString('en-US')} جنيه<br>
    //            <span>النسبة من الاستهلاك: ${percentages[dataPointIndex]}%</span>
    //          </div>
    //        `;
    //                }
    //            },
    //            legend: {
    //                position: 'top'
    //            }
    //        };

    //        const chart = new ApexCharts(document.querySelector("#graphnew"), options);
    //        chart.render();
    //    }
    //});
    $.ajax({
        type: "GET",
        url: '/Dashboard/GetConsumGender',
        success: function (data) {
            const labels = data.map(item => item.company);
            const grossValues = data.map(item => item.gross);
            const netValues = data.map(item => item.net);
            const percentages = data.map(item => item.percent);

            // const ctx = document.getElementById('myChart').getContext('2d');
            new Chart(graphnew2, {
                type: 'bar',
                data: {
                    labels: labels,
                    datasets: [
                        {
                            label: 'إجمالي الاستهلاك',
                            data: grossValues,
                            backgroundColor: '#2362c1',
                            yAxisID: 'amount'
                        },
                        {
                            label: 'صافي الاستهلاك',
                            data: netValues,
                            backgroundColor: '#3fa1fc',
                            yAxisID: 'amount'
                        }
                    ]
                },
                options: {
                    responsive: true,
                    scales: {
                        amount: {
                            type: 'linear',
                            position: 'left',
                            beginAtZero: true,
                            title: {
                                display: true,
                                text: 'القيمة'
                            },
                            ticks: {
                                callback: function (value) {
                                    return value.toLocaleString('en-US');
                                }
                            }
                        }
                    },
                    plugins: {
                        tooltip: {
                            callbacks: {
                                label: function (context) {
                                    const value = context.raw;
                                    return context.dataset.label + ': ' + value.toLocaleString('en-US') + ' جنيه';
                                },
                                afterBody: function (context) {
                                    const index = context[0].dataIndex;
                                    const percent = percentages[index];
                                    return 'النسبة من الاستهلاك: ' + percent + '%';
                                }
                            }
                        },
                        legend: {
                            position: 'top'
                        }
                    }
                }
            });
        }
    });
    new Chart(graph33, {
        type: 'bar',
        data: {
            labels: [
                'Less tan 10,000',
                'From 10,000 To 40,000',
                'From 40,000 To 60,000',
                'From 60,000 To 80,000',
                'From 80,000 To 100,000',
                'From 100,000 To 200,000',
                'From 200,000 To 300,000',
                'From 300,000 To 400,000',
                'From 400,000 To 500,000',
                'Grater Than 500,000'
            ],
            datasets: [{
                label: 'صافي الخدمات',
                data: [
                    39289637.70,
                    22969966.86,
                    5391530.31,
                    4676887.30,
                    4717747.70,
                    14012040.78,
                    8050667.74,
                    11089625.95,
                    2666063.58,
                    14572176.28
                ],
                backgroundColor: '#3fa1fc',
                borderWidth: 1
            }]
        },
        options: {
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: {
                        callback: function (value) {
                            return value.toLocaleString('en-US');
                        }
                    }
                }
            },
            plugins: {
                legend: {
                    labels: {
                        font: {
                            size: 14,
                            family: "Open Sans"
                        }
                    }
                }
            }
        }
    });

    new Chart(graph34, {
        type: 'bar',
        data: {
            labels: [
                'Less tan 10,000',
                'From 10,000 To 40,000',
                'From 40,000 To 60,000',
                'From 60,000 To 80,000',
                'From 80,000 To 100,000',
                'From 100,000 To 200,000',
                'From 200,000 To 300,000',
                'From 300,000 To 400,000',
                'From 400,000 To 500,000',
                'Grater Than 500,000'
            ],
            datasets: [{
                label: 'عدد الحالات',
                data: [
                    10451,
                    1382,
                    110,
                    67,
                    53,
                    98,
                    32,
                    32,
                    6,
                    20
                ],
                backgroundColor: '#3fa1fc',
                borderWidth: 1
            }]
        },
        options: {
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: {
                        callback: function (value) {
                            return value.toLocaleString('en-US');
                        }
                    }
                }
            },
            plugins: {
                legend: {
                    labels: {
                        font: {
                            size: 14,
                            family: "Open Sans"
                        }
                    }
                }
            }
        }
    });



    //const genderOptions = {
    //    chart: {
    //        type: 'donut',
    //        height: 350
    //    },
    //    series: [10157, 2093],
    //    labels: ['ذكر', 'انثى'],
    //    colors: ['#4f256a', '#8d6aaf'],
    //    dataLabels: {
    //        enabled: true,
    //        formatter: function (val) {
    //            // Show only the percentage in the chart label
    //            return val.toFixed(1) + '%';
    //        },
    //        style: {
    //            fontWeight: 'bold'
    //        }
    //    },
    //    tooltip: {
    //        y: {
    //            formatter: function (value, { series }) {
    //                const total = series.reduce((sum, v) => sum + v, 0);
    //                const percent = ((value / total) * 100).toFixed(1);
    //                // Show only the percentage in tooltip
    //                return `${value} (${percent}%)`;
    //            }
    //        }
    //    },
    //    legend: {
    //        position: 'bottom'
    //    }
    //};

    //new ApexCharts(document.querySelector("#doughnutChartApexCharts"), genderOptions).render();

    //const typeOptions = {
    //    chart: {
    //        type: 'donut',
    //        height: 350
    //    },
    //    series: [3660, 8590],
    //    labels: ['موظفين', 'معاشات'],
    //    colors: ['#4f256a', '#8d6aaf'],
    //    dataLabels: {
    //        enabled: true,
    //        formatter: function (val, opts) {
    //            const value = opts.w.config.series[opts.seriesIndex];
    //            return `${value} (${val.toFixed(1)}%)`;
    //        },
    //        style: {
    //            fontWeight: 'bold'
    //        }
    //    },
    //    tooltip: {
    //        y: {
    //            formatter: function (value, { series, seriesIndex, w }) {
    //                const total = series.reduce((sum, v) => sum + v, 0);
    //                const percent = ((value / total) * 100).toFixed(1);
    //                return `${value} (${percent}%)`;
    //            }
    //        }
    //    },
    //    legend: {
    //        position: 'bottom'
    //    }
    //};

    //new ApexCharts(document.querySelector("#doughnutChartApexChartsTwo"), typeOptions).render();
}
function ChartComparisonScreen() {
    const graph27 = document.getElementById('graphChart27');
    const graph28 = document.getElementById('graphChart28');
    const graph29 = document.getElementById('graphChart29');
    const graph30 = document.getElementById('graphChart30');
    const graph31 = document.getElementById('graphChart31');
    const graph32 = document.getElementById('graphChart32');

    new Chart(graph27, {
        type: 'bar',
        data: {
            labels: [
                '2022-2023',
                '2023-2024',
                '2024-2025'
            ],
            datasets: [{
                label: 'صافي الصرف (بالجنيه)',
                data: [
                    196228599.45,
                    262420174.68,
                    127436344.19
                ],
                backgroundColor: '#3fa1fc',
                borderWidth: 1
            }]
        },
        options: {
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: {
                        callback: function (value) {
                            return value.toLocaleString('en-US');
                        }
                    }
                }
            },
            plugins: {
                legend: {
                    labels: {
                        font: {
                            size: 14,
                            family: "Open Sans"
                        }
                    }
                }
            }
        }
    });

    //new Chart(graph28, {
    //    type: 'bar',
    //    data: {
    //        labels: [
    //            '2022-2023',
    //            '2023-2024',
    //            '2024-2025'
    //        ],
    //        datasets: [{
    //            label: 'صافي الصرف (بالجنيه)',
    //            data: [
    //                187921405.11,
    //                258984736.25,
    //                156615312.56
    //            ],
    //            backgroundColor: '#3fa1fc',
    //            borderWidth: 1
    //        }]
    //    },
    //    options: {
    //        scales: {
    //            y: {
    //                beginAtZero: true,
    //                ticks: {
    //                    callback: function (value) {
    //                        return value.toLocaleString('en-US');
    //                    }
    //                }
    //            }
    //        },
    //        plugins: {
    //            legend: {
    //                labels: {
    //                    font: {
    //                        size: 14,
    //                        family: "Open Sans"
    //                    }
    //                }
    //            }
    //        }
    //    }
    //});
    new Chart(graph28, {
        type: 'line',
        data: {
            labels: ['2022-2023', '2023-2024', '2024-2025'],
            datasets: [{
                label: 'صافي الصرف (بالجنيه)',
                data: [
                    187921405.11,
                    258984736.25,
                    156615312.56
                ],
                borderColor: '#3fa1fc',
                backgroundColor: '#3fa1fc22',
                fill: true,
                tension: 0.3,
                pointRadius: 5,
                pointBackgroundColor: '#3fa1fc'
            }]
        },
        options: {
            responsive: true,
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: {
                        callback: value => value.toLocaleString('en-US')
                    }
                }
            },
            plugins: {
                legend: {
                    labels: {
                        font: { size: 14, family: 'Open Sans' }
                    }
                }
            }
        }
    });

    new Chart(graph29, {
        type: 'bar',
        data: {
            labels: ['2022-2023', '2023-2024', '2024-2025'],
            datasets: [
                {
                    label: '500118',
                    data: [6340679.72, 8776573.46, 5906272.04],
                    backgroundColor: '#4f256a'
                },
                {
                    label: '500119',
                    data: [62599582.23, 72888166.58, 33095029.03],
                    backgroundColor: '#8d6aaf'
                },
                {
                    label: '500120',
                    data: [41403778.78, 57375382.33, 27667279.53],
                    backgroundColor: '#2362c1'
                },
                {
                    label: '500121',
                    data: [84385009.99, 121013327.16, 59737417.55],
                    backgroundColor: '#3fa1fc'
                },
                {
                    label: '500122',
                    data: [1499548.74, 2366725.15, 1030346.04],
                    backgroundColor: '#e74c3c'
                }
            ]
        },
        options: {
            responsive: true,
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: {
                        callback: value => value.toLocaleString('en-US')
                    }
                }
            },
            plugins: {
                legend: {
                    position: 'top',
                    labels: {
                        usePointStyle: true,        
                        pointStyle: 'circle',       
                        font: {
                            size: 14,
                            family: "Open Sans"
                        }
                    }
                }
            }
        }
    });


    new Chart(graph30, {
        type: 'bar',
        data: {
            labels: ['2022-2023', '2023-2024', '2024-2025'],
            datasets: [

                {
                    label: '500118',
                    data: [5954883.55, 8752211.34, 6809483.49],
                    backgroundColor: '#4f256a'
                },
                {
                    label: '500119',
                    data: [58530209.77, 74472640.61, 40914950.34],
                    backgroundColor: '#8d6aaf'
                },
                {
                    label: '500120',
                    data: [39657803.95, 57107879.25, 33662545.39],
                    backgroundColor: '#2362c1'
                },
                {
                    label: '500121',
                    data: [82366540.56, 116380315.77, 73853841.50],
                    backgroundColor: '#3fa1fc'
                },
                {
                    label: '500122',
                    data: [1411967.27, 2271689.28, 1374491.84],
                    backgroundColor: '#e74c3c'
                }
            ]
        },
        options: {
            responsive: true,
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: {
                        callback: value => value.toLocaleString('en-US')
                    }
                }
            },
            plugins: {
                legend: {
                    position: 'top',
                    labels: {
                        usePointStyle: true,        
                        pointStyle: 'circle',   
                        font: {
                            size: 14,
                            family: 'Open Sans'
                        }
                    }
                }
            }
        }
    });


    new Chart(graph31, {
        type: 'bar',
        data: {
            labels: [
                '2022-2023',
                '2023-2024',
                '2024-2025'
            ],
            datasets: [{
                label: 'عدد الحالات',
                data: [
                    13720,
                    13764,
                    12250
                ],
                backgroundColor: '#3fa1fc',
                borderWidth: 1
            }]
        },
        options: {
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: {
                        callback: function (value) {
                            return value.toLocaleString('en-US');
                        }
                    }
                }
            },
            plugins: {
                legend: {
                    labels: {
                        font: {
                            size: 14,
                            family: "Open Sans"
                        }
                    }
                }
            }
        }
    });

    new Chart(graph32, {
        type: 'bar',
        data: {
            labels: ['2022-2023', '2023-2024', '2024-2025'],
            datasets: [
                {
                    label: '500118',
                    data: [491, 493, 459],
                    backgroundColor: '#4f256a'
                },
                {
                    label: '500119',
                    data: [4904, 4915, 4134],
                    backgroundColor: '#8d6aaf'
                },
                {
                    label: '500120',
                    data: [2689, 2754, 2344],
                    backgroundColor: '#2362c1'
                },
                {
                    label: '500121',
                    data: [5363, 5342, 5089],
                    backgroundColor: '#3fa1fc'
                },
                {
                    label: '500122',
                    data: [273, 260, 224],
                    backgroundColor: '#e74c3c'
                }
            ]
        },
        options: {
            responsive: true,
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: {
                        callback: value => value.toLocaleString('en-US')
                    }
                }
            },
            plugins: {
                legend: {
                    position: 'top',
                    labels: {
                        font: {
                            size: 14,
                            family: "Open Sans"
                        }
                    }
                }
            }
        }
    });

}
function drawGraphChart() {

    //const pie = document.getElementById('pieChart');
    //const pieTwo = document.getElementById('pieChartTwo');
    const pie3 = document.getElementById('pieChart3');



    const graphFour = document.getElementById('graphChartFour');


    new Chart(graphFour, {
        type: 'bar',
        data: {
            labels: [
                'مستشفى الجوى',
                'مستشفى القاهرة التخصصى- داخلى',
                'صيدلية محمد منير خفاجى',
                'مجموعه صيدليات اى زد لادارة وتطوير المشروعات الدوائية',
                'صيدلية /سامى فريد - المطار - مصر الجديده - القاهرة',
                'مستشفى النزهة الدولى',
                'مستشفى تاون',
                'مجموعه صيدليات سيف - القاهرة',
                'صيدليات مصر',
                'مستشفى النيل بدراوى'
            ],
            datasets: [{
                label: 'صافي الخدمات',
                data: [
                    11935995.83,
                    10595368.17,
                    7363130.678,
                    6218001.074,
                    5934788.208,
                    4983527.48,
                    4398819.722,
                    3853259.214,
                    3635896.456,
                    3071241.26
                ],
                backgroundColor: '#3fa1fc',
                borderWidth: 1
            }]
        }
        ,
        options: {
            scales: {
                y: {
                    beginAtZero: true
                }
            }
            ,
            plugins: {
                legend: {
                    labels: {
                        font: {
                            size: 14,
                            family: "Open Sans"
                        }
                    }
                }
            }
        }
    });

    //new Chart(pie, {
    //    type: 'pie',
    //    data: {
    //        labels: [
    //            'عيادات أطباء',
    //            'خدمات خارج الهيئة الطبية',
    //            'مراكز علاج طبيعي',
    //            'صيدليات',
    //            'مراكز اشعة',
    //            'مستشفيات',
    //            'معامل تحاليل'
    //        ],
    //        datasets: [{
    //            label: 'عدد المنتفعين من الخدمة',
    //            data: [361, 739, 801, 49666, 3197, 23625, 5822],
    //            backgroundColor: [
    //                '#2362c1',
    //                '#3fa1fc',
    //                '#f5c542',
    //                '#ff6384',
    //                '#36a2eb',
    //                '#ff9f40',
    //                '#9966ff'
    //            ],
    //            borderWidth: 1
    //        }]
    //    },
    //    options: {
    //        plugins: {
    //            legend: {
    //                display: true, position: 'right',
    //                labels: {
    //                    font: {
    //                        size: 14,
    //                        family: "Open Sans"
    //                    }
    //                }
    //            },
    //            tooltip: {
    //                callbacks: {
    //                    label: function (context) {
    //                        const value = context.raw;
    //                        const total = context.chart._metasets[0].total;
    //                        const percent = ((value / total) * 100).toFixed(2);
    //                        return `${context.label}: ${value.toLocaleString()} (${percent}%)`;
    //                    }
    //                }
    //            }
    //        }
    //    }
    //});


    //new Chart(pieTwo, {
    //    type: 'pie',
    //    data: {
    //        labels: [
    //            'عيادات أطباء',
    //            'مراكز علاج طبيعي',
    //            'صيدليات',
    //            'مراكز أشعة',
    //            'مستشفيات',
    //            'معامل تحاليل'
    //        ],
    //        datasets: [{
    //            label: 'عدد المستفيدين',
    //            data: [57, 56, 126, 57, 305, 16],
    //            backgroundColor: [
    //                '#2362c1',
    //                '#3fa1fc',
    //                '#f5c542',
    //                '#ff6384',
    //                '#36a2eb',
    //                '#9966ff'
    //            ],
    //            borderWidth: 1
    //        }]
    //    },
    //    options: {
    //        plugins: {
    //            legend: {
    //                display: true, position: 'right',
    //                labels: {
    //                    font: {
    //                        size: 14,
    //                        family: "Open Sans"
    //                    }
    //                }
    //            },
    //            tooltip: {
    //                callbacks: {
    //                    label: function (context) {
    //                        const value = context.raw;
    //                        const total = context.dataset.data.reduce((a, b) => a + b, 0);
    //                        const percent = ((value / total) * 100).toFixed(1);
    //                        return `${context.label}: ${value} (${percent}%)`;
    //                    }
    //                }
    //            }
    //        }
    //    }
    //});


    new Chart(pie3, {
        type: 'pie',
        data: {
            labels: [
                'Pharmacy',
                'Lab',
                'Ray',
                'OutPatient',
                'InPatient'
            ],
            datasets: [{
                label: 'عدد الخدمات',
                data: [49666, 5822, 3197, 361, 23625], // 🟡 Adjust values as appropriate for each label
                backgroundColor: [
                    '#ff6384', // Pharmacy
                    '#9966ff', // Lab
                    '#36a2eb', // Ray
                    '#2362c1', // OutPatient
                    '#ff9f40'  // InPatient
                ],
                borderWidth: 1
            }]
        },
        options: {
            plugins: {
                legend: {
                    display: true, position: 'right',
                    labels: {
                        font: {
                            size: 14,
                            family: "Open Sans"
                        }
                    }
                },
                tooltip: {
                    callbacks: {
                        label: function (context) {
                            const value = context.raw;
                            const total = context.chart._metasets[0].total;
                            const percent = ((value / total) * 100).toFixed(2);
                            return `${context.label}: ${value.toLocaleString()} (${percent}%)`;
                        }
                    }
                }
            }
        }
    });










    const genderOptions = {
        chart: {
            type: 'donut',
            height: 350
        },
        series: [10157, 2093],
        labels: ['ذكر', 'انثى'],
        colors: ['#2362c1', '#3fa1fc'],
        dataLabels: {
            enabled: true,
            formatter: function (val, opts) {
                const value = opts.w.config.series[opts.seriesIndex];
                return `${value} (${val.toFixed(1)}%)`;
            },
            style: {
                fontWeight: 'bold'
            }
        },
        tooltip: {
            y: {
                formatter: function (value, { series, seriesIndex, w }) {
                    const total = series.reduce((sum, v) => sum + v, 0);
                    const percent = ((value / total) * 100).toFixed(1);
                    return `${value} (${percent}%)`;
                }
            }
        },
        legend: {
            position: 'bottom'
        }
    };

    new ApexCharts(document.querySelector("#doughnutChartApexCharts"), genderOptions).render();

    const typeOptions = {
        chart: {
            type: 'donut',
            height: 350
        },
        series: [3660, 8590],
        labels: ['موظفين', 'معاشات'],
        colors: ['#2362c1', '#3fa1fc'],
        dataLabels: {
            enabled: true,
            formatter: function (val, opts) {
                const value = opts.w.config.series[opts.seriesIndex];
                return `${value} (${val.toFixed(1)}%)`;
            },
            style: {
                fontWeight: 'bold'
            }
        },
        tooltip: {
            y: {
                formatter: function (value, { series, seriesIndex, w }) {
                    const total = series.reduce((sum, v) => sum + v, 0);
                    const percent = ((value / total) * 100).toFixed(1);
                    return `${value} (${percent}%)`;
                }
            }
        },
        legend: {
            position: 'bottom'
        }
    };

    new ApexCharts(document.querySelector("#doughnutChartApexChartsTwo"), typeOptions).render();

    $(function () {

    })

    function getInfoCard() {

    }
}

