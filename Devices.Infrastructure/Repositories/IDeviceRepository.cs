using Devices.Domain.Entities;
using Devices.Domain.Enums;

namespace Devices.Infrastructure.Repositories;

public interface IDeviceRepository
{
    Task<Device> CreateAsync(Device device, CancellationToken ct = default);
    Task<Device?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<Device>> GetAllAsync(CancellationToken ct = default);
    Task<IEnumerable<Device>> GetByBrandAsync(string brand, CancellationToken ct = default);
    Task<IEnumerable<Device>> GetByStateAsync(DeviceState state, CancellationToken ct = default);
    Task UpdateAsync(Device device, CancellationToken ct = default);
    Task DeleteAsync(Device device, CancellationToken ct = default);
}
