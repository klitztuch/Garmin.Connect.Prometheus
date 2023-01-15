using System.Globalization;
using Prometheus;

namespace Garmin.Connect.Prometheus.Worker;

public class Worker : BackgroundService
{
    private static readonly Gauge RestingHeartRate =
        Metrics.CreateGauge("garmin_connect_wellnessheartrates_restingheartrate", "Resting heart rate for the day",
            labelNames: new []
            {
                "export_date"
            });

    private static readonly Gauge MinHeartRate = Metrics.CreateGauge("garmin_connect_wellnessheartrates_minheartrate",
        "Minimun heart rate for the day",
        labelNames: new []
        {
            "export_date"
        });

    private static readonly Gauge MaxHeartRate = Metrics.CreateGauge("garmin_connect_wellnessheartrates_maxheartrate",
        "Maximum heart rate for the day",
        labelNames: new []
        {
            "export_date"
        });

    private readonly TimeSpan _delay = new(1, 0, 0, 0);
    private readonly IGarminConnectClient _garminConnectClient;
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger,
        IGarminConnectClient garminConnectClient)
    {
        _logger = logger;
        _garminConnectClient = garminConnectClient;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            await Run(stoppingToken);

            await Task.Delay(_delay, stoppingToken);
        }
    }

    private async Task Run(CancellationToken stoppingToken)
    {
        var heartRates = await _garminConnectClient.GetWellnessHeartRates(DateTime.Today.AddDays(-1));
        RestingHeartRate.WithLabels(heartRates.CalendarDate.ToString(CultureInfo.CurrentCulture))
            .Set(heartRates.RestingHeartRate);
        MinHeartRate.WithLabels(heartRates.CalendarDate.ToString(CultureInfo.CurrentCulture))
            .Set(heartRates.MinHeartRate);
        MaxHeartRate.WithLabels(heartRates.CalendarDate.ToString(CultureInfo.CurrentCulture))
            .Set(heartRates.MaxHeartRate);
    }
}