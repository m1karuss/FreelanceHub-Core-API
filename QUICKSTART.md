# Quick Start Guide

## 📋 Prerequisites

- **.NET SDK 8.0+** - [Download](https://dotnet.microsoft.com/download/dotnet/8.0)
- **MySQL 8.0+** - [Download](https://dev.mysql.com/downloads/) OR **Docker Desktop** (recommended)
- **Git** - [Download](https://git-scm.com/downloads)

## 🚀 Fastest Way to Run

### Option 1: Docker Compose (Recommended - No MySQL Installation Needed)

```bash
# 1. Clone repository
git clone https://github.com/yourusername/FreelanceHub-Core-API.git
cd FreelanceHub-Core-API

# 2. Copy and configure environment file
cp .env.example .env
# Edit .env with your preferred passwords (optional, defaults work fine)

# 3. Start all services (API + MySQL + Redis)
docker-compose up -d

# 4. Wait 30 seconds for services to initialize, then view logs
docker-compose logs -f api
```

**✅ Services will be available at:**
- **API**: http://localhost:5000
- **Swagger UI**: http://localhost:5000/swagger
- **MySQL**: localhost:3306
- **Redis**: localhost:6379

**🛑 To stop:**
```bash
docker-compose down
```

---

### Option 2: Local Development (Manual Setup)

**Step 1: Install MySQL and Create Database**

```bash
# Login to MySQL
mysql -u root -p

# Create database
CREATE DATABASE freelancehub_dev;
EXIT;
```

**Step 2: Configure Application Settings**

```bash
# Copy example configuration
cp src/FreelanceHub.API/appsettings.Development.json.example src/FreelanceHub.API/appsettings.Development.json

# Edit appsettings.Development.json and update:
# - ConnectionStrings:DefaultConnection (your MySQL credentials)
# - JwtSettings:Secret (generate a secure 32+ character key)
```

**Step 3: Install EF Core Tools (if not installed)**

```bash
dotnet tool install --global dotnet-ef
```

**Step 4: Run Database Migrations**

```bash
cd src/FreelanceHub.API
dotnet ef database update --project ../FreelanceHub.Infrastructure
```

**Step 5: Run the API**

```bash
dotnet run
```

**✅ API will be available at:**
- **HTTPS**: https://localhost:5001
- **HTTP**: http://localhost:5000
- **Swagger**: https://localhost:5001/swagger

---

### Option 3: Docker for Infrastructure Only (Hybrid Approach)

```bash
# Start only MySQL and Redis in Docker
docker-compose -f docker-compose.dev.yml up -d

# Run migrations
cd src/FreelanceHub.API
dotnet ef database update --project ../FreelanceHub.Infrastructure

# Run API locally
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

---

## 🧪 Testing with Swagger UI (Recommended for Beginners)

### Step 1: Open Swagger
Navigate to: **http://localhost:5000/swagger** (or https://localhost:5001/swagger for local dev)

### Step 2: Register a New User
1. Find **POST /api/auth/register** endpoint
2. Click **"Try it out"**
3. Use this example request body:
```json
{
  "email": "john.doe@example.com",
  "password": "Test@123456",
  "confirmPassword": "Test@123456",
  "firstName": "John",
  "lastName": "Doe",
  "phoneNumber": "+1234567890",
  "role": "Freelancer"
}
```
4. Click **"Execute"**
5. You should get a **200 OK** response

### Step 3: Login
1. Find **POST /api/auth/login** endpoint
2. Click **"Try it out"**
3. Use this request body:
```json
{
  "email": "john.doe@example.com",
  "password": "Test@123456"
}
```
4. Click **"Execute"**
5. **Copy the `accessToken`** from the response

### Step 4: Authorize Swagger
1. Click the **"Authorize"** button (🔒 icon) at the top right
2. In the "Value" field, enter: `Bearer YOUR_ACCESS_TOKEN`
   - Example: `Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...`
3. Click **"Authorize"**, then **"Close"**

### Step 5: Test Protected Endpoints
Now you can test any protected endpoint:
- **GET /api/projects** - Browse projects
- **POST /api/projects** - Create a project (Client role)
- **POST /api/bids** - Submit a bid (Freelancer role)
- **GET /api/notifications** - Get your notifications

---

## 📱 Testing with Postman

### Import Collection
1. Open Postman
2. Click **Import** → **File**
3. Select `docs/FreelanceHub-API.postman_collection.json`
4. Collection will be imported with all endpoints

### Set Environment Variables
1. Create a new environment in Postman
2. Add variables:
   - `baseUrl`: `http://localhost:5000`
   - `accessToken`: (will be set automatically after login)

### Run Requests
1. Start with **Auth → Register**
2. Then **Auth → Login** (token will be saved automatically)
3. All other requests will use the saved token

---

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
