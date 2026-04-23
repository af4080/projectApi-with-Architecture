# Copilot Instructions for Controllers in projectApiAngular

## Project Context
This document provides detailed instructions for the controllers in the projectApiAngular, a .NET 8 ASP.NET Core Web API for a Chinese sales gift system. Refer to the main `.github/copilot-instructions.md` for overall project guidelines, tech stack, and setup.

## Controllers Overview
The API uses RESTful conventions with controllers organized by entity. Most endpoints require JWT authentication, with role-based access (admin for management operations, user for personal actions). Controllers inject services for business logic and return `IActionResult` with appropriate HTTP status codes.

## Controller Coding Guidelines
- **Asynchronous Programming**: All actions are `async Task<IActionResult>`; use `await` for service calls.
- **Authorization**: Use `[Authorize]` attributes; `[AllowAnonymous]` for public endpoints.
- **Model Validation**: Check `ModelState.IsValid`; return `BadRequest(ModelState)` if invalid.
- **Error Handling**: Wrap service calls in try-catch; return `BadRequest` for exceptions, `NotFound` for missing resources.
- **Logging**: Inject `ILogger<TController>` and log key operations.
- **Response Formats**: Use DTOs for inputs/outputs; return JSON with `Ok()`, `CreatedAtAction()`, etc.
- **Routing**: Standard REST routes like `api/[controller]`; use attributes for custom routes.
- **Dependency Injection**: Inject interfaces (e.g., `IService`) in constructor.

## AuthController
Handles user authentication.

**Authorization**: None required.

**Endpoints**:
- `POST /api/auth/login`: Logs in user with email/password. Returns JWT token.
- `POST /api/auth/register`: Registers new user. Returns created user DTO.

## BasketController
Manages user shopping baskets.

**Authorization**: Requires "user" role.

**Endpoints**:
- `GET /api/basket/me`: Gets current user's basket items.
- `POST /api/basket`: Adds item to basket.
- `PUT /api/basket/{id}/amount`: Updates quantity of basket item.
- `DELETE /api/basket/{id}`: Removes item from basket.
- `POST /api/basket/buy-all`: Purchases all items in basket.

## CategoryController
Manages gift categories.

**Authorization**: Requires "admin" role.

**Endpoints**:
- `GET /api/category`: Gets all categories.
- `POST /api/category`: Creates new category.
- `DELETE /api/category/{id}`: Deletes category.
- `PUT /api/category/{id}`: Updates category.

## DonnerController
Manages gift donors.

**Authorization**: Requires "admin" role.

**Endpoints**:
- `GET /api/donner`: Gets all donors.
- `GET /api/donner/{id}`: Gets donor by ID.
- `GET /api/donner/byname/{name}`: Gets donor by name.
- `GET /api/donner/byemail?email=...`: Gets donor by email.
- `GET /api/donner/bygift/{giftId}`: Gets donor by associated gift ID.
- `POST /api/donner`: Creates new donor.
- `DELETE /api/donner/{id}`: Deletes donor.
- `PUT /api/donner/{id}`: Updates donor.

## GiftController
Manages gifts.

**Authorization**: Admin for CRUD; anonymous for reads.

**Endpoints**:
- `GET /api/gift`: Gets all gifts (anonymous).
- `GET /api/gift/{name}`: Gets gift by name (anonymous).
- `GET /api/gift/doner/{name}`: Gets gifts by donor name.
- `GET /api/gift/numcustomer/{count}`: Gets gifts by customer count.
- `POST /api/gift`: Creates new gift.
- `DELETE /api/gift/{id}`: Deletes gift.
- `PATCH /api/gift/{name}`: Updates gift.
- `GET /api/gift/paged?pageNumber=1&pageSize=10`: Gets paginated gifts (anonymous).
- `GET /api/gift/{giftId}/winner`: Gets winner for gift (anonymous).

## LotteryController
Handles lottery operations and winners.

**Authorization**: Requires "admin" role.

**Endpoints**:
- `POST /api/lottery`: Runs lottery for all gifts.
- `POST /api/lottery/{giftName}`: Runs lottery for specific gift.
- `GET /api/lottery`: Gets all gift winners.
- `PUT /api/lottery/newSale`: Starts new sale (resets winners).
- `GET /api/lottery/downled-winner-zip`: Downloads winners as ZIP file.

## PurcheseController
Manages purchases (raffle ticket buys).

**Authorization**: User for adding; admin for reports.

**Endpoints**:
- `POST /api/purchese`: Adds new purchase (user).
- `GET /api/purchese/buyers`: Gets buyers details (admin).
- `GET /api/purchese/gifts/sorted`: Gets gifts sorted by sales (admin).
- `GET /api/purchese/gift/{name}`: Gets purchases for gift (admin).
- `GET /api/purchese/ordered-by-price`: Gets purchases ordered by price (admin).
- `GET /api/purchese/total-revenue`: Gets total sales revenue (admin).

## Common Patterns
- **CRUD Operations**: Standard GET/POST/PUT/DELETE for entities.
- **Pagination**: Used in GiftController for large datasets.
- **File Downloads**: LotteryController for ZIP exports.
- **Basket Management**: Unique user-gift pairs; buy-all converts to purchases.
- **Lottery**: Random winner selection; prevents re-assignment.
- **Validation**: Required fields, max lengths on DTOs.
- **Exception Handling**: Custom exceptions like GiftAlreadyAssignedException handled in middleware.</content>
<parameter name="filePath">c:\לימודים\יד\project api-angular\project-wiith-Architecture\.github\copilot-instructions-controllers.md