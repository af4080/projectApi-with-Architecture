Plan: .NET 8 API → Scalable Microservices
TL;DR: Decompose your monolith into 5 autonomous services (Identity, Catalog, Shopping, Lottery, Gateway) with REST inter-service calls for state queries and asynchronous events for domain events. Each service owns its database; Shopping and Lottery use denormalized read-only caches of Catalog/User data, synchronized via events. This enables independent scaling, team autonomy, and flexible deployments while maintaining hybrid consistency (strict ACID for purchases, eventual consistency for lottery outcomes).

Steps
Phase 1: Foundation & Infrastructure Setup
Create shared infrastructure project → Services

IServiceClient interface for REST calls (circuit breaker pattern)
IEventPublisher interface for async messaging (abstraction over RabbitMQ/Service Bus)
EventContracts namespace with domain events (PurchaseCreatedEvent, GiftWinnerSelectedEvent, etc.)
Correlation ID middleware for distributed tracing
Update Program.cs to register shared services
Set up message broker (RabbitMQ/Azure Service Bus priority—use Docker for dev)

Define exchange/queue topology for domain events
Create message models in shared infrastructure
Create API Gateway scaffold → New project ProjectApiAngular.Gateway

Single entry point for all client requests
Centralized JWT validation
Route configuration (Catalog service at /catalog/*, Shopping at /shopping/*, etc.)
Rate limiting and request logging
Technology: Ocelot (.NET library) or reverse proxy (nginx Docker container)
Phase 2: Decompose into Services (Sequential)
2A. Extract Identity Service → New project ProjectApiAngular.Identity
Move from AuthController.cs: Register, Login endpoints
Move UserService.cs and IUserRepository
Move User.cs model and EF configuration
Create new database Chinese_Sales_Identity
Expose internal gRPC endpoints: ValidateUserAsync(), GetUserAsync()
Implement IServiceClient for outbound calls (none initially)
Tests: Verify JWT generation, password hashing, role assignment intact
2B. Extract Catalog Service → New project ProjectApiAngular.Catalog
Move from GiftController.cs: All CRUD endpoints for Gifts
Move from CategoryController.cs: All CRUD for Categories
Move from DonnerController.cs: All CRUD for Donners
Move services: GiftService, CategoryService, DonnerService + repositories
Move models: Gift.cs, Category.cs, Donner.cs
Create new database Chinese_Sales_Catalog
Add IEventPublisher to publish GiftCreatedEvent, GiftUpdatedEvent when entities change
Consume GiftWinnerSelectedEvent from Lottery Service → update Gift.WinnerId
Internal data validator: admin role check (calls Identity Service via gRPC)
Tests: CRUD operations, event publishing
2C. Extract Shopping Service → New project ProjectApiAngular.Shopping
Move from BasketController.cs: All Basket endpoints
Move from PurcheseController.cs: Checkout, Purchase history
Move services: BasketService, PurcheseService + repositories
Move models: Basket.cs, Purchase.cs
Create new database Chinese_Sales_Shopping
Denormalization layer:
Add UserCache table (Id, Name, Email) ← synced from Identity via UserRegisteredEvent
Add GiftCache table (Id, Name, Price, ImagePath, DonerId) ← synced from Catalog via GiftCreatedEvent, GiftUpdatedEvent
Event handling: Subscribe to GiftWinnerSelectedEvent → cache that gift is unavailable
External calls: Call Catalog Service to validate gift exists before purchase
Data consistency:
POST /shopping/basket/checkout is synchronous (must succeed/fail atomically)
Publish PurchaseCreatedEvent after successful commit
Tests: Basket CRUD, checkout flow, cache synchronization
2D. Extract Lottery Service → New project ProjectApiAngular.Lottery
Move LotteryController.cs: Draw endpoints
Move LotteryService logic
Create standalone database Chinese_Sales_Lottery or use shared read access
Data sources:
Subscribe to PurchaseCreatedEvent → add ticket to in-memory pool per gift
Call Catalog Service gRPC to fetch Gift details (read-only)
Call Shopping Service REST to fetch all Purchases (read-only snapshot)
Drawing logic:
SELECT random User from Purchases for each Gift
Publish GiftWinnerSelectedEvent (User ID, Gift ID)
No direct database write—rely on Catalog Service consuming event to update winner
Compensation: If Catalog Service rejects update, implement Saga retry logic
Tests: Lottery drawing, event publishing, eventual consistency verification
2E. Create API Gateway (if using Ocelot) → ProjectApiAngular.Gateway project
Ocelot configuration: Route /api/catalog/* → http://catalog-service:5001
Route /api/shopping/* → http://shopping-service:5002
Route /api/lottery/* → http://lottery-service:5003
Auth middleware: intercept all requests, validate JWT from Identity Service
Failed auth → 401 Unauthorized
Pass x-correlation-id header to downstream services for tracing
2F. Update existing Program.cs → becomes bare entry point
Keep only API Gateway registration
Remove individual service registrations (they now have their own Program.cs)
Legacy clients redirected to Gateway
Phase 3: Data Migration & Consistency
Database split strategy:

Run all 4 service databases in parallel (don't delete monolith DB yet)
Write migration script: copy Users table → Chinese_Sales_Identity
Copy Gifts, Categories, Donners → Chinese_Sales_Catalog
Copy Baskets, Purchases → Chinese_Sales_Shopping
Validate row counts match
Event-driven cache synchronization:

When new User created in Identity Service → publish UserCreatedEvent
Shopping Service subscribes → inserts into UserCache
Same pattern for Gifts and Catalog changes
Verify caches stay synchronized (monitoring query)
Test data consistency:

Create Gift in Catalog → Shopping Service cache receives event within 2 seconds
Make Purchase → Lottery Service sees ticket in pool
Run Lottery → Catalog Service receives winner update event
Verify Gift.WinnerId populated end-to-end
Phase 4: Integration & Testing
Contract testing (Pact or similar)

Define Shopping Service consumer contract: calls Catalog /gifts/{id} and expects {id, price, name, donerId}
Catalog Service provider contract: guarantees that schema
Verify both sides before deploy
Trace distributed transactions:

Add correlation ID middleware in each service
Log all inter-service calls with correlation ID
Track flow: User buys gift → Shopping publishes event → Lottery consumes → Catalog updates
Use ELK Stack or Azure Application Insights for centralized logs
Deployment readiness:

Each service gets independent Docker container
Optional Kubernetes manifests (or Docker Compose for now)
Health check endpoints on each service (/health)
Graceful shutdown on SIGTERM (drain in-flight requests)
Phase 5: Cutover & Decommission
Run parallel monolith + microservices for 1-2 weeks

Sync writes to both systems
Read traffic routed to microservices
Monitor error rates
Switch traffic to API Gateway

All client requests now go to Gateway
Legacy monolith in read-only mode (for data validation)
Decommission monolith

Archive database backup
Delete Chinese_SalesDbContext.cs (no longer used)
Remove unused controllers/services from legacy project
Verification
After Phase 2 (services extracted):

✅ Each service has independent Program.cs and runs on separate port (5125, 5126, 5127, etc.)
✅ API calls via Gateway return same responses as monolith
✅ Events publish to message broker (visible in RabbitMQ Management UI or Service Bus metrics)
✅ Services subscribe to events (verify in logs: "Event received...")
After Phase 3 (data migrated):

✅ Query Chinese_Sales_Identity.Users → count matches original
✅ Query Chinese_Sales_Catalog.Gifts → count matches original
✅ Run SELECT COUNT(*) on all cache tables → equal to source tables
After Phase 4 (integrated):

✅ Create a new user in Identity Service
✅ Shopping Service cache receives UserCreatedEvent → verify in cache within 2 seconds
✅ Create a Basket, Checkout → generates Purchase
✅ Lottery Service sees Purchase in event stream
✅ Run Lottery draw → GiftWinnerSelectedEvent published
✅ Catalog Service receives event → Gift.WinnerId populated
✅ Frontend queries gift via Gateway → WinnerId is set
Manual validation:

Decisions
Why 5 services + Gateway? Gateway separates auth/routing from business logic; 4 core services align to bounded contexts (Identity, Catalog, Shopping, Lottery)
Why REST + async? REST for queries (Catalog lookups) and commands (Basket add); async events for domain notifications (Purchase created, Winner selected) → decoupling + eventual consistency
Why denormalized caches? Avoids N+1 queries and inter-service latency; Shopping doesn't need to call Catalog every basket view
Why Hybrid consistency? Purchases must be ACID (consistency within Shopping DB); Lottery outcome propagation can be eventual (Catalog updates within seconds)
Why separate databases? Eliminates shared schema coordination; scaling independent services; avoids single DB bottleneck; replaces cascading deletes with event-driven cleanup
Deployment undecided? Recommend Docker Compose for dev, Kubernetes for production (but plan works with IIS too—just deploy each .NET service to separate Windows Service)
Does this plan align with your vision? Any adjustments needed before we proceed to implementation?