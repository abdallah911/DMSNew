
function ChartLiveScreen() {
    const pie4 = document.getElementById('pieChart4');

    const graphSix = document.getElementById('graphChartSix');
    const graphFive = document.getElementById('graphChartFive');
    const graph11 = document.getElementById('graphChart11');


    new Chart(pie4, {
        type: 'pie',
        data: {
            labels: [
                'daily',
                'Chronic'
            ],
            datasets: [{
                label: 'عدد الخدمات (ادوية)',
                data: [126, 305],
                backgroundColor: [
                    '#36a2eb',
                    '#ff6384'
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
                            const total = context.dataset.data.reduce((a, b) => a + b, 0);
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
                            backgroundColor: '#6c5ce7',
                            yAxisID: 'amount'
                        },
                        {
                            label: 'صافي الاستهلاك',
                            data: netValues,
                            backgroundColor: '#00b894',
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
        url: '/Dashboard/GetAllConsumGroup',
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
                            backgroundColor: '#6c5ce7',
                            yAxisID: 'amount'
                        },
                        {
                            label: 'صافي الاستهلاك',
                            data: netValues,
                            backgroundColor: '#00b894',
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
        url: '/Dashboard/GetTypeProviderLive',
        success: function (data) {
            const labels = data.map(item => item.typeprovider);
            const netValues = data.map(item => item.net);

            new Chart(graph11, {
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

    $.ajax({
        type: "GET",
        url: '/Dashboard/getLiveCard',        
        success: function (data) {                                      
                $('#NumberApproval').text(data.Approv);
                $('#NumberOnline').text(data.Onlin);            
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
                            backgroundColor: '#6c5ce7',
                            yAxisID: 'amount'
                        },
                        {
                            label: 'صافي الاستهلاك',
                            data: netValues,
                            backgroundColor: '#00b894',
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
                            backgroundColor: '#6c5ce7',
                            yAxisID: 'amount'
                        },
                        {
                            label: 'صافي الاستهلاك',
                            data: netValues,
                            backgroundColor: '#00b894',
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
                            backgroundColor: '#6c5ce7',
                            yAxisID: 'amount'
                        },
                        {
                            label: 'صافي الاستهلاك',
                            data: netValues,
                            backgroundColor: '#00b894',
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
                            backgroundColor: '#6c5ce7',
                            yAxisID: 'amount'
                        },
                        {
                            label: 'صافي الاستهلاك',
                            data: netValues,
                            backgroundColor: '#00b894',
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
    new Chart(graph25, {
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
                    '#abe6a4',
                    '#1796f8'
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
                    '#abe6a4',
                    '#1796f8'
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
                            backgroundColor: '#6c5ce7',
                            yAxisID: 'amount'
                        },
                        {
                            label: 'صافي الاستهلاك',
                            data: netValues,
                            backgroundColor: '#00b894',
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
                            backgroundColor: '#6c5ce7',
                            yAxisID: 'amount'
                        },
                        {
                            label: 'صافي الاستهلاك',
                            data: netValues,
                            backgroundColor: '#00b894',
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
    new Chart(graph34, {
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
    new Chart(graph28, {
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
    new Chart(graph29, {
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
    new Chart(graph30, {
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

    new Chart(graph31, {
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
    new Chart(graph32, {
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
    //                '#abe6a4',
    //                '#1796f8',
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
    //                '#abe6a4',
    //                '#1796f8',
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
                    '#abe6a4', // OutPatient
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
        colors: ['#abe6a4', '#1796f8'],
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
        colors: ['#abe6a4', '#1796f8'],
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

