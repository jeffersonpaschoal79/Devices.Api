using Devices.Api.EndPoints;
using Devices.Application.UseCases;
using Devices.Infrastructure.Data;
using Devices.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configure the application to listen on port 80
builder.WebHost.UseUrls("http://*:80");

// Add services to the container.
// Configure DbContext with PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

//Register repositories and use cases
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

// Apply pending migrations at startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // Retry logic for database migration to handle transient failures
    var retries = 20;
    while (retries-- > 0)
    {
        try
        {
            db.Database.Migrate();
            break;
        }
        catch (Npgsql.NpgsqlException)
        {
            Console.WriteLine("Database is not ready yet. Retrying in 3 seconds...");
            await Task.Delay(3000);
        }
    }
}

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

