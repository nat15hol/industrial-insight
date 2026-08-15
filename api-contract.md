---
name: API_Contract
description: REST API endpoint specifications for Industrial Insight.
---

# API Contract

## Overview

This document defines the REST API endpoints for Industrial Insight. All endpoints are served by the ASP.NET Core Web API backend and consumed by the React frontend.

**Base path:** `/api`

**Authentication:** JWT Bearer token, obtained via `POST /api/auth/login` or `POST /api/auth/register`. Include as `Authorization: Bearer <token>` on all protected endpoints.

**Roles:** `Technician`, `Manager`. New self-registered accounts default to `Technician`. `Manager` accounts are seeded or otherwise controlled — there is no self-service path to the `Manager` role.

All error responses follow a consistent shape:

```json
{
  "error": "string",
  "details": "string | null"
}
```

---

## Auth

| Method | Path | Purpose | Auth Required | Role |
| :--- | :--- | :--- | :--- | :--- |
| POST | `/api/auth/register` | Register a new account (created as Technician) | No | — |
| POST | `/api/auth/login` | Authenticate and return a JWT | No | — |

### POST /api/auth/register

**Request**

```json
{
  "name": "string",
  "email": "string",
  "password": "string"
}
```

**Response `201 Created`**

```json
{
  "userId": "int",
  "name": "string",
  "email": "string",
  "role": "Technician"
}
```

### POST /api/auth/login

**Request**

```json
{
  "email": "string",
  "password": "string"
}
```

**Response `200 OK`**

```json
{
  "token": "string",
  "userId": "int",
  "name": "string",
  "role": "Technician | Manager"
}
```

**Response `401 Unauthorized`** on invalid credentials.

---

## Machines

| Method | Path | Purpose | Auth Required | Role |
| :--- | :--- | :--- | :--- | :--- |
| GET | `/api/machines` | List all machines | Yes | Technician, Manager |
| GET | `/api/machines/{id}` | Get details of a specific machine | Yes | Technician, Manager |
| POST | `/api/machines` | Create a machine | Yes | Manager |
| PUT | `/api/machines/{id}` | Update a machine | Yes | Manager |

### GET /api/machines

**Response `200 OK`**

```json
[
  {
    "machineId": "int",
    "name": "string",
    "status": "string",
    "runtime": "number",
    "locationId": "int",
    "locationName": "string"
  }
]
```

### GET /api/machines/{id}

**Response `200 OK`** — same shape as a single item above, plus optionally recent incident/telemetry summaries.

**Response `404 Not Found`** if the machine does not exist.

---

## Incidents

| Method | Path | Purpose | Auth Required | Role |
| :--- | :--- | :--- | :--- | :--- |
| POST | `/api/incidents` | Report a new incident | Yes | Technician |
| GET | `/api/incidents` | List incidents | Yes | Technician, Manager |
| GET | `/api/incidents/{id}` | Get incident detail | Yes | Technician, Manager |
| PUT | `/api/incidents/{id}` | Update incident status/priority | Yes | Manager |

A Technician sees incidents relevant to them (e.g. their own reports and assigned machines); a Manager sees all incidents. Exact filtering rules are an implementation detail but must not leak cross-role data.

### POST /api/incidents

**Request**

```json
{
  "machineId": "int",
  "description": "string"
}
```

If the optional AI-assisted assessment is enabled and available, the backend may attempt to enrich the incident with a suggestion before saving. This must never block saving the incident — see AI section below.

**Response `201 Created`**

```json
{
  "incidentId": "int",
  "machineId": "int",
  "reportedByUserId": "int",
  "description": "string",
  "status": "Open",
  "priority": "string | null",
  "category": "string | null",
  "aiSuggestion": {
    "category": "string",
    "priority": "string",
    "recommendedAction": "string"
  } | null,
  "createdAt": "datetime",
  "resolvedAt": "datetime | null"
}
```

### PUT /api/incidents/{id}

**Request**

```json
{
  "status": "Open | InProgress | Resolved",
  "priority": "string",
  "category": "string"
}
```

Only a Manager may prioritize or change incident status. Setting `status` to `Resolved` may set `resolvedAt` server-side.

**Response `200 OK`** with the updated incident.

**Response `403 Forbidden`** if called by a Technician.

---

## Maintenance Tasks

| Method | Path | Purpose | Auth Required | Role |
| :--- | :--- | :--- | :--- | :--- |
| POST | `/api/maintenance` | Create and assign a task from an incident | Yes | Manager |
| GET | `/api/maintenance` | List maintenance tasks | Yes | Technician (own only), Manager (all) |
| GET | `/api/maintenance/{id}` | Get task detail | Yes | Technician (if assigned), Manager |
| PUT | `/api/maintenance/{id}` | Update task status | Yes | Technician (if assigned), Manager |

A Technician may only view and update tasks assigned to them. Attempting to access or modify another Technician's task returns `403 Forbidden`.

### POST /api/maintenance

**Request**

```json
{
  "incidentId": "int",
  "assignedToUserId": "int",
  "description": "string"
}
```

**Response `201 Created`**

```json
{
  "taskId": "int",
  "incidentId": "int",
  "assignedToUserId": "int",
  "description": "string",
  "status": "ToDo",
  "createdAt": "datetime",
  "completedAt": "datetime | null"
}
```

### PUT /api/maintenance/{id}

**Request**

```json
{
  "status": "ToDo | Doing | Done"
}
```

Completing a task (`status: "Done"`) may trigger the related incident's `resolvedAt` to be set.

**Response `200 OK`** with the updated task.

---

## Telemetry & Pipeline

| Method | Path | Purpose | Auth Required | Role |
| :--- | :--- | :--- | :--- | :--- |
| GET | `/api/telemetry/{machineId}` | Get recent telemetry records for a machine | Yes | Technician, Manager |
| GET | `/api/pipeline/runs` | List recent pipeline runs | Yes | Manager |
| GET | `/api/pipeline/runs/{id}` | Get pipeline run detail | Yes | Manager |

Pipeline ingestion itself (CSV → validation → SQL Server) runs as a backend process/routine, not as a user-facing POST endpoint, unless a manual trigger is explicitly implemented.

### GET /api/telemetry/{machineId}

**Response `200 OK`**

```json
[
  {
    "telemetryId": "int",
    "machineId": "int",
    "timestamp": "datetime",
    "temperature": "number",
    "pressure": "number",
    "vibration": "number",
    "energy": "number"
  }
]
```

### GET /api/pipeline/runs

**Response `200 OK`**

```json
[
  {
    "runId": "int",
    "startedAt": "datetime",
    "finishedAt": "datetime",
    "recordsProcessed": "int",
    "recordsAccepted": "int",
    "recordsRejected": "int",
    "duplicates": "int",
    "dataQualityPct": "number",
    "status": "string"
  }
]
```

---

## Dashboard

| Method | Path | Purpose | Auth Required | Role |
| :--- | :--- | :--- | :--- | :--- |
| GET | `/api/dashboard/summary` | Aggregated KPI values | Yes | Technician, Manager |
| GET | `/api/dashboard/priority-scores` | Ranked Priority Score list per machine | Yes | Technician, Manager |

### GET /api/dashboard/summary

**Response `200 OK`**

```json
{
  "totalMachines": "int",
  "openIncidents": "int",
  "averageResolutionTimeHours": "number",
  "latestPipelineRun": {
    "runId": "int",
    "status": "string",
    "finishedAt": "datetime"
  },
  "pipelineDataQualityPct": "number"
}
```

### GET /api/dashboard/priority-scores

**Response `200 OK`**

```json
[
  {
    "machineId": "int",
    "machineName": "string",
    "priorityScore": "int",
    "bucket": "HIGH | MEDIUM | LOW",
    "openIncidents": "int",
    "hasCriticalIncident": "boolean",
    "recurringIssue": "boolean",
    "recommendedAction": "string | null"
  }
]
```

`priorityScore` is calculated server-side using the rule defined in `docs` / the product specification:

```
score = 40 × min(open_incidents / 5, 1)
      + 40 × (has_critical_incident ? 1 : 0)
      + 20 × (recurring_issue ? 1 : 0)
```

This is a transparent, rule-based score — not output from a predictive model.

---

## AI-Assisted Incident Assessment (Optional, P1)

This is only relevant if the AI feature is implemented. If disabled or not implemented, incident creation and the rest of the API function unchanged.

| Method | Path | Purpose | Auth Required | Role |
| :--- | :--- | :--- | :--- | :--- |
| POST | `/api/incidents/{id}/ai-suggestion` | Request an AI suggestion for an existing incident description | Yes | Technician |

**Request:** none (uses the incident's existing description) or `{ "description": "string" }` if requesting before save.

**Response `200 OK`**

```json
{
  "category": "string",
  "priority": "string",
  "recommendedAction": "string",
  "rationale": "string"
}
```

**Response `502 Bad Gateway` or `200 OK` with `null` body** if the AI service is unavailable or returns a response that fails schema validation — the caller (frontend) must treat this as "no suggestion available" and allow the incident to be saved manually regardless.

The AI response is treated as untrusted external input and is validated against a C# validation schema server-side before ever being returned to the frontend or persisted.

---

## Status Codes Used Throughout

| Code | Meaning |
| :--- | :--- |
| 200 | Success |
| 201 | Resource created |
| 400 | Validation error (malformed request body) |
| 401 | Missing or invalid JWT |
| 403 | Authenticated but not authorized (RBAC denial) |
| 404 | Resource not found |
| 502 | Upstream dependency (e.g. AI provider) unavailable — never returned for core P0 endpoints |
