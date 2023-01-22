FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY "Garmin.Connect.Prometheus.Worker.csproj" "Garmin.Connect.Prometheus.Worker/"
RUN dotnet restore "Garmin.Connect.Prometheus.Worker/Garmin.Connect.Prometheus.Worker.csproj"
COPY . "Garmin.Connect.Prometheus.Worker/"
WORKDIR "/src/Garmin.Connect.Prometheus.Worker"
RUN dotnet build "Garmin.Connect.Prometheus.Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Garmin.Connect.Prometheus.Worker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Garmin.Connect.Prometheus.Worker.dll"]
