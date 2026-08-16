---
name: Sequence Diagrams
description: Sequence diagrams for the two most critical/non-trivial flows in Industrial Insight — login/authentication and the optional AI-assisted incident assessment.
---

# Sequence Diagrams

These cover the flows worth visualizing separately from `api-contract.md`: authentication (used by every protected request) and the AI-assisted assessment flow (the one flow in the system with a required fallback path). Simple CRUD flows (e.g. `GET /api/machines`) are not included — the request/response shapes in `api-contract.md` are sufficient on their own.

## Login

Reflects `POST /api/auth/login` (see `api-contract.md`) and the JWT strategy decided in `decisions/0004-jwt-auth-strategy.md` (60-minute token, no refresh token, role embedded in claims).

```mermaid
sequenceDiagram
    actor U as User
    participant F as Frontend
    participant A as Backend API (Auth)
    participant D as Database

    U->>F: Enter email + password
    F->>A: POST /api/auth/login
    A->>D: Look up user by email
    D-->>A: User record (incl. PasswordHash, RoleId)
    alt credentials valid
        A->>A: Verify password hash
        A->>A: Issue JWT (sub, role, email, iat, exp)
        A-->>F: 200 OK { token, userId, name, role }
        F->>F: Store token in memory/context
        F-->>U: Redirect to dashboard
    else credentials invalid
        A-->>F: 401 Unauthorized
        F-->>U: Show "invalid email or password"
    end
```

**Subsequent protected requests** (not re-diagrammed per-endpoint — this pattern is constant across the API):

```mermaid
sequenceDiagram
    actor U as User
    participant F as Frontend
    participant A as Backend API

    F->>A: Request with Authorization: Bearer <token>
    alt token valid and not expired
        A->>A: Validate signature + expiry
        A->>A: Enforce role/ownership check (server-side RBAC)
        A-->>F: 200 OK (or relevant success response)
    else token missing or invalid
        A-->>F: 401 Unauthorized
        F->>F: Clear auth state
        F-->>U: Redirect to login
    else token valid but role insufficient
        A-->>F: 403 Forbidden
        F-->>U: Show "not authorized"
    end
```

## AI-Assisted Incident Assessment (Optional, P1)

Reflects `POST /api/incidents/{id}/ai-suggestion` in `api-contract.md`. This flow is diagrammed specifically because it has a mandatory fallback path — per `decisions/0002-ai-feature-optional-and-pluggable.md`, the AI feature must never block incident creation, so the "AI unavailable" branch below is not an edge case, it's a first-class outcome.

```mermaid
sequenceDiagram
    actor U as Technician
    participant F as Frontend
    participant A as Backend API
    participant AI as AI Service (pluggable)
    participant D as Database

    U->>F: Write incident description, request AI suggestion
    F->>A: POST /api/incidents/{id}/ai-suggestion
    A->>AI: Send description for assessment

    alt AI responds and passes schema validation
        AI-->>A: category, priority, recommendedAction, rationale
        A->>A: Validate response against C# schema
        A-->>F: 200 OK { category, priority, recommendedAction, rationale }
        F-->>U: Show suggestion (still editable before save)
    else AI unavailable or response fails validation
        AI-->>A: Timeout / error / malformed response
        A-->>F: 502 Bad Gateway  or  200 OK with null body
        F-->>U: "No suggestion available" — manual fields shown, unblocked
    end

    U->>F: Confirm (AI-assisted or fully manual) category/priority
    F->>A: POST /api/incidents (save)
    A->>D: Persist incident (AiSuggestion populated only if one was accepted)
    D-->>A: Saved
    A-->>F: 201 Created
    F-->>U: Incident saved
```

## Notes

- Both diagrams describe **decided** behavior (from `api-contract.md` and existing ADRs), not new decisions — nothing here should require implementation choices beyond what's already documented.
- The login diagram's "no refresh token" branch is intentionally simple, per ADR 0004: an expired token always results in a hard redirect to login, never a silent retry.
- The AI flow's fallback branch is the more important half of the diagram — it's what makes ADR 0002 ("AI is never a single point of failure") concrete and testable.
