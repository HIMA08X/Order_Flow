# OrderFlow

A small e-commerce backend project built with **.NET 8**.

OrderFlow is an educational backend designed to show how a simple order
system can evolve beyond basic CRUD into a cleaner architecture using
**Clean Architecture, CQRS, MediatR, Redis caching, a read model, and
background processing**.

## Features

-   Create orders with one or more items
-   Validate order and item data
-   Calculate order totals
-   Get order details by ID
-   List orders
-   Dashboard read model for optimized reads
-   Redis caching for frequently requested order details
-   Background processing for pending orders
-   Periodic refresh of the dashboard read model
-   Swagger / OpenAPI documentation

## Architecture

``` text
Order_Flow
|
|-- Order_Flow.Domain
|-- Order_Flow.App
|-- Order_Flow.Infrastructure
`-- Order_Flow.API
```

### Domain

Contains the core business entities and rules:

``` text
Order_Flow.Domain
`-- Entities
    |-- Order.cs
    |-- Order_Item.cs
    `-- Customer.cs
```

The Domain layer does not depend on SQL Server, Redis, the API, or other
infrastructure concerns.

### Application

Contains the use cases and application logic:

``` text
Order_Flow.App
`-- Orders
    |-- Commands
    |   `-- Create_Order
    |-- Queries
    |   |-- Get_Order_By_Id
    |   |-- Get_Orders
    |   `-- Get_Dashboard_Orders
    `-- Interfaces
```

The project uses **CQRS**: Commands change application state, while
Queries read data. **MediatR** is used to dispatch Commands and Queries
to their handlers.

### Infrastructure

Contains implementations for external concerns such as:

-   Entity Framework Core
-   SQL Server
-   Repository implementations
-   Redis caching
-   Dashboard read model
-   Background processing

### API

Contains the HTTP layer. Controllers stay thin and send Commands and
Queries through MediatR instead of containing the business logic
themselves.

## Request Flow

### Create Order

``` text
Client
  |
  v
POST /api/orders
  |
  v
Create_Order_Command
  |
  v
MediatR
  |
  v
Create_Order_Command_Handler
  |
  v
IOrderRepository
  |
  v
OrderRepository
  |
  v
EF Core
  |
  v
SQL Server
```

### Get Order Details

``` text
Client
  |
  v
GET /api/orders/{id}
  |
  v
Query -> MediatR -> Handler
  |
  v
Redis Cache
  |
  +-- Cache Hit -> Return data
  |
  `-- Cache Miss
          |
          v
      SQL Server
          |
          v
      Store in Redis
          |
          v
        Return
```

## CQRS

### Commands

Commands are responsible for changing application state.

Example:

``` text
Create_Order_Command
```

### Queries

Queries are responsible for reading data.

Examples:

``` text
Get_Order_By_Id_Query
Get_Orders_Query
Get_Dashboard_Orders_Query
```

Separating reads from writes makes the application easier to organize
and allows read operations to be optimized independently.

## Database

The transactional data is stored in **SQL Server** using **Entity
Framework Core**.

Main entities:

-   Customers
-   Orders
-   Order Items

SQL Server is the source of truth for the transactional data.

## Redis Caching

Redis is used for frequently requested order details.

Example cache key:

``` text
order:1
```

Cached data has an expiration time, and relevant cached order details
are invalidated when the order changes.

## Dashboard Read Model

The dashboard uses a separate read-optimized model:

``` text
OrderDashboardReadModels
```

It contains information such as:

-   Order ID
-   Customer name
-   Item count
-   Total
-   Status
-   Last update time

This model is a projection of the transactional data, not the source of
truth.

## Background Processing

A `BackgroundService` periodically:

1.  Finds pending orders.
2.  Changes their status to `Completed`.
3.  Invalidates affected order caches.
4.  Refreshes the dashboard read model.
5.  Waits before the next cycle.

The background service creates a dependency scope so scoped services
such as repositories and `DbContext` can be used safely.

## API Endpoints

  Method   Endpoint                  Purpose
  -------- ------------------------- --------------------
  POST     `/api/orders`             Create an order
  GET      `/api/orders/{id}`        Get order details
  GET      `/api/orders`             List orders
  GET      `/api/dashboard/orders`   Get dashboard data

## Example Request

### Create Order

``` http
POST /api/orders
Content-Type: application/json
```

``` json
{
  "customerId": 1,
  "items": [
    {
      "productName": "Laptop",
      "quantity": 2,
      "unitPrice": 30000
    },
    {
      "productName": "Mouse",
      "quantity": 1,
      "unitPrice": 500
    }
  ]
}
```

The total for this example is:

``` text
2 x 30000 + 1 x 500 = 60500
```

## Technologies

-   C#
-   .NET 8
-   ASP.NET Core Web API
-   Entity Framework Core
-   SQL Server
-   Redis
-   MediatR
-   Swagger / OpenAPI
-   REST API
-   Clean Architecture
-   CQRS
-   Repository Pattern
-   BackgroundService

## Getting Started

### 1. Clone the repository

``` bash
git clone https://github.com/YOUR_USERNAME/OrderFlow.git
cd OrderFlow
```

### 2. Configure SQL Server and Redis

Update `Order_Flow.API/appsettings.json`:

``` json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=OrderFlowDb;Trusted_Connection=True;TrustServerCertificate=True;",
  "Redis": "localhost:6379"
}
```

Use your own SQL Server instance if necessary.

### 3. Start Redis

Redis should be available on:

``` text
localhost:6379
```

For Docker:

``` bash
docker run --name orderflow-redis -p 6379:6379 -d redis
```

### 4. Create the database

From Visual Studio Package Manager Console:

``` powershell
Add-Migration InitialCreate -Project Order_Flow.Infrastructure -StartupProject Order_Flow.API
```

Then:

``` powershell
Update-Database -Project Order_Flow.Infrastructure -StartupProject Order_Flow.API
```

### 5. Run the API

Run `Order_Flow.API` and open Swagger to test the endpoints.

## Project Structure

``` text
Order_Flow
|
|-- Order_Flow.Domain
|   `-- Entities
|       |-- Order.cs
|       |-- Order_Item.cs
|       `-- Customer.cs
|
|-- Order_Flow.App
|   `-- Orders
|       |-- Commands
|       |   `-- Create_Order
|       |-- Queries
|       |   |-- Get_Order_By_Id
|       |   |-- Get_Orders
|       |   `-- Get_Dashboard_Orders
|       `-- Interfaces
|
|-- Order_Flow.Infrastructure
|   |-- Data
|   |-- Repositories
|   |-- Caching
|   |-- ReadModels
|   `-- Background
|
`-- Order_Flow.API
    `-- Controllers
```

## Why I Built This Project

I built OrderFlow as a practical way to understand backend architecture
beyond simple CRUD.

The main goal was to understand how the different parts of a backend fit
together:

-   How a request reaches a Handler
-   Why Commands and Queries are separated
-   How Domain entities hold business rules
-   Why Application depends on abstractions instead of infrastructure
    details
-   How EF Core communicates with SQL Server
-   Why Redis can reduce repeated database reads
-   Why a read model can make dashboard queries more efficient
-   Why background work should not be tied to an HTTP request

The project also helped me understand the trade-off between keeping an
application simple and introducing more advanced patterns when they
actually solve a problem.

## Scope

This is an educational backend project. It intentionally does not
include:

-   Authentication and authorization
-   Payment gateways
-   Microservices
-   Kafka or RabbitMQ
-   Kubernetes
-   Event sourcing
-   Sharding
-   Complex inventory management

The focus is on understanding backend architecture and the technologies
used in the project.

## Author

**Ibrahim**

Computer Science Student \| Backend .NET Developer

Focused on building backend applications with **C#, ASP.NET Core, SQL
Server, Entity Framework Core, REST APIs, and clean backend
architecture**.
