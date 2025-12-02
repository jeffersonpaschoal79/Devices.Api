namespace Devices.Application.Dtos;
/// <summary>
/// Defines the Data Transfer Object (DTO) for a device.
/// </summary>
public class DeviceDto
{
    public string Name { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string? State { get; set; }
}
