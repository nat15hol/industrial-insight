# Changelog

All notable changes to Industrial Insight are documented in this file.

Format loosely follows [Keep a Changelog](https://keepachangelog.com/); dates use YYYY-MM-DD.

## [Unreleased]

### Added
- Initial documentation set: `architecture.md`, `database.md`, `dataset-specification.md`, `setup.md`, `testing.md`, `known-limitations.md`.
- Revised `api-contract.md`, `erd.md`, and `ui-wireframes.md` for consistency with `project-product-specification.md` and `student-pre-project-planning-template.md` (unified Technician/Manager roles, corrected data model fields, added missing endpoints and screens).
- ADRs: `0001-priority-score-rule-based-not-ml.md`, `0002-ai-feature-optional-and-pluggable.md`, `0003-technician-cannot-self-register-as-manager.md`.

## [0.1.0] - 2026-08-17

### Added
- Project foundation: repository structure, initial planning documents (`project-product-specification.md`, `student-pre-project-planning-template.md`, `README.md`).
- Initial (unrevised) drafts of `api-contract.md`, `erd.md`, `ui-wireframes.md`.

---

## Notes on maintaining this file

- Update `[Unreleased]` as work happens; cut a dated section (e.g. `[0.2.0] - 2026-08-24`) at natural milestones — a good fit is each of the three checkpoints (Day 7, Day 14, Day 18).
- Entries should be short and grouped as **Added**, **Changed**, **Fixed**, or **Removed**.
- This file tracks project/documentation history, not commit-level detail — commit messages and PR history cover that.
