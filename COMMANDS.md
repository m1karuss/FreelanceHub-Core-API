# Development Commands

## 🏗️ Build & Run

```bash
# Restore dependencies
dotnet restore

# Build solution
dotnet build

# Run API
cd src/FreelanceHub.API
dotnet run

# Run with watch (auto-reload)
dotnet watch run
```

## 🗄️ Database Migrations

```bash
# Add new migration
dotnet ef migrations add MigrationName \
  --project src/FreelanceHub.Infrastructure \
  --startup-project src/FreelanceHub.API

# Update database
dotnet ef database update \
  --project src/FreelanceHub.Infrastructure \
  --startup-project src/FreelanceHub.API

# Remove last migration
dotnet ef migrations remove \
  --project src/FreelanceHub.Infrastructure \
  --startup-project src/FreelanceHub.API

# Generate SQL script
dotnet ef migrations script \
  --project src/FreelanceHub.Infrastructure \
  --startup-project src/FreelanceHub.API \
  --output migration.sql
```

## 🧪 Testing

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test /p:CollectCoverage=true

# Run specific test project
dotnet test tests/FreelanceHub.UnitTests

# Run tests with filter
dotnet test --filter "FullyQualifiedName~AuthService"
```

## 🐳 Docker Commands

```bash
# Build image
docker build -t freelancehub-api:latest .

# Run container
docker run -d -p 5000:80 freelancehub-api:latest

# View logs
docker logs -f freelancehub-api

# Execute command in container
docker exec -it freelancehub-api bash

# Docker Compose
docker-compose up -d          # Start services
docker-compose down           # Stop services
docker-compose logs -f api    # View API logs
docker-compose ps             # List services
docker-compose restart api    # Restart API
```

## 📦 NuGet Packages

```bash
# Add package
dotnet add package PackageName

# Update package
dotnet add package PackageName --version x.x.x

# Remove package
dotnet remove package PackageName

# List packages
dotnet list package
```

## 🔍 Code Quality

```bash
# Format code
dotnet format

# Analyze code
dotnet build /p:TreatWarningsAsErrors=true

# Clean solution
dotnet clean
```

## 🚀 Deployment

```bash
# Publish for production
dotnet publish -c Release -o ./publish

# Create Docker image for production
docker build -t freelancehub-api:v1.0.0 .

# Tag and push to registry
docker tag freelancehub-api:v1.0.0 registry.example.com/freelancehub-api:v1.0.0
docker push registry.example.com/freelancehub-api:v1.0.0
```
