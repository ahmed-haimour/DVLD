# ----- Build Stage -----
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /source

COPY src/DrivingLicenseManagement.Domain/DrivingLicenseManagement.Domain.csproj src/DrivingLicenseManagement.Domain/
COPY src/DrivingLicenseManagement.Application/DrivingLicenseManagement.Application.csproj src/DrivingLicenseManagement.Application/
COPY src/DrivingLicenseManagement.Infrastructure/DrivingLicenseManagement.Infrastructure.csproj src/DrivingLicenseManagement.Infrastructure/
COPY src/DrivingLicenseManagement.Api/DrivingLicenseManagement.Api.csproj src/DrivingLicenseManagement.Api/
RUN dotnet restore src/DrivingLicenseManagement.Api/DrivingLicenseManagement.Api.csproj

COPY src/ src/
RUN dotnet publish src/DrivingLicenseManagement.Api/DrivingLicenseManagement.Api.csproj -c Release -o /app --no-restore

# ----- Runtime Stage -----
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app .

EXPOSE 8080
ENTRYPOINT ["dotnet", "DrivingLicenseManagement.Api.dll"]
