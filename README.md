Devices API

A simple, clean and fully containerized .NET API designed to manage devices.
It follows a layered architecture (Domain → Application → Infrastructure → API) with clear separation of concerns, basic validation rules, error handling with Result<T>, EF Core migrations, Docker support, and automated tests.

Architecture Overview

Domain Layer

- Contains the core business model (Device aggregate root).
- Enforces business rules such as:
	Name and Brand are required.
- Devices in InUse state cannot have Name/Brand updated.
- Devices in InUse cannot be deleted.
- Creation time cannot be updated.


Application Layer

- Contains all use cases.
- Each action returns a Result<T> (success/failure with typed payload or error message).
- No framework dependencies.


Infrastructure Layer

- EF Core implementation of the repository.
- PostgreSQL database provider.
- Database migrations.


API Layer

- Minimal API endpoints (/api/devices).
- Global exception handling and HTTP result mapping.
- Dockerized runtime environment.


Unit Tests (xUnit + Moq + FluentAssertions)

- Covers the core use cases and domain validations.


Running the Project With Docker

This project includes a complete Docker setup with:

- devices.api
- PostgreSQL
- Built-in EF Core migrations automatically applied at startup

Requirements

- Docker Desktop (Linux container mode)

Steps

- From the project root:
	docker compose up --build
- Once the containers start:
	API will run at → http://localhost:7080
	PostgreSQL will run at → localhost:5432
	Database name → devices
	User → dbuser
	Password → pwd123

The API logs will show EF Core applying migrations on startup for a fresh database.

To shut everything down:

- docker compose down

If you want a clean run:

- docker compose down -v
- docker compose up --build


API Endpoints

POST /api/devices
Creates a new device.
Body:
{
  "name": "Laptop",
  "brand": "Dell",
  "state": "Available"
}

GET /api/devices
Returns all devices.

GET /api/devices/{id}
Returns a single device.

GET /api/devices/by-brand
Returns all devices based on brand.

GET /api/devices/by-state
Returns all devices based on state.

PUT /api/devices/{id}
Updates a device (fully).

Notes:
A device in InUse cannot have Name or Brand changed.

PATCH /api/devices/{id}
Updates parts of a device.

Notes:
A device in InUse cannot have Name or Brand changed.

DELETE /api/devices/{id}
Deletes a device.

Notes:
A device in InUse cannot be deleted.


Running Tests

Tests are located in the Devices.UnitTests project and cover:

- Domain rules
- Use cases
- Edge cases (invalid updates, invalid transitions, deletion rules)

Run them with:

dotnet test


Database Migrations

Migrations run automatically when the API container starts.
If you want to generate new migrations locally:
- dotnet ef migrations add <MigrationName> -p Devices.Infrastructure -s Devices.Api
- dotnet ef database update -p Devices.Infrastructure -s Devices.Api


What This Project Demonstrates

- Clean architecture with clear boundaries
- Domain-driven modelling of the Device aggregate
- Validation through the domain model (not controllers)
- Minimal APIs with clean HTTP result mapping
- Docker-first development
- Automated migrations
- Unit testing of all use cases
- Repository pattern with EF Core
- Friendly developer experience for running locally


Tech Stack

.NET 9
EF Core 9
PostgreSQL 17
Docker & Docker Compose
xUnit
FluentAssertions
Moq

Final Notes

The goal was to build something simple, reliable, and easy to run while following good engineering practices.
If you clone the repository and run docker compose up, everything should work end-to-end without manual steps.

If you need extra documentation, diagrams, or want to extend this into a production-ready service, feel free to reach out.
