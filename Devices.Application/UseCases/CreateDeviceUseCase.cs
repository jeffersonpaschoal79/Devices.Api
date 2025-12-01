using Devices.Application.Dtos;
using Devices.Domain.Common;
using Devices.Domain.Entities;
using Devices.Domain.Enums;
using Devices.Infrastructure.Repositories;

namespace Devices.Application.UseCases;

public class CreateDeviceUseCase
{
    private readonly IDeviceRepository _repo;

    public CreateDeviceUseCase(IDeviceRepository repo) => _repo = repo;

    public async Task<Result<Device>> CreateAsync(DeviceDto dto, CancellationToken ct = default)
    {
        var state = ParseState(dto.State);

        var creation = Device.Create(dto.Name, dto.Brand, state);
        if (!creation.IsSuccess)
            return Result<Device>.Failure(creation.Error ?? "validation failed");

        var device = creation.Value!; // device created and validated

        var persisted = await _repo.CreateAsync(device, ct);
        return Result<Device>.Success(persisted);
    }

    private static DeviceState? ParseState(string? rawState)
    {
        if (string.IsNullOrWhiteSpace(rawState))
            return null;

        return Enum.TryParse<DeviceState>(rawState, true, out var parsed)
            ? parsed
            : null;
    }
}
