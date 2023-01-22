using Garmin.Connect.Models;
using Prometheus;

namespace Garmin.Connect.Prometheus.Lib;

public class UserSummary : IUserSummary
{
    private const string METRIC_NAME = "usersummary";
    private readonly IGarminConnectClient _garminConnectClient;
    public List<Gauge>? Gauges;

    public UserSummary(IGarminConnectClient garminConnectClient)
    {
        _garminConnectClient = garminConnectClient;
        InitMetrics();
    }

    public async Task Export()
    {
        var userSummary = await _garminConnectClient.GetUserSummary(DateTime.Today.AddDays(-1));
        if (Gauges == null)
        {
            return;
        }

        foreach (var gauge in Gauges)
        {
            var property = userSummary.GetType()
                .GetProperties()
                .FirstOrDefault(o => gauge.Name.Contains(o.Name));
            if (double.TryParse(property?.GetValue(userSummary)?.ToString(), out var value))
            {
                gauge.Set(value);
            }
        }
    }

    private void InitMetrics()
    {
        var dummyGarminStats = new GarminStats();
        Gauges = dummyGarminStats.GetType()
            .GetProperties()
            .Select(propertyInfo => Metrics.CreateGauge($"{MetricDefaults.METRIC_PREFIX}_{METRIC_NAME}_{propertyInfo.Name}", propertyInfo.Name)).ToList();
    }
}