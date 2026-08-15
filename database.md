---
name: Database
description: Database structure, relationships, and persistence for Industrial Insight.
---

# Database

## Overview

Industrial Insight uses **SQL Server** as its relational database, accessed exclusively through **Entity Framework Core**. This document describes the tables, relationships, and persistence conventions. For the visual entity-relationship diagram and field-level notes, see [erd.md](erd.md).

## Tables

| Table | Purpose |
| --- | --- |
| `Roles` | Defines the two system roles (`Technician`, `Manager`). |
| `Users` | Account credentials (hashed password), name, email, and role. |
| `Locations` | Physical location associated with one or more machines. |
| `Machines` | Core asset record: name, status, runtime, location. |
| `TelemetryRecords` | Validated machine telemetry loaded by the data pipeline. |
| `Incidents` | Technician-reported problems tied to a machine, including optional AI suggestion fields. |
| `MaintenanceTasks` | Work items created from an incident, assigned to a Technician, tracked to completion. |
| `PipelineRuns` | Log of each pipeline execution: processed/accepted/rejected counts, duplicates, data-quality percentage, status. |

## Relationships

- `Roles` 1—* `Users`
- `Locations` 1—* `Machines`
- `Machines` 1—* `TelemetryRecords`
- `Machines` 1—* `Incidents`
- `Users` 1—* `Incidents` (as reporter)
- `Incidents` 1—0..* `MaintenanceTasks`
- `Users` 1—* `MaintenanceTasks` (as assignee)
- `PipelineRuns` is an independent log table — not linked via foreign key to `Machines`, since a single pipeline run processes telemetry across potentially many machines.

`MaintenanceTasks` intentionally does not duplicate `MachineId`. The associated machine is reachable via `MaintenanceTask → Incident → Machine`. See `decisions/` if this normalization becomes a query-performance concern later in development.

## Migrations

- Managed through EF Core Migrations (`dotnet ef migrations add`, `dotnet ef database update`).
- The initial migration establishes the empty core tables (`Roles`, `Users`, `Machines`, `Locations`).
- Subsequent migrations add `Incidents`, `MaintenanceTasks`, `TelemetryRecords`, and `PipelineRuns` as those features are implemented.
- Migrations must run cleanly from a documented clean setup — this is a requirement of Checkpoint 1 and Checkpoint 3. See [setup.md](setup.md).

## Seed / Demo Data

A minimal seed is established early (Day 1–2) for `Roles` and initial `Users`, since self-registration only ever creates `Technician` accounts — at least one `Manager` account must be seeded or otherwise controlled.

The full demo dataset required for the final presentation includes, at minimum:

- 1 Manager
- 2 Technicians
- ≥ 5 machines
- ≥ 5 incidents across different statuses
- ≥ 3 maintenance tasks, at least 1 completed
- Telemetry data
- At least 1 successful `PipelineRun`
- At least 1 partially failed `PipelineRun` demonstrating data-quality handling

If the AI feature is implemented, the demo data should include at least one incident that produces a meaningful AI suggestion.

## Data Quality & Validation

Validation of incoming telemetry (required fields, data types, missing/invalid values, duplicate detection) happens in the pipeline layer before data is written to `TelemetryRecords` — see [dataset-specification.md](dataset-specification.md). The database itself enforces referential integrity (foreign keys) and required-field constraints at the schema level as a second line of defense, not as the primary validation mechanism.

## Out of Scope

- No sharding, replication, or production-scale infrastructure — a single local SQL Server instance is sufficient for this project's scope.
- No historical data archival/retention policy beyond what is needed for the demo dataset.

See [known-limitations.md](known-limitations.md) for the complete list.
