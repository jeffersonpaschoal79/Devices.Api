using Devices.Application.Dtos;
using Devices.Application.UseCases;
using Devices.Domain.Entities;
using Devices.Domain.Enums;

namespace Devices.Api.EndPoints;
/// <summary>
/// Defines the API endpoints for managing devices.
/// </summary>
public static class DevicesEndpoints
{
    public static void MapDevices(this WebApplication app)
    {
        var group = app.MapGroup("/api/devices");

        //Create device
        group.MapPost("", async (DeviceDto dto, CreateDeviceUseCase uc) =>
        {
            if (dto is null)
                return Results.BadRequest(new { error = "Input parameters required" });

            var result = await uc.CreateAsync(dto);

            if (!result.IsSuccess) 
                return Results.BadRequest(new { error = result.Error });

            var created = result.Value!;

            return Results.Created($"/api/devices/{created.Id}", created);
        })
        .WithName("CreateDevice")
        .Produces<Device>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest);

        //Get all devices
        group.MapGet("", async (GetDevicesUseCase uc) =>
        {
            var all = await uc.GetAllAsync();
            return Results.Ok(all);
        })
                .WithName("GetAllDevices")
                .Produces<IEnumerable<Device>>(StatusCodes.Status200OK);

        // Get devices by Id
        group.MapGet("/{id:int}", async (int id, GetDevicesUseCase uc) =>
        {
            var device = await uc.GetByIdAsync(id);
            return device is null ? Results.NotFound(new { error = "Device not found" }) : Results.Ok(device);
        })
        .WithName("GetDeviceById")
        .Produces<Device>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        // Get device by brand
        group.MapGet("/by-brand", async (string? brand, GetDevicesUseCase uc) =>
        {
            if (string.IsNullOrWhiteSpace(brand)) return Results.BadRequest(new { error = "Brand is required" });

            var list = await uc.GetByBrandAsync(brand);
            return Results.Ok(list);
        })
        .WithName("GetDevicesByBrand")
        .Produces<IEnumerable<Device>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);

        // Get device by state
        group.MapGet("/by-state", async (string? state, GetDevicesUseCase uc) =>
        {
            if (string.IsNullOrWhiteSpace(state)) return Results.BadRequest(new { error = "State is required" });

            if (!Enum.TryParse<DeviceState>(state, true, out var parsed))
                return Results.BadRequest(new { error = "invalid state. valid values: Available, InUse, Inactive" });

            var list = await uc.GetByStateAsync(parsed);
            return Results.Ok(list);
        })
        .WithName("GetDevicesByState")
        .Produces<IEnumerable<Device>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);

        // Delete device by id
        group.MapDelete("/{id:int}", async (int id, DeleteDeviceUseCase uc) =>
        {
            var result = await uc.DeleteAsync(id);
            if (!result.IsSuccess)
                return result.Error switch
                {
                    "Device not found" => Results.NotFound(new { error = result.Error }),
                    "In-use devices cannot be deleted" => Results.BadRequest(new { error = result.Error }),
                    _ => Results.Problem(result.Error)
                };

            return Results.NoContent();
        })
        .WithName("DeleteDevice")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound);

        // Update device by id
        group.MapPut("/{id:int}", async (int id, DeviceDto dto, UpdateDeviceUseCase uc) =>
        {
            var result = await uc.UpdateAsync(id, dto);
            if (!result.IsSuccess)
                return result.Error switch
                {
                    "Device not found" => Results.NotFound(new { error = result.Error }),
                    "Cannot update Name or Brand when device is in use" =>
                        Results.BadRequest(new { error = result.Error }),
                    _ => Results.BadRequest(new { error = result.Error })
                };

            return Results.Ok(result.Value);
        })
        .WithName("UpdateDevice")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status400BadRequest);

        group.MapPatch("/{id:int}", async (int id, DeviceDto dto, UpdateDeviceUseCase uc) =>
        {
            var result = await uc.UpdatePartialAsync(id, dto);
            if (!result.IsSuccess)
                return result.Error switch
                {
                    "Device not found" => Results.NotFound(new { error = result.Error }),
                    "Cannot update Name or Brand when device is in use" =>
                        Results.BadRequest(new { error = result.Error }),
                    _ => Results.BadRequest(new { error = result.Error })
                };

            return Results.Ok(result.Value);
        })
        .WithName("PatchDevice")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status400BadRequest); 
    }
}