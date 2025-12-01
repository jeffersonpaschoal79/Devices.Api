using Devices.Application.Dtos;
using Devices.Application.UseCases;
using Devices.Domain.Entities;

namespace Devices.Api.EndPoints;

public static class DevicesEndpoints
{
    public static void MapDevices(this WebApplication app)
    {
        var group = app.MapGroup("/api/devices");

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

        group.MapGet("", async (GetDevicesUseCase uc) =>
        {
            var all = await uc.GetAllAsync();
            return Results.Ok(all);
        })
                .WithName("GetAllDevices")
                .Produces<IEnumerable<Device>>(StatusCodes.Status200OK);
    }
}