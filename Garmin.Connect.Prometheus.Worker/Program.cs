using Garmin.Connect;
using Garmin.Connect.Auth;
using Garmin.Connect.Prometheus.Lib;
using Garmin.Connect.Prometheus.Worker;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Prometheus;

var host = WebApplication.CreateBuilder(args);
var login = host.Configuration["Garmin:Login"];
var password = host.Configuration["Garmin:Password"];
var auth = new BasicAuthParameters(login, password);
var garminContext = new GarminConnectContext(new HttpClient(), auth);

host.Services
    .AddHostedService<Worker>()
    .AddSingleton<IGarminConnectClient, GarminConnectClient>(_ => new GarminConnectClient(garminContext))
    .AddSingleton<IWellnessHeartRates, WellnessHeartRates>()
    .AddSingleton<IUserSummary, UserSummary>();

// healthcheck
host.Services.AddHealthChecks()
    .AddCheck("Parameter", () =>
    {
        if (login == null || password == null)
        {
            return HealthCheckResult.Unhealthy("Missing login parameters");
        }

        return HealthCheckResult.Healthy();
    });

var app = host.Build();

// add prometheus
app.UseHttpMetrics();
app.MapMetrics();

app.Run();