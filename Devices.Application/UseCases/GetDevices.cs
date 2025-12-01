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

}
