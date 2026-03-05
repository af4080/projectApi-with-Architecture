# Copilot Instructions for projectApiAngular

## Project Summary
This is a .NET 8 ASP.NET Core Web API backend for an Angular frontend application managing a Chinese sales gift system. The API handles user authentication, gift donations, categories, purchases, baskets, and lotteries. It supports donors contributing gifts, users purchasing raffle tickets, and admins managing the system. Key features include JWT-based authentication with role-based access (admin for management), CRUD operations on gifts, categories, and users, basket management, purchase tracking, and lottery drawing for gift winners.

## Tech Stack
- **Framework**: .NET 8, ASP.NET Core Web API with implicit usings and nullable reference types enabled
- **Database**: SQL Server with Entity Framework Core 8 (code-first approach with migrations)
- **Authentication**: JWT Bearer tokens with BCrypt.Net-Next for password hashing
- **Logging**: Serilog with file sink (daily rolling logs in Logs/ directory)
- **Documentation**: Swagger/OpenAPI with JWT bearer authentication support
- **Other Libraries**: CsvHelper for data export, System.IdentityModel.Tokens.Jwt for JWT handling
- **CORS**: Configured for Angular dev server at http://localhost:4200

## Coding Guidelines
- **Asynchronous Programming**: Use async/await for all database and I/O operations; avoid blocking calls
- **Dependency Injection**: Register services as scoped in Program.cs; inject interfaces (e.g., ILogger, repositories)
- **Data Transfer**: Use DTOs for all API inputs/outputs with DataAnnotations for validation (Required, MaxLength, etc.)
- **Logging**: Inject ILogger in services and log key operations (Information level for successes, errors for failures)
- **API Design**: Follow RESTful conventions; controllers return IActionResult with appropriate status codes
- **Entity Framework**: Use explicit Include() for navigation properties; avoid lazy loading
- **Exception Handling**: Use try-catch in controllers; custom middleware handles GiftAlreadyAssignedException
- **Naming Conventions**: PascalCase for classes, methods, properties; camelCase for private fields; interfaces prefixed with 'I'
- **Code Organization**: Business logic in services, data access in repositories; keep controllers thin
- **Security**: Validate inputs; use BCrypt for passwords; JWT for auth; admin role for sensitive operations
- **Static Files**: Images stored in wwwroot/, paths as strings in database

## Project Structure
- **Controllers/**: API endpoints organized by entity (e.g., GiftController, AuthController); most require JWT auth, admin role for CRUD
- **Services/**: Business logic layer with interfaces (e.g., IGiftService); handle mapping between entities and DTOs
- **Repositories/**: Data access layer with EF Core queries; interfaces define contracts
- **Models/**: Entity classes (User, Gift, Category, etc.) with EF configurations in DbContext
- **DTO/**: Request/response DTOs (ReadGiftDto, CreateGiftDto) with validation
- **Configurations/**: JwtSettings class for token configuration
- **Middlewares/**: RequestLog for logging requests; custom exception handler for GiftAlreadyAssignedException
- **Migrations/**: EF Core migration files for database schema changes
- **Validations/**: Custom attributes (e.g., StrongPasswordAttribute)
- **wwwroot/**: Static assets like gift images
- **Logs/**: Serilog output directory (created at runtime)
- **Properties/launchSettings.json**: Launch profiles for HTTP/HTTPS

## Database Schema Overview
- **Users**: Id, Name, Email (unique), Password (hashed), Phone, Role (admin/user)
- **Donners**: Id, Name, Email (unique), Phone; one-to-many with Gifts
- **Categories**: Id, Name (unique); one-to-many with Gifts
- **Gifts**: Id, Name (unique), Description, Price, ImagePath, CategoryId, DonerId, WinnerId (nullable)
- **Purchases**: Id, PurchaseDate, CustomerId, GiftId; tracks raffle ticket purchases
- **Baskets**: Id, UserId, GiftId (unique pair); cart functionality
- **Manager**: Commented out in DbContext (not used)
- Relationships: Restrict delete on foreign keys to prevent data loss

## Existing Tools and Resources
- **Database Connection**: Hardcoded to local SQL Server (DESKTOP-SVITPH4\Chinese_Sale); use integrated security
- **Swagger UI**: Accessible at /swagger; includes JWT auth setup
- **Launch Profiles**: HTTP (5125), HTTPS (7097); launches browser to Swagger
- **CORS Policy**: "AllowLocalhost" for Angular frontend
- **Logging Configuration**: Serilog writes to Logs/app-.log with daily intervals
- **Testing Tools**: projectApiAngular.http for basic API testing; no unit/integration tests
- **EF Tools**: dotnet ef commands for migrations (add, update, etc.)
- **Package Manager**: NuGet packages listed in .csproj

## Setup and Build Instructions
1. **Prerequisites**: SQL Server installed and running; .NET 8 SDK
2. **Database Setup**: Ensure database exists or run `dotnet ef database update` to apply migrations
3. **Restore Dependencies**: `dotnet restore`
4. **Build**: `dotnet build` (outputs to bin/Debug/net8.0/)
5. **Run**: `dotnet run` (starts on http://localhost:5125; Swagger at /swagger)
6. **Debug**: Use VS Code debugger or attach to process
7. **Migrations**: If schema changes, `dotnet ef migrations add <Name>` then update

## Common Patterns and Workflows
- **Adding a New Entity**: Create Model, add to DbContext with configurations, create Repository/Interface, Service/Interface, DTOs, Controller endpoint
- **Authentication Flow**: Login endpoint returns JWT; client includes in Authorization header
- **CRUD Operations**: Repository handles EF queries; Service maps to DTOs and logs; Controller returns HTTP responses
- **Lottery System**: Purchases represent tickets; lottery service draws winners randomly
- **Basket Management**: Unique User-Gift pairs; prevents duplicates
- **Image Handling**: Upload to wwwroot/, store path in Gift.ImagePath
- **Validation**: Use [Required], [MaxLength] on DTOs; custom attributes in Validations/
- **Error Handling**: Controllers catch exceptions; middleware handles custom exceptions
- **Pagination**: Implemented in GiftService for large datasets
- **Unique Constraints**: Enforced in DbContext OnModelCreating (emails, names)

## Potential Gotchas and Best Practices
- **Database Connection**: Hardcoded; change in appsettings.json for different environments
- **JWT Security**: Secret key in appsettings (not production-ready); use environment variables
- **Manager Entity**: Commented out; if needed, uncomment and add migrations
- **Custom Exception**: GiftAlreadyAssignedException thrown when assigning winner to already won gift
- **EF Includes**: Always include related entities to avoid N+1 queries
- **Async Best Practices**: Use ToListAsync(), FirstOrDefaultAsync(); avoid synchronous EF methods
- **CORS**: Only localhost:4200 allowed; update for production
- **Static Files**: Served from wwwroot/; ensure paths are relative
- **Migrations**: Run update before starting app; check for pending migrations
- **Logging**: Check Logs/ directory for errors; Serilog configured in Program.cs
- **No Unit Tests**: Add xUnit/Moq for testing services/repositories
- **API Versioning**: Not implemented; consider for future changes
- **Rate Limiting**: Not configured; add for production security</content>
<parameter name="filePath">c:\לימודים\יד\project api-angular\xxxxx\.github\copilot-instructions.md