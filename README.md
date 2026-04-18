# HairSync - Professional Management API ✂️🗓️

**HairSync** is a robust, scalable backend solution designed for professional service management, specifically tailored for hair salons and independent stylists. Built with **.NET Core** and following **Clean Architecture** principles, the system focuses on high-performance appointment scheduling, user management, and secure data handling.

## 🚀 Key Features
- **Appointment Lifecycle:** Full CRUD operations for scheduling, updating, and managing services.
- **Secure Authentication:** JWT-based identity management with Role-Based Access Control (RBAC).
- **Domain-Driven Design (DDD):** Business logic isolated from infrastructure to ensure maintainability.
- **Data Integrity:** Transactional operations using Entity Framework Core and SQL Server.
- **API Documentation:** Fully documented interactive endpoints via Swagger/OpenAPI.

## 🏗️ Architecture & Patterns
This project implements **Clean Architecture** to ensure a clear separation of concerns:

- **Domain:** Contains core entities, interfaces, and domain-specific logic. Zero external dependencies.
- **Application:** Handles Use Cases, DTOs, and mapping logic. 
- **Infrastructure:** Implements data persistence (EF Core), external services, and repository patterns.
- **WebAPI:** The entry point. Handles HTTP requests, Middleware, and Dependency Injection configuration.

### Design Principles Applied:
- **SOLID Principles:** Ensuring decoupled and testable code.
- **Repository Pattern:** Abstracting data access to keep the business logic clean.
- **Dependency Injection:** Native .NET DI for managing service lifecycles.

## 🛡️ Security Mindset
- **Authentication:** Stateless JWT tokens for secure client-server communication.
- **Authorization:** Granular permissions (Admin/Employee/Client) to enforce the Principle of Least Privilege.
- **Input Validation:** Strict validation schemas to prevent common vulnerabilities like SQL Injection.

## 🛠️ Tech Stack
- **Framework:** .NET Core (C#)
- **ORM:** Entity Framework Core
- **Database:** SQL Server
- **Documentation:** Swagger UI
- **Security:** Microsoft.AspNetCore.Authentication.JwtBearer

## ⚙️ Getting Started

1. **Clone the repository:**
   ```
   git clone [https://github.com/geromassera/HairSync-Backend.git](https://github.com/geromassera/HairSync-Backend.git)

2. **Update Connection String:**
Modify the appsettings.json in the HairSync.API project with your local SQL Server instance.

3. **Apply Migrations:**
 ```dotnet ef database update --project HairSync.Infrastructure --startup-project HairSync.API```

4. **Run the API**
  ```dotnet run --project HairSync.API``` 

## 👥 Collaborators
This project was developed as part of a degree project at **Universidad Tecnológica Nacional (UTN)** by:
- [Gerónimo Massera](https://github.com/geromassera) - Full-Stack
- [Nicolas Boscia](https://github.com/NicoBoscia) - Full-Stack
- [Nicolas Admet](https://github.com/nicoadmet) - Full-Stack

---
*Special thanks to our tutors at UTN for their guidance during the development of this project.*
