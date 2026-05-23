# FreelanceHub Core API - Architecture Documentation

## 🏛️ System Architecture

### High-Level Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                        Client Layer                          │
│  (Web App, Mobile App, Third-party Integrations)            │
└────────────────────┬────────────────────────────────────────┘
                     │ HTTPS/REST
                     ▼
┌─────────────────────────────────────────────────────────────┐
│                     API Gateway / Load Balancer              │
│                    (NGINX, Azure API Gateway)                │
└────────────────────┬────────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────────┐
│                   FreelanceHub.API Layer                     │
│  ┌──────────────────────────────────────────────────────┐  │
│  │  Controllers  │  Middleware  │  Filters  │  Auth     │  │
│  └──────────────────────────────────────────────────────┘  │
└────────────────────┬────────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────────┐
│              FreelanceHub.Application Layer                  │
│  ┌──────────────────────────────────────────────────────┐  │
│  │  Services  │  DTOs  │  Validators  │  Mappings       │  │
│  └──────────────────────────────────────────────────────┘  │
└────────────────────┬────────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────────┐
│               FreelanceHub.Domain Layer                      │
│  ┌──────────────────────────────────────────────────────┐  │
│  │  Entities  │  Interfaces  │  Enums  │  Exceptions    │  │
│  └──────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
                     ▲
                     │
┌────────────────────┴────────────────────────────────────────┐
│            FreelanceHub.Infrastructure Layer                 │
│  ┌──────────────────────────────────────────────────────┐  │
│  │  Repositories  │  DbContext  │  UnitOfWork  │ Cache  │  │
│  └──────────────────────────────────────────────────────┘  │
└────────────────────┬────────────────────────────────────────┘
                     │
        ┌────────────┴────────────┐
        ▼                         ▼
┌──────────────┐          ┌──────────────┐
│    MySQL     │          │    Redis     │
│   Database   │          │    Cache     │
└──────────────┘          └──────────────┘
```

---

## 🔄 Request Flow

### Typical API Request Flow

```
1. Client Request
   ↓
2. API Gateway (Rate Limiting, CORS)
   ↓
3. Authentication Middleware (JWT Validation)
   ↓
4. Authorization Middleware (Role Check)
   ↓
5. Controller (Route Handling)
   ↓
6. Validation Filter (FluentValidation)
   ↓
7. Application Service (Business Logic)
   ↓
8. Repository (Data Access)
   ↓
9. DbContext (EF Core)
   ↓
10. MySQL Database
   ↓
11. Response Mapping (AutoMapper)
   ↓
12. Response Serialization (JSON)
   ↓
13. Client Response
```

---

## 🎯 Clean Architecture Layers

### 1. Domain Layer (Core)

**Purpose:** Contains enterprise business rules and entities.

**Responsibilities:**
- Define domain entities
- Define repository interfaces
- Define domain exceptions
- Define enums and value objects
- **No dependencies on other layers**

**Key Components:**
```
Domain/
├── Entities/
│   ├── User.cs
│   ├── Project.cs
│   ├── Bid.cs
│   └── ...
├── Interfaces/
│   ├── IRepository.cs
│   ├── IUserRepository.cs
│   └── IUnitOfWork.cs
├── Enums/
│   ├── UserRole.cs
│   └── ProjectStatus.cs
└── Exceptions/
    ├── DomainException.cs
    └── EntityNotFoundException.cs
```

**Design Decisions:**
- ✅ Rich domain models with behavior
- ✅ Interface segregation
- ✅ No infrastructure concerns
- ✅ Framework-agnostic

---

### 2. Application Layer

**Purpose:** Contains application business logic and use cases.

**Responsibilities:**
- Define DTOs (Data Transfer Objects)
- Implement application services
- Define validation rules
- Define AutoMapper profiles
- Orchestrate domain objects

**Dependencies:** Domain Layer only

**Key Components:**
```
Application/
├── DTOs/
│   ├── Auth/
│   ├── Project/
│   └── Common/
├── Services/
│   ├── Interfaces/
│   └── Implementations/
├── Validators/
│   ├── RegisterRequestValidator.cs
│   └── CreateProjectRequestValidator.cs
└── Mappings/
    └── MappingProfile.cs
```

**Design Decisions:**
- ✅ Thin DTOs (no logic)
- ✅ Service interfaces for testability
- ✅ FluentValidation for complex rules
- ✅ AutoMapper for object mapping

---

### 3. Infrastructure Layer

**Purpose:** Implements data access and external services.

**Responsibilities:**
- Implement repositories
- Configure EF Core
- Implement caching
- Implement external service integrations
- Database migrations

**Dependencies:** Domain Layer, Application Layer

**Key Components:**
```
Infrastructure/
├── Data/
│   ├── ApplicationDbContext.cs
│   └── UnitOfWork.cs
├── Repositories/
│   ├── Repository.cs
│   ├── UserRepository.cs
│   └── ProjectRepository.cs
├── Configurations/
│   ├── UserConfiguration.cs
│   └── ProjectConfiguration.cs
└── Services/
    ├── CacheService.cs
    └── EmailService.cs
```

**Design Decisions:**
- ✅ Generic repository base class
- ✅ Fluent API for entity configuration
- ✅ Unit of Work for transactions
- ✅ Optimized EF Core queries

---

### 4. API Layer (Presentation)

**Purpose:** Exposes HTTP endpoints and handles requests.

**Responsibilities:**
- Define REST API endpoints
- Handle HTTP requests/responses
- Implement middleware
- Configure dependency injection
- API documentation (Swagger)

**Dependencies:** Application Layer, Infrastructure Layer

**Key Components:**
```
API/
├── Controllers/
│   ├── AuthController.cs
│   ├── ProjectsController.cs
│   └── BidsController.cs
├── Middleware/
│   ├── ExceptionHandlingMiddleware.cs
│   └── RequestLoggingMiddleware.cs
├── Filters/
│   └── ValidationFilter.cs
└── Extensions/
    └── ServiceExtensions.cs
```

**Design Decisions:**
- ✅ RESTful conventions
- ✅ Global exception handling
- ✅ Consistent response format
- ✅ API versioning ready

---

## 🔐 Authentication Architecture

### JWT Token Flow

```
┌─────────────┐
│   Client    │
└──────┬──────┘
       │ 1. POST /api/auth/login
       │    { email, password }
       ▼
┌─────────────────────────────────┐
│      AuthController             │
└──────┬──────────────────────────┘
       │ 2. Validate credentials
       ▼
┌─────────────────────────────────┐
│      AuthService                │
│  - Verify password              │
│  - Generate Access Token        │
│  - Generate Refresh Token       │
│  - Store Refresh Token in DB    │
└──────┬──────────────────────────┘
       │ 3. Return tokens
       ▼
┌─────────────┐
│   Client    │
│  Stores:    │
│  - Access   │
│  - Refresh  │
└──────┬──────┘
       │ 4. Subsequent requests
       │    Authorization: Bearer {access_token}
       ▼
┌─────────────────────────────────┐
│  JwtMiddleware                  │
│  - Validate token               │
│  - Extract claims               │
│  - Set User context             │
└──────┬──────────────────────────┘
       │ 5. Authorized request
       ▼
┌─────────────────────────────────┐
│      Controller                 │
└─────────────────────────────────┘
```

### Token Refresh Flow

```
Access Token Expired
       │
       ▼
Client sends Refresh Token
       │
       ▼
Server validates Refresh Token
       │
       ├─ Valid ──────────┐
       │                  ▼
       │         Generate new Access Token
       │         Generate new Refresh Token
       │         Revoke old Refresh Token
       │         Return new tokens
       │
       └─ Invalid ────────┐
                          ▼
                   Return 401 Unauthorized
                   Client redirects to login
```

---

## 💾 Database Design

### Entity Relationships

```
Users (1) ──────────── (1) FreelancerProfile
  │                         
  │ (1)                     
  │                         
  ├──────────── (1) ClientProfile
  │
  │ (1)
  │
  ├──────────── (*) RefreshTokens
  │
  │ (1)
  │
  ├──────────── (*) Projects (as Client)
  │
  │ (1)
  │
  ├──────────── (*) Bids (as Freelancer)
  │
  │ (1)
  │
  ├──────────── (*) Messages (as Sender)
  │
  │ (1)
  │
  ├──────────── (*) Messages (as Receiver)
  │
  │ (1)
  │
  ├──────────── (*) Payments (as Sender)
  │
  │ (1)
  │
  ├──────────── (*) Payments (as Receiver)
  │
  │ (1)
  │
  ├──────────── (*) Reviews (as Reviewer)
  │
  │ (1)
  │
  ├──────────── (*) Reviews (as Reviewee)
  │
  │ (1)
  │
  ├──────────── (*) Notifications
  │
  │ (1)
  │
  └──────────── (*) UserActivities


Projects (1) ──────── (*) Bids
    │
    │ (1)
    │
    ├──────────── (*) Milestones
    │
    │ (1)
    │
    └──────────── (*) Payments
```

### Indexing Strategy

**High-Performance Indexes:**

```sql
-- Users
CREATE INDEX idx_users_email ON Users(Email);
CREATE INDEX idx_users_role ON Users(Role);
CREATE INDEX idx_users_status ON Users(Status);

-- Projects
CREATE INDEX idx_projects_status ON Projects(Status);
CREATE INDEX idx_projects_category ON Projects(Category);
CREATE INDEX idx_projects_client_id ON Projects(ClientId);
CREATE INDEX idx_projects_published_at ON Projects(PublishedAt);

-- Bids
CREATE UNIQUE INDEX idx_bids_project_freelancer ON Bids(ProjectId, FreelancerId);
CREATE INDEX idx_bids_status ON Bids(Status);

-- Payments
CREATE UNIQUE INDEX idx_payments_transaction_id ON Payments(TransactionId);
CREATE INDEX idx_payments_status ON Payments(Status);

-- Messages
CREATE INDEX idx_messages_sender_id ON Messages(SenderId);
CREATE INDEX idx_messages_receiver_id ON Messages(ReceiverId);
CREATE INDEX idx_messages_is_read ON Messages(IsRead);

-- RefreshTokens
CREATE UNIQUE INDEX idx_refresh_tokens_token ON RefreshTokens(Token);
CREATE INDEX idx_refresh_tokens_user_id ON RefreshTokens(UserId);
```

---

## 🚀 Performance Optimization

### Caching Strategy

**Redis Caching Layers:**

```
┌─────────────────────────────────────┐
│     Application Layer               │
└──────────┬──────────────────────────┘
           │
           ▼
    ┌──────────────┐
    │ Cache Check  │
    └──────┬───────┘
           │
    ┌──────┴──────┐
    │             │
    ▼             ▼
  Hit           Miss
    │             │
    │             ▼
    │      ┌──────────────┐
    │      │  Database    │
    │      └──────┬───────┘
    │             │
    │             ▼
    │      ┌──────────────┐
    │      │  Cache Set   │
    │      └──────┬───────┘
    │             │
    └─────────────┘
           │
           ▼
    ┌──────────────┐
    │   Response   │
    └──────────────┘
```

**Cached Data:**
- User profiles (15 min TTL)
- Project listings (5 min TTL)
- Freelancer profiles (30 min TTL)
- Notification counts (1 min TTL)

### Query Optimization

**EF Core Best Practices:**

```csharp
// ✅ Good: Explicit eager loading
var projects = await _context.Projects
    .Include(p => p.Client)
    .Include(p => p.Bids)
        .ThenInclude(b => b.Freelancer)
    .Where(p => p.Status == ProjectStatus.Open)
    .ToListAsync();

// ❌ Bad: N+1 queries
var projects = await _context.Projects.ToListAsync();
foreach (var project in projects)
{
    var client = await _context.Users.FindAsync(project.ClientId);
}

// ✅ Good: Projection for DTOs
var projectDtos = await _context.Projects
    .Select(p => new ProjectDto
    {
        Id = p.Id,
        Title = p.Title,
        ClientName = p.Client.FirstName + " " + p.Client.LastName
    })
    .ToListAsync();

// ✅ Good: AsNoTracking for read-only queries
var projects = await _context.Projects
    .AsNoTracking()
    .Where(p => p.Status == ProjectStatus.Open)
    .ToListAsync();
```

---

## 🔒 Security Architecture

### Defense in Depth

```
Layer 1: Network Security
  ├─ HTTPS/TLS encryption
  ├─ Firewall rules
  └─ DDoS protection

Layer 2: API Gateway
  ├─ Rate limiting
  ├─ IP whitelisting
  └─ Request throttling

Layer 3: Authentication
  ├─ JWT validation
  ├─ Token expiration
  └─ Refresh token rotation

Layer 4: Authorization
  ├─ Role-based access control
  ├─ Resource ownership checks
  └─ Policy-based authorization

Layer 5: Input Validation
  ├─ FluentValidation
  ├─ Model binding validation
  └─ SQL injection prevention

Layer 6: Data Protection
  ├─ Password hashing (BCrypt)
  ├─ Sensitive data encryption
  └─ Secure configuration management

Layer 7: Audit & Monitoring
  ├─ Activity logging
  ├─ Security event tracking
  └─ Anomaly detection
```

---

## 📊 Scalability Strategy

### Horizontal Scaling

```
┌─────────────────────────────────────────┐
│         Load Balancer (NGINX)           │
└────────┬────────┬────────┬──────────────┘
         │        │        │
    ┌────▼───┐ ┌─▼────┐ ┌─▼────┐
    │ API 1  │ │ API 2│ │ API 3│
    └────┬───┘ └──┬───┘ └──┬───┘
         │        │        │
         └────────┴────────┘
                  │
         ┌────────▼────────┐
         │  Shared Redis   │
         └────────┬────────┘
                  │
         ┌────────▼────────┐
         │  MySQL Cluster  │
         │  (Master/Slave) │
         └─────────────────┘
```

### Vertical Scaling

- **Database:** Increase CPU, RAM, SSD
- **Redis:** Increase memory
- **API:** Increase CPU cores

### Microservices Evolution

```
Monolith (Current)
       │
       ▼
Modular Monolith
       │
       ▼
Microservices
  ├─ Auth Service
  ├─ Project Service
  ├─ Payment Service
  ├─ Messaging Service
  └─ Notification Service
```

---

## 🛠️ Development Workflow

### Git Branching Strategy

```
main (production)
  │
  ├─ develop (staging)
  │    │
  │    ├─ feature/user-authentication
  │    ├─ feature/project-management
  │    ├─ bugfix/payment-calculation
  │    └─ hotfix/security-patch
  │
  └─ release/v1.0.0
```

### CI/CD Pipeline

```
1. Code Push
   ↓
2. Build & Compile
   ↓
3. Run Unit Tests
   ↓
4. Run Integration Tests
   ↓
5. Code Quality Analysis (SonarQube)
   ↓
6. Security Scan
   ↓
7. Build Docker Image
   ↓
8. Push to Container Registry
   ↓
9. Deploy to Staging
   ↓
10. Automated Tests
   ↓
11. Manual Approval
   ↓
12. Deploy to Production
   ↓
13. Health Check
   ↓
14. Rollback (if needed)
```

---

## 📈 Monitoring & Observability

### Metrics to Track

**Application Metrics:**
- Request rate (req/sec)
- Response time (p50, p95, p99)
- Error rate (%)
- Active users
- API endpoint usage

**Infrastructure Metrics:**
- CPU usage (%)
- Memory usage (%)
- Disk I/O
- Network throughput
- Database connections

**Business Metrics:**
- User registrations
- Projects created
- Bids submitted
- Payments processed
- Active freelancers/clients

### Logging Strategy

```
Structured Logging (Serilog)
  ├─ Information: Normal operations
  ├─ Warning: Potential issues
  ├─ Error: Handled exceptions
  └─ Critical: System failures

Log Destinations:
  ├─ Console (Development)
  ├─ File (Production)
  ├─ Application Insights (Cloud)
  └─ ELK Stack (Enterprise)
```

---

## 🎯 Future Enhancements

### Phase 2 Features

- [ ] Real-time messaging (SignalR)
- [ ] File upload/storage (Azure Blob)
- [ ] Email notifications (SendGrid)
- [ ] SMS notifications (Twilio)
- [ ] Payment gateway integration (Stripe)
- [ ] Advanced search (Elasticsearch)
- [ ] GraphQL API
- [ ] gRPC for internal services

### Phase 3 Features

- [ ] Machine learning recommendations
- [ ] Fraud detection system
- [ ] Multi-language support (i18n)
- [ ] Mobile app backend (BFF pattern)
- [ ] Blockchain-based escrow
- [ ] AI-powered matching algorithm

---

## 📚 References

### Official Documentation

- [ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [SOLID Principles](https://en.wikipedia.org/wiki/SOLID)

### Recommended Reading

- "Clean Architecture" by Robert C. Martin
- "Domain-Driven Design" by Eric Evans
- "Patterns of Enterprise Application Architecture" by Martin Fowler
- "Building Microservices" by Sam Newman

---

**Last Updated:** 2026-05-23  
**Version:** 1.0.0  
**Maintained by:** FreelanceHub Development Team
