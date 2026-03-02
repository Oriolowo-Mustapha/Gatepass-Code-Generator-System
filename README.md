# Gatepass Code Generator System

A secure, role-based **Gatepass Management REST API** built with **.NET 10** and **Clean Architecture**. The system generates unique gate passes with QR codes, manages visitor check-in/check-out, and provides reporting and audit capabilities.

## Features

- **Authentication & Authorization** — JWT-based auth with refresh tokens, role-based access (`Administrator`, `Security`), forgot/reset password via email.
- **Gatepass Requests** — Create and manage gatepass requests for Visitors, Employees, Contractors, Vehicles, Materials, and more.
- **QR Code Generation** — Each approved gatepass receives a unique code and QR code image for verification at access points.
- **Security Operations** — Verify gatepasses, check in visitors, and check out visitors with full tracking.
- **Organization Management** — Create and manage departments and access points.
- **Reporting** — Daily visitor logs, gatepass statistics, and overstay reports.
- **Admin Panel** — System settings management and audit log viewing.
- **Notifications** — In-app notification system with read/unread tracking.
- **Email Service** — SMTP email support (Brevo/Sendinblue) for password resets and notifications.

## Tech Stack

| Layer | Technology |
|---|---|
| Runtime | .NET 10 |
| API | ASP.NET Core Web API |
| Database | PostgreSQL (via Npgsql) |
| ORM | Entity Framework Core 10 |
| Auth | JWT Bearer Tokens |
| Mediator | MediatR 14 |
| Validation | FluentValidation |
| QR Codes | QRCoder |
| Password Hashing | BCrypt.Net |
| Email | MailKit |
| API Docs | Swagger / Swashbuckle |

## Architecture

The solution follows **Clean Architecture** principles with four projects:

```
Gatepass Code Generator System/   # Web API (Presentation)
├── Controllers/
├── Program.cs
│
Application/                      # Use Cases & Interfaces
├── Features/                     # CQRS commands & queries (MediatR)
│   ├── Auth/
│   ├── GatepassRequests/
│   ├── Security/
│   ├── Organization/
│   ├── Admin/
│   ├── Reports/
│   └── Notifications/
├── DTOs/
├── Interfaces/
├── Behaviors/                    # MediatR pipeline (validation)
└── Exceptions/
│
Domain/                           # Entities & Enums
├── Entities/
│   ├── User, Role
│   ├── Gatepass, GatepassRequest
│   ├── Visitor, VehicleDetails
│   ├── CheckInOut, AccessPoint
│   ├── Department, Notification
│   ├── AuditLog, SystemConfiguration
│   └── BaseEntity
└── Enum/
    ├── GatepassType
    └── ApprovalStatus
│
Infrastructure/                   # Data Access & External Services
├── Context/                      # EF Core DbContext & Data Seeder
├── Repositories/                 # Generic + Specialized repos, UoW
├── Services/                     # Token, Email, QR, Password, etc.
├── Middleware/                   # Global exception handling
└── Migrations/
```

## API Endpoints

### Auth (`/api/auth`)
| Method | Endpoint | Description |
|---|---|---|
| POST | `/register` | Register a new user |
| POST | `/login` | Login and receive JWT + refresh token |
| POST | `/refresh-token` | Refresh an expired access token |
| POST | `/forgot-password` | Request a password reset email |
| POST | `/reset-password` | Reset password with token |

### Gatepass Requests (`/api/gatepassrequests`) — *Requires Auth*
| Method | Endpoint | Description |
|---|---|---|
| POST | `/` | Create a new gatepass request |

### Security (`/api/security`) — *Role: Security*
| Method | Endpoint | Description |
|---|---|---|
| POST | `/verify` | Verify a gatepass by unique code |
| POST | `/checkin` | Check in a visitor |
| POST | `/checkout` | Check out a visitor |

### Organization (`/api/organization`) — *Role: Administrator*
| Method | Endpoint | Description |
|---|---|---|
| GET | `/departments` | List all departments |
| POST | `/departments` | Create a department |
| GET | `/accesspoints` | List all access points |
| POST | `/accesspoints` | Create an access point |

### Admin (`/api/admin`) — *Role: Administrator*
| Method | Endpoint | Description |
|---|---|---|
| GET | `/settings` | Get system settings |
| PUT | `/settings` | Update a system setting |
| GET | `/auditlogs` | Get audit logs (optional date filters) |

### Reports (`/api/reports`) — *Requires Auth*
| Method | Endpoint | Description |
|---|---|---|
| GET | `/daily-log?date=` | Get daily visitor log |
| GET | `/statistics` | Get gatepass statistics |
| GET | `/overstays` | Get overstay report |

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL](https://www.postgresql.org/download/)

### Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/Oriolowo-Mustapha/Gatepass-Code-Generator-System.git
   cd "Gatepass Code Generation System"
   ```

2. **Configure the database**

   Update the connection string in `Gatepass Code Generator System/appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Host=localhost;Port=5432;Database=GatepassDb;Username=<your_user>;Password=<your_password>"
   }
   ```

3. **Configure JWT settings**

   Update the JWT section in `appsettings.json` with your own secret key:
   ```json
   "Jwt": {
     "Key": "<your-secret-key-at-least-32-characters>",
     "Issuer": "GatepassCodeGeneratorSystem",
     "Audience": "GatepassCodeGeneratorSystem",
     "ExpiryInMinutes": "60"
   }
   ```

4. **Configure email (optional)**

   Update the `EmailSettings` section in `appsettings.json` with your SMTP provider credentials.

5. **Apply migrations and run**
   ```bash
   cd "Gatepass Code Generator System"
   dotnet ef database update --project ../Infrastructure
   dotnet run
   ```

6. **Access the API**

   Swagger UI is available in development at: `https://localhost:<port>/swagger`

### Default Admin Account

The application seeds a default administrator account on first run. Update the `AdminUser` section in `appsettings.json` to configure credentials before startup.

## License

This project is for educational and demonstration purposes.
