# Inventory Management System

## Build:

docker compose up -d --build

## Links:

#### Frontends

- swagger: http://localhost:8080/index.html
- mailhog (stmp mock server): http://localhost:8025

#### APIs

- audit mock server: http://localhost:1081
- wms mock server: http://localhost:1080

## Tests

[![Tests](https://github.com/mateus-u/inventory-management/actions/workflows/run-tests-and-update-readme.yml/badge.svg)](https://github.com/mateus-u/inventory-management/actions/workflows/run-tests-and-update-readme.yml)

## Architecture & Design

### 1. Clean Architecture
Strongly influenced by [Jason Taylor's Clean Architecture template](https://github.com/jasontaylordev/CleanArchitecture), the solution follows a layered approach with clear separation of concerns:
- **Domain Layer**: Core business logic, entities, and domain events
- **Application Layer**: Use cases, business rules orchestration
- **Infrastructure Layer**: External concerns (database, external services)
- **Presentation Layer (WebAPI)**: API endpoints and HTTP concerns

### 2. Domain Events
Domain events are automatically dispatched when entities are saved to the database through an EF Core interceptor. Currently using mediator for convenience in this small project, but the implementation can be easily swapped for message brokers like **RabbitMQ**, **Kafka**, or **Azure Service Bus**.

### 3. Command Query Separation (CQS)
Commands and queries are separated for better organization.

### 4. Custom Mediator Implementation
Since the MediatR library changed its license, I implemented a custom mediator. For production applications, this would be replaced with proper message broker infrastructure. 

### 5. Authentication Strategy
Authentication is **not implemented** to facilitate manual testing. The `ICurrentUser` interface currently returns mocked values for logging purposes. In a production environment, this interface would be implemented to extract user information from JWT tokens or integrate with SSO providers (e.g., Keycloak, Azure AD) or something like that.

### 6. Testing Strategy

#### Unit Tests (Domain Layer)
Only the Domain layer has unit tests, covering entities, value objects, domain events, enums, and common base classes.

#### Functional Tests (Application Layer)
End-to-end functional tests validate use cases using an in-memory database. External services are mocked when appropriate, allowing tests to cover the entire application flow from request to response while validating business rules and integration between layers.
