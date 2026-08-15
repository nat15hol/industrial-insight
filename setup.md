---
name: Setup
description: Local development and setup instructions for Industrial Insight.
---

# Setup

## Prerequisites

- .NET SDK (version matching `server/server.csproj`'s target framework — currently .NET 10)
- Node.js and npm (for the `client/` Vite + React app)
- SQL Server (local instance, e.g. SQL Server Developer Edition or LocalDB)
- Visual Studio (recommended for `server/`) and/or VS Code (recommended for `client/`)

## 1. Clone the repository

```bash
git clone <repository-url>
cd industrial-insight
```

## 2. Backend setup (`server/`)

```bash
cd server
dotnet restore
```

### Configure the database connection

Set the SQL Server connection string via **.NET User Secrets** (not hardcoded, not committed):

```bash
cd server
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<your-connection-string>"
```

If the optional AI-assisted incident assessment is enabled with an external provider, its API key is also configured via User Secrets (or environment variables if deployed) — never hardcoded in source. If using the local mock AI implementation, no key is required.

### Run migrations

From `server/`:

```bash
dotnet ef migrations add InitialCreate   # first time only
dotnet ef database update
```

This creates the schema described in [database.md](database.md) and [erd.md](erd.md).

### Seed data

Seed data for `Roles` and at least one `Manager` account is applied automatically on startup / via a seed script (implementation detail — update this section once finalized). Self-registration only ever creates `Technician` accounts.

### Run the backend

```bash
cd server
dotnet run
```

The API should now be available (default ASP.NET Core dev port, typically `https://localhost:5001` or similar — confirm against `launchSettings.json`).

Swagger/OpenAPI UI is available at `/swagger` for manual endpoint testing, matching the endpoints documented in [api-contract.md](api-contract.md).

## 3. Frontend setup (`client/`)

```bash
cd client
npm install
npm run dev
```

The frontend expects the backend API base URL to be configured (e.g. via a `.env` file / Vite env variable) — update this section with the exact variable name once implemented.

## 4. Verify a clean setup

To confirm the system starts cleanly from scratch (required for Checkpoint 1 and Checkpoint 3):

1. Drop/recreate the local database (or point to a fresh one).
2. Run `dotnet ef database update`.
3. Confirm seed data loads (at least one Manager account exists).
4. Start the backend; confirm it starts without errors.
5. Start the frontend; confirm it loads and can reach the backend.
6. Register a new (Technician) account, log in, and confirm a protected page loads.

## 5. Loading demo/telemetry data

Sample CSV files for the data pipeline are in `data/telemetry/` (`sample-valid.csv`, `sample-invalid.csv`, `sample-duplicates.csv`). Trigger ingestion according to however the pipeline is invoked (manual command, endpoint, or startup routine — update this section once finalized) to populate `TelemetryRecords` and produce `PipelineRun` records.

## Troubleshooting

- **Migrations fail to apply** — confirm the connection string in User Secrets is correct and SQL Server is running and reachable.
- **Frontend cannot reach backend** — confirm the API base URL configured in the frontend matches the backend's actual running port, and that CORS is configured to allow the frontend's origin.
- **AI suggestions never appear** — expected if the AI feature is not implemented or not configured; the incident workflow remains fully functional without it (see [architecture.md](architecture.md)).

For running the test suite, see [testing.md](testing.md).
