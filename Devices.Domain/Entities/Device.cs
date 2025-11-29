using Devices.Domain.Enums;

namespace Devices.Domain.Entities;

public sealed class Device
{
    public int Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string Brand { get; private set; } = null!;
    public DeviceState State { get; private set; }
    public DateTime CreatedAt { get; private set; }

}
