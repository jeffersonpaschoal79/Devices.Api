using Devices.Application.Dtos;
using Devices.Application.UseCases;
using Devices.Domain.Entities;
using Devices.Domain.Enums;
using Devices.Infrastructure.Repositories;
using FluentAssertions;
using Moq;

namespace Devices.Api.Tests.UseCases;

/// <summary>
/// UpdateDeviceUseCase unit tests.
/// </summary>
public class UpdateDeviceUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_DeviceExistsAndValid_UpdatesAndReturnsSuccess()
    {
        // Arrange
        var device = Device.Create("Old", "Brand", DeviceState.Available).Value!;
        var repoMock = new Mock<IDeviceRepository>();
        repoMock.Setup(r => r.GetByIdAsync(device.Id, It.IsAny<CancellationToken>())).ReturnsAsync(device);
        repoMock.Setup(r => r.UpdateAsync(device, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var uc = new UpdateDeviceUseCase(repoMock.Object);

        var dto = new DeviceDto { Name = "New", Brand = "Brand", State = "Inactive" };

        // Act
        var result = await uc.UpdateAsync(device.Id, dto);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Name.Should().Be("New");
        result.Value.Brand.Should().Be("Brand");
        repoMock.Verify(r => r.UpdateAsync(device, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_DeviceNotFound_ReturnsFailure()
    {
        // Arrange
        var repoMock = new Mock<IDeviceRepository>();
        repoMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync((Device?)null);

        var uc = new UpdateDeviceUseCase(repoMock.Object);
        var dto = new DeviceDto { Name = "N", Brand = "B", State = "Available" };

        // Act
        var result = await uc.UpdateAsync(999, dto);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Device not found");
    }

    [Fact]
    public async Task ExecuteAsync_InvalidChange_WhenInUse_ReturnsFailure()
    {
        // Arrange
        var device = Device.Create("Name", "Brand", DeviceState.InUse).Value!;
        var repoMock = new Mock<IDeviceRepository>();
        repoMock.Setup(r => r.GetByIdAsync(device.Id, It.IsAny<CancellationToken>())).ReturnsAsync(device);

        var uc = new UpdateDeviceUseCase(repoMock.Object);
        var dto = new DeviceDto { Name = "NewName", Brand = "SomeBrand", State = "Available" };

        // Act
        var result = await uc.UpdateAsync(device.Id, dto);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Cannot update Name or Brand when device is in use");
    }
}
