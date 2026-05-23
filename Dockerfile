# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["FreelanceHub.sln", "./"]
COPY ["src/FreelanceHub.Domain/FreelanceHub.Domain.csproj", "src/FreelanceHub.Domain/"]
COPY ["src/FreelanceHub.Application/FreelanceHub.Application.csproj", "src/FreelanceHub.Application/"]
COPY ["src/FreelanceHub.Infrastructure/FreelanceHub.Infrastructure.csproj", "src/FreelanceHub.Infrastructure/"]
COPY ["src/FreelanceHub.API/FreelanceHub.API.csproj", "src/FreelanceHub.API/"]

# Restore dependencies
RUN dotnet restore "src/FreelanceHub.API/FreelanceHub.API.csproj"

# Copy all source files
COPY . .

# Build the application
WORKDIR "/src/src/FreelanceHub.API"
RUN dotnet build "FreelanceHub.API.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "FreelanceHub.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Copy published application
COPY --from=publish /app/publish .

# Create logs directory
RUN mkdir -p /app/logs

# Expose ports
EXPOSE 80
EXPOSE 443

# Set environment variables
ENV ASPNETCORE_URLS=http://+:80

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=40s --retries=3 \
  CMD curl -f http://localhost/health || exit 1

# Run the application
ENTRYPOINT ["dotnet", "FreelanceHub.API.dll"]
