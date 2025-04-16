# Use the official .NET SDK to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Copy csproj and restore
COPY *.sln .
COPY Ecommerce1/*.csproj ./Ecommerce1/
RUN dotnet restore

# Copy everything and build
COPY Ecommerce1/. ./Ecommerce1/
WORKDIR /source/Ecommerce1
RUN dotnet publish -c Release -o /app/publish

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Expose port (make sure your app uses this)
EXPOSE 8080

# Set environment (optional)
ENV ASPNETCORE_URLS=http://+:8080

# Entry point
ENTRYPOINT ["dotnet", "Ecommerce1.dll"]
