# Student Pre-Project Planning Template

# Project Information

**Project Title:** Industrial Insight — A Maintenance Prioritization Platform

**Project Type:** Full-stack web application with data engineering and optional AI-assisted functionality

**Problem Statement:** Companies operating many machines often have information scattered across multiple sources (machine data, incident reports, service history, manual maintenance notes), making it difficult to quickly identify which machines and problems need attention first.

**Target Users:** Technicians who operate and report issues on industrial machines, and Managers who oversee maintenance, prioritize incidents, and monitor operational KPIs — primarily at small and mid-sized manufacturers running multiple industrial machines without a unified way to prioritize maintenance work.

**Individual or Group Project:** Individual (solo project)

**Student**

| Name | Email | Role(s) |
| --- | --- | --- |
| Henrik Oldehed | henrik.oldehed@gmail.com | Backend / Frontend / Optional AI Integration / Data Engineering / Scrum Master / UI — all roles (solo project) |

# Project Overview

## Describe Your Project

Industrial Insight helps maintenance teams identify which machines need attention first — and understand why. It combines machine telemetry, technician incident reports, and maintenance history into a single prioritization workflow, so managers can act on the most urgent problems instead of piecing together scattered data sources.

The system collects machine telemetry data, quality-assures it through a validation pipeline built into the backend, stores it in SQL Server, and exposes it via a REST API to a React application. Technicians report incidents; the system combines these with machine data to produce a transparent, explainable Priority Score per machine. Managers use this score to decide what deserves attention first, assign maintenance tasks, and monitor operational KPIs through a dashboard.

An optional AI-assisted incident assessment may analyze incident descriptions and suggest category, priority, and recommended action, feeding directly into the Priority Score calculation. This AI functionality is a P1 enhancement and is not required for the core operational MVP.

Industrial Insight is designed to sit on top of existing operational data and workflows — adding a prioritization layer rather than requiring a full system replacement.

The central data and operational flow is:

**Machine telemetry + incident reports → data quality validation → priority scoring → AI-assisted assessment → maintenance action → resolution & analytics**

The project is developed using a priority-gated approach. The core product is completed and stabilized before optional enhancements are added.

## Problem Statement

Companies with many machines often struggle to quickly answer the question that matters most: **which machine needs attention first, and why?** Related questions include:

- Which machines have the most incidents?
- Which problems are recurring?
- How fast are incidents resolved?
- Is incoming telemetry data complete and correct?

Industrial Insight centralizes this information and turns it into a prioritized, actionable view — usable both for daily operational work and for follow-up, maintenance, and reporting.

## Customer & Wedge

**Target customer:** small and mid-sized manufacturers operating multiple industrial machines, where maintenance information currently lives in spreadsheets, informal notes, or disconnected systems rather than a unified platform.

**Why this is an easier entry point than a full system replacement:** larger industrial companies typically already run established enterprise CMMS/EAM systems. Rather than competing with those systems, Industrial Insight is positioned to sit on top of a company's existing machine data and maintenance workflows, adding a prioritization layer that answers "what should we deal with first?" without requiring the customer to migrate away from tools they already use.

This is a product-positioning assumption for the purpose of this project, not the result of customer interviews or market research.

# Scope Management & Development Strategy

The project has 18 development days (Day 1–18, August 17 – September 3), followed by one presentation-only day (Day 19, September 4, no new features/development).

The project does not assume that every originally planned feature must be completed. Instead, development follows four priority levels:

| Priority | Scope | Rule |
| --- | --- | --- |
| **P0** | Authentication (new accounts default to Technician; Manager accounts seeded/controlled), RBAC, Machines, Incidents, Maintenance Tasks, database and core frontend | **Must work** |
| **P1** | Data pipeline, dashboard, backend tests, documentation, essential UI polish, **optional AI-assisted incident assessment if time permits** | **Should work** |
| **P1.5** | Statistical anomaly detection, pipeline scheduling and additional observability | **Added only if P0/P1 are stable** |
| **P2** | CI, Docker Compose, additional non-essential polish | **Bonus only** |

The AI-assisted incident assessment is therefore treated as an **optional P1 enhancement** rather than a mandatory requirement.

A paid external AI service is not required for the project.

If the AI feature is implemented, it may use:

- A local mock AI implementation
- A local open-source model
- An optional external AI provider

The choice of implementation must not introduce a dependency that threatens the stability of the core system.

## Core Scope Rule

A higher-priority level must never be started while a lower-priority level is unstable.

The day-by-day plan is therefore a guide rather than an unconditional sequence. If a previous feature becomes unstable, fixing it takes priority over starting a new feature.

Optional AI development must never take priority over unfinished or unstable P0 functionality.

## Hard Scope Rule

If Checkpoint 1 or Checkpoint 2 is delayed, all optional features are immediately frozen.

No new features or architectural changes are introduced until the core workflow is stable.

The AI-assisted incident assessment is considered optional and may be removed from the active scope at any point if it threatens the schedule or stability.

Day 18 Freeze Rule: No new features may be introduced on Day 18. If the system is still unstable at the end of Day 18, the latest stable state verified at Checkpoint 2 becomes the presentation baseline. Remaining issues are documented as known limitations rather than addressed through last-minute feature development.

## Definition of "Unstable"

A previous feature or priority level is considered unstable if at least one of the following applies:

- Its defined "Done when" criterion has not been fulfilled.
- An existing relevant automated test is failing.
- A previously working integration flow has regressed.

Examples of instability: authentication no longer works; RBAC allows a Technician to access Manager-only functionality; incident creation fails; a previously working API/database integration is broken; the pipeline previously loaded valid data but now fails or produces incorrect results; a critical test that previously passed is now red.

## Unfinished vs. Unstable

An unfinished feature is not automatically unstable.

Examples of unfinished but stable work:

- dashboard has not received additional visual polish;
- README still needs an architecture diagram;
- CI has not been implemented;
- AI-assisted incident assessment has not been implemented;
- anomaly detection has not yet been added;
- a page could be improved for mobile.

These do not automatically block progression.

## Daily Decision Algorithm

At the end of each development day:

- Is the current priority level stable? No → fix/continue the current level. Yes → continue.
- Has previously completed functionality regressed? Yes → fix the regression. No → continue.
- Is meaningful development time remaining? No → stop and document what remains. Yes → move to the next highest-priority task.
- Is an optional feature consuming time needed for core functionality? Yes → stop the optional feature and return to the core scope.

The goal is therefore:

**Build the smallest complete system first, stabilize it, test it, and only then expand it.**

# Main Features

| Feature | Description | Priority |
| --- | --- | --- |
| Authentication & RBAC | JWT-based login/register with two roles (Technician, Manager) and server-side role enforcement on protected endpoints | P0 |
| Machines CRUD | Create, view and update machines; list and detail views with status, runtime and location | P0 |
| Incident reporting | Technicians report incidents tied to a machine; Managers view, prioritize and manage them | P0 |
| Maintenance tasks | Managers create and assign tasks from incidents; Technicians update task status to completed | P0 |
| AI-assisted incident assessment | Optional backend AI/local mock integration that analyzes incident descriptions and returns structured category/priority/recommended action; output is validated before use and the feature degrades gracefully if unavailable | P1 Optional |
| Data pipeline | C# ingestion routine ingests synthetic machine telemetry (CSV), validates it, flags data-quality issues, transforms it and loads it into SQL Server | P1 |
| Analytics dashboard | KPI cards and operational metrics based on SQL aggregation, including machine and incident information and pipeline status. A minimal dashboard/API view should exist by Day 13; fuller KPI coverage, including Average Resolution Time using `Incident.ResolvedAt`, is added Day 16 | P1 |
| Backend test suite | xUnit coverage of critical flows including authentication, RBAC denial, incident creation and pipeline validation. AI-specific tests are included only if the AI feature is implemented | P1 |
| Documentation | README, ERD, architecture overview, setup instructions, AI/pipeline explanation, testing instructions, and a Known Limitations / Production Gaps section | P1 |
| Statistical anomaly detection | Simple ±2 standard deviation threshold rule for identifying out-of-range telemetry values | P1.5 |
| Pipeline scheduling | Periodic execution of the telemetry pipeline | P1.5 |
| Pipeline observability | Additional visibility into pipeline runs, validation results and data-quality outcomes | P1.5 |
| CI pipeline | GitHub Actions workflow running xUnit and building the frontend on push | P2 |
| Docker Compose | Containerized local setup for the full stack; remains P2 and is bonus only | P2 |

**Issue Tracker link:** *(to be added)*

# User Roles

| Role | Permissions |
| --- | --- |
| Technician | View relevant machines and machine history; report incidents; view own assigned maintenance tasks; update own assigned task status |
| Manager | View all machines and incidents; prioritize and manage incidents; create and assign maintenance tasks; view dashboard and KPIs; view data pipeline runs |

Server-side authorization is required.

A Technician must not be able to access Manager-only functionality or modify maintenance tasks assigned to another user.

# Technology Stack & Tools

## Backend

[X] C#  
[X] ASP.NET Core Web API  
[ ] MVC  
[X] Entity Framework Core  
[X] Identity  
[ ] Clean Architecture  
[X] Identity / Auth  
[ ] Repository / Unit of Work

**Other:** ASP.NET Identity, JWT Bearer authentication, FluentValidation (or DataAnnotations), CsvHelper (CSV ingestion)

## Frontend

[ ] HTML / CSS  
[ ] Bootstrap  
[X] Tailwind  
[ ] JavaScript  
[X] TypeScript  
[X] React

**Other:** Vite, React Router, recharts (optional, if time allows)

## Database

[X] SQL Server  
[ ] SQLite

**Other:** —

## AI & Automation

[ ] OpenAI API  
[ ] ML.NET  
[X] AI-assisted code generation  
[X] AI-powered testing/debugging

### Optional AI-assisted incident assessment

The AI-assisted incident assessment is an optional P1 application feature.

If implemented, it analyzes an incident description and returns structured information containing:

- `category`
- `priority`
- `recommended_action`

The implementation may use a local mock service, local open-source model, or optional external AI provider such as OpenAI or Anthropic.

A paid external AI subscription is **not required**.

The application architecture should keep the AI provider replaceable and prevent the AI service from becoming a dependency for incident creation.

The AI response is treated as untrusted external input and must be validated against a C# validation schema before being used by the application.

A malformed or invalid response follows the same fallback path as an unavailable AI service.

## Dev Tools

[X] GitHub  
[ ] Postman  
[X] Scrum Board (GitHub Projects / Trello / Jira / Other)

**IDE:** Visual Studio for the `/backend` solution (C#/ASP.NET Core, EF Core migrations); VS Code for the `/frontend` project (React/TypeScript). Both live in the same Git repository.

Swagger/OpenAPI (auto-generated via ASP.NET Core / Swashbuckle) is used for manual endpoint testing instead of Postman.

# AI & Development Tools

AI coding assistants are used as development support throughout the project — for boilerplate code, generating test cases, debugging, SQL help, and development assistance.

Every central piece of logic must nevertheless be understood and explainable by the student in the presentation.

Concrete examples of AI-generated code that was reviewed, modified or corrected during development will be documented as "Challenges & Solutions" in the final presentation.

AI-generated output is therefore treated as development assistance rather than automatically correct or production-ready code.

The use of AI coding assistants during development is separate from the optional AI-assisted incident assessment product feature.

# Optional AI Feature

The AI-assisted incident assessment is an optional P1 decision-support feature.

When enabled, a Technician can submit an incident description and receive a structured suggestion containing:

- Category
- Priority
- Recommended action

The Technician reviews the suggestion and can accept or edit it before saving the incident.

The AI feature is a decision-support layer on top of an already-working manual incident flow. It is not intended to replace human judgement or become a standalone chatbot.

The AI response is validated against a defined C# validation schema before being used by the application.

AI output is therefore treated as untrusted external input.

The system is designed to degrade gracefully:

- If the AI service is unavailable, the incident can still be saved.
- If the AI response is malformed or fails schema validation, the incident can still be saved.
- If the AI feature is disabled or not implemented, the manual incident workflow remains fully functional.
- The AI feature must never become a single point of failure for incident reporting.

The operational P0 workflow must therefore work without the AI service.

AI implementation should only proceed after the manual incident and maintenance workflow is stable and if sufficient development time remains.

# Testing

AI assistance may be used to generate initial test-case drafts and assist with debugging. However, test correctness, relevance and coverage of critical flows are reviewed and owned by the student.

Critical test areas include:

- Authentication
- RBAC denial
- Incident creation
- Pipeline validation
- Duplicate handling/data-quality rules
- Database relationships
- Clean setup/restart verification

If the optional AI-assisted incident assessment is implemented, additional tests should cover:

- AI success scenario
- AI service failure/fallback scenario
- AI malformed-response/schema-validation scenario

AI-specific tests are not required if the optional AI feature is not implemented.

# Deployment

Deployment is optional and outside the core project scope.

Deployment may be attempted if the application is stable and sufficient time remains, but it will never take priority over core functionality, testing, or presentation readiness.

If deployment is attempted, it should happen once the system is already stable and well before Day 18.

Day 18 is reserved as a freeze/stabilization day rather than for a first deployment attempt.

**Docker Compose remains P2 regardless of schedule and may be implemented only as a bonus item if the core application is already stable.**

# Database Planning

| Table/Entity | Purpose |
| --- | --- |
| Users | Stores account credentials (hashed password), name, email and role |
| Roles | Defines the two system roles (Technician, Manager) and their permissions |
| Machines | Core asset record: status, runtime and location of each industrial machine |
| Locations | Physical location associated with one or more machines |
| TelemetryRecords | Validated machine telemetry data loaded by the data pipeline (temperature, pressure, vibration, energy) |
| Incidents | Technician-reported problems tied to a machine, including optional AI suggestion fields (category, priority, recommended_action) |
| MaintenanceTasks | Work items created from an incident, assigned to a Technician, tracked to completion and linked to the incident/machine history |
| PipelineRuns | Log of each pipeline execution: processed/accepted/rejected counts, duplicates, data quality percentage and status |

**Database design tool:** dbdiagram.io

## Database Design & Schema Draft

### Entity List

- Users — UserId (PK), Name, Email, PasswordHash, RoleId (FK)
- Roles — RoleId (PK), Name
- Locations — LocationId (PK), Name, Address
- Machines — MachineId (PK), LocationId (FK), Name, Status, Runtime
- TelemetryRecords — TelemetryId (PK), MachineId (FK), Timestamp, Temperature, Pressure, Vibration, Energy
- Incidents — IncidentId (PK), MachineId (FK), ReportedByUserId (FK), Description, Status, Priority, Category, optional AiSuggestion, CreatedAt, ResolvedAt (nullable, timestamp)
- MaintenanceTasks — TaskId (PK), IncidentId (FK), AssignedToUserId (FK), Status, CreatedAt, CompletedAt
- PipelineRuns — RunId (PK), StartedAt, FinishedAt, RecordsProcessed, RecordsAccepted, RecordsRejected, Duplicates, DataQualityPct, Status

## Relationships

- Roles 1—* Users
- Locations 1—* Machines
- Machines 1—* TelemetryRecords
- Machines 1—* Incidents
- Users 1—* Incidents (as reporter)
- Incidents 1—0..* MaintenanceTasks
- Machines 1—* MaintenanceTasks (derived through Incident → Machine; MachineId is intentionally not duplicated on MaintenanceTasks)
- Users 1—* MaintenanceTasks (as assignee)
- PipelineRuns is independent, referenced only by timestamp/log for dashboard display

`Incident.ResolvedAt` is the timestamp used to determine when an incident was resolved. Completion of an associated maintenance task may trigger the incident to be marked as resolved.

If AI is not implemented, the AI-specific incident fields may remain empty or be omitted from the active application model where practical.

## ERD Diagram

**Textual relationship overview:**

Location → Machine → { TelemetryRecord, Incident (+ optional AI suggestion fields), MaintenanceTask }

## Database Schema (ERD) — Summary

- Table: Users (related to Roles)
- Table: Machines (related to Locations)
- Table: Incidents (related to Machines and Users)
- Table: MaintenanceTasks (related to Incidents and Users)
- Table: TelemetryRecords (related to Machines)
- Table: PipelineRuns (standalone log table)

*Relationship type used throughout: One-to-Many.*

**Full ERD diagram link:** *(to be added)*

# UI/UX Planning

## Main Pages

| Page | Purpose |
| --- | --- |
| Login / Register | Authenticate existing users or register a new account. New accounts are created as Technician by default; Manager accounts are seeded or created through controlled administration, not open self-registration. |
| Machines list & detail | Browse machines, view status/runtime/location and see relevant machine history |
| Incident reporting form | Technician submits a new incident. Optional AI suggestion with accept/edit functionality is shown only when the AI feature is enabled and available. |
| Incidents list & detail | Manager view of incidents, including prioritization and management controls |
| Maintenance tasks | Manager creates/assigns tasks; Technician views own assigned tasks and marks them completed |
| Dashboard | Priority Score list (machines ranked by urgency) plus KPI cards: total machines, open incidents, average resolution time, latest pipeline run and pipeline data-quality percentage |
| Pipeline runs | Manager-oriented view of recent pipeline runs, validation outcomes and data-quality percentage |

## Priority Score

Each machine is assigned a transparent Priority Score, shown directly in the dashboard as an actionable item rather than a passive statistic:

```text
Machine A — Priority 82/100 — HIGH
3 unresolved incidents · recurring issue
Recommended action: Inspect bearing assembly
[Create maintenance task]
```

The score is calculated from explicit, explainable rules — open incident count, incident severity, and recurrence — rather than a predictive model. The project deliberately frames this as **maintenance prioritization**, not "predictive maintenance": the underlying logic is rule-based and transparent, not a trained model forecasting failures. This distinction is kept explicit in the UI, documentation, and presentation.

The Priority Score reuses the same underlying logic as the "Problematic machines" KPI defined below, presented as a ranked, actionable list instead of a single aggregate count. If the optional AI-assisted incident assessment is implemented, its category/priority/rationale output feeds into this same score.

## Dashboard KPI Definitions

**Total machines:** Number of machines stored in the database.

**Open incidents:** Number of incidents that are not resolved.

**Problematic machines (Priority Score basis):** Machines with either:

- at least 2 open incidents, or
- at least 1 critical incident

during the selected reporting period. This is the same rule that drives the Priority Score list above.

The default reporting period is the most recent 7 days unless otherwise specified by the dashboard implementation.

**Average resolution time:** Average time between `Incident.CreatedAt` and `Incident.ResolvedAt` for resolved incidents.

**Latest pipeline run:** Most recent `PipelineRun`, including status and processing statistics.

**Pipeline data quality percentage:** Data-quality percentage calculated by the pipeline and stored in `PipelineRuns`.

# Development Plan

The project consists of 18 development days (Day 1–18, August 17 – September 3), followed by Day 19 (September 4) reserved solely for delivering the presentation.

Other presentations are expected to be held that day too, so no development, fixing, or rehearsal time is available on Day 19.

All preparation must be completed by the end of Day 18.

The plan is intentionally adaptive. The priority gate and stability rules take precedence over the calendar if development takes longer or shorter than expected.

# Week 1 — Foundation

**Mon Aug 17 – Sun Aug 23, Day 1–7**

## Day 1 (Mon Aug 17) — Project Foundation

Project setup, Git repository, backend structure, SQL Server connection, EF Core, initial migration for empty tables (Roles, Users, Machines, Locations).

**Seed/demo data:** Establish the seed-data mechanism and, where practical, prepare initial role/user seed data. Demo machine/location data does not need to be complete on Day 1.

**Done when:** Backend starts, database connection works, migrations run without errors.

## Day 2 (Tue Aug 18) — Authentication

Register, login, password hashing, JWT creation/validation, protected endpoint, authentication error handling, and seed data for Roles/Users.

New self-registered accounts default to Technician. Manager accounts are seeded or otherwise controlled.

**Done when:** A user can register, log in, receive a JWT and access a protected endpoint.

## Day 3 (Wed Aug 19) — RBAC + Machines Backend

Technician/Manager roles, server-side RBAC, Machine CRUD, Location relationships, Machine status/runtime, and seed data for Machines/Locations.

**Done when:** RBAC works and machine CRUD persists correctly in SQL Server.

## Day 4 (Thu Aug 20) — Frontend Foundation

Vite + React + TypeScript, Tailwind, React Router, Login/register, Auth context/state, Protected routes, Basic navigation/layout.

**Done when:** A user can register, log in and enter protected application pages.

## Day 5 (Fri Aug 21) — Machines Frontend

Machine list, Machine detail, Status, Runtime, Location, API integration, Loading/error states.

**Done when:** A logged-in user can browse machines and view real database data.

## Day 6 (Sat Aug 22) — Frontend & Integration Polish

Extra time for cleanup, edge cases, and consistency across auth/RBAC/Machines before the checkpoint; absorbs any slippage from Days 1–5.

**Done when:** No regressions are introduced; auth/RBAC/Machines are ready for the Day 7 checkpoint.

## Day 7 (Sun Aug 23) — Integration Checkpoint / Buffer

Verify:

**Register → Login → JWT → RBAC → Machine API → Machine frontend**

If stable, use remaining time for cleanup, validation, seed/demo data and preparation for the incident workflow.

Checkpoint 1 does not require the AI feature to be implemented.

**Gate:** Do not proceed if authentication, RBAC or Machines are unstable.

**Done when:** Checkpoint 1 passed.

# Week 2 — Core Flow + Data Pipeline

**Mon Aug 24 – Sun Aug 30, Day 8–14**

## Day 8 (Mon Aug 24) — Incidents Backend

Incident model, Machine/User relationships, Incident creation, Retrieval, Status, Priority, Category, Validation, Authorization.

Incident creation is P0.

**Done when:** A Technician can create an incident for a machine and a Manager can retrieve it.

## Day 9 (Tue Aug 25) — Incidents Frontend

Incident reporting form, Machine selection, Description, Incident list, Incident detail, Validation, Loading/error states.

**Done when:** A Technician can select a machine, describe a problem and save an incident.

## Day 10 (Wed Aug 26) — Maintenance Tasks

MaintenanceTask model, Incident relationship, Task assignment, Manager task creation, Technician task list, Task status, Completion tracking.

The Technician must only be able to view and update maintenance tasks assigned to that Technician. Managers have the required management and assignment permissions.

**Done when:** Incident → Manager creates task → Technician receives task → Technician completes task.

## Day 11 (Thu Aug 27) — Optional AI-assisted incident assessment

Only start this task if:

- P0 incident reporting is stable.
- Maintenance task workflow is stable.
- No critical regression exists.
- Meaningful development time remains.

If implemented, develop the AI service abstraction, structured response, Category, Priority, Recommended action, C# validation schema and graceful failure handling.

A local mock implementation is acceptable and may be preferred if no external AI service is available.

A paid external AI subscription is not required.

The AI response is treated as untrusted external input. A malformed response, invalid structure or unavailable AI service must trigger the same fallback path: the incident can still be saved.

**Done when:** If the AI feature is implemented, a Technician can receive a valid suggestion, review/edit it, and save the incident even when the AI service fails or returns invalid data.

**If not implemented:** Day 11 becomes buffer/stabilization time for P0 functionality.

### Risk Block Note — Day 8–11

These four days should be treated as one continuous risk block rather than four independent features, since the integration between RBAC, incident status transitions, maintenance tasks and optional AI fallback — not the CRUD code itself — is the primary risk.

If Day 8 or 9 is not fully stable by end of day, delay Day 10 and any AI work rather than proceeding on schedule.

The AI feature must be the first item removed from the schedule if the core workflow is under pressure.

## Day 12 (Fri Aug 28) — Data Pipeline: Ingestion & Validation

Synthetic telemetry CSV, C# CSV ingestion (CsvHelper), schema validation, required fields, data types, missing values, invalid values, duplicate detection and data-quality metrics.

**Done when:** The pipeline can process synthetic CSV data and clearly distinguish accepted, rejected and duplicate records.

## Day 13 (Sat Aug 29) — Pipeline Transformation & SQL Server + Minimal Dashboard

Data transformation, Machine mapping, SQL Server loading, TelemetryRecords, PipelineRuns, processed/accepted/rejected counts, duplicate count, data-quality percentage and pipeline status.

A minimal dashboard/API implementation should also be established where practical, based on real persisted data.

At minimum it should support:

- Total machines
- Open incidents
- Latest pipeline status
- Pipeline data-quality percentage

**Done when:** CSV → Validate → Transform → SQL Server → PipelineRun works end-to-end, and the minimal dashboard/API views can retrieve real persisted data.

## Day 14 (Sun Aug 30) — Vertical Slice Checkpoint

Verify the operational workflow:

**Login → Incident → Manager handles incident → Maintenance Task → Completion**

Verify the data workflow:

**CSV → Validation → Transformation → SQL Server → PipelineRun → backend/API access**

If the optional AI feature has been implemented, also verify:

**Incident → AI suggestion → Technician review/edit**

The operational workflow must remain functional without the AI service.

**Gate:** If either core workflow is unstable, do not start dashboard expansion or stretch features. Use available time to stabilize the core system.

**Done when:** Checkpoint 2 passed — the most important development checkpoint.

# Week 3 — Stabilization, Analytics, Quality & Presentation

**Mon Aug 31 – Fri Sep 4, Day 15–19**

## Day 15 (Mon Aug 31) — Integration, Testing & Stabilization

Integration testing, API endpoint review, RBAC review, pipeline validation scenarios, database relationship checks, seed/demo data and regression fixes.

If the optional AI feature has been implemented, test:

- AI success scenario
- AI failure scenario
- AI schema-validation scenario

**Done when:** Major P0/P1 workflows can be executed repeatedly without breaking.

## Day 16 (Tue Sep 1) — Analytics + Dashboard

SQL aggregation endpoints, Dashboard API integration and full KPI coverage.

Dashboard should include, where practical:

- Total machines
- Open incidents
- Problematic machines / Priority Score
- Average resolution time using `Incident.CreatedAt → Incident.ResolvedAt`
- Latest pipeline run
- Pipeline data-quality percentage

Problematic machines are defined as machines with at least 2 open incidents or at least 1 critical incident during the selected reporting period, with the default reporting period being the most recent 7 days. This same rule underlies the Priority Score below.

### Priority Score (v1, rule-based — define before implementing)

```
score = 40 × min(open_incidents / 5, 1)
      + 40 × (has_critical_incident ? 1 : 0)
      + 20 × (recurring_issue ? 1 : 0)

→ 0–100, bucketed:
  HIGH   ≥ 70
  MEDIUM 40–69
  LOW    < 40
```

`recurring_issue` = 2+ incidents of the same category on the same machine within the reporting period.

This is intentionally simple and explainable, not a predictive model. Weights are a starting point — adjust only if time permits, and keep whatever is shipped in sync with this section. If the AI-assisted incident assessment (Day 11) is implemented, its category/priority output can feed into `recurring_issue` and `has_critical_incident`, but the score must work correctly without AI input.

**Done when:** The dashboard displays meaningful real data from the database rather than hardcoded values, including a Priority Score (or the plain "Problematic machines" KPI, if Priority Score is descoped) per machine.

## Day 17 (Wed Sep 2) — Backend Tests + Optional Stretch

Primary:

Approximately 6–10 meaningful xUnit tests covering:

- Authentication
- RBAC denial
- Incident creation
- Pipeline validation
- Duplicate handling
- Data-quality rules
- Database relationships

If the optional AI feature is implemented, include:

- AI success
- AI failure fallback
- AI malformed-response/schema-validation fallback

Secondary, only if P0/P1 are stable:

- ±2 standard deviation anomaly detection rule.

**Done when:** Critical tests pass; anomaly detection is added only if stable.

**Gate:** Fix tests and existing functionality before implementing anomaly detection or other stretch features.

## Day 18 (Thu Sep 3) — Final Stabilization & Presentation Preparation

**Freeze day — no new features.**

No new features unless required to fix a critical issue.

### UI

Prioritize demo pages:

- Login
- Machines
- Incident reporting
- Incident management
- Maintenance tasks
- Dashboard

Prioritize:

- Loading states
- Error states
- Consistent layout
- Basic responsiveness
- Clear status/priority presentation

If the AI feature was implemented, ensure its UI is stable but do not add new AI functionality.

### Documentation

Complete:

- README
- Setup instructions
- ERD
- Architecture overview
- Technology stack
- Optional AI explanation
- Pipeline explanation
- Testing instructions
- Known Limitations / Production Gaps section

### Final verification

Run full regression testing and the complete demo from a clean state.

Prepare backup/demo data.

The seed/demo dataset should include:

- 1 Manager
- 2 Technicians
- ≥5 machines
- ≥5 incidents across different statuses
- ≥3 maintenance tasks
- At least 1 completed maintenance task
- Telemetry data
- At least 1 successful PipelineRun
- At least 1 partially failed PipelineRun demonstrating data-quality handling

If the AI feature is implemented, prepare a demonstration incident that produces a meaningful suggestion.

Deployment, if attempted, should already have happened earlier once the system was stable.

**Done when:** Checkpoint 3 passed — the system is presentable, documented, reproducible, and a new developer can understand what it does and how to run it.

If not, the Day 18 Freeze Rule applies: present the last stable state from Checkpoint 2 and document remaining issues as known limitations.

## Day 19 (Fri Sep 4) — Presentation Day

Deliver the final presentation.

No development, fixing, or rehearsal — other presentations are expected to be held this day, so all preparation must already be complete.

**Done when:** The presentation is delivered reliably from start to finish.

# Future Improvements (Not in 18-Day Scope)

The following improvements were deliberately excluded from the 18-day scope because they would introduce new external dependencies (cloud accounts, credentials, network/auth failure modes, or new orchestration systems) late in the project, which conflicts with the stability-gate principle above.

- Cloud element in the data pipeline: upload telemetry CSV to a cloud object store (e.g. S3 or Azure Blob) before processing, or introduce a separate, optional Python/Airflow scheduling service in Docker (outside the core C#/.NET stack), to strengthen the data-engineering profile of the project.
- Validation against real external data: ingest from a live external API or a real-world dataset instead of only synthetic CSV, to prove robustness against real-world data messiness such as schema drift, unexpected nulls and encoding issues.
- Integration with a more advanced local AI model, if future development time or hardware permits.
- Integration with an external AI provider, if a suitable provider becomes available and the dependency is acceptable.

All of these are natural extensions of the existing architecture.

The core concept remains:

**Collect → Validate → Transform → Store → Expose → Visualize → Act**

# Stretch Goals

Stretch goals are only started after P0/P1 are stable.

## P1.5 — Preferred order

1. Statistical anomaly detection
2. Pipeline scheduling
3. Additional pipeline observability

Anomaly detection is preferred because it has relatively low implementation cost and provides strong data-engineering/statistical value.

## P2 — Bonus only

4. GitHub Actions CI
5. Docker Compose
6. Additional non-essential UI polish

Docker Compose remains P2 regardless of available time and must never displace P0/P1 stabilization.

# Scope Reduction Strategy

If development falls behind, features are removed in the following order.

## Cut first

- CI
- Advanced observability
- Pipeline scheduling
- Extra UI polish
- Docker Compose (P2, bonus only)
- Optional AI-assisted incident assessment

## Cut next

- Additional dashboard metrics
- Non-essential machine-history functionality
- Convenience features
- Statistical anomaly detection

## Protect

- Authentication
- RBAC
- Machines
- Incidents
- Maintenance Tasks
- SQL Server
- Core data pipeline
- Critical automated tests
- Basic dashboard
- Documentation (including Known Limitations section)
- Stable demo flow

The optional AI-assisted incident assessment is **not protected** if its implementation threatens the stability or completion of the core system.

The guiding principle is:

**Cut breadth before cutting stability.**

# Critical Checkpoints

## Checkpoint 1 — Day 7

**Question:** Is the foundation stable?

Required:

- Authentication
- RBAC
- Machines
- Frontend foundation
- SQL Server/database connection
- Migrations
- Clean documented setup/restart procedure

AI is not a Checkpoint 1 requirement.

**Pass condition:** Foundation works without regression and the system can be restarted from a documented clean setup.

## Checkpoint 2 — Day 14

**Question:** Is the core product stable?

Required operational workflow:

**Login → Incident → Manager handling → Maintenance Task → Completion**

Required data workflow:

**CSV → Validation → Transformation → SQL Server → PipelineRun → API access**

If the optional AI feature has been implemented, it should be available as an additional layer, but the operational workflow must remain functional without it.

**Pass condition:** Both the operational and data workflows work end-to-end with persisted data and no critical regression.

## Checkpoint 3 — Day 18

**Question:** Is the project presentation-ready?

Required:

- Complete stable demo flow
- P0 functionality stable
- Meaningful P1 functionality complete
- Critical tests passing
- Documentation complete
- Known Limitations / Production Gaps documented
- Clean setup/restart verified
- Demo data prepared
- Regression testing completed
- Presentation prepared

The AI-assisted incident assessment is not required for Checkpoint 3 if it was not completed.

If implemented, it should be stable enough to demonstrate or clearly documented as a working optional component.

This is a freeze day — no new features.

If the system is still unstable at the end of Day 18, the Day 18 Freeze Rule applies: present the last stable state from Checkpoint 2 and document remaining issues as known limitations.

Day 19 is presentation-only, with no remaining preparation time.

# Final Demonstration Flow

A recommended demonstration sequence is:

1. Login as Technician
2. View a machine
3. Report an incident
4. Review/save incident
5. Switch to Manager
6. Review incident
7. Prioritize/manage incident
8. Create/assign maintenance task
9. Switch to Technician
10. Complete maintenance task
11. Open dashboard
12. Show KPI data
13. Show pipeline/data-quality result
14. Briefly explain architecture and data flow

### Optional AI demonstration

If the AI-assisted incident assessment has been implemented, additionally:

1. Report an incident with a meaningful description
2. Show the AI-generated category/priority/action
3. Review or edit the suggestion
4. Save the incident
5. Briefly demonstrate or explain the fallback behavior

The AI demonstration is optional.

The core presentation should remain coherent without it.

If the AI service is unavailable during the demonstration, the manual incident workflow must remain demonstrable.

The demonstration should focus on showing a coherent system rather than maximizing the number of individual features.

# Definition of Done

A feature is considered complete when:

- The implementation exists.
- Relevant API/database/frontend integration works.
- Relevant validation is implemented.
- Authorization rules work where applicable.
- Existing functionality has not regressed.
- The defined "Done when" criterion is satisfied.
- Relevant tests exist for critical functionality.

For optional AI functionality specifically:

- The external or local response is validated against the expected schema.
- Invalid/malformed responses do not break incident creation.
- AI service failure does not block the operational incident workflow.
- The feature can be disabled without breaking the manual incident workflow.

For pipeline functionality specifically:

- Validation rules are applied.
- Accepted, rejected and duplicate records are distinguishable.
- PipelineRun records the relevant execution and data-quality results.

*"Code written" does not automatically mean "done." "Works once" does not automatically mean "stable."*

# Final Project Success Definition

The project is successful if it demonstrates a coherent end-to-end system in which:

- machine data enters through a quality-controlled pipeline;
- validated data is stored in SQL Server;
- technicians interact with machines and report incidents;
- managers handle incidents and maintenance tasks;
- operational information is exposed through APIs;
- operational KPIs are summarized in a dashboard;
- critical authorization and validation rules are enforced;
- the core system can be started from a clean documented setup.

The optional AI-assisted incident assessment can additionally provide decision support for incident classification and recommended action if implemented.

## MVP Success

The MVP is considered successful when the smallest complete core system works end-to-end.

### Operational MVP

**Login → RBAC → Machines → Incident creation → Manager incident handling → Maintenance Task → Completion**

The AI-assisted incident assessment is an optional P1 decision-support layer on top of this workflow. It may enrich the incident-reporting experience, but the operational workflow must remain fully functional without the AI service.

### Data & Analytics MVP

**CSV → Validate → Transform → SQL Server → PipelineRun → Dashboard/API views**

The dashboard must display real persisted database data rather than only hardcoded values.

## Final Success

Final success additionally requires:

- MVP workflows remain stable after regression testing.
- Critical automated tests pass.
- Application starts from a clean documented setup.
- Database migrations work from the documented setup.
- Documentation is complete.
- Demo data is prepared.
- No critical known issue prevents demonstration.
- Dashboard KPIs use defined and consistent calculations.
- Known Limitations / Production Gaps are documented.

If the optional AI feature is implemented, final success additionally requires:

- AI output validation works.
- AI failure does not block incident creation.
- AI functionality is clearly presented as decision support rather than autonomous decision-making.

If the AI feature is not implemented, this does **not** prevent the project from meeting the MVP or final project success criteria.

Everything beyond this core system is an enhancement.

Success is therefore defined as:

**P0 fully complete and stable, plus a meaningful and complete portion of P1 reached before Week 3 stabilization begins.**

The AI-assisted incident assessment is an optional P1 enhancement and must not be allowed to compromise P0 stability, the data pipeline, dashboard, testing, documentation, or presentation readiness.

P1.5 and P2 items must never be allowed to consume Week 3 stabilization time.

The development strategy is therefore:

**Build the smallest complete system first. Stabilize it. Test it. Then expand it only when the existing system is safe.**

**18 development days (August 17 – September 3) + 1 presentation-only day (Day 19, September 4).**

**Presentation: September 4.**