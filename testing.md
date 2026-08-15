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

```bash
cd backend
dotnet test
```

Run a specific test project if the solution contains more than one:

```bash
dotnet test IndustrialInsight.Tests
```

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
