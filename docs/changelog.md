# Changelog

All notable changes to Industrial Insight are documented in this file.

Format loosely follows [Keep a Changelog](https://keepachangelog.com/); dates use YYYY-MM-DD.

## [Unreleased]

### Added
- Initial documentation set: `architecture.md`, `database.md`, `dataset-specification.md`, `setup.md`, `testing.md`, `known-limitations.md`.
- Revised `api-contract.md`, `erd.md`, and `ui-wireframes.md` for consistency with `project-product-specification.md` and `student-pre-project-planning-template.md` (unified Technician/Manager roles, corrected data model fields, added missing endpoints and screens).
- ADRs: `0001-priority-score-rule-based-not-ml.md`, `0002-ai-feature-optional-and-pluggable.md`, `0003-technician-cannot-self-register-as-manager.md`.
- Backend Web API using ASP.NET Core and Entity Framework Core, with SQL Server connectivity and the initial domain model.
- User registration with BCrypt password hashing, JWT-based authentication, and role-based authorization for `Technician` and `Manager`.
- Full CRUD for the `Machine` entity, including Manager-only authorization for create, update, and delete.
- `Machine`–`Location` relationship via `LocationId`, with validation that referenced Locations exist.
- `Status` and `Runtime` fields on `Machine`, exposed through the API.
- Development seed data for Machines and Locations.
- Frontend connection to the authenticated Machine API, displaying machine information.

### Fixed
- Entity primary-key naming corrected to follow EF Core conventions.
- SQL Server multiple-cascade-path conflict resolved in entity relationships.
- Seeded roles and users aligned with the project specification.

## [0.1.0] - 2026-08-17

### Added
- Project foundation: repository structure, initial planning documents (`project-product-specification.md`, `student-pre-project-planning-template.md`, `README.md`).
- Initial (unrevised) drafts of `api-contract.md`, `erd.md`, `ui-wireframes.md`.

---

## Notes on maintaining this file

- Update `[Unreleased]` as work happens; cut a dated section (e.g. `[0.2.0] - 2026-08-24`) at natural milestones — a good fit is each of the three checkpoints (Day 7, Day 14, Day 18).
- Entries should be short and grouped as **Added**, **Changed**, **Fixed**, or **Removed**.
- This file tracks project/documentation history, not commit-level detail — commit messages and PR history cover that.