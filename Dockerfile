# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["Ecommerce1.sln", "."]
COPY ["Ecommerce1/Ecommerce1.csproj", "Ecommerce1/"]
COPY ["BusinessLayer/BusinessLayer.csproj", "BusinessLayer/"]
COPY ["ConnectionLayer/ConnectionLayer.csproj", "ConnectionLayer/"]

# Restore NuGet packages
RUN dotnet restore "Ecommerce1.sln"

# Copy all source code
COPY . .

# Build and publish the API project
WORKDIR "/src/Ecommerce1"
RUN dotnet publish -c Release -o /app

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
ENV ASPNETCORE_URLS=http://*:5000
EXPOSE 5000
ENTRYPOINT ["dotnet", "Ecommerce1.dll"]