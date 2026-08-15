# Industrial Insight

**A maintenance prioritization platform for industrial operations**

Industrial Insight helps maintenance teams identify which machines need attention first — and understand why. It combines machine telemetry, technician incident reports, and maintenance history into a single prioritization workflow, so managers can act on the most urgent problems instead of digging through scattered data sources.

The system is designed to sit on top of existing operational data and workflows, adding a transparent intelligence layer rather than replacing existing tools.

---

## Overview

Companies operating many machines often have information scattered across different sources, making it difficult to quickly answer questions such as:

- Which machines need attention first, and why?
- Which problems are recurring?
- How quickly are incidents resolved?
- Is incoming telemetry data complete and valid?

Industrial Insight answers these questions through one continuous workflow:

**Machine telemetry + incident reports → data quality validation → priority scoring → AI-assisted assessment → maintenance action → resolution & analytics**

Each machine receives a transparent, explainable Priority Score based on open incidents, severity, and recurrence — so the team always knows not just *what* needs attention, but *why*.

**Target users:** small and mid-sized manufacturers operating multiple industrial machines, where maintenance information currently lives in spreadsheets, informal notes, or disconnected systems.

Industrial Insight is not designed to replace an existing maintenance or CMMS system. It is designed to sit on top of existing data and workflows as a prioritization layer, helping teams decide what to work on first without requiring a full system migration.

The AI-assisted incident assessment is an optional decision-support layer and is **not required for the core prioritization workflow**.

---

## Key Features

### Authentication & Role-Based Access Control

- JWT-based authentication
- User registration and login
- Technician and Manager roles
- Server-side authorization
- New self-registered users are created as Technicians
- Manager accounts are seeded or otherwise controlled

### Machine Management

- View machines
- View machine details
- Machine status and runtime
- Machine location
- Machine-related operational information

### Incident Reporting

Technicians can report incidents associated with machines.

Managers can:

- View incidents
- Prioritize incidents
- Manage incident status
- Follow up on operational issues

Incident reporting and management work independently of the optional AI functionality.

### Priority Score

Each machine is assigned a transparent Priority Score based on:

- number of open incidents
- incident severity
- unresolved issue count
- recurrence of similar problems

Example:

```text
Machine A — Priority 82/100 — HIGH
3 unresolved incidents · recurring issue
Recommended action: Inspect bearing assembly
[Create maintenance task]
```

The score is calculated using explicit, explainable rules rather than a predictive model — the goal is maintenance **prioritization**, not predictive maintenance. This keeps the scoring transparent and lets the team understand exactly why a machine is flagged.

### AI-Assisted Incident Assessment

AI-assisted assessment is an optional **P1 enhancement** that feeds into the core prioritization workflow — it is not a standalone feature.

When available, it analyzes an incident description and provides structured suggestions for:

- Category
- Priority
- Recommended action
- A short, description-based rationale (e.g. "description indicates abnormal vibration pattern")

These suggestions feed directly into the machine's Priority Score. The Technician reviews and can accept or edit the suggestion before it is saved.

AI output is treated as **untrusted external input** and is validated against a defined C# validation schema before being used by the application.

The AI service must never be a dependency for the core workflow. If it is unavailable, disabled, or returns an invalid response, the incident can still be created and prioritized manually.

The AI functionality can be implemented using:

- A local mock AI service for development and demonstration
- A local open-source model, if available
- An optional external AI provider such as OpenAI or Anthropic

**No paid AI service is required to run or demonstrate the core application.**

Note: the AI rationale is based on the incident description text only. It does not compare against historical incidents unless that capability is explicitly implemented and documented.

## Maintenance Tasks

Managers can create and assign maintenance tasks from incidents.

Technicians can:

- View their assigned tasks
- Update task status
- Complete assigned tasks

## Data Pipeline

The project includes a C# data ingestion pipeline, built into the backend, for synthetic machine telemetry.

The pipeline performs:

1. CSV ingestion
2. Schema validation
3. Required-field validation
4. Data-type validation
5. Timestamp validation
6. Measurement-range validation
7. Machine-reference validation
8. Duplicate detection
9. Transformation
10. SQL Server loading
11. Pipeline execution tracking

The pipeline distinguishes between:

- Accepted records
- Rejected records
- Duplicate records

## Analytics Dashboard

The dashboard provides operational KPIs based on persisted database data, including the per-machine Priority Score described above.

Planned/implemented metrics include:

- Total machines
- Open incidents
- Problematic machines (the underlying rule behind each machine's Priority Score)
- Average incident resolution time
- Latest pipeline run
- Pipeline data-quality percentage

Dashboard data is based on real database values rather than hardcoded demonstration values.

## Architecture

At a high level, Industrial Insight consists of:

```text
                    ┌─────────────────────┐
                    │      React UI       │
                    │ TypeScript + Vite   │
                    └──────────┬──────────┘
                               │
                               │ REST API
                               ▼
                    ┌───────────────────────┐
                    │ ASP.NET Core Backend  │
                    │                       │
                    │ Auth / RBAC           │
                    │ Incidents             │
                    │ Machines              │
                    │ Maintenance           │
                    │ Dashboard             │
                    │ CSV Ingestion         │
                    │ Optional AI           │
                    └───────┬─────┬─────────┘
                            │     │
                 ┌──────────┘     └──────────────┐
                 ▼                               ▼
        ┌─────────────────┐             ┌─────────────────┐
        │   SQL Server    │             │ Optional AI     │
        │  (via EF Core)  │             │ Provider        │
        │                 │             │                 │
        │ Users           │             │ Structured      │
        │ Machines        │             │ incident        │
        │ Incidents       │             │ suggestions     │
        │ Tasks           │             └─────────────────┘
        │ Telemetry       │
        │ PipelineRuns    │
        └─────────────────┘

```

For the detailed architecture, see:

- [Architecture](Architecture.md)
- [Database](DATABASE.md)
- [API Contract](API_Contract.md)
- [Dataset Specification](Dataset_Specification.md)

## Technology Stack

### Backend

- C#
- ASP.NET Core Web API
- Entity Framework Core
- ASP.NET Identity
- JWT authentication
- REST API

### Frontend

- React
- TypeScript
- Vite
- Tailwind CSS
- React Router
- Recharts, where applicable

### Data Engineering

- C# (CSV ingestion, embedded in backend)
- CsvHelper
- Synthetic CSV telemetry data
- Data validation
- Duplicate detection
- Data transformation

### Database

- SQL Server

### Optional AI

The AI-assisted incident assessment is an optional P1 feature.

The application is designed so that the AI provider can be replaced without changing the core incident workflow.

Possible implementations include:

- Local mock AI service
- Local open-source AI model
- Optional external AI provider such as Anthropic or OpenAI

All AI responses are validated through C# validation before being used by the application.

A paid external AI API is **not required** for the core project.

### Development Tools

- GitHub
- Git
- Swagger / OpenAPI
- GitHub Projects / Scrum board
- xUnit

## Data Pipeline

The telemetry pipeline processes synthetic machine data using the following flow:

```text
CSV
 │
 ▼
Ingestion
 │
 ▼
Schema Validation
 │
 ▼
Required Fields / Data Types
 │
 ▼
Timestamp Validation
 │
 ▼
Range Validation
 │
 ▼
Machine Validation
 │
 ▼
Duplicate Detection
 │
 ├── Rejected
 ├── Duplicate
 │
 ▼
Accepted Records
 │
 ▼
Transformation
 │
 ▼
SQL Server
 │
 ▼
PipelineRun
 │
 ▼
REST API
 │
 ▼
Dashboard

```

The synthetic dataset intentionally contains both valid and problematic records so that the system can demonstrate data-quality handling.

The primary measurements are:

| **Measurement** | **Unit** |
| --------------- | -------- |
| Temperature     | °C       |
| Pressure        | bar      |
| Vibration       | mm/s     |
| Energy          | kWh      |

For the complete dataset definition and validation rules, see:

[Dataset Specification](Dataset_Specification.md)

## Data Quality

The pipeline calculates a data-quality percentage using:

```text
Data Quality % =
Accepted Records / Processed Records × 100

```

For example:

```text
Processed: 100
Accepted: 90
Rejected: 7
Duplicates: 3

Data Quality: 90%

```

Each pipeline execution is recorded in the `PipelineRuns` table with information such as:

- Processing start/end time
- Records processed
- Records accepted
- Records rejected
- Duplicate count
- Data-quality percentage
- Pipeline status

## Database

The main database entities are:

- `Users`
- `Roles`
- `Locations`
- `Machines`
- `TelemetryRecords`
- `Incidents`
- `MaintenanceTasks`
- `PipelineRuns`

The main relationships include:

```text
Role
  └── Users

Location
  └── Machines
        ├── TelemetryRecords
        ├── Incidents
        │     └── MaintenanceTasks
        └── ...

Users
  ├── Incidents
  └── MaintenanceTasks

```

`PipelineRuns` is used to track pipeline executions and data-quality results.

For the complete database design and schema, see:

- [Database Documentation](DATABASE.md)
- [ERD](ERD.md)

## User Roles

### Technician

Technicians can:

- View relevant machines and machine history
- Report incidents
- View their assigned maintenance tasks
- Update their own assigned task status

### Manager

Managers can:

- View machines and incidents
- Prioritize and manage incidents
- Create and assign maintenance tasks
- View operational dashboards and KPIs
- View pipeline runs and data-quality results

Authorization is enforced server-side.

A Technician must not be able to access Manager-only functionality or modify maintenance tasks assigned to another user.

## Core Workflows

### Operational Workflow

```text
Login
  ↓
RBAC
  ↓
Machines
  ↓
Incident Creation
  ↓
Manager Incident Handling
  ↓
Maintenance Task
  ↓
Task Completion

```

### Optional AI-Assisted Incident Workflow

```text
Technician reports incident
          ↓
     AI analysis
          ↓
Category / Priority /
Recommended Action
          ↓
Technician reviews/edits
          ↓
      Save incident

```

If AI is unavailable:

```text
Technician reports incident
          ↓
Manual category / priority /
action selection
          ↓
      Save incident

```

The manual workflow is always available.

### Data Workflow

```text
CSV
 ↓
Validation
 ↓
Transformation
 ↓
SQL Server
 ↓
PipelineRun
 ↓
API
 ↓
Dashboard

```

## Dashboard KPIs

The dashboard is based on defined business rules.

### Total machines

Number of machines stored in the database.

### Open incidents

Number of incidents that have not been resolved.

### Problematic machines

A machine is considered problematic if it has either:

- At least 2 open incidents, or
- At least 1 critical incident

during the selected reporting period.

The default reporting period is the most recent 7 days unless otherwise specified by the dashboard implementation.

### Average resolution time

Average time between:

```text
Incident.CreatedAt
        ↓
Incident.ResolvedAt

```

Only resolved incidents are included.

### Latest pipeline run

The most recent `PipelineRun`, including its status and processing statistics.

### Pipeline data quality

The data-quality percentage calculated during the pipeline execution.

## Getting Started

Detailed setup instructions are maintained separately in:

`SETUP.md`

The documented setup should allow a developer to start the project from a clean environment, including:

- Installing dependencies
- Configuring environment variables
- Starting SQL Server
- Running database migrations
- Preparing seed/demo data
- Starting the backend
- Starting the frontend
- Running the application

### Prerequisites

The project requires the development environment specified in `SETUP.md`.

Expected core dependencies include:

- .NET SDK
- Node.js / npm
- SQL Server
- Git

**Recommended IDEs:** Visual Studio for the backend (`/backend`, C#/ASP.NET Core, EF Core migrations) and VS Code for the frontend (`/frontend`, React/TypeScript). Any editor with equivalent tooling can be used instead.

**No external AI API account or paid AI subscription is required for the core application.**

Additional requirements may apply only if an optional external AI provider is enabled.

## Environment Configuration

Environment-specific configuration and secrets should not be committed to the repository.

The backend uses standard ASP.NET Core configuration conventions:

- `appsettings.json` — non-sensitive default configuration, committed to the repository
- `appsettings.Development.json` — local overrides, excluded from source control via `.gitignore`
- **.NET User Secrets** (`dotnet user-secrets`) — used for local secrets such as the SQL Server connection string, JWT signing key, and any optional external AI provider API key, so nothing sensitive is stored in a committed file

The frontend uses a `.env.example` template for its own local configuration (e.g. the API base URL):

```text
.env.example

```

Copy the example configuration into a local `.env` file and provide the required values according to the instructions in `SETUP.md`.

If an external AI provider is used, its API key must be configured through User Secrets (or environment variables in production) and must never be committed to source control.

Secrets and API keys must never be committed to source control.

For security-related guidance, see:

`SECURITY.md`

## Running the Application

The exact commands for running the backend, frontend, database and pipeline are documented in:

`SETUP.md`

The intended local development environment consists of:

```text
SQL Server
    ↓
ASP.NET Core Backend
 (includes CSV ingestion pipeline)
    ↓
React Frontend

```

The core application does not require an external AI service.

If the optional AI functionality is implemented, it can run through a local mock/local model or an externally configured provider.

## Testing

The project uses automated backend tests with xUnit (.NET) for critical functionality.

Critical test areas include:

- Authentication
- RBAC denial
- Incident creation
- AI success scenario, if AI functionality is implemented
- AI service failure fallback, if AI functionality is implemented
- AI malformed-response/schema-validation fallback, if AI functionality is implemented
- Pipeline validation
- Duplicate handling
- Data-quality rules
- Database relationships
- Clean setup/restart verification

The core incident workflow must be tested independently of AI availability.

Run the tests according to the commands documented in:

`TESTING.md`

## Security

Security considerations include:

- Password hashing
- JWT authentication
- Server-side RBAC
- Protected API endpoints
- Environment-based secret configuration
- Validation of external AI responses
- Protection against unauthorized access to maintenance tasks

The AI service, when enabled, is treated as an untrusted external dependency. AI output is validated before being used by the application.

For more information:

`SECURITY.md`

## Documentation

The repository contains additional documentation for different aspects of the project.

| **Document**                       | **Description**                                   |
| ---------------------------------- | ------------------------------------------------- |
| `Architecture.md`                  | System architecture and technical design          |
| `API_Contract.md`                  | REST API contract and endpoint expectations       |
| `DATABASE.md`                      | Database structure, relationships and persistence |
| `Dataset_Specification.md`         | Synthetic telemetry dataset and validation rules  |
| `DEPLOYMENT.md`                    | Deployment and operational deployment guidance    |
| `SECURITY.md`                      | Security principles and implementation            |
| `SETUP.md`                         | Local development and setup instructions          |
| `TESTING.md`                       | Testing instructions and test strategy            |
| `ERD.md`                           | Entity relationship diagram                       |
| `Project_Product_Specification.md` | Product requirements and project specification    |

## Project Scope

Industrial Insight is developed as a priority-gated project.

The goal is to build the smallest complete system first, stabilize it, test it, and only then expand it.

### P0 — Core

The core system includes:

- Authentication
- RBAC
- Machines
- Incidents
- Maintenance Tasks
- SQL Server
- Core frontend functionality

P0 must function independently of any AI service.

### P1 — Main Enhancements

The main enhancements include:

- AI-assisted incident assessment
- Data pipeline
- Analytics dashboard
- Backend tests
- Project documentation

AI is considered an optional P1 enhancement and should only be implemented after the P0 system is stable.

### P1.5 — Stretch

Potential extensions include:

- Statistical anomaly detection
- Pipeline scheduling
- Additional pipeline observability

### P2 — Bonus

Potential bonus work includes:

- GitHub Actions CI
- Docker Compose
- Additional non-essential UI polish

P1.5 and P2 functionality must not compromise the stability of P0/P1 functionality.

If time is limited, optional AI functionality, P1.5 and P2 features should be deprioritized in favor of completing and stabilizing the core system.

## MVP

### Operational MVP

```text
Login
  ↓
RBAC
  ↓
Machines
  ↓
Incident Creation
  ↓
Manager Incident Handling
  ↓
Maintenance Task
  ↓
Completion

```

### Data & Analytics MVP

```text
CSV
  ↓
Validate
  ↓
Transform
  ↓
SQL Server
  ↓
PipelineRun
  ↓
Dashboard / API

```

The AI-assisted incident assessment is **not required for the operational MVP**.

It is a P1 decision-support layer that can be added on top of the operational MVP if sufficient development time remains.

The system must remain fully functional without an external AI service.

## Deployment

Deployment is optional and outside the core project scope.

If deployment is attempted, it should only be done after the core system is stable.

Docker Compose is considered a P2 bonus feature and must not displace core functionality, testing, stabilization or presentation readiness.

For deployment information, see:

`DEPLOYMENT.md`

## Known Limitations / Production Gaps

This project is primarily a demonstration and educational project rather than a production industrial control system.

Known limitations include:

- Telemetry data is synthetic.
- The project does not require real-time telemetry streaming.
- Cloud-based telemetry ingestion is outside the core scope.
- Real external industrial datasets are outside the core scope.
- Production-scale infrastructure is outside the core scope.
- Advanced anomaly detection is optional.
- Scheduled pipeline execution is optional.
- Additional observability is optional.
- CI is optional.
- Docker Compose is a bonus feature.
- The AI-assisted incident assessment is optional and may be implemented as a local mock, local model, or external AI integration depending on available development time and resources.

The AI-assisted incident assessment, when implemented, is a decision-support feature and does not replace human judgement.

AI-generated suggestions must therefore be reviewed by the Technician before being used.

## Future Improvements

Potential future extensions include:

- Real industrial telemetry datasets
- Live telemetry APIs
- Cloud object storage
- Scheduled pipeline execution
- Real-time telemetry streaming
- Schema-drift detection
- Advanced anomaly detection
- Additional telemetry measurements
- Historical trend analysis
- Expanded observability
- CI/CD
- Containerized deployment
- Integration with a local or external AI provider

These improvements can be added without changing the core concept of:

**Collect → Validate → Transform → Store → Expose → Visualize → Act**

## Project Context

Industrial Insight is developed as an individual student project combining:

- Full-stack web development
- Backend API development
- React frontend development
- Data engineering
- SQL Server database design
- Optional AI-assisted functionality
- Automated testing
- Operational analytics

AI coding assistants may be used during development for boilerplate generation, test-case generation, debugging, SQL assistance and development support.

Central application logic is nevertheless reviewed and understood by the student and must be explainable during the final presentation.

The optional AI-assisted incident assessment is not intended to represent development or training of a custom AI model. It is an application-level integration/decision-support feature using a configurable AI service or local implementation.

## Project Status

**Development period:** August 17 – September 3, 2026

**Presentation:** September 4, 2026

The project follows a stability-first development strategy:

> **Build the smallest complete system first. Stabilize it. Test it. Then expand it.**

Development priority is:

```text
P0 Core System
      ↓
Stabilization
      ↓
Testing
      ↓
P1 Data & Analytics
      ↓
Optional AI Assistant
      ↓
P1.5 / P2 Stretch Features

```

The final presentation focuses on demonstrating a coherent end-to-end system rather than maximizing the number of individual features.

## License

This project is developed as a student project.

License information: **[To be added]**