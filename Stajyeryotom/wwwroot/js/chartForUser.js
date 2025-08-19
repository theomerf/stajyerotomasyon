window.loadHistogramForUser = function () {
    const rawData = window.dailyDataForComponent;

    const days = Object.keys(rawData);
    const values = Object.values(rawData);

    const labeledDays = days.map((day, i) => `${day} (${values[i]})`);

    const ctx = document.getElementById('dailyApplicationChart').getContext('2d');

    new Chart(ctx, {
        type: 'bar',
        data: {
            labels: labeledDays,
            datasets: [{
                label: 'Günlük Rapor Sayısı',
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
                            return ` ${context.parsed.y} rapor`;
                        }
                    }
                }
            }
        }
    });
}

window.loadHistogramForUser();