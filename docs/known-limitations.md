---
name: Known Limitations
description: Known limitations and production gaps for Industrial Insight.
---

# Known Limitations / Production Gaps

Industrial Insight is a student project and educational demonstration, not a production industrial control system. This document lists deliberate scope boundaries and, where applicable, anything left unfinished (but stable) at the time of the final presentation.

## Deliberate Scope Boundaries (Anti-Scope)

These were excluded from the outset and are not considered gaps to be "fixed" — see `project-product-specification.md`, Anti-Scope section:

- Cloud-based telemetry ingestion (e.g. upload to S3/Azure Blob before processing).
- Live external telemetry APIs — the pipeline uses synthetic CSV data only.
- Real-time telemetry streaming.
- Full production-grade deployment and infrastructure.
- Advanced machine-learning or predictive-maintenance models — the Priority Score is deliberately rule-based and explainable, not a trained model.
- Full enterprise CMMS/EAM functionality — Industrial Insight is positioned as a prioritization layer that sits on top of existing systems, not a replacement for them.
- Mobile application.
- Advanced notification systems.
- Advanced BI/reporting functionality.
- Production-grade monitoring, alerting, and disaster recovery.

## Optional / Priority-Gated Features

The following are optional (P1/P1.5/P2) and may or may not be present in the final submission depending on how development time was allocated. Their absence does not indicate instability — see the Planning Template's distinction between "unfinished" and "unstable":

- **AI-assisted incident assessment** (P1) — optional decision-support layer. If not implemented, the core incident workflow is unaffected and fully functional.
- **Statistical anomaly detection** (P1.5) — simple ±2 standard deviation threshold rule for telemetry values.
- **Pipeline scheduling** (P1.5) — periodic/automated pipeline execution, as opposed to manual/on-demand.
- **Additional pipeline observability** (P1.5).
- **GitHub Actions CI** (P2, bonus only).
- **Docker Compose** (P2, bonus only) — must never displace P0/P1 stabilization regardless of available time.
- **Additional non-essential UI polish** (P2).

## Known Gaps at Time of Presentation

*(To be filled in during Week 3 stabilization — Day 15–18 — as the actual state of the system becomes clear. Per the Day 18 Freeze Rule, if the system is still unstable at the end of Day 18, the last stable state verified at Checkpoint 2 becomes the presentation baseline, and remaining issues are documented here rather than fixed under time pressure.)*

Example entry format:

- **[Feature/area]** — [what is missing or incomplete] — [why it was deprioritized] — [workaround/impact, if any].

## Security & Deployment

Detailed security and deployment documentation (`security.md`, `deployment.md`) are added once there is concrete implementation to document, consistent with the project's stability-first approach — no placeholder documentation is maintained for features not yet built. Until then:

- Deployment is optional and outside the core project scope; if attempted, it happens only after the core system is stable and never displaces core functionality, testing, or presentation readiness.
- Baseline security measures (password hashing, JWT auth, server-side RBAC, environment-based secrets, AI output validation) are described in [architecture.md](architecture.md) pending a dedicated `security.md`.

## Future Improvements (Explicitly Out of the 18-Day Scope)

Deliberately excluded because they would introduce new external dependencies (cloud accounts, credentials, new orchestration systems) late in the project, which conflicts with the project's stability-gate principle:

- A cloud element in the data pipeline (e.g. S3/Azure Blob staging), or a separate Python/Airflow scheduling service outside the core C#/.NET stack.
- Validation against a real external dataset or live API, to test robustness against real-world data messiness (schema drift, unexpected nulls, encoding issues).
- Integration with a more advanced local AI model, if future hardware/time permits.
- Integration with an external AI provider, if a suitable provider and acceptable dependency profile becomes available.

All of the above are natural extensions of the existing architecture and do not require redesigning the core concept:

**Collect → Validate → Transform → Store → Expose → Visualize → Act**
