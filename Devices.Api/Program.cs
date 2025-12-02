using Devices.Api.EndPoints;
using Devices.Application.UseCases;
using Devices.Infrastructure.Data;
using Devices.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<IDeviceRepository, DeviceRepository>();
builder.Services.AddScoped<CreateDeviceUseCase>();
builder.Services.AddScoped<GetDevicesUseCase>();
builder.Services.AddScoped<DeleteDeviceUseCase>();
builder.Services.AddScoped<UpdateDeviceUseCase>();

builder.Services.AddEndpointsApiExplorer();

// Register Swagger services
builder.Services.AddSwaggerGen();

builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


app.UseSwagger();
app.UseSwaggerUI();
app.MapDevices();

app.UseHttpsRedirection();

app.Run();

