using Garmin.Connect;
using Garmin.Connect.Auth;
using Garmin.Connect.Prometheus.Worker;
using Prometheus;

var host = WebApplication.CreateBuilder(args);
var login = host.Configuration["Garmin:Login"];
var password = host.Configuration["Garmin:Password"];
var auth = new BasicAuthParameters(login, password);
var garminContext = new GarminConnectContext(new HttpClient(), auth);


host.Services
    .AddHostedService<Worker>()
    .AddSingleton<IGarminConnectClient, GarminConnectClient>(_ => new GarminConnectClient(garminContext));

var app = host.Build();

// add prometheus
app.UseHttpMetrics();
app.MapMetrics();

app.Run();