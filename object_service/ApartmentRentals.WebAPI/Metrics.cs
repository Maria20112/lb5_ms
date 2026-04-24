using Prometheus;

public static class UptimeMetrics
{
    // Gauge хранит значение, которое может расти и уменьшаться
    private static readonly Prometheus.Gauge UptimeGauge = Metrics.CreateGauge(
        "uptime_app_seconds",
        "Application uptime in seconds"
    );

    // Метод для обновления значения
    public static void Update()
    {
        // Вычисляем время работы с момента старта
        var uptime = DateTime.UtcNow - _startTime;
        UptimeGauge.Set(uptime.TotalSeconds);
    }

    private static readonly DateTime _startTime = DateTime.UtcNow;
}