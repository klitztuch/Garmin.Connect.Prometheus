using System.Globalization;
using Garmin.Connect.Models;
using Microsoft.Extensions.Logging;
using Prometheus;

namespace Garmin.Connect.Prometheus.Lib;

public class WellnessHeartRates : IWellnessHeartRates
{
    private const string METRIC_NAME = "wellnessheartrates";
    private readonly IGarminConnectClient _garminConnectClient;
    private readonly ILogger<WellnessHeartRates> _logger;

    private static readonly Gauge RestingHeartRate =
        Metrics.CreateGauge($"{MetricDefaults.METRIC_PREFIX}_{METRIC_NAME}_restingheartrate", "Resting heart rate for the day",
            "export_date");

    private static readonly Gauge MinHeartRate = Metrics.CreateGauge($"{MetricDefaults.METRIC_PREFIX}_{METRIC_NAME}_minheartrate",
        "Minimun heart rate for the day", "export_date");

    private static readonly Gauge MaxHeartRate = Metrics.CreateGauge($"{MetricDefaults.METRIC_PREFIX}_{METRIC_NAME}_maxheartrate",
        "Maximum heart rate for the day", "export_date");

    public WellnessHeartRates(IGarminConnectClient garminConnectClient,
        ILogger<WellnessHeartRates> logger)
    {
        _garminConnectClient = garminConnectClient;
        _logger = logger;
    }
    public async Task Export()
    {
        GarminHr heartRates;
        try
        {
            heartRates = await _garminConnectClient.GetWellnessHeartRates(DateTime.Today.AddDays(-1));
        }
        catch (Exception e)
        {
            _logger.LogError("Garmin Login Error: {Exception}", e);
            return;
        }
        var exportDate = heartRates.CalendarDate.ToString(CultureInfo.CurrentCulture);
        RestingHeartRate.WithLabels(exportDate)
            .Set(heartRates.RestingHeartRate);
        MinHeartRate.WithLabels(exportDate)
            .Set(heartRates.MinHeartRate);
        MaxHeartRate.WithLabels(exportDate)
            .Set(heartRates.MaxHeartRate);
    }
}