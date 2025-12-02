using Devices.Application.UseCases;
using Devices.Domain.Entities;
using Devices.Domain.Enums;
using Devices.Infrastructure.Repositories;
using FluentAssertions;
using Moq;

namespace Devices.Api.Tests.UseCases;

/// <summary>
/// GetDevicesUseCase unit tests.
/// </summary>
public class GetDevicesUseCaseTests
{
    [Fact]
    public async Task GetAllAsync_ReturnsSuccessWithDevices()
    {
        // Arrange
        var sample = new List<Device>
        {
            Device.Create("N1","B1", DeviceState.Available).Value!,
            Device.Create("N2","B2", DeviceState.InUse).Value!
        };

        var repoMock = new Mock<IDeviceRepository>();
        repoMock.Setup(r => r.GetAllAsync(default)).ReturnsAsync(sample);

        var uc = new GetDevicesUseCase(repoMock.Object);

        // Act
        var result = await uc.GetAllAsync();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(sample);
    }

    [Fact]
    public async Task GetByIdAsync_NotFound_ReturnsFailure()
    {
        // Arrange
        var repoMock = new Mock<IDeviceRepository>();
        repoMock.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync((Device?)null);

        var uc = new GetDevicesUseCase(repoMock.Object);

        // Act
        var result = await uc.GetByIdAsync(1);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Device not found");
    }

    [Fact]
    public async Task GetByIdAsync_Found_ReturnsSuccessAndDevice()
    {
        // Arrange
        var device = Device.Create("N", "B", DeviceState.Available).Value!;
        var repoMock = new Mock<IDeviceRepository>();
        repoMock.Setup(r => r.GetByIdAsync(device.Id, default)).ReturnsAsync(device);

        var uc = new GetDevicesUseCase(repoMock.Object);

        // Act
        var result = await uc.GetByIdAsync(device.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(device);
    }
}
