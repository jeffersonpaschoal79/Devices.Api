using Devices.Application.Dtos;
using Devices.Domain.Entities;
using Devices.Domain.Enums;
using Devices.Domain.Common;
using Devices.Infrastructure.Repositories;

namespace Devices.Application.UseCases;

public class UpdateDeviceUseCase
{
    private readonly IDeviceRepository _repo;

    public UpdateDeviceUseCase(IDeviceRepository repo) => _repo = repo;

    public async Task<Result<Device>> UpdateAsync(int id, DeviceDto dto, CancellationToken ct = default)
    {
        // basic validation
        if (string.IsNullOrWhiteSpace(dto.Name)) return Result<Device>.Failure("Name is required");
        if (string.IsNullOrWhiteSpace(dto.Brand)) return Result<Device>.Failure("Brand is required");
        if (string.IsNullOrWhiteSpace(dto.State)) return Result<Device>.Failure("State is required");

        var existing = await _repo.GetByIdAsync(id, ct);
        if (existing is null) return Result<Device>.Failure("Device not found");

        var state = ParseState(dto.State);

        // For PUT we consider replace: enforce Name/Brand given and apply
        var updateResult = existing.Update(dto.Name, dto.Brand, state);
        if (!updateResult.IsSuccess) return Result<Device>.Failure(updateResult.Error!);

        await _repo.UpdateAsync(existing, ct);
        return Result<Device>.Success(existing);
    }

    public async Task<Result<Device>> UpdatePartialAsync(int id, DeviceDto dto, CancellationToken ct = default)
    {
        var existing = await _repo.GetByIdAsync(id, ct);
        if (existing is null) return Result<Device>.Failure("Device not found");

        var state = ParseState(dto.State);

        var updateName = string.IsNullOrWhiteSpace(dto.Name) ? existing.Name : dto.Name;
        var updateBrand = string.IsNullOrWhiteSpace(dto.Brand) ? existing.Brand : dto.Brand;
        var updateState = state ?? existing.State;

        // Note: Update method enforces "cannot change Name/Brand when InUse"
        var updateResult = existing.Update(updateName, updateBrand, updateState);
        if (!updateResult.IsSuccess) return Result<Device>.Failure(updateResult.Error!);

        await _repo.UpdateAsync(existing, ct);
        return Result<Device>.Success(existing);
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
