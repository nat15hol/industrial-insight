# 0002: AI-assisted incident assessment is optional and pluggable

**Date:** 2026-08-17
**Status:** Decided

## Context

Industrial Insight includes an AI-assisted incident assessment: given an incident description, it can suggest a category, priority, and recommended action. This is attractive to demonstrate AI integration skills, but it introduces an external dependency (a network call to an AI provider) into what would otherwise be a fully self-contained system.

The project has a hard 18-day timeline, is developed solo, and core operational functionality (P0) must never be put at risk by an optional enhancement.

## Decision

The AI-assisted incident assessment is:

1. **Optional (P1)** — implemented only after the core incident/maintenance workflow (P0) is stable, and may be descoped entirely without affecting MVP or Final success criteria.
2. **Pluggable** — implemented behind a single service abstraction so the underlying provider (local mock, local open-source model, or external provider such as OpenAI or Anthropic) can be swapped without touching the incident workflow.
3. **Never a single point of failure** — if the AI service is unavailable, disabled, not implemented, or returns a malformed/invalid response, the incident can still be created and saved. The operational P0 workflow must function identically whether or not AI is available.
4. **Treated as untrusted input** — every AI response is validated against a defined C# schema before being persisted or shown to the user. A response that fails validation follows the exact same fallback path as an unavailable service.

## Why

- **Risk isolation.** An external AI call can fail, time out, or return garbage. Making it optional and schema-gated means that failure mode cannot cascade into breaking incident reporting — the single most important P0 workflow in the system.
- **No paid dependency required.** A local mock implementation is fully acceptable for demonstration purposes, so the project's core success does not depend on having (or paying for) an external AI subscription.
- **Development sequencing matches risk.** Per the Day 8–11 "risk block," AI work is explicitly the first thing dropped if the core incident/maintenance workflow is under schedule pressure — this decision operationalizes that priority in the architecture itself, not just the schedule.
- **Honesty about role.** The AI is decision support, not an autonomous decision-maker — the Technician always reviews and can accept or edit the suggestion before saving.

## Alternatives Considered

- **AI as a required part of incident creation** — rejected: makes P0 functionality dependent on an external service, unacceptable given the project's stability-first strategy.
- **Building a custom-trained model instead of calling an API** — rejected: out of scope, disproportionate effort for an application-level decision-support feature; see `0001-priority-score-rule-based-not-ml.md` for the related reasoning on the Priority Score itself.
- **Skipping AI entirely** — considered and remains a valid outcome if time runs out; the architecture supports this without any structural change, since AI was designed as an add-on layer from the start.

## Consequences

- Slightly more upfront design work (the service abstraction, the schema validation layer, the fallback path) than a "just call the API directly" approach — accepted as worthwhile given the risk it removes.
- Tests must explicitly cover the AI success, failure, and malformed-response paths, not just the happy path (see `testing.md`).
- If the AI feature is not implemented at all, this ADR still stands as the record of *why* the feature was designed to be safely omittable.
