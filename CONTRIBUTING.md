# Contributing to FreelanceHub Core API

Thank you for your interest in contributing to FreelanceHub Core API! This document provides guidelines and instructions for contributing.

## 🤝 How to Contribute

### Reporting Bugs

If you find a bug, please create an issue with:
- Clear description of the problem
- Steps to reproduce
- Expected vs actual behavior
- Environment details (.NET version, OS, etc.)

### Suggesting Features

Feature requests are welcome! Please:
- Check if the feature already exists or is planned
- Provide clear use cases
- Explain why this feature would be valuable

### Pull Requests

1. **Fork the repository**
2. **Create a feature branch**
   ```bash
   git checkout -b feature/your-feature-name
   ```

3. **Make your changes**
   - Follow the existing code style
   - Write clean, maintainable code
   - Add tests for new functionality
   - Update documentation as needed

4. **Test your changes**
   ```bash
   dotnet test
   ```

5. **Commit your changes**
   ```bash
   git commit -m "feat: add new feature"
   ```
   
   Use conventional commits:
   - `feat:` - New feature
   - `fix:` - Bug fix
   - `docs:` - Documentation changes
   - `test:` - Adding tests
   - `refactor:` - Code refactoring
   - `style:` - Code style changes
   - `chore:` - Maintenance tasks

6. **Push to your fork**
   ```bash
   git push origin feature/your-feature-name
   ```

7. **Create a Pull Request**
   - Provide a clear description
   - Reference any related issues
   - Ensure all tests pass

## 📋 Development Guidelines

### Code Style

- Follow C# coding conventions
- Use meaningful variable and method names
- Keep methods small and focused
- Add XML documentation for public APIs

### Architecture

This project follows **Clean Architecture**:
- **Domain Layer**: Business entities and interfaces (no dependencies)
- **Application Layer**: Business logic and DTOs (depends on Domain)
- **Infrastructure Layer**: Data access and external services (depends on Domain)
- **API Layer**: Controllers and middleware (depends on Application and Infrastructure)

### Testing

- Write unit tests for business logic
- Use meaningful test names: `MethodName_Scenario_ExpectedResult`
- Aim for high code coverage (>80%)
- Mock external dependencies

### Database Migrations

When adding/modifying entities:
```bash
dotnet ef migrations add YourMigrationName --project src/FreelanceHub.Infrastructure --startup-project src/FreelanceHub.API
```

### Running Locally

1. **Prerequisites**
   - .NET 8.0 SDK
   - MySQL 8.0+
   - Redis (optional)

2. **Setup**
   ```bash
   # Clone the repository
   git clone https://github.com/yourusername/FreelanceHub-Core-API.git
   cd FreelanceHub-Core-API

   # Restore dependencies
   dotnet restore

   # Update connection string in appsettings.Development.json
   
   # Run migrations
   dotnet ef database update --project src/FreelanceHub.Infrastructure --startup-project src/FreelanceHub.API

   # Run the application
   dotnet run --project src/FreelanceHub.API
   ```

3. **Using Docker**
   ```bash
   docker-compose up -d
   ```

## 🔍 Code Review Process

All submissions require review. We use GitHub pull requests for this purpose:
- Maintainers will review your code
- Address any feedback or requested changes
- Once approved, your PR will be merged

## 📝 Documentation

- Update README.md if adding new features
- Add XML comments to public APIs
- Update ARCHITECTURE.md for architectural changes
- Keep QUICKSTART.md up to date

## ⚖️ License

By contributing, you agree that your contributions will be licensed under the MIT License.

## 💬 Questions?

Feel free to open an issue for any questions or clarifications.

---

**Thank you for contributing to FreelanceHub Core API!** 🎉
