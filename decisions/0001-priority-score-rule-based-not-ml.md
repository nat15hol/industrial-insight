# 0001: Priority Score is rule-based, not a machine-learning model

**Date:** 2026-08-17
**Status:** Decided

## Context

Industrial Insight needs a way to tell a Manager which machine deserves attention first. A natural instinct — and a plausible portfolio-flexing choice — would be to train a predictive model on incident/telemetry history and have it output a "risk score" or forecast likely failures.

## Decision

The Priority Score is calculated using an explicit, hand-defined rule:

```
score = 40 × min(open_incidents / 5, 1)
      + 40 × (has_critical_incident ? 1 : 0)
      + 20 × (recurring_issue ? 1 : 0)

→ bucketed: HIGH ≥ 70, MEDIUM 40–69, LOW < 40
```

It is explicitly framed throughout the project as **maintenance prioritization**, not **predictive maintenance**.

## Why

- **Explainability.** A Manager can look at any score and see exactly why a machine is flagged (open incident count, a critical incident, recurrence) — no black box. This matters both for real operational trust and for being able to defend the design in the final presentation.
- **No training data.** A predictive model needs a meaningful volume of historical failure data to be honest. This project uses synthetic telemetry over an 18-day window — training a real model on it would produce a score that looks sophisticated but is not actually validated against anything.
- **Time and risk.** Model training, evaluation, and tuning is a substantial time sink relative to the 18-day schedule, and a poorly-validated model is worse than an honest rule — it would overstate the system's actual capability.
- **Honesty over impressiveness.** The project deliberately avoids overstating the AI component. Calling a hand-tuned weighted sum "predictive maintenance" would be misleading; calling it "maintenance prioritization" is accurate.

## Alternatives Considered

- **Trained ML model (e.g. logistic regression or gradient boosting on incident/telemetry features)** — rejected: no real training data, disproportionate time cost, and would misrepresent the system's actual sophistication.
- **AI-generated score (via the optional AI-assisted assessment)** — rejected as the *sole* source of the score: would make the core prioritization feature dependent on an optional, potentially-unavailable AI service, violating the project's "AI must never be a single point of failure" principle. The AI's `category`/`priority` output can *feed into* the rule-based score's inputs (`recurring_issue`, `has_critical_incident`), but never replace the rule.
- **No score at all, just a sortable incident table** — rejected: less actionable, and misses the project's stated differentiator of turning scattered data into a prioritized, explainable view.

## Consequences

- The scoring logic lives in one place, is unit-testable in isolation, and has no external dependency.
- Score weights (40/40/20) are a starting assumption, not empirically tuned — this is documented as a known limitation, not hidden.
- If time permits, weights may be adjusted, but any change must be reflected consistently in `architecture.md`, `api-contract.md`, and the dashboard UI.
