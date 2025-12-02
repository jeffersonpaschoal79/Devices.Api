using Devices.Application.Dtos;
using Devices.Application.UseCases;
using Devices.Domain.Entities;
using Devices.Infrastructure.Repositories;
using FluentAssertions;
using Moq;

namespace Devices.Api.Tests.UseCases;
/// <summary>
/// CreateDeviceUseCase unit tests.
/// </summary>
public class CreateDeviceUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidDto_ReturnsSuccessResultAndAddsDevice()
    {
        // Arrange
        var repoMock = new Mock<IDeviceRepository>();
        repoMock
            .Setup(r => r.CreateAsync(It.IsAny<Device>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Device d, CancellationToken ct) => d);

        var uc = new CreateDeviceUseCase(repoMock.Object);

        var dto = new DeviceDto { Name = "X", Brand = "Y", State = "Available" };

        // Act
        var result = await uc.CreateAsync(dto, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Name.Should().Be("X");
        result.Value.Brand.Should().Be("Y");
        repoMock.Verify(r => r.CreateAsync(It.IsAny<Device>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithInvalidDto_ReturnsFailureAndDoesNotCallRepository()
    {
        // Arrange
        var repoMock = new Mock<IDeviceRepository>();
        var uc = new CreateDeviceUseCase(repoMock.Object);

        var dto = new DeviceDto { Name = "", Brand = "Y" };

        // Act
        var result = await uc.CreateAsync(dto, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().NotBeNullOrWhiteSpace();
        repoMock.Verify(r => r.CreateAsync(It.IsAny<Device>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
