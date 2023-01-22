using Garmin.Connect.Models;
using Moq;

namespace Garmin.Connect.Prometheus.Lib.Tests;

public class UserSummaryTests
{
    private readonly UserSummary _userSummary;

    public UserSummaryTests()
    {
        var garminConnectClientMock = new Mock<IGarminConnectClient>();
        garminConnectClientMock.Setup(client => client.GetUserSummary(It.IsAny<DateTime>()))
            .ReturnsAsync(new GarminStats());
        _userSummary = new UserSummary(garminConnectClientMock.Object);
    }
    [Fact]
    public async Task ExportUserSummaryTest()
    {
        // arrange
        const int expectedMetricsCount = 87;
        // act
        await _userSummary.Export();
        // assert
        var actualMetricsCount = _userSummary.Gauges?.Count;
        Assert.Equal(expectedMetricsCount, actualMetricsCount);
    }
}