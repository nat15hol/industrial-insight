---
name: Testing
description: Testing strategy and instructions for Industrial Insight.
---

# Testing

## Strategy

Testing is performed continuously throughout development using:

- **Unit tests** for validation logic and business rules (e.g. Priority Score calculation, pipeline validation rules).
- **Integration tests** for authentication, database interactions, RBAC enforcement, and AI fallback behavior.
- **Manual acceptance scenarios** at the MVP and Final Validation checkpoints (see the Planning Template's checkpoint definitions).

AI assistance may be used to generate initial test-case drafts and assist with debugging, but test correctness, relevance, and coverage of critical flows are reviewed and owned by the student.

## Critical Test Areas

Approximately 6–10 meaningful xUnit tests cover, at minimum:

- Authentication (register, login, JWT issuance)
- RBAC denial (a Technician cannot access Manager-only endpoints or another Technician's maintenance task)
- Incident creation
- Pipeline validation (required fields, data types, invalid values)
- Duplicate handling / data-quality rules
- Database relationships (e.g. cascading/foreign key behavior where relevant)
- Clean setup/restart verification (manual, not necessarily automated — see [setup.md](setup.md))

If the optional AI-assisted incident assessment is implemented, additional tests cover:

- **AI success scenario** — a valid incident description produces a structured suggestion that passes schema validation.
- **AI failure/fallback scenario** — the AI service is unavailable; the incident can still be saved.
- **AI malformed-response/schema-validation scenario** — the AI returns invalid/incomplete structured data; it is rejected by validation and the incident can still be saved.

AI-specific tests are **not required** if the optional AI feature is not implemented — their absence does not block MVP or Final success criteria.

## Running the Tests

The current solution structure (`server/server.csproj`) is a single project rather than a multi-project split — a deliberate, pragmatic choice for a solo 18-day project (see `decisions/` if this warrants its own ADR later). Testing can proceed either way; pick whichever fits how far the project has grown:

### Option A — Tests inside `server/` (simplest, good starting point)

Add a `Tests/` folder directly inside `server/` and reference xUnit via the same `server.csproj`, or add xUnit as a package reference to the existing project. Run with:

```bash
cd server
dotnet test
```

This is the lowest-friction option and is a reasonable place to start — no new project to wire up, no project references to maintain.

### Option B — Separate test project (`server.Tests/`)

If the test suite grows large enough that mixing test code into the main project feels awkward, split it out:

```bash
cd server
dotnet new xunit -o ../server.Tests
cd ../server.Tests
dotnet add reference ../server/server.csproj
```

Then run with:

```bash
cd server.Tests
dotnet test
```

Update the solution file (`industrial-insight.slnx`) to include the new project either way, so `dotnet test` from the repository root (if run that way) picks it up.

**Recommendation for this project's scope:** start with Option A. With ~6–10 critical tests, a separate test project is unlikely to pay for its own setup cost within the 18-day timeline. Revisit only if the test suite genuinely becomes unwieldy inside `server/`.

## Acceptance Criteria Reference

The following Given/When/Then scenarios (defined in `project-product-specification.md`) are the basis for the critical test areas above:

- **Incident creation** — a logged-in Technician submitting a valid incident results in it being saved and associated with the selected machine.
- **RBAC** — a Technician attempting a Manager-only operation, or modifying another user's maintenance task, is denied server-side.
- **AI success** — a valid incident description, with the AI service available, produces a schema-valid structured suggestion.
- **AI fallback** — an unavailable or malformed AI response never blocks incident creation.
- **Maintenance task completion** — a Technician completing an assigned task allows the related incident's `ResolvedAt` to be recorded.

## Regression Testing

Before each checkpoint (Day 7, Day 14, Day 18), the full critical test suite is run and the system is manually walked through its core operational and data workflows to confirm no regression. A previously working integration flow silently breaking is treated as **instability** per the Planning Template's definition, and takes priority over starting new work.

## Out of Scope

- End-to-end browser automation (e.g. Playwright/Cypress) is not required for this project's scope.
- Load/performance testing is not required.
- Frontend unit tests are not required in the current scope, since only backend (xUnit) testing is defined as a critical requirement.

See [known-limitations.md](known-limitations.md) for the complete list.
