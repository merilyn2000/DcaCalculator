function drawChart(canvasId, chartData) {
    setTimeout(() => {
        var canvasElement = document.getElementById(canvasId);
        if (!canvasElement) {
            console.error("Canvas element not found");
            return;
        }

        var ctx = canvasElement.getContext('2d');
        if (!ctx) {
            console.error("Canvas context not found");
            return;
        }

        var chart = new Chart(ctx, {
            type: 'line',
            data: {
                labels: chartData.labels,
                datasets: [{
                    label: 'Current Value in EUR',
                    data: chartData.values,
                    borderColor: 'rgba(75, 192, 192, 1)',
                    borderWidth: 2,
                    fill: false
                }]
            },
            options: {
                responsive: true,
                scales: {
                    x: {
                        title: {
                            display: true,
                            text: 'Date'
                        }
                    },
                    y: {
                        title: {
                            display: true,
                            text: 'Value in EUR'
                        }
                    }
                }
            }
        });
    }, 100);
}
