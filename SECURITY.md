# Security Policy

## Supported Versions

| Version | Supported          |
| ------- | ------------------ |
| 1.0.x   | :white_check_mark: |

## Reporting a Vulnerability

If you discover a security vulnerability, please follow these steps:

### 1. Do Not Open a Public Issue

Please do not create a public GitHub issue for security vulnerabilities.

### 2. Report Privately

Send an email to: **security@freelancehub.com** (or your email)

Include:
- Description of the vulnerability
- Steps to reproduce
- Potential impact
- Suggested fix (if any)

### 3. Response Time

- **Initial Response**: Within 48 hours
- **Status Update**: Within 7 days
- **Fix Timeline**: Depends on severity

## Security Measures

### Authentication & Authorization
- JWT tokens with 15-minute expiration
- Refresh token rotation
- BCrypt password hashing (cost factor: 12)
- Role-based access control (RBAC)
- Account lockout after 5 failed attempts

### Data Protection
- HTTPS only in production
- Sensitive data encryption at rest
- SQL injection prevention (parameterized queries)
- XSS protection (input sanitization)
- CSRF protection

### API Security
- Rate limiting (1000 req/hour for authenticated users)
- CORS configuration
- Request validation with FluentValidation
- Global exception handling (no sensitive data in errors)

### Infrastructure
- Docker container isolation
- Environment variable management
- Secrets not committed to repository
- Regular dependency updates

## Best Practices for Contributors

1. Never commit secrets, API keys, or passwords
2. Use environment variables for sensitive configuration
3. Follow OWASP Top 10 guidelines
4. Run security tests before submitting PRs
5. Keep dependencies up to date

## Security Updates

Security patches will be released as soon as possible after verification. Users will be notified via:
- GitHub Security Advisories
- Release notes
- Email (for critical vulnerabilities)

## Acknowledgments

We appreciate responsible disclosure and will acknowledge security researchers who report vulnerabilities (with permission).
