# Priority Score — v1 Design Decisions (#69)

## Context

Issue #69 defines the Priority Score formula as:

```
score = 40 * min(open / 5, 1) + 40 * has_critical + 20 * recurring_issue
```

bucketed into `HIGH` / `MEDIUM` / `LOW`.

The formula itself is given in the issue, but three of its components are not
explicitly defined anywhere in the project specification
(`Project_Product_Specification.md`, `dataset-specification.md`):

- what counts as `has_critical`
- what counts as `recurring_issue`
- what score thresholds map to `HIGH` / `MEDIUM` / `LOW`

The spec's "Problematic Machines" section (`Project_Product_Specification.md`,
"Problematic Machines / Priority Score") defines a related but separate rule
(≥2 open incidents OR ≥1 critical incident, within a 7-day reporting period)
for a different feature (#68). It does not define the Priority Score formula's
internal components, and its 7-day window is not carried over here, since
#69's formula does not reference a time period.

Rather than leave these undefined or guess silently while implementing, the
following v1 decisions were made explicitly and are documented here so the
reasoning is traceable — the same approach used for the incident data
contract earlier in the project.

## Decisions

### `open`

```
open = number of currently open incidents for the machine (no time window)
```

The formula references `open` without qualification, so no time-based
restriction is introduced. This may be revisited if testing shows a single
old, unresolved incident undesirably keeps a machine's score elevated
indefinitely — but that should be decided based on observed behavior, not
assumed in advance.

### `has_critical`

```
has_critical = true if the machine has at least 1 open incident with
               Priority = "High"
```

The spec's "Problematic Machines" section uses the term "critical" to
describe machine severity, but the data model only has three priority
levels (`High` / `Medium` / `Low`) — there is no separate `Critical` value.
`High` is treated as the equivalent of "critical" for this purpose.

### `recurring_issue`

```
recurring_issue = true if the machine has at least 2 open incidents
                   within the same Category
```

Chosen over keyword/description matching (e.g. matching "oil leak" across
multiple incident descriptions) because it is deterministic, testable, and
uses an existing structured field rather than free-text heuristics similar
to the AI mock's keyword matching. This is a simple v1 rule, not a claim
that it captures every real-world notion of a "recurring" problem.

### Bucket thresholds

```
HIGH   → score >= 70
MEDIUM → score 40–69
LOW    → score < 40
```

Maximum possible score is 100 (40 + 40 + 20). With these thresholds, a
HIGH score always requires `has_critical = true`, since the `open` and
`recurring_issue` components alone can contribute at most 60 points
(40 + 20). The remaining points needed to reach 70 can come from the
open-incident contribution alone or from a combination of open
incidents and `recurring_issue`. 40 as the MEDIUM floor captures
machines with moderate open-incident load even without `has_critical`
or `recurring_issue`.

## Status

These are v1, rule-based decisions — deliberately simple and explainable,
consistent with the project's stated approach to Priority Score (rule-based,
not a predictive model). They are open to revision if testing against real
demo data reveals unintended behavior.
