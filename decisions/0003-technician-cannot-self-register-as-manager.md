# 0003: Self-registration always creates a Technician; Manager accounts are seeded/controlled

**Date:** 2026-08-17
**Status:** Decided

## Context

The system has two roles: `Technician` and `Manager`. Manager permissions are meaningfully more powerful — prioritizing incidents, assigning maintenance tasks, viewing dashboards and pipeline results. The registration flow needed a decision on whether a new user could choose their own role at signup.

## Decision

`POST /api/auth/register` always creates a new account with the `Technician` role. There is no self-service path to register directly as `Manager`. Manager accounts are created via seed data or another controlled administrative mechanism (e.g. a seeded demo account, or promotion by an existing Manager if that capability is ever added — not in current scope).

## Why

- **Realistic RBAC modeling.** In a real maintenance organization, elevated permissions (assigning work, viewing organization-wide KPIs) are not something a new user grants themselves by picking an option on a signup form. Defaulting to the least-privileged role is the correct default in general, independent of this specific domain.
- **Security surface reduction.** An open self-registration path to an elevated role is an obvious and easy-to-miss vulnerability class. Removing the possibility entirely is simpler and more robust than trying to gate it correctly (e.g. with an invite code or admin approval flow), which would add scope not otherwise needed for this project.
- **Matches the demo dataset.** The required seed/demo dataset already specifies exactly one Manager and two Technicians — this decision is what makes that seed data the *only* way a Manager account exists, keeping the system's actual behavior consistent with what's demonstrated.
- **Testable RBAC boundary.** This decision creates a clean, explicit test case: "a self-registered user can never reach Manager-only functionality," which is one of the critical RBAC-denial tests required in `testing.md`.

## Alternatives Considered

- **Role selector at registration** — rejected: trivially allows anyone to grant themselves Manager permissions, defeating the purpose of RBAC.
- **Invite-code or admin-approval-gated Manager registration** — considered but rejected as out of scope: adds a meaningful amount of additional workflow (invite generation, approval UI/endpoint) for a permission model this project does not need to demonstrate. Noted as a plausible Future Improvement if the project were extended.
- **Manual database edit to create a Manager (no seed mechanism at all)** — rejected: fails the "clean, documented, reproducible setup" requirement for Checkpoint 1 and Checkpoint 3; a Manager account must exist automatically from documented setup steps, not require manual intervention.

## Consequences

- `setup.md` must document how the seeded Manager account's credentials are obtained/reset for the demo, since there is no in-app way to create one.
- Any future "promote user to Manager" capability would be a new, deliberate feature addition — not a re-interpretation of existing registration behavior — and should get its own ADR if implemented.
