using Devices.Application.Dtos;
using Devices.Domain.Common;
using Devices.Domain.Entities;
using Devices.Domain.Enums;
using Devices.Infrastructure.Repositories;

namespace Devices.Application.UseCases;

public class GetDevicesUseCase
{
    private readonly IDeviceRepository _repo;

    public GetDevicesUseCase(IDeviceRepository repo) => _repo = repo;

    public async Task<Result<IEnumerable<Device>>> GetAllAsync(CancellationToken ct = default)
    {

        var devices = await _repo.GetAllAsync(ct);

        return Result<IEnumerable<Device>>.Success(devices);
    }

    public async Task<Result<Device>> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var device = await _repo.GetByIdAsync(id, ct);
        if (device is null)
            return Result<Device>.Failure("Device not found");

        return Result<Device>.Success(device);
    }

    public async Task<Result<IEnumerable<Device>>> GetByBrandAsync(string brand, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(brand))
            return Result<IEnumerable<Device>>.Failure("Brand is required");

        var list = await _repo.GetByBrandAsync(brand, ct);
        return Result<IEnumerable<Device>>.Success(list);
    }

    public async Task<Result<IEnumerable<Device>>> GetByStateAsync(DeviceState state, CancellationToken ct = default)
    {
        var list = await _repo.GetByStateAsync(state, ct);
        return Result<IEnumerable<Device>>.Success(list);
    }
}
