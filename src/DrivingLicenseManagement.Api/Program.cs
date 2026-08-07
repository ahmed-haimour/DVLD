using DrivingLicenseManagement.Application;
using DrivingLicenseManagement.Infrastructure;
using DrivingLicenseManagement.Infrastructure.Data;
using DrivingLicenseManagement.Infrastructure.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

await app.Services.SeedRolesAsync();
await app.Services.SeedApplicationTypesAsync();
await app.Services.SeedTestTypesAsync();

app.MapGet("/", () => "Hello World!");

app.Run();
