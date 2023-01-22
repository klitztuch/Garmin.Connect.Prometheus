using Garmin.Connect.Prometheus.Lib;

namespace Garmin.Connect.Prometheus.Worker;

public class Worker : BackgroundService
{
    private readonly TimeSpan _delay = new(1, 0, 0, 0);
    private readonly ILogger<Worker> _logger;
    private readonly IUserSummary _userSummary;
    private readonly IWellnessHeartRates _wellnessHeartRates;

    public Worker(ILogger<Worker> logger,
        IWellnessHeartRates wellnessHeartRates,
        IUserSummary userSummary)
    {
        _logger = logger;
        _wellnessHeartRates = wellnessHeartRates;
        _userSummary = userSummary;
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
        await _wellnessHeartRates.Export();
        await _userSummary.Export();
    }
}