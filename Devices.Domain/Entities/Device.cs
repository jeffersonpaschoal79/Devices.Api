using Devices.Domain.Common;
using Devices.Domain.Enums;

namespace Devices.Domain.Entities;
/// <summary>
/// Device entity representing a device in the system.
/// </summary>
public sealed class Device
{
    public int Id { get; private set; }
    public string Name { get; set; } = null!;
    public string Brand { get; set; } = null!;
    public DeviceState State { get; set; }
    public DateTime CreatedAt { get; private set; }

    private Device() { }

    public static Result<Device> Create(string name, string brand, DeviceState? state = null)
    {
        // Validate required fields
        if (string.IsNullOrWhiteSpace(name))
            return Result<Device>.Failure("Name is required");
        if (string.IsNullOrWhiteSpace(brand))
            return Result<Device>.Failure("Brand is required");

        var d = new Device
        {
            Name = name.Trim(),
            Brand = brand.Trim(),
            State = state ?? DeviceState.Available,
            CreatedAt = DateTime.UtcNow
        };

        return Result<Device>.Success(d);
    }

    public Result Update(string? name, string? brand, DeviceState? state)
    {
        // Prevent changing Name or Brand if device is in use
        if (State == DeviceState.InUse && (name != null || brand != null))
            return Result.Failure("Cannot update Name or Brand when device is in use");

        // Validate and update fields
        if (name is not null)
        {
            if (string.IsNullOrWhiteSpace(name)) return Result.Failure("Name is required");
            Name = name.Trim();
        }

        if (brand is not null)
        {
            if (string.IsNullOrWhiteSpace(brand)) return Result.Failure("Brand is required");
            Brand = brand.Trim();
        }

        if (state.HasValue)
            State = state.Value;

        return Result.Success();
    }
}
