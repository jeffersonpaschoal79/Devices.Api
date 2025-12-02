using Devices.Domain.Entities;
using Devices.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace Devices.Api.Tests.Domain;
/// <summary>
/// Device entity Domain unit tests.
/// </summary>
public class DeviceTests
{
    [Fact]
    public void Create_WithValidValues_ReturnsSuccessDevice()
    {
        var result = Device.Create("iPhone", "Apple", DeviceState.Available);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Name.Should().Be("iPhone");
        result.Value.Brand.Should().Be("Apple");
        result.Value.State.Should().Be(DeviceState.Available);
        result.Value.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithInvalidName_ReturnsFailure(string? name)
    {
        var result = Device.Create(name!, "Brand");

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Name is required");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithInvalidBrand_ReturnsFailure(string? brand)
    {
        var result = Device.Create("Name", brand!);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Brand is required");
    }

    [Fact]
    public void Update_WhenInUse_CannotChangeNameOrBrand()
    {
        var d = Device.Create("N", "B", DeviceState.InUse).Value!;
        var res = d.Update("NewName", null, null);

        res.IsSuccess.Should().BeFalse();
        res.Error.Should().Contain("Cannot update Name or Brand when device is in use");
    }

    [Fact]
    public void Update_PartialChange_UpdatesOnlyProvidedFields()
    {
        var d = Device.Create("N", "B", DeviceState.Available).Value!;
        var res = d.Update("N2", null, DeviceState.Inactive);

        res.IsSuccess.Should().BeTrue();
        d.Name.Should().Be("N2");
        d.Brand.Should().Be("B");
        d.State.Should().Be(DeviceState.Inactive);
    }
}
