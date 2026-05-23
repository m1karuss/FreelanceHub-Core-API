# API Documentation

## Base URL

```
Development: http://localhost:5000/api
Production: https://your-domain.com/api
```

## Authentication

All protected endpoints require a JWT Bearer token in the Authorization header:

```http
Authorization: Bearer <your_access_token>
```

---

## 🔐 Authentication Endpoints

### Register User

Create a new user account.

**Endpoint:** `POST /api/auth/register`

**Request Body:**
```json
{
  "email": "john.doe@example.com",
  "password": "SecurePass@123",
  "confirmPassword": "SecurePass@123",
  "firstName": "John",
  "lastName": "Doe",
  "phoneNumber": "+1234567890",
  "role": "Freelancer"
}
```

**Response:** `200 OK`
```json
{
  "success": true,
  "message": "Registration successful",
  "data": {
    "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "email": "john.doe@example.com",
    "role": "Freelancer"
  }
}
```

**Validation Rules:**
- Email: Valid email format, unique
- Password: Min 8 characters, 1 uppercase, 1 lowercase, 1 digit, 1 special char
- Role: `Client`, `Freelancer`, `Admin`, `Moderator`

---

### Login

Authenticate and receive access/refresh tokens.

**Endpoint:** `POST /api/auth/login`

**Request Body:**
```json
{
  "email": "john.doe@example.com",
  "password": "SecurePass@123"
}
```

**Response:** `200 OK`
```json
{
  "success": true,
  "message": "Login successful",
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "base64-encoded-refresh-token",
    "expiresAt": "2026-05-24T03:00:00Z",
    "user": {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "email": "john.doe@example.com",
      "firstName": "John",
      "lastName": "Doe",
      "role": "Freelancer",
      "emailVerified": true
    }
  }
}
```

---

### Refresh Token

Get a new access token using refresh token.

**Endpoint:** `POST /api/auth/refresh-token`

**Request Body:**
```json
{
  "refreshToken": "base64-encoded-refresh-token"
}
```

**Response:** `200 OK`
```json
{
  "success": true,
  "data": {
    "accessToken": "new-jwt-token",
    "refreshToken": "new-refresh-token",
    "expiresAt": "2026-05-24T03:15:00Z"
  }
}
```

---

### Logout

Revoke refresh token.

**Endpoint:** `POST /api/auth/revoke-token`  
**Auth Required:** Yes

**Request Body:**
```json
{
  "refreshToken": "base64-encoded-refresh-token"
}
```

**Response:** `200 OK`
```json
{
  "success": true,
  "message": "Token revoked successfully"
}
```

---

## 📋 Project Endpoints

### Get All Projects

Retrieve paginated list of projects with filters.

**Endpoint:** `GET /api/projects`

**Query Parameters:**
- `page` (int, default: 1) - Page number
- `pageSize` (int, default: 10) - Items per page
- `status` (string, optional) - Filter by status: `Draft`, `Published`, `InProgress`, `Completed`, `Cancelled`
- `category` (string, optional) - Filter by category
- `minBudget` (decimal, optional) - Minimum budget
- `maxBudget` (decimal, optional) - Maximum budget
- `search` (string, optional) - Search in title/description

**Example Request:**
```http
GET /api/projects?page=1&pageSize=10&status=Published&minBudget=500
```

**Response:** `200 OK`
```json
{
  "success": true,
  "data": {
    "items": [
      {
        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "title": "E-commerce Website Development",
        "description": "Need a full-stack developer for building an e-commerce platform",
        "budget": 5000.00,
        "status": "Published",
        "category": "Web Development",
        "skillsRequired": ["ASP.NET Core", "React", "MySQL"],
        "clientId": "client-guid",
        "clientName": "Jane Smith",
        "createdAt": "2026-05-20T10:00:00Z",
        "deadline": "2026-06-30T23:59:59Z",
        "bidsCount": 12
      }
    ],
    "totalCount": 45,
    "page": 1,
    "pageSize": 10,
    "totalPages": 5
  }
}
```

---

### Get Project by ID

Get detailed information about a specific project.

**Endpoint:** `GET /api/projects/{id}`

**Response:** `200 OK`
```json
{
  "success": true,
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "title": "E-commerce Website Development",
    "description": "Detailed project description...",
    "budget": 5000.00,
    "status": "Published",
    "category": "Web Development",
    "skillsRequired": ["ASP.NET Core", "React", "MySQL"],
    "attachments": ["file1.pdf", "file2.png"],
    "client": {
      "id": "client-guid",
      "name": "Jane Smith",
      "rating": 4.8,
      "projectsPosted": 15
    },
    "createdAt": "2026-05-20T10:00:00Z",
    "updatedAt": "2026-05-21T14:30:00Z",
    "deadline": "2026-06-30T23:59:59Z",
    "bidsCount": 12
  }
}
```

---

### Create Project

Create a new project (Client only).

**Endpoint:** `POST /api/projects`  
**Auth Required:** Yes (Client role)

**Request Body:**
```json
{
  "title": "Mobile App Development",
  "description": "Need an experienced developer to build a cross-platform mobile app",
  "budget": 8000.00,
  "category": "Mobile Development",
  "skillsRequired": ["Flutter", "Firebase", "REST API"],
  "deadline": "2026-07-31T23:59:59Z",
  "attachments": []
}
```

**Response:** `201 Created`
```json
{
  "success": true,
  "message": "Project created successfully",
  "data": {
    "id": "new-project-guid",
    "title": "Mobile App Development",
    "status": "Draft"
  }
}
```

---

### Update Project

Update project details (Owner only).

**Endpoint:** `PUT /api/projects/{id}`  
**Auth Required:** Yes (Owner)

**Request Body:**
```json
{
  "title": "Updated Title",
  "description": "Updated description",
  "budget": 9000.00,
  "deadline": "2026-08-15T23:59:59Z"
}
```

**Response:** `200 OK`

---

### Delete Project

Soft delete a project (Owner only).

**Endpoint:** `DELETE /api/projects/{id}`  
**Auth Required:** Yes (Owner)

**Response:** `204 No Content`

---

## 💼 Bid Endpoints

### Submit Bid

Submit a bid on a project (Freelancer only).

**Endpoint:** `POST /api/bids`  
**Auth Required:** Yes (Freelancer role)

**Request Body:**
```json
{
  "projectId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "amount": 4500.00,
  "deliveryTime": 30,
  "coverLetter": "I have 5 years of experience in e-commerce development...",
  "milestones": [
    {
      "title": "Design & Planning",
      "amount": 1500.00,
      "durationDays": 10
    },
    {
      "title": "Development",
      "amount": 2500.00,
      "durationDays": 15
    },
    {
      "title": "Testing & Deployment",
      "amount": 500.00,
      "durationDays": 5
    }
  ]
}
```

**Response:** `201 Created`
```json
{
  "success": true,
  "message": "Bid submitted successfully",
  "data": {
    "id": "bid-guid",
    "projectId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "amount": 4500.00,
    "status": "Pending"
  }
}
```

---

### Get Project Bids

Get all bids for a project (Project owner only).

**Endpoint:** `GET /api/bids/project/{projectId}`  
**Auth Required:** Yes (Project owner)

**Response:** `200 OK`
```json
{
  "success": true,
  "data": [
    {
      "id": "bid-guid",
      "freelancer": {
        "id": "freelancer-guid",
        "name": "John Doe",
        "rating": 4.9,
        "completedProjects": 45
      },
      "amount": 4500.00,
      "deliveryTime": 30,
      "coverLetter": "I have 5 years of experience...",
      "status": "Pending",
      "submittedAt": "2026-05-21T10:00:00Z"
    }
  ]
}
```

---

### Accept Bid

Accept a bid (Client only).

**Endpoint:** `POST /api/bids/{id}/accept`  
**Auth Required:** Yes (Project owner)

**Response:** `200 OK`
```json
{
  "success": true,
  "message": "Bid accepted successfully",
  "data": {
    "bidId": "bid-guid",
    "status": "Accepted",
    "projectStatus": "InProgress"
  }
}
```

---

## 💳 Payment Endpoints

### Create Payment

Create a payment for a milestone.

**Endpoint:** `POST /api/payments`  
**Auth Required:** Yes (Client role)

**Request Body:**
```json
{
  "projectId": "project-guid",
  "freelancerId": "freelancer-guid",
  "amount": 1500.00,
  "milestoneId": "milestone-guid",
  "paymentMethod": "CreditCard"
}
```

**Response:** `201 Created`

---

### Get Payment History

Get user's payment history.

**Endpoint:** `GET /api/payments/history`  
**Auth Required:** Yes

**Response:** `200 OK`
```json
{
  "success": true,
  "data": [
    {
      "id": "payment-guid",
      "amount": 1500.00,
      "status": "Completed",
      "projectTitle": "E-commerce Website",
      "createdAt": "2026-05-22T15:00:00Z"
    }
  ]
}
```

---

## 💬 Message Endpoints

### Send Message

Send a message to another user.

**Endpoint:** `POST /api/messages`  
**Auth Required:** Yes

**Request Body:**
```json
{
  "receiverId": "user-guid",
  "content": "Hello, I'd like to discuss the project details.",
  "projectId": "project-guid"
}
```

**Response:** `201 Created`

---

### Get Conversation

Get conversation with a specific user.

**Endpoint:** `GET /api/messages/conversation/{userId}`  
**Auth Required:** Yes

**Response:** `200 OK`
```json
{
  "success": true,
  "data": [
    {
      "id": "message-guid",
      "senderId": "sender-guid",
      "senderName": "John Doe",
      "content": "Hello, I'd like to discuss...",
      "sentAt": "2026-05-23T10:00:00Z",
      "isRead": true
    }
  ]
}
```

---

## 🔔 Notification Endpoints

### Get Notifications

Get user's notifications.

**Endpoint:** `GET /api/notifications`  
**Auth Required:** Yes

**Query Parameters:**
- `unreadOnly` (bool, default: false)
- `page` (int, default: 1)
- `pageSize` (int, default: 20)

**Response:** `200 OK`
```json
{
  "success": true,
  "data": {
    "items": [
      {
        "id": "notification-guid",
        "type": "BidReceived",
        "title": "New bid on your project",
        "message": "John Doe submitted a bid on 'E-commerce Website'",
        "isRead": false,
        "createdAt": "2026-05-23T14:00:00Z",
        "actionUrl": "/projects/project-guid/bids"
      }
    ],
    "unreadCount": 5
  }
}
```

---

### Mark as Read

Mark notification as read.

**Endpoint:** `POST /api/notifications/{id}/read`  
**Auth Required:** Yes

**Response:** `200 OK`

---

## ⭐ Review Endpoints

### Create Review

Create a review for a completed project.

**Endpoint:** `POST /api/reviews`  
**Auth Required:** Yes

**Request Body:**
```json
{
  "projectId": "project-guid",
  "revieweeId": "user-guid",
  "rating": 5,
  "comment": "Excellent work! Delivered on time and exceeded expectations."
}
```

**Response:** `201 Created`

---

## Error Responses

All endpoints may return the following error responses:

### 400 Bad Request
```json
{
  "success": false,
  "message": "Validation failed",
  "errors": {
    "Email": ["Email is required"],
    "Password": ["Password must be at least 8 characters"]
  }
}
```

### 401 Unauthorized
```json
{
  "success": false,
  "message": "Unauthorized. Please login."
}
```

### 403 Forbidden
```json
{
  "success": false,
  "message": "You don't have permission to access this resource"
}
```

### 404 Not Found
```json
{
  "success": false,
  "message": "Resource not found"
}
```

### 500 Internal Server Error
```json
{
  "success": false,
  "message": "An error occurred while processing your request"
}
```

---

## Rate Limiting

- **Authenticated users:** 1000 requests per hour
- **Unauthenticated users:** 100 requests per hour

Rate limit headers:
```
X-RateLimit-Limit: 1000
X-RateLimit-Remaining: 999
X-RateLimit-Reset: 1716523200
```

---

## Postman Collection

Import the Postman collection from `docs/FreelanceHub-API.postman_collection.json` for easy testing.

---

## Support

For API support, please open an issue on GitHub or contact the development team.
