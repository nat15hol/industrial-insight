---
name: Architecture
description: System architecture and technical design for Industrial Insight.
---

# Architecture

## Overview

Industrial Insight is a full-stack web application consisting of a React frontend, an ASP.NET Core Web API backend, a SQL Server database, an embedded C# telemetry ingestion pipeline, and an optional, pluggable AI-assisted incident assessment layer.

> **Repository layout note:** "backend" and "frontend" below refer to roles, not folder names. In the repository, the backend lives in `server/` and the frontend in `client/`. See [setup.md](setup.md) for exact paths and commands.

The system is built around one continuous data and operational flow:

**Machine telemetry + incident reports → data quality validation → priority scoring → AI-assisted assessment (optional) → maintenance action → resolution & analytics**

## High-Level Component Diagram

```text
                    ┌─────────────────────┐
                    │      React UI       │
                    │ TypeScript + Vite   │
                    └──────────┬──────────┘
                               │
                               │ REST API (JWT Bearer)
                               ▼
                    ┌───────────────────────┐
                    │ ASP.NET Core Backend  │
                    │                       │
                    │ Auth / RBAC           │
                    │ Machines              │
                    │ Incidents             │
                    │ Maintenance Tasks     │
                    │ Dashboard / KPIs      │
                    │ CSV Ingestion Pipeline│
                    │ Optional AI Layer     │
                    └───────┬─────┬─────────┘
                            │     │
                 ┌──────────┘     └──────────────┐
                 ▼                               ▼
        ┌─────────────────┐             ┌─────────────────┐
        │   SQL Server    │             │ Optional AI     │
        │  (via EF Core)  │             │ Provider        │
        │                 │             │ (mock / local /  │
        │ Users           │             │  external)       │
        │ Roles           │             │                 │
        │ Locations       │             │ Structured      │
        │ Machines        │             │ incident        │
        │ TelemetryRecords│             │ suggestions      │
        │ Incidents       │             └─────────────────┘
        │ MaintenanceTasks│
        │ PipelineRuns    │
        └─────────────────┘
```

See [erd.md](erd.md) for the full entity relationship diagram and field-level detail.

## Layers

### Frontend (React + TypeScript + Vite)

- Talks to the backend exclusively via the REST API described in [api-contract.md](api-contract.md).
- Holds the JWT in memory/context after login; attaches it as `Authorization: Bearer <token>` on protected requests.
- Route protection mirrors backend RBAC: a Technician never sees Manager-only UI, but the frontend check is a UX convenience only — the backend is the authority (see Security below).

### Backend (ASP.NET Core Web API)

Organized around the core domains defined in the data model:

- **Auth & RBAC** — registration, login, JWT issuance, role enforcement (`Technician` / `Manager`) on protected endpoints.
- **Machines** — CRUD, status/runtime/location.
- **Incidents** — creation, retrieval, prioritization, status transitions, optional AI suggestion attachment.
- **Maintenance Tasks** — creation/assignment (Manager), status updates (assigned Technician only).
- **Dashboard** — SQL aggregation endpoints for KPIs and the Priority Score list.
- **Data Pipeline** — CSV ingestion routine (CsvHelper), embedded in the backend rather than a separate service.
- **Optional AI Layer** — a replaceable service abstraction around whatever AI provider is configured (mock, local model, or external).

### Database (SQL Server via EF Core)

Relational schema described in [erd.md](erd.md) and [database.md](database.md). All persistence goes through EF Core; no raw SQL outside of the aggregation queries backing dashboard KPIs, where SQL aggregation is used deliberately for performance and clarity.

### Data Pipeline

A separate, sequential flow from the operational request/response flow above:

**Synthetic CSV → C# ingestion (CsvHelper) → validation → transformation → SQL Server load → PipelineRun record**

The pipeline distinguishes accepted, rejected, and duplicate records, and records data-quality percentage and processing statistics for each run. See [dataset-specification.md](dataset-specification.md).

### Optional AI-Assisted Incident Assessment

A decision-support layer, not a dependency of the core operational workflow:

**Incident description → AI provider (mock / local / external) → structured suggestion → C# schema validation → attached to incident (pending Technician review)**

Design principles:

- The AI provider is swappable behind a single service abstraction. Switching between a mock, a local model, and an external provider (e.g. OpenAI or Anthropic) must not require changes to the incident workflow.
- AI output is treated as **untrusted external input**. It is validated against a defined C# schema before it is ever persisted or shown to the user.
- If the AI service is unavailable, times out, or returns a malformed/invalid response, the incident can still be created and saved — the AI layer degrades gracefully and is never a single point of failure for the P0 operational workflow.

## Priority Score

Each machine receives a rule-based, explainable Priority Score (0–100), not a predictive-model output:

```
score = 40 × min(open_incidents / 5, 1)
      + 40 × (has_critical_incident ? 1 : 0)
      + 20 × (recurring_issue ? 1 : 0)

→ bucketed: HIGH ≥ 70, MEDIUM 40–69, LOW < 40
```

`recurring_issue` = 2+ incidents of the same category on the same machine within the reporting period (default: most recent 7 days). If the optional AI-assisted assessment is implemented, its `category` / `priority` output can feed into `recurring_issue` and `has_critical_incident`, but the score must remain computable — and correct — without any AI input. See `decisions/0001-priority-score-rule-based-not-ml.md`.

## Security Model

- Password hashing via ASP.NET Identity.
- JWT Bearer authentication on all protected endpoints.
- **Server-side RBAC is the authority.** Frontend route/UI restrictions are a convenience layer only; every protected endpoint independently enforces role checks and, where relevant, ownership checks (e.g. a Technician may only view/update their own assigned maintenance tasks).
- AI responses are treated as untrusted input and schema-validated before use.

Full detail in `security.md` (added once implemented — see `known-limitations.md` in the interim).

## Cross-Cutting Design Decisions

Architectural decisions with lasting rationale are recorded as ADRs rather than only in this document:

- `decisions/0001-priority-score-rule-based-not-ml.md`
- `decisions/0002-ai-feature-optional-and-pluggable.md`
- `decisions/0003-technician-cannot-self-register-as-manager.md`

## Known Constraints

- Telemetry ingestion is batch/synthetic-CSV based, not real-time streaming.
- The AI layer requires no paid external subscription — a local mock implementation is a fully acceptable and supported configuration for demonstration.
- Deployment and containerization (Docker Compose, CI) are explicitly P2/bonus and do not affect the core architecture described here.

See [known-limitations.md](known-limitations.md) for the complete list of scope boundaries.
