window.loadCharts = function () {
    const ctx = document.getElementById('departmentPieChart').getContext('2d');
    const chartData = window.chartDataForComponent;

    Chart.register(ChartDataLabels);

    const chart = new Chart(ctx, {
        type: 'pie',
        data: chartData,
        options: {
            responsive: true,
            maintainAspectRatio: false,
            layout: {
                padding: {
                    top: 20,
                    bottom: 20,
                    left: 40,
                    right: 40
                }
            },
            plugins: {
                legend: {
                    display: false
                },
                tooltip: {
                    callbacks: {
                        label: function (context) {
                            const label = context.label || '';
                            const value = context.parsed;
                            const total = context.dataset.data.reduce((a, b) => a + b, 0);
                            const percentage = ((value / total) * 100).toFixed(1);
                            return ` ${label}: ${value} kişi (${percentage}%)`;
                        }
                    }
                },
                datalabels: {
                    backgroundColor: '#fff',
                    borderColor: '#ddd',
                    borderRadius: 4,
                    borderWidth: 1,
                    color: '#333',
                    font: {
                        weight: 'bold',
                        size: 10
                    },
                    padding: {
                        top: 6,
                        bottom: 6,
                        left: 8,
                        right: 8
                    },
                    formatter: function (value, context) {
                        const total = context.dataset.data.reduce((a, b) => a + b, 0);
                        const percentage = ((value / total) * 100).toFixed(1);
                        const label = context.chart.data.labels[context.dataIndex];

                        let shortLabel = label;
                        if (label.length > 19) {
                            shortLabel = label.substring(0, 12) + '...';
                        }

                        return shortLabel + '\n' + percentage + '%';
                    },
                    anchor: 'end',
                    align: 'start',
                    offset: 15,
                    textAlign: 'center',
                    display: function (context) {
                        return true;
                    }
                }
            },
            animation: {
                animateScale: true,
                animateRotate: true
            }
        }
    });

    createSimpleLegend(chartData);
}


function createSimpleLegend(chartData) {
    const legendContainer = document.getElementById('customLegend');
    const total = chartData.datasets[0].data.reduce((a, b) => a + b, 0);

    let legendHTML = '';

    chartData.labels.forEach((label, index) => {
        const color = chartData.datasets[0].backgroundColor[index];

        legendHTML += `
                <div class="simple-legend-item">
                    <span class="simple-legend-color" style="background-color: ${color}"></span>
                    <span class="simple-legend-text">${label}</span>
                </div>
            `;
    });

    legendContainer.innerHTML = legendHTML;
}

window.loadHistogram = function () {
    const rawData = window.monthlyDataForComponent;

    const months = Object.keys(rawData);
    const values = Object.values(rawData);

    const labeledMonths = months.map((month, i) => `${month} (${values[i]})`);

    const ctx = document.getElementById('monthlyApplicationChart').getContext('2d');

    new Chart(ctx, {
        type: 'bar',
        data: {
            labels: labeledMonths,
            datasets: [{
                label: 'Aylık Başvuru Sayısı',
                data: values,
                backgroundColor: 'rgba(75, 192, 192, 0.6)',
                borderColor: 'rgba(75, 192, 192, 1)',
                borderWidth: 1,
                borderRadius: 6,
                barThickness: 40,
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: {
                        stepSize: 1,
                        precision: 0
                    }
                }
            },
            plugins: {
                legend: {
                    display: false
                },
                tooltip: {
                    callbacks: {
                        label: function (context) {
                            return ` ${context.parsed.y} başvuru`;
                        }
                    }
                }
            }
        }
    });
}

window.loadCharts();
window.loadHistogram();
