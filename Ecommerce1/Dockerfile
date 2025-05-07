# Use .NET 8 SDK to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution files
COPY ../ConnectionLayer ./ConnectionLayer
COPY ../BussLayer ./BussLayer
COPY . .

# Restore dependencies
RUN dotnet restore API.csproj

# Build and publish the app
RUN dotnet publish API.csproj -c Release -o /app/publish

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "API.dll"]
