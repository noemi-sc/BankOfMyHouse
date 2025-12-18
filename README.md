# BankOfMyHouse

A full-stack personal banking and investment portfolio management system built with Angular 20 and ASP.NET Core 9.

## Features

- **User Authentication** - JWT-based authentication with access/refresh tokens
- **Bank Account Management** - Create accounts with auto-generated Italian IBANs
- **Transaction Processing** - Internal and external transfers with multi-currency support
- **Investment Portfolio** - Buy shares, track investments, view portfolio performance
- **Real-time Stock Prices** - Live price updates via SignalR WebSocket
- **Historical Charts** - Visualize stock price trends with Chart.js

## Tech Stack

### Frontend
- **Framework**: Angular 20
- **Language**: TypeScript 5.8
- **UI**: Angular Material (rose-red theme)
- **State**: Angular Signals
- **Charts**: Chart.js with ng2-charts
- **Real-time**: SignalR client
- **Package Manager**: pnpm

### Backend
- **Framework**: ASP.NET Core 9
- **Language**: C# 13
- **API**: FastEndpoints
- **ORM**: Entity Framework Core
- **Database**: PostgreSQL 17
- **Real-time**: SignalR
- **Mapping**: Mapster

## Project Structure

```
BankOfMyHouse/
├── BankOfMyHouse.Client/           # Angular 20 frontend
│   └── src/app/
│       ├── auth/                   # Login, register, guards
│       ├── bankAccounts/           # Account management
│       ├── transactions/           # Transaction CRUD
│       ├── investments/            # Investment creation
│       ├── dashboard/              # Main UI components
│       └── services/               # API services
│
├── BankOfMyHouse.Server/           # .NET 9 backend
│   ├── BankOfMyHouse.API/          # Endpoints, DTOs, validators
│   ├── BankOfMyHouse.Application/  # Services, SignalR hub
│   ├── BankOfMyHouse.Domain/       # Entities, value objects
│   └── BankOfMyHouse.Infrastructure/ # EF Core, repositories
│
├── docker-compose.yml              # Container orchestration
└── BankOfMyHouse.sln               # Visual Studio solution
```

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Node.js 20+](https://nodejs.org/)
- [pnpm](https://pnpm.io/) (`npm install -g pnpm`)
- [PostgreSQL 17](https://www.postgresql.org/download/) or [Docker](https://www.docker.com/)

## Getting Started

### Option 1: Using Docker (Recommended)

The easiest way to start the project is using Docker Compose, which sets up PostgreSQL automatically.

```bash
# Clone the repository
git clone https://github.com/yourusername/BankOfMyHouse.git
cd BankOfMyHouse

# Start PostgreSQL and API containers
docker-compose up -d

# Install frontend dependencies
cd BankOfMyHouse.Client
pnpm install

# Start the Angular dev server
pnpm start
```

The application will be available at:
- **Frontend**: http://localhost:4200
- **API**: http://localhost:57460
- **API Docs**: http://localhost:57460/scalar

### Option 2: Manual Setup

#### 1. Database Setup

Install PostgreSQL 17 and create a database:

```sql
CREATE DATABASE bankOfMyHouse;
```

Update the connection string in `BankOfMyHouse.Server/BankOfMyHouse.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=bankOfMyHouse;Username=postgres;Password=yourpassword"
  }
}
```

#### 2. Backend Setup

```bash
# Navigate to the server folder
cd BankOfMyHouse.Server

# Restore NuGet packages
dotnet restore

# Apply database migrations
dotnet ef database update --project BankOfMyHouse.Infrastructure --startup-project BankOfMyHouse.API

# Run the API
dotnet run --project BankOfMyHouse.API
```

The API will start at `http://localhost:57460`.

#### 3. Frontend Setup

```bash
# Navigate to the client folder
cd BankOfMyHouse.Client

# Install dependencies
pnpm install

# Start the development server
pnpm start
```

The frontend will start at `http://localhost:4200`.

## Default Test Users

The database is automatically seeded with test users:

| Username | Email | Password |
|----------|-------|----------|
| username1 | user1@example.com | Password123! |
| username2 | user2@example.com | Password123! |
| username3 | user3@example.com | Password123! |

## API Endpoints

### Authentication
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/users/auth/login` | Login and get JWT tokens |
| POST | `/users/auth/register` | Register new user |
| POST | `/users/auth/refresh` | Refresh access token |
| POST | `/users/auth/logout` | Logout |
| GET | `/users/details` | Get user details |
| GET | `/users/current` | Get current user |

### Bank Accounts
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/accounts/create` | Create bank account |
| GET | `/accounts` | Get user's accounts |

### Transactions
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/transactions/create` | Create transaction |
| GET | `/transactions` | Get transactions (with filters) |

### Investments
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/investments/create` | Create investment |
| GET | `/investments` | Get user's investments |
| GET | `/investments/companies` | List available companies |
| GET | `/investments/historical-prices` | Get historical stock prices |

### Reference Data
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/currencies` | Get available currencies |
| GET | `/payment-categories` | Get payment categories |

## Development

### Running Tests

**Frontend:**
```bash
cd BankOfMyHouse.Client
pnpm test
```

### Building for Production

**Frontend:**
```bash
cd BankOfMyHouse.Client
pnpm build
```

**Backend:**
```bash
cd BankOfMyHouse.Server
dotnet publish -c Release
```

### Database Migrations

```bash
# Add a new migration
dotnet ef migrations add MigrationName --project BankOfMyHouse.Infrastructure --startup-project BankOfMyHouse.API

# Apply migrations
dotnet ef database update --project BankOfMyHouse.Infrastructure --startup-project BankOfMyHouse.API
```

## Configuration

### Backend Configuration (`appsettings.json`)

```json
{
  "JwtSettings": {
    "Issuer": "BankOfMyHouse",
    "Audience": "BankOfMyHouseUsers",
    "Secret": "your-secret-key-min-32-characters",
    "ExpirationInMinutes": 60,
    "RefreshTokenExpirationInDays": 7
  },
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=bankOfMyHouse;Username=postgres;Password=yourpassword"
  }
}
```

### Frontend Configuration

API base URL is configured in individual services. Default: `http://localhost:57460`

## Architecture

### Backend Layers

1. **API Layer** - FastEndpoints with request/response DTOs
2. **Application Layer** - Business logic, services, SignalR hub
3. **Domain Layer** - Entities, value objects, repository interfaces
4. **Infrastructure Layer** - EF Core, repository implementations

### Key Patterns

- **Repository Pattern** - Data access abstraction
- **Service Pattern** - Business logic encapsulation
- **Factory Pattern** - Entity creation via static methods
- **Value Objects** - Immutable domain primitives (IbanCode)

### Frontend Architecture

- **Standalone Components** - No NgModules
- **Signal-based State** - Reactive state management
- **OnPush Change Detection** - Performance optimization
- **Feature-based Structure** - Organized by domain

## Real-time Features

The application uses SignalR for real-time stock price updates:

- **Hub Endpoint**: `/personalInvestments`
- **Events**: `TransferChartData`, `TransferAllPrices`
- **Update Frequency**: Every second

## License

MIT License

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request
