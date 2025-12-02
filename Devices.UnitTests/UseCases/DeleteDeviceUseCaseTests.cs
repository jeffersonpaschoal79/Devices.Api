using Devices.Application.UseCases;
using Devices.Domain.Entities;
using Devices.Domain.Enums;
using Devices.Infrastructure.Repositories;
using FluentAssertions;
using Moq;

namespace Devices.Api.Tests.UseCases;

public class DeleteDeviceUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_DeviceNotFound_ReturnsFailure()
    {
        // Arrange
        var repo = new Mock<IDeviceRepository>();
        repo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync((Device?)null);

        var uc = new DeleteDeviceUseCase(repo.Object);

        // Act
        var res = await uc.DeleteAsync(1);

        // Assert
        res.IsSuccess.Should().BeFalse();
        res.Error.Should().Contain("Device not found");
    }

    [Fact]
    public async Task ExecuteAsync_DeviceInUse_ReturnsFailure()
    {
        // Arrange
        var device = Device.Create("N", "B", DeviceState.InUse).Value!;
        var repo = new Mock<IDeviceRepository>();
        repo.Setup(r => r.GetByIdAsync(device.Id, It.IsAny<CancellationToken>())).ReturnsAsync(device);

        var uc = new DeleteDeviceUseCase(repo.Object);

        // Act
        var res = await uc.DeleteAsync(device.Id);

        // Assert
        res.IsSuccess.Should().BeFalse();
        res.Error.Should().Contain("In-use devices cannot be deleted");
    }

    [Fact]
    public async Task ExecuteAsync_DeviceRemovable_DeletesAndReturnsSuccess()
    {
        // Arrange
        var device = Device.Create("N", "B", DeviceState.Available).Value!;
        var repo = new Mock<IDeviceRepository>();
        repo.Setup(r => r.GetByIdAsync(device.Id, It.IsAny<CancellationToken>())).ReturnsAsync(device);
        repo.Setup(r => r.DeleteAsync(device, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var uc = new DeleteDeviceUseCase(repo.Object);

        // Act
        var res = await uc.DeleteAsync(device.Id);

        // Assert
        res.IsSuccess.Should().BeTrue();
        repo.Verify(r => r.DeleteAsync(device, It.IsAny<CancellationToken>()), Times.Once);
    }
}
