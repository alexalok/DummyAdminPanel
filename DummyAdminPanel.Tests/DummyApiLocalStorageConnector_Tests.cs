using System;
using System.Threading;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using DummyAdminPanel.Connectors.DummyApi;
using DummyAdminPanel.Connectors.DummyApi.LocalStorage;
using DummyAdminPanel.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace DummyAdminPanel.Tests;

public class DummyApiLocalStorageConnector_Tests
{
    IOptions<DummyApiSettings> _settingsStub = Options.Create<DummyApiSettings>(new()
    {
        CacheLifetimeSeconds = 1000
    });

    [Fact]
    public async Task Ensure_TryGetAllUsers_Returns_Null_When_Expired()
    {
        // Arrange
        var ls = new Mock<ILocalStorageService>(MockBehavior.Strict);
        ls.Setup(l => l.GetItemAsync<DateTime?>(DummyApiLocalStorageConnector.CacheKeyForExpiry, It.IsAny<CancellationToken?>()))
            .ReturnsAsync(DateTime.MinValue);
        ls.Setup(l => l.ClearAsync(It.IsAny<CancellationToken?>())).Returns(ValueTask.CompletedTask);

        var connector = new DummyApiLocalStorageConnector(_settingsStub, ls.Object, new SystemClock());

        // Act
        var users = await connector.TryGetAllUsers(0);

        // Assert
        Assert.Null(users);
    }

    [Fact]
    public async Task Ensure_TryGetAllUsers_Clears_Cache_When_Expired()
    {
        // Arrange
        var ls = new Mock<ILocalStorageService>(MockBehavior.Strict);
        ls.Setup(l => l.GetItemAsync<DateTime?>(DummyApiLocalStorageConnector.CacheKeyForExpiry, It.IsAny<CancellationToken?>()))
            .ReturnsAsync(DateTime.MinValue);
        ls.Setup(l => l.ClearAsync(It.IsAny<CancellationToken?>())).Returns(ValueTask.CompletedTask);

        var connector = new DummyApiLocalStorageConnector(_settingsStub, ls.Object, new SystemClock());

        // Act
        var users = await connector.TryGetAllUsers(0);

        // Assert
        ls.Verify(l => l.ClearAsync(It.IsAny<CancellationToken?>()), Times.Once);
    }

    [Fact]
    public async Task Ensure_TryGetAllUsers_Does_Not_Clear_Cache_When_Not_Expired()
    {
        // Arrange
        var ls = new Mock<ILocalStorageService>(MockBehavior.Strict);
        ls.Setup(l => l.GetItemAsync<DateTime?>(DummyApiLocalStorageConnector.CacheKeyForExpiry, It.IsAny<CancellationToken?>()))
            .ReturnsAsync(DateTime.MaxValue);
        ls.Setup(l => l.ClearAsync(It.IsAny<CancellationToken?>())).Returns(ValueTask.CompletedTask);
        ls.Setup(l => l.GetItemAsync<DummyApiList<UserPreview>?>(It.IsAny<string>(), It.IsAny<CancellationToken?>())).ReturnsAsync(() => null);

        var connector = new DummyApiLocalStorageConnector(_settingsStub, ls.Object, new SystemClock());

        // Act
        var users = await connector.TryGetAllUsers(0);

        // Assert
        ls.Verify(l => l.ClearAsync(It.IsAny<CancellationToken?>()), Times.Never);
    }

    [Fact]
    public async Task Ensure_CachePageOfUsers_Clears_Cache_If_Expired()
    {
        // Arrange
        var ls = new Mock<ILocalStorageService>(MockBehavior.Strict);
        ls.Setup(l => l.GetItemAsync<DateTime?>(DummyApiLocalStorageConnector.CacheKeyForExpiry, It.IsAny<CancellationToken?>()))
            .ReturnsAsync(DateTime.MinValue);
        ls.Setup(l => l.ClearAsync(It.IsAny<CancellationToken?>())).Returns(ValueTask.CompletedTask);
        ls.Setup(l => l.SetItemAsync<It.IsAnyType>(It.IsAny<string>(), It.IsAny<It.IsAnyType>(),
            It.IsAny<CancellationToken?>())).Returns(() => ValueTask.CompletedTask);

        var connector = new DummyApiLocalStorageConnector(_settingsStub, ls.Object, new SystemClock());

        // Act
        await connector.CachePageOfUsers(new(null!, 1, 0, 1));

        // Assert
        ls.Verify(l => l.ClearAsync(It.IsAny<CancellationToken?>()), Times.Once);
    }

    [Fact]
    public async Task Ensure_CachePageOfUsers_Updates_Expiry_If_Expired()
    {
        // Arrange
        var ls = new Mock<ILocalStorageService>(MockBehavior.Strict);
        ls.Setup(l => l.GetItemAsync<DateTime?>(DummyApiLocalStorageConnector.CacheKeyForExpiry, It.IsAny<CancellationToken?>()))
            .ReturnsAsync(DateTime.MinValue);
        ls.Setup(l => l.ClearAsync(It.IsAny<CancellationToken?>())).Returns(ValueTask.CompletedTask);
        ls.Setup(l => l.SetItemAsync<It.IsAnyType>(It.IsAny<string>(), It.IsAny<It.IsAnyType>(),
            It.IsAny<CancellationToken?>())).Returns(() => ValueTask.CompletedTask);

        var now = DateTimeOffset.UnixEpoch;
        var clock = new Mock<ISystemClock>(MockBehavior.Strict);
        clock.Setup(c => c.UtcNow).Returns(now);

        var connector = new DummyApiLocalStorageConnector(_settingsStub, ls.Object, clock.Object);

        // Act
        await connector.CachePageOfUsers(new(null!, 1, 0, 1));

        // Assert
        var expectedExpiry = now.UtcDateTime.Add(connector.CacheLifetime);
        ls.Verify(l => l.SetItemAsync<DateTime>(DummyApiLocalStorageConnector.CacheKeyForExpiry, expectedExpiry, It.IsAny<CancellationToken?>()),
            Times.Once);
    }
}