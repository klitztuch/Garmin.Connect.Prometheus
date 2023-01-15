using System.Globalization;
using Prometheus;

namespace Garmin.Connect.Prometheus.Lib;

public class WellnessHeartRates : IWellnessHeartRates
{
    private const string METRIC_NAME = "wellnessheartrates";
    private readonly IGarminConnectClient _garminConnectClient;

    private static readonly Gauge RestingHeartRate =
        Metrics.CreateGauge($"{MetricDefaults.METRIC_PREFIX}_{METRIC_NAME}_restingheartrate", "Resting heart rate for the day",
            "export_date");

    private static readonly Gauge MinHeartRate = Metrics.CreateGauge($"{MetricDefaults.METRIC_PREFIX}_{METRIC_NAME}_minheartrate",
        "Minimun heart rate for the day", "export_date");

    private static readonly Gauge MaxHeartRate = Metrics.CreateGauge($"{MetricDefaults.METRIC_PREFIX}_{METRIC_NAME}_maxheartrate",
        "Maximum heart rate for the day", "export_date");

    public WellnessHeartRates(IGarminConnectClient garminConnectClient)
    {
        _garminConnectClient = garminConnectClient;
    }
    public async Task Export()
    {
        var heartRates = await _garminConnectClient.GetWellnessHeartRates(DateTime.Today.AddDays(-1));
        var exportDate = heartRates.CalendarDate.ToString(CultureInfo.CurrentCulture);
        RestingHeartRate.WithLabels(exportDate)
            .Set(heartRates.RestingHeartRate);
        MinHeartRate.WithLabels(exportDate)
            .Set(heartRates.MinHeartRate);
        MaxHeartRate.WithLabels(exportDate)
            .Set(heartRates.MaxHeartRate);
    }
}