# Industrial Insight — Complete Project and Product Specification

**Project Baseline v1.0 — Updated**

---

# 0. Discovery – Before the Idea Is Locked

## Problem / Opportunity

Companies that operate many industrial machines often have information distributed across machine data, incident reports, service history, and manual notes. This makes it difficult to quickly answer the question that matters most: **which machine needs attention first, and why?**

**The opportunity:** to turn scattered operational data into a prioritized, actionable view — where Technicians report incidents, Managers see which machines need attention first, and machine data is quality-assured before it informs that priority.

## Who Has the Problem?

**Technicians:** need to quickly report problems and access relevant machine and maintenance information.

**Managers:** need to know which machines and problems deserve attention first, allocate maintenance work accordingly, and monitor KPIs.

The business is also indirectly affected, as better information flow can contribute to more efficient maintenance and improved data quality.

**Target customer profile:** small and mid-sized manufacturers operating multiple industrial machines, where maintenance information currently lives in spreadsheets, informal notes, or disconnected systems rather than a unified platform. This is a project assumption for scoping purposes, not the result of market segmentation research.

## How Do They Solve It Today?

For the project's hypothetical target audience, information is assumed to be handled through a combination of:

- manual reports
- spreadsheets
- separate maintenance systems
- machine/telemetry systems
- manual notes
- different data sources

Industrial Insight models this problem by bringing corresponding information together in one shared system.

## Why Are Existing Solutions Insufficient?

For the purpose of this project, the main problem is that information can be fragmented and difficult to combine. For example, a separate machine-data source does not necessarily explain why an incident occurred or whether a maintenance task has been completed — and even when the data exists, it rarely answers "what should we deal with first?" without manual cross-referencing.

**Important:** this is a project assumption and not the result of conducted customer interviews.

## User / Customer Interviews

No formal customer interviews are planned within the current scope. The project is a student project and therefore builds on defined user roles and realistic industrial scenarios.

Future validation should interview:

- maintenance technicians
- maintenance managers
- operations managers

## Industry and Competitor Analysis

Conceptually, the project overlaps with industrial CMMS, EAM, MES, and IoT platforms.

Industrial Insight does not attempt to compete with these full-scale systems or replace them. Instead, the project is positioned as a prioritization layer that could sit on top of a company's existing machine data and maintenance workflows — larger industrial companies typically already run established enterprise CMMS/EAM systems, so competing head-on would require replacing tools a customer already relies on. The project's focus is to demonstrate a smaller end-to-end solution that combines:

**telemetry → data quality → SQL Server → API → frontend → incident management → priority scoring → AI-assisted assessment → dashboard**

## Hypotheses and Assumptions

- Technicians need a simpler way to report incidents.
- Managers need better visibility into incidents and maintenance.
- Centralized machine and incident data makes operational information more useful.
- AI can provide useful suggestions for incident category, priority, and recommended action.
- Automated data validation improves the reliability of telemetry data.
- A small system can demonstrate value without implementing a complete enterprise system.

## Alternative Solutions

Alternative approaches could have been:

- an incident/maintenance system only
- a telemetry dashboard only
- a separate BI system on top of the database
- manual management using, for example, Excel
- a complete commercial CMMS/EAM system

The project instead chooses an integrated prototype.

## Why Should We Build This?

To demonstrate how data engineering, backend, frontend, AI, and operational workflows can be combined into one coherent system.

# Value Proposition & Differentiation

Industrial Insight helps maintenance teams identify which machines need attention first — and understand why. It combines operational incident management, maintenance workflows, AI-assisted incident analysis, and quality-controlled machine telemetry into a single prioritization workflow.

The main differentiator of the project is not that it attempts to replace a complete industrial EAM/CMMS or IoT platform. Instead, it demonstrates how several normally separated capabilities can be connected into one end-to-end prioritization workflow:

**Machine data → Data quality → Storage → API → Operational workflow → Priority scoring → AI-assisted assessment → Analytics**

Each machine receives a transparent, rule-based Priority Score — built from open incident count, severity, and recurrence — so the team understands not just *what* needs attention, but *why*. The scoring is deliberately framed as **maintenance prioritization**, not "predictive maintenance": it is explainable and rule-based rather than a trained model forecasting failures, and the project is intentionally honest about that distinction rather than overstating the AI component.

The AI component is deliberately implemented as decision support rather than as an autonomous system. The Technician remains responsible for reviewing and accepting or modifying the AI suggestion, and its output feeds into the same Priority Score used throughout the rest of the system.

The project therefore focuses on demonstrating the integration between data engineering, operational software, AI assistance, and analytics, rather than maximizing the number of individual features.

# What Must Be True for the Project to Succeed?

Project success is governed by four progressive validation levels:

- **POC:** critical technical assumptions are verified.
- **Prototype:** critical components work together.
- **MVP:** core workflows work end-to-end with real persistence and critical requirements fulfilled.
- **Final:** the MVP is stable, tested, and reproducible.

# 1. Vision & Goals

## Project / Product Vision

Industrial Insight helps maintenance teams at small and mid-sized manufacturers identify which machines need attention first — and understand why — by combining quality-controlled machine data, operational workflows, transparent priority scoring, and AI assistance into a single prioritization layer that sits on top of existing operational data.

## Purpose

To create a working full-stack prototype that demonstrates how industrial data can be transformed into operational information and action.

## Goals

- Implement authentication and RBAC.
- Manage machines and incidents.
- Manage maintenance tasks.
- Integrate AI for incident analysis.
- Build a quality-controlled telemetry pipeline.
- Store data in SQL Server.
- Display operational KPIs in a dashboard.
- Test critical backend flows.
- Document the system.

## Expected Value

### For the Technician

- simpler incident reporting
- AI assistance during reporting
- visibility into their own maintenance tasks

### For the Manager

- centralized incident management
- incident prioritization and work assignment
- KPI visibility
- visibility into pipeline results and data quality

### For the Business

- improved data quality
- centralized information
- ability to identify recurring problems

# Success Criteria / KPIs

## MVP Success

The MVP is successful when the following core workflows work end-to-end:

**Operational MVP:**

**Login → RBAC → Machines → Incident Creation → Manager Incident Handling → Maintenance Task → Completion**

**Data & Analytics MVP:**

**CSV → Validate → Transform → SQL Server → PipelineRun → Dashboard/API Views**

The AI-assisted incident assessment is a P1 feature layered on top of the functioning operational workflow. AI should improve incident reporting when the service is available, but the operational MVP workflow must be executable without the AI service.

The dashboard must display real, persisted data from the database rather than only hardcoded values.

## Final Success

Final success additionally requires that:

- MVP workflows remain stable after regression testing.
- Critical automated tests pass.
- Critical authorization and validation rules work.
- AI success, schema validation, and fallback work correctly.
- The application starts from a clean, documented setup.
- Database migrations work from the documented setup.
- Documentation is complete.
- Demo data is prepared.
- No critical known issue prevents demonstration.
- Dashboard KPI definitions are consistent and reproducible.
- Known Limitations / Production Gaps are documented.

## Operational Metrics

The system should be able to display the following operational KPIs:

### Total Machines

The number of machines stored in the database.

### Open Incidents

The number of incidents that are not resolved.

### Problematic Machines / Priority Score

A machine is classified as problematic if it has:

- at least **2 open incidents**, or
- at least **1 critical incident**

during the selected reporting period.

The default reporting period is the most recent **7 days**, unless the dashboard implementation specifies another period.

This same rule underlies the machine-level **Priority Score** shown in the UI (see Section 3, UI/UX Planning in the Planning Template), where problematic machines are surfaced as a ranked, actionable list — e.g. "Machine A — Priority 82/100 — HIGH · 3 unresolved incidents · recurring issue · [Create maintenance task]" — rather than only as an aggregate dashboard count. The score is rule-based and explainable, not a predictive model.

### Average Resolution Time

Average resolution time is calculated as the average time between:

`Incident.CreatedAt → Incident.ResolvedAt`

for resolved incidents.

`ResolvedAt` is nullable and is set when the incident is considered resolved. Completion of an associated maintenance task may be the event that causes the incident to be marked as resolved.

### Latest Pipeline Run

The latest `PipelineRun`, including its status and relevant processing/data-quality statistics.

### Pipeline Data-Quality Percentage

The data-quality percentage calculated by the pipeline and stored in `PipelineRuns`.

# Scope

## In Scope

- Authentication / RBAC
- Machines
- Incidents
- Maintenance Tasks
- AI-assisted incident assessment
- Telemetry pipeline
- SQL Server
- Dashboard / minimal dashboard/API views
- Testing
- Documentation

## Out of Scope

- real production machines
- live external telemetry
- cloud-based pipeline
- full-scale enterprise deployment
- advanced ML
- complete CMMS/EAM functionality

# Anti-Scope

The following functionality is explicitly excluded from the development scope:

- Cloud-based telemetry ingestion
- Live external telemetry APIs
- Real-time telemetry streaming
- Full production-grade deployment and infrastructure
- Advanced machine-learning or predictive-maintenance models
- Full enterprise CMMS/EAM functionality
- Mobile application
- Advanced notification systems
- Advanced BI/reporting functionality
- Production-grade monitoring, alerting, and disaster recovery

These items may be considered after the project as Future Improvements, but they must not introduce scope pressure during the core development period.

# MVP / First Delivery

The MVP is the smallest complete product that demonstrates the core operational and data capabilities of Industrial Insight.

## Operational MVP

**Login → RBAC → Machines → Incident Creation → Manager Incident Handling → Maintenance Task → Completion**

The AI-assisted incident assessment is a P1 decision-support layer on top of this workflow. When available, it enriches the incident-reporting experience, but the operational workflow must remain fully functional without the AI service.

## Data & Analytics MVP

**CSV → Validate → Transform → SQL Server → PipelineRun → Minimal Dashboard/API Views**

The data workflow must use real persisted data and demonstrate that accepted, rejected, and duplicate telemetry records can be distinguished.

# 2. Feasibility & Validation

## Technical Feasibility

The project is technically feasible because it uses established technologies:

- React + TypeScript
- ASP.NET Core
- SQL Server
- Entity Framework Core
- C# CSV ingestion (CsvHelper)
- JWT
- xUnit
- AI API

The architecture is relatively simple and suitable for a solo project.

## Critical Uncertainties

The main uncertainties are:

- AI API integration and structured responses.
- AI response validation and fallback.
- Time required for full-stack integration.
- Data-quality rules in the pipeline.
- Keeping authentication/RBAC stable.
- Completing testing and presentation preparation within the available time.

# POC, Prototype, MVP & Final Validation

The project uses four progressive validation levels to reduce uncertainty and verify that the solution is viable at increasing levels of completeness.

## POC — Technical Feasibility

A POC validates whether a critical technical assumption can work in isolation or in a minimal technical setup.

Examples:

- technical foundation
- server-side RBAC
- structured AI response and schema validation

## Prototype — Solution Integration

A prototype validates whether the selected technical components work together in realistic user and data flows.

## MVP — Product Validation

The MVP validates whether the smallest complete product delivers the intended core functionality end-to-end.

The operational MVP does not depend on the AI service being available.

## Final Validation — Presentation Readiness

Final validation verifies stability, regression status, documentation, reproducibility, and demo readiness.

# Validation Process

| Validation | Purpose | Level | Success Criterion |
|---|---|---|---|
| React → ASP.NET Core → SQL Server minimal flow | Verify technical integration | POC | Data can be authenticated, persisted, and retrieved |
| RBAC enforcement | Verify server-side authorization | POC | Unauthorized operations are rejected server-side |
| AI structured response + schema validation | Verify technical feasibility | POC | Representative incident descriptions produce valid structured output, while malformed output is rejected safely |
| Incident reporting | Verify realistic user flow | Prototype | Technician can complete the incident flow through the UI |
| Incident → AI → Manager → Task | Verify integrated operational flow | Prototype | Main components work together and AI failure does not block incident creation |
| Telemetry validation | Verify validation logic | POC | Valid, invalid, missing, and duplicate records are correctly classified |
| Data pipeline | Verify integrated data workflow | Prototype | Full pipeline works with representative data |
| Operational workflow | Validate complete operational product | MVP | Full operational workflow works end-to-end |
| Data & analytics workflow | Validate complete data product | MVP | Full data workflow works end-to-end |
| Stability & reproducibility | Validate submission/demo readiness | Final | No critical regression and demo works from a clean setup |

# 3. Product & Requirements

# Use Cases / User Stories

## Technician

- As a Technician, I want to be able to log in.
- As a Technician, I want to see relevant machines.
- As a Technician, I want to report an incident.
- As a Technician, I want to receive an AI suggestion when the AI service is available.
- As a Technician, I want to be able to accept or modify the AI suggestion.
- As a Technician, I want to see my maintenance tasks.
- As a Technician, I want to be able to mark an assigned task as completed.

## Manager

- As a Manager, I want to see all incidents.
- As a Manager, I want to prioritize incidents.
- As a Manager, I want to create maintenance tasks.
- As a Manager, I want to assign tasks.
- As a Manager, I want to see KPIs.
- As a Manager, I want to see pipeline results.

# Functional Requirements

The system shall, among other things:

- authenticate users
- enforce server-side RBAC
- manage machines
- create and manage incidents
- create and assign maintenance tasks
- integrate AI
- import and validate telemetry
- store data in SQL Server
- display KPIs through dashboard/API views

## Authorization Requirements

A Technician:

- must not use Manager-only endpoints
- must not access Manager-only functionality
- may only view their own assigned maintenance tasks
- may only update their own assigned maintenance tasks

A Manager:

- may view all relevant machines and incidents
- may prioritize and manage incidents
- may create and assign maintenance tasks
- may view dashboard/KPI data
- may view pipeline results

## AI Requirements

The AI feature:

- analyzes the incident description
- returns structured information
- must return `category`, `priority`, and `recommended_action`
- must be validated against a C# validation schema
- must treat AI output as untrusted external input
- must not block incident creation when the service fails
- must not block incident creation when the AI response is malformed or invalid

# Acceptance Criteria

## Incident

**Given** that a Technician is logged in,

**when** the Technician submits a valid incident,

**then** the incident is saved and associated with the selected machine.

## RBAC

**Given** that a Technician attempts to use a Manager-only endpoint or update a maintenance task assigned to another user,

**when** the request is sent,

**then** the server returns an authorization error and the operation is denied.

## AI Success

**Given** that the AI service is available,

**when** a Technician submits a valid incident description,

**then** the backend returns a structured AI suggestion containing `category`, `priority`, and `recommended_action` that passes C# schema validation.

## AI Fallback

**Given** that the AI service is unavailable or returns a malformed/invalid response,

**when** an incident is submitted,

**then** the incident can still be saved without being blocked by the AI functionality.

## Maintenance Task

**Given** that a Manager has created and assigned a maintenance task,

**when** the assigned Technician updates the task status to completed,

**then** the task is saved as completed and the incident's resolution can be recorded through `ResolvedAt`.

# 4. Solution Design

## System Architecture & Data Architecture

**React frontend → ASP.NET Core Web API → EF Core → SQL Server**

Separate data flow:

**CSV → C# Ingestion Routine → Validation → Transformation → SQL Server**

AI:

**ASP.NET Core → AI API → Structured Suggestion → Schema Validation → ASP.NET Core → React**

with clear fallback behavior if external calls fail or return invalid responses.

## Core Relationships

**Location → Machine → TelemetryRecords**

**Machine → Incident → MaintenanceTask**

**User → Incident**

**User → MaintenanceTask**

**PipelineRun** is an independent log/tracking entity for pipeline executions and data quality.

# 5. Data / AI

## Data Sources & Pipeline

The primary data source for the pipeline is synthetic CSV telemetry.

The pipeline checks:

- required fields
- data types
- missing values
- invalid values
- duplicates

The result is divided into:

- accepted
- rejected
- duplicates

`PipelineRun` stores, among other things:

- records processed
- records accepted
- records rejected
- duplicates
- data-quality percentage
- status
- timestamps

## Model Strategy

The AI-assisted incident assessment uses an external model for structured text analysis:

- `category`
- `priority`
- `recommended_action`

It is purely a decision-support feature that is reviewed by the Technician.

## AI Output Validation

AI output is treated as untrusted external input.

The backend must validate the response against a defined C# validation schema before the response is used by the application.

If:

- the AI service is unavailable,
- the AI response is malformed,
- the AI response is missing required fields, or
- the AI response does not follow the expected structure,

the incident must still be able to be saved.

This ensures that the AI functionality does not become a single point of failure.

# 6. Engineering Setup

## Structure & Tools

**Backend:** ASP.NET Core, Entity Framework Core, ASP.NET Identity, JWT.

**Frontend:** React, TypeScript, Vite, Tailwind, React Router.

**Data:** C# (CSV ingestion, embedded in backend), SQL Server.

**Environment:** Secrets are handled through .NET User Secrets locally (and environment variables/configuration providers if deployed). No API keys or other secrets shall be hardcoded in source code.

# 7. Quality & Security

## Testing Strategy

Testing is performed continuously using:

- unit tests for validation and business logic
- integration tests for authentication, database interactions, RBAC, and AI fallback
- manual acceptance scenarios for MVP and final validation points

Critical tests shall cover:

- authentication
- RBAC denial
- incident creation
- AI success
- AI failure fallback
- AI malformed-response/schema-validation fallback
- pipeline validation
- duplicate handling
- data-quality rules

## Reproducibility

The system must be able to start from a documented clean setup.

This means that:

- the database connection can be established
- migrations can be run
- the backend can be started
- the frontend can be started
- seed/demo data can be loaded according to the documentation

# 8. Deployment & Operations

The primary environment is a local development environment.

Deployment is optional and outside the core scope.

If deployment is attempted, it must take place after the core application is stable and must not be prioritized over core functionality, testing, or presentation readiness.

Docker Compose is a separate **P2 bonus goal** and must not affect P0/P1 stabilization.

# 9. Risk & Compliance

| Risk | Consequence | Mitigation |
|---|---|---|
| AI API unavailable | AI functionality unavailable | Graceful fallback; incident creation continues |
| AI returns invalid/malformed data | Incorrect AI suggestions | C# validation schema check + fallback |
| Authentication/RBAC becomes unstable | Security risk | P0, testing, and early checkpoint |
| Pipeline validation is incorrect | Incorrect data | Explicit validation + testing |
| Full-stack integration takes too long | Scope is missed | Vertical slice / MVP focus |
| Too much stretch work | Core becomes unstable | Priority gates and strict checkpoints |
| Database/migration setup only works locally | Poor reproducibility | Clean setup/restart test and documented migrations |
| Dashboard KPI definitions are unclear | Misleading results | Explicit KPI definitions and SQL-based aggregation |

# 10. Execution Planning

The detailed Day 1–18 plan, priority gates, Day 8–11 risk block, Day 18 freeze rule, and Presentation Day 19 are defined in the **Student Pre-Project Planning Template**.

The Baseline describes the product requirements and validation levels; the Planning document describes the detailed execution plan.

## Milestones & Checkpoints

| Milestone / Checkpoint | Day | Description |
|---|---:|---|
| Foundation / POC 1 | 1–3 | Technical foundation, JWT, RBAC, and database foundation |
| Frontend foundation & Machines | 4–5 | Machine user interface |
| AI technical feasibility | Early validation | Verification of structured AI responses and schema validation |
| Checkpoint 1 — Foundation Validation | 7 | Foundation works without regressions and can be started from a clean documented setup |
| Incidents & Maintenance | 8–11 | Incident reporting, maintenance workflow, and AI layer |
| Pipeline & Telemetry Validation | 12–13 | Data engineering and data quality |
| Checkpoint 2 — MVP Validation | 14 | Validation of complete Operational & Data MVP workflows |
| Stabilization & Dashboard | 15–16 | Stabilization and dashboard/KPI views |
| Test Completion & Regression | 17 | Final tests and regression testing |
| Checkpoint 3 / Freeze — Final Validation | 18 | Stable, documented, and presentation-ready |
| Presentation Day | 19 | Demonstration |

# 11. Documentation & Knowledge

The project is delivered with:

- a complete README with instructions for local execution, environment variables, and migrations
- documentation of system architecture
- API contracts
- validation rules
- AI schema and fallback behavior
- pipeline rules
- Validation Results for POC, Prototype, MVP, and Final Validation
- a clear description of Known Limitations / Production Gaps
- ERD
- testing instructions

AI-generated code must be explainable by the student. Examples of AI-generated or AI-assisted work that has been reviewed, modified, or corrected may be documented as Challenges & Solutions.

# 12. Feedback Loop

## Reprioritization and Scope Reduction Strategy

If the project falls behind schedule, strict reprioritization is applied according to the principle:

**P0 is protected at all costs.**

P1 is prioritized after P0 is stable.

P1.5 and P2 may only be implemented if P0/P1 are stable.

Under time pressure, the following are removed or reduced first:

1. CI
2. Docker Compose
3. Advanced observability
4. Pipeline scheduling
5. Extra UI polish
6. Additional dashboard metrics
7. Statistical anomaly detection
8. Convenience features

Core workflows, stability, testing, and presentation readiness are protected.

# Final Product Definition

Industrial Insight is a coherent full-stack prototype demonstrating how industrial data can move from collection and quality control to prioritized operational action.

The intended end-to-end architecture is:

**Machine telemetry**

↓

**Validation & data quality**

↓

**SQL Server**

↓

**ASP.NET Core**

↓

**React**

↓

**Incident management**

↓

**Priority scoring**

↓

**AI-assisted assessment**

↓

**Maintenance workflow**

↓

**Operational dashboard**

The system succeeds when the core operational and data workflows function end-to-end with real persistence, server-side authorization, critical validation, meaningful tests, and reproducible setup.

AI enhances the prioritization workflow but is not required for the core incident workflow to function.

Everything beyond this core system is an enhancement and must never be allowed to destabilize the core product.

**Project duration:** 18 development days (August 17 – September 3)

**Presentation:** September 4

**Project strategy:** Build the smallest complete system first. Stabilize it. Test it. Then expand it only when the existing system is safe.