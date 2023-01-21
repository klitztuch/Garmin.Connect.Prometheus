using Moq;

namespace Garmin.Connect.Prometheus.Lib.Tests;

public class UserSummaryTests
{
    private readonly UserSummary _userSummary;

    public UserSummaryTests()
    {
        var garminConnectClientMock = new Mock<IGarminConnectClient>();
        _userSummary = new UserSummary(garminConnectClientMock.Object);
    }
    [Fact]
    public void Test1()
    {
    }
}