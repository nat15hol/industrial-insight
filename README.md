# Industrial Insight

![.NET](https://img.shields.io/badge/.NET-10-512BD4?logo=dotnet&logoColor=white)
![React](https://img.shields.io/badge/React-TypeScript-61DAFB?logo=react&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL_Server-EF_Core-CC2927?logo=microsoftsqlserver&logoColor=white)
![Status](https://img.shields.io/badge/status-in_development-yellow)
[![License](https://img.shields.io/badge/license-MIT-blue)](LICENSE)

**A maintenance prioritization platform for industrial operations**

Industrial Insight helps maintenance teams identify which machines need attention first — and understand why. It combines machine telemetry, technician incident reports, and maintenance history into a single prioritization workflow, so managers can act on the most urgent problems instead of digging through scattered data sources.

The system is designed to sit on top of existing operational data and workflows, adding a transparent intelligence layer rather than replacing existing tools.

---

## Table of Contents

- [Overview](#overview)
- [Project Status](#project-status-1)
- [Key Features](#key-features)
- [Architecture](#architecture)
- [Technology Stack](#technology-stack)
- [Data Pipeline](#data-pipeline)
- [Data Quality](#data-quality)
- [Database](#database)
- [User Roles](#user-roles)
- [Core Workflows](#core-workflows)
- [Dashboard KPIs](#dashboard-kpis)
- [Getting Started](#getting-started)
- [Environment Configuration](#environment-configuration)
- [Running the Application](#running-the-application)
- [Testing](#testing)
- [Security](#security)
- [Documentation](#documentation)
- [Project Scope](#project-scope)
- [MVP](#mvp)
- [Deployment](#deployment)
- [Known Limitations / Production Gaps](#known-limitations--production-gaps)
- [Future Improvements](#future-improvements)
- [Project Context](#project-context)
- [Author](#author)
- [License](#license)

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

## Project Status

Development runs August 17 – September 3, 2026 (18 days), with the presentation on September 4. Progress is tracked via one [GitHub Milestone](https://github.com/nat15hol/industrial-insight/milestones) per day, each scoped to a specific set of issues. This table reflects the plan at the start of development and will be updated as milestones close.

| Day | Milestone | Focus | Issues | Status |
| :-: | --- | --- | :-: | --- |
| 1 | [Project Foundation](https://github.com/nat15hol/industrial-insight/milestone/1) | DB setup, EF Core, seed mechanism | 0/4 | ⏳ Not started |
| 2 | [Authentication](https://github.com/nat15hol/industrial-insight/milestone/2) | Registration, login, JWT | 0/5 | ⏳ Not started |
| 3 | [RBAC + Machines Backend](https://github.com/nat15hol/industrial-insight/milestone/3) | Role enforcement, Machine CRUD | 0/4 | ⏳ Not started |
| 4 | [Frontend Foundation](https://github.com/nat15hol/industrial-insight/milestone/4) | Vite/React setup, routing, auth UI | 0/4 | ⏳ Not started |
| 5 | [Machines Frontend](https://github.com/nat15hol/industrial-insight/milestone/5) | Machine list/detail views | 0/3 | ⏳ Not started |
| 6 | [Frontend & Integration Polish](https://github.com/nat15hol/industrial-insight/milestone/6) | Cleanup & edge cases | 0/1 | ⏳ Not started |
| 7 | [Integration Checkpoint 1](https://github.com/nat15hol/industrial-insight/milestone/7) | Foundation validation | 0/2 | ⏳ Not started |
| 8 | [Incidents Backend](https://github.com/nat15hol/industrial-insight/milestone/8) | Incident model, create/retrieve, validation | 0/4 | ⏳ Not started |
| 9 | [Incidents Frontend](https://github.com/nat15hol/industrial-insight/milestone/9) | Incident reporting form, list/detail views | 0/3 | ⏳ Not started |
| 10 | [Maintenance Tasks](https://github.com/nat15hol/industrial-insight/milestone/10) | Task model, assignment, status tracking | 0/5 | ⏳ Not started |
| 11 | [Optional AI-Assisted Assessment](https://github.com/nat15hol/industrial-insight/milestone/11) | AI service abstraction, schema validation, fallback | 0/5 | ⏳ Not started (P1, optional) |
| 12 | [Data Pipeline: Ingestion](https://github.com/nat15hol/industrial-insight/milestone/12) | CSV ingestion, schema/duplicate validation | 0/5 | ⏳ Not started |
| 13 | [Pipeline Transformation + Dashboard](https://github.com/nat15hol/industrial-insight/milestone/13) | Transformation, SQL Server load, minimal dashboard | 0/4 | ⏳ Not started |
| 14 | [Vertical Slice Checkpoint 2](https://github.com/nat15hol/industrial-insight/milestone/14) | MVP validation (operational, data, AI) | 0/3 | ⏳ Not started |
| 15 | [Integration & Stabilization](https://github.com/nat15hol/industrial-insight/milestone/15) | Integration testing, regression fixes | 0/4 | ⏳ Not started |
| 16 | [Analytics + Dashboard](https://github.com/nat15hol/industrial-insight/milestone/16) | Full KPI set, Priority Score, dashboard UI | 0/7 | ⏳ Not started |
| 17 | [Backend Tests + Stretch](https://github.com/nat15hol/industrial-insight/milestone/17) | xUnit test suite, optional anomaly detection | 0/5 | ⏳ Not started |
| 18 | [Final Stabilization & Presentation Prep](https://github.com/nat15hol/industrial-insight/milestone/18) | Freeze, docs, demo rehearsal | 0/4 | ⏳ Not started |
| 19 | — | Presentation Day | — | — |

**Overall: 0 / 74 issues closed (0%)**

> Per the project's Day 18 Freeze Rule, if the system is unstable at the end of Day 18, the last stable state verified at Checkpoint 2 (Day 14) becomes the presentation baseline — see [known-limitations.md](known-limitations.md).

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

- [Architecture](architecture.md)
- [Database](database.md)
- [API Contract](api-contract.md)
- [Dataset Specification](dataset-specification.md)

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

[Dataset Specification](dataset-specification.md)

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

- [Database Documentation](database.md)
- [ERD](erd.md)

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

[`setup.md`](setup.md)

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

The project requires the development environment specified in [`setup.md`](setup.md).

Expected core dependencies include:

- .NET SDK
- Node.js / npm
- SQL Server
- Git

**Recommended IDEs:** Visual Studio for the backend (`server/`, C#/ASP.NET Core, EF Core migrations) and VS Code for the frontend (`client/`, React/TypeScript). Any editor with equivalent tooling can be used instead.

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

Copy the example configuration into a local `.env` file and provide the required values according to the instructions in [`setup.md`](setup.md).

If an external AI provider is used, its API key must be configured through User Secrets (or environment variables in production) and must never be committed to source control.

Secrets and API keys must never be committed to source control.

For security-related guidance, see `security.md`. **Not yet created** — will be added once concrete security implementation exists to document (see [`known-limitations.md`](known-limitations.md)). In the meantime, baseline security measures are described in [`architecture.md`](architecture.md).

## Running the Application

The exact commands for running the backend, frontend, database and pipeline are documented in:

[`setup.md`](setup.md)

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

[`testing.md`](testing.md)

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

For more information, see `security.md`. **Not yet created** — added once concrete security implementation exists to document. In the meantime, see [`architecture.md`](architecture.md) for the baseline security model and [`known-limitations.md`](known-limitations.md) for current status.

## Documentation

The repository contains additional documentation for different aspects of the project.

| **Document**                              | **Description**                                                   |
| ------------------------------------------ | ------------------------------------------------------------------ |
| [`architecture.md`](architecture.md)       | System architecture and technical design                          |
| [`api-contract.md`](api-contract.md)       | REST API contract and endpoint expectations                       |
| [`database.md`](database.md)               | Database structure, relationships and persistence                 |
| [`erd.md`](erd.md)                         | Entity relationship diagram                                       |
| [`dataset-specification.md`](dataset-specification.md) | Synthetic telemetry dataset and validation rules      |
| [`ui-wireframes.md`](ui-wireframes.md)     | Conceptual UI wireframes for key screens                          |
| [`setup.md`](setup.md)                     | Local development and setup instructions                          |
| [`testing.md`](testing.md)                 | Testing instructions and test strategy                            |
| [`known-limitations.md`](known-limitations.md) | Known limitations and production gaps                         |
| [`changelog.md`](changelog.md)             | Project documentation change history                              |
| [`decisions/`](decisions/)                 | Architecture Decision Records (ADRs)                               |
| [`project-product-specification.md`](project-product-specification.md) | Product requirements and project specification |
| [`student-pre-project-planning-template.md`](student-pre-project-planning-template.md) | Detailed day-by-day execution plan and checkpoints |
| `security.md`                              | **Not yet created.** Added once concrete security implementation exists to document. |
| `deployment.md`                            | **Not yet created.** Deployment is optional/P2 scope; added only if attempted. |

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

For deployment information, see `deployment.md`. **Not yet created** — added only if deployment is actually attempted, consistent with its P2/bonus status.

## Known Limitations / Production Gaps

This project is primarily a demonstration and educational project rather than a production industrial control system. The summary below is a snapshot — see [`known-limitations.md`](known-limitations.md) for the complete, maintained list.

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

## Author

**Henrik Oldehed**

Data Engineer | Analytics Specialist — building full-stack systems end-to-end

GitHub: https://github.com/nat15hol
LinkedIn: https://www.linkedin.com/in/henrikoldehed/

Full-stack student project spanning backend, frontend, database design, and data engineering: ASP.NET Core, React/TypeScript, SQL Server, and a C# ingestion pipeline combined into a single maintenance-prioritization workflow — authentication, RBAC, incident/task management, and analytics included.

## License

This project is developed as a student project.

Licensed under the [MIT License](LICENSE).