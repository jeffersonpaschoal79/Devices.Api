using Devices.Domain.Common;
using Devices.Domain.Enums;

namespace Devices.Domain.Entities;

public sealed class Device
{
    public int Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string Brand { get; private set; } = null!;
    public DeviceState State { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Device() { }

    public static Result<Device> Create(string name, string brand, DeviceState? state = null)
    {
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
        if (State == DeviceState.InUse && (name != null || brand != null))
            return Result.Failure("Cannot update Name or Brand when device is in use");

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
