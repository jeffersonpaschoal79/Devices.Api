using Devices.Domain.Common;
using Devices.Domain.Enums;
using Devices.Infrastructure.Repositories;

namespace Devices.Application.UseCases;

/// <summary>
/// Use case for deleting a device.
/// </summary>
public class DeleteDeviceUseCase
{
    private readonly IDeviceRepository _repo;

    public DeleteDeviceUseCase(IDeviceRepository repo) => _repo = repo;

    public async Task<Result> DeleteAsync(int id, CancellationToken ct = default)
    {
        var device = await _repo.GetByIdAsync(id, ct);
        if (device is null)
            return Result.Failure("Device not found");

        // domain rule: cannot delete device in use
        if (device.State == DeviceState.InUse)
            return Result.Failure("In-use devices cannot be deleted");

        // perform deletion
        await _repo.DeleteAsync(device, ct);

        return Result.Success();
    }
}
