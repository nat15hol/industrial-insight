---
name: Dataset Specification
description: Synthetic telemetry dataset structure and pipeline validation rules.
---

# Dataset Specification

## Overview

The data pipeline ingests **synthetic** machine telemetry from CSV files. No real production machines or live external telemetry feeds are used — this is explicitly out of scope (see [known-limitations.md](known-limitations.md)).

## Source Format

CSV files, one row per telemetry reading, ingested via **CsvHelper** in the C# backend.

### Expected Columns

| Column | Type | Notes |
| --- | --- | --- |
| `MachineId` | int | Must reference an existing `Machine`. |
| `Timestamp` | datetime | ISO 8601 recommended. |
| `Temperature` | float | |
| `Pressure` | float | |
| `Vibration` | float | |
| `Energy` | float | |

Exact column names/order should match whatever the ingestion routine implements; this table is the reference contract between the sample CSVs and the pipeline code.

## Sample Files

Located under `data/telemetry/`:

- `sample-valid.csv` — well-formed records that should be fully accepted.
- `sample-invalid.csv` — records with missing required fields, wrong data types, or out-of-range/invalid values, used to verify rejection handling.
- `sample-duplicates.csv` — records that duplicate existing entries (same `MachineId` + `Timestamp`, or as defined by the duplicate-detection rule), used to verify duplicate handling.

These files back both manual testing and the automated pipeline-validation test cases described in [testing.md](testing.md).

## Validation Rules

The pipeline validates each record against the following, in order:

1. **Schema validation** — all required columns present.
2. **Required-field validation** — no missing values in required fields.
3. **Data-type validation** — numeric fields parse as numbers, timestamps parse as valid dates.
4. **Timestamp validation** — timestamp is a plausible value (not malformed, not absurdly out of range).
5. **Measurement-range validation** — values fall within a plausible operating range (exact thresholds are an implementation detail, documented in code comments where defined).
6. **Machine-reference validation** — `MachineId` corresponds to an existing `Machine` record.
7. **Duplicate detection** — a record matching an already-ingested record (by machine + timestamp, or equivalent key) is flagged as a duplicate rather than silently re-inserted or silently rejected.

Each record is classified into exactly one outcome:

- **Accepted** — passes all checks, transformed and loaded into `TelemetryRecords`.
- **Rejected** — fails one or more validation checks; not loaded.
- **Duplicate** — otherwise valid, but matches an existing record; not re-loaded, but counted separately from rejects.

## Transformation

Accepted records are transformed (type coercion, mapping to the `TelemetryRecords` entity shape) before being persisted via EF Core to SQL Server.

## Pipeline Run Tracking

Every pipeline execution produces one `PipelineRun` record capturing:

- `RecordsProcessed`, `RecordsAccepted`, `RecordsRejected`, `Duplicates`
- `DataQualityPct` (accepted ÷ processed, or an equivalent defined calculation)
- `Status`
- `StartedAt`, `FinishedAt`

This is the data source for the dashboard's "Latest Pipeline Run" and "Pipeline Data-Quality Percentage" KPIs — see [architecture.md](architecture.md).

## Out of Scope

- Real-time/streaming telemetry ingestion.
- Cloud-based ingestion (e.g. upload to S3/Azure Blob before processing).
- Schema-drift detection against a live external data source.
- Scheduled/recurring pipeline execution (P1.5 stretch goal only, not core scope).

See [known-limitations.md](known-limitations.md) for the full list and rationale.
