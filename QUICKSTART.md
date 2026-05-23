# Quick Start Guide

## 🚀 Fastest Way to Run

### Option 1: Docker Compose (Recommended)

```bash
# Clone repository
git clone https://github.com/yourusername/FreelanceHub-Core-API.git
cd FreelanceHub-Core-API

# Copy environment file
cp .env.example .env

# Start all services
docker-compose up -d

# View logs
docker-compose logs -f api
```

**Services will be available at:**
- API: http://localhost:5000
- Swagger: http://localhost:5000
- PHPMyAdmin: http://localhost:8080
- Redis Commander: http://localhost:8081

### Option 2: Local Development

```bash
# Start infrastructure only
docker-compose -f docker-compose.dev.yml up -d

# Run migrations
cd src/FreelanceHub.API
dotnet ef database update --project ../FreelanceHub.Infrastructure

# Run API
dotnet run
```

## 📝 First API Call

### Register User

```bash
curl -X POST http://localhost:5000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "john@example.com",
    "password": "Test@123456",
    "confirmPassword": "Test@123456",
    "firstName": "John",
    "lastName": "Doe",
    "phoneNumber": "+1234567890",
    "role": "Freelancer"
  }'
```

### Login

```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "john@example.com",
    "password": "Test@123456"
  }'
```

Response:
```json
{
  "success": true,
  "message": "Login successful",
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "base64-encoded-token",
    "expiresAt": "2026-05-23T15:17:00Z",
    "user": {
      "id": "guid",
      "email": "john@example.com",
      "firstName": "John",
      "lastName": "Doe",
      "role": "Freelancer",
      "emailVerified": false
    }
  }
}
```

## 🔑 Using JWT Token

```bash
# Use access token in Authorization header
curl -X GET http://localhost:5000/api/users/me \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN"
```

## 🛑 Stop Services

```bash
# Stop all containers
docker-compose down

# Stop and remove volumes (clean slate)
docker-compose down -v
```

## 📚 Next Steps

1. Explore Swagger UI: http://localhost:5000
2. Check API documentation in README.md
3. Review ARCHITECTURE.md for system design
4. See PORTFOLIO-GUIDE.md for showcasing tips
