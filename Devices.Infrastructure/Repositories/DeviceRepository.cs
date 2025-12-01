using Microsoft.EntityFrameworkCore;
using Devices.Domain.Entities;
using Devices.Domain.Enums;
using Devices.Infrastructure.Data;

namespace Devices.Infrastructure.Repositories;

public class DeviceRepository : IDeviceRepository
{
    private readonly AppDbContext _db;
    public DeviceRepository(AppDbContext db) => _db = db;

    public async Task<Device> AddAsync(Device device, CancellationToken ct = default)
    {
        _db.Devices.Add(device);
        await _db.SaveChangesAsync(ct);
        return device;
    }

    public async Task DeleteAsync(Device device, CancellationToken ct = default)
    {
        _db.Devices.Remove(device);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<IEnumerable<Device>> GetAllAsync(CancellationToken ct = default) =>
        await _db.Devices.AsNoTracking().ToListAsync(ct);

    public async Task<Device?> GetByIdAsync(int id, CancellationToken ct = default) =>
        await _db.Devices.Where(d => d.Id == id).FirstOrDefaultAsync(ct);

    public async Task<IEnumerable<Device>> GetByBrandAsync(string brand, CancellationToken ct = default) =>
        await _db.Devices.AsNoTracking().Where(d => d.Brand == brand).ToListAsync(ct);

    public async Task<IEnumerable<Device>> GetByStateAsync(DeviceState state, CancellationToken ct = default) =>
        await _db.Devices.AsNoTracking().Where(d => d.State == state).ToListAsync(ct);

    public async Task UpdateAsync(Device device, CancellationToken ct = default)
    {
        _db.Devices.Update(device);
        await _db.SaveChangesAsync(ct);
    }
}
